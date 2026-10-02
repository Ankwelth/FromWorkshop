//RelayNet
// RELAY-NET V4.5 - base: script original del usuario.
// Extensiones: fragmentacion logica, FNV-1a global/por-parte, reconstruccion,
// recuperacion RX->RX, IDs fisicos reciclables y entrega de comandos a RN_COMMAND_PB.
// El motor Tick/SendSignal, STEP y el cierre fisico DT->ACK conservan el comportamiento original.

const int MY_IP = 1;
const int MY_MAC = 1;
const int IP_BROADCAST = 255;
const int TTL_MAX = 15;
const double STEP = 0.02;
const int FRAGMENT_CHARS = 900;
const int MAX_RETRIES = 3;
const double COMPLETED_KEEP = 30.0;
const string SECONDARY_PB_NAME = "RN_COMMAND_PB";

const int CONFIRM = 1;
const int DST_IP = 2;
const int SRC_IP = 3;
const int DST_MAC = 4;
const int SRC_MAC = 5;
const int ID = 6;
const int TTL = 7;
const int HOP_CNT = 8;
const int HOP_LST = 9;
const int HOP_SP = 10;
const int DT = 11;
const int ACK = 12;

const string FRAG_MAGIC = "@RN4F|";
const string RESEND_MAGIC = "@RN4R|";

class Sig
{
    public int Kind;
    public int Value;
    public Sig(int kind, int value) { Kind = kind; Value = value; }
}

class Packet
{
    public int DstIP;
    public int SrcIP;
    public int DstMAC;
    public int SrcMAC;
    public int Id;
    public int Ttl;
    public List<int> Hops = new List<int>();
    public List<byte> Data = new List<byte>();
}

class TxItem
{
    public List<Sig> Signals;
    public Packet Packet;
    public int Pos;
    public int Retries;
    public double Timer;
    public double Backoff;
    public bool WaitingAck;
    public bool IsAck;
    public string Frame;
    public string LogicalHash = "";
    public int Part;
    public int Total;
    public bool AdvancesLogical;

    public TxItem(List<Sig> signals, Packet packet, bool isAck, string frame)
    {
        Signals = signals;
        Packet = packet;
        IsAck = isAck;
        Frame = frame;
    }
}

class LogicalSend
{
    public int DstIP;
    public int DstMAC;
    public string Text = "";
    public string Hash = "";
    public int Total;
    public int NextPart = 1;
}

class SentMessage
{
    public int DstIP;
    public int DstMAC;
    public string Text = "";
    public string Hash = "";
    public int Total;
    public double LastUse;
}

class Assembly
{
    public int SrcIP;
    public int SrcMAC;
    public string Hash = "";
    public int Total;
    public Dictionary<int, string> Parts = new Dictionary<int, string>();
    public HashSet<int> Requested = new HashSet<int>();
    public double LastActivity;
    public double LastRequest;
}

class Link
{
    public string Prefix;
    public IMyFunctionalBlock[] Type = new IMyFunctionalBlock[13];
    public IMyFunctionalBlock[] Data = new IMyFunctionalBlock[4];

    public Queue<TxItem> Queue = new Queue<TxItem>();
    public TxItem Current;
    public Queue<LogicalSend> LogicalQueue = new Queue<LogicalSend>();
    public LogicalSend LogicalCurrent;
    public HashSet<int> UsedIds = new HashSet<int>();

    public int RxField = -1;
    public int RxValue;
    public bool RxHasPair;
    public bool RxDrop;
    public bool RxInData;
    public List<bool> DataBits = new List<bool>();
    public StringBuilder Message = new StringBuilder();

    public int DstIP;
    public int SrcIP;
    public int DstMAC;
    public int SrcMAC;
    public int Id = -1;
    public int Ttl;
    public int HopCount;
    public List<int> Hops = new List<int>();
    public int DataCount;

    public string Trace = "";
    public string LastFrame = "";

    public Link(string prefix) { Prefix = prefix; }
}

Link tx = new Link("TX");
Link rx = new Link("RX");

IMyTextSurface output;
IMyTextSurface input;

int lastSrcIP = -1;
int lastSrcMAC = -1;
int lastRxChannel = -1;
int lastRxId = -1;
string lastCommand = "";
string lastReceived = "";
double clock = 0;
double lastResponseAt = -1000.0;
string lastResponseKey = "";
const double RESPONSE_LATCH = 0.50;

Dictionary<string, SentMessage> sentMessages = new Dictionary<string, SentMessage>();
Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
Dictionary<string, double> completed = new Dictionary<string, double>();
Random random = new Random();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

