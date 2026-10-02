/*  
[MEA]New Automatic Mining Program_Mother Ship Section  
Authors:Missile Engineering Agency Team（China）  
Contributors: MEA group leader（Mothership Program), zzhou（Mining ship control program), Schwarzengel（main designer), mentong[small miner], as well as other active partners.  
  
[Installation Instructions]

To the Mothership :  
One widescreen LCD, name must correspond 
A master seat or cockpit, name must correspond  
A camera, the name must correspond, note: the camera must be common line with the main control seat, otherwise there will be a large error in scanning asteroids  
A programming block, loaded with this code (don't use the name as for mining ships) 
A timing block, set the countdown to 1, execute: (trigger itself immediately) (start itself) (run the programming block command is empty), then trigger this timing block  
   
For mining ships :  
One programming block, names must correspond, no need to loop with timing blocks  
At least one thruster both front and rear  
At least one gyroscope  
IMPORTANT:The connector must be directly behind the remote control block, otherwise it cannot be docked. 

Names for blocks can be changed and checked under this instruction.  

[How to use]
First dock your mothership and mining ships through the connectors (green state)  
After installing the mining ships, you need to make sure that all the mining ships' programming blocks are set with the correct names, then put the mining ship's code into the mining ship's programming block, no need to run, no need to time the block to trigger it, just put it in and ignore it  
(Note that you must make sure that the closest mining ship connector to the programming block is used to lock with the mother ship, at the same time, ejectors count as connectors, if you have ejectors, make sure that the distance between the ejectors and the programming block is greater than the distance between the connector used to connect with the mother ship and the programming block)  
Then initialize the programming block of the mothership  
To initialize it, click the Edit Code button on the mothership's programming block, and then click Save.  
After initialization, the LCD will automatically display the number of mining ships.

You need to set a command OnOff manually for the programming block of the mothership  
The procedure is to sit on the seat, drag the programming block to the shortcut bar, select Run, and enter the command OnOff at the end.  
After you press OnOff once, you will enter the control state.  
In the control state, the programming block will turn off the thruster control of the main control seat, at this time your WASD button corresponds to the up and down, left and right operations  
This operation can be reflected by the cursor on the LCD panel  
Spacebar is OK, C key is back  
When you need to regain control of the ship's movement, just press the OnOff button of the programming block once more.

  
[Mining ships control]
Under this page you can view the status of mining ships, when the number of mining ships exceeds the number that can be displayed on the page, you can press WS to turn the page  
The first option is to give commands to all ships at the same time  
Press AD to select the command, the command contains 4:  
Stop: immediately stop all actions and turn on thrusters and gyroscope inertia suppression to slow down  
Dock: Use in an already docked state is ineffective, allowing the ship to return to the mothership and automatically dock and lock.  
	The ship will go through 3 states.  
	1:Fly directly in front of the corresponding mothership connector, when arrival is detected, enter state 2, if the mother ship moves during this process, the small ship will follow until arrival  
	2: Start steering and make your own remote control block rear facing the mother ship connector, so you must ensure that the connector is always installed behind the remote control block, when the gyroscope torque detected is very small, that means it has been aligned, then enter state 3  
	3:Start controlling the forward thruster advance and keep docking speed continuously until the connector locks automatically. When it is about to reach the connector position, it will detect whether it is on the normal of the mother ship connector, if the deviation is too big, it will return to state 1. so it is better to keep the mother ship stationary during this process  
Follow: The mining ships will follow the mother ship in formation [set docking distance] meters behind the mother ship, and keep each ship that far apart from each other  
Mine: The mining command can only be given if there is a selected target. The mining ship will build a spatial square container based on the target coordinates and radius, and then the mining ship will plan its path according to the mining accuracy until it has gone through the area.  
  
[Target Management]  
You can check the scanning status of the camera in this page, the automatic scanning of the camera is on by default  
During auto scan, you can scan up to 2 km per second, you can adjust the scanning range by AD and spacebar  
You can record the scanned target here and send it to the mining ship later to execute the mining command, you can select the target by WS and manage the delete or add operation.  
  
[Mining ships status]  
Here you can check the status of each mining ship, including the power and the items of all the containers on the ship  
WS switch mining ship, AD switch items  
  
[Parameter Setting]  
You can modify some parameters here, the details are explained  
  
[Mining ships configuration]  
You can configure some custom parameters in the mining ship code, especially in the code (customizable variables) section  
  
[Communication protocols]  
The mining ship writes state to CustomData of its own programming block:  
The mother ship controls the mining ship via TryRun(arguments):  
0 mining ship ID * 1 command * 2 target coordinates * 3 target radius * 4 mining accuracy * 5 whether to return automatically (1 yes; 0 no)  
The mothership detects the CustomData of the mining ship programming block to get its status:  
0 mining ship ID * 1 mining ship command * 2 mining ship status * 3 mining ship coordinates * 4 mining ship speed * 5 power consumption ratio  
  
Update note]  
This program since v4.0 version  
v4.1: Fixed the camera scanning interval algorithm, before the scanning distance is greater than 2000 meters can not be correctly scanned, now fixed, the scanning interval is every 2000 meters need 1 second, so the farther the scanning interval need to wait for a longer time  
v4.2: Fixed a bug that caused the main program to report an error when returning to the home page from the mine ship management page  
v4.3: Added a new command for whether to return to the ship automatically.  
*/  
  
