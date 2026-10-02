
int APSIndex=0;
long tmFrequency = 10;
void DoMain(){tmFrequency=Tm(10);
	if(IsTick(tmAPSFrequency)){tmAPSFrequency=Tm(APSFrequency);
	for(int i=0; i<10; i++)if(APSIndex<Connectors.Count){
		
		var Connector = Connectors[APSIndex++];
		if(Connector.Status != MyShipConnectorStatus.Unconnected) continue;
		Vector3D? up=null;
		if(Control!=null)
				switch(GetDirection(Control.Orientation, Connector, Base6Directions.Direction.Forward)){
				case (Base6Directions.Direction.Up):
				case (Base6Directions.Direction.Down):up=Control.WorldMatrix.Forward;break;
				default:up=Control.WorldMatrix.Up;break;}
		var gap = (Connector.CubeGrid.GridSizeEnum == MyCubeSize.Large)?1.25:0.5;
		APS_Send(new MovedObj(Connector, gap, up).ToStr());
		
	}else {APSIndex=0;break;}}
}


// Settings
string GroupName = "";
int Version=1;
double PBLimit = 0.25;
long APSFrequency = 120;
string APSSendTag = "APS.Sender";
string APSReceiveTag = "APS.Receive";
string NoAPSMarker = "[NoAPS]";
string LCDMarker = "[APS]";
int UseLCDCockpit =2;

void WriteSettings(){
	Me.CustomData=
"// === SCRIPT SETTIGNS ==="+
"\nVersion = 8"+
"\nGroupName = "+GroupName+
"\nRuntime.Limit = "+Str(PBLimit+0.05)+
"\nAPS.Frequency = " + (APSFrequency/60.0).ToString("f2") +
"\nAPS.Send = " + APSSendTag +
"\nAPS.Receive = " + APSReceiveTag +
"\nMarker.NoUseAPS = " + NoAPSMarker +
"\nMarker.LCD = "+LCDMarker+
"\nLCD.Cockpit = "+UseLCDCockpit +
"";
SettingsHashCode=Me.CustomData.GetHashCode();Version = 8;
}
long SettingsHashCode=0;

bool ReadSetting(string Cmd, bool Change = false){
	var Data=Cmd.Split('=');
	if(Data.Length>=1){
		string sValue = (Data.Length>=2)? Data[1].Trim() : "", sValueLow = sValue.ToLower();
		switch(Data[0].Trim().ToLower()){
		case("groupname"):GroupName=sValue;return true;
		case("version"):if(ReadValue(sValueLow))Version=(int)rValue;return true;
		case("runtime.limit"):if(ReadValue(sValue))PBLimit=Math.Max(rValue-0.05,0.1);return true;
		case("aps.frequency"):if(ReadValue(sValueLow))APSFrequency=(long)(rValue*60);return true;
		case("aps.send"):APSSendTag=sValue;return true;
		case("aps.receive"):APSReceiveTag=sValue;return true;
		case("marker.nouseaps"):NoAPSMarker=sValue;return true;
		case("marker.lcd"):LCDMarker=sValue;return true;		
		case("lcd.cockpit"):if(ReadValue(sValueLow))UseLCDCockpit=(int)rValue;return true;
	}}
	return false;
}

// Cockpit
static IMyShipController Control = null;
List<IMyShipController> Controls=new List<IMyShipController>();
void SelectControl(){
Control = null;var Works = Control;
foreach (var c in Controls)if(c.IsFunctional){Works=c;if (c.IsMainCockpit){Control=c;break;}if (c.IsUnderControl)Control=c;}
Control=Control??Works;}

