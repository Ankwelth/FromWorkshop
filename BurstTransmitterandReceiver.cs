// Burst Transmitter &  Receiver \\

// This program uses the antenna that it's attached to to send a single string message
// the string that is sent is whatever is typed in when it's ran.
//
// This script can only transmit one thing at a time, if an argument is passed while the script
// is already transmitting it will be ignored. Transmissions take about a second.
//-------------------   

// || Configuration is below || \\

//------------------- 
// Name config
//------------------- 
//Type the name of your Antenna you want to send the messages with.
const string antennaName = "Antenna";

//(Optional)Type the name of the LCD you want the script to output to.
const string lcdNameSend = "LCD Panel"; // Sent transmission LCD
const string lcdNameRecieve = "LCD Panel 2"; // Recieved transmission LCD
// Type the channel the burst trasmitter will transmit to here.
const string channelName = "Burst Channel 01"; // Using the default channel is not recommended.
const string unicastChannelName = "Burst channel 01"; // You can provide a unicast channel to listen/recieve on, however, this transmitter doesn't have unicast sending functionality. 

const string locationPhrase = "{location}"; //If this phrase is passed as part of a transmission the phrase will automatically be replaced with GPS coordinates for your burst antenna's current location.
//if anything else is typed along with this phrase it'll be untouched and sent along with the GPS coordinates.

//------------------- 
// Burst radius config
//------------------- 
// This is the radius you want the Antenna to be set to when sending the message (0 ~ 50000) [If you're using a modded antenna it could be more than 50000]
const int sendingRadius = 50000;

// This is the radius you want the Antenna to be set to when the script has finished sending the message (0 ~ 50000) [If you're using a modded antenna it could be more than 50000]
const int finishedRadius = 0;

//------------------- 
// LCD config
//------------------- 
// Enable LCD Output?
const bool LCDOutput = false;

// How many lines of transmission history should be recorded to the LCD? [2 is the minimum]
const int LCDLines = 5;

//------------------- 
// Passphrase config
//------------------- 
// When you change the values below make sure the PASSPHRASEs and TIMER BLOCK names match what you typed! 
// It's CAsE-SeNSitiVE!
//------------------- 
// This code RECEIVES a message that has been broadcast and checks if it.
// If the message matches any of the corresponding passphrases below, it will start the timer block that passphrase was assigned to.
//------------------- 
// The passphrases and timers are all separate, this means Passphrase 1 will always activate Timer 1, but Passphrase 1 
// would never activate Timer 2 or Timer 3, and vice versa.
//------------------- 
// Below is the configuration for the passphrase and timer pairs, you can add as many pairs as you want.  
//------------------- 
        List<string> Passphrases = new List<string>()  
        {  
            // Change the strings in the quotes to whatever you want. You can even add more(or less) than three if you need to.
                 "Share your location",  
                 "Passphrase number two",  
                 "Passphrase number three" // <-- but please remember that the last line differs, and no colon is used.
        };  
   
        List<string> Timers = new List<string>()  
        {  
            // The names of the corresponding timer blocks for each passphrase, like last time, the names can be whatever you want.
                     "Timer1",  
                     "Timer2",  
                     "Timer3" // <-- but please remember, like last time, that the last line differs and doesn't have a colon.
        }; 
 
        //You may change "TriggerNow" to "Start" instead if you want. 
        string TimerAction = "TriggerNow";  

//------------------------------
// Do not change the code below
//------------------------------

string msg ; 
int count=0;
IMyBroadcastListener _myBroadcastListener;
IMyUnicastListener _uListener;
public Program()
{
	_myBroadcastListener=IGC.RegisterBroadcastListener(channelName);
	_myBroadcastListener.SetMessageCallback(channelName); 
	 _uListener = IGC.UnicastListener;
	_uListener.SetMessageCallback(unicastChannelName);
}

