#region ScriptCustomzableValues
const string _RemoteLCDidentifier = "[RemoteLCD]"; // TagFor Remote info LCD
const string _PBcustomArgDelimiters = "{}"; // Separators used for PB custom arguments
const char _MultipleCmdDelimiter = ';'; // MUST be ONE character between single quote ; Separators used for several commands
#endregion

#region ScriptInfo
/*
======================= CHANGELOG =======================
Version 5.6 [Add possibility to send multiple commands in one argument]
Version 5.5 [Correction case msg without space]
Version 5.4 [Add the possibility to customize the argument passing delimiters]
Version 5.3 [Add the possibility to send arguments to PB: "Run {Test argument value} PB target block name"]
Version 5.2 [Add ComChannel option as first argument, + antena check message]
Version 5.1 [Add Custom data Ini to customize default actions + process overides commands]
Version 4.0 [Add of LCD display + correction of messages]
Version 3.0 [Add of rotors, lights, projectors and Warheads]
Version 2.0 [Add of groups processing]

======================= DESCRIPTION =======================
Need to control hangar door opening, piston reverse, or a timer with a lot of actions in it (start, stop, trigger now) ?
This is why i did this script.

Now it is possible to customize the default action: if no particular action mentioned at the begining of the argument, perform the default action on the block.
Now it is possible to overide the default action, with any actions acceptable by block type.

/!\ Note that for non listed block type (in custom data), called with no action or an invalid action, the action will be to try to toggle the block on/off.
/!\ Camera "View" action is not valid as the action is performed by the PB, not the player (PB cannot "view" through camera)

Actions can be found at these adressses:
				https://steamcommunity.com/sharedfiles/filedetails/?id=360966557
                https://spaceengineerswiki.com/Programming_Guide/Action_List
                https://github.com/malware-dev/MDK-SE/wiki/List-Of-Terminal-Properties-and-Actions

It handles a received BROADCAST message: process the message and trigger actions.
It then responds with a UNICAST message back to the sending script.
It works both ways: PB on grid 1 can send commands to PB on grid 2, but PB on grid 2 can also send commands to PB on grid 1.

Thanks to Malware for the tutorials.
                This is based on MDK IGC Example 4 by wicorel.
                Example source: 
                https://github.com/Wicorel/WicoSpaceEngineers/tree/master/Modular/MDK%20IGC%20Example%204
                
                To learn more about ingame scripts, go to:
                https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts


======================= HOW TO USE =======================

Needs 2 grids, each one with:
 - An antenna
 - A programmable block (PB) with this script loaded

Sending commands:

 - Any block or group:
                Run the PB with the name of the block or the group to "activate" to trigger default actions listed in custom data (per block type), or toggle on/off if block type is not in the list

 - Override default action:
                Run the PB with the required action to perform at the begining and the name of the block or group as argument
                example of actions: "Start", "Stop", "TriggerNow", ... see posssible actions at addresses mentionned above

                Example of argument: "TriggerNow MyBaddassShip - Hangar - Roof door"
				=> default action on door blocks as "TriggerNow" is not a valid action, and trigger timers in the group as "TriggerNow" is a valid action and thus overide timer default action ("Start")
				
                Example of argument: "Detonate MyWarhead trap"
				=> Note that for "Detonate" action, warheads must be armed first, no need for "StartCountdown" action

======================= NOTES =======================

As it works with broadcasted messages:
 - no need to specify the destination PB name: all PBs with this script will process commands sent by another PB using this script.
 - if 2 or more grids have a block (a door for example) with the same name, all the blocks will be "toggled" (= doors on both grids will be opened or closed depending on their status)
 
*/
#endregion

WicoIGC _wicoIGC;

UpdateType _utTriggers = UpdateType.Terminal | UpdateType.Trigger | UpdateType.Mod | UpdateType.Script;
UpdateType _utUpdates = UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100 | UpdateType.Once;

#region ScriptGlobalParams
const int _TxTmaxLength=1000; // Max text length to display on LCD
const int _TxTDataSeparator=167; // code for separator character
const string INI_SECTION_GENERAL = "Remote Control"; // Name of the custom data section containing default actions
#endregion

