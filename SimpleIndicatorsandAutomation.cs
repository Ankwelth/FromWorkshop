/*
Author: GruntBlender
Last Revision: 18/11/23

Script for simple automation and indicators.
Instructions:
  Naming:
    -To monitor the status of a block, add "Monitored" anywhere 
        in its name. Edit MonitorTag to adjust this.
    -To display status of a block on a light, add "Indicator" to 
        the light name, IndicatorTag controls this.
    -To control a block, add "Controlled" to its name, this tag is 
        adjusted with the ControlTag variable.
    -You can edit the ErrorDisplay variable to the name of a panel 
        you with the script to print status information on.
        
  Channels:
    -A channel is just a word added to the Custom Data box of a block, using 
        different words allows different indicators to monitor separate blocks.
    -Monitored blocks only need the channel word in the Custom Data box.
    -Indicator lights need three words; first determines what to monitor on 
        target block, second determines how the information is shown, third is
        the channel word that determines what block is monitored.
        E.G., a light might have "WORK COL Reactor1" in Custom data, this will
        change the color of the light based on whether the reactor is on/off.
    -Controlled blocks need three words; first determines what to monitor on 
        target block, second determines what to do with controlled block, third
        is the channel word that determines what block is monitored.
    
  Valid monitor words:
    WORK (Reports whether the block is powered), 
    STATUS (Reports status of the block, like connector locking or door opening),
	VALUE (Reports a continuous value that reflects things like a piston's extension
			or air pressure reported by an air vent),
	MASS (Used on control blocks like cockpits to get the mass of the ship)
    
  Valid indicator/control words:
    ON(Turns light on when condition is met), 
    COL(Changes color based on condition, OnColor when condition met, 
      OffColor when opposite, WarnColor in the middle, as in when room is
      pressurizing or door is opening/closing)
    OPEN (Opens the door when report is positive, closes when negative)
    LOCK (Locks the connector or landing gear when report is positive, 
          unlocks when negative or middle)
	EXTEND (Extends a piston when report is positive, retracts otherwise)
  
    
*/




    //SETTINGS:
    string ErrorDisplay = "LCD Panel SIA Status";
    string MonitorTag = "Monitored";
    string IndicatorTag = "Indicator";
    string ControlTag = "Controlled";
    int[] OnColor = {127, 255, 64};
    int[] OffColor = {255, 32, 16};
    int[] WarnColor = {255, 127, 0};
    int[] ErrColor = {255, 0, 127};
    



public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

