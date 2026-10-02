string[] dNm = {"Front", "Rear", "Left", "Right", "Top", "Bottom"};
double[] dDs = {-1, -1, -1, -1, -1, -1};
double[] dOf = {0, 0.8, 0, 0, 0, 0.25};
double[] dockEntryDist = {15.0, 50.0, 15.0, 15.0, 10.0, 10.0};
double flyAltLimit = 100.0;
double[] sFZ = {1.1, 1.5, 1.2, 1.2, 1.2};
List<string> logBuf = new List<string>();
string logPrev = "";
string udLogPrev = "";
List<IMyShipConnector> cons = new List<IMyShipConnector>();
List<IMyLandingGear> grs = new List<IMyLandingGear>();
List<IMyBatteryBlock> bats = new List<IMyBatteryBlock>();
List<IMyGasTank> tks = new List<IMyGasTank>();
List<IMyThrust> thrs = new List<IMyThrust>();
List<IMyLightingBlock> lts = new List<IMyLightingBlock>();
List<IMyCockpit> cks = new List<IMyCockpit>();
List<IMyCargoContainer> cgs = new List<IMyCargoContainer>();
List<IMyGyro> gys = new List<IMyGyro>();
List<IMyTextPanel> pnl = new List<IMyTextPanel>();
IMyRemoteControl rcB = null;
List<IMyCameraBlock>[] dCam = new List<IMyCameraBlock>[6];
List<int> psI = new List<int>();
List<int> psT = new List<int>();
bool wCon = false;
bool wGr = false;
bool isRT = false;
int ulSt = 0;
double timer = -1;
int skDt = 0;
bool wRch = false;
string tag = "";
int dDi = -1;
Vector3D dT;
Vector3D dAD;
bool dAc = false;
int dSt = 0;
bool dCn = false;
double dEl = 0;
double dCS = 0.6;
double dTO = 120.0;
string dMs = "";
double dbUT = 0;
string dbPL = "";
string dbAL = "";
string dbHL = "";
string dbEL = "";
Vector3D dbTg;
double dbEF = 0, dbER = 0, dbEU = 0;
bool dbApE = false;
double dbSL = 0;
int adjWt = 0;
double adjGn = 0.5;
Vector3D adjPP;
string tGy = "";
double tGA = 180;
Vector3D tIF;
Vector3D tIU;
Vector3D fSP;
string dPh = "";
Vector3D dW;
Vector3D dSF;
Vector3D fTg;
double fSF = 100, fSL = 100, fSU = 100;
double fAD = 3.0;
string fLb = "";
bool fAp = false;
bool fAltPre = false;
string fAltRetPh = "";
Vector3D fAltRetTg;
double fAT = 0;
int fRC = 0;
Vector3D fEvD;
int fEvT = 0;
double fEvH = 0;
double fEvA = 0;
string thMs = "";
double gndAlt = -1;
double dED = -1;
List<string> memList = new List<string>();
string nxPh = "";
string appMode = "";
int depDi = -1;
Vector3D goDP;
List<Vector3D> viaQ = new List<Vector3D>();
List<string> viaN = new List<string>();
int viaI = 0;
string dbDN = "";
bool mOn = false;
int mIdx = 0;
int mSub = 0;
int mSubIdx = 0;
List<string> mNm = new List<string>();
List<Vector3D> mPt = new List<Vector3D>();
List<bool> mMem = new List<bool>();
bool cDn = false;
double cRG = 2.0;
double cPG = 2.0;
double cYG = 1.0;
double cTG = 3.0;
double cTm = 0;
double dlt;
Vector3D cStF;
Vector3D cStU;
Vector3D cStR;
bool cBr = false;
double cCT = 0;
double cLA = 0;
double cBA = 0;
Vector3D cSF;
bool cOn = false;
double cVU = 0;
double cVD = 0;
double cVSA = 0;
double cVLA = 0;
double cMs = 0;
double cGL = 0;
double cFD = 0;
double cBD = 0;
double cTP = 0;
double cTY = 0;
Vector3D cFSP;
Vector3D cFLP;
string uSt = "";
double csTimer = 0;
Vector3D csLastPos;
Vector3D csLastFwd;
Vector3D csLastUp;
double dGA = -1;
List<IMyThrust>[] tG = new List<IMyThrust>[6];
double[] thV = new double[6];
double[] gyV = new double[3];
const double R2D = 180.0 / Math.PI;
const StringComparison SC = StringComparison.OrdinalIgnoreCase;
static double Abs(double v){return Math.Abs(v);}
static double VD(Vector3D a, Vector3D b){return Vector3D.Dot(a,b);}
static Vector3D VN(Vector3D v){return Vector3D.Normalize(v);}
static Vector3D VX(Vector3D a, Vector3D b){return Vector3D.Cross(a,b);}
bool TP(string s,out double v){return double.TryParse(s,out v);}
public Program() {
 string name = Me.CustomName;
 int s = name.IndexOf('[');
 int e = name.IndexOf(']');
 if (s >= 0 && e > s)
 tag = name.Substring(s, e - s + 1);
 for (int i = 0; i < 6; i++)
 { dCam[i] = new List<IMyCameraBlock>(); tG[i] = new List<IMyThrust>(); }
 Runtime.UpdateFrequency = UpdateFrequency.Update10;
 ParseCustomData();
 Refresh();
 wCon = IsConnectorConnected();
 wGr = IsGearLocked();
 wRch = IsAnyRecharging();
 if (Storage.Length > 0) {
 string[] p = Storage.Split(';');
 int di; double dx, dy, dz;
 if (p.Length >= 4 && int.TryParse(p[0], out di)
  && TP(p[1], out dx) && TP(p[2], out dy) && TP(p[3], out dz)) {
  dDi = di; dT = new Vector3D(dx, dy, dz);
  double ax, ay, az;
  if (p.Length >= 7 && TP(p[4], out ax) && TP(p[5], out ay) && TP(p[6], out az))
  { dAD = new Vector3D(ax, ay, az); dAD = dAD.LengthSquared() < 0.5 ? Vector3D.Zero : VN(dAD); }
  double fx, fy, fz;
  if (p.Length >= 10 && TP(p[7], out fx) && TP(p[8], out fy) && TP(p[9], out fz))
  { dSF = new Vector3D(fx, fy, fz); dSF = dSF.LengthSquared() < 0.5 ? Vector3D.Zero : VN(dSF); }
  double cr, cp, cy, ct;
  if (p.Length >= 15 && TP(p[10], out cr) && TP(p[11], out cp)
  && TP(p[12], out cy) && TP(p[13], out ct)) {
  cRG = cr; cPG = cp; cYG = cy; cTG = ct; cDn = (p[14] == "1");
  double v1, v2;
  if (p.Length >= 17 && TP(p[15], out v1) && TP(p[16], out v2)) { cVU = v1; cVD = v2; }
  if (p.Length >= 19 && TP(p[17], out v1) && TP(p[18], out v2)) { cMs = v1; cGL = v2; }
  if (p.Length >= 20 && TP(p[19], out v1)) dGA = v1;
  double tp, ty, fd, bd;
  if (p.Length >= 24 && TP(p[20], out tp) && TP(p[21], out ty)
   && TP(p[22], out fd) && TP(p[23], out bd))
  { cTP = tp; cTY = ty; cFD = fd; cBD = bd; }
  if(p.Length>=25){string mb=p[24];for(int si=25;si<p.Length;si++)mb+=";"+p[si];
  string[] ml=mb.Split('|');for(int mi=0;mi<ml.Length&&memList.Count<10;mi++)if(ml[mi].Length>0)memList.Add(ml[mi]);} } } }
 {string cd=Me.CustomData;int ms=cd.IndexOf("[MEMORY]");
 if(ms>=0){string mp=cd.Substring(ms+8).Trim();int nl=mp.IndexOf('\n');
  if(nl>=0)mp=mp.Substring(0,nl).Trim();
  if(mp.Length>0){string[] ml=mp.Split('|');for(int mi=0;mi<ml.Length&&memList.Count<10;mi++){string em=ml[mi];if(em.Length>0){string en;Vector3D ep,ea;double ee;int edi;if(MemDes(em,out en,out ep,out ea,out ee,out edi)&&FindMem(en)<0)memList.Add(em);}}}}}
 Echo("Tag: " + (tag.Length > 0 ? tag : "(none)") + " mem:" + memList.Count); } 
public void Save() {
 if (dDi >= 0)
 Storage = dDi + ";" + dT.X + ";" + dT.Y + ";" + dT.Z
  + ";" + dAD.X + ";" + dAD.Y + ";" + dAD.Z
  + ";" + dSF.X + ";" + dSF.Y + ";" + dSF.Z
  + ";" + cRG + ";" + cPG + ";" + cYG + ";" + cTG
  + ";" + (cDn ? "1" : "0") + ";" + cVU + ";" + cVD
  + ";" + cMs + ";" + cGL
  + ";" + dGA
  + ";" + cTP + ";" + cTY + ";" + cFD + ";" + cBD
  + ";" + string.Join("|", memList);
 else
 Storage = "";
 string cd = Me.CustomData; int ms = cd.IndexOf("[MEMORY]");
 if (ms >= 0) cd = cd.Substring(0, ms).TrimEnd();
 if (memList.Count > 0) cd += "\n[MEMORY]\n" + string.Join("|", memList);
 Me.CustomData = cd; }
void ParseCustomData() {
 psI.Clear();
 psT.Clear();
 string cd = Me.CustomData;
 if (cd.Length == 0) return;
 string[] lines = cd.Split('\n');
 for (int i = 0; i < lines.Length; i++) {
 string line = lines[i].Trim();
 if (line.Length >= 9 && line.StartsWith("screen", SC)) {
  int eq = line.IndexOf('=');
  if (eq < 0) continue;
  string idxPart = line.Substring(6, eq - 6).Trim();
  string typePart = line.Substring(eq + 1).Trim().ToLower();
  int idx;
  if (!int.TryParse(idxPart, out idx)) continue;
  int typeId = -1;
  if (typePart == "ideb" || typePart == "debug") typeId = 0;
  else if (typePart == "i0" || typePart == "speed") typeId = 1;
  else if (typePart == "i1" || typePart == "cargo") typeId = 2;
  else if (typePart == "i2" || typePart == "battery" || typePart == "bat") typeId = 3;
  else if (typePart == "ilog" || typePart == "log" || typePart == "context") typeId = 4;
  if (typeId < 0) continue;
  psI.Add(idx);
  psT.Add(typeId); } } }
void Refresh() {
 cons.Clear(); grs.Clear(); bats.Clear(); tks.Clear(); thrs.Clear();
 lts.Clear(); cks.Clear(); cgs.Clear(); gys.Clear(); pnl.Clear();
 rcB = null;
 for (int i = 0; i < 6; i++) { dCam[i].Clear(); tG[i].Clear(); }
 var all = new List<IMyTerminalBlock>();
 GridTerminalSystem.GetBlocksOfType(all);
 for (int i = 0; i < all.Count; i++) {
 if (!MatchTag(all[i])) continue;
 var b = all[i];
 if (b is IMyShipConnector) cons.Add((IMyShipConnector)b);
 else if (b is IMyLandingGear) grs.Add((IMyLandingGear)b);
 else if (b is IMyBatteryBlock) bats.Add((IMyBatteryBlock)b);
 else if (b is IMyGasTank) tks.Add((IMyGasTank)b);
 else if (b is IMyThrust) thrs.Add((IMyThrust)b);
 else if (b is IMyLightingBlock) lts.Add((IMyLightingBlock)b);
 else if (b is IMyCockpit) cks.Add((IMyCockpit)b);
 else if (b is IMyCargoContainer) cgs.Add((IMyCargoContainer)b);
 else if (b is IMyGyro) gys.Add((IMyGyro)b);
 else if (b is IMyTextPanel) pnl.Add((IMyTextPanel)b);
 else if (b is IMyRemoteControl && rcB == null) rcB = (IMyRemoteControl)b;
 else if (b is IMyCameraBlock) {
  for (int d = 0; d < 6; d++)
  if (b.CustomName.Contains(dNm[d]))
  { dCam[d].Add((IMyCameraBlock)b); ((IMyCameraBlock)b).EnableRaycast = true; break; } } }
 CTG(); }
bool MatchTag(IMyTerminalBlock block) {
 if (tag.Length == 0) return true;
 return block.CustomName.Contains(tag); }
bool IsIgnored(IMyTerminalBlock block) {
 return block.CustomName.IndexOf("ignore", SC) >= 0; }
bool IsConnectorConnected() {
 for (int i = 0; i < cons.Count; i++)
 if (cons[i].Status == MyShipConnectorStatus.Connected) return true;
 return false; }
bool IsGearLocked() {
 for (int i = 0; i < grs.Count; i++)
 if (grs[i].IsLocked) return true;
 return false; }
bool IsAnyRecharging() {
 for (int i = 0; i < bats.Count; i++)
 if (bats[i].ChargeMode == ChargeMode.Recharge) return true;
 return false; }
void ToggleShip(bool lk) {
 for (int i = 0; i < thrs.Count; i++)
 { if (IsIgnored(thrs[i])) continue; thrs[i].Enabled = !lk; }
 for (int i = 0; i < tks.Count; i++) tks[i].Stockpile = lk;
 for (int i = 0; i < lts.Count; i++)
 { if (IsIgnored(lts[i])) continue; lts[i].Enabled = !lk; }
 for (int i = 0; i < bats.Count; i++)
 { if (IsIgnored(bats[i])) continue; bats[i].ChargeMode = lk ? ChargeMode.Recharge : ChargeMode.Discharge; }
 if (lk) { for (int i = 0; i < grs.Count; i++) grs[i].Lock(); for (int i = 0; i < cons.Count; i++) cons[i].Connect(); }
 else if (!isRT) { isRT = true; ulSt = 1; timer = 0; CTG(); } }
void ETh(){for(int i=0;i<thrs.Count;i++){if(!IsIgnored(thrs[i]))thrs[i].Enabled=true;}}
string ChkReq(bool nd, bool nk, bool ng, bool nt) {
 if (nd && dDi < 0) return "ERR: no target. Use 'set' first";
 if (nk && cks.Count == 0) return "ERR: no cockpit";
 if (ng && gys.Count == 0) return "ERR: no gys";
 if (nt && thrs.Count == 0) return "ERR: no thrs";
 return ""; }