public void Main(string argument, UpdateType updateSource)
{
    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    if (dt <= 0) dt = STEP;
    clock += dt;

    Init();

    if (!string.IsNullOrWhiteSpace(argument))
    {
        string arg = argument.Trim();
        if (!arg.StartsWith("TX:") && !arg.StartsWith("RX:") &&
            !arg.StartsWith("txdata:") && !arg.StartsWith("rxdata:"))
            lastCommand = arg;
        ExecuteArgument(arg);
    }

    CheckAssemblies();
    CleanupCompleted();
    Tick(tx, dt);
    Tick(rx, dt);

    if (!string.IsNullOrWhiteSpace(argument) &&
        !argument.StartsWith("TX:") && !argument.StartsWith("RX:") &&
        !argument.StartsWith("txdata:") && !argument.StartsWith("rxdata:"))
        Status();
}

void ExecuteArgument(string a)
{
    if (a.StartsWith("sendTX:"))
    {
        SendArgument(tx, a.Substring(7));
        return;
    }

    if (a.StartsWith("sendRX:"))
    {
        SendArgument(rx, a.Substring(7));
        return;
    }

    if (a.Equals("response", StringComparison.OrdinalIgnoreCase))
    {
        if (lastSrcIP < 0 || lastSrcMAC < 0)
        {
            Echo("ERROR: response sin SRC");
            return;
        }

        string text = ReadInputText();
        if (text.Length == 0)
        {
            Echo("ERROR: SUPERFICIE DE ENTRADA VACIA O NO DISPONIBLE");
            return;
        }

        string responseKey = lastSrcIP + ":" + lastSrcMAC + ":" + HashText(text);
        if (responseKey == lastResponseKey && clock - lastResponseAt < RESPONSE_LATCH)
        {
            Echo("RESPONSE DUP IGNORADA");
            return;
        }
        lastResponseKey = responseKey;
        lastResponseAt = clock;

        QueueLogical(rx, lastSrcIP, lastSrcMAC, text);
        Echo("RESPONSE -> RX " + lastSrcIP + "/" + lastSrcMAC);
        return;
    }

    if (a.StartsWith("TX:"))
    {
        HandleRelayArgument(tx, a);
        return;
    }

    if (a.StartsWith("RX:"))
    {
        HandleRelayArgument(rx, a);
        return;
    }

    if (a == "txdata:00" || a == "txdata:01" ||
        a == "txdata:10" || a == "txdata:11")
    {
        HandleRelayArgument(tx, a);
        return;
    }

    if (a == "rxdata:00" || a == "rxdata:01" ||
        a == "rxdata:10" || a == "rxdata:11")
    {
        HandleRelayArgument(rx, a);
        return;
    }

    Echo("ARGUMENTO desconocido: " + a);
}

void SendArgument(Link link, string arg)
{
    string[] p = arg.Split(',');
    if (p.Length < 2)
    {
        Echo("ERROR: destino " + link.Prefix + " invalido");
        return;
    }

    int ip;
    int mac;
    if (!int.TryParse(p[0], out ip) || !int.TryParse(p[1], out mac))
    {
        Echo("ERROR: destino " + link.Prefix + " invalido");
        return;
    }

    if (input == null)
    {
        Echo("ERROR: SUPERFICIE INPUT [1] NO DISPONIBLE");
        return;
    }

    string text = ReadInputText();
    if (text.Length == 0)
    {
        Echo("ERROR: PANTALLA 2 (SURFACE[1]) ESTA VACIA");
        return;
    }

    QueueLogical(link, ip, mac, text);
}

string ReadInputText()
{
    if (input == null) return "";
    StringBuilder sb = new StringBuilder();
    input.ReadText(sb, false);
    return sb.ToString().Trim();
}

void QueueLogical(Link link, int dstIP, int dstMAC, string text)
{
    LogicalSend m = new LogicalSend();
    m.DstIP = dstIP;
    m.DstMAC = dstMAC;
    m.Text = text;
    m.Hash = HashText(text);
    m.Total = Math.Max(1, (text.Length + FRAGMENT_CHARS - 1) / FRAGMENT_CHARS);
    link.LogicalQueue.Enqueue(m);

    SentMessage cache = new SentMessage();
    cache.DstIP = dstIP;
    cache.DstMAC = dstMAC;
    cache.Text = text;
    cache.Hash = m.Hash;
    cache.Total = m.Total;
    cache.LastUse = clock;
    sentMessages[m.Hash] = cache;

    Log(link, "MESSAGE QUEUED HASH:" + m.Hash + " PARTS:" + m.Total + " CHARS:" + text.Length);
}

void PumpLogical(Link link)
{
    if (link.Current != null || link.Queue.Count > 0) return;

    if (link.LogicalCurrent == null)
    {
        if (link.LogicalQueue.Count == 0) return;
        link.LogicalCurrent = link.LogicalQueue.Dequeue();
    }

    LogicalSend m = link.LogicalCurrent;
    if (m.NextPart > m.Total)
    {
        link.LogicalCurrent = null;
        PumpLogical(link);
        return;
    }

    QueueFragment(link, m.DstIP, m.DstMAC, m.Text, m.Hash, m.NextPart, m.Total, true);
}

string FragmentText(string text, int part)
{
    int start = (part - 1) * FRAGMENT_CHARS;
    if (start < 0 || start >= text.Length) return "";
    int len = Math.Min(FRAGMENT_CHARS, text.Length - start);
    return text.Substring(start, len);
}

