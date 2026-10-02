/*  
[MEA]New Automatic Mining Program_Mother Ship Section  
Authors:Missile Engineering Agency Team（China）  
Contributors: MEA group leader（Mothership Program), zzhou（Mining ship control program), Schwarzengel（main designer), mentong[small miner], as well as other active partners. 
  
[Description]
This program does not need to name any blocks, the only set timer block, you can set its own name, the program will always trigger this block in the work process, the trigger action can also be customized, but only for ordinary action  
including: switch "OnOff_On", "OnOff_Off", timing block "TriggerNow" "Start", sound block "PlaySounds", etc., the specific can be checked the official manual  
This program does not need a timing block to loop to trigger the execution, as long as the mother ship's programming block is working properly, it will automatically run this programming block on its own loop  
  
[Working Logic]  
The program is initialized when it is first detached from the connector, and during initialization it will acquire the following:  
  
Remote control block: get by type, only use the first random, if there is none the program will report an error  
Thruster: get by type, and automatically determine the direction, there must be at least one forwards and backwards, otherwise it will report an error  
Gyroscope: get by type, if there is none, an error will be reported  
Antenna: get by type, only use the first one randomly, if there is none, it will not report an error, it will broadcast its state to the antenna. This function has a switch  
Containers: get by type, the program will overview all the containers, and record the list of items inside, if there is no error will not be reported  
Drill: get by type, the program will automatically turn on all the drills when working, and turn off automatically at other times, you can't manually control the drill switch when the program is running, no error will be reported if there is no drill  
Battery: get by type, the program will iterate through all the batteries and record the current total remaining power, no error will be reported if there is no battery.  
Timer block: get by name, one, you can customize its name and action below.
Sensor block (for sensor mode): get by type, the program will use it for sensor mode - especially mining (change range in all directions to 1 m, except front - in front togle it to meters from sensor to tip of the drill + 1 m (for small ship drill; other drills - increse value until minig ship is mining in straitgh line, without wigling around), no error will be reported if there is no sensor.  
  
[Communication Rules]  
The mining ship writes status to its own CustomData of the programming block.  
0 mining ship ID * 1 mining ship command * 2 mining ship status * 3 mining ship coordinates * 4 mining ship speed * 5 power consumption ratio * item 1 ^ item 2 ^ item 3 *  
  
[Update Description]  
This program since v4.0 version  
v4.1: Fixed the docking process algorithm, now the docking is more intelligent. Fixed a problem that the mining ship program would unlock the connector and re-dock even if it was already docked after it had been commanded when the host gave the docking command.  
v4.2: Fixed a decision of docking process. The previous decision was that if the Yaw and Pitch values of the first random gyroscope in the docking process were less than DockAngularPrecision, then the steering was decided to be completed and the thrusters were turned on, but the case of Roll was ignored and the case of negative numbers was also ignored. Now added  
v4.3: Added a new sensor mode, get by type, when the sensor exists, works properly, and SensorMode is on, a determination is made under both Work and Dock commands, and if the sensor is detected, the remote control of the remote block is forced off and the back push and drill are turned on  
v4.4: Modified the determination of the containers and the battery, so that no error will be reported if it is damaged or lost in the middle  
In sensor mode, the workflow has been changed. Under the work command, once the sensor is detected, the ship will turn off the remote control block's path navigation and force the control thruster forward until the sensor does not detect the object  
*/  
  
string stAcitionBlock = "Act_Timer";//Timing blocks triggered at work with customizations  
string ActionInWork = "Start";  
  
//Customizable function switches  
bool SensorMode = true;  
  