string stMotherShipCockpit = "#ATM_Cockpit"; //Mothership's master control seat name, a necessity  
string stTargetCamera = "#ATM_Lock_Camera"; //The name of the camera used for scanning, a necessity  
string stMainLCD = "#ATM_ShowLCD"; //The main LCD panel name (It must be widescreen, otherwise the display is not complete), a necessity  
  
//Programming blocks for mining ships, the program gets them by name, and with multiple mining ships they are all called by just one name    
//Note: The program will also get the remote control block of the mining ship, but without setting the name, it is searched by type and matched one-to-one according to the distance to the programming block  
string stMiningShipComputer = "MS_Computer"; //mining ship's probraming block, a necessity. DO NOT USE FOR MOTHERSHIP'S PROGRAMING BLOCK
  
IMyCockpit oCockpit;  
IMyCameraBlock oTargetCamera;  
IMyTextPanel oMainLCD;  
  
List <IMyTerminalBlock> osMS_Computer; //Programming blocks for automatic mining ships, os indicates List of blocks MS indicates mining ships  
List <IMyTerminalBlock> osMS_Remote; //Unmanned control blocks for automated mining vessels, with automatic acquisition based on distance  
  
//OS control information  
Vector3 WASDControl;  
int OSSelect_X = 1;  
int OSSelect_Y = 1;  
int OSPage = 0; //0 Home, 1 Mining ship control, 2 Target control, 3 Mining ship status, 4 Parameter setting, 5 is a separate target selection page  
int OSSelect_Main = 1; //Home Page 
bool IsFreeInput = true; //Detect button fully released  
  
//Mining ship command and status information  
List<int> MiningShipCommand; //Command: 1Stop, 2Dock, 3Follow, 4Work  
List<int> MiningShipStatus; //Mining ship status: 0 unknown, 1 stopped, 2 in transit, 3 arrived, 4 working, 5 docked, 6 damaged  
List<Vector3D> MiningShipPosition; //Mining ship coordinates  
List<Vector3D> MiningShipSpeed; //Mining ship speed  
List<double> MiningShipPower;//Mining ship power  
List<string> MiningShipBoxContant;  
List<Vector3D> MiningShipTargetPosition; //Mining ship work target coordinates  
List<MyDetectedEntityInfo> MiningShipTargetPositionInfo; //Mining coordinates for a working ship  
int SelectTargetInt = -1; //The currently selected target serial number, -1 means no target sellected  
bool IsAutoReturn = false;  
  
//Scanning target information  
List <MyDetectedEntityInfo> TargetInfo;  
MyDetectedEntityInfo FoundObjectInfo;  
bool IsAutoScan = true;  
double ScanDistance = 500; //Scan range, from which the auto-scan frequency is calculated  
  
//Parameter setting information  
double FollowInterval = 100;//Distance between mining ships while following  
int MiningAccuracy = 7;//Mining accuracy, equivalent to scanning intervals  
  
//Display control information  
string sp10 = "                    ";  
string sp7 = "             ";  
string sp4 = "        ";  
string sp2 = "    ";  
string sp1 = "  ";  
string br = "\n";  
  
//Main logic of script  
bool init = false;  
int t = 0;  
bool isOScontrol = false;  
string debug = "System OK";  
  
void Main(string arguments)  
{  
	if(!init) //Initialize to get the block  
	{  
		GetBlocks();  
	}  
	  
	t++;  
	  
	if(arguments == "OnOff") //Main Switch  
	{  
		isOScontrol = !isOScontrol;  
	}  
	  
	if(isOScontrol) //Key Control  
	{  
		MainOS();  
	}  
	else //Restore ship control  
	{  
		if(oCockpit.IsFunctional)  
		{  
			oCockpit.ControlThrusters = true;  
			oCockpit.ControlWheels = true;  
		}  
		else  
		{  
			debug = "Cockpit Was Broken";  
		}  
	}  
	  
	if(IsAutoScan) //Automated scanning  
	{  
		if(t%(60*ScanDistance/2000) == 0)  
		{  
			Scan();  
		}  
	}  
	  
	GetMiningShipStatus();//Update ship status  
	CommandMiningShip();  
	  
	MainLCDInfo(); //Display main LCD  
	Echo(debug);//System self-test  
  
}  
  