void QueueFragment(Link link, int dstIP, int dstMAC, string fullText,
                   string fullHash, int part, int total, bool advancesLogical)
{
    string payload = FragmentText(fullText, part);
    string partHash = HashText(payload);
    string envelope = FRAG_MAGIC + fullHash + "|" + part + "|" + total + "|" +
                      partHash + "|" + payload.Length + "|" + payload;

    TxItem item = BuildDataItem(link, dstIP, dstMAC, envelope);
    item.LogicalHash = fullHash;
    item.Part = part;
    item.Total = total;
    item.AdvancesLogical = advancesLogical;
    link.Queue.Enqueue(item);

    Log(link, "QUEUED ID:" + item.Packet.Id + " HASH:" + fullHash +
              " PART:" + part + "/" + total + " DATA:" + payload.Length);
}

void QueueInternalResendRequest(int dstIP, int dstMAC, string hash, int part)
{
    string data = RESEND_MAGIC + hash + "|" + part + "|";
    TxItem item = BuildDataItem(rx, dstIP, dstMAC, data);
    rx.Queue.Enqueue(item);
    Log(rx, "RESPONSE RESEND -> RX " + dstIP + "/" + dstMAC +
            " HASH:" + hash + " PART:" + part);
}

TxItem BuildDataItem(Link link, int dstIP, int dstMAC, string data)
{
    Packet p = new Packet();
    p.DstIP = dstIP;
    p.SrcIP = MY_IP;
    p.DstMAC = dstMAC;
    p.SrcMAC = MY_MAC;
    p.Id = AllocateId(link);
    p.Ttl = TTL_MAX;
    p.Hops.Add(MY_IP);

    for (int i = 0; i < data.Length; i++)
        p.Data.Add((byte)data[i]);

    List<Sig> signals = BuildSignals(p, false);
    string frame = BuildFrameSummary(link, p, false, "DATA");
    TxItem item = new TxItem(signals, p, false, frame);
    item.Backoff = random.NextDouble() * STEP * 4.0;
    link.LastFrame = frame;
    return item;
}

int AllocateId(Link link)
{
    int id = 1;
    while (link.UsedIds.Contains(id)) id++;
    link.UsedIds.Add(id);
    return id;
}

void ReleaseId(Link link, int id)
{
    if (id > 0) link.UsedIds.Remove(id);
}

List<Sig> BuildSignals(Packet p, bool isAck)
{
    List<Sig> s = new List<Sig>();
    AddType(s, CONFIRM);
    AddField(s, DST_IP, p.DstIP);
    AddField(s, SRC_IP, p.SrcIP);
    AddField(s, DST_MAC, p.DstMAC);
    AddField(s, SRC_MAC, p.SrcMAC);
    AddField(s, ID, p.Id);

    if (isAck)
    {
        AddType(s, ACK);
        return s;
    }

    AddField(s, TTL, p.Ttl);
    AddField(s, HOP_CNT, p.Hops.Count);
    AddType(s, HOP_LST);
    for (int i = 0; i < p.Hops.Count; i++)
    {
        AddPairs(s, p.Hops[i]);
        if (i + 1 < p.Hops.Count) AddType(s, HOP_SP);
    }
    AddType(s, HOP_LST);

    AddType(s, DT);
    for (int i = 0; i < p.Data.Count; i++)
    {
        byte b = p.Data[i];
        AddPair(s, (b >> 6) & 3);
        AddPair(s, (b >> 4) & 3);
        AddPair(s, (b >> 2) & 3);
        AddPair(s, b & 3);
    }
    AddType(s, DT);

    // Transporte original: la trama DATA termina DT -> ACK.
    AddType(s, ACK);
    return s;
}

void AddField(List<Sig> s, int type, int value)
{
    AddType(s, type);
    AddPairs(s, value);
    AddType(s, type);
}

void AddType(List<Sig> s, int type) { s.Add(new Sig(0, type)); }
void AddPair(List<Sig> s, int pair) { s.Add(new Sig(1, pair)); }

void AddPairs(List<Sig> s, int value)
{
    if (value == 0)
    {
        AddPair(s, 0);
        return;
    }

    List<int> pairs = new List<int>();
    int v = value;
    while (v > 0)
    {
        pairs.Add(v & 3);
        v >>= 2;
    }
    for (int i = pairs.Count - 1; i >= 0; i--) AddPair(s, pairs[i]);
}

string BuildFrameSummary(Link link, Packet p, bool isAck, string extra)
{
    if (isAck)
        return link.Prefix + " ACK ID:" + p.Id + " -> " + p.DstIP + "/" + p.DstMAC;
    return link.Prefix + " ID:" + p.Id + " -> " + p.DstIP + "/" + p.DstMAC +
           " BYTES:" + p.Data.Count + " " + extra;
}

