const int S_PWR=0,S_CRG=1,S_PRD=2,S_DOC=3,S_LIF=4,S_DMG=5,S_WRK=6,S_GRD=7;
static readonly string[] SKEYS={"*power","*cargo","*production","*docked","*lifesupport","*damage","*working","*grid"};
static readonly string[] SNAME={"POWER GRID","CARGO","PRODUCTION","DOCKED SHIPS","LIFE SUPPORT","DAMAGE","WORKING BLOCKS","GRID INFO"};
static readonly string[] PLBL={"OVERVIEW","GRAPH","SOURCES","CONSUMERS"};
static readonly string[] PRD_TABS={"OVERVIEW","ASSEMBLERS","REFINERIES","REACTORS","H2/O2 GENERATORS"};
static readonly string[] CRG_TABS={"OVERALL","INGOTS","ORES","COMPONENTS","FOOD","WEAPONS & AMMO","TOOLS","MISC"};
static readonly string[] WRK_TABS={"ALL","DRILLS","PISTONS","ROTORS","DOORS","LANDING GEAR","PARACHUTES"};
static readonly string[] LIF_TABS={"OVERALL","OXYGEN","HYDROGEN","VENTS"};
static readonly string[] CN={"red","green","blue","cyan","yellow","orange","white","teal","skyblue","purple","pink","lime","darknavy","gray","darkgray","gold","coral","magenta","navy","olive"};
static readonly Color[] CV={new Color(220,60,60),new Color(0,210,80),new Color(60,110,255),new Color(0,210,210),new Color(220,210,0),new Color(220,130,0),new Color(210,210,210),new Color(0,175,155),new Color(80,175,255),new Color(160,60,220),new Color(220,80,180),new Color(80,220,40),new Color(5,10,20),new Color(120,130,140),new Color(60,65,70),new Color(220,185,0),new Color(220,90,70),new Color(200,0,200),new Color(20,40,120),new Color(100,110,0)};

const string TP="\x01",TH="\x02",TO="\x03",TC="\x04",TD="\x05";
int _scr = 0;
static double _time = 0;

class SE
{
    public IMyTextSurface Surf;
    public IMyTerminalBlock Blk;
    public bool IsCockpit;
    public int Scr = 0;
    public float ScrollPos = 0f;
    public int Tab = 0;
    public int Pv = 0;
    public int Cf = 0;
    public int Pf = 0;
    public int Lf = 0;
    public bool Auto = true;
    public float ScrollSpeed = 0.2f;
    public int ForcedScr = -1;
    public bool TouchEnabled = false;
}

List<SE> _surfs=new List<SE>();
IMyTextSurface _statS=null;
bool _first=true; int _tick=0;
List<float> _netHistory = new List<float>();
const int HIST_MAX = 60;

List<IMyBatteryBlock> _bat =new List<IMyBatteryBlock>();
List<IMyReactor> _rct =new List<IMyReactor>();
List<IMySolarPanel> _sol =new List<IMySolarPanel>();
List<IMyGasTank> _h2 =new List<IMyGasTank>();
List<IMyGasTank> _o2 =new List<IMyGasTank>();
List<IMyGasGenerator> _gg =new List<IMyGasGenerator>();
List<IMyCargoContainer> _crg =new List<IMyCargoContainer>();
List<IMyAssembler> _asm =new List<IMyAssembler>();
List<IMyRefinery> _ref =new List<IMyRefinery>();
List<IMyShipConnector> _con =new List<IMyShipConnector>();
List<IMyOxygenFarm> _ofm =new List<IMyOxygenFarm>();
List<IMyAirVent> _ven =new List<IMyAirVent>();
List<IMyPowerProducer> _wnd =new List<IMyPowerProducer>();
List<IMyShipDrill> _drl =new List<IMyShipDrill>();
List<IMyPistonBase> _pst =new List<IMyPistonBase>();
List<IMyMotorStator> _rot =new List<IMyMotorStator>();
List<IMyDoor> _dor =new List<IMyDoor>();
List<IMyLandingGear> _lgr =new List<IMyLandingGear>();
List<IMyParachute> _par =new List<IMyParachute>();
List<IMyCockpit> _cpt =new List<IMyCockpit>();
List<string> _dn =new List<string>();
List<float> _dp =new List<float>();

// Grid Info
int _totalDoors = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string arg, UpdateType src)
{
    _time += 0.1;
    if(_first){ScanGrid();_first=false;}
    _tick++; 
    if(_tick >= 100){ScanGrid();_tick=0;}

    ScanDmg();
    UpdatePowerHistory();
    DoInput(arg);

    foreach(var se in _surfs)
    {
        if(se.Blk != null)
        {
            se.ScrollSpeed = GetScrollSpeed(se.Blk.CustomData);
            se.ForcedScr = GetForcedWindow(se.Blk.CustomData);
            se.TouchEnabled = CD(se).ToLower().Contains("*touch");
            if(!HasAnyScreen(se)) se.Scr = -1;
            else if(se.Scr < 0 || !SOn(se, se.Scr)) se.Scr = GetFirstEnabledScreen(se);
        }
    }

    foreach(var se in _surfs) if(se.Auto && se.Surf != null && se.Scr >= 0)
    {
        se.ScrollPos += se.ScrollSpeed;
        var lines = BldL(se.Scr, GetTabForScreen(se));
        int ps = PS(se);
        int maxS = Math.Max(0, lines.Count - ps);
        if(maxS > 0)
        {
            if(se.ScrollPos >= maxS) se.ScrollPos = 0;
        }
        else se.ScrollPos = 0;
    }

    DrawAll();
    DrawPBStatus();
}

// ====================== SCAN & UTILITY ======================
void ScanDmg()
{
    _dn.Clear(); _dp.Clear();
    var all = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(all, b => b.IsSameConstructAs(Me));
    foreach(var b in all)
    {
        var slim = b.CubeGrid.GetCubeBlock(b.Position);
        if(slim == null) continue;
        float integrity = slim.MaxIntegrity > 0 ? (slim.BuildIntegrity - slim.CurrentDamage) / slim.MaxIntegrity : 0f;
        if(integrity < 0.99f)
        {
            _dn.Add(b.CustomName);
            _dp.Add(integrity);
        }
    }
}

void UpdatePowerHistory()
{
    float net = 0;
    foreach(var r in _rct) net += r.CurrentOutput;
    foreach(var s in _sol) net += s.CurrentOutput;
    foreach(var w in _wnd) net += w.CurrentOutput;
    foreach(var b in _bat) net -= b.CurrentOutput;
    _netHistory.Add(net);
    if(_netHistory.Count > HIST_MAX) _netHistory.RemoveAt(0);
}