//The function to get the blocks during initialization  
void GetBlocks()  
{  
	oCockpit = GridTerminalSystem.GetBlockWithName(stMotherShipCockpit) as IMyCockpit;  
	oTargetCamera = GridTerminalSystem.GetBlockWithName(stTargetCamera) as IMyCameraBlock;  
	oMainLCD = GridTerminalSystem.GetBlockWithName(stMainLCD) as IMyTextPanel;  
	if(oCockpit == null)  
	{  
		debug = "Cockpit Not Found";  
		return;  
	}  
	if(oTargetCamera == null)  
	{  
		debug = "Camera Not Found";  
		return;  
	}  
	if(oMainLCD == null)  
	{  
		debug = "LCD Not Found";  
		return;  
	}  
  
	//Initialize target groups  
	TargetInfo = new List<MyDetectedEntityInfo>();  
  
	GetMiningShip();  
	init = true;  
}  
  
//Functions for obtaining unmanned mining ships  
void GetMiningShip()  
{  
	//Get programming blocks  
	osMS_Computer = new List<IMyTerminalBlock>();  
	GridTerminalSystem.SearchBlocksOfName(stMiningShipComputer, osMS_Computer);  
	  
	//Get the nearest remote control block  
	osMS_Remote = new List<IMyTerminalBlock>();  
	  
	//Initialize ship commands, etc.  
	MiningShipCommand = new List<int>();  
	MiningShipStatus = new List<int>();  
	MiningShipPosition = new List<Vector3D>();  
	MiningShipSpeed = new List<Vector3D>();  
	MiningShipTargetPosition = new List<Vector3D>();  
	MiningShipPower = new List<double>();  
	MiningShipBoxContant = new List<string>();  
	MiningShipTargetPositionInfo = new List<MyDetectedEntityInfo>();  
	  
	for(int i = 0; i < osMS_Computer.Count; i ++)  
	{  
		MiningShipCommand.Add(1);//Default docked state  
		MiningShipStatus.Add(0);//Default state unknown  
		MiningShipPosition.Add(new Vector3D());  
		MiningShipSpeed.Add(new Vector3D());  
		MiningShipTargetPosition.Add(new Vector3D());  
		MiningShipTargetPositionInfo.Add(new MyDetectedEntityInfo());  
		MiningShipPower.Add(0);//Default power level 0  
		MiningShipBoxContant.Add("none");  
	}  
	  
	  
	List<IMyTerminalBlock> remotes = new List<IMyTerminalBlock>(); //Temporary remote control block list  
	GridTerminalSystem.GetBlocksOfType<IMyRemoteControl> (remotes);  
  
	if(remotes.Count >= osMS_Computer.Count)  
	{  
	for (int i = 0; i < osMS_Computer.Count; i++)   
    {  
		double currDist = 0;   
		double closestDist = Double.MaxValue;  
		IMyTerminalBlock closestBlock = null;  
		for (int j = 0; j < remotes.Count; j++)  
		{  
			currDist = (osMS_Computer[i].GetPosition() - remotes[j].GetPosition()).Length();   
			if (currDist < closestDist)   
			{  
				closestDist = currDist;   
				closestBlock = remotes[j];   
			}   
		}  
		osMS_Remote.Add(closestBlock);  
    }  
	}  
	else  
	{  
		debug = "MinerShips Remotes Count is Less than It's Computers Count";  
	}  
}  
  
//Functions using camera scan  
void Scan()  
{  
	if(oTargetCamera.IsFunctional)  
	{  
		oTargetCamera.EnableRaycast = true;  
		FoundObjectInfo = oTargetCamera.Raycast(oTargetCamera.AvailableScanRange, 0, 0);  
	}  
	else  
	{  
		debug = "Camera Was Broken";  
	}  
}  
  
//Update mining ship status  
void GetMiningShipStatus()  
{  
	for(int i = 0; i < osMS_Computer.Count; i ++)  
	{  
		if(osMS_Computer[i].IsFunctional && osMS_Computer[i].CustomData.Length > 0)//The mining ship programming block is working fine and has CustomData  
		{  
			//string CustomData = Ship ID(int 0 to few)*Current command(int 1 to 4)*Current Status (int 1 to 3，Stop, in-transit, and arrival, respectively)*Current Coordinates (Vector3D)*Current speed(Vector3D)  
			MiningShipStatus[i] = int.Parse(osMS_Computer[i].CustomData.Split('*')[2]);  
			Vector3D pos = new Vector3D();  
			Vector3D.TryParse(osMS_Computer[i].CustomData.Split('*')[3], out pos);  
			MiningShipPosition[i] = pos;  
			Vector3D speed = new Vector3D();  
			Vector3D.TryParse(osMS_Computer[i].CustomData.Split('*')[4], out speed);  
			MiningShipSpeed[i] = speed;  
			MiningShipPower[i] = double.Parse(osMS_Computer[i].CustomData.Split('*')[5]);  
			MiningShipBoxContant[i] = osMS_Computer[i].CustomData.Split('*')[6];  
		}  
		else //Mining ship programming block damaged  
		{  
			MiningShipStatus[i] = 6; //Damaged status  
			//A possible update is reserved here, this program also acquires the paired remote, if the programming block is damaged this program can forcibly recall the mining ship  
		}  
	}  
}  
  