void Tick(Link link, double dt)
{
    if (link.Current == null && link.Queue.Count == 0)
        PumpLogical(link);

    if (link.Current == null && link.Queue.Count > 0)
    {
        link.Current = link.Queue.Dequeue();
        link.Current.Pos = 0;
        link.Current.Timer = 0;
    }

    if (link.Current == null) return;
    TxItem item = link.Current;

    if (item.Backoff > 0)
    {
        item.Backoff -= dt;
        if (item.Backoff > 0) return;
        item.Backoff = 0;
    }

    if (item.WaitingAck)
    {
        item.Timer += dt;
        if (item.Timer >= AckTimeout(item))
        {
            if (item.Retries >= MAX_RETRIES)
            {
                Echo("DROP " + link.Prefix + " ID:" + item.Packet.Id + " ACK timeout");
                ReleaseId(link, item.Packet.Id);
                if (item.AdvancesLogical) link.LogicalCurrent = null;
                link.Current = null;
                return;
            }

            item.Retries++;
            item.Pos = 0;
            item.Timer = 0;
            item.WaitingAck = false;
            item.Backoff = (0.5 + random.NextDouble()) * STEP * (item.Retries + 1);
            Echo("RETRY " + link.Prefix + " ID:" + item.Packet.Id + " #" + item.Retries);
        }
        return;
    }

    item.Timer += dt;
    if (item.Timer < STEP) return;
    item.Timer -= STEP;

    if (item.Pos < item.Signals.Count)
    {
        SendSignal(link, item.Signals[item.Pos]);
        item.Pos++;
    }

    if (item.Pos >= item.Signals.Count)
    {
        if (item.IsAck)
        {
            link.Current = null;
            return;
        }

        item.WaitingAck = true;
        item.Timer = 0;
        Echo("WAIT ACK " + link.Prefix + " ID:" + item.Packet.Id +
             (item.Part > 0 ? " PART " + item.Part + "/" + item.Total : ""));
    }
}

double AckTimeout(TxItem item)
{
    // Formula original: se conserva sin reinterpretar STEP.
    return 1.0 + item.Signals.Count * STEP + 0.25;
}

void SendSignal(Link link, Sig sig)
{
    if (sig.Kind == 0)
    {
        if (sig.Value < 1 || sig.Value > 12) return;
        IMyFunctionalBlock block = link.Type[sig.Value];
        if (block == null)
        {
            Log(link, "TX ERROR MISSING " + link.Prefix + "_RN_" + TypeName(sig.Value));
            return;
        }
        block.ApplyAction("SendSignal");
        return;
    }

    if (sig.Kind == 1 && sig.Value >= 0 && sig.Value < 4)
    {
        IMyFunctionalBlock block = link.Data[sig.Value];
        if (block == null)
        {
            Log(link, "TX ERROR MISSING " + link.Prefix + "_RN_DT_" + PairText(sig.Value));
            return;
        }
        block.ApplyAction("SendSignal");
    }
}

public void Save() { }

void HandleRelayArgument(Link link, string a)
{
    string prefix = link.Prefix + ":";

    if (a == prefix + "CONFIRM") { ResetRx(link); return; }
    if (a == prefix + "ACK") { HandleAck(link); return; }
    if (a == prefix + "HOP_SP") { HandleHopSeparator(link); return; }
    if (a == prefix + "DT") { HandleDT(link); return; }

    for (int t = 1; t <= 12; t++)
    {
        if (a == prefix + TypeName(t))
        {
            HandleType(link, t);
            return;
        }
    }

    string dataPrefix = link.Prefix == "TX" ? "txdata:" : "rxdata:";
    if (a.StartsWith(dataPrefix) && a.Length == dataPrefix.Length + 2)
    {
        string pairText = a.Substring(dataPrefix.Length);
        if (IsPair(pairText))
        {
            HandlePair(link, Bin2(pairText));
            return;
        }
    }

    Log(link, "ARGUMENTO NO RECONOCIDO: " + a);
}

void HandleType(Link link, int type)
{
    if (type == CONFIRM) { ResetRx(link); return; }
    if (type == ACK) { HandleAck(link); return; }
    if (type == HOP_SP) { HandleHopSeparator(link); return; }
    if (type == DT) { HandleDT(link); return; }
    if (link.RxDrop) return;

    if (link.RxField == type)
    {
        FinishField(link);
        return;
    }

    if (link.RxField != -1)
    {
        link.RxDrop = true;
        Log(link, "ERROR: campo sin cierre " + TypeName(link.RxField));
        return;
    }

    link.RxField = type;
    link.RxValue = 0;
    link.RxHasPair = false;
}

void HandlePair(Link link, int pair)
{
    if (link.RxDrop) return;
    if (link.RxInData)
    {
        AddDataPair(link, pair);
        return;
    }
    if (link.RxField < 0 || link.RxField == CONFIRM ||
        link.RxField == ACK || link.RxField == HOP_SP) return;

    link.RxValue = (link.RxValue << 2) | pair;
    link.RxHasPair = true;
}

