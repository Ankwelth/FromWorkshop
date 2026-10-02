/*Orbital Drop Position Calculator by sunoko*/
/*Updated by Brenku - Special thanks to these guys: MartinRWolfe and Malware */
/*Updated by JejeServeur */

//=============BASIC SETTINGS=============//
//jump position offset distance from edge of gravitational field
double offsetDistance = 1000;
//text panel tag, write in CustomData
const string lcdName = "CalcLCD";
//show planet data to lcd(only display, not configuable)
bool showPlanetDataToLCD = true;
//========================================//

//--------DO NOT EDIT BELLOW CODE---------//
//-------------other settings-------------//
//script name
const string scriptName = "Orbital Drop Position Calculator";
//--------built in planet settings--------//
//Earth-Like
Vector3D earthlikePos = new Vector3D(0.5,0.5,0.5);
double maxHillRadiusEarthlike = 67200;
double gravityEarthlike = 1.0;
//Moon
Vector3D moonPos = new Vector3D(16384.5,136384.5,-113615.5);
double maxHillRadiusMoon = 9785;
double gravityMoon = 0.25;
//Mars
Vector3D marsPos = new Vector3D(1031072.5,131072.5,1631072.5);
double maxHillRadiusMars = 67200;
double gravityMars = 0.9;
//Europa
Vector3D europaPos = new Vector3D(916384.5,16384.5,1616384.5);
double maxHillRadiusEuropa = 10070;
double gravityEuropa = 0.25;
//Alien
Vector3D alienPos = new Vector3D(131072.5,131072.5,5731072.5);
double maxHillRadiusAlien = 67200;
double gravityAlien = 1.1;
//Titan
Vector3D titanPos = new Vector3D(36384.5,226384.5,5796384.5);
double maxHillRadiusTitan = 9785;
double gravityTitan = 0.25;
//Triton
Vector3D tritonPos = new Vector3D(-284463.5,-2434463.5,365536.5); 
double maxHillRadiusTriton = 48151.8;
double gravityTriton = 1.0;
//pertam
Vector3D pertamPos = new Vector3D(-3967231.5,-32231.5,-767231.5);
double maxHillRadiuspertam = 30818.1621;
double gravitypertam = 1.2;

//---------------block list---------------//
List<IMyTextPanel> lcdList = new List<IMyTextPanel>();
//----------------variable----------------//
List<PlanetData> planetsList = new List<PlanetData>();
string dropPos = "-";
string jumpPos = "-";
Vector3D subjectPos = Vector3D.Zero;

public Program()//Program() is run once at loading PB
{
    Echo(scriptName);
    if(!Me.CustomData.Contains("#end")){
    //load default planet data
        planetsList.Add(new PlanetData("Earthlike",earthlikePos,maxHillRadiusEarthlike,gravityEarthlike));
        planetsList.Add(new PlanetData("Moon",moonPos,maxHillRadiusMoon,gravityMoon));
        planetsList.Add(new PlanetData("Mars",marsPos,maxHillRadiusMars,gravityMars));
        planetsList.Add(new PlanetData("Europa",europaPos,maxHillRadiusEuropa,gravityEuropa));
        planetsList.Add(new PlanetData("Alien Planet",alienPos,maxHillRadiusAlien,gravityAlien));
        planetsList.Add(new PlanetData("Titan",titanPos,maxHillRadiusTitan,gravityTitan));
		planetsList.Add(new PlanetData("Triton",tritonPos,maxHillRadiusTriton,gravityTriton));
		planetsList.Add(new PlanetData("Pertam",pertamPos,maxHillRadiuspertam,gravitypertam));
        StoreConfig();
    }
    WriteToLCD();
}

public void Main(string argument,UpdateType type)
{
    Echo(scriptName);
    if(Me.CustomData.Contains("#end")){
        LoadConfig();
        LoadPlanetData();
    }
//convert gps to Vector3D
    Vector3D inputPos = Vector3D.Zero;
    string gpsName = "";
    if(!TryConvertGPSToVector3D(argument,out inputPos,out gpsName)){
        Echo("ERROR : Translation Failure");
        return;
    }
//find most close planet
    PlanetData planet = FindClosestPlanet(inputPos);
    if(planet == null){
        Echo("ERROR : There Are No Planet Nearby");
        return;
    }
//calc jump position
    Vector3D drop = (Vector3D.Normalize(inputPos - planet.Position) * (planet.GravityRadius)) + planet.Position;
    Vector3D jump = (Vector3D.Normalize(inputPos - planet.Position) * (planet.GravityRadius + offsetDistance)) + planet.Position;
    dropPos = ConvertVector3DToGPS("Drop Position",drop);
    jumpPos = ConvertVector3DToGPS("Jump Position",jump);
    Echo("Calculate Complete\nPlease press Custom Data button");
    WriteToLCD();
    StoreConfig();
}

public class PlanetData
{
    public string Name {get;}
    public Vector3D Position {get;}
    public double MaxHillRadius {get;}
    public double DefaultGravity {get;}
    public double GravityRadius {get;}
    