bool _areWeInited = false;
string _broadCastTag;
string _unicastTag;
string _SourceGridName;

MyIni _ini = new MyIni();

#region Default Actions
private static Dictionary<string, TypeAction> BuildDefaultActDico()
{
    var TypeActions = new Dictionary<string, TypeAction>();
	
	AddToActDico(TypeActions, "BroadCastChannel", "CoolRemoteScriptChannel");
	AddToActDico(TypeActions, "IMyTimerBlock", "Start");
	AddToActDico(TypeActions, "IMyWarhead", "StartCountdown");
	AddToActDico(TypeActions, "IMyDoor", "Open");
	AddToActDico(TypeActions, "IMyAdvancedDoor", "Open");
	AddToActDico(TypeActions, "IMyAirtightHangarDoor", "Open");
	AddToActDico(TypeActions, "IMyAirtightSlideDoor", "Open");
	AddToActDico(TypeActions, "IMyPistonBase", "Reverse");
	AddToActDico(TypeActions, "IMyExtendedPistonBase", "Reverse");
	AddToActDico(TypeActions, "IMyMotorStator", "Reverse");
	AddToActDico(TypeActions, "IMyMotorAdvancedStator", "Reverse");
	AddToActDico(TypeActions, "IMyBatteryBlock", "Recharge");
	AddToActDico(TypeActions, "IMyGasTank", "Stockpile");
	AddToActDico(TypeActions, "IMyAirVent", "Depressurize");
	AddToActDico(TypeActions, "IMyShipController", "HandBrake");
	AddToActDico(TypeActions, "IMyCockpit", "HandBrake");
	AddToActDico(TypeActions, "IMyCryoChamber", "HandBrake");
	AddToActDico(TypeActions, "IMyMotorSuspension", "Braking");
	AddToActDico(TypeActions, "IMyLandingGear", "SwitchLock");
	AddToActDico(TypeActions, "IMyParachute", "Open");
	AddToActDico(TypeActions, "IMyProgrammableBlock", "RunWithDefaultArgument");
	AddToActDico(TypeActions, "IMyRemoteControl", "AutoPilot");
	AddToActDico(TypeActions, "IMyShipConnector", "SwitchLock");

    return TypeActions;
}
#endregion

private static void AddToActDico(Dictionary<string, TypeAction> TypeActions, string BlockType, string BlockAction)
{
    TypeAction theBlockTypeAction = new TypeAction();

    theBlockTypeAction.BlockType = BlockType;
    theBlockTypeAction.BlockAction = BlockAction;

    TypeActions.Add(key: theBlockTypeAction.BlockType, value: theBlockTypeAction);
}

public class TypeAction
{
    public string BlockType { get; set; }
    public string BlockAction { get; set; }
}