// Recognizer
int Recognizer = 0;
long tmRecognize = 60;
IMyBlockGroup Group=null;
void OnRecognize(long uf = 36000, bool First=false){tmRecognize=Tm(20);

var recLCD=new List<IMyTextPanel>();
switch(Recognizer){
case(0):{
	if(SettingsHashCode!=Me.CustomData.GetHashCode())if(Me.CustomData.Trim()=="")WriteSettings();else{
			SettingsHashCode=Me.CustomData.GetHashCode();
			foreach(var Setting in Me.CustomData.Split('\n')){var Line=Setting.Trim();
				var l=Line.IndexOf("//");if(l>=0)Line=Line.Substring(0,l);
				if(Line.ToLower()=="settings.end!")break;ReadSetting(Line);}
	}Recognizer++;SearchMyLCD();}break;

case(1):{if(GroupName!=""){
	Group=GridTerminalSystem.GetBlockGroupWithName(GroupName);
	if(Group!=null)Recognizer=20;
	else { Error="Group no found!\n\""+StrL(GroupName,12)+"\"";tmRecognize=Tm(600); }
}else{ Group=null;Recognizer=2;}}break;

//Recognizer without Group
case(2):{
	GridTerminalSystem.GetBlocksOfType(recLCD, b=>(b.CubeGrid==Me.CubeGrid && b is IMyTextSurface && b.CustomName.Contains(LCDMarker)));
	Recognizer++;
}break;

case(3):{
	GridTerminalSystem.GetBlocksOfType(Controls, b=>(b.CubeGrid==Me.CubeGrid && IsCockpit(b.DefinitionDisplayNameText)));
	SelectControl();
	if(Control==null){Error="No Cockpit Found!\n";Recognizer=0;tmRecognize=Tm(600);}else Recognizer++;
}break;

case(4):{
	GridTerminalSystem.GetBlocksOfType(Connectors, b=>(b.CubeGrid==Me.CubeGrid && !b.CustomName.Contains(NoAPSMarker)));
	Recognizer=100;
}break;


//Recognizer from Group
case(20):{
	Group.GetBlocksOfType(recLCD, b=>(b is IMyTextSurface && b.CustomName.Contains(LCDMarker)));
	Recognizer++;
}break;

case(21):{
	Group.GetBlocksOfType(Controls, b=>(IsCockpit(b.DefinitionDisplayNameText)));
	SelectControl();
	if(Control==null){Error="No Cockpit In the Group!\n\""+StrL(GroupName,25)+"\"";Recognizer=0;tmRecognize=Tm(600);}
	else Recognizer++;
}break;

case(22):{
	GridTerminalSystem.GetBlocksOfType(Connectors, b=>(!b.CustomName.Contains(NoAPSMarker)));
	Recognizer=100;
}break;

default:{Recognizer=0;tmRecognize=Tm((CheckError())?uf:600);}break;
}

if(UseLCDCockpit>=0)foreach(var c in Controls)if(c is IMyTextSurfaceProvider){
	var p=c as IMyTextSurfaceProvider;if(UseLCDCockpit<p.SurfaceCount&&Once(c)){LCDDraw.Add(p.GetSurface(UseLCDCockpit));InvalideLCD=true;}
}


if(Error!=null){ReportBlocks=""; }
else if(First){
	tmRecognize=Tm(3600);UpdateInfo();
	if(MyLCD!=null){PrepareText(MyLCD, Color.Green, TextAlignment.CENTER, 1.5f);MyLCD.WriteText(ReportBlocks);}
}}


List<IMyShipConnector> Connectors = new List<IMyShipConnector>();

bool CheckError(){
if(Connectors.Count==0)Error="No Connectors!";
else if(APSSendTag=="")Error="No Sender Tag!";
else if(APSReceiveTag=="")Error="No Receiver Tag!";
else Error=null;
return Error == null;}


// commands
void DoCmd(string cmd){var cmdLow = cmd.ToLower();
if(cmd.StartsWith("*")){APS_Send(cmd.Substring(1));}
if(ReadSetting(cmd,true)){WriteSettings();return;}
}




// Main
int MyLCDIndex=0;
Program(){SetScriptSpeed(UpdateFrequency.Update10,10);}
string Error = "Initialize...";
void Main(string args, UpdateType ut){
	PBCurTime += Runtime.LastRunTimeMs;
	if ((ut & RuntimeFlag) == 0){
		foreach(var cmd in args.Trim().Split(';')) DoCmd(args.Trim());
		DoEcho();
	}else{ Ticks+=Period;CalcSimSpeed();
		bool OneSecond=IsTick(tmSecond);
		if(Error==null){
			if(IsTick(tmRecognize)){OnRecognize();if(Error!=null)return;}
			if(OneSecond&&!CheckError()){tmRecognize=60;Recognizer=0;}
			else if(IsTick(tmFrequency))DoMain();
		}else{
			if(IsTick(tmRecognize))OnRecognize(600,true);
			else if (IsTick(tmLCD)){tmLCD = Tm(60);
				string Txt = Error + "\n[Refresh " + TmSeconds(tmRecognize-Ticks) + "]";
				if (MyLCD != null) { PrepareText(MyLCD, Color.Red, TextAlignment.CENTER, 1.5f); MyLCD.WriteText(Txt); }}
		}		
		
		if(OneSecond){
			if(SettingsHashCode!=Me.CustomData.GetHashCode())
				if(Me.CustomData.Trim()=="")WriteSettings();else{tmRecognize=Tm(60);Recognizer=0;}

			DoEcho();
			tmSecond=Tm(60);}
			if(IsTick(tmLCD))DrawOnLCD();
	}
	cIPS+=Runtime.CurrentInstructionCount;
	if(IsTick(tmIPS)){tmIPS=Tm(60);IPS=cIPS;cIPS=0;}
}



