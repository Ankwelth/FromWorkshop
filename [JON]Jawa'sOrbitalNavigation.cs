////////////////////////////////////////////// -- Jawa's Orbital Navigation -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Script: JON
//  Version: 1.0
//  Date: 4/24/2019
//
//  START HERE
//  Simpley click run in the block control menu.
//  All data will be output into the "Custom Data" section of the block control panel.
//
//  Output:
//  The output will give you coordinates of the nearest: Surface, Orbit, and Drop Position for every planet in the map.
//  Coordinates are calculated using your ship's position or GPS coordinates given via the commands.
//  Coordinates can be copied from Custom Data with ctrl+c then used to create a GPS marker under the GPS menu tab using
//  the "New from clipboard" button.
//
//  Commands: (optional)
//  You can enter commands in two formats:
//  Format 1: GPS:GPS_NAME:X:Y:Z:Planet_Name:Padding_Distance
//  Format 2: Padding_Distance
//
//  -Format 1:
//  --GPS:
//  Entering GPS coordinates as an argument makes the program output data based on the GPS coordinates in addition
//  to the data based on your ship's coordinates.
//  The GPS coordinates are only accepted in the format used by the game when you use the "Copy to clipboard" option.
//  You can replicate this format by typing, but it is recommended to simply use the "Copy to clipboard" button in the GPS tab.
//
//  --Planet_Name: (optional)
//  This argument is optional.  This allows you to specify the planet your GPS coordinates are related to.
//  The name is case sensitive.  The default planet names are lowercase for this script.  If you add your own planets
//  it is recommended to enter their names in lowercase letters as well. 
//  If you do not enter this argument the program will calculate which planet is closest to the coordinates you entered
//  and will use that planet's data for calculations.
//
//  --Padding_Distance: (optional)
//  When using Format 1, this argument can only be used after the Planet_Name argument.
//  This argument allows you to change the distance between the orbit coordinates and the edge of gravity for the planet.
//  The default is 500m.  This means that if you fly or jump to the orbit coordinates you will be 500 meters outside of the range
//  of the planet's gravity.
//
//  -Format 2:
//  This format is used when you only want to change the orbit distance without adding GPS coordinates.
//  This argument allows you to change the distance between the orbit coordinates and the edge of gravity for the planet.
//  The default is 500m.  This means that if you fly or jump to the orbit coordinates you will be 500 meters outside of the range
//  of the planet's gravity.
//
//  Special thanks to Freak_Gene who posted all the planet info on the Steam General Discussions board for SE.
//  His data for Titan and Europa was switched, but don't tell anyone. ;)
//
//
//  The following section allows you to modify constants for this script and add or remove planets for custom worlds.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////// -- SETUP -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


////////////////////////////////////////// -- Planet Constants -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//  If you have a custom world you will need to modify the planet constants below to match your world.
//  Copy the planet template below into the "ADD NEW PLANETS HERE" section to add in new planets.
//  Replace Planetname, X,Y,Z, PLANETRADIUS, and PLANETGRAVDIST with the data of your new planet.
//  Note: PLANETGRAVDIST is the maximum distance from the center of the planet at which the gravity of the planet
//  will affect an object.
//  You can remove default planets in the "Default Planets Here" section
//
//  Planet Template:
//
//  Planets.Add(new Planet("PLANETNAME", new Vector3D(X,Y,Z), PLANETRADIUS, PLANETGRAVDIST));
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

