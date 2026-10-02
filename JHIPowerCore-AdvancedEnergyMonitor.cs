string Lang="DE";

public Program()
{
    Runtime.UpdateFrequency=UpdateFrequency.Update100;
    if(!string.IsNullOrWhiteSpace(Storage)) Lang=Storage;
}

public void Save(){Storage=Lang;}

public void Main(string argument, UpdateType updateSource)
{
    if(argument=="DE"||argument=="EN") Lang=argument;

    var lcds=new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(lcds,x=>x.CustomName.Contains("[POWER]"));

    if(lcds.Count==0)
    {
        Echo("LCD [POWER] fehlt");
        return;
    }

    var bats=new List<IMyBatteryBlock>();
    var reactors=new List<IMyReactor>();
    var solars=new List<IMySolarPanel>();
    var winds=new List<IMyWindTurbine>();
    var tanks=new List<IMyGasTank>();

    GridTerminalSystem.GetBlocksOfType(bats);
    GridTerminalSystem.GetBlocksOfType(reactors);
    GridTerminalSystem.GetBlocksOfType(solars);
    GridTerminalSystem.GetBlocksOfType(winds);
    GridTerminalSystem.GetBlocksOfType(tanks);

    double stored=0,max=0;

    foreach(var b in bats)
    {
        stored+=(double)b.CurrentStoredPower;
        max+=(double)b.MaxStoredPower;
    }

    double bat=max>0?stored/max*100:0;

    float solar=0,wind=0,reactor=0;

    foreach(var s in solars) solar+=s.CurrentOutput;
    foreach(var w in winds) wind+=w.CurrentOutput;
    foreach(var r in reactors) reactor+=r.CurrentOutput;

    double h2=0;

    if(tanks.Count>0)
    {
        foreach(var t in tanks)
            h2+=t.FilledRatio;

        h2=h2/tanks.Count*100;
    }

    string txt="";

    if(Lang=="DE")
    {
       txt+="=== POWER CORE ===\n";
txt+="Made by Jimmy Systems\n";
txt+="------------------\n\n";
        txt+="Batterie: "+bat.ToString("0.0")+"%\n";
        txt+=stored.ToString("0.0")+" / "+max.ToString("0.0")+" MWh\n\n";
        txt+="Solar: "+solar.ToString("0.00")+" MW\n";
        txt+="Wind: "+wind.ToString("0.00")+" MW\n";
        txt+="Reaktor: "+reactor.ToString("0.00")+" MW\n\n";
        txt+="H2: "+h2.ToString("0.0")+"%\n\n";

        if(bat<10) txt+="KRITISCH";
        else if(bat<25) txt+="WARNUNG";
        else txt+="OK";
    }
    else
    {
       txt+="=== POWER CORE ===\n";
txt+="Made by Jimmy Systems\n";
txt+="------------------\n\n";
        txt+="Battery: "+bat.ToString("0.0")+"%\n";
        txt+=stored.ToString("0.0")+" / "+max.ToString("0.0")+" MWh\n\n";
        txt+="Solar: "+solar.ToString("0.00")+" MW\n";
        txt+="Wind: "+wind.ToString("0.00")+" MW\n";
        txt+="Reactor: "+reactor.ToString("0.00")+" MW\n\n";
        txt+="H2: "+h2.ToString("0.0")+"%\n\n";

        if(bat<10) txt+="CRITICAL";
        else if(bat<25) txt+="WARNING";
        else txt+="OK";
    }

    float size=1.2f;
    int lines=txt.Split('\n').Length;

    if(lines>20) size=0.9f;
    if(lines>25) size=0.8f;
    if(lines>30) size=0.7f;

    foreach(var lcd in lcds)
    {
        lcd.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        lcd.FontSize=size;
        lcd.WriteText(txt);
    }

    Echo("POWER CORE");
}