// Info
string ReportBlocks = "";
void UpdateInfo(){
	ReportBlocks = "\nConnectors = " + Connectors.Count;

}



// Echo
void DoEcho(){
	var Txt = EColor("--== APS Script ==--", "00A0FF") + "\nWorkingTime = " + TmSeconds(Ticks) + "\nRuntime.Ms = " + PBTime.ToString("f2") + "\nRuntime.IPS = "  + (IPS*100/(Runtime.MaxInstructionCount+1)).ToString() +"%\n";
	if (DEBUG != null) 
		Txt += "DEBUG:\n " + EColor(DEBUG, "FFFF00")+"\n";
	else{
		if (Control == null) Txt += EColor("Warning: No Cockpit!", "FFFF00") + "\n";
		if (Error != null) Txt += EColor(Error, "FF0000") + "\n";
	}
	Echo(Txt);
}


// LCD
static string DEBUG=null;
IMyTextSurface MyLCD = null;
List<IMyTextSurface> LCDDraw=new List<IMyTextSurface>();
long tmResetImage=3600;
long tmLCD = 0;
bool InvalideLCD = true;
int DrawLCDIndex = 0;

void DrawOnLCD(){
	bool ResetImage = IsTick(tmResetImage);
	if (InvalideLCD) {foreach (var lcd in LCDDraw)PrepareDraw(lcd);InvalideLCD = false;}
	
	if (DrawLCDIndex >= LCDDraw.Count) DrawLCDIndex=0;
	for(int lcdLimit = 0; lcdLimit < 10; lcdLimit++){
		if (DrawLCDIndex >= LCDDraw.Count) break;
		var lcd = LCDDraw[DrawLCDIndex++];
		var frame = lcd.DrawFrame();

		var ViewPos = ((Vector2I)(lcd.TextureSize - lcd.SurfaceSize)) >> 1;
		var ViewSize = ((Vector2I)lcd.SurfaceSize)>>1;

		if (DEBUG!=null)DrawText(frame, Color.Yellow, DEBUG, ViewPos);
		frame.Dispose();
	}
	if(ResetImage){tmResetImage=Tm(3600);tmLCD=Tm(11);}
}
static bool ParseVector3(string sX, string sY, string sZ, out Vector3D v){v=Vector3D.Zero;return (double.TryParse(sX, out v.X)&&double.TryParse(sY, out v.Y)&&double.TryParse(sZ, out v.Z));}
static string StrV3(Vector3D v, string f = "f2", char ch = ',') { return v.X.ToString(f)+ch+v.Y.ToString(f)+ch+v.Z.ToString(f); }

// APS
long tmAPSFrequency = 120;

class MovedObj{
public MovedObj(IMyShipConnector Connector, double gap = 0, Vector3D? u = null){
ID=Connector.EntityId;var m=Connector.WorldMatrix;
Name=Connector.CustomName;
Position=m.Translation + m.Forward * gap;
Velocity=(Vector3D)Connector.CubeGrid.LinearVelocity;
Forward=m.Forward;
Up=u??m.Up;
}
	
public string ToStr(char sc=':'){return "#GPS"+sc+Name+sc+StrV3(Position,"f2",':')+sc+sc+ID+sc+StrV3(Forward,"f2",':')+sc+StrV3(Up,"f2",':')+sc+StrV3(Velocity,"f2",':');}
	
public long ID=0;
public string Name = null;
public Vector3D Position = Vector3D.Zero;
public Vector3D Velocity = Vector3D.Zero;
public Vector3D Forward = Vector3D.Forward;
public Vector3D Up = Vector3D.Up;
}

static string V3ToStr(Vector3D v){return v.X.ToString("f2")+","+v.Y.ToString("f2")+","+v.Z.ToString("f2");}