public void Main(string argument, UpdateType updateSource) {
 dlt = Runtime.TimeSinceLastRun.TotalSeconds;
 if (argument == "lock") {
 ToggleShip(true);
 skDt = 3;
 Echo("=== MANUAL LOCK ==="); }
 else if (argument == "unlock") {
 ToggleShip(false);
 skDt = 3;
 Echo("=== MANUAL UNLOCK ==="); }
 else if (argument == "dock stop" || argument == "stop") {
 if (tGy.Length > 0) { tGy = ""; RG(); }
 SAD(); mOn = false;
 dMs = "STOPPED"; }
 else if (argument == "calibration")
 { string e = ChkReq(false, true, true, false); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  cDn = false;
  cOn = true;
  dAc = true;
  dEl = 0;
  dPh = "CAL_HP";
  uSt = "";
  cTm = 0;
  cBr = false;
  cCT = 0;
  cLA = 0;
  dMs = "CAL START"; } }
 else if (argument.StartsWith("pitch") || argument.StartsWith("roll") || argument.StartsWith("yaw"))
 { string e = ChkReq(false, true, true, false); if (e.Length > 0) dMs = e; else {
  string[] parts = argument.Split(' ');
  string axis = parts[0];
  double angle = 180;
  if (parts.Length >= 2) double.TryParse(parts[1], out angle);
  angle = ClampD(angle, -360, 360);
  if (dAc) SAD();
  tGy = axis;
  tGA = angle;
  MatrixD mat = cks[0].WorldMatrix;
  tIF = mat.Forward;
  tIU = mat.Up;
  dMs = "TEST " + axis.ToUpper() + " " + S0(angle) + "°"; } }
 else if (argument.StartsWith("fwd") || argument.StartsWith("bwd")
 || argument.StartsWith("left") || argument.StartsWith("right")
 || argument.StartsWith("up") || argument.StartsWith("down"))
 { string e = ChkReq(false, true, true, true); if (e.Length > 0) dMs = e; else {
  string cmd; Vector3D dir; MatrixD mat = cks[0].WorldMatrix;
  Vector3D gv = cks[0].GetNaturalGravity();
  bool hg = gv.LengthSquared() > 0.01;
  Vector3D gd = hg ? VN(gv) : Vector3D.Zero;
  if (argument.StartsWith("fwd"))       { cmd = "FWD";   dir = mat.Forward;  }
  else if (argument.StartsWith("bwd"))   { cmd = "BWD";   dir = mat.Backward; }
  else if (argument.StartsWith("left"))  { cmd = "LEFT";  dir = mat.Left;     }
  else if (argument.StartsWith("right")) { cmd = "RIGHT"; dir = mat.Right;    }
  else if (argument.StartsWith("up"))    { cmd = "UP";    dir = mat.Up;       }
  else                                   { cmd = "DOWN";  dir = mat.Down;     }
  if (hg && cmd != "UP" && cmd != "DOWN" && cmd != "FWD")
  { dir = dir - gd * VD(dir, gd); if (dir.LengthSquared() > 0.001) dir = VN(dir); else dir = mat.Forward; }
  int cmdLen = cmd == "RIGHT" ? 5 : cmd == "LEFT" || cmd == "DOWN" ? 4 : 3;
  string args = argument.Substring(cmdLen).Trim();
  string[] parts = args.Split(',');
  string locStr = parts.Length > 0 ? parts[0].Trim() : "";
  double spdArg = 100;
  if (parts.Length >= 2) double.TryParse(parts[1].Trim(), out spdArg);
  spdArg = Math.Max(0.5, spdArg);
  Vector3D origin = CP();
  Vector3D target = Vector3D.Zero;
  string label = "";
  string phase = "FLY_FWD";
  double dist;
  if (double.TryParse(locStr, out dist)) {
  dist = ClampD(Abs(dist), 1, 10000);
  if (cmd == "FWD" && hg) {
   Vector3D pc = Vector3D.Zero; bool hasPl = rcB != null && rcB.TryGetPlanetPosition(out pc);
   if (hasPl) {
   Vector3D up2 = VN(origin - pc);
   double vertComp = VD(dir, up2);
   Vector3D hzDir = dir - up2 * vertComp;
   if (hzDir.LengthSquared() > 0.001) {
    hzDir = VN(hzDir);
    double hzDist = dist * Math.Sqrt(Math.Max(0, 1 - vertComp * vertComp));
    double altDiff = dist * vertComp;
    double R = (origin - pc).Length();
    double theta = hzDist / R;
    Vector3D bp = pc + (up2 * Math.Cos(theta) + hzDir * Math.Sin(theta)) * R;
    Vector3D tUp = VN(bp - pc);
    target = bp + tUp * altDiff; }
   else { target = origin + dir * dist; } }
   else { target = origin + dir * dist; } }
  else { target = origin + dir * dist; }
  label = cmd + " " + S0(dist) + "m";
  if (cmd == "FWD") phase = "FLY_RC"; }
  else if (locStr.StartsWith("Entry", SC)) {
  string entryArg = locStr.Length > 5 ? locStr.Substring(5).Trim() : "";
  if (entryArg.Length == 0) {
   if (dDi < 0) { dMs = "ERR: no target. Use 'set' first"; return; }
   CEP();
   if (dW.LengthSquared() < 0.01) { dMs = "ERR: entry point is zero"; return; }
   target = dW; }
  else { Vector3D gp; string gn;
   if (ParseGPS(entryArg, out gp, out gn)) { target = gp; }
   else { int mi = FindMem(entryArg);
   if (mi < 0) { dMs = "ERR: memory not found: " + entryArg; return; }
   string mn; Vector3D mp, ma; double me; int mdi;
   MemDes(memList[mi], out mn, out mp, out ma, out me, out mdi);
   dT = mp; dAD = ma; dED = me; dDi = mdi >= 0 ? mdi : DirFromAD(); CEP(); target = dW; } }
  label = "FWD Entry";
  phase = "FLY_RC"; }
  else if (string.Equals(locStr, "Home", SC)) {
  if (dDi < 0) { dMs = "ERR: no target. Use 'set' first"; return; }
  label = cmd + " Home";
  dist = (dT - origin).Length();
  if (cmd == "BWD") {
   target = origin + VN(origin - dT) * 100;
   dist = 0;
   phase = "FLY_BWD_HD"; }
  else if (cmd == "FWD") {
   target = dT;
   phase = "FLY_FWD_HD"; }
  else {
   target = origin + dir * dist; } }
  else {
  dMs = "ERR: usage: " + cmd.ToLower() + " [meters],[speed]";
  return; }
  if (dAc) SAD();
  fTg = target; dbTg = target;
  if (cmd == "FWD" || cmd == "BWD") fSF = spdArg;
  else if (cmd == "LEFT" || cmd == "RIGHT") fSL = spdArg;
  else fSU = spdArg;
  if (fAltPre) fSU = 100;
  fAD = (!fAltPre && dist > 0 && dist <= 30) ? 0.3 : 3.0;
  fAp = (!fAltPre && dist > 0 && dist <= 30);
  fAT = 0;
  fSP = origin;
  fLb = cmd;
  fRC = 0;
  REv();
  dAc = true;
  dEl = 0;
  dPh = phase;
  uSt = "";
  CT(); ETh();
  dMs = label + " max=" + S0(spdArg) + "m/s"; } }
 else if (argument == "checkstop")
 { string e = ChkReq(false, true, false, false); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  dAc = true;
  dEl = 0;
  dPh = "TEST_CS";
  uSt = "";
  RCS();
  RG();
  dMs = "TEST CheckStop"; } }
 else if (argument == "hp")
 { string e = ChkReq(false, true, true, false); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  dAc = true;
  dEl = 0;
  dPh = "TEST_HP";
  uSt = "";
  dMs = "TEST HorizontalPosture"; } }
 else if (argument == "head home")
 { string e = ChkReq(true, true, true, false); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  dAc = true;
  dEl = 0;
  dPh = "TEST_HD_HOME";
  uSt = "";
  dMs = "TEST Head Home"; } }
 else if (argument == "head entry")
 { string e = ChkReq(true, true, true, true); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  CEP();
  if (dW.LengthSquared() < 0.01)
  { dMs = "ERR: entry point is zero. Use 'set' first"; }
  else {
  dAc = true;
  dEl = 0;
  fTg = dW;
  dPh = "TEST_HD_ENTRY";
  uSt = "";
  dMs = "Head Entry"; } } }
 else if (argument == "heading")
 { string e = ChkReq(true, true, true, false); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  dAc = true;
  dEl = 0;
  dPh = "TEST_HD";
  uSt = "";
  dMs = "TEST Heading"; } }
 else if (argument == "checkwait")
 { string e = ChkReq(false, true, false, false); if (e.Length > 0) dMs = e; else {
  string reason = NeedRecalibration();
  if (reason.Length > 0)
  dMs = "RECAL: " + reason;
  else {
  double curMass = cks[0].CalculateShipMass().TotalMass;
  double curGrav = cks[0].GetNaturalGravity().Length();
  double massPct = (cMs > 0) ? Abs(curMass - cMs) / cMs * 100 : 0;
  double gravPct = (cGL > 0) ? Abs(curGrav - cGL) / cGL * 100 : 0;
  dMs = "NO CHANGE m=" + S1(massPct) + "% g=" + S1(gravPct) + "%"; } } }
 else if (argument == "dep") {
 string e = ChkReq(false, true, true, true); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  fLb = "DEP";
  if (!InitDep()) {
  if (nxPh.Length > 0) { dAc = true; dEl = 0; NextPhase(); }
  else dMs = "DEP: already safe"; }
  else dMs = "DEP"; } }
 else if (argument.StartsWith("app ") || argument == "app") {
 string appArg = argument.Length > 4 ? argument.Substring(4).Trim().ToUpper() : "";
 if (appArg != "DK" && appArg != "LAND" && appArg != "CON") { dMs = "ERR: app DK|LAND|CON"; }
 else { string e = ChkReq(false, true, true, true); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  appMode = appArg;
  dAc = true; dEl = 0; fLb = "APP";
  CT(); ETh();
  if (appArg == "DK" || appArg == "CON") {
  dCn = true; dSt = 1; uSt = "";
  IDS(); }
  else {
  dPh = "GO_HP"; uSt = ""; }
  dMs = "APP " + appArg; } } }
 else if (argument == "dock") {
 if (dDi < 0)
  dMs = "ERR: no target. Use 'set <Dir>' first";
 else if (cons.Count == 0)
  dMs = "ERR: no cons";
 else { string e = ChkReq(false, true, true, true); if (e.Length > 0) dMs = e; else {
  if (dAc) SAD();
  dbDN = dNm[dDi]; dbTg = dT;
  appMode = "DK"; nxPh = "FWD_E|APP_DK"; fLb = "DOCK";
  CT(); ETh();
  for (int i = 0; i < cons.Count; i++) cons[i].Enabled = true;
  if (!InitDep()) { dAc = true; dEl = 0; NextPhase(); }
  dMs = "DOCK " + dNm[dDi]; } } }
 else if (argument == "fit") {
 if (dDi < 0)
  dMs = "ERR: no target. Use 'set <Dir>' first";
 else if (cks.Count == 0)
  dMs = "ERR: no cks";
 else if (thrs.Count == 0)
  dMs = "ERR: no thrs";
 else {
  dAc = true;
  dEl = 0;
  CEP();
  CT(); ETh();
  dSt = 1;
  uSt = "";
  IDS();
  dMs = "FIT " + dNm[dDi]; } }
 else if (argument == "memory" || argument.StartsWith("memory ")) {
 string sub = argument.Length > 7 ? argument.Substring(7).Trim() : "";
 if (sub == "list") {
  if (memList.Count == 0) { SetErr("memory: empty"); }
  else { string msg = "memory "+memList.Count+"/10:"; for (int i=0;i<memList.Count;i++) { string mn; Vector3D mp,ma; double me; int mdi; if (MemDes(memList[i],out mn,out mp,out ma,out me,out mdi)) msg+="\n["+i+"] "+mn; } dMs=msg; } }
 else if (sub == "clear") { memList.Clear(); dMs = "memory: cleared"; }
 else if (sub.Length == 0) { SetErr("ERR: usage: memory (name)"); }
 else {
  if (string.Equals(sub, "home", SC) || sub == "0") { SetErr("ERR: home is set by 'set' cmd"); }
  else {
  IMyShipConnector mc0=null;for(int ci=0;ci<cons.Count;ci++)if(cons[ci].Status==MyShipConnectorStatus.Connected&&cons[ci].OtherConnector!=null){mc0=cons[ci];break;}
  Vector3D mPos; Vector3D mAD; double mED;
  if (mc0 != null) {
   IMyShipConnector oc = mc0.OtherConnector;
   mPos = oc.GetPosition();
   mAD = VN(-oc.WorldMatrix.Forward);
   mED = dED > 0 ? dED : (dDi >= 0 ? dockEntryDist[dDi] : 15.0); }
  else {
   mPos = CP();
   mAD = cks.Count > 0 ? cks[0].WorldMatrix.Up : Me.WorldMatrix.Up;
   mED = dockEntryDist[4]; }
  Vector3D svAD = dAD; dAD = mAD; int mDi = DirFromAD(); dAD = svAD;
  string entry = MemSer(sub, mPos, mAD, mED, mDi);
  int ex = FindMem(sub);
  if (ex >= 0) memList[ex] = entry;
  else { if (memList.Count >= 10) memList.RemoveAt(0); memList.Add(entry); }
  dMs = "memory: saved [" + sub + "] " + (mc0 != null ? "con" : "nocon") + " (" + memList.Count + "/10)"; } } }
 else if (argument.StartsWith("go ") || argument.StartsWith("land ")
 || argument == "connect" || argument.StartsWith("connect ")) {
 bool isGo = argument.StartsWith("go ");
 bool isLand = argument.StartsWith("land ");
 bool isCon = argument == "connect" || argument.StartsWith("connect ");
 if (isCon && argument == "connect") { SetErr("ERR: usage: connect (name)"); }
 else { string ce = ChkReq(false, true, true, true); if (ce.Length > 0) { SetErr(ce); } else {
  Vector3D destP = Vector3D.Zero; string destN = ""; bool destOk = false;
  if (isCon) {
  string cn = argument.Substring(8).Trim();
  int mi = FindMem(cn);
  if (mi < 0) { SetErr("ERR: memory not found: " + cn); }
  else { string mn; Vector3D mp, ma; double me; int mdi; MemDes(memList[mi], out mn, out mp, out ma, out me, out mdi);
   dT = mp; dAD = ma; dED = me; dDi = mdi >= 0 ? mdi : DirFromAD(); CEP(); destP = dW; destN = mn; destOk = true; } }
  else {
  string gs = isGo ? argument.Substring(3).Trim() : argument.Substring(5).Trim();
  Vector3D gp; if (!ParseGPS(gs, out gp, out destN)) { SetErr("ERR: invalid GPS"); } else { destP = gp; destOk = true; } }
  if (destOk) {
  if (dAc) SAD(); viaI = 0; viaQ.Clear(); viaN.Clear();
  dbDN = destN; goDP = destP; dbTg = destP;
  appMode = isCon ? "CON" : isLand ? "LAND" : "";
  nxPh = isCon ? "FWD_E|APP_CON" : isLand ? "FWD_E|APP_LAND" : "FWD_E|GO_HP";
  fLb = isGo ? "GO" : isLand ? "LAND" : "CONNECT";
  CT(); ETh();
  if (!InitDep()) { dAc = true; dEl = 0; NextPhase(); }
  dMs = fLb + " -> " + destN; }
 }} }
 else if (argument == "imenu") {
 if (mOn) { if (mSub > 0) { mSub = 0; } else { mOn = false; mSub = 0; dMs = "Menu closed"; } }
 else {
  mOn = true; mIdx = 0; mSub = 0; mSubIdx = 0;
  mNm.Clear(); mPt.Clear(); mMem.Clear();
  string[] cdL = Me.CustomData.Split('\n');
  for (int ci = 0; ci < cdL.Length; ci++) {
  Vector3D gp; string gn;
  if (ParseGPS(cdL[ci].Trim(), out gp, out gn))
  { mNm.Add(gn); mPt.Add(gp); mMem.Add(false); } }
  for (int mi = 0; mi < memList.Count; mi++) {
  string mn; Vector3D mp, ma; double me; int mdi;
  if (MemDes(memList[mi], out mn, out mp, out ma, out me, out mdi))
  { mNm.Add(mn); mPt.Add(mp); mMem.Add(true); } }
  dMs = "Menu: " + mNm.Count + " items"; } }
 else if (argument == "u" && mOn) {
 if (mSub > 0) { if (mSubIdx > 0) mSubIdx--; else mSubIdx = mSub == 1 ? 3 : 2; }
 else if (mNm.Count > 0) { mIdx = mIdx > 0 ? mIdx - 1 : mNm.Count - 1; } }
 else if (argument == "d" && mOn) {
 if (mSub > 0) { int mx = mSub == 1 ? 3 : 2; mSubIdx = mSubIdx < mx ? mSubIdx + 1 : 0; }
 else if (mNm.Count > 0) { mIdx = mIdx < mNm.Count - 1 ? mIdx + 1 : 0; } }
 else if (argument == "e" && mOn) {
 if (mSub == 1) {
  if (mSubIdx == 3) { mSub = 0; }
  else if (mSubIdx == 2) {
  if (viaQ.Count < 5) { viaQ.Add(mPt[mIdx]); viaN.Add(mNm[mIdx]); }
  mSub = 0; dMs = "Via+" + mNm[mIdx] + " (" + viaQ.Count + "/5)"; }
  else {
  string sn = mNm[mIdx]; Vector3D sp = mPt[mIdx];
  mOn = false; mSub = 0;
  string ce = ChkReq(false, true, true, true);
  if (ce.Length > 0) { SetErr(ce); }
  else {
   if (dAc) SAD(); viaI = 0;
   if (mSubIdx == 0) {
   dbDN = sn; goDP = sp; dbTg = sp; appMode = ""; nxPh = "FWD_E|GO_HP"; fLb = "GO";
   CT(); ETh();
   if (!InitDep()) { dAc = true; dEl = 0; NextPhase(); }
   dMs = "GO -> " + sn; }
   else {
   dbDN = sn; goDP = sp; dbTg = sp; appMode = "LAND"; nxPh = "FWD_E|APP_LAND"; fLb = "LAND";
   CT(); ETh();
   if (!InitDep()) { dAc = true; dEl = 0; NextPhase(); }
   dMs = "LAND -> " + sn; } } } }
 else if (mSub == 2) {
  if (mSubIdx == 2) { mSub = 0; }
  else if (mSubIdx == 1) {
  string sn = mNm[mIdx];
  int fi = FindMem(sn);
  if (fi >= 0) { string mn; Vector3D mp, ma; double me; int mdi;
   MemDes(memList[fi], out mn, out mp, out ma, out me, out mdi);
   if (viaQ.Count < 5) { viaQ.Add(mp); viaN.Add(mn); } }
  mSub = 0; dMs = "Via+" + sn + " (" + viaQ.Count + "/5)"; }
  else {
  string sn = mNm[mIdx];
  mOn = false; mSub = 0;
  string ce = ChkReq(false, true, true, true);
  if (ce.Length > 0) { SetErr(ce); }
  else {
   if (dAc) SAD(); viaI = 0;
   int fi = FindMem(sn);
   if (fi >= 0) {
   string mn; Vector3D mp, ma; double me; int mdi;
   MemDes(memList[fi], out mn, out mp, out ma, out me, out mdi);
   dT = mp; dAD = ma; dED = me; dDi = mdi >= 0 ? mdi : DirFromAD(); CEP();
   dbDN = sn; goDP = dW; dbTg = dT; appMode = "CON";
   nxPh = "FWD_E|APP_CON"; fLb = "CONNECT";
   CT(); ETh();
   if (!InitDep()) { dAc = true; dEl = 0; NextPhase(); }
   dMs = "CONNECT -> " + sn; }
   else { dMs = "ERR: mem not found"; } } } }
 else if (mNm.Count > 0 && mIdx < mNm.Count) {
  mSub = mMem[mIdx] ? 2 : 1; mSubIdx = 0; } }
 else if (argument.Length > 0) {
 string setDir = "";
 if (argument.StartsWith("set "))
  setDir = argument.Substring(4).Trim();
 else if (argument.StartsWith("set") && argument.Length > 3)
  setDir = argument.Substring(3).Trim();
 if (setDir.Length == 0 && string.Equals(argument, "set", SC)) {
  if (!IsConnectorConnected()) { dMs = "ERR: not connected"; }
  else {
  int autoDir = -1;
  IMyShipConnector activeCon = null;
  for (int ci = 0; ci < cons.Count; ci++) {
  IMyShipConnector mc = cons[ci];
  if (mc.Status == MyShipConnectorStatus.Connected
   || mc.Status == MyShipConnectorStatus.Connectable)
  { activeCon = mc; break; } }
  if (activeCon == null && cons.Count > 0) activeCon = cons[0];
  if (activeCon != null) {
  string myName = activeCon.CustomName;
  for (int d = 0; d < 6; d++) {
   if (myName.IndexOf(dNm[d], SC) >= 0)
   { autoDir = d; break; } }
  if (autoDir < 0 && (activeCon.Status == MyShipConnectorStatus.Connected
   || activeCon.Status == MyShipConnectorStatus.Connectable)
   && activeCon.OtherConnector != null) {
   Vector3D otherFwd = -activeCon.OtherConnector.WorldMatrix.Forward;
   MatrixD mat = cks.Count > 0 ? cks[0].WorldMatrix : activeCon.WorldMatrix;
   double dotF = VD(otherFwd, mat.Forward);
   double dotR = VD(otherFwd, mat.Right);
   double dotU = VD(otherFwd, mat.Up);
   double absF = Abs(dotF), absR = Abs(dotR), absU = Abs(dotU);
   if (absF >= absR && absF >= absU)
   autoDir = dotF > 0 ? 0 : 1;
   else if (absR >= absU)
   autoDir = dotR > 0 ? 3 : 2;
   else
   autoDir = dotU > 0 ? 4 : 5; } }
  if (autoDir >= 0)
  setDir = dNm[autoDir];
  else
  dMs = "ERR: no connector or name"; } }
 bool found = false;
 for (int d = 0; d < 6; d++) {
  if (setDir.Length > 0 && string.Equals(dNm[d], setDir, SC)) {
  if (!IsConnectorConnected()) { dMs = "ERR: set requires connected"; found = true; break; }
  dDi = d;
  dT = cons.Count > 0 ? cons[0].GetPosition()
   : (cks.Count > 0 ? cks[0].GetPosition() : Me.GetPosition());
  dAD = Vector3D.Zero;
  if (cks.Count > 0) {
   MatrixD mat = cks[0].WorldMatrix;
   Vector3D grav = cks[0].GetNaturalGravity();
   bool hasGrav = grav.LengthSquared() > 0.01;
   Vector3D gravDir = hasGrav ? VN(grav) : Vector3D.Zero;
   if (d == 4) { dAD = hasGrav ? -gravDir : mat.Up; }
   else if (d == 5) { dAD = hasGrav ? gravDir : mat.Down; }
   else {
   if (cons.Count > 0) {
    IMyShipConnector mc = cons[0];
    if ((mc.Status == MyShipConnectorStatus.Connected
    || mc.Status == MyShipConnectorStatus.Connectable)
    && mc.OtherConnector != null) {
    dAD = -mc.OtherConnector.WorldMatrix.Forward; } }
   if (dAD.LengthSquared() < 0.5) {
    switch (d) {
    case 0: dAD = mat.Forward;  break;
    case 1: dAD = mat.Backward; break;
    case 2: dAD = mat.Left;     break;
    case 3: dAD = mat.Right;    break;
    default: dAD = mat.Forward; break; } } } }
  if (cks.Count > 0) {
   Vector3D grav = cks[0].GetNaturalGravity();
   bool hasGrav = grav.LengthSquared() > 0.01;
   Vector3D fwd = cks[0].WorldMatrix.Forward;
   if (hasGrav) {
   Vector3D gravDir = VN(grav);
   fwd = fwd - gravDir * VD(fwd, gravDir);
   if (fwd.LengthSquared() > 0.001) fwd = VN(fwd);
   else fwd = cks[0].WorldMatrix.Forward; }
   dSF = fwd; }
  else
   dSF = Vector3D.Zero;
  dGA = ScanDist(5, 200);
  if (dGA >= 200) dGA = -1;
  dED = -1;
  if (cons.Count > 0) {
   IMyShipConnector mc = cons[0];
   if ((mc.Status == MyShipConnectorStatus.Connected
   || mc.Status == MyShipConnectorStatus.Connectable)
   && mc.OtherConnector != null) {
   double parsed = ParseEntryDist(mc.OtherConnector.CustomName);
   if (parsed > 0) dED = parsed; } }
  found = true;
  CEP();
  if(dAD.LengthSquared()>0.5){string he=MemSer("home",dT,dAD,dED>0?dED:dockEntryDist[d],d);int hi=FindMem("home");if(hi>=0)memList[hi]=he;else{if(memList.Count>=10)memList.RemoveAt(0);memList.Insert(0,he);}}
  dMs = "SET " + dNm[d]
   + (dED > 0 ? " E=" + S0(dED) + "m" : "")
   + (dGA > 0 ? " G=" + S1(dGA) : "");
  break; } }
 if (setDir.Length > 0 && !found)
  dMs = "ERR: unknown dir '" + setDir + "'"; }
 bool rechNow = IsAnyRecharging();
 if (wRch && !rechNow) {
 ToggleShip(false);
 skDt = 3;
 Echo("=== RECHARGE OFF ==="); }
 bool conNow = IsConnectorConnected();
 bool gearNow = IsGearLocked();
 if (skDt > 0) {
 skDt--; }
 else {
 if (!wCon && conNow) {
  if (dAc) SAD();
  ToggleShip(true);
  Echo("=== CONNECTOR LOCKED ==="); }
 if (wCon && !conNow) {
  ToggleShip(false);
  Echo("=== CONNECTOR UNLOCKED ==="); }
 if (!wGr && gearNow) {
  if (dAc) SAD();
  ToggleShip(true);
  Echo("=== GEAR/MAG LOCKED ==="); }
 if (wGr && !gearNow) {
  ToggleShip(false);
  Echo("=== GEAR/MAG UNLOCKED ==="); } }
 if (isRT) {
 timer += dlt;
 if (ulSt == 1 && timer >= 2.0) {
  for (int i = 0; i < grs.Count; i++)
  { grs[i].Unlock(); grs[i].Enabled = false; }
  for (int i = 0; i < cons.Count; i++)
  { cons[i].Disconnect(); cons[i].Enabled = false; }
  ulSt = 2;
  timer = 0;
  Echo("=== Gear/Con UNLOCK ==="); }
 else if (ulSt == 2 && timer >= 3.0) {
  for (int i = 0; i < grs.Count; i++) grs[i].Enabled = true;
  for (int i = 0; i < cons.Count; i++) cons[i].Enabled = true;
  isRT = false;
  ulSt = 0;
  timer = -1;
  conNow = IsConnectorConnected();
  gearNow = IsGearLocked();
  skDt = 3;
  Echo("=== Gear/Mag/Connector ON ==="); }
 else {
  if (ulSt == 1)
  Echo("Thruster warm " + S1(2-timer) + "s");
  else
  Echo("Gear/Con ON in " + S1(3-timer) + "s"); } }
 UpdateDistances();
 if (tGy.Length > 0) UTG();
 else if (dAc) UAD();
 USc();
 Echo("Con:" + (conNow ? "Lock" : "Free") + " Gear:" + (gearNow ? "Lock" : "Free"));
 if (mOn && mNm.Count > 0) Echo("Menu[" + mIdx + "] " + mNm[mIdx]);
 else if (mOn) Echo("Menu: empty");
 if (dMs.Length > 0) Echo("Dock> " + dMs);
 if (dAc) Echo("Ph:" + dPh + (fEvT != 0 ? " Ev:" + fEvT + " d:" + S0(fEvH) : ""));
 if (dAc && cks.Count > 0)
 Echo("Dock: " + (dT - cks[0].GetPosition()).Length().ToString("F1") + "m");
 else if (dDi >= 0)
 Echo("Dock: " + dNm[dDi] + " saved");
 else
 Echo("Dock: not set");
 Echo("Tag:" + (tag.Length > 0 ? tag : "*") + " C:" + cons.Count + " T:" + thrs.Count + " K:" + cks.Count + " P:" + pnl.Count);
 wCon = conNow;
 wGr = gearNow;
 wRch = rechNow; }