void Main(string arg)
{
    //Script follows.
    string Status = "";
	float Value = 0f;
    // List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
    // List<IMyTerminalBlock> Monitored = new List<IMyTerminalBlock>();
    // List<IMyTerminalBlock> Controlled = new List<IMyTerminalBlock>();
    // GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Blocks);
	
    List<IMyTerminalBlock> Monitored = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> Controlled = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> Indicators = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(MonitorTag, Monitored);
    GridTerminalSystem.SearchBlocksOfName(ControlTag, Controlled);
    GridTerminalSystem.SearchBlocksOfName(IndicatorTag, Indicators, Ind => Ind is IMyLightingBlock);
    if (Monitored.Count == 0)Status += "No blocks with \"" + MonitorTag + "\" in the name\n";
    else Status += "Found " + Monitored.Count + " monitored block(s)\n";
    Status += "Found " + Controlled.Count + " controled block(s)\n";
    Status += "Found " + Indicators.Count + " indicator light(s)\n";
    Controlled.AddRange(Indicators);

    string[] Command;
    int State = 0;
    
    
//Do controlled blocks.

    
    for (int i = 0; i < Controlled.Count; i++)
    {
        
        //Parse CustomData
        
        Command = Controlled[i].CustomData.Split(' ');
        if (Command.Length < 3)
        {
            Status += "E: Block \"" + Controlled[i].CustomName + "\" bad config\n";
            continue;
        }
        
        //Find target block
        int m=0;
        for ( ; m<Monitored.Count; m++) if (Monitored[m].CustomData.Equals(Command[2]))break;
        
        if (m>=Monitored.Count)
        {
            Status += "No block found on channel \"" + Command[2] + "\" \n    for \"" + Controlled[i].CustomName + "\"\n";
            continue;
        }
        State=0;
        switch (Command[0])
        {
            case "WORK":
                //Check the block is powered
                if (Monitored[m].IsWorking) State=3;
                else State = 1;
                break;
            case "OXY":
                Status += "OXY is obsolete, use VALUE instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("OXY ", "VALUE d");
                continue;
            case "VAC":
                Status += "VAC is obsolete, use VALUE instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("VAC ", "VALUE !d");
                continue;
            case "OPEN":
                Status += "OPEN is obsolete, use STATUS instead\n";
				Controlled[i].CustomData="STATUS"+Controlled[i].CustomData.Substring(4);
                continue;
            case "LOCK":
                Status += "LOCK is obsolete, use STATUS instead\n";
				Controlled[i].CustomData="STATUS"+Controlled[i].CustomData.Substring(4);
                continue;
            case "MOVE":
                Status += "MOVE is obsolete, use STATUS instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("MOVE ", "STATUS i");
                continue;
            case "MASS":
                if(Monitored[m] is IMyShipController)
                {
                    float Mass = ((IMyShipController)Monitored[m]).CalculateShipMass().TotalMass;
                    int Limit = int.Parse(Command[3].Replace(",", ""));
                    if (Mass > Limit)State=3;
                    else State=1;
                }
                else
                {
                    Status += "MASS is not supported on \"" + Monitored[m].CustomName + "\"\n";
                    continue;
                }
                break;
		    case "STATUS":
			    if(Monitored[m] is IMyAirVent)
                {
                    if (((IMyAirVent)Monitored[m]).GetOxygenLevel()>=0.9)State=3; 
                    else if (((IMyAirVent)Monitored[m]).GetOxygenLevel()>=0.1)State=2;
                    else State = 1;
                }
				else if(Monitored[m] is IMyShipConnector)
                {
                    if (((IMyShipConnector)Monitored[m]).Status==MyShipConnectorStatus.Connected)State=3;
                    else if (((IMyShipConnector)Monitored[m]).Status==MyShipConnectorStatus.Connectable)State=2;
                    else State=1;
                }
                else if(Monitored[m] is IMyLandingGear)
                {
                    if (((IMyLandingGear)Monitored[m]).LockMode==LandingGearMode.Locked)State=3;
                    else if (((IMyLandingGear)Monitored[m]).LockMode==LandingGearMode.ReadyToLock)State=2;
                    else State=1;
                }
				else if(Monitored[m] is IMyDoor)
                {
                    if (((IMyDoor)Monitored[m]).OpenRatio >0.95)State=3;
                    else if (((IMyDoor)Monitored[m]).OpenRatio <0.05)State=1;
                    else State=2;
                }
				else if(Monitored[m] is IMyBatteryBlock)
                {
                    if (((IMyBatteryBlock)Monitored[m]).ChargeMode==ChargeMode.Recharge)State=3;
                    else if (((IMyBatteryBlock)Monitored[m]).ChargeMode==ChargeMode.Discharge)State=1;
                    else State=2;
                }
				else if(Monitored[m] is IMyProductionBlock)
                {
					if(Monitored[m].IsWorking){State=3;if (((IMyProductionBlock)Monitored[m]).IsProducing)State=2;}
					else State=1;
                }
				else if(Monitored[m] is IMyGasGenerator)
                {
					if(Monitored[m].IsWorking){State=3;if (((IMyGasGenerator)Monitored[m]).GetInventory().CurrentVolume>1)State=2;}
					else State=1;
                }
				else if(Monitored[m] is IMyPistonBase)
                {
                    if (((IMyPistonBase)Monitored[m]).Status==PistonStatus.Extended)State=3;
                    else if (((IMyPistonBase)Monitored[m]).Status==PistonStatus.Retracted)State=1;
                    else State=2;
                }
				else
				{
				    Status += "STATUS is not currently supported for \"" + Monitored[m].GetType().ToString()+"\"\n";
					State=-1;
				}
			    break;
		    case "VALUE":
			    Value = GetValue(Monitored[m]);
				if(Value<-0.5){
					Status += "VALUE is not currently supported for \"" + Monitored[m].GetType().ToString()+"\"\n";
					State=-1;
					break;;
				}
				State=4;
			    break;
            default:
                //Unknown command
                Status += "Unknown command \"" + Command[0] + "\" on block \"" + Controlled[i] + "\"\n";
                continue;
        }
		
		bool Exclusive = false;
		bool Interim = false;
		bool Inverted = false;
		bool Broad = false;
		
		while(Command[1].Length>1)
		{
			if(Command[1].StartsWith("!"))
			{
				Inverted = true;
				Command[1]=Command[1].Substring(1);
			}
			else if(Command[1].StartsWith("x"))
			{
				Exclusive = true;;
				Command[1]=Command[1].Substring(1);
			}
			else if(Command[1].StartsWith("i"))
			{
				Interim = true;;
				Command[1]=Command[1].Substring(1);
			}
			else if(Command[1].StartsWith("b"))
			{
				Broad = true;;
				Command[1]=Command[1].Substring(1);
			}
			else if(Command[1].StartsWith("d"))
			{
				if(Value<0.05)State=1;
				else if(Value>0.95)State=3;
				else State=2;
				Command[1]=Command[1].Substring(1);
			}
			else break;
		}
		
		if(Exclusive&&State==2)break;
		if(Interim){if(State==2)State=3;else State=1;}
		if(Inverted)State = 4 - State;
		if(Broad&&State==2)State=3;
        switch (Command[1])
        {
            case "ON":
				if(!(Controlled[i] is IMyFunctionalBlock)){Status+="ON/OFF unavailable on \""+Controlled[i].CustomName+"\"\n";break;}
                if (State==3) ((IMyFunctionalBlock)Controlled[i]).Enabled=true;
                else ((IMyFunctionalBlock)Controlled[i]).Enabled=false;
                break;
            case "OFF":
				Status+="OFF is obsolete, use !bON instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("OFF", "!bON");
                break;
            case "COL":
				if(!(Controlled[i] is IMyLightingBlock)){Status+="COL unavailable on \""+Controlled[i].CustomName+"\"\n";break;}
                if (State == 3)((IMyLightingBlock)Controlled[i]).Color = GetCol(OnColor);
                else if (State == 2)((IMyLightingBlock)Controlled[i]).Color = GetCol(WarnColor);
                else if (State == 1) ((IMyLightingBlock)Controlled[i]).Color = GetCol(OffColor);
                else if (State == 4) ((IMyLightingBlock)Controlled[i]).Color = GetCol(Value);
                else if (State == 0) ((IMyLightingBlock)Controlled[i]).Color = GetCol(1-Value);
				else ((IMyLightingBlock)Controlled[i]).Color = GetCol(ErrColor);
                break;
            case "ICOL":
				Status+="ICOL is obsolete, use !COL instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("ICOL", "!COL");
                break;
            case "OPEN":
                if(Controlled[i] is IMyDoor)
                {
                    if (State==3) ((IMyDoor)Controlled[i]).OpenDoor();
                    else ((IMyDoor)Controlled[i]).CloseDoor();
                }
				else Status += "\""+Controlled[i].CustomName+"\" is not a door\n";
                break;
            case "CLOSE":
				Status+="CLOSE is obsolete, use !bOPEN instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("CLOSE", "!bOPEN");
                break;
            case "LOCK":
                if(Controlled[i] is IMyShipConnector)
                {
                    if (State==3) ((IMyShipConnector)Controlled[i]).Connect();
                    else ((IMyShipConnector)Controlled[i]).Disconnect();
                }
                else if(Controlled[i] is IMyLandingGear)
                {
                    if (State==3) ((IMyLandingGear)Controlled[i]).Lock();
                    else ((IMyLandingGear)Controlled[i]).Unlock();
                }
				else Status += "\""+Controlled[i].CustomName+"\" is not a Connector or Landing Gear\n";
                break;
            case "UNLOCK":
				Status+="UNLOCK is obsolete, use !bLOCK instead\n";
				Controlled[i].CustomData=Controlled[i].CustomData.Replace("UNLOCK", "!bLOCK");
                break;
            case "EXTEND":
                if(Controlled[i] is IMyPistonBase)
                {
                    if (State==3) ((IMyPistonBase)Controlled[i]).Extend();
                    else ((IMyPistonBase)Controlled[i]).Retract();
                }
				else Status += "\""+Controlled[i].CustomName+"\" is not a piston\n";
                break;
            case "CHARGE":
                if(Controlled[i] is IMyBatteryBlock)
                {
                    if (State==3) ((IMyBatteryBlock)Controlled[i]).ChargeMode=ChargeMode.Recharge;
                    else ((IMyBatteryBlock)Controlled[i]).ChargeMode=ChargeMode.Auto;
                }
                else if(Controlled[i] is IMyGasTank)
                {
                    if (State==3) ((IMyGasTank)Controlled[i]).Stockpile=true;
                    else ((IMyGasTank)Controlled[i]).Stockpile=false;
                }
				else Status += "\""+Controlled[i].CustomName+"\" is not a Tank or Battery\n";
                break;
            case "TEXT":
                if(Controlled[i] is IMyRadioAntenna)
                {
                    if (State==4) ((IMyRadioAntenna)Controlled[i]).HudText=Convert.ToString(Value*100)+'%';
                    else Status += "TEXT currently only works with percentages";
                }
				else Status += "\""+Controlled[i].CustomName+"\" can't display text\n";
                break;
            default:
                //Unknown command
                Status += "Unknown command \"" + Command[1] + "\" on block \"" + Controlled[i] + "\"\n";
                continue;
        }
    }


//Display Status

    if (ErrorDisplay != "")
    {
        IMyTextPanel Display = GridTerminalSystem.GetBlockWithName(ErrorDisplay) as IMyTextPanel;
        if (Display != null)Display.WriteText(Status, false);
    }
    Echo("Status:\n" + Status);
}