void APS_Send(string MSG){IGC.SendBroadcastMessage(APSSendTag, MSG, TransmissionDistance.TransmissionDistanceMax);}
IMyMessageProvider APSListner = null;
void APS_Receiver(){
	APSListner = APSListner ?? IGC.RegisterBroadcastListener(APSReceiveTag);
	for(int Limit = 0; Limit < 20; Limit++){var Message = APSListner.AcceptMessage().Data;if (Message == null)break;
	else{ var Msg = Message.ToString();
		;
	}}
}


// Server SimSpeed Manager
long LastTime=0;
double PBCurTime = 0;
double PBTime = 0;
string sNow="00:00:00";
string sSim="0.00";
bool SimSpeedLow=false;
bool BurnFuse = false;
long tmBurnWarning = 120;
long tmSecond = 60;
long cIPS = 0;
long IPS = 0;
long tmIPS=0;
void CalcSimSpeed(){if(IsTick(tmSecond)){
 var DT=DateTime.Now;
 long NowTime=DT.ToFileTime();
 sNow=DT.ToString("HH:mm:ss");
 float ServerSimSpeed=10000001.0f/(NowTime-LastTime+1);
 if(ServerSimSpeed>1.0f)ServerSimSpeed=1.0f;
 sSim=ServerSimSpeed.ToString("f2");
 SimSpeedLow=(ServerSimSpeed<0.7);
 LastTime=NowTime; 
 PBTime = PBCurTime/100.0; PBCurTime=0; }
 BurnFuse = PBCurTime < PBLimit;
 if(!BurnFuse) tmBurnWarning = Tm(120); 
}


// Basic
const float G = 9.81f;
const double ToDegree = 57.295779513082320876798;
const double PiDiv2 = 1.5707963267948966192313;
const double Pi = 3.1415926535897932384626;
const double Pi2 = 6.2831853071795864769253;
const UpdateType RuntimeFlag = UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100;
static long Ticks = 0;
static long Period = 0;
static bool IsTick(long tm) { return (Ticks >= tm); }
static long Tm(long p) { return Ticks + p; }
static string TmSeconds(long tm) { return (Math.Max(0, tm) / 60) + " sec"; }
static List<long> IDs = new List<long>();
static bool Once(IMyTerminalBlock b) { long n = b.EntityId;int i = IDs.BinarySearch(n); var r = i < 0; if (r) IDs.Insert(~i, n); return r; }
void SetScriptSpeed(UpdateFrequency f,long p){Runtime.UpdateFrequency=f; Period = p;}
static string EColor(string s,string c){return (DEBUG==null)?"[Color=#FF"+c+"]"+s+"[/Color]":s;}
static float Fix01(double x, double a = 0, double b = 1) { return (float)((x < a) ? a : ((x > b) ? b : x)); }
static bool IsZeroEps(Vector3D v, double eps = 0.01) { return (-eps < v.X && v.X < eps) && (-eps < v.Y && v.Y < eps) && (-eps < v.Z && v.Z < eps); }
static Vector3D N(Vector3D x) { return (x.IsZero()) ? Vector3D.Zero : Vector3D.Normalize(x); }
static Vector2D N(Vector2D x) { return (x.X == 0 && x.Y == 0) ? Vector2D.Zero : Vector2D.Normalize(x); }
static Vector3D N(Vector3D x, double l) { return (x.LengthSquared() > l * l) ? Vector3D.Normalize(x) * l : x; }
static Vector3D VectorIsNot(Vector3D a, Vector3D b) { return new Vector3D((a.X == 0) ? b.X : a.X, (a.Y == 0) ? b.Y : a.Y, (a.Z == 0) ? b.Z : a.Z); }
static string DistToStr(double l){var m="";if(l>500){l/=1000;m="km";}return l.ToString("f2")+m;}
static string Str(double d,string f="f2"){return d.ToString(f);}
static string StrL(string s, int l=20) { return (s.Length>l)?s.Substring(0,l-3)+"...":s; }
static string SetL(string s, int l = 2, char c = ' ') { if (s.Length < l) s = new string(c, l - s.Length) + s; return s; }
static bool IsCockpit(string TypeName) { return !TypeName.Contains("Bed") && !TypeName.Contains("Passenger") && !TypeName.Contains("Toilet") && !TypeName.Contains("Bathroom"); }
static Base6Directions.Direction GetDirection(MyBlockOrientation O, IMyTerminalBlock b, Base6Directions.Direction D) { return O.TransformDirectionInverse(b.Orientation.TransformDirection(D)); }
static Vector3D Projection(MatrixD m, Vector3D v) { return new Vector3D(m.Left.Dot(v), m.Down.Dot(v), m.Forward.Dot(v)); }
static Vector3D ProjectionInv(MatrixD m, Vector3D v) { return (m.Left * v.X) + (m.Down * v.Y) + (m.Forward * v.Z); }
static double GetCurrentForce(IMyThrust t) { return (t != null) ? t.CurrentThrustPercentage : 0; }
static float SignG(double x, float eps = G * 0.4f) {return (x < -eps)? -G : ((eps < x)? G : (float)x);}
static Vector3D GetRotationIndicator(IMyShipController b) { var rotation = b.RotationIndicator; return new Vector3D(rotation.Y, rotation.X, b.RollIndicator); }
static void SortList<T>(List<T> l, Vector3D C){l.Sort(delegate(T v1, T v2){
 var t1=((v1 as IMyTerminalBlock).GetPosition()-C).LengthSquared();
 var t2=((v2 as IMyTerminalBlock).GetPosition()-C).LengthSquared();
 return (t1<t2)?-1:(t1>t2)?1:0;});}
 