void WSurf(IMyTextSurface s, int fi, string txt)
{ s.ContentType = ContentType.TEXT_AND_IMAGE; s.FontSize = (float)sFZ[fi]; s.Alignment = TextAlignment.LEFT; s.WriteText(txt); }
string BuildLog(IMyTextSurface s) {
 float fh = s.FontSize * 28.8f;
 int maxL = Math.Max(3, (int)(s.SurfaceSize.Y / fh));
 while (logBuf.Count > 200) logBuf.RemoveAt(0);
 int start = Math.Max(0, logBuf.Count - maxL);
 var sb = new System.Text.StringBuilder();
 for (int i = start; i < logBuf.Count; i++) {
 if (i > start) sb.Append('\n');
 sb.Append(logBuf[i]); }
 return sb.ToString(); }
void USc() {
 if (cks.Count == 0) return;
 double speed = cks[0].GetShipSpeed();
 double mass = cks[0].CalculateShipMass().TotalMass;
 double totalThrust = 0;
 for (int i = 0; i < thrs.Count; i++)
 totalThrust += thrs[i].MaxEffectiveThrust;
 double stopDist = totalThrust > 0 ? speed * speed * mass / (2.0 * totalThrust) : 0;
 string spd0 = "Spd:" + S1(speed) + "/" + S1(dbSL) + " m/s";
 string remS=dAc?FD((fTg-CP()).Length()):"---";
 string s0Text=spd0+"\nStp:"+S1(stopDist)+"m Rem:"+remS;
 if(dAc&&dbDN.Length>0){int tSt=viaQ.Count+1;
 s0Text+="\n"+fLb+" "+dbDN+" ("+(viaI<viaQ.Count?viaI+1:tSt)+"/"+tSt+")";
 Vector3D cp0=CP();
 if(viaI<viaQ.Count)s0Text+="\nVia:"+viaN[viaI]+"("+FD((ViaAlt(viaQ[viaI])-cp0).Length())+")";
 else s0Text+="\nDirect:"+dbDN+"("+FD((fTg-cp0).Length())+")";}
 else s0Text+="\nNo Destination\n";
 string[] df=new string[6];
 for(int d=0;d<6;d++)df[d]=(dDs[d]>=0?S0(dDs[d]):"--").PadLeft(5);
 string cm=fEvT!=0?"!!":"++";
 double dAl=gndAlt>0?gndAlt:(dDs[5]>=0?dDs[5]:-1);
 string dv=(dAl>=0?S0(dAl):"--").PadLeft(5);
 s0Text+="\n\n    F:"+df[0]+"\n L:"+df[2]+" "+cm+" R:"+df[3]
 +"\n    B:"+df[1]+"\n U:"+df[4]+"  D:"+dv;
 if (fEvT != 0) {
 string evLbl = fEvT == 1 ? "SL" : fEvT == 2 ? "EU" : fEvT == 3 ? "ES" : "EC";
 s0Text += "\n>> " + evLbl + " " + S0(fEvH) + "m"; }
 double curVol = 0;
 double maxVol = 0;
 for (int i = 0; i < cgs.Count; i++) {
 IMyInventory inv = cgs[i].GetInventory();
 curVol += (double)inv.CurrentVolume;
 maxVol += (double)inv.MaxVolume; }
 double cargoPct = maxVol > 0 ? curVol / maxVol * 100.0 : 0;
 List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
 GridTerminalSystem.GetBlocksOfType(allBlocks);
 double totalCur = 0;
 double totalMax = 0;
 List<IMyInventory> allInvs = new List<IMyInventory>();
 for (int i = 0; i < allBlocks.Count; i++) {
 if (!MatchTag(allBlocks[i])) continue;
 for (int n = 0; n < allBlocks[i].InventoryCount; n++) {
  IMyInventory inv = allBlocks[i].GetInventory(n);
  totalCur += (double)inv.CurrentVolume;
  totalMax += (double)inv.MaxVolume;
  allInvs.Add(inv); } }
 double totalPct = totalMax > 0 ? totalCur / totalMax * 100.0 : 0;
 string s1Text = "Cargo " + S0(curVol*1000) + "/" + S0(maxVol*1000) + "L(" + S1(cargoPct) + "%)\n"
 + "Total " + S0(totalCur*1000) + "/" + S0(totalMax*1000) + "L(" + S1(totalPct) + "%)";
 List<string> oreNames = new List<string>();
 List<double> oreAmts = new List<double>();
 for (int i = 0; i < allInvs.Count; i++) {
 List<MyInventoryItem> items = new List<MyInventoryItem>();
 allInvs[i].GetItems(items);
 for (int j = 0; j < items.Count; j++) {
  if (items[j].Type.TypeId == "MyObjectBuilder_Ore") {
  string oreName = items[j].Type.SubtypeId;
  double amt = (double)items[j].Amount;
  int idx = oreNames.IndexOf(oreName);
  if (idx >= 0)
   oreAmts[idx] += amt;
  else {
   oreNames.Add(oreName);
   oreAmts.Add(amt); } } } }
 if (oreNames.Count > 0) {
 s1Text += "\n···· ores ····";
 int iceIdx = oreNames.IndexOf("Ice");
 if (iceIdx >= 0)
  s1Text += "\nIce: " + FormatKg(oreAmts[iceIdx]);
 for (int i = 0; i < oreNames.Count - 1; i++) {
  for (int j = i + 1; j < oreNames.Count; j++) {
  if (string.Compare(oreNames[i], oreNames[j]) > 0) {
   string tp = oreNames[i]; oreNames[i] = oreNames[j]; oreNames[j] = tp;
   double ta = oreAmts[i]; oreAmts[i] = oreAmts[j]; oreAmts[j] = ta; } } }
 for (int i = 0; i < oreNames.Count; i++) {
  if (oreNames[i] == "Ice") continue;
  s1Text += "\n" + oreNames[i] + ": " + FormatKg(oreAmts[i]); } }
 double h2Total = 0; int h2Count = 0;
 double o2Total = 0; int o2Count = 0;
 for (int i = 0; i < tks.Count; i++) {
 string sub = tks[i].BlockDefinition.SubtypeId.ToString();
 if (sub.Contains("Hydrogen"))
 { h2Total += tks[i].FilledRatio; h2Count++; }
 else
 { o2Total += tks[i].FilledRatio; o2Count++; } }
 double h2Pct = h2Count > 0 ? h2Total / h2Count * 100.0 : 0;
 double o2Pct = o2Count > 0 ? o2Total / o2Count * 100.0 : 0;
 double curPow = 0; double maxPow = 0;
 for (int i = 0; i < bats.Count; i++) {
 curPow += bats[i].CurrentStoredPower;
 maxPow += bats[i].MaxStoredPower; }
 double batPct = maxPow > 0 ? curPow / maxPow * 100.0 : 0;
 double netDrain = 0;
 for (int i = 0; i < bats.Count; i++)
 netDrain += bats[i].CurrentOutput - bats[i].CurrentInput;
 string batRemain = "";
 if (netDrain > 0.001 && curPow > 0) {
 double hours = curPow / netDrain;
 if (hours >= 24.0)
  batRemain = " (" + S1(hours/24) + "d)";
 else
  batRemain = " (" + S1(hours) + "h)"; }
 string s2Text = "H2: " + S1(h2Pct) + "%\n"
 + "O2: " + S1(o2Pct) + "%\n"
 + "Bat: " + S1(batPct) + "%" + batRemain;
 for (int c = 0; c < cks.Count; c++) {
 IMyCockpit cockpit = cks[c];
 bool isIndustrial = cockpit.BlockDefinition.SubtypeId.Contains("Industrial");
 int idx0 = isIndustrial ? 1 : 0;
 int idx1 = isIndustrial ? 0 : 1;
 if (cockpit.SurfaceCount > 0) WSurf(cockpit.GetSurface(idx0), 1, s0Text);
 if (cockpit.SurfaceCount > 1) WSurf(cockpit.GetSurface(idx1), 2, s1Text);
 if (cockpit.SurfaceCount > 2) WSurf(cockpit.GetSurface(2), 3, s2Text); }
 dbUT += dlt;
 if (dbUT >= 1.0 && cks.Count > 0) {
 dbUT = 0;
 IMyCockpit ck = cks[0];
 Vector3D cp = CP();
 dbPL = "Current: " + V3S(cp);
 MatrixD m = ck.WorldMatrix;
 Vector3D gv = ck.GetNaturalGravity();
 if (gv.LengthSquared() > 0.01) {
  Vector3D gd = VN(gv);
  Vector3D ag = -gd;
  double pitchA = -CPL(m, gd) * R2D;
  double rollA = -CRE(m, gd, ag) * R2D;
  double yawA = 0;
  if (dDi >= 0) {
  yawA = -CYT(m, dAD, gd, ag) * R2D; }
  dbAL = "Attitude: Y=" + S1(yawA) + " P=" + S1(pitchA) + " R=" + S1(rollA);
  dbHL = DirAttitude(cp, dbTg, m, gd, ag);
  dbEL = DirAttitude(cp, dW, m, gd, ag); }
 else {
  dbAL = "Attitude: no gravity";
  dbHL = "";
  dbEL = ""; } }
 for (int ti = 0; ti < 6; ti++) {
 double sum = 0;
 for (int tj = 0; tj < tG[ti].Count; tj++) sum += tG[ti][tj].CurrentThrust;
 thV[ti] = sum; }
 string dbgText = "";
 string[] tn = {"Front:","Rear :","Left :","Right:","Up   :","Down :"};
 string thDir="";
 for(int i=0;i<6;i++){
 thDir+="\n"+tn[i]+S0(thV[i]).PadLeft(10);
 if(dbApE){double ev=i<2?dbEF:i<4?dbER:dbEU;
 thDir+=" e"+(i<2?"F":i<4?"R":"U")+"="+S2(ev);}}
 if (cks.Count > 0) {
 Vector3D cp = CP();
 MatrixD dm = cks[0].WorldMatrix;
 Vector3D g = cks[0].GetNaturalGravity();
 bool dHG = g.LengthSquared() > 0.01;
 Vector3D posD = dT - cp;
 double dEF = VD(posD, dm.Forward);
 double dER = VD(posD, dm.Right);
 double dEU = dHG ? VD(posD, -VN(g)) : VD(posD, dm.Up);
 if (appMode == "LAND" && gndAlt > 0) dEU = -gndAlt;
 string diffStr = "---";
 if (dDi == 0 || dDi == 1) diffStr = "A=" + S2(dEU) + " S=" + S2(dER);
 else if (dDi == 2 || dDi == 3) diffStr = "A=" + S2(dEU) + " F=" + S2(dEF);
 else if (dDi >= 4) diffStr = "F=" + S2(dEF) + " S=" + S2(dER);
 double curSpd = cks[0].GetShipSpeed();
 string remStr = dAc ? S0((fTg - cp).Length()) + "m" : "---";
 if (dDi >= 0) {
  dbgText = "Status: " + (dAc ? dPh : "IDLE") + "\n"
  + "Target: " + V3S(dbTg) + "\n"
  + "Attitude: " + dbHL + "\n"
  + "Entry: " + V3S(dW) + "\n"
  + "Attitude: " + dbEL + "\n"
  + "Current: " + V3S(cp) + "\n"
  + dbAL + "\n"
  + "Diff: " + diffStr + "\n"
  + "Spd:" + S1(curSpd) + "/" + S1(dbSL) + " Rem:" + remStr
  + thDir; }
 else {
  dbgText = "NOT SET\nTgt:---\nAtt:---\nEnt:---\nAtt:---\n"
  + "Cur:" + V3S(cp) + "\n" + dbAL + "\n"
  + "Diff:---\nSpd:" + S1(curSpd) + "/--- Rem:---"
  + thDir; } }
 else
 dbgText = "NOT SET\nTgt:---\nAtt:---\nEnt:---\nAtt:---\n"
  + "Cur:---\nAtt:---\nDiff:---\nSpd:---/--- Rem:---"
  + thDir;
 string logLine = "";
 if (dAc) {
 logLine = dPh;
 if (uSt.Length > 0) {
  string us = uSt;
  if (us.StartsWith("HD_")) us = us.Substring(3);
  logLine += "/" + us; }
 if (fEvT != 0) {
  string evLbl = fEvT == 1 ? "SL" : fEvT == 2 ? "EU" : fEvT == 3 ? "ES" : "EC";
  logLine += " OBS:" + evLbl + " " + S0(fEvH) + "m";
  if (fEvT >= 2 && cks.Count > 0) {
  MatrixD em = cks[0].WorldMatrix;
  double ep = Math.Atan2(VD(fEvD, em.Up), VD(fEvD, em.Forward)) * R2D;
  double ey = Math.Atan2(VD(fEvD, em.Right), VD(fEvD, em.Forward)) * R2D;
  logLine += " ev:P" + S0(ep) + "Y" + S0(ey); } }
 if (dPh == "FLY_RC" && cks.Count > 0) {
  Vector3D lt = fTg - CP(); MatrixD lm = cks[0].WorldMatrix;
  int ry = ((int)Math.Round(Math.Atan2(VD(lt, lm.Right), VD(lt, lm.Forward)) * R2D / 10.0)) * 10;
  int rd = ((int)Math.Round(lt.Length() / 100.0)) * 100;
  logLine += " d~" + rd + " y~" + ry; }
 if (dMs.Length > 0) {
  bool isKey = dMs.IndexOf("DONE", SC) >= 0 || dMs.IndexOf("LOCK", SC) >= 0
  || dMs.IndexOf("TIMEOUT", SC) >= 0 || dMs.IndexOf("ERR", SC) >= 0
  || dMs.IndexOf("BLOCK", SC) >= 0 || dMs.IndexOf("STOP", SC) >= 0;
  if (isKey) logLine += " " + dMs; } }
 else if (dMs.Length > 0) logLine = dMs;
 if (logLine.Length > 0 && logLine != logPrev) {
 logBuf.Add(logLine);
 logPrev = logLine; }
 string logText = "";
 string mTxt = "";
 if (mOn) {
 var mb = new System.Text.StringBuilder();
 if (mSub == 1 && mIdx < mNm.Count) {
  mb.Append("=== " + mNm[mIdx] + " ===");
  mb.Append('\n'); mb.Append(mSubIdx == 0 ? "> Go" : "  Go");
  mb.Append('\n'); mb.Append(mSubIdx == 1 ? "> Land" : "  Land");
  mb.Append('\n'); mb.Append(mSubIdx == 2 ? "> Via (" + viaQ.Count + "/5)" : "  Via (" + viaQ.Count + "/5)");
  mb.Append('\n'); mb.Append(mSubIdx == 3 ? "> Cancel" : "  Cancel");
  mb.Append("\n---\n[u]Up [d]Down [e]OK"); }
 else if (mSub == 2 && mIdx < mNm.Count) {
  mb.Append("=== " + mNm[mIdx] + " ===");
  mb.Append('\n'); mb.Append(mSubIdx == 0 ? "> Connect" : "  Connect");
  mb.Append('\n'); mb.Append(mSubIdx == 1 ? "> Via (" + viaQ.Count + "/5)" : "  Via (" + viaQ.Count + "/5)");
  mb.Append('\n'); mb.Append(mSubIdx == 2 ? "> Cancel" : "  Cancel");
  mb.Append("\n---\n[u]Up [d]Down [e]OK"); }
 else {
  mb.Append("=== NAV MENU ===");
  Vector3D cp = cks.Count > 0 ? CP() : Me.GetPosition();
  for (int mi = 0; mi < mNm.Count; mi++) {
  double md = (mPt[mi] - cp).Length();
  string ds = md >= 1000 ? S1(md/1000) + "km" : S0(md) + "m";
  string pf = mMem[mi] ? "[C] " : "    ";
  mb.Append('\n'); mb.Append(mi == mIdx ? "> " : "  ");
  mb.Append(pf); mb.Append(mNm[mi]); mb.Append(" ("); mb.Append(ds); mb.Append(")"); }
  if (mNm.Count == 0) mb.Append("\n(empty)");
  mb.Append("\n---\n[u]Up [d]Down [e]Select [imenu]Close"); }
 mTxt = mb.ToString(); }
 string[] pnlK = {"ideb","i0","i1","i2","ilog","imenu"};
 string[] pnlV = {dbgText, s0Text, s1Text, s2Text, "", mTxt};
 for (int i = 0; i < pnl.Count; i++) {
 IMyTextSurface surf = pnl[i] as IMyTextSurface;
 if (surf == null) continue;
 string pName = pnl[i].CustomName;
 bool hasMenu = pName.IndexOf("imenu", SC) >= 0;
 bool hasI0 = pName.IndexOf("i0", SC) >= 0;
 if (hasMenu && hasI0) {
  WSurf(surf, 1, mOn ? mTxt : s0Text); continue; }
 for (int j = 0; j < 6; j++) {
  if (pName.IndexOf(pnlK[j], SC) < 0) continue;
  if (j == 4) {
  if (logText.Length == 0) logText = BuildLog(surf);
  WSurf(surf, j, logText); }
  else if (j == 5) {
  surf.ContentType = ContentType.TEXT_AND_IMAGE;
  surf.FontSize = 1.2f; surf.Alignment = TextAlignment.LEFT;
  surf.WriteText(mOn ? mTxt : "Run 'imenu' to open"); }
  else WSurf(surf, j, pnlV[j]);
  break; } }
 for (int i = 0; i < psI.Count; i++) {
 if (psI[i] >= Me.SurfaceCount) continue;
 int ti = psT[i];
 if (ti == 4) {
  if (logText.Length == 0) logText = BuildLog(Me.GetSurface(psI[i]));
  WSurf(Me.GetSurface(psI[i]), ti, logText); }
 else WSurf(Me.GetSurface(psI[i]), ti, pnlV[ti]); } }