//Customizable variables  
double RadiuRatio = 0.3; //Asteroid radius calculation parameter, turning it down will reduce the working radius  
double DockDistance = 20; //Automatic docking ready distance  
double DockSpeed = 8; //Automatic docking speed limit  
float WorkSpeed = 6f; //The working speed, value must be connected to a f  
float MoveSpeed = 100f; //Daily cruising speed, value must be connected to a f
float DockAngularPrecision = 0.008f; //Docking exclusive, when the gyroscope torque is less than this value, it is judged that the pointing adjustment is over, and the thrusters are turned on to start the connection.  
bool NeedControlAnt = true; //If true, broadcast the status to the antenna  
//Docking exclusive, when the small ship runs to the docking ready position more than half the length of the docking distance after (that is, the second half of the reverse), start to judge the mothership connector to the mining ship connector and the mothership connector positive front normal vector  
//The degree of parallelism of these two vectors, when less than this value, it is judged to be parallel, otherwise it means that the deviation is too large and needs to be re-docked  
double DockLinePrecision = 0.05; //Accuracy in the order of 10^-3, the larger this value the lower the docking requirements  
double MoveAccuracy = 4; //Navigation accuracy, distance less than this value is determined to reach the specified location  
  
  
//Block initialization information  
IMyShipConnector Miner_Connector;  
IMyShipConnector MotherShip_Connector;  
IMyRemoteControl Remote;  
IMyTerminalBlock AcitionBlock;  
IMyTerminalBlock Miner_Ant;  
IMySensorBlock Sensor;  
List<IMyTerminalBlock> AllThrusts;  
List<IMyTerminalBlock> ForwardThrusts;  
List<IMyTerminalBlock> BackwardThrusts;  
List<IMyTerminalBlock> Gyroscopes;  
List<IMyTerminalBlock> Drill = new List<IMyTerminalBlock>();  
List<IMyTerminalBlock> Battery = new List<IMyTerminalBlock>();  
List<IMyTerminalBlock> Boxs = new List<IMyTerminalBlock>();  
  
string[] gyroYawField = null;  
string[] gyroPitchField = null;  
string[] gyroRollField = null;  
float[] gyroYawFactor = null;  
float[] gyroPitchFactor = null;  
float[] gyroRollFactor = null;  
  
//Azimuth calculations information  
MatrixD refWorldMatrix; //Ship Matrix  
const float GYRO_FACTOR = (float)(Math.PI / 30);  
Vector3D Y_VECTOR = new Vector3D(0, -1, 0);  
Vector3D Z_VECTOR = new Vector3D(0, 0, -1);  
Vector3D POINT_ZERO = new Vector3D(0, 0, 0);  
  
double targetYawAngle = 0;  
double targetPitchAngle = 0;  
//double targetRollAngle = 0;  
  
//Work path calculation information  
Vector3D pos1 = new Vector3D();  
Vector3D pos2 = new Vector3D();  
Vector3D pos3 = new Vector3D();  
Vector3D pos4 = new Vector3D();  
  
  
//Command specific variables  
string AllArgumentsInfo = "0*1*{X:0 Y:0 Z:0}*0*0";//Default command  
int ShipId = 0;  
int ShipCommand = 1;  
Vector3D ShipTargetPosition = new Vector3D();  
double ShipTargetRadius = 0;  
int ShipMiningAccuracy = 0;  
  
//Mining ship Status Category  
int ShipStatus = 1; //Mining ship status: 0 unknown, 1 stopped, 2 in transit, 3 arrived, 4 working, 5 docking, 6 damaged  
bool IsTargetSet = false;  
int DockStep = 0; //Docked only; 0 means locked  
  
//Main function logic classes  
int init = 0; //0, get connector and mothership connector and standby, 1, detect received command and connector unlock, 2 get remote and timer block, 3 initialize gyroscope and thruster according to remote, 4 normal working state  
string debug = "Normal";  
int t = 0;  
  
void Main(string arguments)  
{  
	ComputerInit(arguments); //Initialize all, when init=4 then it is normal working state  
  
	if(init == 4) //Start working normally  
	{  
		t ++; //Clocks  
  
		refWorldMatrix = Remote.WorldMatrix; //Get World Matrix  
		//Initialize certain states  
		if(ShipCommand != 4)//Not at work  
		{  
			OpenBlocks(Drill,false);  
			Remote.ApplyAction("CollisionAvoidance_On");  
		}  
		Remote.SetValueFloat("SpeedLimit",MoveSpeed); //Restore cruising speed  
  
		if(arguments.Length > 0) //Update command  
		{  
			if(int.Parse(arguments.Split('*')[1]) != ShipCommand) //Determine if the command is updated and use it to reset IsTargetSet  
			{  
				IsTargetSet = false;  
				DockStep = 1;  
			}  
			AllArgumentsInfo = arguments;  
		}  
  
		//Split and process instructions  
		ShipId = int.Parse(AllArgumentsInfo.Split('*')[0]);  
		ShipCommand = int.Parse(AllArgumentsInfo.Split('*')[1]);  
		Vector3D.TryParse(AllArgumentsInfo.Split('*')[2], out ShipTargetPosition);  
		ShipTargetRadius = double.Parse(AllArgumentsInfo.Split('*')[3]);  
		ShipMiningAccuracy = int.Parse(AllArgumentsInfo.Split('*')[4]);  
  
		switch(ShipCommand) //Execute command actions  
		{  
		case 1: //Stop  
			Remote.SetAutoPilotEnabled(false);  
			SetGyroOverride(false);  
			SetThrustOverride(ForwardThrusts,0);  
			SetThrustOverride(BackwardThrusts,0);  
			ShipStatus = 1;  
			break;  
  
		case 2: //Dock  
			Dock();  
			break;  
  
		case 3://Follow  
			if(t%180 == 0 && !CheckArrival(ShipTargetPosition))  
			{  
				IsTargetSet = false;  
			}  
			GoToPosition(ShipTargetPosition);  
			if(CheckArrival(ShipTargetPosition))  
			{  
				ShipStatus = 3;  
			}  
			else  
			{  
				ShipStatus = 2;  
			}  
			break;  
  
		case 4://Work  
			LockConnector(false);//Unlock connector  
			Work();  
			break;  
		}  
  
	}  
  
	WriteCustomData(); //Write status into panel  
	if(NeedControlAnt) //Broadcast to the antenna  
	{  
		WriteAnt();  
	}  
	Echo(debug);  
	//Debugging  
	Echo("Ship ID: " + ShipId.ToString());  
	Echo("Command: " + ShipCommand.ToString());  
	Echo("Status: " + ShipStatus.ToString());  
}  
  
