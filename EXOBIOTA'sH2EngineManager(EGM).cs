//Exo's hydrogen engine manager

float currentSum;
float maxSum;
string config;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    if (Me.CustomData == ""){
            Me.CustomData = "ENGINES ON: 25%, ENGINES OFF: 90%";
    }
}

string getDetailedInfoValue(IMyTerminalBlock block, string name)    
{   
    string value = "";   
    string[] lines = block.DetailedInfo.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);   
    for (int i = 0; i < lines.Length; i++)    
    {   
        string[] line = lines[i].Split(':');   
        if (line[0].Equals(name))    
        {   
            value = line[1].Substring(1);   
            break;   
        }   
    }   
    return value;   
}

int getPowerAsInt(string text)    
{   
    if (String.IsNullOrWhiteSpace(text))    
    {   
        return 0;   
    }   
    string[] values = text.Split(' ');   
    if (values[1].Equals("kW"))    
    {   
        return (int) (float.Parse(values[0])*1000f);   
    }   
    else if (values[1].Equals("kWh"))    
    {    
        return (int) (float.Parse(values[0])*1000f);    
    }   
    else if (values[1].Equals("MW"))    
    {   
        return (int) (float.Parse(values[0])*1000000f);   
    }   
    else if (values[1].Equals("MWh"))    
    {    
        return (int) (float.Parse(values[0])*1000000f);    
    }   
    else    
    {   
        return (int) float.Parse(values[0]);   
    }   
} 

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & UpdateType.Update100) != 0)
    {
        if (Me.CustomData == ""){
            Me.CustomData = "GENERATORS ON: 25%, GENERATORS OFF: 90%";
        }
        List<IMyTerminalBlock> batteries = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> generatorstemp = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlockGroupWithName("EGM").GetBlocksOfType<IMyBatteryBlock>(batteries);
        GridTerminalSystem.GetBlockGroupWithName("EGM").GetBlocksOfType<IMyPowerProducer>(generatorstemp);
        foreach (IMyPowerProducer generator in generatorstemp) {
             if (generator.DefinitionDisplayNameText.Contains("Hydro") ){
                    generators.Add(generator);
                }
        }

        config = System.Text.RegularExpressions.Regex.Replace(Me.CustomData, "[^0-9,]", "");
        currentSum = 0.0f;
        maxSum = 0.0f;
        foreach (IMyBatteryBlock battery in batteries)
        {
             maxSum += getPowerAsInt(getDetailedInfoValue(battery, "Max Stored Power"));
             currentSum += getPowerAsInt(getDetailedInfoValue(battery, "Stored power"));
        }
        if ((currentSum / maxSum) * 100.0f <= Convert.ToInt32(config.Split(',')[0])) //HERE
        {
            foreach(IMyPowerProducer generator in generators)
            {
                if (generator.DefinitionDisplayNameText.Contains("Hydro")){
                    generator.Enabled = true;
                }
            }
        } else if ((currentSum / maxSum) * 100.0f >= Convert.ToInt32(config.Split(',')[1]))
        {
            foreach (IMyPowerProducer generator in generators)
            {
                if (generator.DefinitionDisplayNameText.Contains("Hydro")){
                    generator.Enabled = false;
                }
            }
        }
        Echo("Tracking: "+batteries.Count() + " Batteries, " + generators.Count() + " Engines.");
        Echo("Current combined charge: "+ (currentSum/1000000).ToString()+" MWh");
        Echo("Maximum combined charge: " + (maxSum/1000000).ToString()+ " MWh");
        Echo("Batteries "+((currentSum/maxSum)*100.0f).ToString() +"% Full");
        Echo("Start engines at: "+ config.Split(',')[0]+"% charge");
        Echo("Stop engines at: "+ config.Split(',')[1]+"% charge");
        Echo("See Custom Data to edit threshold values.");
        Echo("Place all batteries and h2 engines into group named EGM");
    }
}