string FormatKg(double kg) {
 if (kg >= 1000.0)
 return (kg / 1000.0).ToString("F1") + " t";
 return S0(kg) + " kg"; }
void UpdateDistances() {
 for (int d = 0; d < 6; d++) {
 dDs[d] = -1;
 for (int i = 0; i < dCam[d].Count; i++) {
  if (!dCam[d][i].CanScan(50.0)) continue;
  MyDetectedEntityInfo info = dCam[d][i].Raycast(50.0);
  if (!info.IsEmpty()) {
  double dist = (info.HitPosition.Value - dCam[d][i].GetPosition()).Length() - dOf[d];
  if (dDs[d] < 0 || dist < dDs[d])
   dDs[d] = dist; } } }
 string dl = "";
 for (int d = 0; d < 6; d++)
 if (dDs[d] >= 0) dl += " " + dNm[d][0] + S0(dDs[d]);
 if (dl.Length > 0 && dl != udLogPrev) { logBuf.Add("UD" + dl); udLogPrev = dl; } }
string NeedRecalibration() {
 if (!cDn) return "cDn=false";
 if (cks.Count == 0) return "no cockpit";
 double curMass = cks[0].CalculateShipMass().TotalMass;
 double curGrav = cks[0].GetNaturalGravity().Length();
 double massPct = (cMs > 0) ? Abs(curMass - cMs) / cMs * 100 : 0;
 double gravPct = (cGL > 0) ? Abs(curGrav - cGL) / cGL * 100 : 0;
 if (cMs > 0 && massPct > 10)
 return "mass " + S1(massPct) + "% (" + S0(cMs) + "->" + S0(curMass) + ")";
 if (cGL > 0 && gravPct > 10)
 return "grav " + S1(gravPct) + "% (" + S2(cGL) + "->" + S2(curGrav) + ")";
 return ""; }
void REv(){fEvT=0;fEvH=0;fEvA=0;}
void SAD() {
 dAc = false;
 dEl = 0;
 dPh = "";
 cTm = 0;
 cBr = false;
 cCT = 0;
 cLA = 0;
 fAp = false;
 fAltPre = false;
 fAltRetPh = "";
 fAT = 0;
 dSt = 0;
 dCn = false;
 fRC = 0;
 REv();
 fSF = fSL = fSU = 100;
 nxPh = ""; appMode = ""; depDi = -1;
 dbApE = false;
 if (rcB != null) { rcB.SetAutoPilotEnabled(false); rcB.ClearWaypoints(); }
 CT();
 RG(); }
void SetPh(string ph) { dPh = ph; }
bool InitDep() {
 bool isConLk = IsConnectorConnected();
 bool isConbl = false;
 for (int ci = 0; ci < cons.Count; ci++) if (cons[ci].Status == MyShipConnectorStatus.Connectable) { isConbl = true; break; }
 bool isGearLk = IsGearLocked();
 int dt = 0;
 if (isConLk || isConbl) dt = 1;
 else if (isGearLk) dt = 2;
 else { double ga = ScanDist(5, flyAltLimit + 10); if (ga > 0 && ga < flyAltLimit) dt = 3; }
 if (dt == 0) return false;
 if (dt == 1 || dt == 2) ToggleShip(false);
 dAc = true; dEl = 0;
 fSP = CP(); fSF = fSL = fSU = 100; fAD = 3.0; fAp = false; fAT = 0;
 fRC = 0; REv();
 CT(); ETh();
 if (dt == 1) { depDi = -1; dPh = "DEP_EXIT"; uSt = ""; }
 else { depDi = 4; dPh = "DEP_MOVE"; uSt = ""; }
 return true; }
void NextPhase() {
 if (nxPh.Length == 0) { CT(); RG(); SAD(); dMs = fLb + ": DONE"; return; }
 int sep = nxPh.IndexOf('|');
 string next; if (sep >= 0) { next = nxPh.Substring(0, sep); nxPh = nxPh.Substring(sep + 1); }
 else { next = nxPh; nxPh = ""; }
 dAc = true; dEl = 0; fSP = CP(); fAD = 3.0; fAp = false; fAT = 0; fRC = 0;
 REv(); uSt = ""; CT(); RG();
 dPh = next; }
double ScanDist(int camIdx, double maxDist, double pitch = 0, double yaw = 0) {
 if (camIdx < 0 || camIdx > 5) return -1;
 for (int ci = 0; ci < dCam[camIdx].Count; ci++) {
 if (!dCam[camIdx][ci].CanScan(maxDist)) continue;
 MyDetectedEntityInfo hit;
 if (pitch == 0 && yaw == 0)
  hit = dCam[camIdx][ci].Raycast(maxDist);
 else
  hit = dCam[camIdx][ci].Raycast(maxDist, (float)pitch, (float)yaw);
 if (!hit.IsEmpty()) {
  double sd = (hit.HitPosition.Value - dCam[camIdx][ci].GetPosition()).Length();
  logBuf.Add("SC" + camIdx + " " + S0(sd) + "/" + S0(maxDist) + "m P" + S0(pitch) + "Y" + S0(yaw));
  return sd; }
 return maxDist; }
 return -1; }
double ScanClear(int camIdx, double maxDist, double maxAngle, bool horizontal) {
 double d0 = ScanDist(camIdx, maxDist);
 if (d0 < 0) return 0;
 if (d0 >= maxDist) return maxAngle;
 for (double a = 2; a <= maxAngle; a += 2) {
 double p = horizontal ? 0 : a;
 double y = horizontal ? a : 0;
 double dp = ScanDist(camIdx, maxDist, p, y);
 double dn = ScanDist(camIdx, maxDist, -p, -y);
 if (dp >= maxDist) return a;
 if (dn >= maxDist) return -a; }
 return 0; }
Vector3D CurvTgt(Vector3D origin, Vector3D target) {
 if (rcB == null) return target;
 Vector3D pc;
 if (!rcB.TryGetPlanetPosition(out pc)) return target;
 double dist = (target - origin).Length();
 if (dist < 10) return target;
 double R = (origin - pc).Length();
 double safeD = Math.Sqrt(20 * R);
 if (dist <= safeD) return target;
 Vector3D up = VN(origin - pc);
 Vector3D toT = target - origin;
 Vector3D hV = toT - up * VD(toT, up);
 double hDist = hV.Length();
 if (hDist < 1) return target;
 double tR = (target - pc).Length();
 double altDiff = tR - R;
 Vector3D hDir = VN(hV);
 double theta = hDist / R;
 double frac = 1.0;
 double segAlt = R + altDiff * frac;
 return pc + (up * Math.Cos(theta) + hDir * Math.Sin(theta)) * segAlt; }
double SpeedForDistance(double remain, double maxSpd) {
 if (remain > 1000) return Math.Min(maxSpd, 100);
 if (remain > 500) return Math.Min(maxSpd, 50);
 if (remain > 100) return Math.Min(maxSpd, 30);
 if (remain > 30) return Math.Min(maxSpd, 10);
 return Math.Max(1, remain * 0.5); }
void RcWP(Vector3D target, double speed) {
 rcB.ClearWaypoints();
 Vector3D origin = CP();
 double dist = (target - origin).Length();
 dbSL = SpeedForDistance(dist, speed);
 Vector3D pc;
 bool hasPlanet = rcB.TryGetPlanetPosition(out pc);
 int wpCount = 1;
 if (!hasPlanet || dist < 10) {
 rcB.AddWaypoint(target, "FLY_TARGET");
 } else {
 double R = (origin - pc).Length();
 double safeD = Math.Sqrt(20 * R);
 Vector3D up = VN(origin - pc);
 Vector3D toT = target - origin;
 Vector3D hV = toT - up * VD(toT, up);
 double hDist = hV.Length();
 if (hDist < 1 || dist <= safeD) {
  rcB.AddWaypoint(target, "FLY_TARGET");
 } else {
  double tR = (target - pc).Length();
  double altDiff = tR - R;
  Vector3D hDir = VN(hV);
  double segLen = safeD * 0.8;
  int segs = (int)Math.Ceiling(hDist / segLen);
  for (int i = 1; i <= segs; i++) {
  double frac = Math.Min(i * segLen, hDist) / hDist;
  double sHD = hDist * frac;
  double sAlt = R + altDiff * frac;
  double theta = sHD / R;
  Vector3D bp = pc + (up * Math.Cos(theta) + hDir * Math.Sin(theta)) * sAlt;
  if (i == segs) bp = target;
  string nm = i == segs ? "FLY_TARGET" : "WP_" + i;
  rcB.AddWaypoint(bp, nm); }
  wpCount = segs;
 } }
 rcB.SpeedLimit = (float)speed;
 rcB.FlightMode = FlightMode.OneWay;
 rcB.SetAutoPilotEnabled(true);
 logBuf.Add("WP d=" + S0(dist) + " s=" + S0(speed) + " wp=" + wpCount); }
void RcEv(MatrixD mat, Vector3D gravDir, double yawDeg, double remain) {
 Vector3D cp = CP();
 Vector3D fH = mat.Forward - gravDir * VD(mat.Forward, gravDir);
 Vector3D rH = mat.Right - gravDir * VD(mat.Right, gravDir);
 if (fH.LengthSquared() < 0.001 || rH.LengthSquared() < 0.001) return;
 fH = VN(fH); rH = VN(rH);
 double aRad = yawDeg * Math.PI / 180.0;
 fEvD = VN(fH * Math.Cos(aRad) + rH * Math.Sin(aRad));
 double flyDist = Math.Max(fEvH + 50, 100);
 Vector3D target = cp + fEvD * flyDist;
 double safeSpd = SpeedForDistance(Math.Max(0, fEvH - 30), fSF);
 RcWP(target, safeSpd); }
