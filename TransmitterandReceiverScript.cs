// ================================================================== 
// SCRIPT SETUP 
// Channel to listen for signals on
public string ListenChannel = "Default";
//Channel to transmit signals from
public string TransmitChannel = "Default";
// These may be set to the same thing without the transmitter sending the signal to itself. 
// if you wish to disable Debug messages set debug to false.
public bool Debug = true;
//
// ================================================================== 
// TO TRANSMIT
// Requirements: radio antenna, programable block, button(s) 
//  
// Assuming you have completed the SCRIPT SETUP section all that is left is the buttons 
// Drag this programable block to the button you wish to transmit with, and set it to run the programable block. 
// When prompted with the argument, set it to the name of the timer to trigger on the other grid. 
// To transmit on a diffrent channel you may use TimerBlockName , NewChannelName
// Please note that leating and trailing spaces are automaticaly removed before transmitting.
// 
// ================================================================== 
// TO RECEIVE
// Requirements: radio antenna, programable block, timer(s) 
//  
// Assuming you have completed the SCRIPT SETUP section all that is left is the antenna and timers.  
// Name each timer to match what your transmitter will be sending. 
// Finnaly set up your timer to do what ever you want. 
// 
// ================================================================== 

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    List<IMyTerminalBlock> antCheck = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(antCheck);
    if (antCheck.Count == 0){
        Log("ERROR: no connected antenna found.\n");
    }else{
        IGC.RegisterBroadcastListener(ListenChannel);
        Log("Waiting for signal on '" + ListenChannel + "' channel. \n");
    }
}

public void Main(string arg){
    string[] words = arg.Split(',');
    if (words.Count() > 1){
        if (!String.IsNullOrWhiteSpace(words[0]) && !String.IsNullOrWhiteSpace(words[1])){
            IGC.SendBroadcastMessage(words[1].Trim(), words[0].Trim(), TransmissionDistance.TransmissionDistanceMax);
            Log("Transmitted: '" + words[0].Trim() + "' on channel '" + words[1].Trim() + "'.\n");
        }
    }else{
        if (!String.IsNullOrWhiteSpace(words[0])){
            IGC.SendBroadcastMessage(TransmitChannel, words[0].Trim(), TransmissionDistance.TransmissionDistanceMax);
            Log("Transmitted: '" + words[0].Trim() + "' on '" + TransmitChannel + "' channel.\n");
        }
    }
    List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
    IGC.GetBroadcastListeners(listeners);
    if (listeners.Count() >= 1){
        if (listeners[0].HasPendingMessage){
            MyIGCMessage message = listeners[0].AcceptMessage();
            string funct = message.Data.ToString();

            IMyTimerBlock trigger = GridTerminalSystem.GetBlockWithName(funct) as IMyTimerBlock;
            if (trigger != null){
                Log("Received: '" + arg + "'\n");
                trigger.ApplyAction("TriggerNow");
            }else{
                Log("ERROR: recieved '" + arg + "', however that is not recognised as a timer. \n");
            }
        }
    }
}

void Log(string msg){
  if (Debug)  {
    Echo(msg);
  }
}
// Written by etopsirhc