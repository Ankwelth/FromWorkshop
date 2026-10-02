//HellArea Astronomicon v2.10 (2020-2022) / mailto:HellArea@Outlook.com
double RayCastLookupRange = 15000;
const string Logo = "Astronomicon";
string _PanelKey = "_Astro";
string _SearchPanelKey = "_FindAstro";
bool BaseMode = false;

string RadioKey = "ASTRONOMICON";
string CamerasGroupName = "Astro"; //Optional. Group name for cameras.
bool RadioReplication = true;
bool DontRenameMe = false;
float AutopilotSpeedLimit = 100;
// RadioKey is a security key for updating via radio
// Use your own key for better privacy

public Program() { Boot(); if (Core.State > 0) { Show(); } }

public void Main(string Argument, UpdateType UpdateSource)
{
    if (Core.State < 1) { Boot(); return; }
    if (!string.IsNullOrEmpty(Argument)) { ProceedArgument(Argument); }

    if (RadioReplication)
    {
        Tick++; if (Tick > 8)
        {
            Tick = 0;
            IGC.GetBroadcastListeners(Core.Radio.Members);
            Core.Radio.Members.ForEach(R =>
            {
                if (R.HasPendingMessage)
                {
                    Core.Radio.Message = R.AcceptMessage();
                    string M = Core.Radio.Message.Data.ToString();

                    if (M.StartsWith(rMyCount))
                    {
                        int C = -1; int.TryParse(M.Substring(2), out C);
                        if (C != Core.BasePanels.First().CustomData.Length) { IGC.SendBroadcastMessage(RadioKey, rMyBase + Me.CubeGrid.CustomName + rMyBase + Core.BasePanels.First().CustomData, TransmissionDistance.TransmissionDistanceMax); }
                    }

                    else if (M.StartsWith(rMyBase))
                    {
                        int C = Core.Base.Count();
                        string From = M.Substring(2, M.IndexOf(rMyBase, 2) - 2);
                        Core.Replicate(M.Substring(4 + From.Length));
                        InitStorage(false);
                        if (Core.Base.Count() != C) Show(Logo + ".Replication" + _N + sNow() + " Asteroids added: " + (Core.Base.Count() - C) + "\nfrom " + From);
                    }
                }
            });

            IGC.SendBroadcastMessage(RadioKey, rMyCount + Core.BasePanels.First().CustomData.Length, TransmissionDistance.TransmissionDistanceMax);
        }
    }
    //Echo(Runtime.CurrentInstructionCount.ToString());
}

public struct tCore
{
    public int State;
    public IMyShipController GCM;
    public Vector3D GCMPos;
    public List<IMyShipController> SC;
    public List<IMyTextSurface> Surfaces;
    public List<IMyTextSurface> FindSurfaces;
    public List<IMyTextPanel> BasePanels;
    public List<IMyCameraBlock> Cameras;
    public IMyRemoteControl RC;
    public List<string> Base;
    public tTargetInfo Target;
    public string RayCastInfo;
    public tRadio Radio;
    public string GetRayCastInfoLine() => Target.Active ? Target.ID + _D + (Target.Type == MyDetectedEntityType.Asteroid ? "A" : Target.Type == MyDetectedEntityType.Planet ? "P" : "U") + _D + Target.Name + _D + Target.Position + _D + Target.Size + _D + Target.Description + _D + Target.Discovered : "";

    public int GetIndexFromBase(long ID) => Base.FindIndex(R => R.StartsWith(ID + _D));

    public bool TryAddToBase()
    {
        bool R = false;
        if (Target.Active && (Target.Type == MyDetectedEntityType.Asteroid || Target.Type == MyDetectedEntityType.Planet || Target.Type == MyDetectedEntityType.Unknown) && GetIndexFromBase(Target.ID) == -1)
        { Base.Add(GetRayCastInfoLine()); Base.Sort(); R = true; }
        return R;
    }

    public void LoadTargetFromBase(long ID)
    {
        int Index = GetIndexFromBase(ID);
        if (Index != -1) { LoadTargetInfo(Base[Index], ref Target); }
    }

