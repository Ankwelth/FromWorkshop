//Use:
//Run the script to get your maximum vertical acceleration in the PB's details output.
//Set the Panel variable to name of LCD panel if you want to output the info on that instead.

//Settings:
bool Autorun = false;  //Set to true if you want the script to run automatically every 1.5s or so.
string Panel = ""; //Set this to the name of an LCD panel to route output to, if you want.

//Script (do not edit this):

public Program() 
{ 
    if(Autorun)Runtime.UpdateFrequency = UpdateFrequency.Update100;
} 
 
public void Main(String args) 
{

    List<IMyShipController> Controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType<IMyShipController>(Controllers, block => block.IsSameConstructAs(Me));

    List<IMyThrust> Thrusters = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(Thrusters, block => block.IsSameConstructAs(Me));
    
    string Output = "";
    
    float Mass = Controllers[0].CalculateShipMass().TotalMass;

    Single Ions = 0;
    Single Hydro = 0;
    Single Atmo = 0;
    Single Other = 0;

    for(int i=0; i<Thrusters.Count; i++)
    {
            if(Thrusters[i].Orientation.Forward==Base6Directions.Direction.Down)
            {
                    if(Thrusters[i].DefinitionDisplayNameText.Contains("Ion Thrust"))
                        Ions += Thrusters[i].MaxThrust;
                    else if(Thrusters[i].DefinitionDisplayNameText.Contains("Hydrogen Thrust"))
                        Hydro += Thrusters[i].MaxThrust;
                    else if(Thrusters[i].DefinitionDisplayNameText.Contains("Atmospheric Thrust"))
                        Atmo += Thrusters[i].MaxThrust;
                    else Other += Thrusters[i].MaxThrust;

            }
    }
    Single Total = Ions+Hydro+Atmo+Other;
    Output += "Current Mass: "+Mass.ToString("0.0")+"kg\n";
    if(Ions>0)
    {
        string IonThrust = "";
        if(Ions<1000)IonThrust = Ions.ToString("N1");
        else if(Ions<1000000)IonThrust = (Ions/1000).ToString("N1")+"kN";
        else if(Ions<1000000000)IonThrust = (Ions/1000000).ToString("N1")+"MN";
        else if(Ions<1000000000000)IonThrust = (Ions/1000000000).ToString("N1")+"GN";
        Output += "Ion Thrust: "+ IonThrust +", \n    Accel: " + (Ions/Mass).ToString("N2") +"m/s\u00b2 ("+(Ions/Mass/9.81).ToString("N2")+"g)\n";
    }
    if(Atmo>0)
    {
        string AtmoThrust = "";
        if(Atmo<1000)AtmoThrust = Atmo.ToString("N1");
        else if(Atmo<1000000) AtmoThrust = (Atmo/1000).ToString("N1")+"kN";
        else if(Atmo<1000000000) AtmoThrust = (Atmo/1000000).ToString("N1")+"MN";
        else if(Atmo<1000000000000) AtmoThrust = (Atmo/1000000000).ToString("N1")+"GN";
        Output += "Atmospheric Thrust: "+ AtmoThrust +", \n    Accel: " + (Atmo/Mass).ToString("N2") +"m/s\u00b2 ("+(Atmo/Mass/9.81).ToString("N2")+"g)\n";
    }
    if(Hydro>0)
    {
        string HydroThrust = "";
        if(Hydro<1000)HydroThrust = Hydro.ToString("N1");
        else if(Hydro<1000000)HydroThrust = (Hydro/1000).ToString("N1")+"kN";
        else if(Hydro<1000000000) HydroThrust = (Hydro/1000000).ToString("N1")+"MN";
        else if(Hydro<1000000000000) HydroThrust = (Hydro/1000000000).ToString("N1")+"GN";
        Output += "Hydrogen Thrust: "+ HydroThrust +", \n    Accel: " + (Hydro/Mass).ToString("N2") +"m/s\u00b2 ("+(Hydro/Mass/9.81).ToString("N2")+"g)\n";
    }
    if(Other>0)
    {
        string OtherThrust = "";
        if(Other<1000)OtherThrust = Other.ToString("N1");
        else if(Other<1000000) OtherThrust = (Other/1000).ToString("N1")+"kN";
        else if(Other<1000000000) OtherThrust = (Other/1000000).ToString("N1")+"MN";
        else if(Other<1000000000000) OtherThrust = (Other/1000000000).ToString("N1")+"GN";
        Output += "Other Thrust: "+ OtherThrust +", \n    Accel: " + (Other/Mass).ToString("N2") +"m/s\u00b2 ("+(Other/Mass/9.81).ToString("N2")+"g)\n";
    }
    string TotalThrust = "";
    if(Total<1000)TotalThrust = Total.ToString("N1");
    else if(Total<1000000) TotalThrust = (Total/1000).ToString("N1")+"kN";
    else if(Total<1000000000) TotalThrust = (Total/1000000).ToString("N1")+"MN";
    else if(Total<1000000000000) TotalThrust = (Total/1000000000).ToString("N1")+"GN";
    Output += "\nMax Total Thrust: "+ TotalThrust +", \n";
    Output += "Max Acceleration: "+  (Total/Mass).ToString("N2") +"m/s\u00b2 ("+(Total/Mass/9.81).ToString("N2")+"g)\n";
    if(Panel!="")
    {
        IMyTextPanel LCD = GridTerminalSystem.GetBlockWithName(Panel) as IMyTextPanel;
        if(LCD==null)Echo("LCD Panlel \""+Panel+"\" not found. Ouputting here:\n"+Output);
        else
        {
            LCD.ContentType=ContentType.TEXT_AND_IMAGE;
            LCD.WriteText(Output, false);
        }
    }
    else Echo(Output);
} 