//Giving commands to mining ships  
void CommandMiningShip()  
{  
	//string arguments = Ship ID(int 0 to few)*(int 1 to 4)*Target coordinates(Vector3D)*Target radius（double）*Mining accuracy(double) *Whether to return automatically 0 No 1 Yes  
	for(int i = 0; i < osMS_Computer.Count; i ++)  
	{  
		string cmd = "";  
		cmd += i.ToString() + "*";  
		cmd += MiningShipCommand[i].ToString() + "*";  
		switch(MiningShipCommand[i]) //Target coordinates  
		{  
			case 1://Stop  
			cmd += new Vector3D().ToString() + "*";  
			break;  
			case 2://Dock  
			cmd += new Vector3D().ToString() + "*";  
			break;  
			case 3://Follow  
			Vector3D ORIGINPOS4 = oCockpit.GetPosition();  
			var FORWARDPOS4 = oCockpit.Position + Base6Directions.GetIntVector(oCockpit.Orientation.TransformDirection(Base6Directions.Direction.Forward));  
			var FORWARD4 = oCockpit.CubeGrid.GridIntegerToWorld(FORWARDPOS4);                                       
			Vector3D forward_v = -Vector3D.Normalize(FORWARD4 - ORIGINPOS4); //This is a vector pointing backwards  
			cmd += (Me.GetPosition() + forward_v*FollowInterval + i*forward_v*FollowInterval).ToString() + "*";  
			break;  
			case 4://Mining  
			cmd += MiningShipTargetPositionInfo[i].Position.ToString() + "*";  
			break;  
		}  
		if(MiningShipTargetPositionInfo[i].EntityId != 0)//Target radius  
		{  
			cmd += Math.Round(Vector3D.Distance(MiningShipTargetPositionInfo[i].BoundingBox.Min, MiningShipTargetPositionInfo[i].BoundingBox.Max),2) + "*";  
		}  
		else  
		{  
			cmd += "0" + "*";  
		}  
		cmd += MiningAccuracy.ToString() + "*"; //Mining accuracy  
		cmd += (IsAutoReturn ? "1" : "0");  
		IMyProgrammableBlock computer = osMS_Computer[i] as IMyProgrammableBlock;  
		computer.TryRun(cmd);  
		Echo(cmd); //Debugging  
	}  
}  
  