//Helper methods
Color GetCol(int[] Arg)
{
    return new Color(Arg[0], Arg[1], Arg[2]);
}
Color GetCol(float Ratio)
{
	return new Color((int)(OnColor[0]*Ratio+OffColor[0]*(1-Ratio)), (int)(OnColor[1]*Ratio+OffColor[1]*(1-Ratio)), (int)(OnColor[2]*Ratio+OffColor[2]*(1-Ratio)));
}
float GetValue(IMyTerminalBlock Block)
{
	if(Block is IMyAirVent)
		return ((IMyAirVent)Block).GetOxygenLevel();
	else if(Block is IMyBatteryBlock)
		return ((IMyBatteryBlock)Block).CurrentStoredPower/((IMyBatteryBlock)Block).MaxStoredPower;
	else if(Block is IMyDoor)
		return ((IMyDoor)Block).OpenRatio;
	else if(Block is IMyGasTank)
		return (float)((IMyGasTank)Block).FilledRatio;
	else if(Block is IMyCargoContainer)
		return 1f-(float)((IMyCargoContainer)Block).GetInventory().CurrentVolume
			/(int)(((IMyCargoContainer)Block).GetInventory().MaxVolume);
	else if(Block is IMyPistonBase)
		return
			(((IMyPistonBase)Block).CurrentPosition - ((IMyPistonBase)Block).MinLimit)/
			(((IMyPistonBase)Block).MaxLimit-((IMyPistonBase)Block).MinLimit);
	return -1f;
}