/*Orbital Drop Position Calculator by sunoko*/

//=============BASIC SETTINGS=============//
//Distance between drop point and jump point
double offsetDistance = 1000;
//Text panel tag, should write in CustomData
const string lcdName = "ODPCLCD";
//Show planet data on lcd(Only display, not configuable)
bool showPlanetDataToLCD = false;
//========================================//

//--------DO NOT EDIT BELLOW CODE---------//
//-------------other settings-------------//
//script name
const string scriptName = "Orbital Drop Position Calculator";
//----Built in planet settings----//
//Format = {Position.X, Position.Y, Position.Z, MaxHillRadius, DefaultGravity}
double[] planetEarthlike = new double[]{0.5,0.5,0.5,67200,1.0};
double[] planetMoon = new double[]{16384.5,136384.5,-113615.5,9785,0.25};
double[] planetMars = new double[]{1031072.5,131072.5,1631072.5,67200,0.9};
double[] planetEuropa = new double[]{916384.5,16384.5,1616384.5,10070,0.25};
double[] planetArien = new double[]{131072.5,131072.5,5731072.5,67200,1.1};
double[] planetTitan = new double[]{36384.5,226384.5,5796384.5,9785,0.25};
double[] planetTriton = new double[]{-284463.5,-2434463.5,365536.5,48151.8,1.0};
double[] planetArrakis = new double[]{-3967231.5,-32231.5,-767231.5,30818.16,1.2};
//---------------block list---------------//
List<IMyTerminalBlock> lcdList = new List<IMyTerminalBlock>();
//----------------variable----------------//
HashSet<PlanetData> planetsList = new HashSet<PlanetData>();
MyIni ini = new MyIni();
string dropPos = "-";
string jumpPos = "-";
Vector3D subjectPos = Vector3D.Zero;

public Program()//Program() is run once at loading PB
{
    Echo(scriptName);
    Echo("Please input GPS to Argument and pless Run");
    //Load default planet data
    planetsList.Add(new PlanetData("Earthlike",new Vector3D(planetEarthlike[0],planetEarthlike[1],planetEarthlike[2]),planetEarthlike[3],planetEarthlike[4]));
    planetsList.Add(new PlanetData("Moon",new Vector3D(planetMoon[0],planetMoon[1],planetMoon[2]),planetMoon[3],planetMoon[4]));
    planetsList.Add(new PlanetData("Mars",new Vector3D(planetMars[0],planetMars[1],planetMars[2]),planetMars[3],planetMars[4]));
    planetsList.Add(new PlanetData("Europa",new Vector3D(planetEuropa[0],planetEuropa[1],planetEuropa[2]),planetEuropa[3],planetEuropa[4]));
    planetsList.Add(new PlanetData("Alien Planet",new Vector3D(planetArien[0],planetArien[1],planetArien[2]),planetArien[3],planetArien[4]));
    planetsList.Add(new PlanetData("Triton",new Vector3D(planetTriton[0],planetTriton[1],planetTriton[2]),planetTriton[3],planetTriton[4]));
    planetsList.Add(new PlanetData("Arrakis",new Vector3D(planetArrakis[0],planetArrakis[1],planetArrakis[2]),planetArrakis[3],planetArrakis[4]));
    
    ConfigHandler(true);
    WriteToLCD();
}

public void Main(string argument,UpdateType type)
{
    Echo(scriptName);
//convert gps to Vector3D
    Vector3D inputPos = Vector3D.Zero;
    string gpsName = "";
    if(!TryConvertGPSToVector3D(argument,out inputPos,out gpsName)){
        Echo("ERROR : GPS Translation Failure");
        Echo("Please input GPS to Argument and pless Run");
        return;
    }
//find most close planet
    PlanetData planet = FindClosestPlanet(inputPos);
    if(planet == null){
        Echo("ERROR : There Are No Planet Nearby");
        Echo("Please input GPS to Argument and pless Run");
        return;
    }
//calc jump position
    Vector3D drop = (Vector3D.Normalize(inputPos - planet.Position) * (planet.GravityRadius)) + planet.Position;
    Vector3D jump = (Vector3D.Normalize(inputPos - planet.Position) * (planet.GravityRadius + offsetDistance)) + planet.Position;
    string name = $"Drop Pos({gpsName})";
    if(name.Length > 24){
        name = name.Substring(0,21) + "...";
    }
    dropPos = ConvertVector3DToGPS(name,drop);
    name = $"Jump Pos({gpsName})";
    if(name.Length > 24){
        name = name.Substring(0,21) + "...";
    }
    jumpPos = ConvertVector3DToGPS(name,jump);
    Echo("Calculate Complete\nPlease press Custom Data button");
    WriteToLCD();
    ConfigHandler(false);
}