public void Main(string argument, UpdateType updateType) {
var LCDPanelSend = GridTerminalSystem.GetBlockWithName(lcdNameSend) as IMyTextPanel;
var LCDPanelRecieve = GridTerminalSystem.GetBlockWithName(lcdNameRecieve) as IMyTextPanel;
var ant = GridTerminalSystem.GetBlockWithName(antennaName) as IMyRadioAntenna;
var antGPS = GridTerminalSystem.GetBlockWithName(antennaName) as IMyTerminalBlock;
System.DateTime now = System.DateTime.UtcNow;
if ((updateType & (UpdateType.Trigger | UpdateType.Terminal)) != 0)
  {
	writeLCD(LCDPanelSend , now.ToString() + ": Transmitting \"" + argument + "\"");
	Echo("Transmitting..");
	ant.SetValueFloat("Radius",sendingRadius);
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
	msg = argument;
  }
 if ((updateType & (UpdateType.Update100|UpdateType.Once)) != 0) {
	count = count +1;
	if(count==1 & !msg.Contains(locationPhrase)){
		writeLCD(LCDPanelSend ,  now.ToString() + ": \"" + msg + "\""+" sent.");
		Echo("Transmission sent.");
		IGC.SendBroadcastMessage<string>(channelName,  msg);
		ant.SetValueFloat("Radius",finishedRadius);
		Runtime.UpdateFrequency = UpdateFrequency.Once;
	}else{if(count==1 & msg.Contains(locationPhrase)){
		writeLCD(LCDPanelSend ,  now.ToString() + ": \"" + msg + "\""+" sent.");
		Echo("Transmission sent.");
		msg = msg.Replace(locationPhrase, "GPS:Received Signal " + GenerateString(6) + ":" + antGPS.GetPosition().GetDim(0).ToString("F2") + ":" + antGPS.GetPosition().GetDim(1).ToString("F2") + ":" + antGPS.GetPosition().GetDim(2).ToString("F2") + ":");
		IGC.SendBroadcastMessage<string>(channelName,  msg);
		Echo(msg);
		ant.SetValueFloat("Radius",finishedRadius);
		Runtime.UpdateFrequency = UpdateFrequency.Once;
	}}
		if(count==2){
		count =0 ;
		msg = "";
	}
  }
if((updateType & UpdateType.IGC) != 0){
var temporaryString = "";
 while (_myBroadcastListener.HasPendingMessage || _uListener.HasPendingMessage )
		{
			Echo(_myBroadcastListener.HasPendingMessage ?"broad":"uni");
			MyIGCMessage myIGCMessage  = _myBroadcastListener.HasPendingMessage ?_myBroadcastListener.AcceptMessage() : _uListener.AcceptMessage();
				if(myIGCMessage.Data is string)
				{
					checkForPassphrase(myIGCMessage.Data.ToString());
					string customData = LCDPanelRecieve.CustomData;
					string customData1 = LCDPanelSend.CustomData;
					string userIDtoNameDict = "";
						if(customData != ""){
							string[] customDataAry = LCDPanelRecieve.CustomData.Split('\n');
							Dictionary<string, string> result = new Dictionary<string, string>();
							for (int i = 0; i < customDataAry.Length; i++)
							{
								string[] keyvalue = customDataAry[i].Split('=');
								if (keyvalue.Length == 2) 
								{ 
									result.Add(keyvalue[0], keyvalue[1]);
								}
							}
							try{
								userIDtoNameDict = result[myIGCMessage.Source.ToString("X")].ToString(); // Look for nickname in LCD's data
							}
							catch
							{
								userIDtoNameDict = myIGCMessage.Source.ToString("X");
								LCDPanelRecieve.CustomData = LCDPanelRecieve.CustomData + myIGCMessage.Source.ToString("X") + "=" + myIGCMessage.Source.ToString("X") + "\n";
								// Add the ID to custom data and carry on if a nickname cannot be found
							}
						}else{
							userIDtoNameDict = myIGCMessage.Source.ToString("X");
							LCDPanelRecieve.CustomData = LCDPanelRecieve.CustomData + myIGCMessage.Source.ToString("X") + "=" + myIGCMessage.Source.ToString("X") + "\n";
						}
					if(temporaryString == ""){
					temporaryString = "(" + now.ToString() + ") " + userIDtoNameDict + ": " + myIGCMessage.Data.ToString();
					}else{
					temporaryString = temporaryString + "\n" + "(" + now.ToString() + ") " + userIDtoNameDict + ": " + myIGCMessage.Data.ToString();
					}
					
					string str = myIGCMessage.Data.ToString();
					Echo("Received IGC Message");
					Echo("Data=" + myIGCMessage.Data.ToString());
					Echo("Source=" + myIGCMessage.Source.ToString());
				}
		}
writeLCD(LCDPanelRecieve , temporaryString);
}
}

public void writeLCD(IMyTextPanel lcd , string newLine){
if(LCDOutput && lcd != null){
lcd.ContentType = ContentType.TEXT_AND_IMAGE;
			var LCDContent = lcd.GetText();
			var LCDContentAry = LCDContent.Split('\n');
			if(LCDContentAry.Length >= LCDLines + 1){
				LCDContentAry = LCDContentAry.Skip(1).ToArray();
			}
			if(LCDContentAry.Length <= 1){
				lcd.WriteText(newLine + "\n");
			}else{
				lcd.WriteText(string.Join("\n", LCDContentAry) + newLine + "\n");
			}
		}
}

	  public void checkForPassphrase(string Msg)
		{  
			for (int i = 0; i < Passphrases.Count; i++)  
				if (Msg == Passphrases[i])  
				{  
					var Timer = GridTerminalSystem.GetBlockWithName(Timers[i]) as IMyTimerBlock;  
					Timer.GetActionWithName(TimerAction).Apply(Timer);  
				}  
		}
	Random rand = new Random();
		
	public const string Alphabet = 
	"abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		
	  public string GenerateString(int size)
		{
			char[] chars = new char[size];
			for (int i=0; i < size; i++)
			{
				chars[i] = Alphabet[rand.Next(Alphabet.Length)];
			}
			return new string(chars);
		}