void ScanGrid()
{
    G(_bat);G(_rct);G(_sol);G(_gg);G(_crg);G(_asm);G(_ref);
    G(_con);G(_ofm);G(_ven);G(_drl);G(_pst);G(_rot);G(_dor);
    G(_lgr);G(_par);G(_cpt);

    var tk=new List<IMyGasTank>(); GridTerminalSystem.GetBlocksOfType(tk,b=>b.IsSameConstructAs(Me));
    _h2.Clear();_o2.Clear();
    foreach(var t in tk)
        if(t.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen"))_h2.Add(t);
        else _o2.Add(t);

    var pw=new List<IMyPowerProducer>(); GridTerminalSystem.GetBlocksOfType(pw,b=>b.IsSameConstructAs(Me));
    _wnd.Clear();
    foreach(var p in pw)
        if(!(p is IMyBatteryBlock)&&!(p is IMyReactor)&&!(p is IMySolarPanel)&&!(p is IMyGasGenerator))
            _wnd.Add(p);

    // Grid Info - BlockCount removed as requested
    _totalDoors = _dor.Count;

    var oldSurfs = new List<SE>(_surfs);
    _surfs.Clear();_statS=null;

    var all=new List<IMyTerminalBlock>(); GridTerminalSystem.GetBlocksOfType(all,b=>b.IsSameConstructAs(Me));
    foreach(var blk in all)
    {
        string nm=blk.CustomName;
        if(nm=="SBOS Status"){var sp=blk as IMyTextSurface;if(sp!=null)_statS=sp;continue;}
        if(!nm.StartsWith("SBOS"))continue;

        bool isCk=nm.ToLower().Contains("cockpit");
        int idx=0;
        int col=nm.LastIndexOf(':');
        if(col>0){int.TryParse(nm.Substring(col+1),out idx);}

        if(string.IsNullOrEmpty(blk.CustomData.Trim()))
            blk.CustomData=isCk?DefaultCDCockpit():DefaultCD();

        IMyTextSurface surface = null;
        var prov=blk as IMyTextSurfaceProvider;
        if(prov!=null&&idx<prov.SurfaceCount) surface=prov.GetSurface(idx);
        else if(blk is IMyTextSurface) surface = blk as IMyTextSurface;

        if(surface==null) continue;

        var se = new SE{Surf=surface,Blk=blk,IsCockpit=isCk,Scr=S_PWR,ScrollPos=0f,Tab=0,Pv=0,Cf=0,Pf=0,Lf=0,Auto=true,ScrollSpeed=GetScrollSpeed(blk.CustomData),ForcedScr=GetForcedWindow(blk.CustomData),TouchEnabled=false};

        foreach(var old in oldSurfs)
            if(old.Blk != null && old.Blk.EntityId == blk.EntityId)
            {
                se.Scr = old.Scr; se.ScrollPos = old.ScrollPos; se.Tab = old.Tab;
                se.Pv = old.Pv; se.Cf = old.Cf; se.Pf = old.Pf; se.Lf = old.Lf;
                se.Auto = old.Auto; se.ScrollSpeed = old.ScrollSpeed;
                se.ForcedScr = old.ForcedScr;
                se.TouchEnabled = old.TouchEnabled;
                break;
            }
        if(!HasAnyScreen(se)) se.Scr = -1;
        else if(se.Scr < 0 || !SOn(se, se.Scr)) se.Scr = GetFirstEnabledScreen(se);
        _surfs.Add(se);
    }
}

float GetScrollSpeed(string cd)
{
    string lo = cd.ToLower();
    int pos = lo.IndexOf("*scroll-speed");
    if(pos < 0) return 0.2f;
    int dash = cd.IndexOf('-', pos + 13);
    if(dash < 0) return 0.2f;
    string rest = cd.Substring(dash+1);
    int nl = rest.IndexOf('\n');
    string val = (nl>=0?rest.Substring(0,nl):rest).Trim();
    float speed; return float.TryParse(val, out speed) && speed > 0 ? speed : 0.2f;
}

int GetForcedWindow(string cd)
{
    if(string.IsNullOrEmpty(cd)) return -1;
    string lo = cd.ToLower();
    if(lo.Contains("*window - all")) return -1;
    for(int i=0;i<SNAME.Length;i++)
        if(lo.Contains("*window - " + SNAME[i].ToLower().Replace(" ",""))) return i;
    if(lo.Contains("*window - power")) return S_PWR;
    if(lo.Contains("*window - cargo")) return S_CRG;
    if(lo.Contains("*window - production")) return S_PRD;
    if(lo.Contains("*window - docked")) return S_DOC;
    if(lo.Contains("*window - lifesupport")) return S_LIF;
    if(lo.Contains("*window - damage")) return S_DMG;
    if(lo.Contains("*window - working")) return S_WRK;
    if(lo.Contains("*window - grid")) return S_GRD;
    return -1;
}

void G<T>(List<T> lst) where T:class,IMyTerminalBlock {GridTerminalSystem.GetBlocksOfType(lst,b=>b.IsSameConstructAs(Me));}

string DefaultCD()=> "*power\n*cargo\n*production\n*docked\n*lifesupport\n*damage\n*working\n*grid\n*lines-shown - 11\n*scroll-speed - 0.2\n*touch\n*color-power - green\n*color-h2 - red\n*color-o2 - cyan\n*color-cargo - yellow\n*color-damage - orange\n*color-text - teal\n*color-header - skyblue\n*color-background - darknavy\n";

string DefaultCDCockpit()=> "*cockpit-hud\n*color-text - teal\n*color-header - skyblue\n*color-background - darknavy\n*color-power - green\n*color-h2 - red\n*color-o2 - cyan\n";

bool HasAnyScreen(SE se)
{
    if(se.Blk == null) return false;
    string cd = se.Blk.CustomData.ToLower();
    foreach(var k in SKEYS) if(cd.Contains(k)) return true;
    return false;
}

int GetFirstEnabledScreen(SE se)
{
    if(se.Blk == null) return S_PWR;
    string cd = se.Blk.CustomData.ToLower();
    for(int i=0;i<SKEYS.Length;i++)
        if(cd.Contains(SKEYS[i])) return i;
    return S_PWR;
}

// ====================== INPUT ======================
void DoInput(string arg)
{
    if(string.IsNullOrEmpty(arg)) return;

    int targetLCD = 0;
    string cleanArg = arg.ToUpper().Trim();

    if(cleanArg.Length > 0 && char.IsDigit(cleanArg[0]) && cleanArg.Contains("BTN"))
    {
        targetLCD = cleanArg[0] - '0';
        cleanArg = cleanArg.Substring(1);
    }

    switch(cleanArg)
    {
        case "BTN3":
            foreach(var se in _surfs)
            {
                if(targetLCD != 0 && GetLCDNumber(se) != targetLCD) continue;
                if(se.ForcedScr >= 0) continue;
                if(!HasAnyScreen(se)) continue;
                do { se.Scr = (se.Scr + 1) % SNAME.Length; } while(!SOn(se, se.Scr));
                se.ScrollPos = 0;
                se.Tab = se.Pv = se.Cf = se.Pf = se.Lf = 0;
                se.Auto = true;
            }
            if(targetLCD == 0)
            {
                do { _scr = (_scr + 1) % SNAME.Length; } while(!HasScreenGlobal(_scr));
            }
            break;

        case "BTN4":
            foreach(var se in _surfs)
            {
                if(targetLCD != 0 && GetLCDNumber(se) != targetLCD) continue;
                se.Auto = !se.Auto;
            }
            break;

        case "BTN1":
            foreach(var se in _surfs)
            {
                if(targetLCD != 0 && GetLCDNumber(se) != targetLCD) continue;
                if(se.ForcedScr >= 0) continue;
                if(!HasAnyScreen(se)) continue;
                if(se.Scr == S_PWR) se.Pv = (se.Pv + PLBL.Length - 1) % PLBL.Length;
                else if(se.Scr == S_CRG) se.Cf = (se.Cf + CRG_TABS.Length - 1) % CRG_TABS.Length;
                else if(se.Scr == S_PRD) se.Pf = (se.Pf + PRD_TABS.Length - 1) % PRD_TABS.Length;
                else if(se.Scr == S_WRK) se.Tab = (se.Tab + WRK_TABS.Length - 1) % WRK_TABS.Length;
                else if(se.Scr == S_LIF) se.Lf = (se.Lf + LIF_TABS.Length - 1) % LIF_TABS.Length;
                se.ScrollPos = 0;
            }
            break;

        case "BTN2":
            foreach(var se in _surfs)
            {
                if(targetLCD != 0 && GetLCDNumber(se) != targetLCD) continue;
                if(se.ForcedScr >= 0) continue;
                if(!HasAnyScreen(se)) continue;
                if(se.Scr == S_PWR) se.Pv = (se.Pv + 1) % PLBL.Length;
                else if(se.Scr == S_CRG) se.Cf = (se.Cf + 1) % CRG_TABS.Length;
                else if(se.Scr == S_PRD) se.Pf = (se.Pf + 1) % PRD_TABS.Length;
                else if(se.Scr == S_WRK) se.Tab = (se.Tab + 1) % WRK_TABS.Length;
                else if(se.Scr == S_LIF) se.Lf = (se.Lf + 1) % LIF_TABS.Length;
                se.ScrollPos = 0;
            }
            break;
    }
}

bool HasScreenGlobal(int s)
{
    foreach(var se in _surfs) if(SOn(se,s)) return true;
    return false;
}

int GetLCDNumber(SE se)
{
    if(se.Blk == null) return 0;
    string nm = se.Blk.CustomName.ToLower().Trim();
    int num = 0;
    int start = nm.LastIndexOf("button ");
    if(start >= 0 && int.TryParse(nm.Substring(start+7).Trim(), out num)) return num;
    start = nm.LastIndexOf("btn");
    if(start >= 0 && int.TryParse(nm.Substring(start+3).Trim(), out num)) return num;
    start = nm.LastIndexOf("lcd ");
    if(start >= 0 && int.TryParse(nm.Substring(start+4).Trim(), out num)) return num;
    return 0;
}

int GetTabForScreen(SE se)
{
    if(se.Scr==S_PWR) return se.Pv;
    if(se.Scr==S_CRG) return se.Cf;
    if(se.Scr==S_PRD) return se.Pf;
    if(se.Scr==S_WRK) return se.Tab;
    if(se.Scr==S_LIF) return se.Lf;
    return 0;
}

string CD(SE se){return se!=null&&se.Blk!=null?se.Blk.CustomData:"";}
bool SOn(SE se,int i){return i>=0 && CD(se).ToLower().Contains(SKEYS[i]);}
int PS(SE se){return Math.Max(4,PI(CD(se),"*lines-shown",11));}

Color GC(SE se,string k,Color d){return PC(CD(se),k,d);}
Color PC(string cd,string key,Color def)
{
    string lo=cd.ToLower();int pos=lo.IndexOf(key.ToLower());if(pos<0)return def;
    int dash=cd.IndexOf('-',pos+key.Length);if(dash<0)return def;
    string rest=cd.Substring(dash+1);int nl=rest.IndexOf('\n');
    string val=(nl>=0?rest.Substring(0,nl):rest).Trim().ToLower();
    for(int i=0;i<CN.Length;i++)if(val==CN[i])return CV[i];
    string[]p=val.Split(',');if(p.Length>=3){int r,g,b;if(int.TryParse(p[0].Trim(),out r)&&int.TryParse(p[1].Trim(),out g)&&int.TryParse(p[2].Trim(),out b))return new Color(r,g,b);}
    return def;
}
int PI(string cd,string key,int def)
{
    string lo=cd.ToLower();int pos=lo.IndexOf(key.ToLower());if(pos<0)return def;
    int dash=cd.IndexOf('-',pos+key.Length);if(dash<0)return def;
    string rest=cd.Substring(dash+1);int nl=rest.IndexOf('\n');
    string val=(nl>=0?rest.Substring(0,nl):rest).Trim();int r;return int.TryParse(val,out r)?r:def;
}

// ====================== DRAWING ======================
void DrawAll()
{
    foreach(var se in _surfs)
    {
        if(se.IsCockpit){DrawCockpit(se);continue;}
        if(se.Scr < 0 || !SOn(se, se.Scr)){DrawBlank(se);continue;}
        DrawFull(se);
    }
    DrawStatusBar();
}

void DrawCockpit(SE se)
{
    var surf=se.Surf; Vector2 sz=surf.SurfaceSize;
    Color cBg =GC(se,"*color-background",new Color(5,10,20));
    Color cTx =GC(se,"*color-text", new Color(0,175,155));
    Color cHd =GC(se,"*color-header", new Color(80,175,255));
    Color cPwr=GC(se,"*color-power", new Color(0,210,80));
    Color cH2 =GC(se,"*color-h2", new Color(220,60,60));
    Color cO2 =GC(se,"*color-o2", new Color(0,210,210));
    Color cWrn=new Color(220,160,0);
    Color cCrt=new Color(220,40,40);
    Color cEmp=new Color(22,32,48);
    surf.ContentType=VRage.Game.GUI.TextPanel.ContentType.SCRIPT;surf.Script="";
    var f=surf.DrawFrame();
    R(ref f,sz*.5f,sz,cBg);
    float batC=0,batM=0,batO=0; foreach(var b in _bat){batC+=b.CurrentStoredPower;batM+=b.MaxStoredPower;batO+=b.CurrentOutput;}
    float batR=batM>0?batC/batM:0f;
    float h2f=0; foreach(var t in _h2)h2f+=(float)t.FilledRatio; float h2r=_h2.Count>0?h2f/_h2.Count:0f;
    float o2f=0; foreach(var t in _o2)o2f+=(float)t.FilledRatio; float o2r=_o2.Count>0?o2f/_o2.Count:0f;
    float speedMs=0; string altStr="N/A"; string gravStr="N/A";
    bool inGrav=false;
    IMyCockpit activeCpt=null;
    foreach(var c in _cpt){if(c.IsUnderControl){activeCpt=c;break;}}
    if(activeCpt==null&&_cpt.Count>0)activeCpt=_cpt[0];
    if(activeCpt!=null)
    {
        var vel=activeCpt.GetShipVelocities();
        speedMs=(float)vel.LinearVelocity.Length();
        double alt; activeCpt.TryGetPlanetElevation(MyPlanetElevation.Surface,out alt);
        altStr=alt>0?FormatAlt(alt):"N/A";
        var grav=activeCpt.GetNaturalGravity();
        float gLen=(float)grav.Length();
        gravStr=gLen>0.01f?(gLen/9.81f).ToString("F2")+"g":"Space";
        inGrav=gLen>0.01f;
    }
    int dockedCount=0; foreach(var c in _con)if(c.Status==MyShipConnectorStatus.Connected)dockedCount++;
    int dmgCount=_dn.Count;
    float pad=sz.X*.025f;
    float rowH=sz.Y/9.2f;
    float ts=Clamp(sz.X/360f,.45f,1.0f);
    float tsS=ts*.88f;
    R(ref f,new Vector2(sz.X*.5f,rowH*.5f),new Vector2(sz.X,rowH),Darken(cBg,.4f,10,20,30));
    T(ref f,Me.CubeGrid.CustomName, new Vector2(sz.X*.5f,rowH*.05f),cHd,ts*.98f,TextAlignment.CENTER);
    R(ref f,new Vector2(sz.X*.5f,rowH), new Vector2(sz.X,1.5f),cHd);
    float cy=rowH+2f;
    string speedStr=speedMs<1f?"0 m/s":speedMs>=1000f?(speedMs/1000f).ToString("F1")+"km/s":((int)speedMs)+" m/s";
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"SPEED",speedStr,cHd,cTx,-1f,cEmp,cTx);
    cy+=rowH;
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"ALT",altStr,cHd,cTx,-1f,cEmp,cTx);
    cy+=rowH;
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"GRAV",gravStr,cHd,inGrav?cWrn:cTx,-1f,cEmp,cTx);
    cy+=rowH;
    Color bCol=batR<0.1f?cCrt:batR<0.25f?cWrn:cPwr;
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"BAT",(batR*100f).ToString("F0")+"%",cHd,bCol,batR,cEmp,bCol);
    cy+=rowH;
    Color h2Col=h2r<0.1f?cCrt:h2r<0.2f?cWrn:cH2;
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"H2 ",(h2r*100f).ToString("F0")+"%",cHd,h2Col,h2r,cEmp,h2Col);
    cy+=rowH;
    Color o2Col=o2r<0.1f?cCrt:o2r<0.2f?cWrn:cO2;
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"O2 ",(o2r*100f).ToString("F0")+"%",cHd,o2Col,o2r,cEmp,o2Col);
    cy+=rowH;
    string dockStr=dockedCount>0?"DOCKED ("+dockedCount+")":"UNDOCKED";
    Color dockCol=dockedCount>0?cPwr:cTx;
    DrawHudRow(ref f,sz,cy,rowH,pad,ts,tsS,"CON",dockStr,cHd,dockCol,-1f,cEmp,cTx);
    cy+=rowH;
    if(dmgCount>0)
    {
        R(ref f,new Vector2(sz.X*.5f,cy+rowH*.5f),new Vector2(sz.X,rowH),new Color(60,10,10));
        T(ref f,"! DAMAGE: "+dmgCount+" block(s)",new Vector2(sz.X*.5f,cy+rowH*.1f),cCrt,tsS,TextAlignment.CENTER);
    }
    else
    {
        T(ref f,"Hull OK",new Vector2(sz.X*.5f,cy+rowH*.1f),cPwr,tsS,TextAlignment.CENTER);
    }
    f.Dispose();
}