void HandleDT(Link link)
{
    if (link.RxDrop) return;

    if (link.RxInData)
    {
        link.RxInData = false;
        link.RxField = -1;
        string raw = link.Message.ToString();
        Log(link, "DT CLOSE " + link.DataCount + " bytes");
        ProcessReceivedData(link, raw);
        return;
    }

    if (link.RxField != -1)
    {
        link.RxDrop = true;
        Log(link, "ERROR: DT antes de cerrar " + TypeName(link.RxField));
        return;
    }

    link.RxInData = true;
    link.DataBits.Clear();
    link.DataCount = 0;
    link.Message.Clear();
}

void AddDataPair(Link link, int pair)
{
    link.DataBits.Add((pair & 2) != 0);
    link.DataBits.Add((pair & 1) != 0);
    if (link.DataBits.Count < 8) return;

    int value = 0;
    for (int i = 0; i < 8; i++) value = (value << 1) | (link.DataBits[i] ? 1 : 0);
    link.Message.Append((char)value);
    link.DataCount++;
    link.DataBits.Clear();
}

void ProcessReceivedData(Link link, string raw)
{
    if (link.Id < 0 || link.SrcIP < 0 || link.SrcMAC < 0) return;

    // Un paquete estructuralmente recibido siempre obtiene ACK en SU MISMO CANAL.
    EnqueueAck(link, link.SrcIP, link.SrcMAC, link.Id);
    lastRxId = link.Id;

    if (raw.StartsWith(RESEND_MAGIC))
    {
        HandleResendRequest(link, raw);
        return;
    }

    if (!raw.StartsWith(FRAG_MAGIC))
    {
        // Compatibilidad: paquete V4 antiguo sin envoltura.
        lastReceived = raw;
        if (output != null) output.WriteText(raw, false);
        DeliverCommands(raw);
        return;
    }

    string hash;
    string partHash;
    string payload;
    int part;
    int total;
    if (!ParseFragment(raw, out hash, out part, out total, out partHash, out payload))
    {
        Log(link, "FRAGMENT HEADER CORRUPT ID:" + link.Id);
        return;
    }

    string actual = HashText(payload);
    if (actual != partHash)
    {
        Log(link, "CORRUPT HASH:" + hash + " PART:" + part + "/" + total);
        QueueInternalResendRequest(link.SrcIP, link.SrcMAC, hash, part);
        return;
    }

    string completeKey = AssemblyKey(link.SrcIP, link.SrcMAC, hash);
    double completedAt;
    if (completed.TryGetValue(completeKey, out completedAt) && clock - completedAt < COMPLETED_KEEP)
    {
        Log(link, "DUP COMPLETED HASH:" + hash + " PART:" + part);
        return;
    }

    Assembly a;
    if (!assemblies.TryGetValue(completeKey, out a))
    {
        a = new Assembly();
        a.SrcIP = link.SrcIP;
        a.SrcMAC = link.SrcMAC;
        a.Hash = hash;
        a.Total = total;
        a.LastActivity = clock;
        a.LastRequest = clock;
        assemblies[completeKey] = a;
    }

    if (a.Total != total || part < 1 || part > total)
    {
        Log(link, "FRAGMENT META INVALID HASH:" + hash);
        return;
    }

    a.LastActivity = clock;
    if (!a.Parts.ContainsKey(part)) a.Parts[part] = payload;
    a.Requested.Remove(part);
    Log(link, "PART OK HASH:" + hash + " " + part + "/" + total);

    // Si ya vemos una parte posterior, podemos pedir inmediatamente cualquier hueco anterior.
    for (int i = 1; i < part; i++)
    {
        if (!a.Parts.ContainsKey(i) && !a.Requested.Contains(i))
        {
            a.Requested.Add(i);
            QueueInternalResendRequest(a.SrcIP, a.SrcMAC, a.Hash, i);
        }
    }

    TryCompleteAssembly(completeKey, a);
}

bool ParseFragment(string raw, out string hash, out int part, out int total,
                   out string partHash, out string payload)
{
    hash = "";
    partHash = "";
    payload = "";
    part = 0;
    total = 0;

    int pos = FRAG_MAGIC.Length;
    string sPart;
    string sTotal;
    string sLen;
    if (!ReadToken(raw, ref pos, out hash)) return false;
    if (!ReadToken(raw, ref pos, out sPart)) return false;
    if (!ReadToken(raw, ref pos, out sTotal)) return false;
    if (!ReadToken(raw, ref pos, out partHash)) return false;
    if (!ReadToken(raw, ref pos, out sLen)) return false;

    int len;
    if (!int.TryParse(sPart, out part)) return false;
    if (!int.TryParse(sTotal, out total)) return false;
    if (!int.TryParse(sLen, out len)) return false;
    if (len < 0 || pos + len != raw.Length) return false;
    payload = raw.Substring(pos, len);
    return true;
}

bool ReadToken(string text, ref int pos, out string token)
{
    token = "";
    if (pos < 0 || pos > text.Length) return false;
    int end = text.IndexOf('|', pos);
    if (end < 0) return false;
    token = text.Substring(pos, end - pos);
    pos = end + 1;
    return true;
}