void RcEvUp(MatrixD mat, Vector3D gravDir, Vector3D antiGrav, double remain) {
 Vector3D cp = CP();
 Vector3D fH = mat.Forward - gravDir * VD(mat.Forward, gravDir);
 if (fH.LengthSquared() < 0.001) fH = mat.Forward;
 else fH = VN(fH);
 double flyPitch = 88;
 double sd20 = ScanDist(0, 800, 20, 0);
 if (sd20 < 0 || sd20 >= 800) flyPitch = 18;
 else {
 double sd45 = ScanDist(0, 800, 45, 0);
 if (sd45 < 0 || sd45 >= 800) flyPitch = 43; }
 double pRad = flyPitch * Math.PI / 180.0;
 fEvD = VN(fH * Math.Cos(pRad) + antiGrav * Math.Sin(pRad));
 double flyDist = Math.Max(fEvH + 50, 100);
 Vector3D target = cp + fEvD * flyDist;
 double safeSpd = SpeedForDistance(Math.Max(0, fEvH - 30), fSF);
 RcWP(target, safeSpd); }
void CTG() {
 if (cks.Count == 0) return;
 MatrixD m = cks[0].WorldMatrix;
 Vector3D[] ax = {m.Forward, m.Backward, m.Left, m.Right, m.Up, m.Down};
 for (int i = 0; i < 6; i++) tG[i].Clear();
 for (int i = 0; i < thrs.Count; i++) {
 if (IsIgnored(thrs[i])) continue;
 Vector3D tb = thrs[i].WorldMatrix.Backward;
 int best = 0; double bv = VD(tb, ax[0]);
 for (int j = 1; j < 6; j++) { double d = VD(tb, ax[j]); if (d > bv) { bv = d; best = j; } }
 tG[best].Add(thrs[i]); } }
void TF(int d, double f) {
 if (d < 0 || d > 5) return;
 thV[d] = f;
 List<IMyThrust> g = tG[d];
 for (int i = 0; i < g.Count; i++) {
 IMyThrust t = g[i];
 if (!t.IsWorking) continue;
 if (f > 0.01)
  t.ThrustOverride = (float)Math.Min(t.MaxEffectiveThrust, f / g.Count);
 else
 { t.ThrustOverridePercentage = 0f; } } }
void CT() { for (int i = 0; i < 6; i++) TF(i, 0); }
void gy(double p, double y, double r) {
 if (cks.Count == 0) return;
 gyV[0] = p; gyV[1] = y; gyV[2] = r;
 if (p == 0 && y == 0 && r == 0)
 { for (int i = 0; i < gys.Count; i++) { if (IsIgnored(gys[i])) continue; gys[i].Pitch = 0; gys[i].Yaw = 0; gys[i].Roll = 0; gys[i].GyroOverride = false; } return; }
 Vector3D rv = new Vector3D(-p, y, r);
 Vector3D wr = Vector3D.TransformNormal(rv, cks[0].WorldMatrix);
 for (int i = 0; i < gys.Count; i++) {
 if (IsIgnored(gys[i])) continue;
 Vector3D lr = Vector3D.TransformNormal(wr, MatrixD.Transpose(gys[i].WorldMatrix));
 gys[i].Pitch = (float)lr.X; gys[i].Yaw = (float)-lr.Y; gys[i].Roll = (float)-lr.Z;
 gys[i].GyroOverride = true; } }
void RCS() {
 csTimer = 0;
 csLastFwd = Vector3D.Zero;
 csLastUp = Vector3D.Zero; }
bool CheckStop(IMyCockpit cockpit) {
 csTimer += dlt;
 Vector3D pos = CP();
 MatrixD m = cockpit.WorldMatrix;
 if (csLastFwd.LengthSquared() < 0.5) {
 csLastPos = pos;
 csLastFwd = m.Forward;
 csLastUp = m.Up;
 csTimer = 0;
 return false; }
 if (csTimer >= 1.0) {
 double posDiff = (pos - csLastPos).Length();
 double fwdDeg = Math.Acos(ClampD(VD(m.Forward, csLastFwd), -1, 1)) * R2D;
 double upDeg = Math.Acos(ClampD(VD(m.Up, csLastUp), -1, 1)) * R2D;
 csLastPos = pos;
 csLastFwd = m.Forward;
 csLastUp = m.Up;
 csTimer = 0;
 return posDiff < 0.02 && fwdDeg < 0.2 && upDeg < 0.2; }
 return false; }
double CRE(MatrixD mat, Vector3D gd, Vector3D ag) {
 Vector3D desUp = ag - mat.Forward * VD(ag, mat.Forward);
 if (desUp.LengthSquared() < 0.001) return 0;
 desUp = VN(desUp);
 return -Math.Atan2(VD(desUp, mat.Right), VD(desUp, mat.Up)); }
double CPL(MatrixD mat, Vector3D gd) {
 return Math.Asin(ClampD(VD(mat.Forward, gd), -1, 1)); }
double CYT(MatrixD mat, Vector3D toTarget, Vector3D gd, Vector3D ag) {
 Vector3D dirH = toTarget - gd * VD(toTarget, gd);
 Vector3D fwdH = mat.Forward - gd * VD(mat.Forward, gd);
 if (dirH.LengthSquared() < 0.001 || fwdH.LengthSquared() < 0.001) return 0;
 Vector3D nD = VN(dirH);
 Vector3D nF = VN(fwdH);
 return Math.Atan2(VD(VX(nF, nD), ag), VD(nF, nD)); }
double CPT(MatrixD mat, Vector3D toTarget, Vector3D gd) {
 if (toTarget.LengthSquared() < 0.01) return 0;
 Vector3D dir = VN(toTarget);
 double tgtP = Math.Asin(ClampD(-VD(dir, gd), -1, 1));
 double noseP = Math.Asin(ClampD(-VD(mat.Forward, gd), -1, 1));
 return tgtP - noseP; }
bool HorizontalPosture(IMyCockpit cockpit) {
 MatrixD mat = cockpit.WorldMatrix;
 Vector3D gv = cockpit.GetNaturalGravity();
 if (gv.LengthSquared() < 0.01) { uSt = ""; return true; }
 Vector3D gd = VN(gv);
 Vector3D ag = -gd;
 double threshold = 0.05;
 if (uSt == "") uSt = "HP_ROLL";
 if (uSt == "HP_ROLL") {
 double err = CRE(mat, gd, ag);
 if (Abs(err) > threshold) {
  gy(0, 0, ClampD(err / cRG, -3.0, 3.0));
  dMs = "HP ROLL err=" + D1(err) + "°";
  return false; }
 RG();
 RCS();
 uSt = "HP_ROLL_CS";
 return false; }
 if (uSt == "HP_ROLL_CS") {
 dMs = "HP ROLL wait";
 if (CheckStop(cockpit)) uSt = "HP_PITCH";
 return false; }
 if (uSt == "HP_PITCH") {
 double err = CPL(mat, gd);
 if (Abs(err) > threshold) {
  gy(ClampD(err / cPG, -3.0, 3.0), 0, 0);
  dMs = "HP PITCH err=" + D1(err) + "°";
  return false; }
 RG();
 RCS();
 uSt = "HP_PITCH_CS";
 return false; }
 if (uSt == "HP_PITCH_CS") {
 dMs = "HP PITCH wait";
 if (CheckStop(cockpit)) uSt = "HP_YAW_CS";
 return false; }
 if (uSt == "HP_YAW_CS") {
 dMs = "HP YAW wait";
 if (CheckStop(cockpit)) { uSt = ""; return true; }
 return false; }
 return false; }
void RG() { gy(0, 0, 0); }
void UTG() {
 if (cks.Count == 0 || gys.Count == 0)
 { tGy = ""; RG(); return; }
 IMyCockpit cockpit = cks[0];
 MatrixD mat = cockpit.WorldMatrix;
 double curAngle = 0;
 if (tGy == "pitch") {
 double dot = ClampD(VD(mat.Forward, tIF), -1, 1);
 double cross = VD(VX(tIF, mat.Forward), mat.Right);
 curAngle = Math.Atan2(cross, dot) * R2D; }
 else if (tGy == "yaw") {
 double dot = ClampD(VD(mat.Forward, tIF), -1, 1);
 double cross = VD(VX(tIF, mat.Forward), mat.Up);
 curAngle = Math.Atan2(cross, dot) * R2D; }
 else if (tGy == "roll") {
 double dot = ClampD(VD(mat.Up, tIU), -1, 1);
 double cross = VD(VX(mat.Up, tIU), mat.Forward);
 curAngle = Math.Atan2(cross, dot) * R2D; }
 double remain = tGA - curAngle;
 while (remain > 180) remain -= 360;
 while (remain < -180) remain += 360;
 if (Abs(remain) < 1.0) {
 RG();
 dMs = "TEST " + tGy.ToUpper() + " DONE (" + S0(tGA) + "°)";
 tGy = "";
 return; }
 double remainRad = remain * Math.PI / 180.0;
 double gain = cRG;
 if (tGy == "pitch") gain = cPG;
 else if (tGy == "yaw") gain = cYG;
 double speed = ClampD(remainRad / gain, -3.0, 3.0);
 if (Abs(speed) < 0.05) speed = 0.05 * Math.Sign(remain);
 double p = 0, y = 0, r = 0;
 if (tGy == "pitch") p = speed;
 else if (tGy == "yaw") y = speed;
 else if (tGy == "roll") r = speed;
 gy(p, y, r);
 dMs = "TEST " + tGy.ToUpper() + " cur=" + S1(curAngle) + "° tgt=" + S0(tGA) + "°"; }
string DirAttitude(Vector3D from, Vector3D target, MatrixD mat, Vector3D gd, Vector3D ag) {
 Vector3D toTgt = target - from;
 if (toTgt.LengthSquared() < 0.01) return "Y=0.0 P=0.0";
 double yaw = CYT(mat, toTgt, gd, ag) * R2D;
 double relPitch = CPT(mat, toTgt, gd) * R2D;
 return "Y=" + S1(yaw) + " P=" + S1(relPitch); }
string S0(double v){return v.ToString("F0");}
string S1(double v){return v.ToString("F1");}
string S2(double v){return v.ToString("F2");}
string S3(double v){return v.ToString("F3");}
string FD(double d){return d<1000?S0(d)+"m":S1(d/1000)+"km";}
string D1(double r){return S1(r*R2D);}
string V3S(Vector3D v){return S1(v.X)+", "+S1(v.Y)+", "+S1(v.Z);}
Vector3D CP(){return cons.Count>0?cons[0].GetPosition():cks[0].GetPosition();}
double ClampD(double v, double min, double max) {
 return Math.Max(min, Math.Min(max, v)); }
double ParseEntryDist(string name) {
 int mi = name.LastIndexOf('m');
 if (mi < 2) return -1;
 int si = name.LastIndexOf(' ', mi - 1);
 if (si < 0) return -1;
 double v;
 if (double.TryParse(name.Substring(si + 1, mi - si - 1), out v) && v > 0)
 return v;
 return -1; }
bool ParseGPS(string s, out Vector3D pos, out string name) {
 pos = Vector3D.Zero; name = "";
 if (!s.StartsWith("GPS:", SC)) return false;
 string body = s.Substring(4);
 while (body.Length > 0 && body[body.Length - 1] == ':') body = body.Substring(0, body.Length - 1);
 int last = body.LastIndexOf(':'); if (last < 0) return false;
 string tail = body.Substring(last + 1);
 double z;
 if (!double.TryParse(tail, out z))
 { body = body.Substring(0, last); last = body.LastIndexOf(':'); if (last < 0) return false; }
 if (!double.TryParse(body.Substring(last + 1), out z)) return false;
 body = body.Substring(0, last); last = body.LastIndexOf(':'); if (last < 0) return false;
 double y; if (!double.TryParse(body.Substring(last + 1), out y)) return false;
 body = body.Substring(0, last); last = body.LastIndexOf(':'); if (last < 0) return false;
 double x; if (!double.TryParse(body.Substring(last + 1), out x)) return false;
 name = body.Substring(0, last); pos = new Vector3D(x, y, z); return true; }
string MemSer(string n, Vector3D p, Vector3D a, double e, int di)
{ return n+";"+p.X+";"+p.Y+";"+p.Z+";"+a.X+";"+a.Y+";"+a.Z+";"+e+";"+di; }
bool MemDes(string s, out string n, out Vector3D p, out Vector3D a, out double e, out int di) {
 n=""; p=Vector3D.Zero; a=Vector3D.Zero; e=15; di=-1;
 int fi = s.IndexOf(';'); if (fi < 0) return false;
 n = s.Substring(0, fi); string[] q = s.Substring(fi+1).Split(';');
 if (q.Length < 7) return false;
 double px,py,pz,ax,ay,az;
 if (!TP(q[0],out px)||!TP(q[1],out py)||!TP(q[2],out pz)
 ||!TP(q[3],out ax)||!TP(q[4],out ay)||!TP(q[5],out az)||!TP(q[6],out e)) return false;
 p = new Vector3D(px,py,pz); a = VN(new Vector3D(ax,ay,az));
 if (q.Length >= 8) { int d; if (int.TryParse(q[7], out d)) di = d; }
 return true; }
int FindMem(string name) {
 for (int i = 0; i < memList.Count; i++)
 { string mn; Vector3D mp,ma; double me; int mdi; if (MemDes(memList[i],out mn,out mp,out ma,out me,out mdi) && string.Equals(mn,name,SC)) return i; }
 return -1; }
void SetErr(string msg) { dMs = msg; }
int DirFromAD() {
 if (cks.Count == 0 || dAD.LengthSquared() < 0.5) return 0;
 MatrixD m = cks[0].WorldMatrix;
 double df = VD(dAD, m.Forward), dr = VD(dAD, m.Right), du = VD(dAD, m.Up);
 double af = Abs(df), ar = Abs(dr), au = Abs(du);
 if (af >= ar && af >= au) return df > 0 ? 0 : 1;
 if (ar >= au) return dr > 0 ? 3 : 2;
 return du > 0 ? 4 : 5; }
void CEP() {
 if (dDi < 0 || dAD.LengthSquared() < 0.5) return;
 double dist = dED > 0 ? dED : dockEntryDist[dDi];
 dW = dT - dAD * dist; }
Vector3D ViaAlt(Vector3D vp) {
 if (cks.Count == 0) return vp;
 Vector3D g = cks[0].GetNaturalGravity();
 if (g.LengthSquared() < 0.01) return vp;
 Vector3D gd = VN(g); Vector3D cp = CP();
 return vp - gd * VD(vp - cp, gd); }
void IDS() {
 uSt = "";
 CT();
 RG();
 for (int i = 0; i < thrs.Count; i++)
 { if(!IsIgnored(thrs[i]))thrs[i].Enabled=true; }
 Vector3D cp = CP();
 if (dSt == 0) {
 fTg = dW;
 fSF = fSL = fSU = 100;
 fAD = 3.0;
 fAp = false;
 fAT = 0;
 fSP = cp;
 fLb = "FWD";
 REv();
 Vector3D g = cks[0].GetNaturalGravity();
 if (g.LengthSquared() > 0.01 && dGA > 0 && dGA < flyAltLimit) {
  fTg = dW + VN(-g) * (flyAltLimit - dGA); }
 SetPh("FLY_RC"); }
 else if (dSt == 1) {
 if (dDi >= 4)
 { SetPh("DK_FINAL"); }
 else
 { SetPh("DK_ADJ"); } }
 else if (dSt == 2) {
 fAp = true;
 fAT = 0;
 fSF = fSL = fSU = 100;
 fAD = 0.3;
 fSP = cp;
 if (dDi == 1) {
  fTg = cp + VN(cp - dT) * 100;
  fLb = "BWD";
  SetPh("FLY_BWD_HD"); }
 else if (dDi == 0) {
  fTg = dT;
  fLb = "FWD";
  SetPh("FLY_FWD_HD"); }
 else {
  fTg = dT;
  fLb = dDi == 4 ? "UP" : dDi == 5 ? "DOWN" : dDi == 2 ? "LEFT" : "RIGHT";
  SetPh("FLY_FWD"); } } }