    public void Replicate(string Data)
    {
        int i; string[] C, S;
        foreach (string R in Data.Split(new string[] { _N }, StringSplitOptions.None).ToList())
        {
            try
            {
                i = Base.FindIndex(F => F.StartsWith(R.Substring(0, R.IndexOf(_C)) + _D));
                if (i == -1) { Base.Add(R); }
                else
                {
                    C = Base[i].Split(new string[] { _D }, StringSplitOptions.None);
                    S = R.Split(new string[] { _D }, StringSplitOptions.None);
                    if (S[5] != _NA)
                    {
                        if (C[5] == _NA) { Base[i] = Base[i].Replace(C[5], S[5]); }
                        else { if (String.Compare(C[5], S[5], StringComparison.Ordinal) < 0) { Base[i] = Base[i].Replace(C[5], S[5]); } }
                    }
                }
            }
            catch { }
        }
        Base.Sort();
    }

    public bool LoadTargetInfo(string Data, ref tTargetInfo Item)
    {
        string[] C = Data.Split(new string[] { _D }, StringSplitOptions.None);
        if (C.Length == 7)
        {
            long.TryParse(C[0], out Item.ID);
            Item.Type = C[1] == "A" ? MyDetectedEntityType.Asteroid : C[1] == "P" ? MyDetectedEntityType.Planet : MyDetectedEntityType.Unknown;
            Item.Name = C[2];
            Vector3D.TryParse(C[3], out Item.Position);
            Item.Size = C[4];
            Item.Description = C[5];
            DateTime.TryParse(C[6], out Item.Discovered);
            return true;
        }
        else { Item.Name = "\nWarning!\nBroken line in Base detected: " + Data + "\nCount: " + C.Length; return false; }
    }

    public string GetCamerasMaxScanRange() => Cameras.Any() ? (Cameras.OrderByDescending(R => R.AvailableScanRange).First().AvailableScanRange / 1000).ToString("0") + " Km" : _NA;
}

public struct tTargetInfo
{
    public bool Active;
    public long ID;
    public string Name;
    public Vector3D Position;
    public string Size;
    public string Description;
    public DateTime Discovered;
    public MyDetectedEntityType Type;
    public Vector3D Hit;
    public bool IsNew;
}

public struct tRadio
{
    public MyIGCMessage Message;
    public List<IMyBroadcastListener> Members;
}

const string _D = "•", _N = "\n", _NA = "n/a";
string rMyCount = ">#", rMyBase = ">B";
const char _C = '•';
int Tick = 0;
tCore Core;

string HelpText = "  Commands\nScan or #\nNote [my text] or n [..], Note+ [TextToAdd] or n+ [..]\nFind [AnyText] or ? [..] (resuilt to PB Custom Data)\nResetBase, Update\n  Key for LCD's _Astro[#]";
void Boot()
{
    Core = new tCore()
    {
        State = 0,
        Surfaces = new List<IMyTextSurface>(),
        FindSurfaces = new List<IMyTextSurface>(),
        Base = new List<string>(),
        RayCastInfo = BaseMode ? "  Raycast disabled: DataBase mode" : "   Use Scan to get Raycast Info",
        Radio = new tRadio() { Members = new List<IMyBroadcastListener>(), Message = new MyIGCMessage() }
    };
    InitDevices();
    if (!BaseMode)
    {
        if (!Core.SC.Any()) { SetState(0, "There is no Ship Control"); return; }
        Core.GCM = Core.SC.FirstOrDefault(G => G.IsMainCockpit) ?? Core.SC.Where(G => G.IsUnderControl && G.CanControlShip).FirstOrDefault() ?? Core.SC.First();
    }
    if (!Core.BasePanels.Any()) { SetState(0, "There is no TextPanel named [AnyName] _Astro"); return; }
    if (RadioReplication) { IGC.RegisterBroadcastListener(RadioKey); Runtime.UpdateFrequency = UpdateFrequency.Update100; InitStorage(true); }
    SetState(1, Logo + "\nStarted successfully\n");
    Echo(BaseMode ? "Base mode activated\nAsteroids in Base: " + Core.Base.Where(R => !String.IsNullOrEmpty(R)).Count() + "\nTextPanels: " + Core.BasePanels.Count
      : "Main control: " + Core.GCM.CustomName + "\nAsteroids in Base: " + Core.Base.Where(R => !String.IsNullOrEmpty(R)).Count() + "\nCameras: " + Core.Cameras.Count
      + "\nTextPanels: " + Core.BasePanels.Count + "\nShowSurfaces: " + Core.Surfaces.Count + "\nShowFindSurfaces: " + Core.FindSurfaces.Count + "\n\n" + HelpText);
}