public Program()
{
	// ------  Initialize default action list from custom data, or regenerate default list if section INI_SECTION_GENERAL custom data is empty ------
	MyIniParseResult result;
		if (!_ini.TryParse(Me.CustomData, out result)) 
			throw new Exception(result.ToString());
		
	// Get the INI_SECTION_GENERAL section's output value. If this value is set, the system attempts  
	// to retrieve a text panel with the value set. Otherwise output is ignored.  
	
	List<MyIniKey> ListIniKeys = new List<MyIniKey>();
	_ini.GetKeys(INI_SECTION_GENERAL,ListIniKeys);
	
	if (ListIniKeys.Count==0)
	{
		// Build dictionary with default values
		Dictionary<string, TypeAction> DefActionsDico = BuildDefaultActDico();
		
		// set custom data
		foreach (KeyValuePair<string, TypeAction> kvp in DefActionsDico)
		{
			TypeAction TypeActionVar = kvp.Value;
			_ini.Set(INI_SECTION_GENERAL, TypeActionVar.BlockType, TypeActionVar.BlockAction);
		}
		
		var _TxTinfos = "---\n";
		_TxTinfos += INI_SECTION_GENERAL + " informations:" + "\n";
		_TxTinfos += "Communication channel for this PB: BroadCastChannel = \"CoolRemoteScriptChannel\"\n\n";
		_TxTinfos += "If called via this script, directly or within a group, \nall other blocks types (not listed here above)\nwill be toggled On/Off if possible as default action." + "\n";
		_TxTinfos += "You can change default actions if you want (need recompile)." + "\n";
		_TxTinfos += "Or you can add missing  blocks types default actions if you want (need recompile)." + "\n" + "\n";
		_TxTinfos += "See blocks definitions and possible actions on them at addresses below:" + "\n";
		_TxTinfos += "https://steamcommunity.com/sharedfiles/filedetails/?id=360966557" + "\n";
		_TxTinfos += "https://spaceengineerswiki.com/Programming_Guide/Action_List" + "\n";
		_TxTinfos += "https://github.com/malware-dev/MDK-SE/wiki/List-Of-Terminal-Properties-and-Actions" + "\n" + "\n";
		_TxTinfos += "To regenerate default custom data, just delete all and recompile." + "\n";
		_TxTinfos += "---\n";
		
		var _RemotePBsurf = Me.GetSurface(0);
		_RemotePBsurf.WriteText(_TxTinfos,false);
		
		Me.CustomData = _ini.ToString() + _TxTinfos;
	}
	
	// Initialise default broadCast channel
	_broadCastTag = _ini.Get(INI_SECTION_GENERAL, "BroadCastChannel").ToString();
	if (_broadCastTag=="") _broadCastTag = "CoolRemoteScriptChannel";
	
	// ------  End of default actions initialisation ------
    _wicoIGC = new WicoIGC(this);

	// cause ourselves to run again so we can do the init
    Runtime.UpdateFrequency = UpdateFrequency.Once;
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{

    if (!_areWeInited)
    {
        InitMessageHandlers();
        _areWeInited = true;
    }

    // always check for IGC messages in case some aren't using callbacks
    _wicoIGC.ProcessIGCMessages();
    if ((updateSource & UpdateType.IGC) > 0)
    {
        // we got a callback for an IGC message.
        // but we already processed them.
    }
    else if ((updateSource & _utTriggers) > 0)
    {
		if (!CheckAntenas(_HasToBroadcast:true)) {
			DisplayAndReply("ERROR: No working, or enable to broadcast, antena found.", AppendTxT:false);
		} else {
			// if we got a 'trigger' source, send out the received argument
			_unicastTag = Me.EntityId.ToString("X");
			DisplayAndReply("Sending Request: " + argument, AppendTxT:false);
			
			var _FinalbroadCastTag = _broadCastTag;
			string _MsgTxT = argument;
			if (argument.Trim().StartsWith("[") && argument.IndexOf("]")!=-1)
			{
				_FinalbroadCastTag=argument.Substring(1, argument.IndexOf("]")-1).Trim();
				argument=argument.Substring(argument.IndexOf("]")+1).Trim();
			}
			//Echo("_FinalbroadCastTag =" + _FinalbroadCastTag + "\n");
			//Echo("argument =" + argument + "\n");
			
			IGC.SendBroadcastMessage(_FinalbroadCastTag, _unicastTag + char.ConvertFromUtf32(_TxTDataSeparator) + Me.CubeGrid.CustomName + char.ConvertFromUtf32(_TxTDataSeparator) + argument);
		}
    }
    else if ((updateSource & _utUpdates) > 0)
    {
        // it was an automatic update

        // this script doens't have anything to do
    }
}

bool CheckAntenas(bool _HasToBroadcast=false)
{
	List<IMyRadioAntenna> _RadioAntenas = new List<IMyRadioAntenna>();
	GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(_RadioAntenas);
	
	foreach (var _AntenaBlock in _RadioAntenas)
	{
		if (_AntenaBlock.Enabled && (_AntenaBlock.EnableBroadcasting || !_HasToBroadcast) && _AntenaBlock.IsWorking) {
			return true;
		}
	}
	
	List<IMyLaserAntenna> _LaserAntennas = new List<IMyLaserAntenna>();
	GridTerminalSystem.GetBlocksOfType<IMyLaserAntenna>(_LaserAntennas);
	
	foreach (var _AntenaBlock in _LaserAntennas)
	{
		if (_AntenaBlock.Enabled && _AntenaBlock.Status==MyLaserAntennaStatus.Connected && _AntenaBlock.IsWorking) {
			return true;
		}
	}
	
    return false;
}

void InitMessageHandlers()
{
    // creates a BROADCAST channel with the specified tag and calls the handler when messages are processed
    _wicoIGC.AddPublicHandler(_broadCastTag, ProcessingReceivedBroadcastedMSG);

    // calls the handler when UNICAST messages are processed
    _wicoIGC.AddUnicastHandler(ProcessingReceivedUnicastMSG);
}


// Handler for the broadcast messages received.
void ProcessingReceivedBroadcastedMSG(MyIGCMessage msg)
{
    // NOTE: called on ALL received messages; not just 'our' tag

	if (!CheckAntenas())
	{
		DisplayAndReply("ERROR: No working, or enable to broadcast, antena found.", AppendTxT:false);
		return; // Antenas not working
	}

    if (msg.Tag != _broadCastTag)
        return; // not our message

    if (msg.Data is string)
    {
		string[] TmpStr = msg.Data.ToString().Split(char.Parse(char.ConvertFromUtf32(_TxTDataSeparator)));
		_unicastTag = TmpStr[0].Trim(); // Get sender unicast ID sent at the begining of the message
		_SourceGridName = TmpStr[1].Trim(); // Get sender ship name
		
		string _CommandsTxT = TmpStr[2].Trim(); // Get the message text
		
		//Actions based on message text received
		//---------------------------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------------------------
				
				string[] _CommandsList = _CommandsTxT.Split(_MultipleCmdDelimiter);
				
				foreach (var MsgTxT in _CommandsList) // Process several commands send in one argument
				{
					string _BlockOrGroupName=MsgTxT.Trim();
					
					// Extract optional command at the begining, if any
					string _Command = MsgTxT.Substring(0, MsgTxT.IndexOf(" ")+1).Trim(); // First word is the command (if any)				
					
					if (_Command.Length>0)
					{
						_Command = _Command.Substring(0,1).ToUpper() + _Command.Substring(1,_Command.Length-1).ToLower(); //Première lettre en majuscules
						_BlockOrGroupName = MsgTxT.Substring(_Command.Length).Trim(); // Get the block or group name if a command is set
					}
					
					// Extract optional argument to send to other PB, if any
					string _OptArg = "";
					if (_BlockOrGroupName.Trim().StartsWith(_PBcustomArgDelimiters.Substring(0,1)) && _BlockOrGroupName.IndexOf(_PBcustomArgDelimiters.Substring(1,1))!=-1)
					{
						_OptArg=_BlockOrGroupName.Substring(1, _BlockOrGroupName.IndexOf(_PBcustomArgDelimiters.Substring(1,1))-1).Trim();
						_BlockOrGroupName=_BlockOrGroupName.Substring(_BlockOrGroupName.IndexOf(_PBcustomArgDelimiters.Substring(1,1))+1).Trim();
					}
					
					// For compatibility with previous version of the script
					if (_Command.ToLower() == "trigger") {
						_Command="TriggerNow";
					}
					
					// Find block
					var _TargetBlock = GridTerminalSystem.GetBlockWithName(MsgTxT); // If no command set (just the name transmitted)
					if (_TargetBlock == null) {
						_TargetBlock = GridTerminalSystem.GetBlockWithName(_BlockOrGroupName); // if block not found, maybe a command was sent: check the short name
					}
					
					//Find group
					var _TargetGroup = GridTerminalSystem.GetBlockGroupWithName(MsgTxT); // If no command set (just the groupname transmitted)
					if (_TargetGroup == null) {
						_TargetGroup = GridTerminalSystem.GetBlockGroupWithName(_BlockOrGroupName); // if group not found, maybe a command was sent: check the short name
					}
					
					if (_TargetBlock == null && _TargetGroup == null)
					{
						// If the block or group does not exist, request may not be for this ship/station, display only a warning in PB details, NOT on PB screen or LCDs.
						Echo("Not on this grid: " + MsgTxT);
						return;
					}
					else 
					{
						List<IMyTerminalBlock> ListTargetBlocks = new List<IMyTerminalBlock>();
						
						if (_TargetGroup != null)
						{
							// Lists of all the blocks within the group.
							_TargetGroup.GetBlocks(ListTargetBlocks);
						} else if (_TargetBlock != null) {
							// Add the unique block to the list
							ListTargetBlocks.Add(_TargetBlock);
						}
						
						// Goes through all blocks of the list
						foreach (var _ListedBlock in ListTargetBlocks)
						{
							if(Me.CubeGrid == _ListedBlock.CubeGrid) // Avoid actions on connected grids
							{
								string _Type = _ListedBlock.GetType().ToString();
								_Type=_Type.Substring(_Type.LastIndexOf(".")+1);
								if (!_Type.StartsWith("I")) _Type="I"+_Type;
								bool _ListedBlockStatus = true;

								if (_ListedBlock.GetProperty("OnOff") != null) {
									_ListedBlockStatus = _ListedBlock.GetValueBool("OnOff");
								}
								
								if (_ListedBlock.GetActionWithName(_Command)!= null)
								{
									if (_Command != "OnOff" && _Command != "OnOff_On" && _Command != "OnOff_Off" && !_ListedBlockStatus) {
										DisplayAndReply("Denied : ''" + _ListedBlock.CustomName + "'' is off.",msg.Source);
										return;
									} else {
										if(_Type=="IMyProgrammableBlock" && _Command=="Run")
										{
											IMyProgrammableBlock PBtoRUN = (IMyProgrammableBlock)_ListedBlock;
											if (!PBtoRUN.IsRunning)
											{
												if (!PBtoRUN.TryRun(_OptArg)) // TryRun returns true if the action was applied, false otherwise.
												{
													// Error, PB may be busy (already running)
													DisplayAndReply("Error : ''" + _ListedBlock.CustomName + "'' may be already running.",msg.Source);
												}
											}
										} else {
											_ListedBlock.ApplyAction(_Command);
										}
									}
								} else if (_ini.Get(INI_SECTION_GENERAL, _Type).ToString()!= "") {
									string _TmpCommand = _ini.Get(INI_SECTION_GENERAL, _Type).ToString();
									if (_TmpCommand != "OnOff" && _TmpCommand != "OnOff_On" && _TmpCommand != "OnOff_Off" && !_ListedBlockStatus) {
										DisplayAndReply("Denied : ''" + _ListedBlock.CustomName + "'' is off.",msg.Source);
										return;
									} else {
										if(_Type=="IMyProgrammableBlock" && _TmpCommand=="Run")
										{
											IMyProgrammableBlock PBtoRUN = (IMyProgrammableBlock)_ListedBlock;
											if (!PBtoRUN.IsRunning)
											{
												if (!PBtoRUN.TryRun(_OptArg)) // TryRun returns true if the action was applied, false otherwise.
												{
													// Error, PB may be busy (already running)
													DisplayAndReply("Error : ''" + _ListedBlock.CustomName + "'' may be already running.",msg.Source);
												}
											}
										} else {
											_ListedBlock.ApplyAction(_TmpCommand);
										}
									}
								} else if (_ListedBlock.GetActionWithName("OnOff")!= null) {
									_ListedBlock.ApplyAction("OnOff");
								}
							}
						}
					}
				}
	
		//---------------------------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------------------------
		
		// Now reply to the sender and let them know we received the message
		DisplayAndReply(_CommandsTxT + " processed.");
		IGC.SendUnicastMessage(msg.Source, _unicastTag, Me.CubeGrid.CustomName+": Done.");
    }
}

// Function to display text on the PB screen or on LCD with the tag 
// defined in _RemoteLCDidentifier constant in their name
void DisplayAndReply(string _TxTtoDisplay, long MsgSource = -1, bool AppendTxT=true)
{
	string _TxTonScreen;
	
	if (_TxTtoDisplay!="") {
	
		if (MsgSource!=-1) {
			IGC.SendUnicastMessage(MsgSource, _unicastTag, Me.CubeGrid.CustomName+" "+_TxTtoDisplay);
		}
		
		if (_SourceGridName!=null) {
			_TxTtoDisplay= "Request from: " + _SourceGridName + "; " + _TxTtoDisplay;
		}
		
		Echo(_TxTtoDisplay);
		
		var _RemotePBsurf = Me.GetSurface(0);
		if (_RemotePBsurf.ContentType != ContentType.TEXT_AND_IMAGE) {_RemotePBsurf.ContentType = ContentType.TEXT_AND_IMAGE;}
		if(_RemotePBsurf.GetText()!="") {
			if (_RemotePBsurf.GetText().StartsWith("---")) AppendTxT=false;
			_TxTonScreen=_TxTtoDisplay+"\n";
		} else {
			_TxTonScreen=_TxTtoDisplay+"\n";
		}
		if((int) _TxTonScreen.Length > (int) _TxTmaxLength)
		{
			if (_TxTonScreen.LastIndexOf("\n",_TxTmaxLength)!=-1) {
				_TxTonScreen=_TxTonScreen.Remove(_TxTonScreen.LastIndexOf("\n",_TxTmaxLength)-1);
			} else {
				_TxTonScreen=_TxTonScreen.Remove(_TxTmaxLength);
			}
		}
		_RemotePBsurf.WriteText(_TxTonScreen,AppendTxT);
		_TxTonScreen="";

		List<IMyTerminalBlock> ListRemoteMSGTXTLCDs = new List<IMyTerminalBlock>();
		GridTerminalSystem.SearchBlocksOfName(_RemoteLCDidentifier,ListRemoteMSGTXTLCDs);
		
		foreach (var _RemoteMSGTXTLCD in ListRemoteMSGTXTLCDs)
		{
			if (_RemoteMSGTXTLCD is IMyTextSurface && _RemoteMSGTXTLCD != Me) {
				((IMyTextSurface)_RemoteMSGTXTLCD).ContentType = ContentType.TEXT_AND_IMAGE;
				if(((IMyTextSurface)_RemoteMSGTXTLCD).GetText()!="") {
					_TxTonScreen=_TxTtoDisplay+"\n";
				} else {
					_TxTonScreen=_TxTtoDisplay+"\n";
				}
				if((int) _TxTonScreen.Length > (int) _TxTmaxLength) {
					if (_TxTonScreen.LastIndexOf("\n",_TxTmaxLength)!=-1) {
						_TxTonScreen=_TxTonScreen.Remove(_TxTonScreen.LastIndexOf("\n",_TxTmaxLength)-1);
					} else {
						_TxTonScreen=_TxTonScreen.Remove(_TxTmaxLength);
					}
				}
				((IMyTextSurface)_RemoteMSGTXTLCD).WriteText(_TxTonScreen,AppendTxT);
				_TxTonScreen="";
			}
		}
	}
}

void ProcessingReceivedUnicastMSG(MyIGCMessage msg)
{
    // NOTE: Called for ALL received unicast messages
	_unicastTag = Me.EntityId.ToString("X");
    if (msg.Tag != _unicastTag)
        return; // not our message

    if (msg.Data is string)
    {
        //-----------------------------------
		//Actions on data received
		DisplayAndReply("Feedback: " + msg.Data.ToString());
    }

}

// Source is available from: https://github.com/Wicorel/WicoSpaceEngineers/tree/master/Modular/IGC
class WicoIGC
{
    // the one and only unicast listener.  Must be shared amoung all interested parties
    IMyUnicastListener _unicastListener;

    /// <summary>
            /// the list of unicast message handlers. All handlers will be called on pending messages
            /// </summary>
    List<Action<MyIGCMessage>> _unicastMessageHandlers = new List<Action<MyIGCMessage>>();

    /// <summary>
            /// List of 'registered' broadcst message handlers.  All handlers will be called on each message received
            /// </summary>
    List<Action<MyIGCMessage>> _broadcastMessageHandlers = new List<Action<MyIGCMessage>>();
    /// <summary>
            /// List of broadcast channels.  All channels will be checked for incoming messages
            /// </summary>
    List<IMyBroadcastListener> _broadcastChannels = new List<IMyBroadcastListener>();

    MyGridProgram _gridProgram;
    bool _debug = false;
    IMyTextPanel _debugTextPanel;

    /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="myProgram"></param>
            /// <param name="debug"></param>
    public WicoIGC(MyGridProgram myProgram, bool debug = false)
    {
        _gridProgram = myProgram;
        _debug = debug;
        _debugTextPanel = _gridProgram.GridTerminalSystem.GetBlockWithName("IGC Report") as IMyTextPanel;
        if (_debug) _debugTextPanel?.WriteText("");
    }

    /// <summary>
            /// Call to add a handler for public messages.  Also registers the tag with IGC for reception.
            /// </summary>
            /// <param name="channelTag">The tag for the channel.  This should be unique to the use of the channel.</param>
            /// <param name="handler">The handler for messages when received. Note that this handler will be called with ALL broadcast messages; not just the one from ChannelTag</param>
            /// <param name="setCallback">Should a callback be set on the channel. The system will call Main() when the IGC message is received.</param>
            /// <returns></returns>
    public bool AddPublicHandler(string channelTag, Action<MyIGCMessage> handler, bool setCallback = true)
    {
        IMyBroadcastListener publicChannel;
        // IGC Init
        publicChannel = _gridProgram.IGC.RegisterBroadcastListener(channelTag); // What it listens for
        if (setCallback) publicChannel.SetMessageCallback(channelTag); // What it will run the PB with once it has a message

        // add broadcast message handlers
        _broadcastMessageHandlers.Add(handler);

        // add to list of channels to check
        _broadcastChannels.Add(publicChannel);
        return true;
    }

    /// <summary>
            /// Add a unicast handler.
            /// </summary>
            /// <param name="handler">The handler for messages when received. Note that this handler will be called with ALL Unicast messages. Always sets a callback handler</param>
            /// <returns></returns>
    public bool AddUnicastHandler(Action<MyIGCMessage> handler)
    {
        _unicastListener = _gridProgram.IGC.UnicastListener;
        _unicastListener.SetMessageCallback("UNICAST");
        _unicastMessageHandlers.Add(handler);
        return true;

    }
    /// <summary>
            /// Process all pending IGC messages.
            /// </summary>
    public void ProcessIGCMessages()
    {
        bool bFoundMessages = false;
        if (_debug) _gridProgram.Echo(_broadcastChannels.Count.ToString() + " broadcast channels");
        if (_debug) _gridProgram.Echo(_broadcastMessageHandlers.Count.ToString() + " broadcast message handlers");
        if (_debug) _gridProgram.Echo(_unicastMessageHandlers.Count.ToString() + " unicast message handlers");
        // TODO: make this a yield return thing if processing takes too long
        do
        {
            bFoundMessages = false;
            foreach (var channel in _broadcastChannels)
            {
                if (channel.HasPendingMessage)
                {
                    bFoundMessages = true;
                    var msg = channel.AcceptMessage();
                    if (_debug)
                    {
                        _gridProgram.Echo("Broadcast received. TAG:" + msg.Tag);
                        _debugTextPanel?.WriteText("IGC:" +msg.Tag+" SRC:"+msg.Source.ToString("X")+"\n",true);
                    }
                    foreach (var handler in _broadcastMessageHandlers)
                    {
                        handler(msg);
                    }
                }
            }
        } while (bFoundMessages); // Process all pending messages

        if (_unicastListener != null)
        {
            // TODO: make this a yield return thing if processing takes too long
            do
            {
                // since there's only one channel, we could just use .HasPendingMessages directly.. but this keeps the code loops the same
                bFoundMessages = false;

                if (_unicastListener.HasPendingMessage)
                {
                    bFoundMessages = true;
                    var msg = _unicastListener.AcceptMessage();
                    if (_debug) _gridProgram.Echo("Unicast received. TAG:" + msg.Tag);
                    foreach (var handler in _unicastMessageHandlers)
                    {
                        // Call each handler
                        handler(msg);
                    }
                }
            } while (bFoundMessages); // Process all pending messages
        }

    }
}