void DHP(IMyCockpit cockpit, Vector3D target, string label, string nextPhase,
 Vector3D velocity, double mass, Vector3D gravity) {
 Vector3D cp = CP();
 MatrixD mat = cockpit.WorldMatrix;
 bool hasGrav = gravity.LengthSquared() > 0.01;
 Vector3D gd = hasGrav ? VN(gravity) : Vector3D.Zero;
 Vector3D ag = -gd;
 Vector3D toTgt = target - cp;
 double yawRad = hasGrav ? CYT(mat, toTgt, gd, ag) : 0;
 double pitchRad = hasGrav ? CPT(mat, toTgt, gd) : 0;
 double rollRad = hasGrav ? CRE(mat, gd, ag) : 0;
 double threshold = (dPh == "DK_FINAL") ? 0.008 : 0.0175;
 if (uSt == "" || uSt == "HD_ROLL") {
 uSt = "HD_ROLL";
 if (Abs(rollRad) > threshold) {
  gy(0, 0, ClampD(rollRad / cRG, -3.0, 3.0));
  dMs = label + " ROLL=" + D1(rollRad) + "°"; }
 else {
  RG();
  RCS();
  uSt = "HD_ROLL_CS"; } }
 else if (uSt == "HD_ROLL_CS") {
 dMs = label + " ROLL wait";
 if (CheckStop(cockpit)) {
  if (Abs(rollRad) > threshold) uSt = "HD_ROLL";
  else uSt = "HD_YAW"; } }
 else if (uSt == "HD_YAW") {
 if (Abs(yawRad) > threshold) {
  gy(0, ClampD(yawRad / cYG, -1.5, 1.5), 0);
  dMs = label + " YAW=" + D1(yawRad) + "°"; }
 else {
  RG();
  RCS();
  uSt = "HD_YAW_CS"; } }
 else if (uSt == "HD_YAW_CS") {
 dMs = label + " YAW wait";
 if (CheckStop(cockpit)) {
  if (Abs(rollRad) > threshold) uSt = "HD_ROLL";
  else if (Abs(yawRad) > threshold) uSt = "HD_YAW";
  else uSt = "HD_PITCH"; } }
 else if (uSt == "HD_PITCH") {
 Vector3D toTgt2 = target - cp;
 double ad2 = VD(toTgt2, ag);
 double hSq2 = toTgt2.LengthSquared() - ad2 * ad2;
 double hD2 = hSq2 > 0 ? Math.Sqrt(hSq2) : 0;
 double tgtPitch = hD2 > 0.1 ? Abs(Math.Atan2(ad2, hD2)) : (Abs(ad2) > 0.1 ? Math.PI / 2.0 : 0);
 double absDist = toTgt2.Length();
 if (hasGrav && !dPh.StartsWith("CAL") && !fAltPre
  && tgtPitch >= Math.PI / 6.0
  && absDist > 30.0) {
  double needAlt2 = Abs(ad2) - hD2 * Math.Tan(Math.PI / 9.0);
  needAlt2 = Math.Max(needAlt2, 5.0);
  Vector3D adjDir2 = ad2 >= 0 ? ag : gd;
  int altCamIdx = ad2 >= 0 ? 4 : 5;
  double altSpace = ScanDist(altCamIdx, needAlt2 + 10);
  if (altSpace >= 0 && altSpace < needAlt2 + 1.0) {
  double availAlt = altSpace - 1.0;
  bool isFwdCmd = dCn || dPh == "TEST_HD_ENTRY" || dPh == "FLY_FWD_HD";
  if (availAlt < 1.0) {
   if (!isFwdCmd) {
   RG(); CT(); SAD();
   dMs = label + ": ALT BLOCKED (" + S1(altSpace) + "m)";
   return; } }
  else {
   needAlt2 = availAlt; }
  if (isFwdCmd && availAlt < needAlt2) {
   double fwdSpace = ScanDist(0, 800);
   if (fwdSpace >= 0 && fwdSpace < 5.0) {
   RG(); CT(); SAD();
   dMs = label + ": FWD+ALT BLOCKED";
   return; }
   double fwdDist = Math.Max(5.0, (fwdSpace > 0 ? fwdSpace : 800) - 1.0);
   Vector3D lFwd = mat.Forward - gd * VD(mat.Forward, gd);
   if (lFwd.LengthSquared() < 0.01) lFwd = target - cp;
   lFwd = VN(lFwd - gd * VD(lFwd, gd));
   fTg = cp + lFwd * fwdDist;
   fSP = cp; fAD = 3.0; fAp = false; fAT = 0; fSU = 100;
   fRC = 0; REv();
   fAltRetPh = dPh;
   fAltRetTg = target;
   fAltPre = true;
   fLb = label + " FwdClr";
   uSt = "";
   CT(); RG(); RCS();
   dPh = "ALT_LV";
   dMs = label + " ALT BLK FWD " + S1(fwdDist) + "m";
   return; } }
  fTg = cp + adjDir2 * needAlt2;
  fSP = cp; fAD = 0.3; fAp = false; fAT = 0; fSU = 100;
  fRC = 0; REv();
  fAltRetPh = dPh;
  fAltRetTg = target;
  fAltPre = true;
  fLb = label + " AltAdj";
  uSt = "";
  CT();
  RG();
  RCS();
  dPh = "ALT_LV";
  dMs = label + " ALT adj=" + S1(needAlt2) + "m"; }
 else if (Abs(pitchRad) > threshold) {
  gy(ClampD(pitchRad / cPG, -3.0, 3.0), 0, 0);
  dMs = label + " PITCH=" + D1(pitchRad) + "°"; }
 else {
  RG();
  RCS();
  uSt = "HD_PITCH_CS"; } }
 else if (uSt == "HD_PITCH_CS") {
 dMs = label + " PITCH wait";
 if (CheckStop(cockpit)) {
  if (Abs(rollRad) > threshold) uSt = "HD_ROLL";
  else if (Abs(yawRad) > threshold) uSt = "HD_YAW";
  else if (Abs(pitchRad) > threshold) uSt = "HD_PITCH";
  else {
  uSt = "";
  CT();
  RG();
  if (nextPhase == "") { SAD(); dMs = label + ": DONE"; }
  else {
   if (nextPhase == "FLY_FWD" || nextPhase == "FLY_RC") {
   double fwdScan = ScanDist(0, 800);
   if (fwdScan > 0 && fwdScan < 800) {
    for (int pi = 1; pi <= 10; pi++) {
    double sp = pi * 2.0;
    double sd = ScanDist(0, 800, sp, 0);
    if (sd < 0 || sd >= 800) {
     double evPitch = sp + 2;
     double pRad = evPitch * Math.PI / 180.0;
     Vector3D lFwd = mat.Forward - gd * VD(mat.Forward, gd);
     if (lFwd.LengthSquared() < 0.01) lFwd = (target - cp);
     lFwd = VN(lFwd - gd * VD(lFwd, gd));
     double climbDist = Math.Min(fwdScan * 0.8, 200);
     fTg = cp + VN(lFwd * Math.Cos(pRad) + ag * Math.Sin(pRad)) * climbDist;
     fSP = cp; fAD = 3.0; fAp = false; fAT = 0; fSU = 100;
     fRC = 0; REv();
     fAltRetPh = nextPhase;
     fAltRetTg = target;
     fAltPre = true;
     fLb = label + " PreClimb";
     dPh = nextPhase;
     return; } } } }
   fSP = cp;
   dPh = nextPhase; } } } } }
void DFL(IMyCockpit cockpit, MatrixD mat, Vector3D vel, double mass,
 bool hasGrav, Vector3D gravDir, Vector3D antiGrav) {
 Vector3D cp = CP();
 Vector3D toTgt = fTg - cp;
 Vector3D lineDir = VN(fTg - fSP);
 double remain = VD(toTgt, lineDir);
 Vector3D fromStart = cp - fSP;
 double proj = VD(fromStart, lineDir);
 Vector3D dev = fromStart - lineDir * proj;
 double devMag = dev.Length();
 double ld_f = VD(lineDir, mat.Forward);
 double ld_u = hasGrav ? VD(lineDir, antiGrav) : VD(lineDir, mat.Up);
 double ld_r = VD(lineDir, mat.Right);
 double devU = hasGrav ? VD(dev, antiGrav) : VD(dev, mat.Up);
 double devR = VD(dev, mat.Right);
 double devF = VD(dev, mat.Forward);
 bool depSkip = (dPh == "DEP_MOVE" || dPh == "DEP_EXIT");
 fRC++;
 if (hasGrav && !fAp && !fAltPre && !depSkip && remain > 10 && fRC % 10 == 0) {
 double sd5 = ScanDist(5, 300);
 if (sd5 > 0) gndAlt = sd5;
 double fScanMax = Math.Min(remain, 800);
 if (fEvT == 0) {
  double fd = ScanDist(0, fScanMax);
  if (fd > 0 && fd < fScanMax && fd > fAD) {
  fEvH = fd;
  double clr = ScanClear(0, 800, 20, true);
  if (clr != 0) {
   fEvT = 1; fEvA = clr;
   Vector3D fH = VN(mat.Forward - gravDir * VD(mat.Forward, gravDir));
   Vector3D rH = VN(mat.Right - gravDir * VD(mat.Right, gravDir));
   double aRad = Abs(clr) * Math.PI / 180.0;
   fEvD = clr > 0
   ? VN(fH * Math.Cos(aRad) + rH * Math.Sin(aRad))
   : VN(fH * Math.Cos(aRad) - rH * Math.Sin(aRad)); }
  else { fEvT = 2; fEvA = 0; fEvD = antiGrav; } } }
 else {
  double fd = ScanDist(0, fScanMax);
  if (fd < 0 || fd >= fScanMax) { REv(); }
  else if (fEvT == 1 && fRC % 20 == 0) {
  double clr2 = ScanClear(0, 800, 20, true);
  if (clr2 != 0) {
   fEvA = clr2;
   Vector3D fH = VN(mat.Forward - gravDir * VD(mat.Forward, gravDir));
   Vector3D rH = VN(mat.Right - gravDir * VD(mat.Right, gravDir));
   double aRad2 = Abs(clr2) * Math.PI / 180.0;
   fEvD = clr2 > 0
   ? VN(fH * Math.Cos(aRad2) + rH * Math.Sin(aRad2))
   : VN(fH * Math.Cos(aRad2) - rH * Math.Sin(aRad2)); } } } }
 if (!fAp && !fAltPre && remain < 30) {
 int camIdx = -1;
 if (ld_f > 0.5) camIdx = 0;
 else if (ld_f < -0.5) camIdx = 1;
 else if (ld_r > 0.5) camIdx = 3;
 else if (ld_r < -0.5) camIdx = 2;
 else if (ld_u > 0.3) camIdx = 4;
 else if (ld_u < -0.3) camIdx = 5;
 if (camIdx >= 0) {
  double hitD = ScanDist(camIdx, remain + 5);
  if (hitD > 0 && hitD < remain) {
  fAp = true;
  fAT = 0;
  fAD = 0.3; } } }
 if (fAp) {
 fAT += dlt;
 if (fAT > 30) {
  bool sAP = fAltPre; string sRP = fAltRetPh; Vector3D sRT = fAltRetTg;
  string sNP = nxPh; string sAM = appMode; int sDD = depDi;
  CT(); RG(); SAD();
  fAltPre = sAP; fAltRetPh = sRP; fAltRetTg = sRT;
  nxPh = sNP; appMode = sAM; depDi = sDD;
  dMs = fLb + ": TIMEOUT";
  return; } }
 if (remain < 0 || toTgt.Length() < fAD) {
 if (!fAp) {
  bool sAP = fAltPre; string sRP = fAltRetPh; Vector3D sRT = fAltRetTg;
  string sNP = nxPh; string sAM = appMode; int sDD = depDi;
  CT(); RG(); SAD();
  fAltPre = sAP; fAltRetPh = sRP; fAltRetTg = sRT;
  nxPh = sNP; appMode = sAM; depDi = sDD;
  dMs = fLb + ": DONE d=" + S1(toTgt.Length());
  return; }
 CT();
 Vector3D posErr = fTg - cp;
 double errU = hasGrav ? VD(posErr, antiGrav) : VD(posErr, mat.Up);
 double errR = VD(posErr, mat.Right);
 double errF = VD(posErr, mat.Forward);
 double errMain = 0;
 bool needCorr = false;
 if (Abs(ld_f) > 0.5)
 { needCorr = Abs(errU) >= 0.08 || Abs(errR) >= 0.08; errMain = errF; }
 else if (Abs(ld_u) > 0.3)
 { needCorr = Abs(errF) >= 0.08 || Abs(errR) >= 0.08; errMain = errU; }
 else if (Abs(ld_r) > 0.5)
 { needCorr = Abs(errU) >= 0.08 || Abs(errF) >= 0.08; errMain = errR; }
 if (needCorr || Abs(errMain) > 0.1) {
  double vU = hasGrav ? VD(vel, antiGrav) : VD(vel, mat.Up);
  double vR = VD(vel, mat.Right); double vF = VD(vel, mat.Forward);
  if (Abs(ld_f) > 0.5) {
  if (Abs(errU) >= 0.08) { double f = (ClampD(errU * 0.8, -2, 2) - vU) * mass * cTG; if (f > 0) TF(4, f); else TF(5, -f); }
  if (Abs(errR) >= 0.08) { double f = (ClampD(errR * 0.8, -2, 2) - vR) * mass * cTG; if (f > 0) TF(3, f); else TF(2, -f); }
  { double f = (ClampD(errF * 0.3, -1, 1) - vF) * mass * cTG; if (f > 0) TF(0, f); else TF(1, -f); } }
  else if (Abs(ld_u) > 0.3) {
  if (Abs(errF) >= 0.08) { double f = (ClampD(errF * 0.8, -2, 2) - vF) * mass * cTG; if (f > 0) TF(0, f); else TF(1, -f); }
  if (Abs(errR) >= 0.08) { double f = (ClampD(errR * 0.8, -2, 2) - vR) * mass * cTG; if (f > 0) TF(3, f); else TF(2, -f); }
  { double f = (ClampD(errU * 0.3, -1, 1) - vU) * mass * cTG; if (f > 0) TF(4, f); else TF(5, -f); } }
  else if (Abs(ld_r) > 0.5) {
  if (Abs(errU) >= 0.08) { double f = (ClampD(errU * 0.8, -2, 2) - vU) * mass * cTG; if (f > 0) TF(4, f); else TF(5, -f); }
  if (Abs(errF) >= 0.08) { double f = (ClampD(errF * 0.8, -2, 2) - vF) * mass * cTG; if (f > 0) TF(0, f); else TF(1, -f); }
  { double f = (ClampD(errR * 0.3, -1, 1) - vR) * mass * cTG; if (f > 0) TF(3, f); else TF(2, -f); } }
  thMs = "CORR eU=" + S2(errU) + " eR=" + S2(errR) + " eF=" + S2(errF);
  dbEF = errF; dbER = errR; dbEU = errU; dbApE = true;
  dMs = fLb + " " + thMs;
  return; }
 dbApE = false;
 CT();
 RG();
 for (int i = 0; i < grs.Count; i++) grs[i].Lock();
 for (int i = 0; i < cons.Count; i++) cons[i].Connect();
 if (IsConnectorConnected() || IsGearLocked())
 { SAD(); dMs = fLb + ": DOCKED"; }
 else
 { dMs = fLb + ": ARRIVE pos OK, waiting lock"; }
 return; }
 double fMS = Abs(ld_u) >= Abs(ld_f) && Abs(ld_u) >= Abs(ld_r) ? fSU : Abs(ld_r) >= Abs(ld_f) ? fSL : fSF;
 double spd = fMS;
 if (remain > 1000) spd = Math.Min(fMS, 100.0);
 else if (remain > 500) spd = Math.Min(fMS, 50.0);
 else if (remain > 100) spd = Math.Min(fMS, 30.0);
 else if (remain > 30) spd = Math.Min(fMS, 10.0);
 else spd = Math.Max(1.0, remain * 0.5);
 Vector3D flyLineDir = lineDir;
 if (fEvT != 0 && fEvH > 0) {
 double safeD = Math.Max(0, fEvH - 30);
 spd = Math.Max(2, Math.Min(spd, safeD * 0.05));
 flyLineDir = fEvD; }
 dbSL = spd;
 double mainVel = VD(vel, flyLineDir);
 double gndCorr = 0;
 if (hasGrav && gndAlt > 0 && gndAlt < flyAltLimit && !fAp && !fAltPre) {
 gndCorr = (flyAltLimit - gndAlt) * 0.3; }
 double mainF = (spd - mainVel) * mass * cTG;
 for (int ti = 0; ti < thrs.Count; ti++) {
 IMyThrust thr = thrs[ti];
 if (IsIgnored(thr)) continue;
 Vector3D tb = thr.WorldMatrix.Backward;
 double align = VD(tb, flyLineDir);
 double anf = VD(tb, mat.Forward);
 double anl = VD(tb, mat.Right);
 double anu = hasGrav ? VD(tb, antiGrav) : VD(tb, mat.Up);
 if (align > 0.1 && mainF > 0)
  thr.ThrustOverride = (float)Math.Min(thr.MaxEffectiveThrust, mainF * align);
 else if (align > 0.1 && thr.ThrustOverridePercentage > 0)
  thr.ThrustOverridePercentage = 0f;
 else if (fAp) {
  double f = 0; bool se = (fEvT != 0);
  if (Abs(ld_f) > 0.5) {
  double vU = hasGrav ? VD(vel, antiGrav) : VD(vel, mat.Up);
  if (!se && gndCorr == 0 && Abs(devU) >= 0.3 && Abs(anu) > 0.5)
   f += (ClampD(-devU * 0.5, -2, 2) - vU) * mass * cTG * anu;
  if (!se && Abs(devR) >= 0.3 && Abs(anl) > 0.5)
   f += (ClampD(-devR * 0.5, -2, 2) - VD(vel, mat.Right)) * mass * cTG * anl; }
  else if (Abs(ld_u) > 0.3) {
  if (Abs(devF) >= 0.3 && Abs(anf) > 0.5)
   f += (ClampD(-devF * 0.5, -2, 2) - VD(vel, mat.Forward)) * mass * cTG * anf;
  if (!se && Abs(devR) >= 0.3 && Abs(anl) > 0.5)
   f += (ClampD(-devR * 0.5, -2, 2) - VD(vel, mat.Right)) * mass * cTG * anl; }
  else if (Abs(ld_r) > 0.5) {
  double vU = hasGrav ? VD(vel, antiGrav) : VD(vel, mat.Up);
  if (!se && gndCorr == 0 && Abs(devU) >= 0.3 && Abs(anu) > 0.5)
   f += (ClampD(-devU * 0.5, -2, 2) - vU) * mass * cTG * anu;
  if (Abs(devF) >= 0.3 && Abs(anf) > 0.5)
   f += (ClampD(-devF * 0.5, -2, 2) - VD(vel, mat.Forward)) * mass * cTG * anf; }
  if (f > 0.01) thr.ThrustOverride = (float)Math.Min(thr.MaxEffectiveThrust, f);
  else if (thr.ThrustOverridePercentage > 0) thr.ThrustOverridePercentage = 0f; }
 else if (gndCorr != 0 && Abs(anu) > 0.5) {
  double velU = VD(vel, antiGrav);
  double fU = (ClampD(gndCorr, -5, 5) - velU) * mass * cTG * anu;
  if (fU > 0.01) thr.ThrustOverride = (float)Math.Min(thr.MaxEffectiveThrust, fU);
  else if (thr.ThrustOverridePercentage > 0) thr.ThrustOverridePercentage = 0f; }
 else if (thr.ThrustOverridePercentage > 0) thr.ThrustOverridePercentage = 0f; }
 if (fEvT != 0) {
 double fV = VD(vel, lineDir);
 if (fV > 0.5) {
  double bF = fV * mass * cTG;
  for (int bi = 0; bi < thrs.Count; bi++) {
  IMyThrust bt = thrs[bi];
  if (IsIgnored(bt)) continue;
  double ba = -VD(bt.WorldMatrix.Backward, lineDir);
  if (ba > 0.1 && bt.ThrustOverride < 0.01f)
   bt.ThrustOverride = (float)Math.Min(bt.MaxEffectiveThrust, bF * ba); } } }
 double rc = 0, pc = 0, yc = 0;
 double thr2 = 0.035;
 if (hasGrav) {
 double rErr = CRE(mat, gravDir, antiGrav);
 if (Abs(rErr) > 0.0524)
  rc = ClampD(rErr / cRG, -3.0, 3.0); }
 if (hasGrav && !depSkip && ((devMag >= 2.0 && Abs(ld_f) > 0.5 && !fAp && remain > 30) || fEvT != 0)) {
 Vector3D aimDir = fEvT != 0 ? fEvD : (ld_f > 0 ? toTgt : -toTgt);
 double yErr = CYT(mat, aimDir, gravDir, antiGrav);
 double pErr = CPT(mat, aimDir, gravDir);
 if (Abs(yErr) > thr2) yc = ClampD(yErr / cYG, -3.0, 3.0);
 if (Abs(pErr) > thr2) pc = ClampD(pErr / cPG, -3.0, 3.0); }
 if (rc != 0 || pc != 0 || yc != 0)
 gy(pc, yc, rc);
 else
 RG();
 if (fEvT != 0) thMs = "EV" + fEvT + " h=" + S0(fEvH) + " spd=" + S1(spd);
 else if (fAp) thMs = "AP dU=" + S1(devU) + " dR=" + S1(devR) + " dF=" + S1(VD(dev, mat.Forward));
 else thMs = "";
 string modeStr = fAp ? "[AP]" : fEvT != 0 ? "[EV" + fEvT + "]" : "";
 dMs = fLb + modeStr + " d=" + S1(remain) + " v=" + S1(mainVel); }