List<IMyTerminalBlock> GridBlocks = new List<IMyTerminalBlock>();
void InitDevices()
{
    if (!GridBlocks.Any() || Core.State < 1) { GridTerminalSystem.GetBlocksOfType(GridBlocks, R => R.CubeGrid == Me.CubeGrid); }
    if (!DontRenameMe) Me.CustomName = " •• " + Logo;
    try { Me.GetSurface(1).WriteText(HelpText); Me.GetSurface(1).ContentType = ContentType.TEXT_AND_IMAGE; Me.GetSurface(1).FontSize = 2f; } catch { }
    Core.SC = new List<IMyShipController>();
    Core.Cameras = new List<IMyCameraBlock>();
    Core.BasePanels = new List<IMyTextPanel>();
    Core.Surfaces.AddList(GetSurfaces(_PanelKey, ref GridBlocks));
    Core.Surfaces.Add(Me.GetSurface(0));
    Core.Surfaces.ForEach(R => { R.ContentType = ContentType.TEXT_AND_IMAGE; });
    Core.FindSurfaces.AddList(GetSurfaces(_SearchPanelKey, ref GridBlocks));
    Core.FindSurfaces.ForEach(R => { R.ContentType = ContentType.TEXT_AND_IMAGE; });
    Core.BasePanels.AddRange(GridBlocks.Where(R => R is IMyTextPanel && R.CustomName.ToLower().Contains(_PanelKey.ToLower())).Cast<IMyTextPanel>());
    Core.SC.AddRange(GridBlocks.Where(R => R is IMyShipController && (R as IMyShipController).CanControlShip).Cast<IMyShipController>());
    IMyBlockGroup CG = GridTerminalSystem.GetBlockGroupWithName(CamerasGroupName);
    if (CG != null) { CG.GetBlocksOfType(Core.Cameras); } else { GridBlocks.ForEach(B => { if (B is IMyCameraBlock) { Core.Cameras.Add(B as IMyCameraBlock); } }); }
    Core.Cameras.ForEach(R => R.EnableRaycast = true);
}

void ActivateAutopilot()
{
    List<IMyRemoteControl> B = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType(B, R => R.CubeGrid.IsSameConstructAs(Me.CubeGrid));
    if (B.Any())
    {
        if (Core.RC == null)
        {
            if (B.Count > 1)
            {
                if (B.Where(R => R.IsAutoPilotEnabled).Any()) { B.ForEach(R => R.SetAutoPilotEnabled(false)); Show("\n\n  " + sNow() + "  >_ Autopilot is canceled", true); }
                else
                {
                    if (B.Where(R => R.CustomName.ToLower().Contains((" • " + Logo).ToLower())).Any()) { Core.RC = B.Where(R => R.CustomName.ToLower().Contains((" • " + Logo).ToLower())).First(); }
                    else { Core.RC = B.First(); Core.RC.CustomName = " • " + Logo; }
                }
            }
            else { Core.RC = B[0]; }
        }

        if (Core.RC.IsAutoPilotEnabled) { Core.RC.SetAutoPilotEnabled(false); Show("\n\n  " + sNow() + "  >_ Autopilot is canceled", true); }
        else
        {
            bool M1 = false;
            if (Core.RC.FlightMode != FlightMode.OneWay) { M1 = true; Core.RC.FlightMode = FlightMode.OneWay; }
            Core.RC.SetCollisionAvoidance(true);
            if (Core.Target.Active)
            {
                int i = int.Parse(Core.Target.Size.Substring(1));
                Core.RC.SpeedLimit = AutopilotSpeedLimit;
                Core.RC.ClearWaypoints();
                Core.RC.AddWaypoint(Core.Target.Position - Vector3D.Normalize(Core.Target.Position - Core.GCMPos) * i * .75f, Core.Target.Name);
                Core.RC.SetAutoPilotEnabled(true);
                Show("  " + sNow() + "  >_\n    " + Core.RC.CustomName + ".Activated\n\n    Destination: " + Core.Target.Name
                  + (M1 ? "\n\n  System Message:\n    Flight Mode changed to One Way" : String.Empty));
            }
        }
    }
    else { Show("There is no Remote Control block"); }
}