void DrawHudRow(ref MySpriteDrawFrame f,Vector2 sz,float cy,float rowH,float pad,float ts,float tsS,string label,string val,Color lCol,Color vCol,float barRatio,Color barEmp,Color barFill)
{
    T(ref f,label,new Vector2(pad,cy+rowH*.1f),lCol,tsS,TextAlignment.LEFT);
    if(barRatio>=0f)
    {
        float bW=sz.X*.34f; float bH=rowH*.38f;
        float bX=sz.X*.205f; float bY=cy + rowH*.48f;
        R(ref f,new Vector2(bX+bW*.5f,bY+bH*.5f),new Vector2(bW,bH),barEmp);
        if(barRatio>0f){float fw=bW*Clamp(barRatio,0f,1f);R(ref f,new Vector2(bX+fw*.5f,bY+bH*.5f),new Vector2(fw,bH),barFill);}
    }
    T(ref f,val,new Vector2(sz.X - pad,cy+rowH*.1f),vCol,ts,TextAlignment.RIGHT);
    R(ref f,new Vector2(sz.X*.5f,cy+rowH-.75f),new Vector2(sz.X,.75f),new Color(20,35,50));
}

string FormatAlt(double alt)
{
    if(alt>=1000.0)return(alt/1000.0).ToString("F2")+"km";
    return((int)alt)+"m";
}

