/*
Communications System v1.1
Communications system between grids
written by FoxtrotDelta

22 February 2020
UPDATED 6 May 2021

commands: freq_<frequency>, freq_default, call_<callsign>, cmd_help, cmd_clear
*/

public Program()
{
   Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
   IMyBlockGroup allTerminals = GridTerminalSystem.GetBlockGroupWithName("Comms Displays");
   List<IMyTextPanel> terminalList = new List<IMyTextPanel>();

   allTerminals.GetBlocksOfType<IMyTextPanel>(terminalList);
   
   if(terminalList.Count == 0)
   {
       return;
   }
   communicate(terminalList);
}


public void communicate(List<IMyTextPanel> terminals)
{
   string tag = "comms";
   string callsign = "user";
   IMyProgrammableBlock pb = GridTerminalSystem.GetBlockWithName("Comms Computer") as IMyProgrammableBlock;
   IMyRadioAntenna antenna = GridTerminalSystem.GetBlockWithName("Comms Array") as IMyRadioAntenna;
   string [] displayTokens;
   string transmitString = "";
   string displayText = "";
   MyIGCMessage message = new MyIGCMessage();
   List<string> lines = new List<string>();
   

   try
   {
       if(!(pb.IsFunctional))
       {
           throw new NullReferenceException();
       }
       if(!(antenna.IsFunctional))
       {
           throw new NullReferenceException();
       }
   }
   catch(NullReferenceException e)
   {
       try
       {
           Echo("NO SIGNAL");
       }
       catch(NullReferenceException n)
       {
           Echo("Antenna Destroyed");
       }

       return;
   }
   
   if(antenna.CustomData == "")
   {
       antenna.CustomData = tag + ":" + callsign;
   }
   else
   {
       string [] antennaTokens = antenna.CustomData.Split(':');

       tag = antennaTokens[0];
       callsign = antennaTokens[1];  
   }
   

   string pbData = pb.CustomData;
   string [] pbTokens = pbData.Split('\n');

   for(int i = 0; i < terminals.Count; i++)
   {
       displayText = terminals[i].GetText();
       terminals[i].CustomData = displayText;
   
       if(displayText != "")
       {
           displayTokens = displayText.Split('\n');
       
           for(int j = 0; j < displayTokens.Length; j++)
           {           
               lines.Add(displayTokens[j]);                 
           }
 
           if(pbData == "")
           {          
               transmitString = lines[(lines.Count) - 1];
               pb.CustomData += (transmitString + "\n");
               
               terminals[i].WriteText("");
               
               
           }
           else if(lines[(lines.Count) - 1] != pbTokens[(pbTokens.Length) - 1])
           {         
               transmitString = lines[(lines.Count) - 1];
               pb.CustomData += (transmitString + "\n");        
           }  
       }
   }

   if(transmitString != "")
   {
       if((transmitString.Length >= 6) && transmitString.Substring(0, 5) == "freq_")
       {
           setFrequency(transmitString.Substring(5), antenna);
       }
       else if((transmitString.Length >= 6) && transmitString.Substring(0, 5) == "call_")
       {
           setCallsign(transmitString.Substring(5), antenna);
       }
       else if(transmitString == "cmd_clear")
       {
           clear(pb, terminals);
           return;
       }
       else if(transmitString == "cmd_help")
       {
           help(terminals);
       }
       else
       {
           IGC.SendBroadcastMessage(tag, callsign + ": " + transmitString, TransmissionDistance.TransmissionDistanceMax);
       }      
       for(int i = 0; i < terminals.Count; i++)
       {
       displayText = terminals[i].GetText();
       displayTokens = displayText.Split('\n');
       
           if(displayText == "")
           {
               terminals[i].WriteText(transmitString + "\n", true);
           }
           else
           {
               if(transmitString == displayTokens[displayTokens.Length - 1])
               {
                   terminals[i].WriteText("");
                   terminals[i].WriteText(terminals[i].CustomData + "\n", true);
               }
               else
               {
                   terminals[i].WriteText(transmitString + "\n", true);
               }             
           }
       }
   }


   IMyBroadcastListener receiver = IGC.RegisterBroadcastListener(tag);
   
   if(receiver.HasPendingMessage)
   {
       message = receiver.AcceptMessage();
       
       string receiverString = message.Data.ToString();
      
       for(int i = 0; i < terminals.Count; i++)
       {
           terminals[i].WriteText("<< " + receiverString + "\n", true);
       }
       pb.CustomData += (receiverString + "\n");
  }
  
   foreach(IMyTextPanel t in terminals)
   {
       displayText = t.GetText();
       displayTokens = displayText.Split('\n');
       

       if(displayTokens.Length >= 19)
       {
           t.WriteText(displayTokens[(displayTokens.Length) - 2] + "\n");
       }
   }

   Echo("End");
}


public void setFrequency(string freq, IMyRadioAntenna antenna)
{
   string [] antennaTokens = antenna.CustomData.Split(':');
   if(freq == "default")
   {       
       antenna.CustomData = "comms:" + antennaTokens[1];
   }
   else
   {
   antenna.CustomData = freq + ":" + antennaTokens[1];
   }
}
public void clear(IMyProgrammableBlock pb, List<IMyTextPanel> terminals)
{
   pb.CustomData = "";

   foreach(IMyTextPanel t in terminals)
   {
       t.WriteText("");
   }
   
}

public void help(List<IMyTextPanel> terminals)
{
   foreach(IMyTextPanel t in terminals)
   {
       string [] messageTokens = t.CustomData.Split('\n');

       t.WriteText("Communcations Help:\nType your messages on ONE line ONLY."
       + "\n\nType \"cmd_help\" to access \nthis help screen.\n\nType \"cmd_clear\" to clear message hisory \nand display."
       + "\n\nType \"freq_<frequency name here>\" to \nchange your communications frequency.\n\nType \"freq_default\"" +
       " to revert to default \n\"comms\" frequency.\n\nType \"call_<callsign>\" for custom callsign.\n");

      t.CustomData = "Communcations Help:\nType your messages on ONE line ONLY."
       + "\n\nType \"cmd_help\" to access \nthis help screen.\n\nType \"cmd_clear\" to clear message hisory \nand display."
       + "\n\nType \"freq_<frequency name here>\" to \nchange your communications frequency.\n\nType \"freq_default\"" +
       " to revert to default \n\"comms\" frequency.\n\nType \"call_<callsign>\" for custom callsign.\n" + 
       messageTokens[messageTokens.Length - 1];

   }
}

void setCallsign(string callsign, IMyRadioAntenna antenna)
{
   string [] antennaTokens = antenna.CustomData.Split(':');
   
   antenna.CustomData = antennaTokens[0] + ":" + callsign;
   
}