void InitStorage(bool Read)
{
    if (Read) { if (Core.BasePanels.Any()) { Core.Base.Clear(); Core.Base = Core.BasePanels.First().CustomData.Split(new string[] { _N }, StringSplitOptions.RemoveEmptyEntries).ToList(); } }
    else { string S = String.Join(_N, Core.Base).Trim(); Core.BasePanels.ForEach(P => P.CustomData = S); }
}

List<IMyTextSurface> GetSurfaces(string Key, ref List<IMyTerminalBlock> Blocks)
{
    Key = Key.ToLower();
    IMyTextSurfaceProvider N; bool Q = false;
    List<IMyTextSurface> S = new List<IMyTextSurface>();
    foreach (IMyTerminalBlock T in Blocks.Where(R => R.CustomName.ToLower().Contains(Key) && R is IMyTextSurfaceProvider))
    {
        N = T as IMyTextSurfaceProvider; Q = false;
        for (int i = 0; i < N.SurfaceCount; i++) { if (T.CustomName.ToLower().Contains(Key + i.ToString("0"))) { S.Add(N.GetSurface(i)); Q = true; } }
        if (!Q) { S.Add(N.GetSurface(0)); }
    }
    return S;
}

void SetState(int State, string Message = null)
{
    Core.State = State;
    if (!String.IsNullOrEmpty(Message)) { Echo(sNow() + "  " + Message); }
}