void ComputerInit(string arguments)//Pass in arguments to detect the command  
{  
  
	if(init == 0)  
	{  
		List<IMyTerminalBlock> AllConnectorsTemp = new List<IMyTerminalBlock>();  
		GridTerminalSystem.GetBlocksOfType<IMyShipConnector> (AllConnectorsTemp);  
  
		Miner_Connector = GetNearestTypeBlock(AllConnectorsTemp, Me) as IMyShipConnector; //Get the nearest connector  
		if(Miner_Connector != null)  
		{  
			MotherShip_Connector = Miner_Connector.OtherConnector;  
		}  
  
		if(Miner_Connector != null && MotherShip_Connector != null) //Successfully acquired the connector  
		{  
			init = 1;  
		}  
		else  
		{  
			debug = "Connector not acquired";  
		}  
	}  
	else if(init == 1)  
	{  
		if(arguments.Length > 0 && arguments.Split('*')[1] != "1" && arguments.Split('*')[1] != "2" && (int)Miner_Connector.Status == 2)//There is a command, and it is not a stop or dock command, and the connector is locked  
		{  
			Miner_Connector.ApplyAction("SwitchLock");//Unlock  
		}  
		if((int)Miner_Connector.Status != 2) //Unlocked  
		{  
			init = 2;  
		}  
	}  
	else if(init == 2 && GetRemoteAndActionBlock()) //Get all other function blocks  
	{  
		init = 3;  
		refWorldMatrix = Remote.WorldMatrix; //Get World Matrix  
	}  
	else if(init == 3 && InitThrustsAndGryoscopes())  
	{  
		init = 4; //Successful initialization of all blocks, including thrusters and gyroscopes  
	}  
}  
  
bool GetRemoteAndActionBlock()  
{  
	List<IMyTerminalBlock> AllRemoteTemp = new List<IMyTerminalBlock>();  
	GridTerminalSystem.GetBlocksOfType<IMyRemoteControl> (AllRemoteTemp);//Get remote control block  
  
	AcitionBlock = GridTerminalSystem.GetBlockWithName(stAcitionBlock);//Get timer block  
  
	List<IMyTerminalBlock> AllAntTemp = new List<IMyTerminalBlock>();  
	GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna> (AllAntTemp);  
  
	Drill = new List<IMyTerminalBlock>(); //Get the drills  
	GridTerminalSystem.GetBlocksOfType<IMyShipDrill> (Drill);  
  
	Battery = new List<IMyTerminalBlock>();//Get batteries  
	GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock> (Battery);  
  
	Boxs = new List<IMyTerminalBlock>(); //Get containers  
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer> (Boxs);  
	  
	List<IMyTerminalBlock> Sensors_Temp = new List<IMyTerminalBlock>(); //Get gyroscopes  
	GridTerminalSystem.GetBlocksOfType<IMySensorBlock> (Sensors_Temp);  
	if(Sensors_Temp[0] != null)  
	{  
		Sensor = Sensors_Temp[0] as IMySensorBlock;  
	}  
  
	if(AllAntTemp.Count > 0)  
	{  
		Miner_Ant = AllAntTemp[0];  
	}  
  
	if(AllRemoteTemp.Count > 0)  
	{  
		Remote = AllRemoteTemp[0] as IMyRemoteControl;  
	}  
	else  
	{  
		debug = "Remote control block not found";  
		return false;  
	}  
	return true;  
}  
  