void DrawBlank(SE se)
{
    se.Surf.ContentType=VRage.Game.GUI.TextPanel.ContentType.SCRIPT;se.Surf.Script="";
    var f=se.Surf.DrawFrame(); Vector2 sz=se.Surf.SurfaceSize;
    R(ref f,sz*.5f,sz,GC(se,"*color-background",new Color(5,10,20)));
    T(ref f,"SBOS Ready",new Vector2(sz.X*.5f,sz.Y*.5f),GC(se,"*color-text",new Color(0,175,155)),.8f,TextAlignment.CENTER);
    f.Dispose();
}

void DrawFull(SE se)
{
    var surf=se.Surf; Vector2 sz=surf.SurfaceSize;
    Color cBg =GC(se,"*color-background",new Color(5,10,20));
    Color cTx =GC(se,"*color-text", new Color(0,175,155));
    Color cHd =GC(se,"*color-header", new Color(80,175,255));
    Color cPwr=GC(se,"*color-power", new Color(0,210,80));
    Color cH2 =GC(se,"*color-h2", new Color(220,60,60));
    Color cO2 =GC(se,"*color-o2", new Color(0,210,210));
    Color cCrg=GC(se,"*color-cargo", new Color(220,210,0));
    Color cDmg=GC(se,"*color-damage", new Color(220,130,0));
    Color cWrn=new Color(220,160,0); Color cCrt=new Color(220,40,40); Color cEmp=new Color(22,32,48);
    int ps=PS(se);
    float ts =Clamp(sz.X/520f,.46f,1.08f);
    float tsS =ts*.79f;
    float tsF =Clamp(sz.X/720f,.34f,.88f);
    float hdrH=sz.Y*.118f;
    float ftrH=sz.Y*.13f;
    float div =1.4f;
    float padX=sz.X*.03f;
    float cTop=hdrH+div;
    float cBot=sz.Y-ftrH-div;
    float cH =cBot-cTop;
    float lnH =cH/ps;

    surf.ContentType=VRage.Game.GUI.TextPanel.ContentType.SCRIPT;surf.Script="";
    var frame=surf.DrawFrame();
    R(ref frame,sz*.5f,sz,cBg);
    R(ref frame,new Vector2(sz.X*.5f,hdrH),new Vector2(sz.X,div),cHd);
    float h1y=hdrH*.1f;
    T(ref frame,"SBOS v3.4", new Vector2(padX,h1y), cHd,ts*0.78f, TextAlignment.LEFT);
    T(ref frame,Me.CubeGrid.CustomName, new Vector2(sz.X - padX, h1y), cTx,ts*0.78f, TextAlignment.RIGHT);
    float h2y=hdrH*.52f;
    int dispScr = se.ForcedScr >= 0 ? se.ForcedScr : se.Scr;
    string currentTab = GetTabsForScreen(dispScr)[GetTabForScreen(se)];
    if(se.Auto)
    {
        Color flash = (Math.Sin(_time * 8) > 0) ? new Color(255,240,80) : cWrn;
        T(ref frame,"AUTO SCROLL "+se.ScrollSpeed.ToString("F1"),new Vector2(padX, h2y),flash,tsS*0.72f,TextAlignment.LEFT);
        T(ref frame,SNAME[dispScr], new Vector2(sz.X - padX*2.5f, h2y), cHd,ts*0.82f, TextAlignment.RIGHT);
    }
    else
    {
        T(ref frame,currentTab, new Vector2(padX, h2y), cHd,ts*0.82f, TextAlignment.LEFT);
        T(ref frame,SNAME[dispScr], new Vector2(sz.X - padX*2.5f, h2y), cHd,ts*0.82f, TextAlignment.RIGHT);
    }
    var lines = BldL(dispScr, GetTabForScreen(se));
    int maxS=Math.Max(0,lines.Count-ps);
    int scroll = (int)se.ScrollPos;
    float frac = se.ScrollPos - scroll;
    float cy=cTop+4f - frac*lnH;
    for(int i=scroll;i<lines.Count&&i<scroll+ps+1;i++,cy+=lnH)
    {
        if(i<0) continue;
        string raw=lines[i];
        if(string.IsNullOrEmpty(raw))continue;
        if(raw.StartsWith("I|"))
        {
            string[] parts = raw.Substring(2).Split('|');
            if(parts.Length >= 3)
            {
                string iconTexture = parts[0];
                string name = parts[1];
                string amount = parts[2];
                Vector2 iconPos = new Vector2(padX + 16, cy + lnH*0.5f - lnH*0.37f);
                Vector2 iconSize = new Vector2(lnH*0.78f, lnH*0.78f);
                frame.Add(new MySprite(SpriteType.TEXTURE, iconTexture, iconPos, iconSize, Color.White));
                T(ref frame,name,new Vector2(padX + lnH*1.08f,cy),cTx,tsS*0.78f,TextAlignment.LEFT);
                T(ref frame,amount,new Vector2(sz.X - padX,cy),cTx,tsS*0.82f,TextAlignment.RIGHT);
            }
            continue;
        }
        char tag=raw[0];
        bool isB=tag==TP[0]||tag==TH[0]||tag==TO[0]||tag==TC[0]||tag==TD[0];
        if(isB)
        {
            Color bc=tag==TP[0]?cPwr:tag==TH[0]?cH2:tag==TO[0]?cO2:tag==TC[0]?cCrg:cDmg;
            string[]pts=raw.Substring(1).Split('|');
            float ratio=0f;string lbl="";
            if(pts.Length>=1)float.TryParse(pts[0],out ratio);
            if(pts.Length>=2)lbl=pts[1];
            ratio=Clamp(ratio,0f,1f);
            float bH =lnH*.4f;
            float nameY=cy - 2;
            float barY=cy + lnH*.52f;
            float bMY=barY + bH*.5f;
            T(ref frame,Tr(lbl,38),new Vector2(padX,nameY),cTx,tsS*.78f,TextAlignment.LEFT);
            R(ref frame,new Vector2(padX + (sz.X*.49f)*.5f,bMY),new Vector2(sz.X*.49f,bH),cEmp);
            if(ratio>0f){float fw=sz.X*.49f*ratio;R(ref frame,new Vector2(padX + fw*.5f,bMY),new Vector2(fw,bH),bc);}
            T(ref frame,(ratio*100f).ToString("F0")+"%",new Vector2(sz.X - padX*1.1f,barY),bc,tsS*.85f,TextAlignment.RIGHT);
        }
        else
        {
            Color col=cTx;
            if(raw.Contains("CRITICAL")||raw.Contains("! CRIT"))col=cCrt;
            else if(raw.Contains("WARNING")||raw.Contains("! WARN"))col=cWrn;
            else if(raw.TrimStart().StartsWith("--")&&raw.TrimEnd().EndsWith("--"))
            {
                col=cHd;
                T(ref frame,raw,new Vector2(sz.X*.5f,cy),col,tsS*.78f,TextAlignment.CENTER);
                continue;
            }
            T(ref frame,raw,new Vector2(padX,cy),col,tsS*.78f,TextAlignment.LEFT);
        }
    }
    if(maxS>0)
    {
        float pw=Math.Max(3f,sz.X*.007f);
        float px=sz.X-pw-2f;
        float th=Math.Max(pw*3.5f,cH*.08f);
        float pp=se.ScrollPos / maxS;
        float py=cTop+pp*(cH-th)+th*.5f;
        R(ref frame,new Vector2(px,cTop+cH*.5f),new Vector2(pw,cH),cEmp);
        R(ref frame,new Vector2(px,py), new Vector2(pw*2.8f,th),cTx);
    }
    float fTop=sz.Y-ftrH;
    R(ref frame,new Vector2(sz.X*.5f,fTop),new Vector2(sz.X,div),cHd);
    R(ref frame,new Vector2(sz.X*.5f,fTop+ftrH*.5f),new Vector2(sz.X,ftrH),cHd);
    string[]bl=BL2(se.Scr);
    float fr1=fTop+div+ftrH*.09f;
    float fr2=fTop+div+ftrH*.56f;
    float qw=sz.X*.5f;
    T(ref frame,bl[0],new Vector2(qw*.48f, fr1),cTx, tsF,TextAlignment.CENTER);
    T(ref frame,bl[1],new Vector2(qw*1.52f,fr1),cTx, tsF,TextAlignment.CENTER);
    T(ref frame,bl[2],new Vector2(qw*.48f, fr2),cWrn,tsF,TextAlignment.CENTER);
    T(ref frame,bl[3],new Vector2(qw*1.52f,fr2),cHd, tsF,TextAlignment.CENTER);
    frame.Dispose();
}