void ProceedArgument(string Argument, bool Stop = false)
{
    string A = Argument.ToLower();
    Core.GCMPos = BaseMode ? Me.GetPosition() : Core.GCM.GetPosition();
    if ((A == "scan" || A == "#") && Core.Cameras.Any())
    {
        if (!RayCastForward()) { Core.RayCastInfo = "   " + sNow() + " " + (Core.Cameras.Any() ? "Wait for camera will ready\n" + Core.GetCamerasMaxScanRange() : "No Camera found"); }
        else
        {
            if (!Core.Target.Active) { Core.RayCastInfo = "   " + sNow() + " No success (Cam: " + Core.GetCamerasMaxScanRange() + ")"; }
            else { UpdateRaycastInfo(); }
        }
        Show();
    }
    else if (A.StartsWith("note ") && Core.Target.Active && (Core.Target.Type == MyDetectedEntityType.Asteroid || Core.Target.Type == MyDetectedEntityType.Unknown)) { Core.Target.Description = DateTime.Now.ToString("yyMMddhhmmss") + ":" + Argument.Substring(5); Core.Base[Core.GetIndexFromBase(Core.Target.ID)] = Core.GetRayCastInfoLine(); UpdateRaycastInfo(); InitStorage(false); LastRCID = -1; Show(); }
    else if (A.StartsWith("n ") && Core.Target.Active && (Core.Target.Type == MyDetectedEntityType.Asteroid || Core.Target.Type == MyDetectedEntityType.Unknown)) { Core.Target.Description = DateTime.Now.ToString("yyMMddhhmmss") + ":" + Argument.Substring(2); Core.Base[Core.GetIndexFromBase(Core.Target.ID)] = Core.GetRayCastInfoLine(); UpdateRaycastInfo(); InitStorage(false); LastRCID = -1; Show(); }
    else if (A.StartsWith("note+ ") && Core.Target.Active && (Core.Target.Type == MyDetectedEntityType.Asteroid || Core.Target.Type == MyDetectedEntityType.Unknown)) { Core.Target.Description = (Core.Target.Description == _NA ? "" : DateTime.Now.ToString("yyMMddhhmmss") + ":" + GetNoteText( Core.Target.Description)) + Argument.Substring(6); Core.Base[Core.GetIndexFromBase(Core.Target.ID)] = Core.GetRayCastInfoLine(); UpdateRaycastInfo(); InitStorage(false); LastRCID = -1; Show(); }
    else if (A.StartsWith("n+ ") && Core.Target.Active && (Core.Target.Type == MyDetectedEntityType.Asteroid || Core.Target.Type == MyDetectedEntityType.Unknown)) { Core.Target.Description = (Core.Target.Description == _NA ? "" : DateTime.Now.ToString("yyMMddhhmmss") + ":" +GetNoteText(  Core.Target.Description)) + Argument.Substring(2); Core.Base[Core.GetIndexFromBase(Core.Target.ID)] = Core.GetRayCastInfoLine(); UpdateRaycastInfo(); InitStorage(false); LastRCID = -1; Show(); }
    else if (A.StartsWith("find ")) { BaseSearch(Argument.Substring(5)); }
    else if (A.StartsWith("?")) { BaseSearch(Argument.Substring(1).Trim()); }
    else if (A == "resetbase") { Core.Base.Clear(); Core.BasePanels.ForEach(R => R.CustomData = ""); InitStorage(false); Echo("Base reset complete"); }
    else if (A == "update") { InitStorage(true); Echo(sNow() + " Updated from DataBase"); }
    else if (A.StartsWith("+")) { AddGPS(Argument.Substring(1).Trim()); }
    else if (A.StartsWith("-")) { RemoveGPS(Argument.Substring(1).Trim()); }
    else if (A.StartsWith("=")) { AddPosition(Argument.Substring(1).Trim()); }
    else if (A == (">") && !BaseMode) { ActivateAutopilot(); }
    else if (A.StartsWith(">") && !BaseMode) { BaseSearch(Argument.Substring(1).Trim()); ActivateAutopilot(); }
    InitStorage(false);
}

void AddGPS(string GPS)
{
    Core.Target = new tTargetInfo() { Name = GetGPSName(GPS), Discovered = DateTime.Now, Size = "x0", ID = DateTime.Now.Ticks, Position = GetAsVector3D(GPS), Type = MyDetectedEntityType.Unknown, Description = _NA };
    Core.Target.Active = Core.Target.Name != "0";
    Core.TryAddToBase();
    string M = Core.Target.Active ? "  GPS point was added:\n\n    • " + Core.Target.Name + "\n\n  Objects in base: " + (Core.Base.Count()) : "    GPS point is not added.\n\n    Command usage:\n    +GPS:AnyName:[X]:[Y]:[Z]:{Color]:";
    Show("  " + M); Echo(M);
}

void AddPosition(string Name)
{
    long ID = DateTime.Now.Ticks;
    if (String.IsNullOrEmpty(Name)) { Name = "Point: " + (ID % 4096).ToString("X"); }
    Core.Target = new tTargetInfo() { Name = Name, Active = true, Discovered = DateTime.Now, Size = "x0", ID = ID, Position = Core.GCMPos, Type = MyDetectedEntityType.Unknown, Description = _NA };
    Me.CustomData = " Added point:\n" + GetAsGPS(Core.Target.Position, Name);
    Core.TryAddToBase();
    string M = "  GPS point was added:\n\n    • " + Core.Target.Name + "\n\n    Stored in PB CustomData\n\n  Objects in base: " + (Core.Base.Count());
    Show("  " + M); Echo(M);
}