void HandleResendRequest(Link link, string raw)
{
    int pos = RESEND_MAGIC.Length;
    string hash;
    string sPart;
    if (!ReadToken(raw, ref pos, out hash)) return;
    if (!ReadToken(raw, ref pos, out sPart)) return;
    int part;
    if (!int.TryParse(sPart, out part)) return;

    SentMessage m;
    if (!sentMessages.TryGetValue(hash, out m))
    {
        Log(link, "RESEND CACHE MISS HASH:" + hash + " PART:" + part);
        return;
    }
    if (part < 1 || part > m.Total) return;

    m.LastUse = clock;
    // La peticion se recibio en 'link'. La recuperacion sale por ESE MISMO canal:
    // TX->TX o RX->RX. Como response usa RX, normalmente esto sera RX->RX.
    QueueFragment(link, link.SrcIP, link.SrcMAC, m.Text, m.Hash, part, m.Total, false);
    Log(link, "RESEND HASH:" + hash + " PART:" + part + " VIA " + link.Prefix);
}

void TryCompleteAssembly(string key, Assembly a)
{
    if (a.Parts.Count < a.Total) return;

    StringBuilder sb = new StringBuilder();
    for (int i = 1; i <= a.Total; i++)
    {
        string p;
        if (!a.Parts.TryGetValue(i, out p)) return;
        sb.Append(p);
    }

    string full = sb.ToString();
    if (HashText(full) != a.Hash)
    {
        Log(rx, "GLOBAL HASH FAIL " + a.Hash + " -> REQUEST ALL");
        a.Parts.Clear();
        a.Requested.Clear();
        a.LastActivity = clock;
        for (int i = 1; i <= a.Total; i++)
        {
            a.Requested.Add(i);
            QueueInternalResendRequest(a.SrcIP, a.SrcMAC, a.Hash, i);
        }
        return;
    }

    lastReceived = full;
    if (output != null) output.WriteText(full, false);
    completed[key] = clock;
    assemblies.Remove(key);
    DeliverCommands(full);
    Echo("MESSAGE OK HASH:" + a.Hash + " PARTS:" + a.Total + " CHARS:" + full.Length);
}

void CheckAssemblies()
{
    // IMPORTANTE:
    // NO pedir automaticamente "partes que faltan" por un temporizador corto.
    //
    // No se usa un timeout corto para declarar una parte perdida
    // solo por no haber llegado todavía la siguiente parte.
    //
    // La versión anterior esperaba solo unos segundos y empezaba a mandar
    // /resend de PART 2 mientras PART 2 todavía estaba transmitiéndose.
    // Eso llenaba RX de peticiones/reenvíos y podía crear el bucle observado.
    //
    // La recuperación sigue existiendo, pero se dispara únicamente cuando
    // hay evidencia real:
    //   1) hash individual incorrecto -> se pide ESA parte inmediatamente;
    //   2) llega una parte posterior y existe un hueco anterior -> se pide
    //      únicamente la parte anterior que falta.
    //
    // El ACK/retry físico del emisor ya se ocupa de un paquete que no llegó.
    // Por eso aquí no hacemos recuperación basada solamente en tiempo.
}

void CleanupCompleted()
{
    List<string> remove = new List<string>();
    foreach (KeyValuePair<string, double> kv in completed)
        if (clock - kv.Value >= COMPLETED_KEEP) remove.Add(kv.Key);
    for (int i = 0; i < remove.Count; i++) completed.Remove(remove[i]);
}

string AssemblyKey(int srcIP, int srcMAC, string hash)
{
    return srcIP + ":" + srcMAC + ":" + hash;
}

void DeliverCommands(string message)
{
    string commands = ExtractCommands(message);

    IMyProgrammableBlock pb = GridTerminalSystem.GetBlockWithName(SECONDARY_PB_NAME) as IMyProgrammableBlock;
    if (pb == null)
    {
        if (commands.Length > 0)
            Echo("WARN: PB secundario no encontrado: " + SECONDARY_PB_NAME);
        return;
    }

    if (commands.Length == 0)
    {
        // Mensaje valido SIN comandos: limpiar solo la pantalla del secundario.
        // No se ejecuta TryRun(""), por lo que ningun comando anterior se repite.
        IMyTextSurfaceProvider surfaces = pb as IMyTextSurfaceProvider;
        if (surfaces != null && surfaces.SurfaceCount > 0)
        {
            IMyTextSurface secondaryOutput = surfaces.GetSurface(0);
            secondaryOutput.ContentType = ContentType.TEXT_AND_IMAGE;
            secondaryOutput.WriteText("", false);
        }
        return;
    }

    // Todos los comandos extraidos del mismo mensaje se entregan juntos, una sola vez.
    // RN_COMMAND_PB es responsable de encolar el lote si ya esta ocupado.
    if (!pb.TryRun(commands))
        Echo("WARN: " + SECONDARY_PB_NAME + " no pudo encolar el lote");
}