//OS control functions  
void MainOS()  
{  
	string arguments = "";  
	//Initialize some loop settings  
	if(ScanDistance > 150000) //Scanning distance  
	{ScanDistance = 500;}  
	if(MiningAccuracy < 5)  
	{MiningAccuracy = 5;}  
	if(MiningAccuracy > 100)  
	{MiningAccuracy = 100;}  
	if(FollowInterval < 25)  
	{FollowInterval = 25;}  
	if(FollowInterval > 1000)  
	{FollowInterval = 1000;}  
	  
	if(oCockpit.IsFunctional)  
	{  
		oCockpit.ControlThrusters = false;  
		oCockpit.ControlWheels = false;  
		WASDControl = oCockpit.MoveIndicator;//Get the main control seat button: X left and right -+, Y up and down +-, Z front and back -+  
	}  
	else  
	{  
		debug = "Cockpit Was Broken";   
		return;  
	}  
	  
	if(WASDControl.X > 0)  
	{arguments = "Right";}  
	else if(WASDControl.X < 0)  
	{arguments = "Left";}  
	else if(WASDControl.Y > 0)  
	{arguments = "Enter";}  
	else if(WASDControl.Y < 0)  
	{arguments = "Back";}  
	else if(WASDControl.Z < 0)  
	{arguments = "Up";}  
	else if(WASDControl.Z > 0)  
	{arguments = "Down";}  
	else {IsFreeInput = true;}  
	  
	if(IsFreeInput)  
	{  
	switch(arguments)  
	{  
		case ("Left"):  
		if(OSPage == 0 && OSSelect_Main > 1)  
		{OSSelect_Main -= 1;}  
		else if(OSPage != 0 && OSPage != 4 && OSSelect_X > 1)  
		{OSSelect_X -= 1;}  
		else if(OSPage == 4)  
		{  
			switch(OSSelect_Y)//Parameter configuration page option modification, 1 following interval, 2 mining accuracy  
			{  
				case 1:  
				FollowInterval -= 25;  
				break;  
				case 2:  
				MiningAccuracy -= 5;  
				break;  
			}  
		}  
		IsFreeInput = false;  
		break;  
		  
		case("Right"):  
		if(OSPage == 0 && OSSelect_Main < 4)//Home Page  
		{OSSelect_Main += 1;}  
		if(OSPage == 1 && OSSelect_X < 4) // Mining ship control page, 1 stop, 2 dock, 3 follow, 4 work  
		{OSSelect_X += 1;}  
		else if(OSPage == 2 && OSSelect_X < 5) //Target control page, 1 add, 2 remove, 3 clear, 4 switch auto-scan, 5 switch auto-scan distance  
		{OSSelect_X += 1;}  
		else if(OSPage == 3 && OSSelect_X < MiningShipBoxContant[OSSelect_Y-1].Split('^').Length - 4)  
		{OSSelect_X += 1;}  
		else if(OSPage == 4)  
		{  
			switch(OSSelect_Y)  
			{  
				case 1:  
				if(FollowInterval < 200)  
					{FollowInterval += 25;}  
				else if(FollowInterval < 1000)  
					{FollowInterval += 100;}  
				break;  
				case 2:  
				MiningAccuracy += 5;  
				break;  
			}  
		}  
		IsFreeInput = false;  
		break;  
		  
		case ("Enter"):  
		if(OSPage == 0) //Click OK from home page  
		{  
			OSSelect_X = 1;  
			OSSelect_Y = 1;  
			OSPage = OSSelect_Main;  
		}  
		else if(OSPage == 1) //Press OK on the mining ship control page  
		{  
			if(OSSelect_Y == 1) //Give commands to all mining ships at the same time  
			{  
				if(OSSelect_X != 4)  
				{  
					for(int i = 0; i < osMS_Computer.Count; i ++)  
					{  
						MiningShipCommand[i] = OSSelect_X;  
					}  
				}  
				else if(OSSelect_X == 4 && SelectTargetInt != -1) //Working instructions  
				{  
					for(int i = 0; i < osMS_Computer.Count; i ++)  
					{  
						MiningShipTargetPositionInfo[i] = TargetInfo[SelectTargetInt];  
						MiningShipCommand[i] = OSSelect_X;  
					}  
				}  
			}  
			else //Single mining ship command  
			{  
				if(OSSelect_X != 4)  
				{  
					MiningShipCommand[OSSelect_Y-2] = OSSelect_X;  
				}  
				else if(OSSelect_X == 4 && SelectTargetInt != -1)  
				{  
					MiningShipTargetPositionInfo[OSSelect_Y-2] = TargetInfo[SelectTargetInt];  
					MiningShipCommand[OSSelect_Y-2] = OSSelect_X;  
				}  
			}  
		}  
		else if(OSPage == 2)//Press OK for the target page  
		{  
			if(OSSelect_X == 1 && FoundObjectInfo.Name == "Asteroid")//Add  
				{ TargetInfo.Add(FoundObjectInfo); }  
			else if(OSSelect_X == 2)//Delete  
				{   
					TargetInfo.RemoveAt(OSSelect_Y - 1);   
					if(OSSelect_Y > 1){OSSelect_Y -= 1;}  
				}  
			else if(OSSelect_X == 3)//清空  
				{ TargetInfo = new List<MyDetectedEntityInfo>(); }  
			else if(OSSelect_X == 4)//Automatic scanning for open hooks  
				{ IsAutoScan = !IsAutoScan; }  
			else if(OSSelect_X == 5)//Modify scanning distance  
			{   
				if(ScanDistance < 3000)  
				{ScanDistance += 500; }  
				else if(ScanDistance < 10000)  
				{ScanDistance += 1000;}  
				else if(ScanDistance < 50000)  
				{ScanDistance += 5000;}  
				else  
				{ScanDistance += 10000;}  
			}  
		}  
		else if(OSPage == 3)  
		{}  
		else if(OSPage == 4 && OSSelect_Y == 3) //Switch automatic return  
		{ IsAutoReturn = !IsAutoReturn;}  
		else if(OSPage == 4 && OSSelect_Y == 4) //Select Reset mining ship and return to the main screen  
		{ GetMiningShip(); OSPage = 0; OSSelect_Main = 1; OSSelect_X = 1; OSSelect_Y = 1;}  
		IsFreeInput = false;  
		break;  
		  
		case ("Back"): //Back to home page  
		OSPage = 0;  
		OSSelect_X = 1;  
		OSSelect_Y = 1;  
		break;  
		  
		case("Up"):  
		if(OSPage != 0 && OSSelect_Y > 1)  
		{OSSelect_Y -= 1;}  
		IsFreeInput = false;  
		break;  
		  
		case("Down"):  
		if(OSPage == 1 && OSSelect_Y < osMS_Computer.Count + 1)  
		{OSSelect_Y += 1;}  
		else if(OSPage == 2 && OSSelect_Y < TargetInfo.Count)  
		{OSSelect_Y += 1;}  
		else if(OSPage == 3 && OSSelect_Y < osMS_Computer.Count)  
		{OSSelect_Y += 1;}  
		else if(OSPage == 4 && OSSelect_Y < 4) //Parameter setting page, 1 follow interval, 2 mining accuracy, 3 auto docking switch after completion, 4 reset mining ship  
		{OSSelect_Y += 1;}  
		IsFreeInput = false;  
		break;  
	}  
	}  
}  
  
