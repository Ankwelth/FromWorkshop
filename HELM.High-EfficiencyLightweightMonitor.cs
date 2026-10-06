iI t;string i1=null;Dictionary<string,List<IMyTerminalBlock>>b9=new Dictionary<string,List<IMyTerminalBlock>>();Dictionary<string,bool>gh=new Dictionary<string,bool>();
Dictionary<string,bA>ev=new Dictionary<string,bA>();Dictionary<string,Color>ng=new Dictionary<string,Color>();List<IMyTerminalBlock>cK=new List<IMyTerminalBlock>();
List<IMyTerminalBlock>di=new List<IMyTerminalBlock>();bool ge;int iX;string ew="";StringBuilder ez=new StringBuilder();readonly Dictionary<string,double>aH=new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);
readonly Dictionary<string,string>a9=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);IMyBroadcastListener iY;IMyUnicastListener iZ;
readonly Dictionary<string,double>ni=new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase);bool cL=false;Dictionary<v,IMyTextSurface>ca=new Dictionary<v,IMyTextSurface>();
Dictionary<v,string>b8=new Dictionary<v,string>();static readonly string[]mV=new string[101];static Program(){for(int g_=0;g_<=100;
g_++){mV[g_]=g_+"%";}}static string mI(double wn){int e4=(int)Math.Round(wn);if(e4<0)e4=0;else if(e4>100)e4=100;return mV[e4];
}class aB{public List<MySprite>f=new List<MySprite>();public List<cE>eo=new List<cE>();public int cz= -1;public Color[]fK;
public Color fH;public bool fI;public List<iH>b3=new List<iH>();public Vector2 ei,iC;public int et;public bool eg=false;
public List<fL>d3=new List<fL>();}class iH{public string mT;public int iM;public Vector2 mZ,m5;}class fL{public int f0;public Color h8,fR;
public List<az>ga,fX;public string ic,il,im,ip;public bool mO,ir;}class cE{public bD ek;public int ac= -1;
public int a7= -1;public int dc= -1;public bool mK=false;public List<int>H=new List<int>();public float[]cG=new float[8];
public float bg;public float aE;public float bf;public float bz;public double f2= -99999.0;public string mQ;public float F= -1f;
public bool nb;public float mx=12f;}Dictionary<v,aB>bI=new Dictionary<v,aB>();int nj=0;double i5=0;
double eB=0;double C=0;int ey=0;Dictionary<v,double>a8=new Dictionary<v,double>();Dictionary<v,double>ex=new Dictionary<v,double>();
Dictionary<IMyTextSurface,v>gl=new Dictionary<IMyTextSurface,v>();double gk;bool eA;Dictionary<string,int>gn=new Dictionary<string,int>();
List<string>dj=new List<string>();Dictionary<string,int>go=new Dictionary<string,int>();double i7,i4;HashSet<string>gj=new HashSet<string>();
Dictionary<string,double[]>i3=new Dictionary<string,double[]>();Dictionary<string,double>eD=new Dictionary<string,double>();List<MyInventoryItem>i0=new List<MyInventoryItem>();
int cb= -1,gm;double i6;Dictionary<v,double>gf=new Dictionary<v,double>();int eC=0;const string r5="0.40.0";
IMyTextSurface ba;bool iW=true;double gg=0;string nh="";string i2="";int gi=0,i_=0;List<KeyValuePair<string,int>>dh=new List<KeyValuePair<string,int>>();
const double sn=120;const float rA=22f;public Program(){Runtime.UpdateFrequency=UpdateFrequency.Update10;try{iY=IGC.RegisterBroadcastListener("HELM");
iY.SetMessageCallback("HELM_IGC");iZ=IGC.UnicastListener;iZ.SetMessageCallback("HELM_IGC");}catch{}f9();
iN();mN();}public void Main(string s3,UpdateType wH){double oo=Runtime.TimeSinceLastRun.TotalSeconds;C+=oo;sl();
string qe=(s3 ?? "").Trim();string ap=qe.ToLowerInvariant();if(ap=="reload"){f9();}else if(ap=="refresh"){iJ();}else if(ap=="boot"){
iN();}else if(ap=="probe"){eB=C+20.0;rU();Echo("[HELM] Test pattern on every screen for 20 s");return;}else if(ap=="starter"){if(string.IsNullOrWhiteSpace(Me.CustomData))Me.CustomData=sK;
f9();iN();}else if(ap=="console on"||ap=="console off"){iW=ap=="console on";mN();gg=0;}else if(ap=="names"){if(Me.CustomData.Contains("# --- names")){
Echo("HELM: the names are already at the end of Custom Data. Delete that block to list them again.");return;}var fc=new List<string>();foreach(var uP in d_())fc.Add(uP.CustomName);
var pH=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(pH);foreach(var uQ in pH)fc.Add("[group] "+uQ.Name);fc.Sort();Me.CustomData+="\n# --- names (delete when done) ---\n# "+string.Join("\n# ",fc);
Echo("HELM: "+fc.Count+" names written to the end of Custom Data.");return;}else if(ap=="scan"||ap=="diagnose"||ap=="test"){sG();return;}else if(ap.StartsWith("set ")){
string hx=qe.Substring(4).Trim();int ou=hx.IndexOf('=');int vT=hx.IndexOf(' ');int lJ=ou>0 ? ou : vT;if(lJ>0){string pb=hx.Substring(0,lJ).Trim();
string rf=hx.Substring(lJ+1).Trim();mo($"{pb}={rf}");Echo($"[HELM] Set dynamic variable '{pb}' = {rf}");}return;}else if(ap.StartsWith("alert")){
string fB=ap.Length>5 ? ap.Substring(5).Trim(): "toggle";if(fB=="toggle")cL= !cL;else cL=fB=="red"||fB=="on"||fB=="1"||fB=="true";
Echo($"[HELM] Red alert state: {cL}");return;}else if(Me.CustomData!=i1){f9();}eC++;i5+=oo;if(i5>=sn){
i5=0;iJ();}if(eB>0){if(C<eB){iV();return;}eB=0;bI.Clear();b8.Clear();
gg=0;}if(t==null||t.W.Count==0){iV();mz();Echo("HELM: no valid @screen configured. Check Custom Data.");return;}var bS=t.W[nj%t.W.Count];
nj++;i2=bS.bk;bool jb=a8.Count>0;if(jb)su();double op;if(!a8.ContainsKey(bS)&&(bS.bG!=null|| !(ex.TryGetValue(bS,out op)&&C<op))){
if(sz(bS))jb=true;if(bS.bG==null&&bS.em>0)ex[bS]=C+bS.em/60.0;}ey=jb ? 0 : ey+1;
Runtime.UpdateFrequency=ey<=t.W.Count+1 ? UpdateFrequency.Update10 : UpdateFrequency.Update100;sJ();sI();sH();iV();
mz();gi=Runtime.CurrentInstructionCount;if(gi>i_)i_=gi;}void mz(){if(ew.Length>0)Echo(ew);if(ez.Length>0)Echo(ez.ToString());
}List<IMyTerminalBlock>d_(bool sU=false){if(!ge){di.Clear();GridTerminalSystem.GetBlocks(di);cK.Clear();cK.AddRange(di);
cK.RemoveAll(s7=> !s7.IsSameConstructAs(Me));iX=di.Count-cK.Count;ge=true;}return sU ? di : cK;}IMyTerminalBlock mC(string pD,bool sW=false){
var jm=GridTerminalSystem.GetBlockWithName(pD);if(jm==null||sW||jm.IsSameConstructAs(Me))return jm;var jd=d_();for(int g0=0;g0<jd.Count;g0++)if(jd[g0].CustomName.IndexOf(pD,StringComparison.OrdinalIgnoreCase)>=0)return jd[g0];
return null;}void rL(string pE,List<IMyTerminalBlock>e7,bool np=false){var oN=GridTerminalSystem.GetBlockGroupWithName(pE);if(oN==null)return;int nG=e7.Count;
var fy=new List<IMyTerminalBlock>();oN.GetBlocks(fy);mm(fy,e7,np);if(e7.Count>nG)return;var oY=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(oY);
foreach(var ru in oY){if(ru.Name.IndexOf(pE,StringComparison.OrdinalIgnoreCase)<0)continue;fy.Clear();ru.GetBlocks(fy);mm(fy,e7,np);if(e7.Count>nG)return;
}}void mm(List<IMyTerminalBlock>t1,List<IMyTerminalBlock>o8,bool sV=false){foreach(var jn in t1)if((sV||jn.IsSameConstructAs(Me))&& !o8.Contains(jn))o8.Add(jn);
}void f9(){i1=Me.CustomData;t=mM.se(i1);ew="";if(t.a2.Count>0){var qu=new StringBuilder("HELM config errors:\n");
foreach(var tL in t.a2)qu.AppendLine(" - "+tL);ew=qu.ToString();}if(t.eu.Count>0){var rs=new StringBuilder("HELM notes:\n");foreach(var ws in t.eu)rs.AppendLine(" - "+ws);
ew+=rs.ToString();}ev.Clear();foreach(var jo in t.cA)if(!ev.ContainsKey(jo.ai))ev[jo.ai]=jo;bI.Clear();b8.Clear();
a8.Clear();ex.Clear();gf.Clear();gl.Clear();gk=C;r_();iJ();}void sl(){try{
m_(iY);m_(iZ);}catch{}}void m_(IMyMessageProvider kT){for(int pB=0;kT!=null&&pB<8&&kT.HasPendingMessage;
pB++){var kY=kT.AcceptMessage();if(kY.Tag=="HELM"&&kY.Data is string)mo((string)kY.Data);}}void mo(string p0){if(string.IsNullOrEmpty(p0))return;
var pX=p0.Split(';');for(int ko=0;ko<pX.Length;ko++){var fj=pX[ko].Trim();if(fj.Length==0)continue;int eQ=fj.IndexOf('=');if(eQ<0)eQ=fj.IndexOf(':');
if(eQ>0){string bq=fj.Substring(0,eQ).Trim();string bw=fj.Substring(eQ+1).Trim();if(bq.Equals("alert",StringComparison.OrdinalIgnoreCase)){cL=bw.Equals("red",StringComparison.OrdinalIgnoreCase)||bw=="1"||bw.Equals("true",StringComparison.OrdinalIgnoreCase)||bw.Equals("on",StringComparison.OrdinalIgnoreCase);
}else{double hh;if(double.TryParse(bw,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out hh)){if(double.IsNaN(hh)||double.IsInfinity(hh))continue;
a9.Remove(bq);aH[bq]=hh;}else if(bw.Equals("true",StringComparison.OrdinalIgnoreCase)||bw.Equals("on",StringComparison.OrdinalIgnoreCase)){
a9.Remove(bq);aH[bq]=100.0;}else if(bw.Equals("false",StringComparison.OrdinalIgnoreCase)||bw.Equals("off",StringComparison.OrdinalIgnoreCase)){
a9.Remove(bq);aH[bq]=0.0;}else{aH[bq]=0.0;a9[bq]=bw;}ni[bq]=C;}}}}const string sK="# HELM starter. Set output: to the name of your LCD (any part of the name works), then run this block with the argument: reload\n@bind batt = block(\"Battery\")\n@screen Demo\noutput: \"LCD\", 0\nlayout: grid(1, 1)\n@panel p at(0, 0, 1, 1) title: \"HELM\"\nline: \"BATTERY \" bar(batt.percent) gradient(#ef4444, #eab308, #22c55e)\n";
void rU(){var je=new List<IMyTextSurface>();if(ba!=null)je.Add(ba);foreach(var uR in ca.Values)je.Add(uR);foreach(var vO in je)rT(vO);
}static void bF(List<MySprite>vU,float tE,float tF,float wt,float t8,Color tr){vU.Add(new MySprite{Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=new Vector2(tE,tF),Size=new Vector2(wt,t8),Color=tr}
);}void rT(IMyTextSurface lK){Vector2 aY=lK.SurfaceSize,aM=lK.TextureSize,au=(aM-aY)/2f;var aL=new List<MySprite>();Color gX=new Color(150,150,150);
bF(aL,aM.X/2f,2f,aM.X,4f,gX);bF(aL,aM.X/2f,aM.Y-2f,aM.X,4f,gX);bF(aL,2f,aM.Y/2f,4f,aM.Y,gX);bF(aL,aM.X-2f,aM.Y/2f,4f,aM.Y,gX);
bF(aL,au.X+aY.X/2f,au.Y+aY.Y/2f,aY.X,aY.Y,new Color(20,40,90));float at=32f;bF(aL,au.X+at/2f,au.Y+at/2f,at,at,new Color(255,0,0));bF(aL,au.X+aY.X-at/2f,au.Y+at/2f,at,at,new Color(0,255,0));
bF(aL,au.X+at/2f,au.Y+aY.Y-at/2f,at,at,new Color(255,255,0));bF(aL,au.X+aY.X-at/2f,au.Y+aY.Y-at/2f,at,at,new Color(255,0,255));float mk=au.Y+aY.Y/2f-30f;
be(aL,"SURFACE "+aY.X.ToString("0")+"x"+aY.Y.ToString("0"),Color.White,au.X+44f,mk,0.9f);be(aL,"TEXTURE "+aM.X.ToString("0")+"x"+aM.Y.ToString("0"),Color.White,au.X+44f,mk+24f,0.9f);
be(aL,"OFFSET  "+au.X.ToString("0")+","+au.Y.ToString("0"),Color.White,au.X+44f,mk+48f,0.9f);using(var tV=lK.DrawFrame()){tV.AddRange(aL);}}static bool mP(string bL){
return bL=="rate"||bL=="eta"||bL=="trend"||bL.Length>3&&(bL.StartsWith("top")&&char.IsDigit(bL[3])||bL.Length>4&&bL.StartsWith("line")&&char.IsDigit(bL[4]));}void r_(){
gn.Clear();dj.Clear();go.Clear();gj.Clear();i3.Clear();cb= -1;i6=i7=i4=0;foreach(var lF in t.W){var kU=new List<List<bD>>();
foreach(var vf in lF.an)kU.Add(vf.av);if(lF.an.Count==0)kU.Add(lF.f3);foreach(var uJ in kU)foreach(var bM in uJ){if(!mP(bM.I))continue;
if(bM.I=="rate"||bM.I=="eta"||bM.I=="trend"){gj.Add(bM.aO);continue;}bool q8=bM.I[0]=='t';var og=q8 ? gn : go;int uL=int.Parse(bM.I.Substring(q8 ? 3 : 4));
int o1;og.TryGetValue(bM.aO,out o1);og[bM.aO]=Math.Max(o1,uL);}}dj.AddRange(gn.Keys);}void sI(){if(go.Count==0||C<i7)return;
i7=C+2.0;foreach(var kK in go){List<IMyTerminalBlock>jw;var qD=b9.TryGetValue(kK.Key,out jw)&&jw.Count>0 ? jw[0]as IMyTextSurface : null;
string[]pu=qD==null ? new string[0]: qD.GetText().Split('\n');for(int fn=1;fn<=kK.Value;fn++)a9[kK.Key+".line"+fn]=fn<=pu.Length ? pu[fn-1].TrimEnd('\r'): "";}}
void sH(){if(gj.Count==0||C<i4)return;i4=C+5.0;foreach(var dn in gj){double h2=a5(dn,"percent"),cq=0.0;double[]gY;
if(i3.TryGetValue(dn,out gY))cq=gY[2]*0.6+(h2-gY[0])/Math.Max(0.1,C-gY[1])*0.4;i3[dn]=new[]{h2,C,cq};double fl=cq*60.0;int c1=(int)Math.Min(359999.0,(cq<0 ? h2/ -cq : cq>0 ?(100.0-h2)/cq : 0.0)+0.5);
a9[dn+".rate"]=(fl>=0 ? "+" : "")+fl.ToString("0.0")+"%/min";a9[dn+".trend"]=fl>0.05 ? "^" : fl< -0.05 ? "v" : "=";a9[dn+".eta"]=Math.Abs(fl)<0.05 ? "--" :(cq<0 ? "empty " : "full ")+(c1>=3600 ? c1/3600+"h "+c1%3600/60+"m" : c1>=60 ? c1/60+"m "+c1%60+"s" : c1+"s");
}}void sJ(){if(dj.Count==0)return;if(cb<0){if(C<i6)return;cb=0;gm=0;eD.Clear();}while(cb<dj.Count&&Runtime.CurrentInstructionCount<20000){
string jz=dj[cb];List<IMyTerminalBlock>gx;b9.TryGetValue(jz,out gx);if(gx!=null&&gm<gx.Count){var nx=gx[gm++];
for(int kp=0;kp<nx.InventoryCount;kp++){i0.Clear();nx.GetInventory(kp).GetItems(i0);foreach(var g7 in i0){string kG=g7.Type.TypeId.Replace("MyObjectBuilder_","");
string pc=kG=="Ore"||kG=="Ingot" ? g7.Type.SubtypeId+" "+kG.ToLowerInvariant(): g7.Type.SubtypeId;double o2;eD.TryGetValue(pc,out o2);eD[pc]=o2+(double)g7.Amount;
}}continue;}var hv=new List<KeyValuePair<string,double>>(eD);hv.Sort((wv,wB)=>wB.Value.CompareTo(wv.Value));for(int dJ=1;dJ<=gn[jz];dJ++)a9[jz+".top"+dJ]=dJ<=hv.Count ? hv[dJ-1].Key+" "+D.mG(hv[dJ-1].Value,"auto"): "-";
eD.Clear();cb++;gm=0;}if(cb>=dj.Count){cb= -1;i6=C+15.0;}}void am(int v2,string wc){dh.Add(new KeyValuePair<string,int>(wc,v2));
}static string iU(string ly,int pC){return ly.Length>pC ? ly.Substring(0,pC): ly;}void mN(){var lr=Me as IMyTextSurfaceProvider;if(lr==null||lr.SurfaceCount<1)return;
ba=lr.GetSurface(0);ba.ContentType=iW ? ContentType.SCRIPT : ContentType.NONE;ba.Script="";ba.ScriptBackgroundColor=Color.Black;
}void iV(){if(C>=gg){gg=C+(ey>t.W.Count+1 ? 5.0 : 1.0);rI();if(iW&&ba!=null&&eB==0)rS();
}Echo(nh);}void rI(){dh.Clear();int jC=0,nN=0;foreach(var ny in t.cA){List<IMyTerminalBlock>kN;if(ny.b2==bh.ea){
jC++;continue;}if(b9.TryGetValue(ny.ai,out kN)){nN+=kN.Count;if(kN.Count>0)jC++;}}int ov=t.a2.Count;am(0,"HELM-[Kernel-(v"+r5+")]");
am(4,"------------------------------");am(1,"krn-cfg");am(2," screens "+t.W.Count+"  binds "+jC+"/"+t.cA.Count+"  errors "+ov);
am(2," alert "+(cL ? "RED" : "off")+"  blocks "+nN+"  docked-ignored "+iX);if(ov>0)am(3," ! "+iU(t.a2[0],40));
else if(t.eu.Count>0)am(3," ? "+iU(t.eu[0],40));am(1,"cfg-dsp");if(t.W.Count==0)am(3," no @screen: run 'starter'");
int vQ=0;foreach(var lz in t.W){if(++vQ>10){am(2," +"+(t.W.Count-10)+" more");break;}IMyTextSurface lL;string qI="--",lR="no block";
if(ca.TryGetValue(lz,out lL)){qI=lL.SurfaceSize.X.ToString("0")+"x"+lL.SurfaceSize.Y.ToString("0");lR=a8.ContainsKey(lz)? "boot" : "live";
}am(lR=="no block" ? 3 : 2," "+iU(lz.bk,14).PadRight(14)+" "+qI.PadRight(9)+" "+lR);}am(1,"perf");am(2," tick "+eC+"  "+Runtime.LastRunTimeMs.ToString("0.000")+" ms");
am(2," instr "+gi+"  peak "+i_+"  max "+Runtime.MaxInstructionCount);am(2," rate "+Runtime.UpdateFrequency+"  idle "+ey+"  up "+(int)C+"s");
if(i2.Length>0)am(2," last "+i2);var qv=new StringBuilder();foreach(var uo in dh)qv.AppendLine(uo.Key);nh=qv.ToString();}static Color rO(int v3){
switch(v3){case 0: return new Color(232,121,249);case 1: return new Color(192,38,211);case 3: return new Color(255,80,255);case 4: return new Color(71,85,105);
}return new Color(170,180,195);}void rS(){Vector2 cX=ba.SurfaceSize;Vector2 k2=(ba.TextureSize-cX)/2f;int jN=30;foreach(var up in dh)jN=Math.Max(jN,up.Key.Length);
Vector2 jM=ba.MeasureStringInPixels(new StringBuilder("M"),"Monospace",1f);float qw=Math.Min(1f,Math.Min(cX.X*0.94f/(jN*jM.X),cX.Y*0.94f/(dh.Count*jM.Y)));
float ww=k2.X+cX.X*0.03f;float rx=k2.Y+cX.Y*0.03f;var lN=new List<MySprite>();lN.Add(new MySprite{Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=k2+cX/2f,Size=cX,Color=new Color(12,12,16)}
);foreach(var pi in dh){be(lN,pi.Key,rO(pi.Value),ww,rx,qw);rx+=jM.Y*qw;}using(var tW=ba.DrawFrame()){tW.AddRange(lN);}}static readonly List<string>rH=new List<string>{
"        ZZ     ZZ","     ZZ ZZ     ZZ ZZ","   ZZVZ ZZ     ZZ ZVZZ","  ZZZZZ ZZZZZZZZZ ZZZZZ"," ZVZ    ZZ     ZZ    ZVZ"," ZGZ    ZZ     ZZ    ZGZ"," ZVVZ   ZZ     ZZ   ZVVZ","  ZVVZ ZZ       ZZ ZUVZ","   ZZZ Z         Z ZZZ","     ZZZZZZZ ZZZZZZZ","        ZZZZ ZZZZ"}
;void iN(){a8.Clear();gl.Clear();gk=C;if(t==null)return;for(int g1=0;g1<t.W.Count;g1++){var dN=t.W[g1];
if(dN.ih>0f&&dN.m2(0)==dN)a8[dN]=C+(g1/Math.Max(1,dN.ms))*dN.mv/60.0;}}void su(){List<v>eP=null;int t7=(a8.Count+3)/4,ui=0;
foreach(var hb in a8){double la=(C-hb.Value)/hb.Key.ih;if(la<0)continue;if(la>=1.0){if(eP==null)eP=new List<v>();eP.Add(hb.Key);continue;
}if((ui++ +eC)%t7==0)rR(hb.Key,la);}if(eP==null)return;foreach(var fu in eP){a8.Remove(fu);if(fu.ig>0f)gf[fu]=C;bI.Remove(fu);
b8.Remove(fu);}if(a8.Count==0)gk=C;}static void be(List<MySprite>vV,string wd,Color tu,float wx,float wC,float vK){
var qW=MySprite.CreateText(wd,"Monospace",tu,vK,TextAlignment.LEFT);qW.Position=new Vector2(wx,wC);vV.Add(qW);}static void ml(List<MySprite>vW,List<string>gs,v hE,float wy,float wD,float vL,float uA){
string[]v0=hE.fM!=null&&hE.fM.Length>0 ? hE.fM : new[]{hE.mt};for(int eZ=0;eZ<gs.Count;
eZ++){p jF=D.mJ(v0,gs.Count>1 ? 100.0*eZ/(gs.Count-1): 0.0);be(vW,gs[eZ],new Color(jF.b4,jF.b0,jF.bW),wy,wD+eZ*uA,vL);}}bool sy(v dO,IMyTextSurface lW){
aB aD,aT;v qz=t.W.Find(rv=>rv!=dO&&rv.bk.Equals(dO.f6,StringComparison.OrdinalIgnoreCase));if(qz==null|| !bI.TryGetValue(qz,out aD)|| !aD.eg)return false;
if(!bI.TryGetValue(dO,out aT))bI[dO]=aT=new aB();Vector2 dR=lW.SurfaceSize;if(aT.eg&&aT.et==aD.et&&aT.ei==dR)return false;
aT.eg=true;aT.et=aD.et;aT.ei=dR;aT.f.Clear();Vector2 hO=aD.ei,pM=aD.iC,pK=(lW.TextureSize-dR)/2f;aT.iC=pK;
int oK=0,q7=aD.f.Count;if(dO.iy!=null){int dI=aD.b3.FindIndex(wz=>wz.mT.Equals(dO.iy,StringComparison.OrdinalIgnoreCase));
if(dI<0)return false;hO=aD.b3[dI].m5;pM=aD.b3[dI].mZ;oK=aD.b3[dI].iM;if(dI+1<aD.b3.Count)q7=aD.b3[dI+1].iM;}float g9=Math.Min(dR.X/hO.X,dR.Y/hO.Y);
Vector2 s4=pK+(dR-hO*g9)/2f;for(int kq=oK;kq<q7;kq++){var bN=aD.f[kq];if(bN.Position.HasValue)bN.Position=(bN.Position.Value-pM)*g9+s4;if(bN.Size.HasValue)bN.Size=bN.Size.Value*g9;
if(bN.Type==SpriteType.TEXT)bN.RotationOrScale*=g9;aT.f.Add(bN);}using(var tX=lW.DrawFrame())tX.AddRange(aT.f);return true;}bool ss(v dP,IMyTextSurface hM){
List<string>eG;if(b8.ContainsKey(dP)|| !t.d0.TryGetValue(dP.cy,out eG))return false;b8[dP]="banner";Vector2 cY=hM.SurfaceSize;
Vector2 k3=(hM.TextureSize-cY)/2f;int gD=1;foreach(var uq in eG)gD=Math.Max(gD,uq.Length);Vector2 eM=hM.MeasureStringInPixels(new StringBuilder("M"),"Monospace",1f);
float hD=Math.Min(cY.X*0.94f/(gD*eM.X),cY.Y*0.94f/(eG.Count*eM.Y));var lO=new List<MySprite>();lO.Add(new MySprite{Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=k3+cY/2f,Size=cY,Color=string.IsNullOrEmpty(dP.bX)? new Color(10,12,16): L(dP.bX)}
);ml(lO,eG,dP,k3.X+(cY.X-gD*eM.X*hD)/2f,k3.Y+(cY.Y-eG.Count*eM.Y*hD)/2f,hD,eM.Y*hD);using(var tY=hM.DrawFrame()){tY.AddRange(lO);
}return true;}void rR(v dQ,double pP){IMyTextSurface fw;if(!ca.TryGetValue(dQ,out fw))return;Vector2 cu=fw.SurfaceSize;
Vector2 k4=(fw.TextureSize-cu)/2f;var fb=dQ.r0();string py=dQ.mu ?? "HELM TELEMETRY KERNEL ONLINE";bool kV=cu.Y>=200f;
List<string>dk;if(dQ.cB==null|| !t.d0.TryGetValue(dQ.cB,out dk))dk=rH;int dt=py.Length+3;foreach(var ur in fb)dt=Math.Max(dt,ur.Length+5);
if(kV)foreach(var us in dk)dt=Math.Max(dt,us.Length);int qm=(kV ? dk.Count+1 : 0)+2+fb.Count;Vector2 ds=fw.MeasureStringInPixels(new StringBuilder("M"),"Monospace",1f);
float cr=Math.Min(1f,Math.Min(cu.X*0.94f/(dt*ds.X),cu.Y*0.94f/(qm*ds.Y)));float h4=k4.X+(cu.X-dt*ds.X*cr)/2f;float c4=k4.Y+(cu.Y-qm*ds.Y*cr)/2f;
float he=ds.Y*cr;var dS=new List<MySprite>();dS.Add(new MySprite{Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=k4+cu/2f,Size=cu,Color=new Color(10,12,16)}
);if(kV){ml(dS,dk,dQ,h4,c4,cr,he);c4+=(dk.Count+1)*he;}be(dS,">> "+py,new Color(34,197,94),h4,c4,cr);c4+=he*2f;int vR=D.rG(pP,fb.Count);
for(int g2=0;g2<vR;g2++){bool pL=g2==fb.Count-1&&pP>=0.8;be(dS,pL ? "[OK]" : "[..]",pL ? new Color(34,197,94): new Color(56,189,248),h4,c4,cr);be(dS,fb[g2],new Color(203,213,225),h4+5f*ds.X*cr,c4,cr);
c4+=he;}using(var tZ=fw.DrawFrame()){tZ.AddRange(dS);}}void iJ(){b9.Clear();ca.Clear();gh.Clear();
ez.Clear();ge=false;if(t==null)return;foreach(var ah in t.cA){var bb=new List<IMyTerminalBlock>();if(ah.b2==bh.ea){
string kE= !string.IsNullOrEmpty(ah.bC)? ah.bC : ah.ai;if(!aH.ContainsKey(kE)){aH[kE]=ah.fS;if(ah.ij!=null)a9[kE]=ah.ij;
}b9[ah.ai]=bb;continue;}var q_=ah.a6.Count>0 ? ah.a6 : new List<string>{ah.m8};foreach(var c2 in q_){
if(string.IsNullOrEmpty(c2))continue;bool pa=c2.Contains("*")||c2.Contains("?");if(ah.b2==bh.iu||pa){var ki=new List<IMyTerminalBlock>();
rL(c2,ki,ah.fJ);if(ki.Count>0){foreach(var oU in ki){if(!bb.Contains(oU))bb.Add(oU);}}else{var nr=d_(ah.fJ);
for(int jp=0;jp<nr.Count;jp++){var hU=nr[jp];if(pa ? D.sa(c2,hU.CustomName): string.Equals(c2,hU.CustomName,StringComparison.OrdinalIgnoreCase)){
if(!bb.Contains(hU))bb.Add(hU);}}}}else{var jv=mC(c2,ah.fJ);if(jv!=null&& !bb.Contains(jv))bb.Add(jv);}}if(bb.Count==0){
sE(ah,q_,bb);if(bb.Count>0)gh[ah.ai]=true;}b9[ah.ai]=bb;}foreach(var bT in t.W){
if(string.IsNullOrEmpty(bT.a4))continue;var k7=mC(bT.a4);if(k7==null){ez.AppendLine($"HELM: output block '{bT.a4}' not found");
continue;}IMyTextSurface c_=null;var ls=k7 as IMyTextSurfaceProvider;var oh=k7 as IMyTextSurface;if(ls!=null&&bT.bE<ls.SurfaceCount){
c_=ls.GetSurface(bT.bE);}else if(oh!=null&&bT.bE==0){c_=oh;}if(c_==null){ez.AppendLine($"HELM: output block '{bT.a4}' has no surface index {bT.bE}");
continue;}c_.ContentType=ContentType.SCRIPT;c_.Script="";c_.ScriptBackgroundColor=Color.Black;ca[bT]=c_;}cK.Clear();
di.Clear();}static readonly string[]mB={"batt,power,pwr","reactor,nuke","solar","wind","tank,hydro,hydrogen,o2,oxygen","vent,air,press","cargo,container,store,inv","jump,drive","thrust,engine","susp,wheel,brake","door,gate,airlock","connect","asm,assembler,fab,prod","refin,smelt,furnace","drill,mine,bore","weld","grind","o2h2,gen,generator","turret,gun,cannon,weapon,defense","proj,blueprint","gyro","grav,gravity","gear,magplate,landing","ant,comm,radio","beacon","light,spot","piston","rotor,hinge","warhead","sensor","camera","parachute","tcb,customturret"}
;static bool rZ(int uj,IMyTerminalBlock w){switch(uj){case 0: return w is IMyBatteryBlock;case 1: return w is IMyReactor;case 2: return w is IMySolarPanel;
case 3: return w is IMyWindTurbine;case 4: return w is IMyGasTank;case 5: return w is IMyAirVent;case 6: return w is IMyCargoContainer;case 7: return w is IMyJumpDrive;
case 8: return w is IMyThrust;case 9: return w is IMyMotorSuspension;case 10: return w is IMyDoor;case 11: return w is IMyShipConnector;case 12: return w is IMyAssembler;
case 13: return w is IMyRefinery;case 14: return w is IMyShipDrill;case 15: return w is IMyShipWelder;case 16: return w is IMyShipGrinder;case 17: return w is IMyGasGenerator;
case 18: return w is IMyLargeTurretBase;case 19: return w is IMyProjector;case 20: return w is IMyGyro;case 21: return w is IMyGravityGeneratorBase;case 22: return w is IMyLandingGear;
case 23: return w is IMyRadioAntenna;case 24: return w is IMyBeacon;case 25: return w is IMyLightingBlock;case 26: return w is IMyPistonBase;case 27: return w is IMyMotorStator;
case 28: return w is IMyWarhead;case 29: return w is IMySensorBlock;case 30: return w is IMyCameraBlock;case 31: return w is IMyParachute;case 32: return w is IMyTurretControlBlock;
}return false;}void sE(bA tj,List<string>wa,List<IMyTerminalBlock>jx){string ud=(tj.ai+" "+string.Join(" ",wa)).ToLowerInvariant();
var kI=new List<int>();for(int ha=0;ha<mB.Length;ha++){foreach(var wu in mB[ha].Split(','))if(ud.Contains(wu)){kI.Add(ha);break;}}if(kI.Count==0)return;
var nq=d_();var pN=new List<IMyTerminalBlock>();for(int kr=0;kr<nq.Count;kr++){var jq=nq[kr];foreach(int uk in kI){if(rZ(uk,jq)){(jq.CubeGrid==Me.CubeGrid ? jx : pN).Add(jq);
break;}}}if(jx.Count==0)jx.AddRange(pN);}void sG(){Echo("=== HELM SELF-DIAGNOSTIC AUDIT ===");if(t==null||t.W.Count==0){
Echo("[WARN] No active @screen configured in Custom Data.");return;}Echo($"Configuration: {t.W.Count} screens, {t.cA.Count} binds");ge=false;
Echo($"Blocks: {d_().Count} on this construct; {iX} on docked ships (ignored)");cK.Clear();Echo("--- BIND RESOLUTION ---");foreach(var ax in t.cA){
if(ax.b2==bh.ea){string kF= !string.IsNullOrEmpty(ax.bC)? ax.bC : ax.ai;double wk=aH.ContainsKey(kF)? aH[kF]: ax.fS;
Echo($"[OK] @bind {ax.ai} (dynamic variable '{kF}' = {wk})");continue;}List<IMyTerminalBlock>jy;if(b9.TryGetValue(ax.ai,out jy)){if(jy.Count>0){
string tR=gh.ContainsKey(ax.ai)&&gh[ax.ai]? " [fallback: typed match]" : "";Echo($"[OK] @bind {ax.ai} ({jy.Count} blocks matched{tR})");
}else{string wb=string.Join(", ",ax.a6.Count>0 ? ax.a6 : new List<string>{ax.m8});Echo($"[WARN] @bind {ax.ai} -> 0 blocks matched \"{wb}\"");
}}else{Echo($"[ERR] @bind {ax.ai} -> unresolved");}}if(aH.Count>0){Echo($"--- DYNAMIC VARIABLES ({aH.Count}) ---");foreach(var pk in aH)Echo($"[VAR] {pk.Key} = {pk.Value}");
}IMyTextSurface jg=null;foreach(var uU in ca.Values){jg=uU;break;}if(jg!=null){var lP=new List<string>();jg.GetSprites(lP);
Echo($"--- SPRITES ({lP.Count}): names usable in bg_texture ---");Echo(string.Join(", ",lP.Take(60).ToArray()));}Echo("--- SURFACE OUTPUT AUDIT ---");
foreach(var bU in t.W){IMyTextSurface hN;if(ca.TryGetValue(bU,out hN)){var qJ=hN.SurfaceSize;var vZ=D.rN(bU);
Echo($"[OK] Screen \"{bU.bk}\" [{bU.f8}] -> {bU.a4} [#{bU.bE}] ({qJ.X:0}x{qJ.Y:0} px, texture {hN.TextureSize.X:0}x{hN.TextureSize.Y:0}, {vZ.aj} sprites)");
}else{Echo($"[ERR] Screen \"{bU.bk}\" -> Missing block \"{bU.a4}\" [#{bU.bE}]");}}Echo("--- PERFORMANCE STATUS ---");
Echo($"Instruction Usage: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount} | Allocs: 0-byte GC");Echo("=== DIAGNOSTIC AUDIT COMPLETE ===");}double sF(IMyTerminalBlock x){
if(x==null)return 0.0;var gv=x as IMyBatteryBlock;if(gv!=null)return gv.MaxStoredPower>0 ?(gv.CurrentStoredPower/gv.MaxStoredPower)*100.0 : 0.0;
var g8=x as IMyJumpDrive;if(g8!=null)return g8.MaxStoredPower>0 ?(g8.CurrentStoredPower/g8.MaxStoredPower)*100.0 : 0.0;var qZ=x as IMyGasTank;
if(qZ!=null)return qZ.FilledRatio*100.0;var ro=x as IMyAirVent;if(ro!=null)return ro.GetOxygenLevel()*100.0;var nV=x as IMyCargoContainer;if(nV!=null){
var g5=nV.GetInventory();if(g5!=null&&g5.MaxVolume.RawValue>0)return(double)g5.CurrentVolume.RawValue/g5.MaxVolume.RawValue*100.0;return 0.0;}var hw=x as IMyReactor;
if(hw!=null)return hw.MaxOutput>0 ?(hw.CurrentOutput/hw.MaxOutput)*100.0 : 0.0;var hG=x as IMySolarPanel;if(hG!=null)return hG.MaxOutput>0 ?(hG.CurrentOutput/hG.MaxOutput)*100.0 : 0.0;
var h3=x as IMyWindTurbine;if(h3!=null)return h3.MaxOutput>0 ?(h3.CurrentOutput/h3.MaxOutput)*100.0 : 0.0;var oi=x as IMyDoor;if(oi!=null)return(double)oi.OpenRatio*100.0;
var hX=x as IMyThrust;if(hX!=null)return hX.MaxEffectiveThrust>0 ?(hX.CurrentThrust/hX.MaxEffectiveThrust)*100.0 : 0.0;var l0=x as IMyMotorSuspension;
if(l0!=null)return l0.Brake ? 100.0 :(l0.Friction*100.0);var jR=x as IMyShipConnector;if(jR!=null)return jR.Status==MyShipConnectorStatus.Connected ? 100.0 :(jR.Status==MyShipConnectorStatus.Connectable ? 50.0 : 0.0);
var p7=x as IMyProductionBlock;if(p7!=null)return p7.IsProducing ? 100.0 : 0.0;var om=x as IMyShipDrill;if(om!=null)return om.Enabled ? 100.0 : 0.0;
var l8=x as IMyShipToolBase;if(l8!=null)return l8.IsActivated ? 100.0 :(l8.Enabled ? 50.0 : 0.0);var kf=x as IMyGasGenerator;if(kf!=null)return kf.AutoRefill ? 100.0 :(kf.Enabled ? 50.0 : 0.0);
var hZ=x as IMyLargeTurretBase;if(hZ!=null)return hZ.IsShooting ? 100.0 :(hZ.HasTarget ? 50.0 :(hZ.Enabled ? 10.0 : 0.0));var fm=x as IMyProjector;
if(fm!=null)return fm.TotalBlocks>0 ?((double)(fm.TotalBlocks-fm.RemainingBlocks)/fm.TotalBlocks)*100.0 : 0.0;var kl=x as IMyGyro;if(kl!=null)return kl.GyroOverride ? 100.0 :(kl.Enabled ? 50.0 : 0.0);
var oW=x as IMyGravityGeneratorBase;if(oW!=null)return Math.Min(100.0,Math.Abs(oW.GravityAcceleration)/9.81*100.0);var ke=x as IMyLandingGear;if(ke!=null)return ke.IsLocked ? 100.0 :(ke.LockMode==LandingGearMode.ReadyToLock ? 50.0 : 0.0);
var ns=x as IMyRadioAntenna;if(ns!=null)return(ns.Radius/50000.0)*100.0;var nE=x as IMyBeacon;if(nE!=null)return(nE.Radius/50000.0)*100.0;
var ps=x as IMyLightingBlock;if(ps!=null)return ps.Intensity*10.0;var hr=x as IMyPistonBase;if(hr!=null)return hr.HighestPosition>0 ?(hr.CurrentPosition/hr.HighestPosition)*100.0 : 0.0;
var qj=x as IMyMotorStator;if(qj!=null)return Math.Min(100.0,Math.Abs(qj.Angle)/6.28318*100.0);var rr=x as IMyWarhead;if(rr!=null)return rr.IsArmed ? 100.0 : 0.0;
var lI=x as IMySensorBlock;if(lI!=null)return!lI.LastDetectedEntity.IsEmpty()? 100.0 :(lI.Enabled ? 25.0 : 0.0);var jI=x as IMyCameraBlock;
if(jI!=null)return jI.IsActive ? 100.0 :(jI.Enabled ? 50.0 : 0.0);var n0=x as IMyParachute;if(n0!=null)return(double)n0.OpenRatio*100.0;
var hW=x as IMyTurretControlBlock;if(hW!=null)return hW.HasTarget ? 100.0 :(hW.IsUnderControl ? 50.0 :(hW.Enabled ? 10.0 : 0.0));var oL=x as IMyFunctionalBlock;
if(oL!=null)return oL.Enabled ? 100.0 : 0.0;return 0.0;}static double r7(IMyTerminalBlock jr,string dx){bool qF=dx.StartsWith("share:",StringComparison.OrdinalIgnoreCase);
if(qF)dx=dx.Substring(6);int nU=dx.IndexOf(':');string kH=dx.Substring(0,nU).ToLowerInvariant();var rb=new MyItemType("MyObjectBuilder_"+(kH=="ammo" ? "AmmoMagazine" : char.ToUpper(kH[0])+kH.Substring(1)),dx.Substring(nU+1));
double lV=0,jK=0;for(int g3=0;g3<jr.InventoryCount;g3++){lV+=(double)jr.GetInventory(g3).GetItemAmount(rb);jK+=(double)jr.GetInventory(g3).MaxVolume;}return qF ?(jK>0 ? lV*rb.GetItemInfo().Volume/jK*100.0 : 0.0): lV;
}double m0(IMyTerminalBlock y,string oB){if(y==null)return 0.0;string c=oB.ToLowerInvariant();if(c=="percent"||c=="pct")return sF(y);
if(c.IndexOf(':')>0)return r7(y,oB);var ce=y as IMyBatteryBlock;if(ce!=null){if(c=="current"||c=="stored"||c=="val"||c=="value"||c=="raw")return ce.CurrentStoredPower;
if(c=="max"||c=="capacity")return ce.MaxStoredPower;if(c=="output")return ce.CurrentOutput;if(c=="maxoutput")return ce.MaxOutput;if(c=="input")return ce.CurrentInput;
if(c=="maxinput")return ce.MaxInput;return ce.CurrentStoredPower;}var kD=y as IMyJumpDrive;if(kD!=null){if(c=="max"||c=="capacity")return kD.MaxStoredPower;
return kD.CurrentStoredPower;}var hR=y as IMyGasTank;if(hR!=null){if(c=="max"||c=="capacity")return hR.Capacity;return hR.FilledRatio*hR.Capacity;
}var rp=y as IMyAirVent;if(rp!=null){if(c=="max"||c=="capacity")return 100.0;return rp.GetOxygenLevel()*100.0;}var nW=y as IMyCargoContainer;if(nW!=null){
var g6=nW.GetInventory();if(g6!=null){if(c=="mass")return(double)g6.CurrentMass;if(c=="max"||c=="capacity")return(double)g6.MaxVolume;return(double)g6.CurrentVolume;
}return 0.0;}var lt=y as IMyPowerProducer;if(lt!=null){if(c=="max"||c=="capacity")return lt.MaxOutput;return lt.CurrentOutput;}var oj=y as IMyDoor;
if(oj!=null){if(c=="max")return 1.0;return(double)oj.OpenRatio*100.0;}var l5=y as IMyThrust;if(l5!=null){if(c=="max"||c=="capacity")return l5.MaxEffectiveThrust;
return l5.CurrentThrust;}var cw=y as IMyMotorSuspension;if(cw!=null){if(c=="brake")return cw.Brake ? 100.0 : 0.0;if(c=="height")return cw.Height;
if(c=="strength")return cw.Strength;if(c=="power")return cw.Power;if(c=="steer")return cw.SteeringOverride*100.0;if(c=="propulsion")return cw.PropulsionOverride*100.0;
return cw.Friction;}var n8=y as IMyShipConnector;if(n8!=null){return n8.Status==MyShipConnectorStatus.Connected ? 100.0 : 0.0;}var lq=y as IMyProductionBlock;
if(lq!=null){if(c=="producing"||c=="active"||c=="running"||c=="state")return lq.IsProducing ? 100.0 : 0.0;if(c=="queue")return lq.IsQueueEmpty ? 0.0 : 1.0;
}var jk=y as IMyAssembler;if(jk!=null){if(c=="disassembling")return jk.Mode==MyAssemblerMode.Disassembly ? 100.0 : 0.0;if(c=="repeating"||c=="loop")return jk.Repeating ? 100.0 : 0.0;
}var kg=y as IMyGasGenerator;if(kg!=null){if(c=="autorefill")return kg.AutoRefill ? 100.0 : 0.0;if(c=="active")return kg.Enabled ? 100.0 : 0.0;}var h_=y as IMyLargeTurretBase;
if(h_!=null){if(c=="shooting")return h_.IsShooting ? 100.0 : 0.0;if(c=="target"||c=="hastarget")return h_.HasTarget ? 100.0 : 0.0;if(c=="range")return h_.Range;
}var cp=y as IMyProjector;if(cp!=null){if(c=="built")return cp.TotalBlocks-cp.RemainingBlocks;if(c=="remaining")return cp.RemainingBlocks;if(c=="total")return cp.TotalBlocks;
if(c=="projecting")return cp.IsProjecting ? 100.0 : 0.0;if(c=="armor")return cp.RemainingArmorBlocks;if(c=="buildable")return cp.BuildableBlocksCount;}var l9=y as IMyShipToolBase;
if(l9!=null){if(c=="working"||c=="active"||c=="activated")return l9.IsActivated ? 100.0 : 0.0;return l9.Enabled ? 100.0 : 0.0;}var dz=y as IMyGyro;if(dz!=null){
if(c=="override")return dz.GyroOverride ? 100.0 : 0.0;if(c=="pitch")return dz.Pitch;if(c=="yaw")return dz.Yaw;if(c=="roll")return dz.Roll;if(c=="power")return dz.GyroPower*100.0;
}var oX=y as IMyGravityGeneratorBase;if(oX!=null){if(c=="g"||c=="accel")return oX.GravityAcceleration;}var oV=y as IMyLandingGear;if(oV!=null){
if(c=="locked")return oV.IsLocked ? 100.0 : 0.0;}var nt=y as IMyRadioAntenna;if(nt!=null){if(c=="radius"||c=="range")return nt.Radius;}var nF=y as IMyBeacon;
if(nF!=null){if(c=="radius"||c=="range")return nF.Radius;}var kQ=y as IMyLightingBlock;if(kQ!=null){if(c=="intensity")return kQ.Intensity;if(c=="radius")return kQ.Radius;
}var hs=y as IMyPistonBase;if(hs!=null){if(c=="pos"||c=="position"||c=="extension")return hs.CurrentPosition;if(c=="velocity")return hs.Velocity;
if(c=="max")return hs.HighestPosition;}var ft=y as IMyMotorStator;if(ft!=null){if(c=="angle"||c=="deg")return ft.Angle*180.0/Math.PI;if(c=="rad")return ft.Angle;
if(c=="velocity"||c=="rpm")return ft.TargetVelocityRPM;if(c=="locked")return ft.RotorLock ? 100.0 : 0.0;}var mi=y as IMyWarhead;if(mi!=null){
if(c=="armed")return mi.IsArmed ? 100.0 : 0.0;if(c=="countdown")return mi.DetonationTime;}var qC=y as IMySensorBlock;if(qC!=null){if(c=="detected")return!qC.LastDetectedEntity.IsEmpty()? 100.0 : 0.0;
}var jJ=y as IMyCameraBlock;if(jJ!=null){if(c=="range")return jJ.AvailableScanRange;if(c=="active")return jJ.IsActive ? 100.0 : 0.0;}var n1=y as IMyParachute;
if(n1!=null){if(c=="open"||c=="openratio")return(double)n1.OpenRatio*100.0;}var dW=y as IMyTurretControlBlock;if(dW!=null){if(c=="target"||c=="hastarget")return dW.HasTarget ? 100.0 : 0.0;
if(c=="azimuth")return dW.AzimuthRotor!=null ? dW.AzimuthRotor.Angle*180.0/Math.PI : 0.0;if(c=="elevation")return dW.ElevationRotor!=null ? dW.ElevationRotor.Angle*180.0/Math.PI : 0.0;
}var oM=y as IMyFunctionalBlock;if(oM!=null){return oM.Enabled ? 100.0 : 0.0;}return 0.0;}double a5(string eJ,string eT){bA bJ;
ev.TryGetValue(eJ,out bJ);if(bJ!=null&&bJ.b2==bh.ea){string pd= !string.IsNullOrEmpty(bJ.bC)? bJ.bC : eJ;
if(eT=="age"||eT=="fresh"){double jl;bool qh=ni.TryGetValue(pd,out jl);return eT=="age" ?(qh ? C-jl : -1):(qh&&C-jl<bJ.my ? 100 : 0);
}double ob;if(aH.TryGetValue(pd,out ob))return ob;return bJ.fS;}double os;if(aH.TryGetValue(eJ,out os))return os;
List<IMyTerminalBlock>eK;if(!b9.TryGetValue(eJ,out eK)||eK.Count==0)throw new InvalidOperationException($"HELM: bind '{eJ}' resolved to zero blocks — check the name/typo");
if(eK.Count==1)return m0(eK[0],eT);var rn=new List<double>();foreach(var s8 in eK)rn.Add(m0(s8,eT));return rC.rB(bJ!=null ? bJ.dZ : aN.h7,rn);
}double iK(string tl){return a5(tl,"percent");}static string bj(double rj,bD dE){if(dE==null||(string.IsNullOrEmpty(dE.es)&&string.IsNullOrEmpty(dE.eb)&&dE.I=="percent")){
return mI(rj);}return D.mG(rj,dE.es,dE.eb);}bool sz(v ab){eA=false;IMyTextSurface c0;
if(!ca.TryGetValue(ab,out c0))return false;if(ab.bG!=null){if(a8.Count>0||ab.m2(C-gk)!=ab)return false;
v p6;eA= !gl.TryGetValue(c0,out p6)||p6!=ab;double oq;if(!eA&&ex.TryGetValue(ab,out oq)&&C<oq)return false;
if(ab.em>0)ex[ab]=C+ab.em/60.0;gl[c0]=ab;if(eA){b8.Remove(ab);if(ab.f6!=null)bI.Remove(ab);
}}if(ab.cy!=null)return ss(ab,c0);if(ab.f6!=null)return sy(ab,c0);if(ab.c8.Equals("ascii",StringComparison.OrdinalIgnoreCase)){
return sA(ab,c0);}return sB(ab,c0);}Color ao(bD pl,Color tG){return pl.iT!=null ? L(pl.iT): tG;
}Color ec(string[]dT,double u6){if(dT.Length==1)return L(dT[0]);double qb=Math.Max(0.0,Math.Min(100.0,u6))/100.0*(dT.Length-1);int ks=Math.Min((int)qb,dT.Length-2);
float hP=(float)(qb-ks);Color cc=L(dT[ks]),gu=L(dT[ks+1]);return new Color((int)(cc.R+(gu.R-cc.R)*hP),(int)(cc.G+(gu.G-cc.G)*hP),(int)(cc.B+(gu.B-cc.B)*hP),(int)(cc.A+(gu.A-cc.A)*hP));
}static Color[]fF(aB tp,string[]v1,string tI,float rw,float ry,float mf,float km){int eH=16;bool o3=tI=="horizontal";var jQ=new Color[eH];
for(int cR=0;cR<eH;cR++){p kd=D.mJ(v1,100.0*cR/(eH-1));jQ[cR]=new Color(kd.b4,kd.b0,kd.bW);float nT=mf/eH,nM=km/eH;q(tp,o3 ? new Vector2(rw+nT*(cR+0.5f),ry+km/2f): new Vector2(rw+mf/2f,ry+nM*(cR+0.5f)),o3 ? new Vector2(nT+1f,km): new Vector2(mf,nM+1f),jQ[cR]);
}return jQ;}static void q(aB tq,Vector2 vh,Vector2 vS,Color tv,float vC=0f,string tH="SquareSimple"){tq.f.Add(new MySprite{Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data=tH,Position=vh,Size=vS,Color=tv,RotationOrScale=vC}
);}void fG(aB cf,string cZ,string[]aS,string n4,bool qL,float aJ,float aZ,float bR,float bl){cZ=cZ.ToLowerInvariant();float V=cZ=="heavy" ? 4f : 2f,fE=bR-aJ,kn=bl-aZ;
Color bK=L(aS!=null&&aS.Length>0 ? aS[0]: n4 ?? "40, 60, 85");Color cg=aS!=null&&aS.Length>1 ? L(aS[aS.Length-1]): bK;Action<float,float,float,float,Color>aW=(wA,wE,qa,p9,ts)=>q(cf,new Vector2(wA+qa/2f,wE+p9/2f),new Vector2(qa,p9),ts);
if(cZ=="corners"){float aU=Math.Max(10f,Math.Min(fE,kn)*0.15f);aW(aJ,aZ,aU,V,bK);aW(aJ,aZ,V,aU,bK);aW(bR-aU,aZ,aU,V,cg);aW(bR-V,aZ,V,aU,cg);aW(aJ,bl-V,aU,V,bK);aW(aJ,bl-aU,V,aU,bK);
aW(bR-aU,bl-V,aU,V,cg);aW(bR-V,bl-aU,V,aU,cg);return;}if(aS!=null&&aS.Length>1&&qL){fF(cf,aS,"horizontal",aJ,aZ,fE,V);fF(cf,aS,"horizontal",aJ,bl-V,fE,V);
}else{aW(aJ,aZ,fE,V,bK);aW(aJ,bl-V,fE,V,cg);}if("|frame|single|heavy|double|rounded|".Contains("|"+cZ+"|")){aW(aJ,aZ,V,kn,bK);aW(bR-V,aZ,V,kn,cg);}if(cZ=="rounded"){
float ch=V*3f;q(cf,new Vector2(aJ,aZ),new Vector2(ch,ch),bK,0f,"Circle");q(cf,new Vector2(bR,aZ),new Vector2(ch,ch),cg,0f,"Circle");q(cf,new Vector2(aJ,bl),new Vector2(ch,ch),bK,0f,"Circle");
q(cf,new Vector2(bR,bl),new Vector2(ch,ch),cg,0f,"Circle");}if(cZ=="double")fG(cf,"single",aS,n4,qL,aJ+5f,aZ+5f,bR-5f,bl-5f);}void rJ(v n,IMyTextSurface lX,aB g){
g.f.Clear();g.eo.Clear();g.b3.Clear();Vector2 af=lX.SurfaceSize;Vector2 wf=lX.TextureSize;Vector2 bx=(wf-af)/2f;
g.iC=bx;float fi=Math.Max(4f,af.X*0.02f);float dG=Math.Max(4f,af.Y*0.02f);string qi=D.m1(n.f8,af.X,af.Y);
if(qi=="compact"){fi=4f;dG=4f;}else if(qi=="corner"){fi=2f;dG=2f;}if(n.iF>=0f)fi=dG=n.iF;
float rc=af.X-(fi*2f);Color dm=new Color(10,12,16);bool ol=true;if(!string.IsNullOrEmpty(n.bX)){var eX=p.el(n.bX);
if(eX.f1)ol=false;else dm=new Color(eX.b4,eX.b0,eX.bW,eX.h5);}g.cz= -1;g.fI=false;g.fH=L(n.mn ?? "#5b0f0f");
g.d3.Clear();if(n.bZ!=null&&n.bZ.Length>1){g.cz=g.f.Count;g.fK=fF(g,n.bZ,n.d2,bx.X+2f,bx.Y+2f,af.X-4f,af.Y-4f);
}else if(ol||n.a1!=null||n.c9!=null){if(n.a1!=null)dm=L(n.a1.First(vo=>vo.db).b_);g.cz=g.f.Count;
g.fK=new[]{dm};q(g,bx+(af/2f),af-new Vector2(4f,4f),dm);if(n.a1!=null||n.c9!=null)g.d3.Add(new fL{
f0=g.cz,h8=dm,fR=dm,ga=n.a1,ic=n.h9,il=n.ia,fX=n.c9,im=n.io,ip=n.iq,mO=true}
);}if(!string.IsNullOrEmpty(n.ib))q(g,bx+(af/2f),af-new Vector2(4f,4f),L(n.mp ?? "255,255,255,24"),0f,n.ib);
if(!n.c8.Equals("none",StringComparison.OrdinalIgnoreCase)){float nO=bx.X+(af.X-rc)/2f;fG(g,n.c8,n.fQ,n.d5,!n.fZ,nO-2f,bx.Y+dG*0.5f-1f,nO+rc+2f,bx.Y+af.Y-dG*0.5f+1f);
}var lg=n.an;if(lg.Count==0){lg=new List<aG>{new aG{de="main",d7=0,en=0,ep=n.ed,eq=n.ee,av=n.f3}
};}float oZ=n.it>=0f ? n.it : 8f;foreach(var r in lg){var hj=D.rM(r.d7,r.en,r.ep,r.eq,n.ed,n.ee,af.X,af.Y,fi,dG,oZ,oZ);
float ay=bx.X+hj.ne;float bP=bx.Y+hj.nf;float aV=hj.nd;float bO=hj.mL;g.b3.Add(new iH{mT=r.de,iM=g.f.Count,mZ=new Vector2(ay,bP),m5=new Vector2(aV,bO)}
);bool qn=r.a1!=null||r.c9!=null;if(r.bZ!=null&&r.bZ.Length>1){fF(g,r.bZ,r.d2!="vertical" ? r.d2 : n.d2,ay,bP,aV,bO);
}else{Color co;if(r.a1!=null)co=L(r.a1.First(vp=>vp.db).b_);else if(!string.IsNullOrEmpty(r.c6))co=L(r.c6);
else if(!string.IsNullOrEmpty(n.iG))co=L(n.iG);else co=Color.Transparent;if(co.A>0||qn){if(qn)g.d3.Add(new fL{f0=g.f.Count,h8=co,fR=co,ga=r.a1,ic=r.h9,il=r.ia,fX=r.c9,im=r.io,ip=r.iq}
);q(g,new Vector2(ay+aV/2f,bP+bO/2f),new Vector2(aV,bO),co);}}if(r.d4!=null&& !r.d4.Equals("none",StringComparison.OrdinalIgnoreCase))fG(g,r.d4,r.fP ?? n.fQ,r.fO ?? n.d5,!n.fZ,ay+1f,bP+1f,ay+aV-1f,bP+bO-1f);
string gU=r.mD ?? n.mE ?? "Monospace";float th=D.r2(n.f8,af.X,af.Y,n.da);float aI=r.mF ?? th;
float eY=0f;if(!string.IsNullOrEmpty(r.cI)){eY=D.mH(bO,aI);float wh=D.r1(bO,aI);
var jc=TextAlignment.LEFT;float l6=ay+4f;string nl=r.iR ?? n.m9 ?? "left";if(nl.Equals("center",StringComparison.OrdinalIgnoreCase)){
jc=TextAlignment.CENTER;l6=ay+(aV/2f);}else if(nl.Equals("right",StringComparison.OrdinalIgnoreCase)){jc=TextAlignment.RIGHT;l6=ay+aV-4f;
}string q1=r.iS ?? n.na;Color wg= !string.IsNullOrEmpty(q1)? L(q1): L(r.fO ?? n.d5 ?? "white");
var q6=MySprite.CreateText(r.cI,gU,wg,wh,jc);q6.Position=new Vector2(l6,bP+2f);g.f.Add(q6);
string[]js=r.fP ?? n.fQ;Color tK=js!=null&&js.Length>0 ? L(js[0]): L(r.fO ?? n.d5 ?? "40, 60, 85");
q(g,new Vector2(ay+aV/2f,bP+Math.Max(1f,eY-2f)),new Vector2(Math.Max(0f,aV-8f),1f),tK);}if(r.iD=="vertical"||r.av.Any(vd=>vd.i==e.gc)){
rK(g,lX,r,gU,aI,ay,bP,aV,bO,eY);continue;}int wG=Math.Max(1,r.av.Count);float u4=Math.Max(1f,bO-eY-8f);
float rt=0f,nn=0f;foreach(var ve in r.av)rt+=1<<ve.gb;float wj=u4/Math.Max(1f,rt);float e5=6f;for(int kt=0;kt<r.av.Count;
kt++){var j=r.av[kt];var P=new cE{ek=j};float dM=wj*(1<<j.gb);float ql=bP+eY+4f+nn;nn+=dM;float N=ql+(dM/2f);
float q3=N-(11f*aI);P.dc=g.f.Count;P.mK=j.ad!=null&&(j.ad.Contains("{val}")||j.ad.Contains("{percent}"));
float e3=j.f_!=null ? 18f*aI : 0f;var pp=MySprite.CreateText(j.ad,gU,j.eh!=null ? L(j.eh): Color.White,aI,TextAlignment.LEFT);
pp.Position=new Vector2(ay+e5+e3,q3);g.f.Add(pp);if(j.f_!=null)q(g,new Vector2(ay+e5+e3/2f,N),new Vector2(e3-2f,e3-2f),Color.White,0f,j.f_);
float uy=Math.Max(24f*aI,(j.ad.Length+1)*12f*aI);float wr=j.iA ? 0f :(!string.IsNullOrEmpty(j.es)|| !string.IsNullOrEmpty(j.eb))? 64f*aI : 52f*aI;
if(j.i!=e.iP){float M=ay+e5+e3+uy;float tg=ay+aV-e5-wr;float K=Math.Max(16f,tg-M);
float S=Math.Max(j.i==e.cF ? 8f : 4f,Math.Min((j.i==e.cF ? 20f : 16f)*aI,dM*(j.i==e.cF ? 0.55f : 0.45f)));
if(j.gb>0)S=dM*0.7f;Color ra=ao(j,new Color(25,30,38));P.bg=M;P.aE=K;P.bf=S;P.bz=N;
if(j.i==e.d1){q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ra);P.ac=g.f.Count;
q(g,new Vector2(M,N),new Vector2(0f,S),L(j.aA));bool vA=j.fU=="left";for(int l7=1;l7<j.iQ;l7++)q(g,new Vector2(M+K*l7/j.iQ,N),new Vector2(1f,S),new Color(0,0,0,150));
if(j.f4!=null)foreach(double px in j.f4)q(g,new Vector2(M+K*(float)(vA ? 1.0-px/100.0 : px/100.0),N),new Vector2(2f,S+6f),Color.White);
if(j.iE!=null)fG(g,"single",null,j.iE,true,M,N-S/2f,M+K,N+S/2f);}else if(j.i==e.fT){
float dK=Math.Max(8f,Math.Min(K/2f,dM*0.85f));float gK=M+K/2f,gL=ql+dM-4f;int jV=j.bH>0 ? j.bH : 16;P.bg=gK;
P.bz=gL;P.aE=dK;for(int jS=0;jS<jV;jS++){float l4=(jS+0.5f)/jV*3.14159f;P.H.Add(g.f.Count);q(g,new Vector2(gK-(float)Math.Cos(l4)*dK*0.8f,gL-(float)Math.Sin(l4)*dK*0.8f),new Vector2(dK*3.14159f/jV*0.85f,Math.Max(3f,dK*0.16f)),ao(j,new Color(24,28,36)),l4-1.5708f);
}P.H.Add(g.f.Count);q(g,new Vector2(gK,gL),new Vector2(3f,dK*0.9f),Color.White);q(g,new Vector2(gK,gL),new Vector2(8f,8f),Color.White,0f,"Circle");
}else if(j.i==e.ej){int lG=j.bH>0 ? j.bH : 10;float oO=2f;float lH=Math.Max(2f,(K-(oO*(lG-1)))/lG);for(int lA=0;
lA<lG;lA++){P.H.Add(g.f.Count);q(g,new Vector2(M+lA*(lH+oO)+(lH/2f),N),new Vector2(lH,S),ao(j,new Color(22,26,32)));
}}else if(j.i==e.cF){q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ao(j,new Color(20,24,30)));int ht=j.bH>0 ? j.bH : 8;
P.cG=new float[ht];float oP=2f;float jO=Math.Max(2f,(K-(oP*(ht-1)))/ht);for(int lb=0;lb<ht;lb++){P.H.Add(g.f.Count);
q(g,new Vector2(M+lb*(jO+oP)+(jO/2f),N),new Vector2(jO,2f),L(j.aA));}}else if(j.i==e.ef){q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ao(j,new Color(20,24,30)));
int lS=10;float oQ=2f;float lT=Math.Max(2f,(K-(oQ*(lS-1)))/lS);for(int lB=0;lB<lS;lB++){P.H.Add(g.f.Count);
q(g,new Vector2(M+lB*(lT+oQ)+(lT/2f),N),new Vector2(lT,S),ao(j,new Color(24,28,36)),0.523599f);}}else if(j.i==e.d6){
int ji=10;float oR=2f;float jj=Math.Max(2f,(K-(oR*(ji-1)))/ji);for(int lC=0;lC<ji;lC++){P.H.Add(g.f.Count);
q(g,new Vector2(M+lC*(jj+oR)+(jj/2f),N),new Vector2(jj,S),ao(j,new Color(24,28,36)),1.570796f,"Triangle");}}else if(j.i==e.d9){
int jW=j.bH>0 ? j.bH : 12;float oS=2f;float jY=Math.Max(2f,(K-(oS*(jW-1)))/jW);float ok=Math.Min(jY,S);for(int lD=0;
lD<jW;lD++){P.H.Add(g.f.Count);q(g,new Vector2(M+lD*(jY+oS)+(jY/2f),N),new Vector2(ok,ok),ao(j,new Color(24,28,36)),0f,"Circle");
}}else if(j.i==e.c7){q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ra);P.ac=g.f.Count;
q(g,new Vector2(M+(K/2f),N),new Vector2(0f,S),L(j.aA));q(g,new Vector2(M+(K/2f),N),new Vector2(2f,S+4f),new Color(80,110,150));
}else if(j.i==e.cJ){q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ao(j,new Color(20,24,30)));P.ac=g.f.Count;
float kJ=Math.Max(4f,S);q(g,new Vector2(M+kJ*0.5f,N),new Vector2(kJ,kJ),new Color(40,50,65),0f,"Circle");}else if(j.i==e.cC){
float dp=Math.Max(6f,Math.Min(S,14f*aI));q(g,new Vector2(M+dp*0.5f,N),new Vector2(dp,dp),new Color(28,34,46));
P.ac=g.f.Count;q(g,new Vector2(M+dp*0.5f,N),new Vector2(dp*0.6f,dp*0.6f),new Color(40,50,65));}else if(j.i==e.cH){
q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ao(j,new Color(18,22,30)));P.ac=g.f.Count;float ln=Math.Max(4f,S*0.7f);
q(g,new Vector2(M+ln+2f,N),new Vector2(ln,ln),new Color(34,197,94),0f,"Circle");}else if(j.i==e.c5){int lm=8;
float oT=2f;float lo=Math.Max(2f,(K-(oT*(lm-1)))/lm);for(int lc=0;lc<lm;lc++){P.H.Add(g.f.Count);q(g,new Vector2(M+lc*(lo+oT)+(lo/2f),N),new Vector2(lo,S),ao(j,new Color(24,28,36)));
}}else if(j.i==e.cD){q(g,new Vector2(M+(K/2f),N),new Vector2(K,S),ao(j,new Color(20,24,30)));P.ac=g.f.Count;
float o6=Math.Max(4f,S);q(g,new Vector2(M+(K/2f),N),new Vector2(o6,o6),L(j.aA),0f,"Triangle");}}var rh=MySprite.CreateText("---",gU,Color.White,aI,TextAlignment.RIGHT);
rh.Position=new Vector2(ay+aV-e5,q3);P.a7=g.f.Count;g.f.Add(rh);g.eo.Add(P);}}g.ei=af;
g.eg=true;}const string sP="~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-";
const int b6=8;void rK(aB ae,IMyTextSurface v4,aG lf,string oH,float eV,float u0,float le,float u1,float pS,float ub){
int uM=Math.Max(1,lf.av.Count);float n5=(u1-12f)/uM;float h0=22f*eV;float bv=le+ub+4f+h0;float a_=Math.Max(20f,le+pS-4f-h0-bv);
float tD=v4.MeasureStringInPixels(new StringBuilder("M"),oH,eV*0.9f).X;for(int g4=0;g4<lf.av.Count;g4++){var cT=lf.av[g4];float bo=u0+6f+n5*(g4+0.5f);
float c3=n5*0.7f;var bs=new cE{ek=cT,nb=true,bg=bo,aE=c3,bf=a_,bz=bv+a_/2f,mx=tD};bs.dc=ae.f.Count;
be(ae.f,cT.ad,cT.eh!=null ? L(cT.eh): Color.White,bo,le+pS-4f-h0,eV);bs.a7=ae.f.Count;
be(ae.f,"---",Color.White,bo,bv-h0,eV);var pm=ae.f[bs.dc];pm.Alignment=TextAlignment.CENTER;ae.f[bs.dc]=pm;
var rg=ae.f[bs.a7];rg.Alignment=TextAlignment.CENTER;ae.f[bs.a7]=rg;q(ae,new Vector2(bo,bv+a_/2f),new Vector2(c3,a_),ao(cT,new Color(25,30,38)));
bs.ac=ae.f.Count;if(cT.i==e.gc){for(int nz=0;nz<b6;nz++){bs.H.Add(ae.f.Count);q(ae,new Vector2(bo,bv+a_),new Vector2(c3-6f,0f),Color.White);
}for(int mg=0;mg<4;mg++){bs.H.Add(ae.f.Count);var qX=MySprite.CreateText("",oH,mg==0 ? new Color(255,255,255,170): new Color(255,255,255,110),eV*0.9f,TextAlignment.CENTER);
qX.Position=new Vector2(bo,bv+a_);ae.f.Add(qX);}Color j_=new Color(90,110,135);q(ae,new Vector2(bo-c3/2f,bv+a_/2f),new Vector2(3f,a_),j_);q(ae,new Vector2(bo+c3/2f,bv+a_/2f),new Vector2(3f,a_),j_);
q(ae,new Vector2(bo,bv+a_),new Vector2(c3+3f,3f),j_);}else{q(ae,new Vector2(bo,bv+a_),new Vector2(c3,0f),L(cT.aA));}ae.eo.Add(bs);
}}bool sO(aB bm,cE E,double fC,double qc,Color gE,bool rl){var bc=E.ek;float nH=E.F;if(E.F<0f)E.F=(float)fC;
E.F=Math.Abs(E.F-(float)fC)>0.05f ? E.F+((float)fC-E.F)*0.35f :(float)fC;float cU=Math.Max(0f,Math.Min(1f,E.F/100f))*E.bf;
float gB=E.bz+E.bf/2f;float gI=E.bg;if(bc.i!=e.gc){var gP=bm.f[E.ac];gP.Size=new Vector2(E.aE,cU);
gP.Position=new Vector2(gI,gB-cU/2f);gP.Color=gE;bm.f[E.ac]=gP;if(rl||E.F!=nH)aQ(bm,E,bj(qc,bc),gE);
return rl||E.F!=nH;}float ju=E.bf/b6;string fo=bc.bB!=null ? D.df(bc.bB,bc.d8!=null ? a5(bc.d8,bc.ii): fC): null;
Color uG=fo!=null ? L(fo): gE;string[]qQ=fo!=null ?(fo.Contains("/")? fo.Split('/'): null): bc.b1&&bc.aP.Length>1 ? bc.aP : null;
for(int cN=0;cN<b6;cN++){float rq=Math.Max(0f,Math.Min(ju,cU-cN*ju));var hu=bm.f[E.H[cN]];hu.Size=new Vector2(E.aE-6f,rq);
hu.Position=new Vector2(gI,gB-cN*ju-rq/2f);hu.Color=qQ!=null ? ec(qQ,100.0*(cN+0.5)/b6): uG;bm.f[E.H[cN]]=hu;}
int tt=Math.Max(1,(int)((E.aE-8f)/E.mx));bool vP=cU>6f;var mj=bm.f[E.H[b6]];mj.Data=vP ? sP.Substring((int)(C*5.0)%6,Math.Min(tt,100)): "";
mj.Position=new Vector2(gI,gB-cU-12f);bm.f[E.H[b6]]=mj;for(int cm=0;cm<3;cm++){var jD=bm.f[E.H[b6+1+cm]];
double u9=(C*(0.35+0.1*cm)+cm*0.37)%1.0;jD.Data=cU>24f ?(cm==1 ? "O" : "o"): "";jD.Position=new Vector2(gI+(cm-1)*E.aE*0.26f,gB-(float)u9*(cU-6f)-10f);
bm.f[E.H[b6+1+cm]]=jD;}aQ(bm,E,bj(qc,bc),gE);return true;}static void aQ(aB nQ,cE lM,string we,Color tw){
var md=nQ.f[lM.a7];md.Data=lM.ek.iA ? "" : we;md.Color=tw;nQ.f[lM.a7]=md;}static void m4(aB nR,cE qK,Color tx){
var ow=nR.f[qK.ac];ow.Color=tx;nR.f[qK.ac]=ow;}bool sB(v bV,IMyTextSurface lY){aB k;
if(!bI.TryGetValue(bV,out k)){k=new aB();bI[bV]=k;}bool qg= !k.eg||k.ei!=lY.SurfaceSize;
if(qg){rJ(bV,lY,k);}bool T=qg||eA;bool eF=(C%bV.h6)<bV.h6/2.0;double cQ= -1.0,n9;
if(gf.TryGetValue(bV,out n9)){cQ=(C-n9)/bV.ig;if(cQ>=1.0){gf.Remove(bV);cQ=1.0;}}bool mh=cL&&eF&&k.fH.A>0;
if(k.cz>=0&&mh!=k.fI){k.fI=mh;for(int e_=0;e_<k.fK.Length;e_++){var nI=k.f[k.cz+e_];nI.Color=mh ? k.fH : k.fK[e_];
k.f[k.cz+e_]=nI;}T=true;foreach(var ti in k.d3)ti.ir=true;}foreach(var aq in k.d3){if(aq.mO&&k.fI)continue;Color eI=aq.h8;
try{if(aq.ga!=null){double nS=a5(aq.ic,aq.il);string hz=D.df(aq.ga,nS);eI=hz!=null&&hz.Contains("/")? ec(hz.Split('/'),nS): L(hz);
}if(aq.fX!=null&&eF&&D.df(aq.fX,a5(aq.im,aq.ip))!=null)eI=k.fH;}catch{}if(eI!=aq.fR||aq.ir){
aq.fR=eI;aq.ir=false;var nP=k.f[aq.f0];nP.Color=eI;k.f[aq.f0]=nP;T=true;}}for(int ku=0;ku<k.eo.Count;ku++){var b=k.eo[ku];
var h=b.ek;double J;bool o0=false;double kZ=h.f5;try{J=a5(h.aO,h.I);if(h.dd!=null)kZ=a5(h.dd,h.ix);
}catch{o0=true;J=0.0;}if(o0){if(b.f2!= -99999.0||(b.a7>=0&&k.f[b.a7].Data!="ERR")){T=true;b.f2= -99999.0;
if(b.a7>=0){var lE=k.f[b.a7];lE.Data="ERR";lE.Color=Color.Red;k.f[b.a7]=lE;}}continue;}double bd=J;
if(kZ>0)J=Math.Min(100.0,bd/kZ*100.0);bool qU=cQ>=0.0&&h.i!=e.iP&&h.i!=e.cJ&&h.i!=e.cC&&h.i!=e.cH&&h.i!=e.cD;
if(qU)J=cQ<0.5 ? cQ*200.0 : 100.0+(J-100.0)*(cQ-0.5)*2.0;double gH=h.d8!=null ? a5(h.d8,h.ii): J;string gF=h.bB!=null ? D.df(h.bB,gH): h.b1 ? D.fY(h.aP,gH): h.aA;
Color Q=L(gF);bool bp=(Q==Color.Red);if(gF!=null&&gF.Contains("/"))Q=ec(gF.Split('/'),gH);if(h.bB==null&&h.b1&& !bV.fZ&&h.aP.Length>1)Q=ec(h.aP,gH);
if(bp&&eF){Q=new Color(255,75,75);T=true;}if(h.id!=null&&D.df(h.id,a5(h.mq,h.mr))!=null){
bp=T=true;if(eF)Q=new Color(Q.R/3,Q.G/3,Q.B/3);}bool a0=qU||Math.Abs(bd-b.f2)>0.1;if(a0)b.f2=bd;
if(b.nb){if(sO(k,b,J,bd,Q,a0))T=true;continue;}if(h.i==e.d1||h.i==e.fT){double oC=(h.iv||h.I=="percent"||h.I=="pct")? J : iK(h.aO);
if(b.F<0f)b.F=(float)oC;float hS=(float)oC;if(Math.Abs(b.F-hS)>0.05f){b.F+=(hS-b.F)*0.35f;
T=true;}else if(b.F!=hS){b.F=hS;T=true;}if(a0)T=true;if(T){float p1=(float)Math.Max(0.0,Math.Min(1.0,b.F/100.0));
float ka=b.aE*p1;if(h.i==e.fT){int eO=b.H.Count-1;for(int dv=0;dv<eO;dv++){var qE=k.f[b.H[dv]];
qE.Color=b.F>=(dv+0.5)/eO*100.0 ?(h.bB==null&&h.b1&&h.aP.Length>1 ? ec(h.aP,(dv+0.5)/eO*100.0): Q): ao(h,new Color(24,28,36));
k.f[b.H[dv]]=qE;}float jf=p1*3.14159f;var k1=k.f[b.H[eO]];k1.Position=new Vector2(b.bg-(float)Math.Cos(jf)*b.aE*0.45f,b.bz-(float)Math.Sin(jf)*b.aE*0.45f);
k1.RotationOrScale=jf-1.5708f;k.f[b.H[eO]]=k1;}else{var gR=k.f[b.ac];gR.Size=new Vector2(ka,b.bf);
gR.Position=new Vector2(h.fU=="left" ? b.bg+b.aE-(ka/2f): b.bg+(ka/2f),b.bz);gR.Color=Q;
k.f[b.ac]=gR;}aQ(k,b,bj(bd,h),Q);}}else if(h.i==e.ej||h.i==e.ef||h.i==e.d6||h.i==e.d9){
if(a0||bp){T=true;double tT=(h.iv||h.I=="percent"||h.I=="pct")? J : iK(h.aO);int qA=b.H.Count;
for(int fv=0;fv<qA;fv++){bool uH=tT>=((fv+0.4)/(double)qA*100.0);Color vN=uH ? Q : ao(h,new Color(24,28,36));var qB=k.f[b.H[fv]];
qB.Color=vN;k.f[b.H[fv]]=qB;}aQ(k,b,bj(bd,h),Q);}}else if(h.i==e.cF){
if(a0||bp){T=true;for(int gZ=0;gZ<b.cG.Length-1;gZ++)b.cG[gZ]=b.cG[gZ+1];b.cG[b.cG.Length-1]=(float)J;
int vg=b.H.Count;for(int fe=0;fe<vg;fe++){float ua=b.cG[fe];float t9=Math.Max(0.06f,Math.Min(1f,ua/100f));float n3=b.bf*t9;
float ty=b.bz+(b.bf/2f)-(n3/2f);var du=k.f[b.H[fe]];du.Size=new Vector2(du.Size.Value.X,n3);
du.Position=new Vector2(du.Position.Value.X,ty);du.Color=Q;k.f[b.H[fe]]=du;}aQ(k,b,bj(bd,h),Q);
}}else if(h.i==e.c7){if(b.F<0f)b.F=(float)J;float hT=(float)J;if(Math.Abs(b.F-hT)>0.05f){
b.F+=(hT-b.F)*0.35f;T=true;}else if(b.F!=hT){b.F=hT;T=true;}if(T||a0){
float o_=b.aE*0.5f;float nZ=b.bg+o_;float of=(float)Math.Max(-1.0,Math.Min(1.0,(b.F-50.0)/50.0));float kb=Math.Abs(of)*o_;
float tU=(of>=0)?(nZ+kb*0.5f):(nZ-kb*0.5f);var gS=k.f[b.ac];gS.Size=new Vector2(kb,b.bf);
gS.Position=new Vector2(tU,b.bz);gS.Color=Q;k.f[b.ac]=gS;aQ(k,b,bj(bd,h),Q);
}}else if(h.i==e.cJ){if(a0||bp){T=true;bool kC=J>=50.0;float ph=Math.Max(4f,b.bf);float un=kC ?(b.bg+b.aE-ph*0.5f):(b.bg+ph*0.5f);
Color pg=kC ?(Q!=Color.White ? Q : new Color(34,197,94)): new Color(60,70,85);var j8=k.f[b.ac];j8.Position=new Vector2(un,b.bz);
j8.Color=pg;k.f[b.ac]=j8;aQ(k,b,kC ? "ON" : "OFF",pg);}}else if(h.i==e.cC){if(a0||bp){
T=true;bool kB=J>=50.0;Color n_=kB ?(Q!=Color.White ? Q : new Color(56,189,248)): new Color(30,36,48);m4(k,b,n_);
aQ(k,b,kB ? "ACTIVE" : "READY",kB ? n_ : new Color(148,163,184));}}else if(h.i==e.cH){if(a0||bp){
T=true;string v_=J>=75.0 ? "ONLINE" :(J>=25.0 ? "STBY" : "OFFLINE");Color qP=J>=75.0 ? new Color(34,197,94):(J>=25.0 ? new Color(234,179,8): new Color(239,68,68));
m4(k,b,qP);aQ(k,b,v_,qP);}}else if(h.i==e.c5){if(a0||bp){T=true;int p2=b.H.Count;
for(int ff=0;ff<p2;ff++){bool uI=J>=((ff+0.5)/(double)p2*100.0);Color vb=uI ? Q : ao(h,new Color(24,28,36));var p3=k.f[b.H[ff]];
p3.Color=vb;k.f[b.H[ff]]=p3;}aQ(k,b,bj(bd,h),Q);}}else if(h.i==e.cD){
T=true;float hA;Color eU;string gT;bool ug=(h.I=="producing"||h.I=="active"||h.I=="running"||h.I=="state"||(h.I!="percent"&&(J<=1.0||J==100.0)));
if(ug){if(J>0.0){b.F=(b.F+0.3927f)%6.28318f;hA=b.F;eU=new Color(34,197,94);gT="RUN";}else{hA=0f;
eU=new Color(148,163,184);gT="IDLE";}}else{hA=(J>55.0)? 0f :((J<45.0)? 3.14159f : 1.57079f);eU=(J>55.0)? new Color(34,197,94):((J<45.0)? new Color(239,68,68): new Color(148,163,184));
gT=(J>55.0)? "CHG" :((J<45.0)? "DSC" : "STBY");}var j9=k.f[b.ac];j9.RotationOrScale=hA;j9.Color=eU;
k.f[b.ac]=j9;aQ(k,b,gT,eU);}else{string gO=null;bA hV;ev.TryGetValue(h.aO,out hV);if(h.I!="age"&&h.I!="fresh")a9.TryGetValue(mP(h.I)? h.aO+"."+h.I :(hV!=null&&hV.bC!=null ? hV.bC : h.aO),out gO);
if(a0||bp||gO!=b.mQ){b.mQ=gO;T=true;Color ma=Q;if(ma==Color.Red&&eF)ma=new Color(255,75,75);aQ(k,b,gO ?? bj(bd,h),ma);
}}if(b.mK&&(a0||bp)){T=true;var pr=k.f[b.dc];string u7=mI(J);string vz=bj(bd,h);
pr.Data=h.ad.Replace("{percent}",u7).Replace("{val}",vz);k.f[b.dc]=pr;}}if(T){k.et++;using(var t_=lY.DrawFrame()){
t_.AddRange(k.f);}}return T;}bool sA(v cs,IMyTextSurface lZ){lZ.FontSize=cs.da;var gG=new List<KeyValuePair<string,string>>();
foreach(var o in cs.f3){double U=0.0;string bu;string bn=null;double k_=o.f5;try{U=a5(o.aO,o.I);if(o.dd!=null)k_=a5(o.dd,o.ix);
}catch(Exception wF){gG.Add(new KeyValuePair<string,string>($"{o.ad}: [UNRESOLVED]","red"));continue;}double qd=U;if(k_>0)U=Math.Min(100.0,qd/k_*100.0);
string ri=bj(qd,o);if(o.i==e.d1){double oD=(o.iv||o.I=="percent"||o.I=="pct")? U : iK(o.aO);
bu=D.st(o.ad,oD,10,ri);bn=o.b1 ? D.fY(o.aP,oD): o.aA;}else if(o.i==e.c7){
bu=D.sr(o.ad,U,10);bn=o.b1 ? D.fY(o.aP,U): o.aA;}else if(o.i==e.cJ){
bu=D.sD(o.ad,U);bn=o.aA ??(U>=50.0 ? "green" : "gray");}else if(o.i==e.cC){bu=D.sw(o.ad,U);
bn=o.aA ??(U>=50.0 ? "cyan" : "gray");}else if(o.i==e.cH){bu=D.sC(o.ad,U);bn=o.aA ??(U>=75.0 ? "lime" :(U>=25.0 ? "yellow" : "red"));
}else if(o.i==e.c5){bu=D.sq(o.ad,U,8);bn=o.b1 ? D.fY(o.aP,U):(o.aA ?? "yellow");
}else if(o.i==e.cD){bool uh=(o.I=="producing"||o.I=="active"||o.I=="running"||o.I=="state"||(o.I!="percent"&&(U<=1.0||U==100.0)));
if(uh){char[]vY=new[]{'|','/','-','\\'};char vJ=vY[Math.Abs(eC)%4];bu=U>0 ? $"{o.ad} [{vJ}] RUN" : $"{o.ad} [==] IDLE";
bn=U>0 ? "green" : "gray";}else{bu=D.sx(o.ad,U,eC);bn=o.aA ??(U>55.0 ? "green" :(U<45.0 ? "red" : "cyan"));
}}else{bu=$"{o.ad}: {ri}";bn=o.bB!=null ? D.df(o.bB,U): null;}gG.Add(new KeyValuePair<string,string>(bu,bn));
}var gA=D.sv(new List<string>(gG.ConvertAll(v9=>v9.Key)),dY: cs.nc);string qH=string.Join("\n",gA);
string p5;if(b8.TryGetValue(cs,out p5)&&p5==qH)return false;b8[cs]=qH;using(var t0=lZ.DrawFrame()){
float rz=4f;float pt=rA*cs.da;try{var pw=lZ.MeasureStringInPixels(new StringBuilder("A"),"Monospace",cs.da);
if(pw.Y>0)pt=pw.Y;}catch{}for(int dA=0;dA<gA.Count;dA++){string n7=null;if(dA>0&&dA<gA.Count-1)n7=gG[dA-1].Value;
var qO=MySprite.CreateText(gA[dA],"Monospace",L(n7),cs.da,TextAlignment.LEFT);qO.Position=new Vector2(4f,rz);t0.Add(qO);
rz+=pt;}}return true;}Color L(string hg){if(string.IsNullOrEmpty(hg))return Color.White;Color e8;if(ng.TryGetValue(hg,out e8))return e8;
p eL=p.el(hg);e8=eL.f1 ? Color.Transparent : new Color(eL.b4,eL.b0,eL.bW,eL.h5);ng[hg]=e8;return e8;}public enum bh{rE,iu,ea}
public enum aN{sb,h7,m7,mS,mR}public class bA{public string ai;public bh b2;public List<string>a6=new List<string>();public string m8{
get{return a6.Count>0 ? a6[0]: "";}set{if(a6.Count==0)a6.Add(value);else a6[0]=value;}}public aN dZ=aN.sb;
public double fS=0.0;public string ij;public string bC;public double my=10;public bool fJ;}public enum e{iP,d1,ej,cF,ef,d6,d9,c7,cJ,cC,cH,c5,cD,gc,fT}
public class az{public string fV;public string b_;public bool db;}public class bD{public e i;public string ad;public string aO;
public string I;public bool b1;public string[]aP;public string aA;public List<az>bB;public string fU="right";
public string es;public string eb;public List<double>f4;public string f_;public List<az>id;public string mq,mr;public bool iA;
public int bH;public int iQ;public string iE;public int gb;public string d8,ii;public string eh,iT;public double f5;public string dd,ix;
public bool iv{get{return f5>0||dd!=null;}}}public struct p{public byte b4,b0,bW,h5;public bool f1;public p(byte vq,byte t2,byte s9,byte sR=255){
b4=vq;b0=t2;bW=s9;h5=sR;f1=false;}public static readonly p sN=new p(0,0,0,0){f1=true};public static readonly p gd=new p(255,255,255);
public static readonly p rD=new p(0,0,0);public static readonly p so=new p(255,0,0);public static readonly p r4=new p(0,255,0);
public static readonly p r8=new p(50,205,50);public static readonly p sQ=new p(255,255,0);public static readonly p sc=new p(255,140,0);
public static readonly p rP=new p(0,255,255);public static readonly p rF=new p(0,120,255);public static readonly p r9=new p(255,0,255);
public static readonly p sm=new p(128,0,128);public static readonly p r3=new p(128,128,128);public static readonly p rQ=new p(60,60,60);
public static p el(string bt){if(string.IsNullOrWhiteSpace(bt))return gd;bt=bt.Trim();if(bt.Equals("transparent",StringComparison.OrdinalIgnoreCase)||bt.Equals("none",StringComparison.OrdinalIgnoreCase))return sN;
if(bt.StartsWith("#")){string ck=bt.Substring(1).Trim();if(ck.Length==6){byte vr=Convert.ToByte(ck.Substring(0,2),16);byte t3=Convert.ToByte(ck.Substring(2,2),16);
byte ta=Convert.ToByte(ck.Substring(4,2),16);return new p(vr,t3,ta);}if(ck.Length==3){byte vs=Convert.ToByte(new string(ck[0],2),16);byte t4=Convert.ToByte(new string(ck[1],2),16);
byte tb=Convert.ToByte(new string(ck[2],2),16);return new p(vs,t4,tb);}}if(bt.Contains(",")){var dH=bt.Split(',');if(dH.Length>=3){byte vt=(byte)Math.Max(0,Math.Min(255,int.Parse(dH[0].Trim())));
byte t5=(byte)Math.Max(0,Math.Min(255,int.Parse(dH[1].Trim())));byte tc=(byte)Math.Max(0,Math.Min(255,int.Parse(dH[2].Trim())));byte sS=dH.Length>3 ?(byte)Math.Max(0,Math.Min(255,int.Parse(dH[3].Trim()))):(byte)255;
return new p(vt,t5,tc,sS);}}switch(bt.ToLowerInvariant()){case "red": return so;case "green": return r4;case "lime": return r8;case "yellow": return sQ;
case "orange": return sc;case "cyan": return rP;case "blue": return rF;case "magenta": return r9;case "purple": return sm;case "white": return gd;
case "black": return rD;case "gray": case "grey": return r3;case "darkgray": case "darkgrey": return rQ;default: return gd;}}}public class bY{
public string[]bZ;public string d2="vertical";public List<az>a1;public string h9,ia;public List<az>c9;public string io,iq;
}public class aG : bY{public string de="default";public int d7=0;public int en=0;public int ep=1;public int eq=1;public string cI;public string iR="left";
public string iS;public string d4;public string mD;public float? mF;public string c6;public string fO;public string[]fP;
public string iD="horizontal";public List<bD>av=new List<bD>();}public class v : bY{public string bk;public string f8="standard";
public string c8="graphic";public string d5;public string[]fQ;public string m9="left";public string na;public string mE="Monospace";
public string bX;public string mu;public int mv=10;public int ms=1;public float ig=1.2f;public float it= -1f,iF= -1f;
public bool fZ;public string iG;public string ib;public string mp;public string mn;public string iy;public string f6;
public string cy;public string cB;public float ih=2.5f;public string mt="cyan";public string[]fM;public List<string>fN=new List<string>();
public List<string>r0(){if(fN!=null&&fN.Count>0)return fN;return new List<string>{"helm-core initializing...","seeking compliant surface displays...","uploading firmware...","awaiting compliance...","helm-core online -- BOOT // SUCCESS"}
;}public float da=1.0f;public int em=0;public double dg;public List<v>bG;public v m2(double vM){
return bG==null ? this : bG[(int)((Math.Max(0,vM)/dg)%bG.Count)];}public float h6=1.6f;public int ed=1;
public int ee=1;public int nc=30;public string a4;public int bE=0;public List<aG>an=new List<aG>();public List<bD>f3{
get{if(an.Count==0)an.Add(new aG{de="main"});return an[0].av;}}}public class b7{public string iz;public List<string>f7=new List<string>();
public Dictionary<string,string>ik=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);public List<string>ie=new List<string>();}public class iI{
public List<bA>cA=new List<bA>();public List<v>W=new List<v>();public List<string>a2=new List<string>();public List<string>eu=new List<string>();
public Dictionary<string,List<string>>d0=new Dictionary<string,List<string>>(StringComparer.OrdinalIgnoreCase);}public static class mM{static readonly Dictionary<string,b7>mw=new Dictionary<string,b7>(StringComparer.OrdinalIgnoreCase);
static mM(){sp();}static void sp(){b5("fighter_hud",new[]{"$screen=\"Cockpit Display\"","$surf=0","$pwr=\"Main Batteries\"","$h2=\"Hydrogen Tanks\"","$cargo=\"Cargo Containers\""}
,@"@bind pwr_$screen=group(""$pwr"",agg:avg)
@bind h2_$screen=group(""$h2"",agg:avg)
@bind crg_$screen=group(""$cargo"",agg:sum)
@screen $screen
output:""$screen"",$surf
layout:grid(2,1)
@panel sys at(0,0,1,1) title:""PROPULSION""
line:""PWR"" bar(pwr_$screen.percent)
line:""H2"" bar(h2_$screen.percent)
@panel hold at(1,0,1,1) title:""STORES""
line:""CARGO"" led(crg_$screen.percent)");b5("reactor_matrix",new[]{"$screen=\"Reactor LCD\"","$surf=0","$reactors=\"Reactors\"","$batteries=\"Batteries\""}
,@"@bind rct_$screen=group(""$reactors"",agg:avg)
@bind bat_$screen=group(""$batteries"",agg:avg)
@screen $screen
output:""$screen"",$surf
layout:grid(1,2)
@panel core at(0,0,1,1) title:""REACTORS""
line:""CORE"" bar(rct_$screen.percent)
@panel grid at(0,1,1,1) title:""BATTERIES""
line:""BANK"" led(bat_$screen.percent)");b5("bridge_status",new[]{"$screen=\"Bridge LCD\"","$surf=0","$power=\"Batteries\"","$o2=\"Air Vents\"","$cargo=\"Cargo\"","$jump=\"Jump Drives\""}
,@"@bind pwr_$screen=group(""$power"",agg:avg)
@bind o2_$screen=group(""$o2"",agg:avg)
@bind crg_$screen=group(""$cargo"",agg:sum)
@bind jmp_$screen=group(""$jump"",agg:avg)
@screen $screen
output:""$screen"",$surf
layout:grid(2,2)
@panel pwr at(0,0,1,1) title:""POWER GRID""
line:""PWR"" bar(pwr_$screen.percent)
@panel life at(0,1,1,1) title:""LIFE SUPPORT""
line:""O2"" bar(o2_$screen.percent)
@panel nav at(1,0,1,1) title:""NAVIGATION""
line:""JUMP"" bar(jmp_$screen.percent)
@panel store at(1,1,1,1) title:""LOGISTICS""
line:""CARGO"" led(crg_$screen.percent)");b5("mining_ops",new[]{"$screen=\"Drill LCD\"","$surf=0","$cargo=\"Cargo Containers\"","$drills=\"Drills\""}
,@"@bind crg_$screen=group(""$cargo"",agg:sum)
@bind drl_$screen=group(""$drills"",agg:avg)
@screen $screen
output:""$screen"",$surf
layout:grid(1,2)
@panel ore at(0,0,1,1) title:""ORE STORAGE""
line:""CARGO"" bar(crg_$screen.percent)
@panel drill at(0,1,1,1) title:""DRILL STATUS""
line:""DRILL"" led(drl_$screen.percent)");b5("airlock",new[]{"$screen=\"Airlock LCD\"","$surf=0","$vent=\"Airlock Vent\"","$outer=\"Outer Door\"","$inner=\"Inner Door\""}
,@"@bind vent_$screen=airvent(""$vent"")
@bind outer_$screen=block(""$outer"")
@bind inner_$screen=block(""$inner"")
@screen $screen
output:""$screen"",$surf
layout:grid(1,2)
@panel env at(0,0,1,1) title:""ATMOSPHERE""
line:""PRESS"" bar(vent_$screen.percent)
@panel doors at(0,1,1,1) title:""SECURITY AIRLOCK""
line:""OUTER"" led(outer_$screen.percent)
line:""INNER"" led(inner_$screen.percent)");b5("rover",new[]{"$screen=\"Rover Cockpit\"","$surf=0","$power=\"Rover Batteries\"","$susp=\"Suspension Wheels\"","$cargo=\"Rover Cargo\"","$h2=\"Hydrogen Engine\""}
,@"@bind bat_$screen=group(""$power"",agg:avg)
@bind susp_$screen=group(""$susp"",agg:avg)
@bind crg_$screen=group(""$cargo"",agg:sum)
@screen $screen
output:""$screen"",$surf
layout:grid(2,2)
@panel pwr at(0,0,1,1) title:""POWER TRAIN""
line:""BATT"" bar(bat_$screen.percent)
@panel susp at(0,1,1,1) title:""CHASSIS""
line:""LOAD"" bar(susp_$screen.percent)
@panel cargo at(1,0,1,2) title:""CARGO""
line:""HOLD"" bar(crg_$screen.percent)");b5("cargo_overview",new[]{"screen","surf=0","crg=Cargo*","ore=Ore*","ingot=Ingot*"},@"@bind crg_$screen=group(""$crg"",agg:avg)
@bind ore_$screen=group(""$ore"",agg:avg)
@bind ing_$screen=group(""$ingot"",agg:avg)
@screen $screen
output:""$screen"",$surf
layout:grid(2,2)
@panel ores at(0,0,1,1) title:""RAW ORE SILOS""
line:""ORES"" bar(ore_$screen.percent)
@panel ingots at(1,0,1,1) title:""REFINED INGOTS""
line:""INGT"" bar(ing_$screen.percent)
@panel comps at(0,1,1,1) title:""COMPONENT STORES""
line:""COMP"" bar(crg_$screen.percent)
@panel ammo at(1,1,1,1) title:""ORDNANCE & AMMO""
line:""NATO"" bar(crg_$screen.percent)");b5("battery_monolith",new[]{"screen","surf=0","batt=Battery*"},@"@bind pwr_$screen=group(""$batt"",agg:avg)
@screen $screen
output:""$screen"",$surf
layout:grid(1,1)
@panel core at(0,0,1,1) title:""POWER CORE MONOLITH"" orientation:vertical
line:""ENERGY"" bar(pwr_$screen.percent) dir:up");}public static void b5(string pF,string[]u5,string tm){var hQ=new b7{iz=pF}
;foreach(var uV in u5){var kL=uV.Split(new[]{'='},2);string pT=iB(kL[0]);hQ.f7.Add(pT);if(kL.Length>1)hQ.ik[pT]=ag(kL[1].Trim());
}var uF=tm.Replace("\r\n","\n").Split('\n');foreach(var ut in uF){string qp=ut.Trim();if(qp.Length>0)hQ.ie.Add(qp);}mw[pF]=hQ;}static string iB(string dF){
dF=dF.Trim();if(dF.StartsWith("$"))dF=dF.Substring(1);return dF.ToLowerInvariant();}public static iI se(string tB){var al=new iI();v d=null;
aG u=null;var p4=sk(tB,al.a2,al.d0);for(int e0=0;e0<p4.Count;e0++){string a=p4[e0].Trim();
if(a.Length==0)continue;try{if(a.StartsWith("@bind")){al.cA.Add(sf(a));}else if(a.StartsWith("@screen")){d=new v{bk=a.Substring("@screen".Length).Trim()}
;al.W.Add(d);u=null;}else if(a.StartsWith("profile:")){s(d,"profile:");string qf=a.Substring("profile:".Length).Trim();
string pI=D.mU(qf);if(pI==null)throw new FormatException("unknown profile '"+qf+"' (expected auto/standard/compact/wide/corner or a hardware profile name)");
d.f8=pI;}else if(a.StartsWith("border:")){s(d,"border:");string nA=a.Substring("border:".Length).Trim();if(u!=null)u.d4=nA;
else d.c8=nA;}else if(a.StartsWith("border_color:")){s(d,"border_color:");string nC=a.Substring("border_color:".Length).Trim();
if(u!=null)u.fO=nC;else d.d5=nC;}else if(a.StartsWith("border_gradient:")||a.StartsWith("border_gradient(")){
s(d,"border_gradient");string cO=a.StartsWith("border_gradient:")? a.Substring("border_gradient:".Length): a.Substring("border_gradient".Length);
cO=cO.Trim();if(cO.StartsWith("(")&&cO.Contains(")"))cO=a3(cO,0);string[]qR=aR(cO).Select(vD=>vD.Trim()).ToArray();if(u!=null)u.fP=qR;
else d.fQ=qR;}else if(a.StartsWith("title_align:")){s(d,"title_align:");string qY=a.Substring("title_align:".Length).Trim().ToLowerInvariant();
if(u!=null)u.iR=qY;else d.m9=qY;}else if(a.StartsWith("title_color:")){s(d,"title_color:");string q0=a.Substring("title_color:".Length).Trim();
if(u!=null)u.iS=q0;else d.na=q0;}else if(a.StartsWith("font_size:")){s(d,"font_size:");float qV=float.Parse(a.Substring("font_size:".Length).Trim(),System.Globalization.CultureInfo.InvariantCulture);
if(u!=null)u.mF=qV;else d.da=qV;}else if(a.StartsWith("font:")){s(d,"font:");string ox=a.Substring("font:".Length).Trim();
if(u!=null)u.mD=ox;else d.mE=ox;}else if(a.StartsWith("bg:")||a.StartsWith("background:")){s(d,"bg:");string dl=a.Substring(a.IndexOf(':')+1).Trim();
bY jt=u!=null ?(bY)u : d;if(dl.StartsWith("color(")&&dl.EndsWith(")")){string nL;jt.a1=mX(dl.Substring(6,dl.Length-7),out nL);
string nJ,nK;iL(nL,out nJ,out nK);jt.h9=nJ;jt.ia=nK;}else if(u!=null)u.c6=dl;
else d.bX=dl;}else if(a.StartsWith("layout:")){s(d,"layout:");sg(a.Substring("layout:".Length).Trim(),d);}else if(a.StartsWith("alert_period:")){
s(d,"alert_period:");d.h6=Math.Max(0.2f,float.Parse(a.Substring("alert_period:".Length).Trim().TrimEnd('s'),System.Globalization.CultureInfo.InvariantCulture));
}else if(a.StartsWith("refresh:")){s(d,"refresh:");d.em=int.Parse(a.Substring("refresh:".Length).Trim());}else if(a.StartsWith("rotate:")){
s(d,"rotate:");double hF=double.Parse(a.Substring(7).Trim().TrimEnd('s'),System.Globalization.CultureInfo.InvariantCulture);if(double.IsNaN(hF)||double.IsInfinity(hF)||hF<1)throw new FormatException("rotate: requires at least 1 second");
d.dg=hF;}else if(a.StartsWith("output:")){s(d,"output:");var hn=aR(a.Substring("output:".Length).Trim());
if(hn.Count==0)throw new FormatException("output: requires a target block name");d.a4=ag(hn[0]);d.bE=hn.Count>1 ? int.Parse(hn[1]): 0;
}else if(a.StartsWith("width:")){s(d,"width:");d.nc=int.Parse(a.Substring("width:".Length).Trim());}else if(a.StartsWith("boot_msg:")||a.StartsWith("boot:")){
s(d,"boot_msg:");d.mu=ag(a.Substring(a.IndexOf(':')+1).Trim());}else if(a.StartsWith("boot_stagger:")){s(d,"boot_stagger:");
d.mv=int.Parse(a.Substring("boot_stagger:".Length).Trim());}else if(a.StartsWith("boot_calibrate:")){s(d,"boot_calibrate:");
string oa=a.Substring("boot_calibrate:".Length).Trim().ToLowerInvariant().TrimEnd('s');d.ig=oa=="off" ? 0f : Math.Max(0f,float.Parse(oa,System.Globalization.CultureInfo.InvariantCulture));
}else if(a.StartsWith("boot_batch:")){s(d,"boot_batch:");d.ms=Math.Max(1,int.Parse(a.Substring("boot_batch:".Length).Trim()));
}else if(a.StartsWith("boot_duration:")){s(d,"boot_duration:");string eN=a.Substring("boot_duration:".Length).Trim().ToLowerInvariant();
bool o5=eN.EndsWith("s");if(o5)eN=eN.Substring(0,eN.Length-1).Trim();float jZ;if(float.TryParse(eN,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out jZ)){
d.ih=o5 ? jZ : jZ/60f;}}else if(a.StartsWith("boot_log:")){s(d,"boot_log:");d.fN.Add(ag(a.Substring("boot_log:".Length).Trim()));
}else if(a.StartsWith("mirror_panel:")){s(d,"mirror_panel:");d.iy=ag(a.Substring("mirror_panel:".Length).Trim());}else if(a.StartsWith("mirror:")){
s(d,"mirror:");d.f6=ag(a.Substring("mirror:".Length).Trim());}else if(a.StartsWith("banner:")){s(d,"banner:");
d.cy=ag(a.Substring("banner:".Length).Trim());}else if(a.StartsWith("boot_logo:")){s(d,"boot_logo:");d.cB=ag(a.Substring("boot_logo:".Length).Trim());
}else if(a.StartsWith("boot_gradient:")||a.StartsWith("boot_gradient(")||a.StartsWith("banner_gradient:")||a.StartsWith("banner_gradient(")){s(d,"boot_gradient");
d.fM=mY(a);}else if(a.StartsWith("bg_gradient:")||a.StartsWith("bg_gradient(")){s(d,"bg_gradient");(u!=null ?(bY)u : d).bZ=mY(a);
}else if(a.StartsWith("bg_gradient_dir:")){s(d,"bg_gradient_dir:");(u!=null ?(bY)u : d).d2=a.Substring("bg_gradient_dir:".Length).Trim().ToLowerInvariant();
}else if(a.StartsWith("flash:")){s(d,"flash:");bY j6=u!=null ?(bY)u : d;string j5=a.Substring("flash:".Length).Trim();
string oz=iO(j5);string oy,oA;iL(j5.Substring(0,j5.Length-oz.Length),out oy,out oA);j6.io=oy;j6.iq=oA;
j6.c9=new List<az>{new az{fV=oz,b_="on"}};}else if(a.StartsWith("bg_texture:")){s(d,"bg_texture:");d.ib=ag(a.Substring("bg_texture:".Length).Trim());
}else if(a.StartsWith("bg_texture_color:")){s(d,"bg_texture_color:");d.mp=a.Substring("bg_texture_color:".Length).Trim();
}else if(a.StartsWith("gap:")||a.StartsWith("padding:")){bool o9=a.StartsWith("gap:");s(d,o9 ? "gap:" : "padding:");float p8=float.Parse(a.Substring(a.IndexOf(':')+1).Trim(),System.Globalization.CultureInfo.InvariantCulture);
if(o9)d.it=p8;else d.iF=p8;}else if(a.StartsWith("gradient_mode:")){s(d,"gradient_mode:");d.fZ=a.Substring("gradient_mode:".Length).Trim().Equals("steps",StringComparison.OrdinalIgnoreCase);
}else if(a.StartsWith("screen_bg:")){s(d,"screen_bg:");d.bX=a.Substring("screen_bg:".Length).Trim();}else if(a.StartsWith("panel_bg:")){
s(d,"panel_bg:");d.iG=a.Substring("panel_bg:".Length).Trim();}else if(a.StartsWith("alert_bg:")){s(d,"alert_bg:");
d.mn=a.Substring("alert_bg:".Length).Trim();}else if(a.StartsWith("boot_color:")||a.StartsWith("banner_color:")){s(d,"boot_color:");
d.mt=a.Substring(a.IndexOf(':')+1).Trim();}else if(a.StartsWith("orientation:")){s(d,"orientation:");string uS=a.Substring("orientation:".Length).Trim().ToLowerInvariant();
if(u!=null)u.iD=uS;}else if(a.StartsWith("@panel")){s(d,"@panel");u=si(a);d.an.Add(u);
}else if(a.StartsWith("title:")){s(d,"title:");if(u==null){u=new aG{de="main",d7=0,en=0,ep=d.ed,eq=d.ee}
;d.an.Add(u);}u.cI=ag(a.Substring("title:".Length).Trim());}else if(a.StartsWith("line:")){s(d,"line:");
if(u==null){u=new aG{de="main",d7=0,en=0,ep=d.ed,eq=d.ee};d.an.Add(u);}u.av.Add(sh(a.Substring("line:".Length).Trim()));
}else{al.a2.Add($"Line {e0 + 1}: unrecognized directive: \"{a}\"");}}catch(Exception tN){al.a2.Add($"Line {e0 + 1}: parse error ({tN.Message}) in \"{a}\"");
}}foreach(var qq in al.W){if(string.IsNullOrEmpty(qq.a4))al.a2.Add($"@screen {qq.bk}: missing required 'output:' — a screen must declare which physical block/surface it renders to");
}foreach(var _ in al.W){if(_.dg>0&&_.bG==null){var hk=al.W.FindAll(qr=>qr.bE==_.bE&&string.Equals(qr.a4,_.a4,StringComparison.OrdinalIgnoreCase));
foreach(var uW in hk)uW.bG=hk;if(hk.Any(uX=>uX.dg!=_.dg)){al.a2.Add("output '"+_.a4+"': every rotating page must use the same rotate: interval");
foreach(var pQ in hk){pQ.bG=null;pQ.dg=0;}}}int k9=_.an.Count(lp=>lp.c6!=null||lp.a1!=null||lp.bZ!=null);if(_.an.Count>1&&k9>0&&k9<_.an.Count)al.eu.Add("screen '"+_.bk+"': only "+k9+" of "+_.an.Count+" panels have their own bg:, so the screen shows two colours. bg: after an @panel colours that panel only; use screen_bg: (or bg: before the first @panel) for the whole screen");
if(_.cy!=null&& !al.d0.ContainsKey(_.cy))al.a2.Add("Screen '"+_.bk+"': banner '"+_.cy+"' is not defined (@banner "+_.cy+" ... @end)");
if(_.cB!=null&& !al.d0.ContainsKey(_.cB))al.a2.Add("Screen '"+_.bk+"': boot_logo '"+_.cB+"' is not defined (@banner "+_.cB+" ... @end)");
}return al;}static void sg(string jA,v qy){int lh=jA.IndexOf('(');if(lh<0)throw new FormatException("missing '(' in layout: specification");
string pf=jA.Substring(0,lh).Trim().ToLowerInvariant();string sZ=a3(jA,lh);var jh=aR(sZ);if(pf=="grid"){if(jh.Count<2)throw new FormatException("layout: grid(cols, rows) requires 2 arguments");
qy.ed=Math.Max(1,int.Parse(jh[0]));qy.ee=Math.Max(1,int.Parse(jh[1]));}else{throw new FormatException($"unknown layout kind '{pf}' (expected grid)");
}}static aG si(string uB){string fq=uB.Substring("@panel".Length).Trim();int gt=fq.IndexOf("at(",StringComparison.OrdinalIgnoreCase);if(gt<0)throw new FormatException("@panel requires an at(...) positioning clause");
string uN=fq.Substring(0,gt).Trim();string nw=fq.Substring(gt);int p_=nw.IndexOf('(');string nv=a3(nw,p_);
var cM=aR(nv);if(cM.Count<2)throw new FormatException("at(col, row) requires at least 2 numbers");var bQ=new aG{de=uN,d7=int.Parse(cM[0]),en=int.Parse(cM[1]),ep=cM.Count>2 ? int.Parse(cM[2]): 1,eq=cM.Count>3 ? int.Parse(cM[3]): 1}
;int no=gt+p_+nv.Length+1;if(no<fq.Length){string aX=fq.Substring(no).Trim();int q5=aX.IndexOf("title",StringComparison.OrdinalIgnoreCase);
if(q5>=0){string fx=aX.Substring(q5+5).Trim();if(fx.StartsWith("(")&&fx.Contains(")"))bQ.cI=ag(a3(fx,0));else if(fx.StartsWith(":")){
string cP=fx.Substring(1).Trim();if(cP.StartsWith("\"")){int ot=cP.IndexOf('"',1);bQ.cI=ot>0 ? cP.Substring(1,ot-1): ag(cP);}else{int qM=cP.IndexOf(' ');
bQ.cI=qM>=0 ? cP.Substring(0,qM): cP;}}}string pJ=fW(aX,"orientation:");if(pJ!=null)bQ.iD=pJ.ToLowerInvariant();string nm=fW(aX,"title_align:");
if(nm!=null)bQ.iR=nm.ToLowerInvariant();string q2=fW(aX,"title_color:");if(q2!=null)bQ.iS=q2;int gz=aX.IndexOf("border:",StringComparison.OrdinalIgnoreCase);
if(gz>=0&& !aX.Substring(gz).StartsWith("border_gradient:",StringComparison.OrdinalIgnoreCase)&& !aX.Substring(gz).StartsWith("border_color:",StringComparison.OrdinalIgnoreCase)){
bQ.d4=fW(aX.Substring(gz),"border:");}int gw=aX.IndexOf("border_gradient:",StringComparison.OrdinalIgnoreCase);if(gw<0)gw=aX.IndexOf("border_gradient(",StringComparison.OrdinalIgnoreCase);
if(gw>=0){string ci=aX.Substring(gw+"border_gradient".Length).Trim();if(ci.StartsWith(":"))ci=ci.Substring(1).Trim();if(ci.StartsWith("(")&&ci.Contains(")"))ci=a3(ci,0);
bQ.fP=aR(ci).Select(vE=>vE.Trim()).ToArray();}}return bQ;}static string fW(string qs,string pe){int o4=qs.IndexOf(pe,StringComparison.OrdinalIgnoreCase);
if(o4<0)return null;string me=qs.Substring(o4+pe.Length).Trim();int qN=me.IndexOf(' ');return qN>=0 ? me.Substring(0,qN): me;}static string[]mY(string kR){
int kh=kR.IndexOfAny(new[]{':','('});string eW=kR.Substring(kR[kh]==':' ? kh+1 : kh).Trim();if(eW.StartsWith("(")&&eW.Contains(")"))eW=a3(eW,0);return aR(eW).Select(vF=>vF.Trim()).ToArray();
}static List<string>sk(string tC,List<string>j3,Dictionary<string,List<string>>te){var rd=new Dictionary<string,b7>(StringComparer.OrdinalIgnoreCase);
var dL=(tC ?? "").Replace("\r\n","\n").Split('\n');var k8=new List<string>();int dB=0;while(dB<dL.Length){string lu=dL[dB++];if(lu.TrimStart().StartsWith("@banner ",StringComparison.OrdinalIgnoreCase)){
var aw=new List<string>();while(dB<dL.Length){string nk=dL[dB++].Replace("\t","    ").TrimEnd();if(nk.Trim().Equals("@end",StringComparison.OrdinalIgnoreCase))break;
aw.Add(nk);}while(aw.Count>0&&aw[0].Length==0)aw.RemoveAt(0);while(aw.Count>0&&aw[aw.Count-1].Length==0)aw.RemoveAt(aw.Count-1);int kA=int.MaxValue;
foreach(var i8 in aw)if(i8.Length>0)kA=Math.Min(kA,i8.Length-i8.TrimStart().Length);for(int e9=0;e9<aw.Count;e9++)if(aw[e9].Length>0)aw[e9]=aw[e9].Substring(kA);
te[lu.TrimStart().Substring(8).Trim()]=aw;continue;}string cn=m6(lu).Trim();if(cn.Length==0)continue;if(cn.StartsWith("@template")){try{
var l2=sj(cn);while(dB<dL.Length){string to=dL[dB++];string jB=m6(to).Trim();if(jB.Equals("@end",StringComparison.OrdinalIgnoreCase))break;
if(jB.Length>0)l2.ie.Add(jB);}rd[l2.iz]=l2;}catch(Exception tO){j3.Add($"Template parse error ({tO.Message}) in \"{cn}\"");}}
else if(cn.StartsWith("@use")){try{mA(cn,rd,k8,j3);}catch(Exception tP){j3.Add($"@use error ({tP.Message}) in \"{cn}\"");
}}else{k8.Add(cn);}}return k8;}static b7 sj(string uC){string lv=uC.Substring("@template".Length).Trim();int li=lv.IndexOf('(');
if(li<0)throw new FormatException("missing '(' in @template header");string uO=lv.Substring(0,li).Trim();string s_=a3(lv,li);var l3=new b7{
iz=uO};foreach(var uY in aR(s_)){var kM=uY.Split(new[]{'='},2);string pU=iB(kM[0]);l3.f7.Add(pU);if(kM.Length>1)l3.ik[pU]=ag(kM[1].Trim());
}return l3;}static void mA(string uD,Dictionary<string,b7>re,List<string>pO,List<string>tM,int oe=0){string hy=uD.Substring("@use".Length).Trim();
int hl=hy.IndexOf('(');string k0=hl>=0 ? hy.Substring(0,hl).Trim(): hy.Trim();string s0=hl>=0 ? a3(hy,hl): "";b7 dV=null;
if(!re.TryGetValue(k0,out dV)){if(!mw.TryGetValue(k0,out dV))throw new FormatException($"unknown template '{k0}'");}var gC=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
foreach(var pj in dV.ik)gC[pj.Key]=pj.Value;var nu=aR(s0);for(int eE=0;eE<nu.Count;eE++){string gr=nu[eE];int j1=gr.IndexOf('=');if(j1>=0){
string ul=iB(gr.Substring(0,j1));string wl=ag(gr.Substring(j1+1).Trim());gC[ul]=wl;}else{if(eE<dV.f7.Count){string um=dV.f7[eE];
gC[um]=ag(gr.Trim());}}}foreach(var tn in dV.ie){string eR=tn;foreach(var hc in gC){eR=eR.Replace("$"+hc.Key,hc.Value).Replace("{"+hc.Key+"}",hc.Value);
}if(eR.StartsWith("@use")){if(oe>3)throw new FormatException("@use nested too deep");mA(eR,re,pO,tM,oe+1);
}else pO.Add(eR);}}static void s(v tA,string tJ){if(tA==null)throw new FormatException($"'{tJ}' used before any @screen declared");
}static string m6(string br){if(string.IsNullOrEmpty(br))return br;bool kz=false;for(int cl=0;cl<br.Length;cl++){char jG=br[cl];if(jG=='"'){kz= !kz;
}else if(!kz){if(jG=='/'&&cl+1<br.Length&&br[cl+1]=='/'){return br.Substring(0,cl);}if(jG=='#'){if(r6(br,cl)){continue;}return br.Substring(0,cl);}}}return br;
}static bool r6(string cV,int o7){int dD=0;for(int kv=o7+1;kv<cV.Length;kv++){char dq=cV[kv];if((dq>='0'&&dq<='9')||(dq>='a'&&dq<='f')||(dq>='A'&&dq<='F'))dD++;
else break;}if(dD==3||dD==4||dD==6||dD==8){int fd=o7+1+dD;if(fd>=cV.Length||char.IsWhiteSpace(cV[fd])||cV[fd]==')'||cV[fd]==','||cV[fd]==':')return true;
}return false;}static bA sf(string uE){string lw=uE.Substring("@bind".Length).Trim();int j2=lw.IndexOf('=');if(j2<0)throw new FormatException("missing '=' in @bind");
string pG=lw.Substring(0,j2).Trim();string j4=lw.Substring(j2+1).Trim();int lj=j4.IndexOf('(');if(lj<0)throw new FormatException("missing '(' in @bind target");
string gW=j4.Substring(0,lj).Trim().ToLowerInvariant();string s1=a3(j4,lj);var cd=aR(s1);if(cd.Count==0)throw new FormatException("@bind target has no arguments");
var ar=new bA{ai=pG};if(gW=="dynamic"||gW=="var"||gW=="mem"){ar.b2=bh.ea;ar.bC=cd.Count>0 ? ag(cd[0].Trim()): pG;
for(int i9=0;i9<cd.Count;i9++){var cS=cd[i9].Split(new[]{':'},2);if(cS.Length==2&&cS[0].Trim().Equals("default",StringComparison.OrdinalIgnoreCase)){double od;
if(double.TryParse(cS[1].Trim(),System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out od))ar.fS=od;else ar.ij=ag(cS[1].Trim());
}else if(cS.Length==2&&cS[0].Trim().Equals("ttl",StringComparison.OrdinalIgnoreCase)){double hY=double.Parse(cS[1].Trim(),System.Globalization.CultureInfo.InvariantCulture);
if(hY<=0||double.IsNaN(hY)||double.IsInfinity(hY))throw new FormatException("ttl: requires positive seconds");ar.my=hY;}}return ar;}ar.b2=gW=="group" ? bh.iu : bh.rE;
string gq=null;for(int gp=0;gp<cd.Count;gp++){var dC=cd[gp].Split(new[]{':'},2);if(dC.Length==2&&dC[0].Trim().Equals("agg",StringComparison.OrdinalIgnoreCase)){
gq=dC[1].Trim().ToLowerInvariant();}else if(dC.Length==2&&dC[0].Trim().Equals("scope",StringComparison.OrdinalIgnoreCase)){ar.fJ=dC[1].Trim().Equals("all",StringComparison.OrdinalIgnoreCase);
}else{ar.a6.Add(ag(cd[gp].Trim()));}}if(ar.a6.Count==0)throw new FormatException("@bind target requires at least one block or group name");
if(ar.b2==bh.iu){if(gq==null)throw new FormatException("group bind requires an explicit 'agg:' (avg/sum/min/max) — no default is assumed (PDR §6)");
switch(gq){case "avg": ar.dZ=aN.h7;break;case "sum": ar.dZ=aN.m7;break;case "min": ar.dZ=aN.mS;break;case "max": ar.dZ=aN.mR;
break;default: throw new FormatException($"unknown agg '{gq}' (expected avg/sum/min/max)");}}return ar;}static bD sh(string gy){var l=new bD();
if(!gy.StartsWith("\""))throw new FormatException("line must start with a quoted label");int j0=gy.IndexOf('"',1);if(j0<0)throw new FormatException("unterminated quoted label");
l.ad=gy.Substring(1,j0-1);string fp=gy.Substring(j0+1).Trim();int hm=fp.IndexOf('(');if(hm<0)throw new FormatException("missing display function, e.g. bar(...)/text(...)");
string z=fp.Substring(0,hm).Trim().ToLowerInvariant();string sY=a3(fp,hm);string ak=fp.Substring(fp.IndexOf(')',hm)+1).Trim();
var jX=sY.Trim().Split('.');if(jX.Length!=2)throw new FormatException("expected 'bindname.field' inside display function, e.g. power.percent");
l.aO=jX[0].Trim();l.I=jX[1].Trim();if(z=="bar")l.i=e.d1;else if(z=="led")l.i=e.ej;else if(z=="spark"||z=="sparkline")l.i=e.cF;
else if(z=="hazard"||z=="stripes")l.i=e.ef;else if(z=="chevron"||z=="chevrons")l.i=e.d6;else if(z=="dots"||z=="dot")l.i=e.d9;
else if(z=="balance"||z=="horizon"||z=="centered")l.i=e.c7;else if(z=="toggle"||z=="switch")l.i=e.cJ;else if(z=="checkbox"||z=="check")l.i=e.cC;
else if(z=="status"||z=="badge"||z=="state")l.i=e.cH;else if(z=="ammo"||z=="pips")l.i=e.c5;else if(z=="flow"||z=="rate"||z=="spinner")l.i=e.cD;
else if(z=="text")l.i=e.iP;else if(z=="tank"||z=="tube"||z=="bucket")l.i=e.gc;else if(z=="dial"||z=="gauge"||z=="arc")l.i=e.fT;
else throw new FormatException($"unknown display function '{z}'");while(ak.Length>0){int jU=ak.IndexOf("dir:",StringComparison.OrdinalIgnoreCase);
int dy=ak.IndexOf('(');if(jU>=0&&(dy<0||jU<dy)){string gJ=ak.Substring(jU+4).Trim();int hH=gJ.IndexOf(' ');l.fU=(hH>=0 ? gJ.Substring(0,hH): gJ).ToLowerInvariant();
ak=(hH>=0 ? gJ.Substring(hH): "").Trim();continue;}int mc=ak.IndexOf("unit:",StringComparison.OrdinalIgnoreCase);if(mc>=0&&(dy<0||mc<dy)){
string h1=ak.Substring(mc+5).Trim();int hI=h1.IndexOf(' ');l.es=(hI>=0 ? h1.Substring(0,hI): h1).Trim().Trim('"');ak=(hI>=0 ? h1.Substring(hI): "").Trim();
continue;}int kc=ak.IndexOf("format:",StringComparison.OrdinalIgnoreCase);if(kc>=0&&(dy<0||kc<dy)){string gQ=ak.Substring(kc+7).Trim();
int hJ=gQ.IndexOf(' ');l.eb=(hJ>=0 ? gQ.Substring(0,hJ): gQ).Trim().Trim('"');ak=(hJ>=0 ? gQ.Substring(hJ): "").Trim();continue;}int hf=ak.IndexOf('(');
if(hf<0)break;string O=ak.Substring(0,hf).Trim().ToLowerInvariant();int pq=O.LastIndexOf(' ');if(pq>=0)O=O.Substring(pq+1).Trim();
string aa=a3(ak,hf);ak=ak.Substring(ak.IndexOf(')',hf)+1).Trim();if(O=="gradient"){l.b1=true;l.aP=aR(aa).Select(vG=>vG.Trim()).ToArray();
if(l.aP.Length<2)throw new FormatException("gradient() needs at least 2 colors");}else if(O=="size"){string l1=aa.Trim().ToLowerInvariant();
l.gb=l1=="large"||l1=="fill" ? 2 : l1=="medium" ? 1 : 0;}else if(O=="marker"){l.f4=new List<double>();foreach(var uK in aR(aa)){double pA;
if(!double.TryParse(uK,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out pA))throw new FormatException("marker() takes numbers from 0 to 100");
l.f4.Add(pA);}}else if(O=="icon")l.f_=aa.Trim();else if(O=="blink"){string jE=aa.Trim(),nD=iO(jE);iL(jE.Substring(0,jE.Length-nD.Length),out l.mq,out l.mr);
l.id=new List<az>{new az{fV=nD,b_="on"}};}else if(O=="value")l.iA=aa.Trim().Equals("off",StringComparison.OrdinalIgnoreCase);
else if(O=="ticks")l.iQ=int.Parse(aa.Trim());else if(O=="segments"||O=="points")l.bH=Math.Max(2,Math.Min(40,int.Parse(aa.Trim())));else if(O=="outline")l.iE=aa.Trim();
else if(O=="color_by"){var jL=aa.Trim().Split('.');if(jL.Length!=2)throw new FormatException("color_by() takes bindname.field");l.d8=jL[0].Trim();
l.ii=jL[1].Trim();}else if(O=="label_color")l.eh=aa.Trim();else if(O=="track")l.iT=aa.Trim();else if(O=="flat"){l.aA=aa.Trim();
}else if(O=="pattern"){string Z=aa.Trim().ToLowerInvariant();if(Z=="hazard"||Z=="stripes")l.i=e.ef;else if(Z=="chevron"||Z=="chevrons")l.i=e.d6;
else if(Z=="dots"||Z=="dot")l.i=e.d9;else if(Z=="led")l.i=e.ej;else if(Z=="balance"||Z=="horizon")l.i=e.c7;
else if(Z=="toggle"||Z=="switch")l.i=e.cJ;else if(Z=="checkbox"||Z=="check")l.i=e.cC;else if(Z=="status"||Z=="badge"||Z=="state")l.i=e.cH;
else if(Z=="ammo"||Z=="pips")l.i=e.c5;else if(Z=="flow"||Z=="rate"||Z=="spinner")l.i=e.cD;}else if(O=="color"){string uf;
l.bB=mX(aa,out uf);}else if(O=="dir"||O=="direction"){l.fU=aa.Trim().ToLowerInvariant();}else if(O=="unit"){
l.es=aa.Trim().Trim('"');}else if(O=="max"){double pz;if(double.TryParse(aa.Trim(),System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out pz))l.f5=pz;
else{var kX=aa.Trim().Split('.');if(kX.Length!=2)throw new FormatException("max() takes a number or bindname.field");l.dd=kX[0].Trim();l.ix=kX[1].Trim();
}}else if(O=="format"||O=="fmt"){l.eb=aa.Trim().Trim('"');}else{throw new FormatException($"unknown modifier '{O}'");}}return l;}static string a3(string hC,int k6){
int jT=0;for(int e1=k6;e1<hC.Length;e1++){if(hC[e1]=='(')jT++;else if(hC[e1]==')'){jT--;if(jT==0)return hC.Substring(k6+1,e1-k6-1);
}}throw new FormatException("unbalanced parentheses");}static List<string>aR(string s2){return s2.Split(',').Select(vH=>vH.Trim()).Where(vI=>vI.Length>0).ToList();
}static List<az>mX(string sX,out string j7){var hB=new List<az>();j7=null;foreach(var lk in aR(sX)){int jP=lk.LastIndexOf(':');
if(jP<0)throw new FormatException("color() entries need 'expr : color'");string hd=lk.Substring(0,jP).Trim();string n6=lk.Substring(jP+1).Trim();
if(hd.Equals("default",StringComparison.OrdinalIgnoreCase)){hB.Add(new az{db=true,b_=n6});}else{string qS=iO(hd);
if(j7==null)j7=hd.Substring(0,hd.Length-qS.Length).Trim();hB.Add(new az{fV=qS,b_=n6});}}if(!hB.Any(vu=>vu.db))throw new FormatException("color() requires a 'default:' fallback entry — PDR-consistent decision: no silent fallback color assumed");
return hB;}static void iL(string vv,out string tk,out string tS){var fg=(vv ?? "").Split('.');if(fg.Length!=2||fg[0].Trim().Length==0||fg[1].Trim().Length==0)throw new FormatException("expected bindname.field in front of the comparison, e.g. batt.percent < 20");
tk=fg[0].Trim();tS=fg[1].Trim();}static string iO(string dw){dw=dw.Trim();foreach(var uT in new[]{">=","<=","==",">","<"}){int kw=dw.IndexOf(uT,StringComparison.Ordinal);
if(kw>0)return dw.Substring(kw).Trim();if(kw==0)return dw;}throw new FormatException($"color() rule has no recognizable comparison operator: '{dw}'");}
static string ag(string cW){if(cW.Length>=2&&cW[0]=='"'&&cW[cW.Length-1]=='"')return cW.Substring(1,cW.Length-2);return cW;}}public static class rC{public static double rB(aN sT,List<double>dX){
if(dX==null||dX.Count==0)throw new InvalidOperationException("cannot aggregate an empty value set");switch(sT){case aN.h7: return dX.Average();case aN.m7: return dX.Sum();
case aN.mS: return dX.Min();case aN.mR: return dX.Max();default: throw new InvalidOperationException("Aggregate() called with Agg.None");}}}public static class D{
public static string st(string uv,double ho,int nB=10,string rm=null){ho=Math.Max(0,Math.Min(100,ho));int oE=(int)Math.Round(nB*(ho/100.0));
string tf=new string('#',oE)+new string('-',nB-oE);string wm=rm!=null ? rm : $"{ho:0}%";return $"{uv} [{tf}] {wm}";
}public static string mG(double cx,string mb=null,string gV=null){if(mb=="auto"){double ja=Math.Abs(cx);if(ja>=1e9)return(cx/1e9).ToString("F1")+"G";
if(ja>=1e6)return(cx/1e6).ToString("F1")+"M";if(ja>=1e3)return(cx/1e3).ToString("F1")+"k";string tQ= !string.IsNullOrEmpty(gV)? gV : "F0";return cx.ToString(tQ);
}string oG= !string.IsNullOrEmpty(gV)? gV :(Math.Abs(cx)>=100.0 ? "F0" : "F1");if(string.IsNullOrEmpty(mb)){return cx.ToString(oG);}return cx.ToString(oG)+mb;
}public static string mU(string vi){string uZ=(vi ?? "").Trim().ToLowerInvariant().Replace("_","").Replace("-","").Replace(" ","");switch(uZ){
case "auto": return "auto";case "standard": case "textpanel": return "standard";case "compact": case "cockpitsmall": return "compact";case "wide": case "widelcd": return "wide";
case "corner": case "cornerlcd": case "cockpitcorner": return "corner";default: return null;}}public static string m1(string vj,float l_,float qT){
string pR=mU(vj)?? "standard";if(pR!="auto")return pR;if(l_<=0f||qT<=0f)return "standard";float qG=Math.Min(l_,qT);
if(qG<=160f)return "corner";if(qG<=300f)return "compact";if(l_>=1000f)return "wide";return "standard";}public static float r2(string vk,float v8,float v6,float gM=1.0f){
string ld=m1(vk,v8,v6);if(ld=="compact")return gM*0.75f;if(ld=="corner")return gM*0.55f;if(ld=="wide")return gM*1.15f;
return gM;}public static float mH(float pY,float oI){if(pY<=0f||oI<=0f)return 0f;return Math.Min(20f*oI,pY*0.18f);
}public static float r1(float u3,float oJ){float uc=mH(u3,oJ);return Math.Min(oJ*0.9f,uc*0.045f);
}public static string sr(string uw,double hp,int cj=5){hp=Math.Max(0,Math.Min(100,hp));double gN=(hp-50.0)/50.0;int fa=0;
int fs=0;if(gN< -0.01)fa=(int)Math.Round(-gN*cj);else if(gN>0.01)fs=(int)Math.Round(gN*cj);if(fa>cj)fa=cj;
if(fs>cj)fs=cj;string uz=new string('-',cj-fa)+new string('<',fa);string vB=new string('>',fs)+new string('-',cj-fs);
return $"{uw} [{uz}|{vB}] {hp:0}%";}public static string sD(string pn,double wo){return wo>=50.0 ? $"{pn} [====O] ON" : $"{pn} [O====] OFF";
}public static string sw(string po,double wp){return wp>=50.0 ? $"{po} [X] ACTIVE" : $"{po} [ ] READY ";}public static string sC(string kO,double rk){
if(rk>=75.0)return $"{kO} [ ONLINE  ]";if(rk>=25.0)return $"{kO} [ STANDBY ]";return $"{kO} [ OFFLINE ]";}public static string sq(string ux,double hq,int pv=8){
hq=Math.Max(0,Math.Min(100,hq));int oF=(int)Math.Round(pv*(hq/100.0));string vc=new string('|',oF)+new string('.',pv-oF);
return $"{ux} [{vc}] {hq:0}%";}public static string sx(string kP,double fk,int q4=0){char[]vX=new[]{'|','/','-','\\'};char qt=vX[(q4>=0 ? q4 : 0)%4];
if(fk>55.0)return $"{kP} {qt} ▲ CHG {fk:0}%";if(fk<45.0)return $"{kP} {qt} ▼ DSC {fk:0}%";return $"{kP} ● STBY {fk:0}%";}public static bool sa(string aC,string e6){
if(aC==null||e6==null)return false;if(aC=="*"||aC==e6)return true;if(!aC.Contains("*")&& !aC.Contains("?"))return string.Equals(aC,e6,StringComparison.OrdinalIgnoreCase);
int aK=0,e2=0,lQ= -1,kW=0;while(e2<e6.Length){if(aK<aC.Length&&(aC[aK]=='?'||char.ToLowerInvariant(aC[aK])==char.ToLowerInvariant(e6[e2]))){
aK++;e2++;}else if(aK<aC.Length&&aC[aK]=='*'){lQ=aK;kW=e2;aK++;}else if(lQ!= -1){aK=lQ+1;kW++;e2=kW;}else{return false;}}while(aK<aC.Length&&aC[aK]=='*')aK++;
return aK==aC.Length;}public static int rG(double u_,int q9){return Math.Max(0,Math.Min(q9,1+(int)(Math.Max(0.0,u_)*q9)));}public static string fY(string[]dU,double ll){
if(dU==null||dU.Length==0)return "white";ll=Math.Max(0,Math.Min(100,ll));int kx=(int)Math.Floor(ll/100.0*dU.Length);if(kx>=dU.Length)kx=dU.Length-1;
return dU[kx];}public static p mJ(string[]cv,double u8){if(cv==null||cv.Length==0)return p.gd;if(cv.Length==1)return p.el(cv[0]);
double vy=Math.Max(0.0,Math.Min(1.0,u8/100.0));double qx=vy*(cv.Length-1);int ky=(int)Math.Floor(qx);int ue=Math.Min(cv.Length-1,ky+1);
double lU=qx-ky;p dr=p.el(cv[ky]);p jH=p.el(cv[ue]);byte vw=(byte)Math.Round(dr.b4+(jH.b4-dr.b4)*lU);
byte t6=(byte)Math.Round(dr.b0+(jH.b0-dr.b0)*lU);byte td=(byte)Math.Round(dr.bW+(jH.bW-dr.bW)*lU);return new p(vw,t6,td);}public static string df(List<az>qo,double wq){
foreach(var lx in qo){if(lx.db)continue;if(rY(lx.fV,wq))return lx.b_;}var oc=qo.FirstOrDefault(vx=>vx.db);return oc!=null ? oc.b_ : null;
}static bool rY(string eS,double fD){eS=eS.Trim();foreach(var k5 in new[]{">=","<=","==",">","<"}){if(eS.StartsWith(k5)){double fr=double.Parse(eS.Substring(k5.Length).Trim(),System.Globalization.CultureInfo.InvariantCulture);
switch(k5){case ">=": return fD>=fr;case "<=": return fD<=fr;case "==": return Math.Abs(fD-fr)<1e-9;case ">": return fD>fr;case "<": return fD<fr;
}}}throw new FormatException($"unsupported comparison expression: '{eS}'");}public static List<string>sv(List<string>tz,int dY){
var hi=new List<string>();hi.Add("+"+new string('-',dY+2)+"+");foreach(var kS in tz){string wi=kS.Length>dY ? kS.Substring(0,dY): kS;
string u2=wi.PadRight(dY);hi.Add("| "+u2+" |");}hi.Add("+"+new string('-',dY+2)+"+");return hi;}public struct mW{public float ne,nf,nd,mL;
}public static mW rM(int n2,int qk,int hK,int hL,int fz,int fA,float v7,float v5,float pV,float pW,float kj,float kk){
fz=Math.Max(1,fz);fA=Math.Max(1,fA);hK=Math.Max(1,Math.Min(fz-n2,hK));hL=Math.Max(1,Math.Min(fA-qk,hL));
float s6=v7-(pV*2f);float s5=v5-(pW*2f);float nY=(s6-(kj*(fz-1)))/fz;float nX=(s5-(kk*(fA-1)))/fA;
float vm=pV+n2*(nY+kj);float vn=pW+qk*(nX+kk);float vl=hK*nY+((hK-1)*kj);float va=hL*nX+((hL-1)*kk);return new mW{
ne=vm,nf=vn,nd=vl,mL=va};}public struct m3{public int sd;public int iw;public int aj;public int aF;public int er;
public int bi;public int sL;public int sM;public int rV;public int rW;
public double rX;}public static m3 rN(v ct){var m=new m3();var pZ=ct.an!=null&&ct.an.Count>0 ? ct.an : new List<aG>{
new aG{av=ct.f3}};m.sd=pZ.Count;m.iw=0;m.aj=0;m.aF=0;if(!string.IsNullOrEmpty(ct.bX)&& !ct.bX.Equals("none",StringComparison.OrdinalIgnoreCase))m.aj++;
if(!string.IsNullOrEmpty(ct.c8)&& !ct.c8.Equals("none",StringComparison.OrdinalIgnoreCase))m.aj+=2;foreach(var fh in pZ){
if(!string.IsNullOrEmpty(fh.c6)&& !fh.c6.Equals("none",StringComparison.OrdinalIgnoreCase))m.aj++;if(!string.IsNullOrEmpty(fh.cI))m.aj+=2;
m.iw+=fh.av.Count;foreach(var uu in fh.av){m.sL++;m.sM++;m.aj+=2;switch(uu.i){case e.d1: m.er++;
m.bi++;m.aj+=2;m.aF++;break;case e.ej: m.bi+=10;m.aj+=10;
m.aF+=10;break;case e.ef: m.bi+=10;m.aj+=11;m.aF+=10;break;case e.d6: m.bi+=10;
m.aj+=10;m.aF+=10;break;case e.d9: m.bi+=12;m.aj+=12;m.aF+=12;
break;case e.cF: m.er++;m.bi+=8;m.aj+=9;m.aF+=8;break;case e.c7: m.er+=2;
m.bi++;m.aj+=3;m.aF++;break;case e.cJ: case e.cC: m.er++;
m.bi++;m.aj+=2;m.aF++;break;case e.cH: case e.cD: m.er++;m.bi+=2;
m.aj+=3;m.aF+=2;break;case e.c5: m.bi+=8;m.aj+=8;m.aF+=8;
break;default: m.aF++;break;}}}m.rV=25+(m.iw*2);m.rW=180+(m.aF*4);
m.rX=0.0;return m;}}