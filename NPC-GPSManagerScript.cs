//////////////////////////////
//    NPC - GPS Manager     //
//  by Misha.Malanyuk (UA)  //
//////////////////////////////


const string PlanetGpsColor = "#FF00FFFF";
const string OrbitalGpsColor = "#FF00FF00";
const string MidSpaceGpsColor = "#FFFFFFFF";
const string DeapSpaceGpsColor = "#FF7F7F7F";
const string RangeGpsColor = "#FFFFFF00";


const double MiddleSpaceSize = 15000 * km;

void PlanetsSettup(){


	// Planet: Name, Gps of Center
	// Size(km): Planet Radius, GapRange on S

	NewPlanet("Moon", 16384, 136384, -113616,
	/* Size: */ 10, 2, 30); // km

	NewPlanet("Earth", 0, 0, 0,
	/* Size: */ 62); // km



	NewPlanet("Europa", 916384, 16384, 1614384,
	/* Size: */ 10, 2, 30); // km

	NewPlanet("Mars", 1031072, 131072, 1631072,
	/* Size: */ 62); // km



	NewPlanet("Titan", 36384, 226384, 5796384,
	/* Size: */ 10, 2, 30); // km

	NewPlanet("Alien", 131072, 131082, 5731072,
	/* Size: */ 62); // km



	NewPlanet("Triton", -284463, -2434463, 365536,
	/* Size: */ 42); // km

	NewPlanet("Pertam", -3967232, -32232, -767232,
	/* Size: */ 30, 10); // km
}








const string Line = "-------------------------------------\n";
const string Title = "--== NPC - GPS Manager 1.0 ==--\n";
const string Info = 
	"\n\n\n"+
	" My.CustomData is storage for GPS of NPC.\n\n"+
	"--==  Commands  ==--\n"+
	" \"GPS\" - add NPC-station to list, and scan of planet near.\n\n"+
	" \"ТІІВ\" - add my current position like NPC-station to list, and scan of planet near.\n\n"+
	" \"32322\" - write into Castom data, NPC station on that input distance (km)\n\n"+
	" \"20..400\" - write into CustomData, NPC station in that input range (km)\n\n"+
	" \"#Write\" - rewrite CustomData\n\n";
const double km = 1000;
const double kmSqr = km * km;
const double MiddleSpaceSizeSqr = MiddleSpaceSize * MiddleSpaceSize;
List<Planet> PlanetList = new List<Planet>();
List<GPS> NPC = new List<GPS>();
Program(){
	PlanetsSettup();
	LoadList();
	Runtime.UpdateFrequency = UpdateFrequency.None;
	Echo(Title);
	Echo(" Planets = "+PlanetList.Count);
	Echo(" GPS = "+NPC.Count);
	Echo(Info);
}