void RemoveGPS(string GPSName)
{
    if (string.IsNullOrEmpty(GPSName)) { return; }
    int F = 0; int A = 0; string S = ""; string[] C;
    for (int i = 0; i < Core.Base.Count; i++) { C = Core.Base[i].Split(_C); if (C[2].Contains(GPSName)) { F++; A = i; S = C[2]; } };
    string M = F == 0 ? "Can't remove cause these is no\n  " + GPSName : F == 1 ? S + "  was removed\n\n  Objects in base: " + (Core.Base.Count() - 1) : "Can't remove cause there is many of\n  " + GPSName + "(" + F + ")";
    if (F == 1) { Core.Base.RemoveAt(A); }
    Show("  " + M); Echo(M);
}

void Show() { Core.Surfaces.ForEach(R => R.WriteText(Core.RayCastInfo)); }
void Show(string Message, bool Append = false) { Core.Surfaces.ForEach(R => R.WriteText(Message, Append)); }

public struct tLines { public double Length; public string Line; };
void BaseSearch(string Request)
{
    List<tLines> T = new List<tLines>();
    string S = Request.ToLower();
    Vector3D V = Vector3D.Zero;
    double G, M = 1000000;
    IEnumerable<string> L = Core.Base.Where(R => R.ToLower().Contains(S));
    Core.Target = new tTargetInfo();
    foreach (string A in L)
    {
        string[] C = A.Split(_C);
        if (C.Length == 7 && Vector3D.TryParse(C[3], out V))
        {
            G = (V - Core.GCMPos).Length() / 1000;
            T.Add(new tLines() { Length = G, Line = G.ToString("0") + " Km  " + GetAsGPS(V, C[2] + " • " + (C[5] == _NA ? "" : GetNoteText(C[5]))) });
            if (G < M)
            {
                M = G;
                Vector3D P; Vector3D.TryParse(C[3], out P);
                Core.Target = new tTargetInfo() { Active = true, Name = C[2], Description = GetNoteText(C[5]), ID = long.Parse(C[0]), Position = P, Size = C[4], Type = C[1] == "A" ? MyDetectedEntityType.Asteroid : C[1] == "P" ? MyDetectedEntityType.Planet : MyDetectedEntityType.Unknown };
                DateTime.TryParse(C[6], out Core.Target.Discovered);
            }
        }
    }
    Me.CustomData = "Search for " + Request + _N + String.Join(_N, T.OrderBy(R => R.Length).Select(R => R.Line));
    Core.FindSurfaces.ForEach(R => R.WriteText(Me.CustomData));
    Echo(sNow() + " Search results (" + T.Count + ") for " + Request + "\nis stored CustomData");
    Show(Core.Target.Active ?
      "  First object from the DataBase:\n   " + Core.Target.Name + "\n   Distance: " + M.ToString("0.0") + " km\n   Description: " + GetNoteText(Core.Target.Description)
      : "No objects is found");
}

long LastRCID = -1;
void UpdateRaycastInfo()
{
    if (Core.Target.Active)
    {
        Core.RayCastInfo = "   Success (Cam: " + Core.GetCamerasMaxScanRange()
          + ")\n     Object Name: " + Core.Target.Name
          + "\n     Size: " + Core.Target.Size
          + "\n     Distance: " + ((Core.Target.Position - Core.GCMPos).Length() / 1000).ToString("0.000") + " Km"
          + "\n     Discovered: " + (Core.Target.IsNew ? "New " + Core.Target.Type : Core.Target.Discovered.ToString("yyyy.MM.dd hh.mm.ss"))
          + (Core.Target.Type == MyDetectedEntityType.Asteroid || Core.Target.Type == MyDetectedEntityType.Unknown ? "\n     User Note: " : "\n     Speed: ") + GetNoteText(Core.Target.Description);
        LastRCID = Core.Target.ID;
    }
}