string[] GetTabsForScreen(int s)
{
    if(s == S_PWR) return PLBL;
    if(s == S_CRG) return CRG_TABS;
    if(s == S_PRD) return PRD_TABS;
    if(s == S_WRK) return WRK_TABS;
    if(s == S_LIF) return LIF_TABS;
    if(s == S_GRD) return new string[]{"GRID OVERVIEW"};
    return new string[]{"INFO"};
}

string[]BL2(int s)
{
    return new[]{"[1] Prev Tab","[2] Next Tab","[3] Cycle Windows","[4] Auto-Scroll"};
}

Color Darken(Color c,float f,int ra,int ga,int ba){return new Color((int)(c.R*f)+ra,(int)(c.G*f)+ga,(int)(c.B*f)+ba);}
void DrawStatusBar()
{
    if(_statS==null)return;
    float bat=BatPct();
    _statS.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    _statS.FontColor=new Color(0,220,180);_statS.BackgroundColor=new Color(5,10,20);
    _statS.Font="Monospace";_statS.FontSize=0.85f;
    string s="SBOS v3.4 | "+Me.CubeGrid.CustomName+" | "+(_scr>=0?SNAME[_scr]:"READY")+"\n";
    s+="PWR ["+FB((int)(bat*20f),20)+"] "+(bat*100f).ToString("F0")+"%";
    if(_dn.Count>0)s+=" ! DMG:"+_dn.Count;
    _statS.WriteText(s);
}