bool InitThrustsAndGryoscopes()  
{  
	//Handling of thrusters  
	ForwardThrusts = new List<IMyTerminalBlock>();  
	BackwardThrusts = new List<IMyTerminalBlock>();  
	AllThrusts = new List<IMyTerminalBlock>();  
	GridTerminalSystem.GetBlocksOfType<IMyThrust> (AllThrusts);  
  
	for(int i = 0; i < AllThrusts.Count; i ++) //Filtering to match forward and backward thrusters  
	{  
		Base6Directions.Direction BlocksForward = AllThrusts[i].WorldMatrix.GetClosestDirection(refWorldMatrix.Forward);  
		switch(BlocksForward)  
		{  
		case Base6Directions.Direction.Forward:  
			BackwardThrusts.Add(AllThrusts[i]);  
			break;  
		case Base6Directions.Direction.Backward:  
			ForwardThrusts.Add(AllThrusts[i]);  
			break;  
		}  
	}  
	if(ForwardThrusts.Count == 0 || BackwardThrusts.Count == 0 )  
	{  
		debug = "No forward and backward thrusters detected";  
		return false;  
	}  
  
	//Handling of gyroscopes  
	Gyroscopes = new List<IMyTerminalBlock>();  
	GridTerminalSystem.GetBlocksOfType<IMyGyro> (Gyroscopes);  
	if(Gyroscopes.Count > 0)  
	{  
		gyroYawField = new string[Gyroscopes.Count];  
		gyroPitchField = new string[Gyroscopes.Count];  
		gyroYawFactor = new float[Gyroscopes.Count];  
		gyroPitchFactor = new float[Gyroscopes.Count];  
		gyroRollField = new string[Gyroscopes.Count];  
		gyroRollFactor = new float[Gyroscopes.Count];  
		for (int i = 0; i < Gyroscopes.Count; i++)  
		{  
			Base6Directions.Direction gyroUp = Gyroscopes[i].WorldMatrix.GetClosestDirection(refWorldMatrix.Up);  
			Base6Directions.Direction gyroLeft = Gyroscopes[i].WorldMatrix.GetClosestDirection(refWorldMatrix.Left);  
			Base6Directions.Direction gyroForward = Gyroscopes[i].WorldMatrix.GetClosestDirection(refWorldMatrix.Forward);  
  
			switch (gyroUp)  
			{  
			case Base6Directions.Direction.Up:  
				gyroYawField[i] = "Yaw";  
				gyroYawFactor[i] = GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Down:  
				gyroYawField[i] = "Yaw";  
				gyroYawFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Left:  
				gyroYawField[i] = "Pitch";  
				gyroYawFactor[i] = GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Right:  
				gyroYawField[i] = "Pitch";  
				gyroYawFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Forward:  
				gyroYawField[i] = "Roll";  
				gyroYawFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Backward:  
				gyroYawField[i] = "Roll";  
				gyroYawFactor[i] = GYRO_FACTOR;  
				break;  
			}  
  
			switch (gyroLeft)  
			{  
			case Base6Directions.Direction.Up:  
				gyroPitchField[i] = "Yaw";  
				gyroPitchFactor[i] = GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Down:  
				gyroPitchField[i] = "Yaw";  
				gyroPitchFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Left:  
				gyroPitchField[i] = "Pitch";  
				gyroPitchFactor[i] = GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Right:  
				gyroPitchField[i] = "Pitch";  
				gyroPitchFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Forward:  
				gyroPitchField[i] = "Roll";  
				gyroPitchFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Backward:  
				gyroPitchField[i] = "Roll";  
				gyroPitchFactor[i] = GYRO_FACTOR;  
				break;  
			}  
  
			switch (gyroForward)  
			{  
			case Base6Directions.Direction.Up:  
				gyroRollField[i] = "Yaw";  
				gyroRollFactor[i] = GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Down:  
				gyroRollField[i] = "Yaw";  
				gyroRollFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Left:  
				gyroRollField[i] = "Pitch";  
				gyroRollFactor[i] = GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Right:  
				gyroRollField[i] = "Pitch";  
				gyroRollFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Forward:  
				gyroRollField[i] = "Roll";  
				gyroRollFactor[i] = -GYRO_FACTOR;  
				break;  
			case Base6Directions.Direction.Backward:  
				gyroRollField[i] = "Roll";  
				gyroRollFactor[i] = GYRO_FACTOR;  
				break;  
			}  
			Gyroscopes[i].ApplyAction("OnOff_On");  
			SetGyroOverride(false);  
		}  
	}  
	else  
	{  
		debug = "No gyroscopes detected";  
		return false;  
	}  
	return true;  
}  
//Function to get the distance to the nearest block  
IMyTerminalBlock GetNearestTypeBlock(List<IMyTerminalBlock> Input_BlockList, IMyTerminalBlock SingleBlock)  
{  
	IMyTerminalBlock closestBlock = null;  
	double currDist = 0;  
	double closestDist = Double.MaxValue;  
	for (int i = 0; i < Input_BlockList.Count; i++)  
	{  
		currDist = (Input_BlockList[i].GetPosition() - SingleBlock.GetPosition()).Length();  
		if (currDist < closestDist)  
		{  
			closestDist = currDist;  
			closestBlock = Input_BlockList[i];  
		}  
	}  
	return closestBlock;  
}  
  