static bool ReadVector3D(ref Vector3D v, string s) {bool b=false;if(s.ToLower().Trim()=="no"){v=Vector3D.Zero;b=true;}else{double x,y,z;var ss=s.Split(',');if(ss.Length>=3&&double.TryParse(ss[0],out x)&&double.TryParse(ss[1],out y)&&double.TryParse(ss[2],out z)){v=new Vector3D(x,y,z);b=true;}}return b;}
static Vector2D? ReadVector2D(string s) {double x,y;var ss=s.Split(',');if(ss.Length>=2&&double.TryParse(ss[0],out x)&&double.TryParse(ss[1],out y))return new Vector2D(x,y);return null;}
static double rValue = 0;static bool ReadValue(string s){if(s.ToLower().Trim()=="no"){rValue=0;return true;};return double.TryParse(s,out rValue);}
static void ReadBool(ref bool v, string s,bool Change) { if((s=="switch"||s=="") && Change)v=!v; else if(s == "yes" || s == "y") v = true; else if(s == "no" || s == "n") v = false; }

void SearchMyLCD(){if (MyLCDIndex >= 0 && Me is IMyTextSurfaceProvider){var Provider = (Me as IMyTextSurfaceProvider);if (Provider != null) MyLCD = Provider.GetSurface(MyLCDIndex);}}
static void PrepareText(IMyTextSurface lcd,Color C,TextAlignment A=TextAlignment.LEFT,float FS=1.5f,float TP = 0.0f,string FN = "DEBUG"){
lcd.ContentType = ContentType.TEXT_AND_IMAGE;lcd.BackgroundColor = new Color(0, 8, 16);lcd.FontSize = FS;lcd.Font = FN;lcd.FontColor = C;lcd.TextPadding = TP;lcd.Alignment = A;lcd.ClearImagesFromSelection();}

static void PrepareDraw(IMyTextSurface l){l.ContentType = ContentType.SCRIPT; l.Script = ""; }

static void DrawTexture(MySpriteDrawFrame frame, Color color, string shape, Vector2I p, Vector2I size, float rotation = 0f){p.Y+=size.Y>>1;frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = shape, Position = p, Size = size, RotationOrScale = rotation, Color = color });}
static void DrawTextureC(MySpriteDrawFrame frame, Color color, string shape, Vector2I p, Vector2I size, float rotation = 0f){p.X-=size.X>>1;frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = shape, Position = p, Size = size, RotationOrScale = rotation, Color = color });}
static void DrawText(MySpriteDrawFrame frame, Color color, string text, Vector2I position, float scale = 0.75f, TextAlignment alignment = TextAlignment.LEFT){frame.Add(new MySprite() { Type = SpriteType.TEXT, Data = text, Position = position, RotationOrScale = scale, Color = color, Alignment = alignment, FontId = "White" });}
static void DrawProgres(MySpriteDrawFrame frame, Color c1, Color c2, Vector2I pos, Vector2I sz, double v){var sz_ = sz;sz_.X=(int)(sz.X*Fix01(v));DrawTexture(frame, c1, "SquareSimple", pos, sz_);pos.X+=sz_.X;sz_.X=sz.X-sz_.X;DrawTexture(frame, c2, "SquareSimple", pos, sz_);}