public class PlanetData
{
    public string Name {get;}
    public Vector3D Position {get; set;}
    public double MaxHillRadius {get; set;}
    public double DefaultGravity {get; set;}
    public double GravityRadius {get; set;}
    
    public PlanetData(string Name,Vector3D Position,double MaxHillRadius = 0,double DefaultGravity = 0)
    {
        this.Name = Name;
        this.Position = Position;
        if(MaxHillRadius != 0)this.MaxHillRadius = MaxHillRadius;
        if(DefaultGravity != 0)this.DefaultGravity = DefaultGravity;
        if(MaxHillRadius != 0 && DefaultGravity != 0)this.GravityRadius = MaxHillRadius * Math.Pow((DefaultGravity / 0.05),1.0/7.0);
    }
}

private void ReplaceOrAddDataValue(string source)
{
    string[] s_data = source.Split(new string[] {"|"},StringSplitOptions.RemoveEmptyEntries);
    Vector3D pos = Vector3D.Zero;
    Vector3D.TryParse(s_data[1],out pos);
    double hill = Double.Parse(s_data[2]);
    double grav = Double.Parse(s_data[3]);
    double gravRad = Double.Parse(s_data[4]);
    foreach(PlanetData data in planetsList){
        if(data.Name == s_data[0]){
            data.Position = pos;
            data.MaxHillRadius = hill;
            data.DefaultGravity = grav;
            data.GravityRadius = gravRad;
            return;
        }
    }
    planetsList.Add(new PlanetData(s_data[0],pos,hill,grav));
}

private void WriteToLCD()
{
    if(!showPlanetDataToLCD){
        return;
    }
    lcdList = GetOwnGridBlock<IMyTextPanel>(b => b.CustomData.Contains(lcdName));
    StringBuilder sb = new StringBuilder();
    sb.Append($"---Result---\nPayload dropping position :\n{dropPos}\nShip jump position :\n{jumpPos}");
    sb.Append("\n\n---Recorded Planet Data---\n");
    foreach(PlanetData p in planetsList){
        sb.Append($"Planet Name : {p.Name}");
        sb.Append($"\n{ConvertVector3DToGPS(p.Name,p.Position)}");
        sb.Append($"\nGravity at planet surface : {p.DefaultGravity}G");
        sb.Append($"\nNatural gravity decreasing altitude(Sea level) : {p.MaxHillRadius}m");
        sb.Append($"\nNatural gravity radius : {p.GravityRadius}m\n\n");
    }
    foreach(IMyTextPanel lcd in lcdList){
        lcd.WriteText(sb.ToString(), false);
    }
}

private bool TryConvertGPSToVector3D(string GPS,out Vector3D pos,out string name)
{
    pos = Vector3D.Zero;
    name = "";
    if(!GPS.Contains("GPS")){
        return false;
    }
    string[] GPSSprit = GPS.Split(':');
    name = GPSSprit[1];
    double x = 0;double y = 0;double z = 0;
    if(!Double.TryParse(GPSSprit[2],out x)){
        return false;
    }
    if(!Double.TryParse(GPSSprit[3],out y)){
        return false;
    }
    if(!Double.TryParse(GPSSprit[4],out z)){
        return false;
    }
    pos = new Vector3D(x,y,z);
    return true;
}