bool CheckArrival(Vector3D pos)  
{  
	if(Vector3D.Distance(Remote.GetPosition(), pos) <= MoveAccuracy)  
	{  
		return true;  
	}  
  
	return false;  
}  
  
void CheckSensor() //Detects if the probe detects, and if so forces a push for getting out of the asteroid's interior  
{  
	if(Sensor != null && Sensor.IsFunctional && SensorMode)  
	{  
		if(Sensor.IsActive)  
		{  
			Remote.SetAutoPilotEnabled(false);  
			SetThrustOverride(ForwardThrusts,(float)(1 - Remote.GetShipSpeed()/WorkSpeed)*100f);  
			OpenBlocks(Drill,true);  
		}  
		else  
		{  
			Remote.SetAutoPilotEnabled(true);  
			SetThrustOverride(ForwardThrusts,0);  
			OpenBlocks(Drill,false);  
		}  
	}  
}  
  
void GoToPosition(Vector3D pos)  
{  
	LockConnector(false);//Unlock connector  
	OpenBlocks(AllThrusts,true);  
	OpenBlocks(Gyroscopes,true);  
	SetGyroOverride(false);  
	SetThrustOverride(ForwardThrusts,0);  
	SetThrustOverride(BackwardThrusts,0);  
	if(!IsTargetSet)  
	{  
		Remote.ClearWaypoints();  
		Remote.AddWaypoint(pos, "Pos");  
		Remote.AddWaypoint(pos, "Pos");  
		Remote.SetAutoPilotEnabled(true);  
		IsTargetSet = true;  
	}  
	//Sensor section  
	CheckSensor();  
}  
  