List<Planet> Planets = new List<Planet>();
public void Constants(ref Double OrbitPaddingDist, ref Double DropPaddingDist)
{
Planets.Clear();

// ------------------------------------------------------------- ADD NEW PLANETS HERE --------------------------------------------------------- \\


// ----------------------------------------------------------------------------------------------------------------------------------------------------------- \\

// ------------------------------------------------- Change Constants Between These Lines --------------------------------------------------- \\
// You can modify the values below to change how close to the planet the GPS markers for orbit and drop are planced.
OrbitPaddingDist = 500;     // This is the distance between the jump point and the edge of each planet's gravitational influence.
DropPaddingDist = -200;    // This is the distance below the edge of each planets gravitational influence.
// ------------------------------------------------------------------------------------------------------------------------------------------------------------ \\


// ---------------------------------------------------------------- Default Planets Here ---------------------------------------------------------------- \\
//Earth\\
Planets.Add(new Planet("earth", new Vector3D(0,0,0), 61250, 103093.4)); //Sea level = 60000
//Moon\\
Planets.Add(new Planet("moon", new Vector3D(16384, 136384, -113616), 9500, 12314.416)); //Sea level = 9500
//Triton\\
Planets.Add(new Planet("triton", new Vector3D(-284463.5, -2434463.5, 365536.5), 40127.5, 73862.89)); //Sea level = 40127.5
//Mars\\
Planets.Add(new Planet("mars", new Vector3D(1031072, 131072, 1631072), 61500, 101553.3)); //Sea level = 60000
//Titan\\
Planets.Add(new Planet("europa", new Vector3D(916384, 16384, 1616384), 9600, 12673.088)); //Sea level = 9500
//Alien\\
Planets.Add(new Planet("alien", new Vector3D(131072, 131072, 5731072), 60000, 104506.7)); //Sea level = 60000
//Europa\\
Planets.Add(new Planet("titan", new Vector3D(36384, 226384, 5796384), 9500, 12314.416)); //Sea level = 9500
//Pertam\\
Planets.Add(new Planet("pertam", new Vector3D(-3967231.5,-32231.5,-767231.5), 30000, 48500));
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






/////////////////////////////////////////  DO NOT EDIT BELOW THIS LINE  \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////// -- Functions -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

public Vector3D MyPosition()
{
        IMyProgrammableBlock ProgramBlock = Me;
        Vector3D MyPos = ProgramBlock.GetPosition();
        return MyPos;
}

Vector3D UVector(Vector3D Vector)
{
    Vector3D UVector;
    double AbsVector = 1;
    AbsVector = Math.Sqrt((Vector.X*Vector.X) + (Vector.Y*Vector.Y) + (Vector.Z*Vector.Z));
    if (AbsVector == 0) AbsVector = 1;
    UVector = Vector/AbsVector;
    return UVector;
}

Vector3D PlanetNearGPS(Vector3D PlanetGPS, Double Radius, Double Padding = 0)
{
    Vector3D NearSurfGPS = PlanetGPS - (Radius+Padding)*UVector(PlanetGPS - MyPosition()) ;
    return NearSurfGPS;
}

Vector3D PlanetGPSPositions(Vector3D PlanetGPS, Vector3D PositionGPS, Double Radius, Double Padding = 0)
{
    Vector3D GPSOrbit = PlanetGPS - (Radius+Padding)*UVector(PlanetGPS - PositionGPS) ;
    return GPSOrbit;
}


string FormatString(Vector3D GPS, string GPSName)
{
    string gps = "GPS:" + GPSName + ":" + Math.Round(GPS.X,2) + ":" + Math.Round(GPS.Y,2) + ":" + Math.Round(GPS.Z,2) + ":";
    return gps;
}

Vector3D FormatVector(string GPS)
{
    try
    {
        string[] GPSSplit = GPS.Split(':');
        if (GPSSplit.Length  == 5 || GPSSplit.Length  == 6 || GPSSplit.Length  == 7)
        {
            Double GPSX = Convert.ToDouble(GPSSplit[2]);
            Double GPSY = Convert.ToDouble(GPSSplit[3]);
            Double GPSZ = Convert.ToDouble(GPSSplit[4]);
            Vector3D VectorGPS = new Vector3D(GPSX,GPSY,GPSZ);
            return VectorGPS;
        }
        else
        {
            Echo ("ERROR: Argument must be in the format of: \nGPS:Name:X:Y:Z:OptionalPlanet\n");
            return new Vector3D();
        }
    }
    catch
    {
        Echo ("ERROR: Argument must be in the format of: \nGPS:Name:X:Y:Z:OptionalPlanet\n");
        return new Vector3D();
    }
}

Double DistCheck(Vector3D Planet, Vector3D GPS)
{
    Vector3D DistVector = Planet - GPS;
    Double Distance = Math.Sqrt((DistVector.X*DistVector.X) + (DistVector.Y*DistVector.Y) + (DistVector.Z*DistVector.Z));
    return Distance;
}

int ClosestPlanet(Vector3D GPS)
{
    double MinDist  = Int32.MaxValue;
    int MinDistElement = 0;
    for (int i=0; i<Planets.Count; ++i)
    {
        Vector3D DistVector = Planets[i].InternalPlanetPos - GPS;
        Double Distance = Math.Sqrt((DistVector.X*DistVector.X) + (DistVector.Y*DistVector.Y) + (DistVector.Z*DistVector.Z));
        if(Distance < MinDist)
        {
            MinDist  = Distance;
            MinDistElement = i;
        }
    }
    return MinDistElement;
}

/////////////////////////////////////// -- Classes -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
public class Planet
{
    public String InternalPlanetName;
    public Vector3D InternalPlanetPos;
    public double InternalPlanetRadius;
    public double InternalPlanetGravDist;

    public Planet(String InternalPlanetName, Vector3D InternalPlanetPos, double InternalPlanetRadius, double InternalPlanetGravDist)
    {
        this.InternalPlanetName = InternalPlanetName;
        this.InternalPlanetPos = InternalPlanetPos;
        this.InternalPlanetRadius = InternalPlanetRadius;
        this.InternalPlanetGravDist = InternalPlanetGravDist;
    }
}

///////////////////////////////////////////////////////// ++ Main ++ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
public void Main(string GPS)
{

//////////////////////////////// -- Variable Initialization -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
double OrbitPaddingDist = 0;
double DropPaddingDist = 0;

Constants(ref OrbitPaddingDist, ref DropPaddingDist);

//--Distances--\\
double PlanetRadius = 0;
double PlanetGravDist = 0;
double PlanetDist = 0;

//--Vectors/Points--\\
Vector3D GPSPosition = new Vector3D(0,0,0);
Vector3D PlanetPos = new Vector3D(0,0,0);

int RefPlanet = 0;
bool PlanetNameGiven = false;
bool GPSGiven = false;


Vector3D OutputNearSurf = new Vector3D(0,0,0);
Vector3D OutputNearOrbit = new Vector3D(0,0,0);
Vector3D OutputNearDrop = new Vector3D(0,0,0);

string OutputText = " ";
OutputText = "";

string[] GPSTest = GPS.Split(':');

///////////////////////////////////// -- Input Parse -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
if (GPSTest[0] == "GPS")
{
    GPSPosition = FormatVector(GPS);
    GPSGiven = true;
}
else
{
    bool TryParseReturn = Double.TryParse(GPS, out OrbitPaddingDist);
    if (TryParseReturn == false) OrbitPaddingDist = 500;
    Echo("Orbital Padding Distance: " + OrbitPaddingDist + "m\n");
}

string PlanetName = "NA";
if (GPSGiven == true)
{
    bool TryParseReturn = false;
    String[] SplitArg = GPS.Split(':');
    if (SplitArg.Length >= 6) PlanetName = SplitArg[5];
    if (SplitArg.Length == 7) TryParseReturn = Double.TryParse(SplitArg[6], out OrbitPaddingDist);
    if (TryParseReturn == false) OrbitPaddingDist = 500;
    Echo("Orbital Padding Distance: " + OrbitPaddingDist + "m\n");
}

///////////////////////////////////// -- Calculations -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

for (int i=0; i<Planets.Count; ++i)
{
    if (PlanetName == Planets[i].InternalPlanetName)
    {
        PlanetNameGiven = true;
        RefPlanet = i;
        break;
    }

}
if (PlanetNameGiven == false) RefPlanet = ClosestPlanet(GPSPosition);

PlanetDist = DistCheck(Planets[RefPlanet].InternalPlanetPos, GPSPosition);
PlanetName = Planets[RefPlanet].InternalPlanetName;
PlanetPos = Planets[RefPlanet].InternalPlanetPos;
PlanetRadius = Planets[RefPlanet].InternalPlanetRadius;
PlanetGravDist = Planets[RefPlanet].InternalPlanetGravDist;

Vector3D GPSSurf = PlanetGPSPositions(PlanetPos, GPSPosition, PlanetRadius);
Vector3D GPSOrbit = PlanetGPSPositions(PlanetPos, GPSPosition, PlanetGravDist, OrbitPaddingDist);
Vector3D GPSDrop = PlanetGPSPositions(PlanetPos, GPSPosition, PlanetGravDist, DropPaddingDist);

if (GPSGiven == true)
{
	OutputText = (OutputText + 
    "------------------------- Custom GPS: " + PlanetName + " -------------------------" + "\n");
    OutputText = OutputText + (FormatString(GPSOrbit, "GPSOrbit") + "\n");
    OutputText = OutputText + (FormatString(GPSDrop, "GPSDrop") + "\n");
    OutputText = OutputText + (FormatString(GPSSurf, "GPSSurf") + "\n");
}

for (int i=0; i<Planets.Count; ++i)
{
    OutputNearSurf = PlanetNearGPS(Planets[i].InternalPlanetPos, Planets[i].InternalPlanetRadius);
    OutputNearOrbit = PlanetNearGPS(Planets[i].InternalPlanetPos, Planets[i].InternalPlanetGravDist, OrbitPaddingDist);
    OutputNearDrop = PlanetNearGPS(Planets[i].InternalPlanetPos, Planets[i].InternalPlanetGravDist, DropPaddingDist);
    OutputText = (OutputText + 
    "-------------------------------- " + Planets[i].InternalPlanetName + " --------------------------------" + "\n" +
    FormatString(OutputNearSurf, Planets[i].InternalPlanetName + "Near" + "Surf") + "\n" + 
    FormatString(OutputNearOrbit, Planets[i].InternalPlanetName + "Near" + "Orbit") + "\n" + 
    FormatString(OutputNearDrop, Planets[i].InternalPlanetName + "Near" + "Drop") + "\n"
    );
}

//////////////////////////////////////// -- Output -- \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

try
{
IMyProgrammableBlock ProgramBlock = Me;
ProgramBlock.CustomData = OutputText;
}
catch {};

if (GPSGiven == true) Echo ("Target Planet: " + PlanetName + "\n");
Vector3D MyPos = MyPosition();
Echo("Current Position: \nX:" + Math.Round(MyPos.X,2) + "\nY:" + Math.Round(MyPos.Y,2) + "\nZ:" + Math.Round(MyPos.Z,2) + "\n");
Echo ("Command Completed" + "\n");
Echo ("Data output into Custom Data");

}