bool RayCastForward()
{
    Core.Target = new tTargetInfo();
    bool R = false; MyDetectedEntityInfo A = new MyDetectedEntityInfo();
    Vector3D Target = Core.GCMPos + (RayCastLookupRange * Core.GCM.WorldMatrix.GetDirectionVector(Base6Directions.Direction.Forward));
    foreach (IMyCameraBlock C in Core.Cameras) { if (C.CanScan(Target)) { A = C.Raycast(Target); R = true; break; } }
    if (R)
    {
        Core.Target.Active = !A.IsEmpty();
        if (Core.Target.Active)
        {
            Core.Target.Hit = A.HitPosition.Value;
            Core.Target.IsNew = Core.GetIndexFromBase(A.EntityId) == -1;
            if (Core.Target.IsNew)
            {
                Core.Target.Discovered = DateTime.Now;
                Core.Target.ID = A.EntityId;
                Core.Target.Type = A.Type;
                Core.Target.Name = GetTheName(ref A);
                Core.Target.Position = A.Position;
                Core.Target.Size = A.Type == MyDetectedEntityType.Asteroid ? "x" + A.BoundingBox.Size.Y.ToString("0") : A.BoundingBox.Size.X.ToString("0") + "x" + A.BoundingBox.Size.Y.ToString("0") + "x" + A.BoundingBox.Size.Z.ToString("0");
                Core.Target.Description = (A.Type == MyDetectedEntityType.Asteroid) ? _NA : A.Velocity.Length().ToString("0.00") + " m/s\n     Status: " + A.Relationship;
                if (Core.TryAddToBase()) { InitStorage(false); }
            }
            else { Core.LoadTargetFromBase(A.EntityId); }
            Me.CustomData = GetAsGPS(Core.Target.Position, Core.Target.Name);
        }
    }
    return R;
}

string GetTheName(ref MyDetectedEntityInfo Ray)
{
    string R = Ray.Name;
    if (Ray.Type == MyDetectedEntityType.Asteroid)
    {
        string[] N = new string[] { "Ceres", "Juno", "Vesta", "Gollum", "Flora", "Victoria", "Gabriella", "Thetis", "Fortuna", "Lutetia", "Themis", "Aurora", "Bellona", "Daphne", "Kalypso",
  "Pandora", "Melene", "Echo", "Erato", "Maja", "Feronia", "Galatea", "Freia", "Sappho", "Beatrix", "Aegis", "Minerva", "Undine", "Circe", "Melpomene", "Thalia", "Adria", "Medusa",
  "Aragorn", "Izila","Urus", "Udam", "Lhotski","Aky","Cyrax","Art","Cowax","Proxima","Pain","Leonardo", "Smile","Azura","Boethiah" ,"Hermorah" ,"Hircine" ,"Dagon" ,"Malacath" ,"Mephala",
  "Meridia" ,"Namira","Nocturnal","Peryite","Sanguine" ,"Vaermina","Molag","Jyggalag","Eris ","Io","Demeter","Apollo" ,"Athena","Hera","Chaos"};
        R = N[(Ray.EntityId / 1000) % (N.Length - 1)] + " " + (Ray.EntityId % 4096).ToString("X");
    }
    return R;
}

string GetNoteText(string Text) => Text.Length > 12 && Text.Substring(12, 1) == ":" ? Text.Substring(13) : Text;

string sNow() => DateTime.Now.ToString("HH:mm:ss");
string GetAsGPS(Vector3D argument, string Name = "Unknown") => String.Format("GPS:{0}:{1:0.00}:{2:0.00}:{3:0.00}:", Name, argument.X, argument.Y, argument.Z);
Vector3D GetAsVector3D(string Address)
{
    string[] C = (Address + ":0:0:0:0").Split(new string[] { ":" }, StringSplitOptions.None);
    return new Vector3D(Convert.ToDouble(C[2]), Convert.ToDouble(C[3]), Convert.ToDouble(C[4]));
}
string GetGPSName(string GPS)
{
    string[] C = (GPS + ":0:0:0:0").Split(new string[] { ":" }, StringSplitOptions.None);
    return C[1];
}