void Dock()  
{  
	OpenBlocks(AllThrusts,true);  
	OpenBlocks(Gyroscopes,true);  
	//Calculate the forward coordinate vector of the corresponding connector  
	Vector3D ORIGINPOS4 = MotherShip_Connector.GetPosition();  
	var FORWARDPOS4 = MotherShip_Connector.Position + Base6Directions.GetIntVector(MotherShip_Connector.Orientation.TransformDirection(Base6Directions.Direction.Forward));  
	var FORWARD4 = MotherShip_Connector.CubeGrid.GridIntegerToWorld(FORWARDPOS4);  
	Vector3D FrontConnectorV =  Vector3D.Normalize(FORWARD4 - ORIGINPOS4);  
	Vector3D StepPos_1 = MotherShip_Connector.GetPosition() + FrontConnectorV * DockDistance; //This is the position 100 meters directly in front of the mother ship connector  
	double NowSpeed = Remote.GetShipSpeed();  
	if((int)Miner_Connector.Status == 2)//Locked, no docking required  
	{  
		DockStep = 0;  
	}  
	if(DockStep == 1)  
	{  
		//First turn off the gyroscope, thrusters, and set the waypoint to 100 meters before the connector, i.e. StepPos1  
		SetGyroOverride(false);  
		SetThrustOverride(ForwardThrusts,0);  
		SetThrustOverride(BackwardThrusts,0);  
  
		GoToPosition(StepPos_1);  
		if(t%120 == 0)//Dynamic reset of target point  
		{  
			IsTargetSet = false;  
		}  
		ShipStatus = 2;  
		  
		//Sensor section  
		CheckSensor();  
	}  
	if(CheckArrival(StepPos_1))//The point of 100 meters is reached step = 1
	{  
		Remote.SetAutoPilotEnabled(false); //Make sure to turn off the remote control here  
		Remote.ClearWaypoints();  
		DockStep = 2;  
		ShipStatus = 5;  
	}  
	if(DockStep == 2)  
	{  
		//Here the world matrix of the remote block is calculated to determine the orientation, and then the direction vector of the connector and the mothership connector is calculated  
		MatrixD refLookAtMatrix = MatrixD.CreateLookAt(new Vector3D(0,0,0), refWorldMatrix.Forward, refWorldMatrix.Up);  
		Vector3D targetVector = Vector3D.Normalize(Vector3D.TransformNormal(Miner_Connector.GetPosition() - MotherShip_Connector.GetPosition(), refLookAtMatrix));//Derive the target orientation unit vector  
  
		AimAtTarget(targetVector);  
		SetGyroOverride(true);//Turn on gyroscope override  
		//Detect gyroscope override values and determine steering completion  
		double gyro_Yaw = 0;  
		double gyro_Pitch = 0;  
		double gyro_Roll = 0;  
		for(int i = 0; i < Gyroscopes.Count; i ++)  
		{  
			if(Gyroscopes[i].IsFunctional)  
			{  
				var gyroscope = Gyroscopes[i]  as IMyGyro;  
				gyro_Yaw += Math.Abs(gyroscope.Yaw);  
				gyro_Pitch += Math.Abs(gyroscope.Pitch);  
				gyro_Roll += Math.Abs(gyroscope.Roll);  
			}  
		}  
		gyro_Yaw = gyro_Yaw/Gyroscopes.Count;  
		gyro_Pitch = gyro_Pitch/Gyroscopes.Count;  
		gyro_Roll = gyro_Roll/Gyroscopes.Count;  
		if(gyro_Yaw <= DockAngularPrecision && gyro_Pitch <= DockAngularPrecision && gyro_Roll <= DockAngularPrecision) //Gyroscope pointing at the target is complete, turn on the thrusters  
		{  
			SetThrustOverride(BackwardThrusts,((float)(1 - NowSpeed/DockSpeed)*100f));  
		}  
		//Detect the slope, when the two connectors are close to less than 10 meters, determine the line slope, if it is too large, revert to step 1 to correct  
		if(Vector3D.Distance(Miner_Connector.GetPosition(),StepPos_1) > 0.5*DockDistance)  
		{  
			Vector3D mothershipconnectorV = Vector3D.Normalize(StepPos_1 - MotherShip_Connector.GetPosition());  
			Vector3D minershipconnectorV = Vector3D.Normalize(Miner_Connector.GetPosition() - MotherShip_Connector.GetPosition());  
			double LineError = (mothershipconnectorV + minershipconnectorV).Length() - (mothershipconnectorV.Length() + minershipconnectorV.Length()); //The difference between the positive front unit vector of the mothership connector, and the connector-to-connector unit vector of the mothership connector, with a normal accuracy of +-0.0005 or less  
			if(Math.Abs(LineError) >= DockLinePrecision)  
			{  
				DockStep = 1;  
			}  
		}  
		LockConnector(true);//Locking connector  
	}  
	//Testing  
  
	if(DockStep == 0)  
	{  
		SetGyroOverride(false);  
		SetThrustOverride(AllThrusts,0);  
		OpenBlocks(AllThrusts,false);  
		OpenBlocks(Gyroscopes,false);  
		Remote.SetAutoPilotEnabled(false);  
		Remote.ClearWaypoints();  
	}  
  
}  
  