void Main(string arg, UpdateType updateSource){
	arg=arg.Trim();var argLow=arg.ToLower();
	if (arg=="") return;

	Echo(Title); 
	Echo(" GPS = "+NPC.Count+"\n");
	int sp = -1;
	var sName = "";
	var gps = Vector3D.Zero;
	
	if (arg[0]=='#'){
		arg=arg.Substring(1).TrimStart();
		if (argLow == "#write") WirteNPCByPlanet();
		else Echo("Sorry Wrong Command");
		Echo(Info);
	return;}
	
	double dist = 0;
	if (double.TryParse(arg, out dist)){
		Me.CustomData = Line;
		Me.CustomData += "            NPC from here ["+dist.ToString("f0")+" km]:\n";
		var max = (dist + 10) * km;
		var min = (dist - 10) * km;
		Echo("  Dist = "+dist.ToString()+" km");
		WriteRange(min, max);
		WirteNPCByPlanet(true);
		Echo(Info);
		return;}
		
	sp = arg.IndexOf("..");
	if (sp >= 0){
		var sMin = arg.Substring(0,sp).TrimEnd();
		var sMax = arg.Substring(sp+2).TrimStart();
		
		double min, max;
		if(double.TryParse(sMin, out min) && double.TryParse(sMax, out max)){
			Me.CustomData = Line;
			Me.CustomData += "            In Range ["+min.ToString("f0")+".."+max.ToString("f0")+" km]:\n";
			Echo("  Add Range into CustomData!");
			WriteRange(min * km, max * km);
			WirteNPCByPlanet(true);
			Echo(Info);
			return;}}

	if (!GpsParse(arg, out sName, out gps) || string.IsNullOrEmpty(sName)) { Echo("Sorry Wrong Command"); Echo(Info); return; }

	sp = sName.IndexOf(" - Station");
	if (sp < 0) { Echo("Wrong GPS name!\n No standart of NPC"); Echo(Info); return; }
	var sFaction = sName.Substring(0,sp).TrimEnd();
	
	int FactionSymbolsCount = sFaction.Length;
	if (FactionSymbolsCount < 3 || FactionSymbolsCount > 8) { Echo("Wrong GPS name!\n NPC-Faction Name Wrong!");  Echo(Info); return; }
	sFaction = " (" + sFaction + ")";

	bool GpsOnSpace = true;
	foreach (var p in PlanetList)
	{
		int cmp = p.CompareGPS(gps);
		if (cmp == 0) { sName = "Tr-" + p.Name + sFaction; GpsOnSpace = false; break; }
		if (cmp == 1) { sName = "Tr-Orb." + p.Name + sFaction; GpsOnSpace = false; break; }
	}

	if (GpsOnSpace)
	{
		var leng = gps.Length();
		if (leng <= MiddleSpaceSize) sName = "Tr-Space " + sFaction;
		else sName = "Tr-DeapSpace " + sFaction;
		sName += " [" + ((leng / 1000 / km).ToString("f0")) + "k]";
	}

	if (!AddToList(sName, gps)) { Echo("Found Duplicate!\n This NPC-station already in the List"); return; }
	Echo("GPS-Named:" + sName);
	Echo("New GPS in the list!");
	WirteNPCByPlanet();
	Echo(Info);
}

void LoadList(){
	var sName = "";
	var gps = Vector3D.Zero;
	foreach (var line in Me.CustomData.Split('\n'))
		if (GpsParse(line, out sName, out gps))
			AddToList(sName, gps);
}

bool AddToList(string sName, Vector3D coord) {
	foreach (var npc in NPC)
		if ((npc.Position - coord).LengthSquared() < kmSqr)return false;
	NPC.Add(new GPS(sName, coord));
	return true;
}

void WriteNPC(){
	Me.CustomData = "";
	foreach (var npc in NPC) Me.CustomData += npc.ToStr();
}


void WriteRange(double min, double max){
//	var minSqr=min * min;
//	var maxSqr=max * max;
	int Count=0;
	var MyPos=Me.GetPosition();
	foreach(var gps in NPC){
		var dist = (gps.Position-MyPos).Length();
		if(min <= dist && dist <= max){
			Me.CustomData+=gps.ToStr(RangeGpsColor)+"\n";
			Me.CustomData+="    ["+(dist/km).ToString("f1")+" km] \n\n";
			Count++;}
	}
	Echo(" Found "+Count+" Stations!");
	Me.CustomData+="\n";
}