string ExtractCommands(string text)
{
    StringBuilder result = new StringBuilder();
    int i = 0;
    while (i < text.Length)
    {
        int start = text.IndexOf('/', i);
        if (start < 0) break;

        int p = start + 1;
        while (p < text.Length && !char.IsWhiteSpace(text[p]) && text[p] != '[' && text[p] != '\\') p++;
        if (p == start + 1)
        {
            i = start + 1;
            continue;
        }

        int bracket = 0;
        bool foundEnd = false;
        int end = p;
        for (; end < text.Length; end++)
        {
            char c = text[end];
            if (c == '[') bracket++;
            else if (c == ']' && bracket > 0) bracket--;
            else if (c == '\\' && bracket == 0)
            {
                foundEnd = true;
                break;
            }
        }

        if (!foundEnd)
        {
            i = start + 1;
            continue;
        }

        if (result.Length > 0) result.Append('\n');
        result.Append(text.Substring(start, end - start + 1));
        i = end + 1;
    }
    return result.ToString();
}

string HashText(string text)
{
    unchecked
    {
        uint h = 2166136261u;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            h ^= (byte)(c & 255);
            h *= 16777619u;
            h ^= (byte)((c >> 8) & 255);
            h *= 16777619u;
        }
        return h.ToString("X8");
    }
}

void HandleHopSeparator(Link link)
{
    if (link.RxDrop || link.RxField != HOP_LST) return;
    if (link.RxHasPair)
    {
        link.Hops.Add(link.RxValue);
        link.RxValue = 0;
        link.RxHasPair = false;
    }
}

void FinishField(Link link)
{
    if (link.RxField < 0 || link.RxDrop) return;
    int value = link.RxHasPair ? link.RxValue : 0;

    switch (link.RxField)
    {
        case DST_IP:
            link.DstIP = value;
            if (value != MY_IP && value != IP_BROADCAST) link.RxDrop = true;
            break;
        case SRC_IP:
            link.SrcIP = value;
            lastSrcIP = value;
            lastRxChannel = link == tx ? 0 : 1;
            break;
        case DST_MAC:
            link.DstMAC = value;
            if (value != MY_MAC) link.RxDrop = true;
            break;
        case SRC_MAC:
            link.SrcMAC = value;
            lastSrcMAC = value;
            break;
        case ID:
            link.Id = value;
            lastRxId = value;
            break;
        case TTL:
            link.Ttl = value;
            if (value <= 0) link.RxDrop = true;
            break;
        case HOP_CNT:
            link.HopCount = value;
            link.Hops.Clear();
            break;
        case HOP_LST:
            if (link.RxHasPair) link.Hops.Add(link.RxValue);
            break;
    }

    link.RxField = -1;
    link.RxValue = 0;
    link.RxHasPair = false;
}

void HandleAck(Link link)
{
    // ACK pertenece estrictamente al mismo canal en que llega: TX->TX, RX->RX.
    if (link.Current == null || link.Current.IsAck) return;

    int ackId = link.Id;
    if (ackId < 0) return;

    if (ackId == link.Current.Packet.Id)
    {
        TxItem done = link.Current;
        Log(link, "ACK OK ID:" + ackId);
        ReleaseId(link, ackId);
        link.Current = null;

        if (done.AdvancesLogical && link.LogicalCurrent != null &&
            link.LogicalCurrent.Hash == done.LogicalHash &&
            link.LogicalCurrent.NextPart == done.Part)
        {
            link.LogicalCurrent.NextPart++;
            if (link.LogicalCurrent.NextPart > link.LogicalCurrent.Total)
                link.LogicalCurrent = null;
        }

        Echo("ACK " + link.Prefix + " OK ID:" + ackId +
             (done.Part > 0 ? " PART " + done.Part + "/" + done.Total : ""));
    }
    else
    {
        Log(link, "ACK ID:" + ackId + " esperado:" + link.Current.Packet.Id);
    }
}

void EnqueueAck(Link link, int dstIP, int dstMAC, int packetId)
{
    Packet p = new Packet();
    p.DstIP = dstIP;
    p.SrcIP = MY_IP;
    p.DstMAC = dstMAC;
    p.SrcMAC = MY_MAC;
    p.Id = packetId;
    p.Ttl = 0;

    List<Sig> signals = BuildSignals(p, true);
    string frame = BuildFrameSummary(link, p, true, "ACK");
    TxItem item = new TxItem(signals, p, true, frame);
    item.Backoff = random.NextDouble() * STEP * 2.0;

    // El ACK va por el mismo Link que recibio el paquete.
    link.Queue.Enqueue(item);
    link.LastFrame = frame;
}

void ResetRx(Link link)
{
    link.RxField = -1;
    link.RxValue = 0;
    link.RxHasPair = false;
    link.RxDrop = false;
    link.RxInData = false;
    link.DstIP = 0;
    link.SrcIP = -1;
    link.DstMAC = 0;
    link.SrcMAC = -1;
    link.Id = -1;
    link.Ttl = 0;
    link.HopCount = 0;
    link.Hops.Clear();
    link.DataBits.Clear();
    link.DataCount = 0;
    link.Message.Clear();
}