void R(ref MySpriteDrawFrame f,Vector2 c,Vector2 s,Color col){f.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",c,s,col));}
void T(ref MySpriteDrawFrame f,string txt,Vector2 p,Color col,float sc,TextAlignment al=TextAlignment.LEFT){f.Add(new MySprite(SpriteType.TEXT,txt,p,null,col,"Monospace",al,sc));}

List<string> BldL(int s, int tab = 0)
{
    switch(s){
        case S_PWR:return PwrL(tab);
        case S_CRG:return CrgL(tab);
        case S_PRD:return PrdL(tab);
        case S_DOC:return DocL();
        case S_LIF:return LifL(tab);
        case S_DMG:return DmgL();
        case S_WRK:return WrkL(tab);
        case S_GRD:return GrdL();
    }
    return new List<string>{"No screen enabled in Custom Data"};
}

string BL(string t,float r,string l){return t+Clamp(r,0f,1f).ToString("F4")+"|"+l;}

// ====================== CONTENT BUILDERS ======================
List<string> PwrL(int tab)
{
    var L=new List<string>();
    float bc=0,bm=0,bo=0,mo=0,ro=0,so=0,wo=0,solarEff=0;
    foreach(var b in _bat){bc+=b.CurrentStoredPower;bm+=b.MaxStoredPower;bo+=b.CurrentOutput;mo+=b.MaxOutput;}
    foreach(var r in _rct)ro+=r.CurrentOutput;
    foreach(var s in _sol){so+=s.CurrentOutput; if(s.MaxOutput>0) solarEff += (s.CurrentOutput / s.MaxOutput);}
    foreach(var w in _wnd)wo+=w.CurrentOutput;
    float ti=ro+so+wo,br=bm>0?bc/bm:0f,lr=mo>0?bo/mo:0f,net=ti-bo;
    if(_sol.Count>0) solarEff /= _sol.Count;
    L.Add(" -- "+PLBL[tab]+" --");L.Add("");
    if(tab==0)
    {
        L.Add(BL(TP,br,"Battery"));
        L.Add(" Input : "+FmtMW(ti));
        L.Add(" Output : "+FmtMW(bo));
        L.Add(" Net : "+(net>=0?"+":"")+FmtMW(net));
        if(net > 0 && bm > bc)
        {
            float timeToFull = (bm - bc) / net;
            L.Add(" Charge Time : "+(timeToFull/60f).ToString("F1")+" min");
        }
        else if(net < 0 && bc > 0)
        {
            float timeToEmpty = bc / -net;
            L.Add(" Discharge Time : "+(timeToEmpty/60f).ToString("F1")+" min");
        }
        L.Add("");
        L.Add(" REACTORS ("+_rct.Count+")");
        foreach(var r in _rct)
        {
            float invR = GetInventoryRatio(r);
            L.Add(BL(TP,invR,Tr(r.CustomName,26) + " " + FmtMW(r.CurrentOutput)));
        }
        L.Add("");
        L.Add(" SOLAR ("+_sol.Count+") Eff: "+(solarEff*100f).ToString("F0")+"%");
        foreach(var s in _sol)
        {
            float eff = s.MaxOutput>0 ? s.CurrentOutput/s.MaxOutput : 0f;
            L.Add(" "+Pad(Tr(s.CustomName,24),24)+" "+FmtMW(s.CurrentOutput)+" ("+(eff*100f).ToString("F0")+"%)");
        }
    }
    else if(tab==1)
    {
        float maxNet=0, minNet=0;
        if(_netHistory.Count>0)
        {
            maxNet=_netHistory[0];
            minNet=_netHistory[0];
            foreach(var v in _netHistory){if(v>maxNet)maxNet=v;if(v<minNet)minNet=v;}
        }
        L.Add(" NET POWER HISTORY");
        L.Add(" Last "+_netHistory.Count+" ticks");
        L.Add(" Max: "+FmtMW(maxNet));
        L.Add(" Min: "+FmtMW(minNet));
        L.Add(" Current net: "+(net>=0?"+":"")+FmtMW(net));
    }
    else if(tab==2)
    {
        L.Add(" REACTORS ("+_rct.Count+") "+FmtMW(ro));
        foreach(var r in _rct)L.Add(" "+Pad(Tr(r.CustomName,26),26)+" "+FmtMW(r.CurrentOutput));
        L.Add(" SOLAR ("+_sol.Count+") "+FmtMW(so)+" Eff: "+(solarEff*100f).ToString("F0")+"%");
        foreach(var s in _sol)L.Add(" "+Pad(Tr(s.CustomName,26),26)+" "+FmtMW(s.CurrentOutput));
        L.Add(" WIND ("+_wnd.Count+") "+FmtMW(wo));
        foreach(var w in _wnd)L.Add(" "+Pad(Tr(w.CustomName,26),26)+" "+FmtMW(w.CurrentOutput));
    }
    else
    {
        L.Add(" Draw : "+FmtMW(bo));
        L.Add(" Supply: "+FmtMW(mo));
        L.Add(BL(TP,lr,"Load "+(lr*100f).ToString("F0")+"%"));
        L.Add(" Net : "+(net>=0?"+":"")+FmtMW(net));
    }
    return L;
}

List<string> PrdL(int tab)
{
    var L=new List<string>();
    L.Add(" -- "+PRD_TABS[tab]+" --");L.Add("");

    if(tab==0 || tab==1)
    {
        L.Add(" ASSEMBLERS ("+_asm.Count+")");
        foreach(var a in _asm)
        {
            float invR = GetInventoryRatio(a);
            L.Add(BL(TP,invR,Tr(a.CustomName,28) + (a.IsProducing?" [PRODUCING]":" [IDLE]")));
            if(!a.IsQueueEmpty)
            {
                var q=new List<MyProductionItem>(); a.GetQueue(q);
                for(int i=0;i<q.Count;i++)
                    L.Add("   → "+Tr(q[i].BlueprintId.SubtypeName,28)+" x"+((float)q[i].Amount).ToString("F0"));
            }
            else L.Add("   Queue empty");
        }
        L.Add("");
    }

    if(tab==0 || tab==2)
    {
        L.Add(" REFINERIES ("+_ref.Count+")");
        foreach(var r in _ref)
        {
            float invR = GetInventoryRatio(r);
            L.Add(BL(TP,invR,Tr(r.CustomName,28) + (r.IsProducing?" [REFINING]":" [IDLE]")));
            if(!r.IsQueueEmpty)
            {
                var q=new List<MyProductionItem>(); r.GetQueue(q);
                for(int i=0;i<q.Count;i++)
                    L.Add("   → "+Tr(q[i].BlueprintId.SubtypeName,28));
            }
        }
        L.Add("");
    }

    if(tab==0 || tab==3)
    {
        L.Add(" REACTORS ("+_rct.Count+")");
        foreach(var r in _rct)
        {
            float invR = GetInventoryRatio(r);
            L.Add(BL(TP,invR,Tr(r.CustomName,28) + " " + FmtMW(r.CurrentOutput)));
        }
        L.Add("");
    }

    if(tab==0 || tab==4)
    {
        L.Add(" H2/O2 GENERATORS ("+_gg.Count+")");
        foreach(var g in _gg)
        {
            float invR = GetInventoryRatio(g);
            L.Add(BL(TP,invR,Tr(g.CustomName,28) + (g.Enabled?" [ON]":" [OFF]")));
        }
    }
    return L;
}

List<string> GrdL()
{
    var L = new List<string>();
    L.Add(" -- GRID OVERVIEW --");
    L.Add("");
    L.Add(" Grid Name : " + Me.CubeGrid.CustomName);
    L.Add(" Total Blocks : N/A (removed for compatibility)");
    L.Add("");
    L.Add(" Batteries   : " + _bat.Count);
    L.Add(" Reactors    : " + _rct.Count);
    L.Add(" Solar Panels: " + _sol.Count);
    L.Add(" H2/O2 Gen   : " + _gg.Count);
    L.Add(" Assemblers  : " + _asm.Count);
    L.Add(" Refineries  : " + _ref.Count);
    L.Add(" Doors       : " + _totalDoors);
    L.Add(" Drills      : " + _drl.Count);
    L.Add(" Cargo       : " + _crg.Count);
    L.Add(" Connectors  : " + _con.Count);
    L.Add("");
    L.Add(" Damage Blocks : " + _dn.Count);
    float net = 0;
    foreach(var r in _rct) net += r.CurrentOutput;
    foreach(var s in _sol) net += s.CurrentOutput;
    foreach(var w in _wnd) net += w.CurrentOutput;
    foreach(var b in _bat) net -= b.CurrentOutput;
    L.Add(" Net Power : " + (net >= 0 ? "+" : "") + FmtMW(net));
    return L;
}

float GetInventoryRatio(IMyProductionBlock b)
{
    if(b==null || b.InventoryCount==0) return 0f;
    var inv = b.GetInventory(0);
    return inv!=null ? (float)inv.CurrentVolume / (float)inv.MaxVolume : 0f;
}

float GetInventoryRatio(IMyGasGenerator g)
{
    if(g==null || g.InventoryCount==0) return 0f;
    var inv = g.GetInventory(0);
    return inv!=null ? (float)inv.CurrentVolume / (float)inv.MaxVolume : 0f;
}

float GetInventoryRatio(IMyTerminalBlock b)
{
    if(b==null || b.InventoryCount==0) return 0f;
    var inv = b.GetInventory(0);
    return inv!=null ? (float)inv.CurrentVolume / (float)inv.MaxVolume : 0f;
}

List<string> CrgL(int tab)
{
    var L=new List<string>();var items=new Dictionary<string,float>();float vc=0,vm=0;
    foreach(var c in _crg)for(int i=0;i<c.InventoryCount;i++){var inv=c.GetInventory(i);vc+=(float)inv.CurrentVolume;vm+=(float)inv.MaxVolume;var il=new List<MyInventoryItem>();inv.GetItems(il);foreach(var it in il){string n=it.Type.SubtypeId;float a=(float)it.Amount;if(items.ContainsKey(n))items[n]+=a;else items[n]=a;}}
    float vr=vm>0?vc/vm:0f;
    L.Add(" Containers: "+_crg.Count);
    L.Add(BL(TC,vr,"Volume "+FmtVol(vc)+" / "+FmtVol(vm)));
    L.Add(" ─────────────────────────────────────");
    L.Add(" -- "+CRG_TABS[tab]+" --");
    var filtered = new Dictionary<string,float>();
    foreach(var kv in items)
    {
        string subtype = kv.Key.ToLower().Trim();
        bool isOre = subtype.Contains("ore") || subtype == "stone" || subtype == "ice";
        bool isIngot = !isOre && (subtype.Contains("ingot") || subtype.Contains("wafer") || subtype == "steel" || subtype == "steelplate");
        bool isComponent = subtype.Contains("component") || subtype == "steelplate" || subtype == "largetube" || subtype == "smalltube" ||
                           subtype == "computer" || subtype == "display" || subtype == "girder" || subtype == "interiorplate" ||
                           subtype == "motor" || subtype.Contains("bulletproof") || subtype == "superconductor" || subtype == "construction" ||
                           subtype == "medical" || subtype == "metalgrid" || subtype == "powercell" || subtype == "radiocomm" ||
                           subtype == "reactor" || subtype == "solarcell" || subtype.Contains("thruster") || subtype.Contains("prototech") || subtype == "zonechip";
        bool isFood = subtype.Contains("food") || subtype.Contains("apple") || subtype.Contains("bacon") || subtype.Contains("bread") || subtype.Contains("canned") ||
                      subtype.Contains("coffee") || subtype.Contains("meat") || subtype.Contains("ration") || subtype.Contains("vegetable") || subtype.Contains("clangcola") ||
                      subtype.Contains("mushroom") || subtype.Contains("fruit");
        bool isAmmo = subtype.Contains("ammo") || subtype.Contains("magazine") || subtype.Contains("missile") || subtype.Contains("rocket") || subtype.Contains("weapon") ||
                      subtype.Contains("pistol") || subtype.Contains("rifle") || subtype.Contains("launcher");
        bool isTool = subtype.Contains("tool") || subtype.Contains("welder") || subtype.Contains("grinder") || subtype.Contains("drill") || subtype.Contains("hand") ||
                      subtype.Contains("anglegrinder");
        if(tab == 0) { filtered[subtype] = kv.Value; continue; }
        if(tab == 1 && isIngot) filtered[subtype] = kv.Value;
        else if(tab == 2 && isOre) filtered[subtype] = kv.Value;
        else if(tab == 3 && isComponent) filtered[subtype] = kv.Value;
        else if(tab == 4 && isFood) filtered[subtype] = kv.Value;
        else if(tab == 5 && isAmmo) filtered[subtype] = kv.Value;
        else if(tab == 6 && isTool) filtered[subtype] = kv.Value;
        else if(tab == 7 && !isIngot && !isOre && !isComponent && !isFood && !isAmmo && !isTool) filtered[subtype] = kv.Value;
    }
    var keys=new List<string>(filtered.Keys);keys.Sort((a,b)=>filtered[b].CompareTo(filtered[a]));
    foreach(var k in keys)
    {
        string iconTexture = GetItemIconTexture(k);
        L.Add("I|" + iconTexture + "|" + Tr(k,32) + "|" + FmtAmt(filtered[k]));
    }
    if(keys.Count==0)L.Add(" No items found in this tab.");
    L.Add("");L.Add(" Total: "+keys.Count+" type(s)");
    return L;
}

string GetItemIconTexture(string subtype)
{
    subtype = subtype.ToLower();
    if(subtype.Contains("ingot") || subtype.Contains("wafer"))
    {
        if(subtype.Contains("iron")) return "MyObjectBuilder_Ingot/Iron";
        if(subtype.Contains("silver")) return "MyObjectBuilder_Ingot/Silver";
        if(subtype.Contains("gold")) return "MyObjectBuilder_Ingot/Gold";
        if(subtype.Contains("platinum")) return "MyObjectBuilder_Ingot/Platinum";
        if(subtype.Contains("silicon")) return "MyObjectBuilder_Ingot/Silicon";
        if(subtype.Contains("nickel")) return "MyObjectBuilder_Ingot/Nickel";
        if(subtype.Contains("cobalt")) return "MyObjectBuilder_Ingot/Cobalt";
        if(subtype.Contains("magnesium")) return "MyObjectBuilder_Ingot/Magnesium";
        if(subtype.Contains("uranium")) return "MyObjectBuilder_Ingot/Uranium";
        if(subtype.Contains("steel")) return "MyObjectBuilder_Ingot/Steel";
    }
    if(subtype.Contains("ore") || subtype == "stone" || subtype == "ice")
    {
        if(subtype.Contains("iron")) return "MyObjectBuilder_Ore/Iron";
        if(subtype.Contains("silver")) return "MyObjectBuilder_Ore/Silver";
        if(subtype.Contains("gold")) return "MyObjectBuilder_Ore/Gold";
        if(subtype.Contains("platinum")) return "MyObjectBuilder_Ore/Platinum";
        if(subtype.Contains("silicon")) return "MyObjectBuilder_Ore/Silicon";
        if(subtype.Contains("nickel")) return "MyObjectBuilder_Ore/Nickel";
        if(subtype.Contains("cobalt")) return "MyObjectBuilder_Ore/Cobalt";
        if(subtype.Contains("magnesium")) return "MyObjectBuilder_Ore/Magnesium";
        if(subtype.Contains("uranium")) return "MyObjectBuilder_Ore/Uranium";
        if(subtype == "stone") return "MyObjectBuilder_Ore/Stone";
        if(subtype == "ice") return "MyObjectBuilder_Ore/Ice";
    }
    if(subtype == "steelplate") return "MyObjectBuilder_Component/SteelPlate";
    if(subtype == "largetube") return "MyObjectBuilder_Component/LargeTube";
    if(subtype == "smalltube") return "MyObjectBuilder_Component/SmallTube";
    if(subtype.Contains("component") || subtype == "construction") return "MyObjectBuilder_Component/Construction";
    if(subtype == "girder") return "MyObjectBuilder_Component/Girder";
    if(subtype == "computer") return "MyObjectBuilder_Component/Computer";
    if(subtype == "interiorplate" || subtype.Contains("interior")) return "MyObjectBuilder_Component/InteriorPlate";
    if(subtype == "display") return "MyObjectBuilder_Component/Display";
    if(subtype == "motor") return "MyObjectBuilder_Component/Motor";
    if(subtype.Contains("bulletproof")) return "MyObjectBuilder_Component/BulletproofGlass";
    if(subtype.Contains("superconductor")) return "MyObjectBuilder_Component/Superconductor";
    if(subtype.Contains("ammo") || subtype.Contains("magazine")) return "MyObjectBuilder_AmmoMagazine/NATO_25x184mm";
    if(subtype.Contains("missile") || subtype.Contains("rocket")) return "MyObjectBuilder_AmmoMagazine/Missile200mm";
    if(subtype.Contains("weapon") || subtype.Contains("pistol") || subtype.Contains("rifle")) return "MyObjectBuilder_AmmoMagazine/NATO_25x184mm";
    if(subtype.Contains("food")) return "MyObjectBuilder_ConsumableItem/Apple";
    if(subtype.Contains("spacecredit") || subtype.Contains("credit")) return "MyObjectBuilder_Component/SpaceCredit";
    if(subtype.Contains("tool") || subtype.Contains("welder") || subtype.Contains("grinder") || subtype.Contains("drill") || subtype.Contains("hand") || subtype.Contains("anglegrinder")) return "MyObjectBuilder_Component/Welder";
    return "MyObjectBuilder_Component/Construction";
}

List<string> DocL()
{
    var L=new List<string>();int docked=0;var seen=new List<long>();
    var allC=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(allC);
    var mine=new List<IMyShipConnector>();foreach(var c in allC)if(c.IsSameConstructAs(Me))mine.Add(c);
    foreach(var con in mine)
    {
        bool conn=con.Status==MyShipConnectorStatus.Connected;
        string st=conn?"DOCKED":con.Status==MyShipConnectorStatus.Connectable?"READY ":"FREE ";
        L.Add(" ["+(conn?"O":"o")+"] "+Pad(Tr(con.CustomName,24),24)+" ["+st+"]");
        if(!conn||con.OtherConnector==null){L.Add("");continue;}
        docked++;
        IMyCubeGrid dg=con.OtherConnector.CubeGrid;
        bool already=false;foreach(var id in seen)if(id==dg.EntityId){already=true;break;}
        if(already){L.Add(" (same ship — see above)");L.Add("");continue;}
        seen.Add(dg.EntityId);
        L.Add(" ┌─ "+Tr(dg.CustomName,36));
        var db=new List<IMyBatteryBlock>();GridTerminalSystem.GetBlocksOfType(db,b=>b.CubeGrid.EntityId==dg.EntityId);
        if(db.Count>0){float bc2=0,bm2=0,bo2=0;foreach(var b in db){bc2+=b.CurrentStoredPower;bm2+=b.MaxStoredPower;bo2+=b.CurrentOutput;}L.Add(BL(TP,bm2>0?bc2/bm2:0f,"PWR "+FmtMW(bc2)+"/"+FmtMW(bm2)+" draw:"+FmtMW(bo2)));}
        var dr=new List<IMyReactor>();GridTerminalSystem.GetBlocksOfType(dr,b=>b.CubeGrid.EntityId==dg.EntityId);
        if(dr.Count>0){float ro=0;foreach(var r in dr)ro+=r.CurrentOutput;L.Add(" | Reactors: "+dr.Count+" out:"+FmtMW(ro));}
        var dh=new List<IMyGasTank>();GridTerminalSystem.GetBlocksOfType(dh,b=>b.CubeGrid.EntityId==dg.EntityId&&b.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen"));
        if(dh.Count>0){float hf=0;foreach(var t in dh)hf+=(float)t.FilledRatio;float hr=hf/dh.Count;L.Add(BL(TH,hr,"H2 "+(hr*100f).ToString("F0")+"% ("+dh.Count+" tanks)"));}
        var do2=new List<IMyGasTank>();GridTerminalSystem.GetBlocksOfType(do2,b=>b.CubeGrid.EntityId==dg.EntityId&&!b.BlockDefinition.SubtypeId.ToLower().Contains("hydrogen"));
        if(do2.Count>0){float of=0;foreach(var t in do2)of+=(float)t.FilledRatio;float or2=of/do2.Count;L.Add(BL(TO,or2,"O2 "+(or2*100f).ToString("F0")+"% ("+do2.Count+" tanks)"));}
        var dc=new List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(dc,b=>b.CubeGrid.EntityId==dg.EntityId);
        if(dc.Count>0){float cc=0,cm=0;var di=new Dictionary<string,float>();foreach(var c in dc)for(int i=0;i<c.InventoryCount;i++){var inv=c.GetInventory(i);cc+=(float)inv.CurrentVolume;cm+=(float)inv.MaxVolume;var il=new List<MyInventoryItem>();inv.GetItems(il);foreach(var it in il){string n=it.Type.SubtypeId;float a=(float)it.Amount;if(di.ContainsKey(n))di[n]+=a;else di[n]=a;}}L.Add(BL(TC,cm>0?cc/cm:0f,"CARGO "+FmtVol(cc)+"/"+FmtVol(cm)+" ("+dc.Count+")"));var dk=new List<string>(di.Keys);dk.Sort((a,b)=>di[b].CompareTo(di[a]));int sh=0;foreach(var k in dk){if(sh>=4)break;L.Add(" | "+Pad(Tr(k,26),26)+" "+FmtAmt(di[k]));sh++;}if(dk.Count>4)L.Add(" | ...+"+(dk.Count-4)+" more types");}
        var dblk=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(dblk,b=>b.CubeGrid.EntityId==dg.EntityId);
        var dn2=new List<string>();var dp2=new List<float>();foreach(var b in dblk){var sl=b.CubeGrid.GetCubeBlock(b.Position);if(sl==null||sl.CurrentDamage<=0f)continue;float p=sl.MaxIntegrity>0?(sl.BuildIntegrity-sl.CurrentDamage)/sl.MaxIntegrity:0f;dn2.Add(b.CustomName);dp2.Add(p);}
        if(dn2.Count==0)L.Add(" | Hull: OK");
        else{L.Add(" | ! DAMAGE: "+dn2.Count+" block(s)");for(int i=0;i<Math.Min(dn2.Count,5);i++)L.Add(BL(TD,dp2[i],Pad(Tr(dn2[i],26),26)+" "+(dp2[i]*100f).ToString("F0")+"%"));if(dn2.Count>5)L.Add(" | ...+"+(dn2.Count-5)+" more");}
        L.Add(" └──────────────────────────────────");L.Add("");
    }
    if(mine.Count==0)L.Add(" No connectors found on this grid.");
    L.Add(" Connectors: "+mine.Count+" Docked: "+docked+" Free: "+(mine.Count-docked));
    return L;
}

List<string> LifL(int tab)
{
    var L = new List<string>();
    float o2r = _o2.Count > 0 ? (float)_o2.Sum(t => t.FilledRatio) / _o2.Count : 0f;
    float h2r = _h2.Count > 0 ? (float)_h2.Sum(t => t.FilledRatio) / _h2.Count : 0f;
    int ggOn = _gg.Count(g => g.Enabled);
    int vok = _ven.Count(v => v.CanPressurize);
    L.Add(" -- "+LIF_TABS[tab]+" --");
    L.Add("");
    if(tab == 0)
    {
        L.Add(" O2 TANKS: "+_o2.Count+" ("+(o2r*100f).ToString("F0")+"%)");
        L.Add(" H2 TANKS: "+_h2.Count+" ("+(h2r*100f).ToString("F0")+"%)");
        L.Add(" GENERATORS: "+ggOn+"/"+_gg.Count);
        L.Add(" VENTS: "+vok+"/"+_ven.Count);
        L.Add(BL(TO,o2r,"OVERALL OXYGEN"));
        L.Add(BL(TH,h2r,"OVERALL HYDROGEN"));
    }
    else if(tab == 1)
    {
        L.Add(" OXYGEN TANKS ("+_o2.Count+")");
        foreach(var t in _o2) L.Add(BL(TO,(float)t.FilledRatio,Tr(t.CustomName,28)));
    }
    else if(tab == 2)
    {
        L.Add(" HYDROGEN TANKS ("+_h2.Count+")");
        foreach(var t in _h2) L.Add(BL(TH,(float)t.FilledRatio,Tr(t.CustomName,28)));
        L.Add("");
        L.Add(" H2/O2 GENERATORS ("+ggOn+"/"+_gg.Count+")");
        foreach(var g in _gg) L.Add(" "+(g.Enabled?"[ON] ":"[OFF]")+" "+Tr(g.CustomName,28));
    }
    else
    {
        L.Add(" AIR VENTS ("+_ven.Count+")");
        foreach(var v in _ven) L.Add(" "+(v.CanPressurize?"[PRESS] ":"[OFF] ")+Tr(v.CustomName,28));
    }
    return L;
}

List<string> DmgL()
{
    var L=new List<string>();
    if(_dn.Count==0){L.Add(" All systems nominal.");L.Add(" No structural damage detected.");}
    else{L.Add(" DAMAGED BLOCKS: "+_dn.Count);L.Add("");for(int i=0;i<_dn.Count;i++)L.Add(BL(TD,_dp[i],Pad(Tr(_dn[i],30),30)+" "+(_dp[i]*100f).ToString("F0")+"%"));}
    return L;
}

List<string> WrkL(int tab)
{
    var L=new List<string>();
    if(tab == 0)
    {
        L.Add(" ── ALL WORKING BLOCKS ──");
        L.AddRange(WrkDrills());
        L.AddRange(WrkPistons());
        L.AddRange(WrkRotors());
        L.AddRange(WrkDoors());
        L.AddRange(WrkLandingGear());
        if(_par.Count>0) L.AddRange(WrkParachutes());
    }
    else if(tab == 1) L.AddRange(WrkDrills());
    else if(tab == 2) L.AddRange(WrkPistons());
    else if(tab == 3) L.AddRange(WrkRotors());
    else if(tab == 4) L.AddRange(WrkDoors());
    else if(tab == 5) L.AddRange(WrkLandingGear());
    else if(tab == 6 && _par.Count>0) L.AddRange(WrkParachutes());
    else L.Add(" No blocks in this tab.");
    return L;
}

List<string> WrkDrills()
{
    var L = new List<string>();
    L.Add(" DRILLS ("+_drl.Count+")");
    if(_drl.Count==0) L.Add(" None found.");
    else
    {
        int on=0;foreach(var d in _drl)if(d.Enabled)on++;
        L.Add(" Active: "+on+" / "+_drl.Count);
        foreach(var d in _drl)
        {
            float invR = GetInventoryRatio(d);
            L.Add(BL(TP,invR,(d.Enabled?"[ON] ":"[OFF]")+" "+Tr(d.CustomName,28)));
        }
    }
    return L;
}

List<string> WrkPistons()
{
    var L = new List<string>();
    L.Add(" PISTONS ("+_pst.Count+")");
    if(_pst.Count==0) L.Add(" None found.");
    foreach(var p in _pst)
    {
        float pos=p.CurrentPosition;
        float min=p.MinLimit; float max=p.MaxLimit;
        float ratio=(max-min)>0?(pos-min)/(max-min):0f;
        string status = p.Enabled ? (Math.Abs(p.Velocity) > 0.01f ? "MOVING" : "HALT") : "OFF";
        string vel=p.Velocity>0?"↑":p.Velocity<0?"↓":"■";
        L.Add(BL(TP,ratio,vel+" "+Tr(p.CustomName,24)+" "+pos.ToString("F1")+"m "+status));
    }
    return L;
}

List<string> WrkRotors()
{
    var L = new List<string>();
    L.Add(" ROTORS ("+_rot.Count+")");
    if(_rot.Count==0) L.Add(" None found.");
    foreach(var r in _rot)
    {
        float deg=r.Angle*(180f/3.14159f);
        string dir = r.TargetVelocityRPM > 0.1f ? "CW" : r.TargetVelocityRPM < -0.1f ? "CCW" : "HALT";
        string vel=r.TargetVelocityRPM>0?"↻":r.TargetVelocityRPM<0?"↺":"■";
        L.Add(" "+vel+" "+Pad(Tr(r.CustomName,26),26)+" "+deg.ToString("F1")+"° "+dir+" "+r.TargetVelocityRPM.ToString("F1")+"rpm");
    }
    return L;
}

List<string> WrkDoors()
{
    var L = new List<string>();
    L.Add(" DOORS ("+_dor.Count+")");
    if(_dor.Count==0) L.Add(" None found.");
    foreach(var d in _dor)
    {
        string st;
        switch(d.Status){
            case DoorStatus.Open: st="[OPEN ]";break;
            case DoorStatus.Closed: st="[CLOSED]";break;
            case DoorStatus.Opening: st="[OPENING]";break;
            case DoorStatus.Closing: st="[CLOSING]";break;
            default: st="[------]";break;
        }
        L.Add(" "+st+" "+Tr(d.CustomName,30));
    }
    return L;
}

List<string> WrkLandingGear()
{
    var L = new List<string>();
    L.Add(" LANDING GEAR ("+_lgr.Count+")");
    if(_lgr.Count==0) L.Add(" None found.");
    foreach(var g in _lgr)
    {
        string st;
        switch(g.LockMode){
            case LandingGearMode.Locked: st="[LOCKED ]";break;
            case LandingGearMode.ReadyToLock:st="[READY ]";break;
            default: st="[UNLOCKED]";break;
        }
        L.Add(" "+st+" "+Tr(g.CustomName,30));
    }
    return L;
}

List<string> WrkParachutes()
{
    var L = new List<string>();
    L.Add(" PARACHUTES ("+_par.Count+")");
    foreach(var p in _par)
    {
        string st=p.Status==DoorStatus.Open?"[DEPLOYED]":p.Enabled?"[ARMED ]":"[OFF ]";
        L.Add(" "+st+" "+Tr(p.CustomName,28));
    }
    return L;
}

float BatPct(){float c=0,m=0;foreach(var b in _bat){c+=b.CurrentStoredPower;m+=b.MaxStoredPower;}return m>0?c/m:0f;}
string FB(int n,int mx){string s="";for(int i=0;i<mx;i++)s+=i<n?"█":"░";return s;}
string FmtMW(float v){if(v>=1000f)return(v/1000f).ToString("F1")+"GW";if(v>=1f)return v.ToString("F1")+"MW";return(v*1000f).ToString("F0")+"kW";}
string FmtVol(float v){if(v>=1000f)return(v/1000f).ToString("F1")+"ML";return v.ToString("F1")+"kL";}
string FmtAmt(float v){if(v>=1000000f)return(v/1000000f).ToString("F1")+"M";if(v>=1000f)return(v/1000f).ToString("F1")+"K";return v.ToString("F0");}
string Tr(string s,int m){if(s==null)return"";return s.Length>m?s.Substring(0,m):s;}
string Pad(string s,int w){if(s==null)s="";if(s.Length>=w)return s.Substring(0,w);return s+new string(' ',w-s.Length);}
float Clamp(float v,float lo,float hi){return v<lo?lo:v>hi?hi:v;}

void DrawPBStatus()
{
    var surf = Me.GetSurface(0);
    if(surf == null) return;
    surf.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    surf.Font = "Monospace";
    surf.FontSize = 0.75f;
    surf.WriteText("=== SBOS v3.4 STATUS ===\n" +
                   "Screens   : " + _surfs.Count + "\n" +
                   "Batteries : " + _bat.Count + " (" + (BatPct()*100f).ToString("F0") + "%)\n" +
                   "Reactors  : " + _rct.Count + "\n" +
                   "Solar     : " + _sol.Count + "\n" +
                   "H2/O2 Gen : " + _gg.Count + "\n" +
                   "Assemblers: " + _asm.Count + "\n" +
                   "Refineries: " + _ref.Count + "\n" +
                   "Doors     : " + _totalDoors + "\n" +
                   "Damage    : " + _dn.Count);
}