private string ConvertVector3DToGPS(string name,Vector3D Position)
{
    return $"GPS:{name}:{Position.X:#.##}:{Position.Y:#.##}:{Position.Z:#.##}:FF00B7EB";
}

private PlanetData FindClosestPlanet(Vector3D coord)
{
    foreach(PlanetData planet in planetsList){
        if(Vector3D.Distance(coord,planet.Position) < planet.GravityRadius){
            return planet;
        }
    }
    return null;
}

private List<IMyTerminalBlock> GetOwnGridBlock<T>(Func<IMyTerminalBlock, bool> collect = null) where T : class, IMyTerminalBlock
{
    var blockList = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyMechanicalConnectionBlock>(blockList);
    HashSet<IMyCubeGrid> CubeGridSet = new HashSet<IMyCubeGrid>();
    CubeGridSet.Add(Me.CubeGrid);
    bool continueLoop;
    IMyMechanicalConnectionBlock block;
//get all CubeGrid connected on ship
    do{
        continueLoop = false;
        for(int i = 0;i < blockList.Count;i++){
            block = blockList[i] as IMyMechanicalConnectionBlock;
            if(CubeGridSet.Contains(block.CubeGrid) || CubeGridSet.Contains(block.TopGrid)){
                CubeGridSet.Add(block.CubeGrid);
                CubeGridSet.Add(block.TopGrid);
                blockList.Remove(blockList[i]);
                continueLoop = true;
            }
        }
    }
    while(continueLoop);

//get filtered block
    blockList.Clear();
    GridTerminalSystem.GetBlocksOfType<T>(blockList,b => CubeGridSet.Contains(b.CubeGrid) && (collect == null || collect(b)));
    return blockList;
}

private void ConfigHandler(bool initial)
{
    ini.Clear();
    MyIniParseResult result;
//---Initialize---//
    if(!Me.CustomData.Contains(scriptName) || !ini.TryParse(Me.CustomData,out result)){
        Me.CustomData = "";
    }
//---Calculation result---//
    string resultName = scriptName + " Calculation Result";
    ini.Set(resultName,"Drop Position",dropPos);
    ini.SetComment(resultName,"Drop Position","Directly above the target point,\nOn the outline of the gravity sphere");

    ini.Set(resultName,"Jump Position",jumpPos);
    ini.SetComment(resultName,"Jump Position","Directly above the target point,\nFurther away from the gravity sphere by the value of offsetDistance");
//---Config section---//
    MyIniValue value = ini.Get(scriptName,"offsetDistance");
    if(!value.IsEmpty){
        offsetDistance = value.ToDouble();
    }
    ini.Set(scriptName,"offsetDistance",offsetDistance);
    ini.SetComment(scriptName,"offsetDistance","Distance between drop point and jump point");

    value = ini.Get(scriptName,"showPlanetDataToLCD");
    if(!value.IsEmpty){
        showPlanetDataToLCD = value.ToBoolean();
    }
    ini.Set(scriptName,"showPlanetDataToLCD",showPlanetDataToLCD);
    ini.SetComment(scriptName,"showPlanetDataToLCD","Show planet data on lcd(Only display, not configuable)");

//---variable section---//
    string variablesName = scriptName + " Variables";

    value = ini.Get(variablesName,"PlanetData");
    if(!value.IsEmpty && initial){
        string[] s_source = value.ToString().Split(new string[] {"\r","\n"},StringSplitOptions.RemoveEmptyEntries);
        foreach(string s in s_source){
            ReplaceOrAddDataValue(s);
        }
    }
    StringBuilder sb = new StringBuilder();
    foreach(PlanetData data in planetsList){
        sb.Append($"{data.Name}|{data.Position.ToString()}|{data.MaxHillRadius}|{data.DefaultGravity}|{data.GravityRadius}\n");
    }
    ini.Set(variablesName,"PlanetData",sb.ToString());
    ini.SetComment(variablesName,"PlanetData","Format = |{Position.X, Position.Y, Position.Z, MaxHillRadius, DefaultGravity} + LF");

    Me.CustomData = ini.ToString();
}