void WirteNPCByPlanet(bool Append = false) {
	if(!Append) Me.CustomData = "";

	for(int i=0; i < PlanetList.Count; i++) {
		var p = PlanetList[i];
		p.NPC_Surface.Clear();
		p.NPC_Orbital.Clear(); }

	var NPC_Space = new List<GPS>();
	foreach (var gps in NPC) {

		bool GpsOnSpace = true;
		for(int i=0; i < PlanetList.Count; i++) {
			var p = PlanetList[i];
			int cmp = p.CompareGPS(gps.Position);
			if (cmp == 0) { p.NPC_Surface.Add(gps); GpsOnSpace = false; break; }
			if (cmp == 1) { p.NPC_Orbital.Add(gps); GpsOnSpace = false; break; }}

		if (GpsOnSpace) NPC_Space.Add(gps);
	}

	foreach(var p in PlanetList){
		int OrbitalCount = p.NPC_Orbital.Count;
		int SurfaceCount = p.NPC_Surface.Count;
		if (OrbitalCount == 0 && SurfaceCount == 0) continue;
		
		if (OrbitalCount > 0){
			Me.CustomData += "\n\n"+Line;
			Me.CustomData += "            "+p.Name+" (Orbital):\n";
			foreach (var gps in p.NPC_Orbital) Me.CustomData += gps.ToStr(OrbitalGpsColor) + '\n';}
		
		if (SurfaceCount > 0){
			Me.CustomData += "\n\n"+Line;
			Me.CustomData += "            " + p.Name + " (Surface):\n";
			foreach (var gps in p.NPC_Surface) Me.CustomData += gps.ToStr(PlanetGpsColor) + '\n';}
	}

	foreach (var p in PlanetList){
		p.NPC_Surface.Sort((gps1, gps2) => (string.Compare(gps1.Name, gps2.Name)));
		p.NPC_Orbital.Sort((gps1, gps2) => (string.Compare(gps1.Name, gps2.Name)));}
	
	int MidCount = 0;
	if(NPC_Space.Count > 0){
		foreach (var gps in NPC_Space)
			if (gps.Position.LengthSquared() <= MiddleSpaceSizeSqr){
				if(MidCount==0){
					Me.CustomData += "\n\n"+Line;
					Me.CustomData += "            Middle Space \n";}
					
				Me.CustomData += gps.ToStr(MidSpaceGpsColor) + '\n';
				MidCount++;}

		if(MidCount < NPC_Space.Count){
			Me.CustomData += "\n\n"+Line;
			Me.CustomData += "            Deap Space\n";
			foreach (var gps in NPC_Space)
				if (gps.Position.LengthSquared() > MiddleSpaceSizeSqr)
					Me.CustomData += gps.ToStr(DeapSpaceGpsColor) + '\n';}
	}

	for(int i=0; i < PlanetList.Count; i++) {
		var p = PlanetList[i];
		p.NPC_Surface.Clear();
		p.NPC_Orbital.Clear(); }

	Echo("GPS sorted by Planets!");
}

bool GpsParse(string str, out string sName, out Vector3D gps)
{
	sName = "";
	gps = Vector3D.Zero;
	var ss = str.Split(':');
	if (ss.Count() < 6) return false;
	if (ss[0] != "GPS") return false;

	double X, Y, Z;
	if (!double.TryParse(ss[2], out X)) return false;
	if (!double.TryParse(ss[3], out Y)) return false;
	if (!double.TryParse(ss[4], out Z)) return false;

	sName = ss[1];
	gps = new Vector3D(X,Y,Z);
	return true;
}

void NewPlanet(string sName, double posX, double posY, double posZ, double radius = 60, double surfaceGap = 10, double spaceGap = 300) {
	PlanetList.Add(new Planet(sName, new Vector3D(posX, posY, posZ), radius+surfaceGap, radius+surfaceGap+spaceGap));}


struct GPS{
public GPS(string sName, Vector3D pos){Name = sName;Position = pos;}
public string ToStr(string sColor="#75C9F1", string f = "f0") { return "GPS:"+Name+":" + Position.X.ToString(f) + ":" + Position.Y.ToString(f) + ":" + Position.Z.ToString(f) + ":" + sColor + ":"; }
public string Name;
public Vector3D Position;
}


struct Planet{
public Planet(string sName, Vector3D pos, double SurfaceGap = 70, double SpaceGap = 270){
		Name = sName;
		Position = pos;
		
		SurfaceGap *= km;
		SpaceGap *= km;

		SurfaceSqr = SurfaceGap * SurfaceGap;
		SpaceSqr = SpaceGap * SpaceGap;
		
		NPC_Surface = new List<GPS>();
		NPC_Orbital = new List<GPS>();}

public int CompareGPS(Vector3D coord) {
		var dist = (coord-Position).LengthSquared();
		if (dist < SurfaceSqr) return 0;
		if (dist < SpaceSqr) return 1;
		return -1;}

public string Name;
public Vector3D Position;
public double SurfaceSqr;
public double SpaceSqr;

public List<GPS> NPC_Surface;
public List<GPS> NPC_Orbital;
}