void Work()  
{  
	if(ShipTargetRadius/ShipMiningAccuracy <= 2)  
	{  
		debug = "Mining target diameter [ " + ShipTargetRadius.ToString() + " ] Too small, turn down the mining accuracy or choose a larger target";  
		return;  
	}  
  
	//Unlock thrusters and gyroscopes control, detect connector lockout safety  
	OpenBlocks(AllThrusts,true);  
	OpenBlocks(Gyroscopes,true);  
	LockConnector(false);//Unlock connector  
	SetGyroOverride(false);  
	SetThrustOverride(AllThrusts,0);  
  
	if(!IsTargetSet)  
	{  
		List <Vector3D> Route = new List<Vector3D>();  
  
		Vector3D pos0 = ShipTargetPosition;  
		double radius = ShipTargetRadius * 0.5 * RadiuRatio; // Note that the double RadiuRatio here is the radius calculation factor, the default value should be 0.5, please put this variable at the top to initialize it for player customization  
		int steplength = ShipMiningAccuracy;  
  
		//Constructing parallel hexahedral containers  
		pos1 = pos0 + new Vector3D(-radius,-radius,-radius);  
		pos2 = pos0 + new Vector3D(radius,-radius,-radius);  
		pos3 = pos0 + new Vector3D(radius,radius,-radius);  
		pos4 = pos0 + new Vector3D(radius,radius,radius);  
  
		Vector3D VCpos42 = Vector3D.Normalize(pos4 - pos2);  
  
		for(int k = 0; k <= Math.Round(Vector3D.Distance(pos4 , pos2),0); k += steplength)  
		{  
			for(int h = steplength; h <= Math.Round(Vector3D.Distance(pos3 , pos2),0); h += steplength)  
			{  
				Vector3D move = new Vector3D();  
				if(h%(2 * steplength) == 0)  
				{  
					move = pos2 + (h * Vector3D.Normalize(pos3 - pos2));  
				}  
				else  
				{  
					move = pos1 + (h * Vector3D.Normalize(pos3 - pos2));  
				}  
				Route.Add(move);  
			}  
			pos1 += (steplength * VCpos42);  
			pos2 += (steplength * VCpos42);  
			pos3 += (steplength * VCpos42);  
		}  
  
		Remote.ClearWaypoints();  
		for(int i = 0; i < Route.Count; i ++)  
		{  
			Remote.AddWaypoint(Route[i], ("WorkPos" + i.ToString()));  
		}  
		Remote.SetAutoPilotEnabled(true);  
		IsTargetSet = true;  
	}  
	//The following asteroid forced advance is detected in probe mode  
	if(SensorMode && Sensor.IsFunctional && Sensor.IsActive)  
	{  
		Remote.SetAutoPilotEnabled(false);  
		SetThrustOverride(ForwardThrusts,((float)(1 - Remote.GetShipSpeed()/WorkSpeed)*100f));  
	}  
	else  
	{  
		SetThrustOverride(ForwardThrusts,0);  
		Remote.SetAutoPilotEnabled(true);  
	}  
  
	//The following determines the status and executes the work timer block  
	OpenBlocks(Drill,true);  
	List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>(); //Get remote waypoints  
	Remote.GetWaypointInfo(waypoints);  
	if(Vector3D.Distance(Remote.GetPosition(), ShipTargetPosition) <= 0.5*ShipTargetRadius) //At work  
	{  
		Remote.SetValueFloat("SpeedLimit" , WorkSpeed); //Set working speed limit  
		Remote.ApplyAction("CollisionAvoidance_Off");//Close collision avoidance  
		ShipStatus = 4;  
		if(AcitionBlock != null) //Fast trigger action  
		{  
			AcitionBlock.ApplyAction(ActionInWork);  
		}  
	}  
	else if(CheckArrival(waypoints[waypoints.Count - 1].Coords))  
	{  
		ShipStatus = 3;   //Arrival  
	}  
	else  
	{  
		ShipStatus = 2;   //In transit  
	}  
  
}  
  
//Write status to CustomData  
void WriteCustomData()  
{  
	float PowerMax = 0;  
	float PowerUser = 0;  
	for(int i = 0; i < Battery.Count; i ++)  
	{  
		var battery = Battery[i] as IMyBatteryBlock;  
		PowerMax += battery.MaxStoredPower;  
		PowerUser += battery.CurrentStoredPower;  
	}  
	if(PowerMax == 0)  
	{  
		PowerMax = 1;  
	}  
	string customdata = "";  
	customdata += ShipId.ToString() + "*";  
	customdata += ShipCommand.ToString() + "*";  
	customdata += ShipStatus.ToString() + "*";  
	customdata += Me.GetPosition().ToString() + "*";  
	if(Remote != null)  
	{  
		customdata += Remote.GetShipVelocities().LinearVelocity.ToString() + "*";  
	}  
	else  
	{  
		customdata += new Vector3D() + "*";  
	}  
	customdata += (PowerUser/PowerMax).ToString() + "*"; //电量系数  
  
	//Write to inventory  
	for(int i = 0; i < Boxs.Count; i ++)  
	{  
		var Items = new List<MyInventoryItem>();
Boxs[i].GetInventory(0).GetItems(Items, null);  
		for(int j = 0; j < Items.Count; j ++)  
		{  
			customdata += Items[j].ToString() + "^";  
		}  
	}  
	customdata += "*";//Get end of Inventory  
	Me.CustomData = customdata;  
}  
  