void AT(Vector3D desiredVel, Vector3D vel, double mass, Vector3D gravity) {
 if (cks.Count == 0) return;
 MatrixD m = cks[0].WorldMatrix;
 Vector3D gv = cks[0].GetNaturalGravity();
 bool hg = gv.LengthSquared() > 0.01;
 Vector3D ag = hg ? VN(-gv) : m.Up;
 Vector3D velErr = desiredVel - vel;
 Vector3D df = velErr * mass * cTG;
 double fF = VD(df, m.Forward); double fR = VD(df, m.Right);
 double fU = hg ? VD(df, ag) : VD(df, m.Up);
 if (fF > 0) TF(0, fF); else TF(1, -fF);
 if (fR < 0) TF(2, -fR); else TF(3, fR);
 if (fU > 0) TF(4, fU); else TF(5, -fU); }
void UAD() {
 if (cks.Count == 0) { SAD(); dMs = "ERR: no cockpit"; return; }
 bool iDp = dPh.StartsWith("DEP");
 if (!isRT && !iDp && (IsConnectorConnected() || IsGearLocked())) {
 SAD();
 dMs = "DOCKED!";
 return; }
 if (!isRT && !iDp) { for (int i = 0; i < cons.Count; i++) {
 if (cons[i].Status == MyShipConnectorStatus.Connectable)
  cons[i].Connect();
 } }
 dEl += dlt;
 if (dEl > dTO) {
 SAD();
 dMs = "TIMEOUT";
 return; }
 IMyCockpit cockpit = cks[0];
 MatrixD mat = cockpit.WorldMatrix;
 Vector3D gravity = cockpit.GetNaturalGravity();
 double mass = cockpit.CalculateShipMass().TotalMass;
 Vector3D velocity = cockpit.GetShipVelocities().LinearVelocity;
 Vector3D cP = CP();
 bool hasGrav = gravity.LengthSquared() > 0.01;
 Vector3D gravDir = hasGrav ? VN(gravity) : Vector3D.Zero;
 Vector3D antiGrav = -gravDir;
 double rollCmd = 0, pitchCmd = 0, yawCmd = 0;
 Vector3D desiredVel = Vector3D.Zero;
 if (dPh.StartsWith("CAL_")) {
 double dt = dlt;
 cTm += dt;
 cCT += dt;
 if (dPh == "CAL_HP") {
  if (HorizontalPosture(cockpit)) {
  cSF = mat.Forward;
  SetPh("CAL_T");
  cTm = 0;
  cCT = 0; }
  return; }
 if (dPh == "CAL_RR" || dPh == "CAL_PR" || dPh == "CAL_YR") {
  if (!uSt.StartsWith("HD_")) {
  if (!HorizontalPosture(cockpit)) {
   return; }
  uSt = "HD_YAW";
  RCS(); }
  if (uSt == "HD_YAW") {
  Vector3D gd = hasGrav ? gravDir : Vector3D.Zero;
  Vector3D ag = -gd;
  double err = CYT(mat, cSF, gd, ag);
  if (Abs(err) > 0.05) {
   gy(0, ClampD(err / cYG, -3.0, 3.0), 0);
   dMs = dPh + " yaw err=" + D1(err) + "°";
   return; }
  RG();
  RCS();
  uSt = "HD_YAW_CS";
  return; }
  if (uSt == "HD_YAW_CS") {
  dMs = dPh + " yaw wait";
  if (!CheckStop(cockpit))
   return; }
  uSt = "";
  if (dPh == "CAL_RR") {
  cStF = mat.Forward;
  cStU = mat.Up;
  SetPh("CAL_P"); }
  else if (dPh == "CAL_PR") {
  cStF = mat.Forward;
  SetPh("CAL_Y"); }
  else if (dPh == "CAL_YR") {
  cTm = 0;
  cCT = 0;
  cBr = true;
  cFSP = cP;
  cFLP = cP;
  SetPh("CAL_FW");
  return; }
  if (!cDn) {
  cBr = true;
  cTm = 0;
  cCT = 0;
  cLA = 0; }
  return; }
 if (dPh == "CAL_FW" || dPh == "CAL_BW") {
  if (cBr) {
  Vector3D thrDir = (dPh == "CAL_FW") ? mat.Forward : mat.Backward;
  AT(thrDir * 2.0, velocity, mass, gravity);
  if (cTm >= 1.0) {
   CT();
   cBr = false;
   cCT = 0;
   cFLP = cP;
   cTm = 0; }
  dMs = dPh + " thrust " + S1(cTm) + "s/1s"; }
  else {
  if (cCT >= 1.0) {
   cCT = 0;
   double posDiff = (cP - cFLP).Length();
   cFLP = cP;
   if (posDiff < 0.02) {
   Vector3D moved = cP - cFSP;
   double dist3D = moved.Length();
   if (dPh == "CAL_FW") {
    cFD = dist3D;
    cFSP = cP;
    cFLP = cP;
    cBr = true;
    cTm = 0;
    cCT = 0;
    SetPh("CAL_BW"); }
   else {
    cBD = dist3D;
    Vector3D fwdMoved = cP - cFSP;
    Vector3D bwdAsForward = -fwdMoved;
    double hComp = VD(bwdAsForward, cSF);
    double vComp = hasGrav ? VD(bwdAsForward, antiGrav) : 0;
    Vector3D rightDir = VX(cSF, antiGrav);
    if (rightDir.LengthSquared() > 0.001) rightDir = VN(rightDir);
    double lComp = VD(bwdAsForward, rightDir);
    if (Abs(hComp) > 0.01) {
    cTP = Math.Atan2(vComp, hComp);
    cTY = Math.Atan2(lComp, hComp); }
    cDn = true;
    cMs = mass;
    cGL = gravity.Length();
    RG();
    dMs = "CAL DONE R=" + S2(cRG)
    + " P=" + S2(cPG)
    + " Y=" + S2(cYG)
    + " T=" + S2(cTG)
    + " Up=" + S3(cVU)
    + " Dn=" + S3(cVD)
    + " Fd=" + S1(cFD)
    + " Bd=" + S1(cBD)
    + " tP=" + D1(cTP)
    + " tY=" + D1(cTY);
    if (!cOn && dDi >= 0) {
    CEP();
    fSF = fSL = fSU = 100;
    dSt = 0;
    uSt = "";
    IDS(); }
    else {
    SAD(); }
    return; } } }
  dMs = dPh + " wait"; }
  return; }
 if (dPh == "CAL_T") {
  if (cTm >= 2.0) {
  double maxUpThrust = 0;
  for (int i = 0; i < thrs.Count; i++) {
   if (IsIgnored(thrs[i])) continue;
   if (VD(thrs[i].WorldMatrix.Backward, antiGrav) > 0.5)
   maxUpThrust += thrs[i].MaxEffectiveThrust; }
  double gravForce = mass * gravity.Length();
  double maxExcessAccel = (maxUpThrust - gravForce) / mass;
  cTG = ClampD(maxExcessAccel * 0.5, 0.5, 10.0);
  cVSA = VD(cP, antiGrav);
  cVLA = cVSA;
  cBr = true;
  cTm = 0;
  cCT = 0;
  SetPh("CAL_VU");
  dMs = "CAL_T done T=" + S2(cTG); }
  else {
  dMs = "CAL_T hover " + S1(cTm) + "s"; }
  return; }
 if (dPh == "CAL_VU" || dPh == "CAL_VD") {
  double curAlt = VD(cP, antiGrav);
  double curDist = 0;
  if (dPh == "CAL_VU")
  curDist = curAlt - cVSA;
  else
  curDist = cVSA - curAlt;
  if (cBr) {
  double vVert = VD(velocity, antiGrav);
  double targetSpd = 2.0;
  if (dPh == "CAL_VU" && vVert < 0.5 && cTm > 0.5)
   targetSpd = 5.0;
  if (dPh == "CAL_VD" && -vVert < 0.5 && cTm > 0.5)
   targetSpd = 5.0;
  if (dPh == "CAL_VU")
   AT(antiGrav * targetSpd, velocity, mass, gravity);
  else
   AT(gravDir * targetSpd, velocity, mass, gravity);
  if (cTm >= 3.0) {
   CT();
   cBr = false;
   cCT = 0;
   cVLA = curAlt;
   cTm = 0; }
  dMs = dPh + " thrust " + S1(curDist) + "m v=" + S1(vVert) + " " + S1(cTm) + "s/3s"; }
  else {
  if (cCT >= 1.0) {
   cCT = 0;
   double settleDiff = Abs(curAlt - cVLA);
   cVLA = curAlt;
   if (settleDiff < 0.02) {
   double totalDist = Abs(curDist);
   double gain = (totalDist > 0.1) ? ClampD(totalDist / 3.0, 0.01, 10.0) : 0.1;
   if (dPh == "CAL_VU") {
    cVU = gain;
    cVSA = curAlt;
    cVLA = curAlt;
    cBr = true;
    cTm = 0;
    cCT = 0;
    SetPh("CAL_VD");
    dMs = "CAL_VU done Up=" + S3(cVU) + " dist=" + S1(totalDist) + "m"; }
   else {
    cVD = gain;
    cTm = 0;
    cCT = 0;
    cBr = true;
    cLA = 0;
    cStF = mat.Forward;
    cStU = mat.Up;
    cStR = mat.Right;
    SetPh("CAL_R");
    dMs = "CAL_VD done Dn=" + S3(cVD) + " dist=" + S1(totalDist) + "m"; } } }
  dMs = dPh + " wait " + S1(curDist) + "m"; }
  return; }
 double curAngle = 0;
 if (dPh == "CAL_R")
  curAngle = Math.Acos(ClampD(VD(mat.Up, cStU), -1, 1));
 else if (dPh == "CAL_P")
  curAngle = Math.Acos(ClampD(VD(mat.Forward, cStF), -1, 1));
 else if (dPh == "CAL_Y")
  curAngle = Math.Acos(ClampD(VD(mat.Forward, cStF), -1, 1));
 if (cBr) {
  double p = 0, y = 0, r = 0;
  if (dPh == "CAL_R") r = 1.0;
  else if (dPh == "CAL_P") p = -1.0;
  else if (dPh == "CAL_Y") y = 1.0;
  gy(p, y, r);
  if (cTm >= 1.0) {
  RG();
  cBA = curAngle;
  if (dPh == "CAL_R" || dPh == "CAL_P") {
   double gain = cBA > 0.01 ? cBA : 0.01;
   if (dPh == "CAL_R") {
   cRG = gain;
   SetPh("CAL_RR"); }
   else {
   cPG = gain;
   SetPh("CAL_PR"); }
   cBr = false;
   cTm = 0;
   cCT = 0;
   uSt = ""; }
  else {
   cBr = false;
   cCT = 0;
   cLA = curAngle;
   cTm = 0;
   RCS(); } }
  dMs = dPh + " burst " + D1(curAngle) + "°"; }
 else {
  if (CheckStop(cockpit)) {
  double gain = cBA > 0.01 ? cBA : 0.01;
  cYG = gain;
  RG();
  uSt = "";
  SetPh("CAL_YR"); }
  dMs = dPh + " wait " + D1(curAngle) + "° burst=" + D1(cBA) + "°"; }
 return; }
 if (dPh == "TEST_CS") {
 if (CheckStop(cockpit)) {
  SAD();
  dMs = "CheckStop: STOPPED"; }
 else
  dMs = "CheckStop: waiting...";
 return; }
 if (dPh == "DEP_EXIT") {
 if (uSt == "") {
  Vector3D cp2 = CP();
  int hi = FindMem("home");
  if (hi >= 0 && dDi >= 0) {
  CEP(); fTg = dW; depDi = dDi; }
  else {
  IMyShipConnector aCon = null;
  for (int ci = 0; ci < cons.Count; ci++)
   if (cons[ci].Status == MyShipConnectorStatus.Connected || cons[ci].Status == MyShipConnectorStatus.Connectable)
   { aCon = cons[ci]; break; }
  int eDi = 4;
  if (aCon != null && aCon.OtherConnector != null) {
   Vector3D oFwd = -aCon.OtherConnector.WorldMatrix.Forward;
   MatrixD cm = cks[0].WorldMatrix;
   double dF = VD(oFwd, cm.Forward), dR = VD(oFwd, cm.Right), dU = VD(oFwd, cm.Up);
   double aF = Abs(dF), aR = Abs(dR), aU = Abs(dU);
   if (aF >= aR && aF >= aU) eDi = dF > 0 ? 0 : 1;
   else if (aR >= aU) eDi = dR > 0 ? 3 : 2;
   else eDi = dU > 0 ? 4 : 5; }
  depDi = eDi;
  double eDist = dockEntryDist[eDi];
  double sd = ScanDist(eDi, eDist + 10);
  if (sd > 0 && sd < eDist) eDist = sd / 2.0;
  Vector3D[] dirs = {mat.Forward, mat.Backward, mat.Left, mat.Right, mat.Up, mat.Down};
  fTg = cp2 + dirs[eDi] * eDist; }
  fSP = CP(); fAp = false; fAT = 0; fAD = 3.0;
  uSt = "FLYING"; }
 DFL(cockpit, mat, velocity, mass, hasGrav, gravDir, antiGrav);
 if (dPh == "") { dAc = true; dPh = "DEP_MOVE"; uSt = ""; }
 return; }
 if (dPh == "DEP_MOVE") {
 if (uSt == "") {
  Vector3D cp2 = CP();
  int eDi = depDi >= 0 ? depDi : 4;
  int revDi = eDi ^ 1;
  double revDist = ScanDist(revDi, flyAltLimit + 10);
  if (revDist > 0 && revDist >= flyAltLimit) {
  dPh = "DEP_HP"; uSt = ""; return; }
  double climbH = flyAltLimit;
  if (revDist > 0) climbH = flyAltLimit - revDist;
  else climbH = flyAltLimit;
  double moveDist = ScanDist(eDi, climbH + 10);
  if (moveDist > 0 && moveDist < climbH) climbH = moveDist / 2.0;
  Vector3D[] dirs = {mat.Forward, mat.Backward, mat.Left, mat.Right,
  hasGrav ? antiGrav : mat.Up, hasGrav ? gravDir : mat.Down};
  fTg = cp2 + dirs[eDi] * climbH;
  fSP = cp2; fAp = false; fAT = 0; fAD = 3.0;
  fLb = "DEP";
  uSt = "FLYING"; }
 DFL(cockpit, mat, velocity, mass, hasGrav, gravDir, antiGrav);
 if (dPh == "") { dAc = true; dPh = "DEP_HP"; uSt = ""; }
 return; }
 if (dPh == "DEP_HP") {
 if (hasGrav && !HorizontalPosture(cockpit)) return;
 RG(); CT();
 NextPhase();
 return; }
 if (dPh == "FWD_E") {
 if (uSt == "") {
  if (appMode == "DK" || appMode == "CON") { CEP(); fTg = dW; }
  else if (viaI < viaQ.Count) { fTg = ViaAlt(viaQ[viaI]); }
  else fTg = goDP;
  fSP = CP(); fAD = 3.0; fAp = false; fAT = 0;
  fSF = fSL = fSU = 100; fRC = 0; REv();
  fLb = "FWD"; }
 DHP(cockpit, fTg, "HEAD", "FLY_RC", velocity, mass, gravity);
 return; }
 if (dPh == "GO_HD") {
 DHP(cockpit, fTg, "HEAD", "FLY_RC", velocity, mass, gravity);
 return; }
 if (dPh == "APP_DK" || dPh == "APP_CON") {
 dCn = true; dSt = 1; uSt = "";
 IDS(); return; }
 if (dPh == "APP_LAND") {
 dPh = "GO_HP"; uSt = ""; return; }
 if (dPh == "GO_HP") {
 if (hasGrav && HorizontalPosture(cockpit)) { RG(); }
 else if (!hasGrav) { }
 else return;
 if (viaI < viaQ.Count) {
  viaI++; Vector3D nt = viaI < viaQ.Count ? ViaAlt(viaQ[viaI]) : goDP;
  fTg = nt; fSP = CP(); fAD = 3.0; fAp = false; fAT = 0; fRC = 0; REv();
  nxPh = "GO_HP";
  dPh = "FWD_E"; uSt = ""; }
 else if (appMode == "LAND") {
  dPh = "LAND_CR"; uSt = ""; fAT = 0;
  CT(); ETh(); }
 else { CT(); RG(); SAD(); dMs = fLb + ": DONE"; }
 return; }
 if (dPh == "LAND_CR") {
 fAT += dlt;
 for (int gi = 0; gi < grs.Count; gi++) grs[gi].Lock();
 for (int ci = 0; ci < cons.Count; ci++) cons[ci].Connect();
 if (fAT > 60) {
  CT(); RG();
  ToggleShip(true); skDt = 3;
  SAD();
  dMs = "LAND: TIMEOUT";
  return; }
 if (IsGearLocked() || IsConnectorConnected()) {
  CT(); RG();
  ToggleShip(true); skDt = 3;
  SAD();
  dMs = "LAND: LOCKED";
  return; }
 double gAlt = ScanDist(5, 200);
 if (gAlt >= 200) gAlt = -1;
 if (gAlt > 0 && hasGrav) dbTg = CP() + gravDir * gAlt;
 double dSpd = 0.5;
 if (gAlt > 0) {
  if (gAlt > 10) dSpd = 3.0;
  else if (gAlt > 5) dSpd = 2.0;
  else if (gAlt > 2) dSpd = 1.0;
  else dSpd = 0.3; }
 if (hasGrav) HorizontalPosture(cockpit);
 double vDown = VD(velocity, gravDir);
 Vector3D desVel = gravDir * dSpd;
 AT(desVel, velocity, mass, gravity);
 double spd = cockpit.GetShipSpeed();
 if ((gAlt > 0 && gAlt < 1.0 && vDown < 0.05 && fAT > 3)
  || (fAT > 5 && spd < 0.1)) {
  CT(); RG();
  ToggleShip(true);
  skDt = 3;
  SAD();
  dMs = "LAND: LOCKED (touch)";
  return; }
 dMs = "LAND" + (gAlt > 0 ? " g=" + S1(gAlt) : "") + " vD=" + S1(vDown);
 return; }
 if (dPh == "TEST_HP") {
 if (HorizontalPosture(cockpit)) {
  RG();
  SAD();
  dMs = "HP: DONE"; }
 return; }
 if (dPh == "TEST_HD") {
 DHP(cockpit, dT, "Heading", "", velocity, mass, gravity);
 return; }
 if (dPh == "TEST_HD_HOME") {
 DHP(cockpit, dT, "Head Home", "", velocity, mass, gravity);
 return; }
 if (dPh == "TEST_HD_ENTRY") {
 DHP(cockpit, fTg, "Head Entry", dCn ? "FLY_RC" : "", velocity, mass, gravity);
 return; }
 if (dPh == "ALT_LV") {
 Vector3D lFwd = mat.Forward - gravDir * VD(mat.Forward, gravDir);
 if (lFwd.LengthSquared() < 0.01) lFwd = mat.Up - gravDir * VD(mat.Up, gravDir);
 DHP(cockpit, cP + VN(lFwd) * 100, "Level", "FLY_FWD", velocity, mass, gravity);
 return; }
 if (dPh == "FLY_FWD_HD") {
 DHP(cockpit, fTg, "FWD_HD", "FLY_FWD", velocity, mass, gravity);
 return; }
 if (dPh == "FLY_RC") {
 if (rcB == null) { dPh = "FLY_FWD"; return; }
 bool chain = dCn;
 int step = dSt;
 Vector3D toTgt = fTg - cP;
 double remain = toTgt.Length();
 double scanPitch = Math.Atan2(VD(toTgt, mat.Up), VD(toTgt, mat.Forward)) * R2D;
 double scanYaw = Math.Atan2(VD(toTgt, mat.Right), VD(toTgt, mat.Forward)) * R2D;
 if (uSt == "") {
  uSt = "RC_FLY";
  fRC = 0;
  REv();
  gndAlt = -1;
  RcWP(fTg, fSF); }
 if (hasGrav) {
  double rErr = CRE(mat, gravDir, antiGrav);
  if (Abs(rErr) > 0.0524)
  gy(0, 0, ClampD(rErr / cRG, -3.0, 3.0));
  else
  RG(); }
 if (fEvT == 4) {
  fEvA += dlt;
  double fd = ScanDist(0, Math.Min(remain, 800), scanPitch, scanYaw);
  if ((fd >= 0 && fd >= Math.Min(remain, 800)) || fEvA > 30) {
  for (int ti = 0; ti < tG[4].Count; ti++) tG[4][ti].ThrustOverridePercentage = 0f;
  rcB.DampenersOverride = true;
  REv();
  RcWP(fTg, fSF); }
  double rcV4 = VD(velocity, remain > 0.1 ? VN(toTgt) : mat.Forward);
  dMs = fLb + "[EC] d=" + S1(remain) + " v=" + S1(rcV4);
  return; }
 if (!rcB.IsAutoPilotEnabled || remain < fAD) {
  rcB.SetAutoPilotEnabled(false);
  rcB.ClearWaypoints();
  RG();
  if (remain < fAD * 3) {
  uSt = "";
  if (fAltPre) {
   fAltPre = false;
   dAc = true;
   Vector3D ncp = CP();
   fSP = ncp; fAD = 3.0; fAp = false; fAT = 0;
   fSU = 100;
   if (fAltRetPh.Length > 0) {
   fTg = fAltRetTg;
   dPh = fAltRetPh;
   fAltRetPh = ""; }
   else {
   fTg = dW;
   dPh = "FLY_RC"; } }
  else if (chain && step < 2) {
   dAc = true;
   dCn = true;
   dSt = step + 1;
   IDS(); }
  else if (chain && step == 2 && dDi >= 2) {
   dAc = true;
   dCn = false;
   uSt = "";
   SetPh("DK_ADJ"); }
  else { NextPhase(); }
  dMs = fLb + ": DONE d=" + S1(remain); }
  else {
  if (fEvT != 0) {
   double fd = ScanDist(0, Math.Min(remain, 800), scanPitch, scanYaw);
   if (fd < 0 || fd >= Math.Min(remain, 800))
   { REv(); } }
  RcWP(fTg, fSF); }
  return; }
 fRC++;
 double fScanMax = Math.Min(remain, 800);
 if (fEvT == 0) {
  if (fRC % 10 == 0) { RcWP(fTg, fSF); }
  if (fRC % 3 == 0) {
  double fd = ScanDist(0, fScanMax, scanPitch, scanYaw);
  if (fd > 0 && fd < fScanMax && fd < remain) {
   fEvT = 1; fEvH = fd; fEvA = 0;
   double safeDist = Math.Max(0, fEvH - 30);
   rcB.SpeedLimit = (float)SpeedForDistance(safeDist, fSF); } } }
 else if (fEvT == 1) {
  double fd = ScanDist(0, fScanMax, scanPitch, scanYaw);
  if (fd >= 0 && (fd >= fScanMax || fd >= remain)) {
  REv();
  RcWP(fTg, fSF); }
  else {
  if (fd > 0) fEvH = fd;
  double safeDist = Math.Max(0, fEvH - 30);
  rcB.SpeedLimit = (float)SpeedForDistance(safeDist, fSF);
  int si = (int)fEvA;
  if (si < 10) {
   double pitch = (si + 1) * 2.0;
   double sd = ScanDist(0, 800, pitch, 0);
   if (sd >= 800) {
   double evPitch = pitch + 2;
   fEvT = 2; fEvA = 0;
   RcEvUp(mat, gravDir, antiGrav, remain); }
   else { fEvA = si + 1; } }
  else if (si < 20) {
   int yi = si - 10;
   double ang = (yi + 1) * 2.0;
   double sdL = ScanDist(0, 800, 0, -ang);
   double sdR = ScanDist(0, 800, 0, ang);
   bool cL = sdL >= 800;
   bool cR = sdR >= 800;
   if (cL || cR) {
   double evYaw;
   if (cL && cR) evYaw = -(ang + 2);
   else if (cL) evYaw = -(ang + 2);
   else evYaw = ang + 2;
   fEvT = 3; fEvA = 0;
   RcEv(mat, gravDir, evYaw, remain); }
   else { fEvA = si + 1; } }
  else {
   fEvT = 4; fEvA = 0;
   rcB.SetAutoPilotEnabled(false);
   rcB.DampenersOverride = false;
   for (int ti = 0; ti < tG[4].Count; ti++)
   tG[4][ti].ThrustOverride = tG[4][ti].MaxEffectiveThrust; } } }
 else if (fEvT == 2) {
  double fd = ScanDist(0, fScanMax, scanPitch, scanYaw);
  if (fd >= 0 && (fd >= fScanMax || fd >= remain)) {
  REv();
  RcWP(fTg, fSF); }
  else {
  double safeSpd = SpeedForDistance(Math.Max(0, fEvH - 30), fSF);
  rcB.SpeedLimit = (float)safeSpd;
  dMs = fLb + "[EU] ev=" + S1(fEvH) + "m"; } }
 else if (fEvT == 3) {
  double fd = ScanDist(0, fScanMax, scanPitch, scanYaw);
  if (fd >= 0 && (fd >= fScanMax || fd >= remain)) {
  REv();
  RcWP(fTg, fSF); }
  else {
  double safeSpd = SpeedForDistance(Math.Max(0, fEvH - 30), fSF);
  rcB.SpeedLimit = (float)safeSpd;
  dMs = fLb + "[ES] ev=" + S1(fEvH) + "m"; } }
 double rcVel = VD(velocity, remain > 0.1 ? VN(toTgt) : mat.Forward);
 if (fEvT == 0 && rcVel > 5) dEl = 0;
 string modeStr = fEvT == 0 ? "" : fEvT == 1 ? "[SL]" : fEvT == 2 ? "[EU]" : fEvT == 3 ? "[ES]" : "[EC]";
 dMs = fLb + modeStr + " d=" + S1(remain) + " v=" + S1(rcVel) + " y=" + S0(scanYaw);
 return; }
 if (dPh == "FLY_FWD") {
 bool chain = dCn;
 int step = dSt;
 DFL(cockpit, mat, velocity, mass, hasGrav, gravDir, antiGrav);
 if (dPh == "") {
  if (fAltPre) {
  fAltPre = false;
  dAc = true;
  Vector3D ncp = CP();
  fSP = ncp; fAD = 3.0; fAp = false; fAT = 0;
  fSU = 100;
  if (fAltRetPh.Length > 0) {
   fTg = fAltRetTg;
   dPh = fAltRetPh;
   fAltRetPh = ""; }
  else {
   fTg = dW;
   dPh = "FLY_RC"; } }
  else if (chain && step < 2) {
  dAc = true;
  dCn = true;
  dSt = step + 1;
  IDS(); }
  else if (chain && step == 2 && dDi >= 2) {
  dAc = true;
  dCn = false;
  uSt = "";
  SetPh("DK_ADJ"); }
  else { NextPhase(); } }
 return; }
 if (dPh == "FLY_BWD_HD") {
 DHP(cockpit, fTg, "BWD_HD", "FLY_BWD", velocity, mass, gravity);
 return; }
 if (dPh == "FLY_BWD") {
 if (uSt == "") {
  CT();
  fTg = dT;
  fAD = 0.3;
  fAp = true;
  fSP = cP;
  uSt = "FLYING"; }
 DFL(cockpit, mat, velocity, mass, hasGrav, gravDir, antiGrav);
 return; }
 if (dPh == "DK_NEXT") {
 dSt++;
 IDS();
 return; }
 else if (dPh == "DK_ADJ") {
 Vector3D cp = CP();
 Vector3D posErr = dW - cp;
 double errF = VD(posErr, mat.Forward);
 double errR = VD(posErr, mat.Right);
 double errU = hasGrav ? VD(posErr, antiGrav) : VD(posErr, mat.Up);
 if (dDi <= 1) errF = 0;
 else if (dDi <= 3) errR = 0;
 else errU = 0;
 double remain = Math.Sqrt(errF * errF + errR * errR + errU * errU);
 if (Abs(errF) < 0.15 && Abs(errR) < 0.15 && Abs(errU) < 0.15) {
  CT(); RG();
  uSt = ""; adjWt = 0; adjGn = 0.5;
  if (dDi >= 4)
  dPh = dCn ? "DK_NEXT" : "";
  else if (dDi == 0)
  { SetPh("DK_FINAL"); }
  else
  { SetPh("DK_YAW"); }
  if (dPh == "") { SAD(); dMs = "FIT: DONE"; }
  return; }
 if (uSt == "") {
  if (adjWt > 0) {
  adjWt--;
  if (adjWt == 0) {
   double moved = (cp - adjPP).Length();
   if (moved < 0.02) adjGn = Math.Min(adjGn + 0.1, 1.0); }
  CT(); return; }
  Vector3D dir; double dist; string lbl;
  double absF = Abs(errF), absR = Abs(errR), absU = Abs(errU);
  if (absU >= absF && absU >= absR)
  { dir = errU > 0 ? (hasGrav ? antiGrav : mat.Up) : (hasGrav ? gravDir : mat.Down); dist = absU; lbl = errU > 0 ? "ADJ UP" : "ADJ DN"; }
  else if (absR >= absF)
  { dir = errR > 0 ? mat.Right : mat.Left; dist = absR; lbl = errR > 0 ? "ADJ RT" : "ADJ LT"; }
  else
  { dir = errF > 0 ? mat.Forward : mat.Backward; dist = absF; lbl = errF > 0 ? "ADJ FWD" : "ADJ BWD"; }
  fTg = cp + dir * (dist * adjGn);
  fSF = fSL = fSU = dCS;
  fAD = 0.15;
  fSP = cp;
  fLb = lbl;
  CT(); ETh();
  uSt = "FLYING"; }
 bool savedChain = dCn;
 DFL(cockpit, mat, velocity, mass, hasGrav, gravDir, antiGrav);
 if (dPh == "") {
  CT(); RG();
  dAc = true;
  dCn = savedChain;
  uSt = "";
  adjWt = 2; adjPP = cp;
  SetPh("DK_ADJ"); }
 return; }
 else if (dPh == "DK_YAW") {
 if (uSt == "") {
  Vector3D toHome = VN(dT - cP);
  Vector3D upRef = hasGrav ? antiGrav : mat.Up;
  if (dDi == 1)
  fTg = cP - toHome * 100;
  else if (dDi == 2)
  fTg = cP - VX(upRef, toHome) * 100;
  else
  fTg = cP + VX(upRef, toHome) * 100; }
 DHP(cockpit, fTg, "DK YAW", "DK_FINAL", velocity, mass, gravity);
 return; }
 else if (dPh == "DK_FINAL") {
 if (uSt == "") {
  if (dDi == 0) {
  Vector3D toHome = VN(dT - cP);
  fTg = cP + toHome * 100; }
  else if (dDi == 1) {
  Vector3D toHome = VN(dT - cP);
  fTg = cP - toHome * 100; }
  else if (dDi >= 4)
  fTg = cP + dSF * 100;
  else {
  Vector3D fwd = cockpit.WorldMatrix.Forward;
  if (hasGrav) {
   fwd = fwd - gravDir * VD(fwd, gravDir);
   if (fwd.LengthSquared() > 0.001) fwd = VN(fwd);
   else fwd = cockpit.WorldMatrix.Forward; }
  fTg = cP + fwd * 100; } }
 string finalNext = (dDi >= 4) ? "DK_ADJ" : (dCn ? "DK_NEXT" : "");
 DHP(cockpit, fTg, "DK FINAL", finalNext, velocity, mass, gravity);
 return; }
 if (gys.Count > 0)
 gy(pitchCmd, yawCmd, rollCmd);
 AT(desiredVel, velocity, mass, gravity); }