//Functions to handle the main LCD display information  
void MainLCDInfo()  
{  
	string info = "";  
	int maxcontentline = 10; //Maximum number of lines to be displayed in the middle content section  
	int startnumber = 0; //Number of first line  
	int endnumber = 0; //Number of last line  
	  
	info += "\n ==================== [ MEA ]  Auto Miner Control Interface ====================\n\n";  
	  
	//Display top navigation  
	if(OSPage == 0)  
	{  
		info += sp7 + (OSSelect_Main == 1 ? "[ Commands ]" : "  Commands  ");  
		info += sp7 + (OSSelect_Main == 2 ? "[ Targets  ]" : "  Targets   ");  
		info += sp7 + (OSSelect_Main == 3 ? "[ States   ]" : "  States    ");  
		info += sp7 + (OSSelect_Main == 4 ? "[ Argument ]" : "  Argument  ");  
	}  
	else  
	{  
		info += sp7 + "  Commands  " + sp7 + "  Targets   " + sp7 + "  States    " + sp7 + "  Argument  ";  
	}  
	info += "\n ----------------------------------------------------------------------------------------------------------------------- \n";  
	  
	//Display the main content  
	switch(OSSelect_Main)  
	{  
		case 1: //Page 1, Mining ship control page  
		info += sp4 + (OSSelect_Y == 1 && OSPage == 1 ? "[ AllShip ]" : "  AllShip  ");  
		info += sp7 + "CMD " + sp7 + "Stat" + sp7 + "Dist" + sp7 + sp7 + "GPS ";  
		  
		//Dealing with page turns  
		startnumber = 0;  
		endnumber = osMS_Computer.Count;  
		if(OSSelect_Y > maxcontentline)  
		{startnumber = OSSelect_Y - maxcontentline;}  
		if(startnumber >= OSSelect_Y)  
		{startnumber = OSSelect_Y;}  
		if(maxcontentline < osMS_Computer.Count)  
		{endnumber = startnumber + maxcontentline - 1;}  
		//End of page turn  
		  
		for(int i = startnumber; i < endnumber; i ++) //Cycle through the list of mining ships and limit the number of displays  
		{  
			info += br + sp4 + sp1 + (OSSelect_Y == i+2 && OSPage == 1 ? "[ " : "  ") + "# " + (i == 1 ? " 1" : i.ToString()) + (OSSelect_Y == i+2 && OSPage == 1 ? " ]" : "  ");  
			info += sp7 + sp2 + (MiningShipCommand[i] == 1 ? "STOP" : (MiningShipCommand[i] == 2 ? "DOCK" : (MiningShipCommand[i] == 3 ? "TAIL" : "WORK")));  
			info += sp1;  
			switch(MiningShipStatus[i])  
			{  
				case 0:  
				info += sp4 + "  ???  " ;  
				break;  
				case 1:  
				info += sp4 + "  Stop  ";  
				break;  
				case 2:  
				info += sp4 + "  OnWay ";  
				break;  
				case 3:  
				info += sp4 + " Arrive ";  
				break;  
				case 4:  
				info += sp4 + "Working ";  
				break;  
				case 5:  
				info += sp4 + "Docking ";  
				break;  
				case 6:  
				info += sp4 + "Damaged ";  
				break;  
			}  
			string dis = Math.Round(Vector3D.Distance(oCockpit.GetPosition(),MiningShipPosition[i]),0).ToString();  
			info += sp4 + sp2 + dis;  
			info += sp7 + Vector3D.Round(MiningShipPosition[i],0).ToString(); //Mining ship coordinates  
		}  
		for(int i = 0; i < maxcontentline - osMS_Computer.Count - 1; i ++) //Middle content area to fill empty lines  
		{  
			info += br;  
		}  
		info += br + " ----------------------------------------------------------------------------------------------------------------------- \n";  
		info += sp4 + (OSSelect_X == 1 && OSPage == 1 ? "[ STOP ]" : "  STOP  ");  
		info += sp7 + (OSSelect_X == 2 && OSPage == 1 ? "[ DOCK ]" : "  DOCK  ");  
		info += sp7 + (OSSelect_X == 3 && OSPage == 1 ? "[FOLLOW]" : " FOLLOW ");  
		info += sp7 + (OSSelect_X == 4 && OSPage == 1 ? "[ WORK ]" : "  WORK  ");  
		info += sp4 + "NowTarget:";  
		if(OSSelect_Y > 1) //Show selected target  
		{  
			if(MiningShipCommand[OSSelect_Y-2] != 4)  
			{  
				info += sp1 + (SelectTargetInt == -1 ? "N/A" : "# " + SelectTargetInt.ToString()); //Show selected target  
			}  
			else if(MiningShipCommand[OSSelect_Y-2] == 4)  
			{  
				info += sp1 + Math.Round(Vector3D.Distance(MiningShipTargetPosition[OSSelect_Y-2], oCockpit.GetPosition()),0).ToString();  
			}  
		}  
		else  
		{  
			info += sp1 + (SelectTargetInt == -1 ? "N/A" : "# " + SelectTargetInt.ToString()); //Show selected target  
		}  
		  
		break;  
		  
		case 2: //Page 2, targets management interface  
		info += sp4 + "Scanning:" + sp4;  
		if(FoundObjectInfo.Name == "Asteroid")  
			{ info += "( Asteroid )"; }  
		else if(FoundObjectInfo.Name != null)  
			{info += "(   Other   )";}  
		else  
			{info += "( Not Found )";}  
		info += sp7 + "Scan Distance:  " + ScanDistance.ToString() + sp4 + "Auto Scan:  " + (IsAutoScan ? "ON" : "OFF");  
		if(FoundObjectInfo.Name == "Asteroid")  
		{  
			info += br + sp10 + "Distance:     ( " + Math.Round(Vector3D.Distance(oCockpit.GetPosition(),FoundObjectInfo.Position),2).ToString() + " )";  
			info += sp10 + "Radius     ( " + Math.Round(Vector3D.Distance(FoundObjectInfo.BoundingBox.Min, FoundObjectInfo.BoundingBox.Max),2).ToString() + " )";  
		}  
		else if(FoundObjectInfo.Name != null)  
			{info += br + sp10 + "Alert: Target is not an asteroid object";}  
		else  
			{info += br;}  
		  
		info += br + sp4 + "Now selected";  
		if(TargetInfo.Count > 0)  
		{  
			SelectTargetInt = OSSelect_Y-1;  
			info += sp4 + "# " + (OSSelect_Y-1).ToString() + sp7 + Math.Round(Vector3D.Distance(oCockpit.GetPosition(),TargetInfo[OSSelect_Y - 1].Position),2).ToString();  
		}  
		else  
		{  
			info += sp7 + "N/A";  
			SelectTargetInt = -1;  
		}  
		info += br + "         -------------------------------------------------------------------------------------------------------         ";  
		  
		//Handling page turns  
		startnumber = 0;  
		endnumber = TargetInfo.Count;  
		maxcontentline -= 4;  
		if(OSSelect_Y > maxcontentline)  
		{startnumber = OSSelect_Y - maxcontentline;}  
		if(startnumber >= OSSelect_Y)  
		{startnumber = OSSelect_Y;}  
		if(maxcontentline < TargetInfo.Count)  
		{endnumber = startnumber + maxcontentline;}  
		//End of page turn  
	  
		info += sp4 + "Numb" + sp10 +  "Dist" + sp10 + "Rad " + sp10 + "GPS ";  
		for(int i = startnumber; i < endnumber; i ++) //Cycle through the list of targets and limit the number of displays  
		{  
			info += br + sp7 + sp1 + (OSSelect_Y-1 == i && OSPage == 2 ? "[ " : "  ") + "# " + (i == 1 ? " 1" : i.ToString()) + (OSSelect_Y-1 == i && OSPage == 2 ? " ]" : "  ");  
			info += sp7 + sp1 + Math.Round(Vector3D.Distance(oCockpit.GetPosition(),TargetInfo[i].Position),2).ToString();  
			info += sp7 + sp1 + Math.Round(Vector3D.Distance(TargetInfo[i].BoundingBox.Min, TargetInfo[i].BoundingBox.Max),2).ToString();  
			info += sp7 + sp1 + Vector3D.Round(TargetInfo[i].Position,0).ToString();  
		}  
		for(int i = 0; i < maxcontentline - TargetInfo.Count; i ++) //Middle content area to fill empty lines  
		{  
			info += br;  
		}  
		  
		info += br + " ----------------------------------------------------------------------------------------------------------------------- \n";  
		info += sp4 + (OSSelect_X == 1 && OSPage == 2 ? "[ ADD  ]" : "  ADD   ");  
		info += sp7 + (OSSelect_X == 2 && OSPage == 2 ? "[ DEL  ]" : "  DEL   ");  
		info += sp7 + (OSSelect_X == 3 && OSPage == 2 ? "[ CLS  ]" : "  CLS   ");  
		info += sp7 + (OSSelect_X == 4 && OSPage == 2 ? "[ AutoScan ]" : "  AutoScan  ");  
		info += sp7 + (OSSelect_X == 5 && OSPage == 2 ? "[ ScanRange]" : "  ScanRange ");  
		break;  
		  
		case 3: //Mining ship status page  
		info += sp4 + "Numb" + sp7 + "CMDs" + sp7 + "Stat" + sp7 + "Dist" + sp7 + sp7 + "GPS ";  
		  
		//Handling page turns  
		startnumber = 0;  
		maxcontentline -= 8;  
		endnumber = osMS_Computer.Count;  
		if(OSSelect_Y > maxcontentline)  
		{startnumber = OSSelect_Y - maxcontentline;}  
		if(startnumber >= OSSelect_Y)  
		{startnumber = OSSelect_Y;}  
		if(maxcontentline < osMS_Computer.Count)  
		{endnumber = startnumber + maxcontentline;}  
		for(int i = startnumber; i < endnumber; i ++) //循环输出飞船列表，限制显示个数  
		{  
			info += br + sp4 + (OSSelect_Y == i+1 && OSPage == 3 ? "[ " : "  ") + "# " + (i == 1 ? " 1" : i.ToString()) + (OSSelect_Y == i+1 && OSPage == 3 ? " ]" : "  ");  
			info += sp7 + (MiningShipCommand[i] == 1 ? "STOP" : (MiningShipCommand[i] == 2 ? "DOCK" : (MiningShipCommand[i] == 3 ? "TAIL" : "WORK")));  
			switch(MiningShipStatus[i])  
			{  
				case 0:  
				info += sp7 + " ???? " ;  
				break;  
				case 1:  
				info += sp7 + " STOP  ";  
				break;  
				case 2:  
				info += sp7 + " OnWay ";  
				break;  
				case 3:  
				info += sp7 + "ARRIVED";  
				break;  
				case 4:  
				info += sp7 + "WORKING";  
				break;  
				case 5:  
				info += sp7 + "DOCKING";  
				break;  
				case 6:  
				info += sp7 + "DAMAGED";  
				break;  
			}  
			string dis = Math.Round(Vector3D.Distance(oCockpit.GetPosition(),MiningShipPosition[i]),0).ToString();  
			info += sp7 + dis;  
			info += sp7 + Vector3D.Round(MiningShipPosition[i],0).ToString(); //Mining ship coordinates  
		}  
		for(int i = 0; i < maxcontentline - osMS_Computer.Count; i ++) //Middle content area to fill empty lines  
		{  
			info += br;  
		}  
		info += br + "         ------------------------------------------------------------------------------------------------------         \n";  
		string lll = "llllllllllllllllllllllllllllllllllllllll";  
		string ooo = "                                        ";  
		int val = (int)(Math.Round(40*MiningShipPower[OSSelect_Y-1],0));  
		info += br + sp4 + "EnergyLeft: [";  
		info += lll.Substring(40 - val) + ooo.Substring(val);  
		info += "]" + br + br;  
		//Show cargo  
		if(MiningShipBoxContant[OSSelect_Y-1].Split('^').Length > 4)  
		{  
			for (int i = OSSelect_X-1; i < MiningShipBoxContant[OSSelect_Y-1].Split('^').Length; i++)  
			{  
				info += sp4 + MiningShipBoxContant[OSSelect_Y-1].Split('^')[i] + br;  
			}  
		}  
		break;  
		  
		case 4: //System parameters setting page  
		info += br + sp7 + (OSSelect_Y == 1 && OSPage == 4 ? "[FOLLOW GAP]" : " FOLLOW GAP ") + sp10 + FollowInterval.ToString() + br;  
		info += br + sp7 + (OSSelect_Y == 2 && OSPage == 4 ? "[ DRILLGAP ]" : "  DRILLGAP  ") + sp10 + MiningAccuracy.ToString() + br;  
		info += br + sp7 + (OSSelect_Y == 3 && OSPage == 4 ? "[ AUTORETN ]" : "  AUTORETN  ") + sp10 + (IsAutoReturn ? "ON" : "OFF") + br;  
		info += br + sp7 + (OSSelect_Y == 4 && OSPage == 4 ? "[ RESET    ]" : "  RESET     ") + br;  
		for (int i = 0; i < maxcontentline - 10; i ++)  
		{  
			info += br;  
		}  
		info += br + " ----------------------------------------------------------------------------------------------------------------------- \n";  
		switch(OSSelect_Y)  
		{  
			case 1:  
			info += sp4 + "The Distance between each miners while following mothership. \n        Choose a suitable value according to the size of your the miners" ;  
			break;  
			case 2:  
			info += sp4 + "The Area which your miner can handle. Larger value need more drills.	\n        [Tips: Seting it below 15 will cause lag]";  
			break;  
			case 3:  
			info += sp4 + "The miner will automatic return to base when:\n        Finished their task, Energy lower than 10%, Cargo more than 95%";  
			break;  
			case 4:  
			info += sp4 + "RESET ALL THE MINERS, use it for adding new miners.\n        Make sure all the miners are docked perfectly, the undocked one will be forgoten";  
			break;  
		}  
		break;  
	}  
	  
	PrintLCD(info, oMainLCD);  
}  
  
//Functions for displaying information to the LCD  
void PrintLCD(string Info, IMyTextPanel LCD)  
{  
	if(LCD.IsFunctional)  
	{  
		LCD.SetValueFloat("FontSize",1f);  
		LCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;  
		LCD.WriteText(Info); 
	}  
	else  
	{  
		debug = "LCD was broken";  
	}  
}  
  
  