    public PlanetData(string Name,Vector3D Position,double MaxHillRadius,double DefaultGravity)
    {
        this.Name = Name;
        this.Position = Position;
        this.MaxHillRadius = MaxHillRadius;
        this.DefaultGravity = DefaultGravity;
        this.GravityRadius = MaxHillRadius * Math.Pow((DefaultGravity / 0.05),1.0/7.0);
    }
}

private void WriteToLCD()
{
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcdList);
    string text = "---Result---\nPayload dropping position :\n" + dropPos + "\nShip jump position :\n" + jumpPos;
    if(showPlanetDataToLCD){
        text += "\n\n---Recorded Planet Data---\n";
        foreach(PlanetData p in planetsList){
            text += "Planet Name : " + p.Name
            + "\n" + ConvertVector3DToGPS(p.Name,p.Position)
            + "\nGravity at planet surface : " + p.DefaultGravity + "G"
            + "\nNatural gravity decreasing altitude(Sea level) : " + p.MaxHillRadius + "m"
            + "\nNatural gravity radius : " + p.GravityRadius + "m\n\n";
        }
    }
    foreach(IMyTextPanel lcd in lcdList){
		if (lcd.CustomName == lcdName) {
			lcd.WriteText(text, false);
			lcd.ContentType = ContentType.TEXT_AND_IMAGE;
		}
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
    return "GPS:" + name + ":" + Position.X.ToString("0.00") + ":" + Position.Y.ToString("0.00") + ":" + Position.Z.ToString("0.00") + ":";
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

private void StoreConfig()
{
    StringBuilder config = new StringBuilder();
    config.Append(scriptName + "\n");
    config.Append("\n//RESULT\n");
    config.Append("//Drop Position = " + dropPos + "\n");
    config.Append("//Jump Position = " + jumpPos + "\n");
    config.Append("\n//CONFIG\n");

    config.Append("offsetDistance = " + offsetDistance.ToString() + "\n\n");
    config.Append("showPlanetDataToLCD = " + showPlanetDataToLCD.ToString() + "\n\n");
    foreach(PlanetData planet in planetsList){
        config.Append("<PlanetData>\n");
        config.Append("Name = " + planet.Name + "\n");
        config.Append("Position = " + planet.Position + "\n");
        config.Append("MaxHillRadius = " + planet.MaxHillRadius.ToString() + "\n");
        config.Append("DefaultGravity = " + planet.DefaultGravity.ToString() + "\n");
        config.Append("</PlanetData>\n");
    }
    
    config.Append("#end");
    Me.CustomData = config.ToString();
}

private void LoadConfig()
{
    string[] configSplit = Me.CustomData.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    foreach(var str in configSplit){
        if(str.Contains("//") || str.Contains("!none") ||str.Contains("#end")){
            continue;
        }
        if(ConvertVariable(str,"offsetDistance = ",ref offsetDistance)){continue;}
        if(ConvertVariable(str,"showPlanetDataToLCD = ",ref showPlanetDataToLCD)){continue;}
    }
}

private void LoadPlanetData()
{
    planetsList.Clear();
    string[] configSplit = Me.CustomData.Split(new string[] { "<PlanetData>", "</PlanetData>" }, StringSplitOptions.RemoveEmptyEntries);
    foreach(var str in configSplit){
       if(!str.Contains("Name") && !str.Contains("GPS") && !str.Contains("MaxHillRadius") && !str.Contains("DefaultGravity")){
            continue;
        }
        string[] dataSplit = str.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string n = "";
        Vector3D p = Vector3D.Zero;
        double m = 0;
        double g = 0;
        foreach(var strd in dataSplit){
            if(ConvertVariable(strd,"Name = ",ref n)){continue;}
            if(ConvertVariable(strd,"Position = ",ref p)){continue;}
            if(ConvertVariable(strd,"MaxHillRadius = ",ref m)){continue;}
            if(ConvertVariable(strd,"DefaultGravity = ",ref g)){continue;}
        }
        planetsList.Add(new PlanetData(n,p,m,g));
    }
}

public bool ConvertVariable(string line,string name,ref bool variable)
{
    if(line.Contains(name)){
        Boolean.TryParse(line.Replace(name,""),out variable);
        return true;
    }
    return false;
}

public bool ConvertVariable(string line,string name,ref int variable)
{
    if(line.Contains(name)){
        Int32.TryParse(line.Replace(name,""),out variable);
        return true;
    }
    return false;
}

public bool ConvertVariable(string line,string name,ref float variable)
{
    if(line.Contains(name)){
        Single.TryParse(line.Replace(name,""),out variable);
        return true;
    }
    return false;
}

public bool ConvertVariable(string line,string name,ref double variable)
{
    if(line.Contains(name)){
        Double.TryParse(line.Replace(name,""),out variable);
        return true;
    }
    return false;
}

public bool ConvertVariable(string line,string name,ref string variable)
{
    if(line.Contains(name)){
        variable = line.Replace(name,"");
        return true;
    }
    return false;
}

public bool ConvertVariable(string line,string name,ref Vector3D variable)
{
    if(line.Contains(name)){
        Vector3D.TryParse(line.Replace(name,""),out variable);
        return true;
    }
    return false;
}