string TypeName(int type)
{
    switch (type)
    {
        case CONFIRM: return "CONFIRM";
        case DST_IP: return "DST_IP";
        case SRC_IP: return "SRC_IP";
        case DST_MAC: return "DST_MAC";
        case SRC_MAC: return "SRC_MAC";
        case ID: return "ID";
        case TTL: return "TTL";
        case HOP_CNT: return "HOP_CNT";
        case HOP_LST: return "HOP_LST";
        case HOP_SP: return "HOP_SP";
        case DT: return "DT";
        case ACK: return "ACK";
    }
    return "?";
}

bool IsPair(string s)
{
    return s.Length == 2 &&
           (s[0] == '0' || s[0] == '1') &&
           (s[1] == '0' || s[1] == '1');
}

int Bin2(string s)
{
    return (s[0] == '1' ? 2 : 0) + (s[1] == '1' ? 1 : 0);
}

string PairText(int pair)
{
    if (pair == 0) return "00";
    if (pair == 1) return "01";
    if (pair == 2) return "10";
    return "11";
}

void Log(Link link, string text)
{
    link.Trace += "\n" + text;
    if (link.Trace.Length > 1600)
        link.Trace = link.Trace.Substring(link.Trace.Length - 1600);
}

void Init()
{
    IMyTextSurfaceProvider surfaces = Me as IMyTextSurfaceProvider;
    if (surfaces != null)
    {
        if (output == null && surfaces.SurfaceCount > 0) output = surfaces.GetSurface(0);
        if (input == null && surfaces.SurfaceCount > 1) input = surfaces.GetSurface(1);
    }

    if (output != null) output.ContentType = ContentType.TEXT_AND_IMAGE;
    if (input != null) input.ContentType = ContentType.TEXT_AND_IMAGE;

    InitLink(tx);
    InitLink(rx);
}

void InitLink(Link link)
{
    for (int i = 1; i <= 12; i++)
    {
        if (link.Type[i] == null)
        {
            string name = link.Prefix + "_RN_" + TypeName(i);
            link.Type[i] = GridTerminalSystem.GetBlockWithName(name) as IMyFunctionalBlock;
            if (link.Type[i] == null) Log(link, "MISSING " + name);
        }
    }

    for (int i = 0; i < 4; i++)
    {
        if (link.Data[i] == null)
        {
            string name = link.Prefix + "_RN_DT_" + PairText(i);
            link.Data[i] = GridTerminalSystem.GetBlockWithName(name) as IMyFunctionalBlock;
            if (link.Data[i] == null) Log(link, "MISSING " + name);
        }
    }
}

void Status()
{
    StringBuilder s = new StringBuilder();
    s.AppendLine("=== RELAY-NET V4 FRAGMENTED ===");
    s.AppendLine("IP:" + MY_IP + " MAC:" + MY_MAC);
    s.AppendLine("PRIMARY PB NAME: no requerido");
    s.AppendLine("SECONDARY: " + SECONDARY_PB_NAME);
    s.AppendLine("OUTPUT[0]: " + (output != null ? "OK" : "FAIL") +
                 " INPUT[1]: " + (input != null ? "OK" : "FAIL"));
    s.AppendLine("LAST ARG:" + (lastCommand.Length == 0 ? "-" : lastCommand));
    s.AppendLine("TX " + TxState(tx) + " Q:" + tx.Queue.Count + " LQ:" + tx.LogicalQueue.Count);
    s.AppendLine("RX " + TxState(rx) + " Q:" + rx.Queue.Count + " LQ:" + rx.LogicalQueue.Count);
    s.AppendLine("ASSEMBLIES:" + assemblies.Count + " CACHE:" + sentMessages.Count);
    s.AppendLine("LAST RX CH:" + (lastRxChannel == 0 ? "TX" : lastRxChannel == 1 ? "RX" : "-") +
                 " SRC:" + lastSrcIP + "/" + lastSrcMAC + " ID:" + lastRxId);
    s.AppendLine();
    s.AppendLine("--- TX FRAME ---");
    s.AppendLine(tx.LastFrame.Length == 0 ? "(none)" : tx.LastFrame);
    s.AppendLine("--- RX FRAME ---");
    s.AppendLine(rx.LastFrame.Length == 0 ? "(none)" : rx.LastFrame);
    s.AppendLine("--- TX TRACE ---");
    s.AppendLine(tx.Trace);
    s.AppendLine("--- RX TRACE ---");
    s.AppendLine(rx.Trace);
    Echo(s.ToString());
}

string TxState(Link link)
{
    if (link.Current == null) return "IDLE";
    if (link.Current.WaitingAck) return "WAIT_ACK";
    if (link.Current.Backoff > 0) return "BACKOFF";
    return "SEND";
}