void WriteAnt()  
{  
	if(Miner_Ant != null)  
	{  
		string info = "\n";  
		info += "ID :" + ShipId.ToString();  
		switch(ShipCommand)  
		{  
		case 1:  
			info += "    Stop\n";  
			break;  
		case 2:  
			info += "    Dock\n";  
			break;  
		case 3:  
			info += "    Follow\n";  
			break;  
		case 4:  
			info += "    Work\n";  
			break;  
		}  
		switch(ShipStatus)  
		{  
		case 0:  
			info += "Status: Unknown\n";  
			break;  
		case 1:  
			info += "Status: Stopped\n";  
			break;  
		case 2:  
			info += "Status: On Way\n";  
			break;  
		case 3:  
			info += "Status: Arrived\n";  
			break;  
		case 4:  
			info += "Status: Working\n";  
			break;  
		case 5:  
			info += "Status: Docking\n";  
			break;  
		}  
		Miner_Ant.CustomName = (info);  
	}  
}  
  
//Control gyroscope aiming associated functions  
void AimAtTarget(Vector3D targetVector)//The third step, use this function to convert the orientation vector into the gyroscope Yaw and Pitch angle, this function can be put within the PID control, deflection coefficient, etc. can be handled here  
{  
//---------- Activate Gyroscopes To Turn Towards Target ----------  
  
	Vector3D yawVector = new Vector3D(targetVector.GetDim(0), 0, targetVector.GetDim(2));  
	Vector3D pitchVector = new Vector3D(0, targetVector.GetDim(1), targetVector.GetDim(2));  
	yawVector.Normalize();  
	pitchVector.Normalize();  
  
	targetYawAngle = Math.Acos(yawVector.Dot(Z_VECTOR)) * GetMultiplierSign(targetVector.GetDim(0));  
	targetPitchAngle = Math.Acos(pitchVector.Dot(Z_VECTOR)) * GetMultiplierSign(targetVector.GetDim(1));  
  
//---------- Set Gyroscope Parameters ----------  
  
	SetGyroYaw(targetYawAngle);  
	SetGyroPitch(targetPitchAngle);  
}  
  
//The following are the gyroscopes and thrusters and connector control functions  
  
void OpenBlocks(List<IMyTerminalBlock> Blocks, bool onoff)  
{  
	string action = (onoff ? "OnOff_On" : "OnOff_Off");  
	for(int i = 0; i < Blocks.Count; i ++)  
	{  
		Blocks[i].ApplyAction(action);  
	}  
}  
  
void LockConnector(bool needlock)  
{  
	if(needlock)  
	{  
		if((int)Miner_Connector.Status != 2)  
		{  
			Miner_Connector.ApplyAction("SwitchLock");  
		}  
	}  
	else  
	{  
		if((int)Miner_Connector.Status == 2)  
		{  
			Miner_Connector.ApplyAction("SwitchLock");  
		}  
	}  
}  
  
void SetThrustOverride(List<IMyTerminalBlock> Blocks, float x)  
{  
	for(int i = 0; i < Blocks.Count; i ++)  
	{  
		Blocks[i].SetValue("Override",x);  
	}  
}  
  
int GetMultiplierSign(double value) //Calculate positive and negative  
{  
	return (value < 0 ? -1 : 1);  
}  
  
void SetGyroOverride(bool bOverride)  
{  
	for (int i = 0; i < Gyroscopes.Count; i++)  
	{  
		if (((IMyGyro)Gyroscopes[i]).GyroOverride != bOverride)  
		{  
			Gyroscopes[i].ApplyAction("Override");  
		}  
	}  
}  
  
void SetGyroYaw(double yawRate)  
{  
	for (int i = 0; i < Gyroscopes.Count; i++)  
	{  
		Gyroscopes[i].SetValue(gyroYawField[i], (float)yawRate * gyroYawFactor[i]);  
	}  
}  
  
void SetGyroPitch(double pitchRate)  
{  
	for (int i = 0; i < Gyroscopes.Count; i++)  
	{  
		Gyroscopes[i].SetValue(gyroPitchField[i], (float)pitchRate * gyroPitchFactor[i]);  
	}  
}