	/*
	Blargmode's Fancy Flight Info (FFI) (V4.0.5, 2021-01-23)

	This script gives you free hands in creating a fancy cockpit with data about your ship. 
	When this project started, I intended to replace the awful hud, then the hud got better. Now it's mostly for looks. 


	There's no settings in this file.

	__/ Setup \__

	• Make sure there's a cockpit/flight seat on the ship. The script uses it to gather a lot of the data and for figuring 
	  out which way is down. If you want a specific block to be used, check the "main cockpit" checkbox.


	• Add the script to a programmable block. 
	
	__/ Creating panels \__

	This place is not good for describing this. Go to the workshop for detailed info. 

	Setup guide: http://blargmode.se/ffi-guide/


	If you can't visit the guide, there is a way.
	You can figure out a whole lot in-game as well if you're a bit crafty. Start by loading a preload like this:
	1. Type the following 3 lines in Custom Data of a block with screens, e.g. an LCD panel.

	   [FFI 1]
	   screen=1
	   preload=speed
	2. Type "update" in the argument box of the programmable block and press run.

	This loads a setup into Custom Data of the LCD. If you misspell speed when doing that, an error message in the programmable 

	block will show you all available preloads. Looking at them and experimenting a bit should give you a pretty good idea of 
	how to make your own panel.
	Misspelling things in general can be a good idea since there's a lot of info hidden in error messages. 


	Good luck!











































	*/

class DataAltitude:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=10000;public string Unit{get;private set;}="m";IMyShipController Controller;
double val;public DataAltitude(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){if(Controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out val)){if(val !=Value)
{Value=val;return true;}}}return false;}}
class DataAltitudeSea:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=10000;public string Unit{get;private set;}="m";IMyShipController Controller;
double val;public DataAltitudeSea(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){if(Controller.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out val))
{if(val !=Value){Value=val;return true;}}}return false;}}
class DataBatteryCharge:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="Wh";List<IMyBatteryBlock>batteries=new List<IMyBatteryBlock>();
double val;double total;public DataBatteryCharge(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyBatteryBlock){batteries.Add(block as IMyBatteryBlock);total+=batteries[batteries.Count-1].MaxStoredPower*1000000;
}}Max=total;}public bool Update(){val=0;for(int i=0;i<batteries.Count;i++){val+=batteries[i].CurrentStoredPower*1000000;}if(val !=Value){Value=val;return true;}return false;}}
class DataConnectors:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="Wh";List<IMyShipConnector>connectors=new List<IMyShipConnector>();
double val;public DataConnectors(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyShipConnector){connectors.Add(block as IMyShipConnector);}}Max=connectors.Count;}public bool Update()
{val=0;for(int i=0;i<connectors.Count;i++){if(connectors[i].Status==MyShipConnectorStatus.Connected)val++;}if(val !=Value){Value=val;return true;}return false;}}
class DataDampeners:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=1;public string Unit{get;private set;}="-";IMyShipController Controller;
double val;public DataDampeners(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){val=(Controller.DampenersOverride ? 1:0);if(val !=Value){Value=val;return true;
}}return false;}}
class DataGravity:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=5;public string Unit{get;private set;}="g";IMyShipController Controller;
double val;public DataGravity(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){val=Controller.GetNaturalGravity().Length();val=val/9.81;if(val !=Value){Value=val;
return true;}}return false;}}
class DataHandbrake:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=1;public string Unit{get;private set;}="#";double val;IMyShipController Controller;
public DataHandbrake(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){if(Controller.HandBrake){val=1;}else{val=0;}if(val !=Value){Value=val;return true;}}
return false;}}
class DataHydrogen:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="l";List<IMyGasTank>tanks=new List<IMyGasTank>();
double val;double total;public DataHydrogen(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyGasTank){if(block.BlockDefinition.SubtypeId.Contains("Hydrogen")){tanks.Add(block as IMyGasTank);
total+=tanks[tanks.Count-1 ].Capacity;}}}Max=total;}public bool Update(){val=0;for(int i=0;i<tanks.Count;i++){val+=tanks[i].FilledRatio*tanks[i].Capacity;}if(val !=Value){Value=val;return true;}return false;
}}
class DataHydrogenTime:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=1;public string Unit{get;private set;}="s";Program p;Dictionary<Data, IData>shipData;
const int VALUES=10;double[] values=new double[VALUES];int index;double val;double lastValue=0;TimeSpan lastTime;public DataHydrogenTime(Dictionary<Data, IData>shipData, Program p){this.shipData=shipData;
this.p=p;}public bool Update(){double rate=MathHelperD.Clamp(lastValue-shipData[Data.Hydrogen].Value, 0, double.MaxValue)/(p.Time-lastTime).TotalSeconds;double distance=shipData[Data.Hydrogen].Value;double time=distance/rate;
values[index]=time;index++;if(index>=VALUES)index=0;val=values.Average();lastValue=shipData[Data.Hydrogen].Value;lastTime=p.Time;if(double.IsNaN(val))val=0;if(val !=Value){Value=val;return true;}return false;
}}
class DataInSeat:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=0;public string Unit{get;private set;}="#";IMyShipController controller;
double val;public DataInSeat(IMyShipController controller){this.controller=controller;}public bool Update(){if(controller !=null){if(controller.IsUnderControl)val=1;else val=0;if(val !=Value){Value=val;
return true;}}return false;}}
class DataJumpDriveCharge:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="Wh";List<IMyJumpDrive>jumpDrives=new List<IMyJumpDrive>();
double val;double total;public DataJumpDriveCharge(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyJumpDrive){jumpDrives.Add(block as IMyJumpDrive);total+=jumpDrives[jumpDrives.Count-1].MaxStoredPower*1000000;
}}Max=total;}public bool Update(){val=0;for(int i=0;i<jumpDrives.Count;i++){val+=jumpDrives[i].CurrentStoredPower*1000000;}if(val !=Value){Value=val;return true;}return false;}}
class DataJumpDriveDistance:IData{public double Value{get;private set;}public double Min{get;private set;}=100;public double Max{get;private set;}=100;public string Unit{get;private set;}="km";List<IMyJumpDrive>jumpDrives=new List<IMyJumpDrive>();
Dictionary<Data, IData>shipData;double val;float minJump=100;double maxDistance=0;public DataJumpDriveDistance(List<IMyTerminalBlock>blocks, Dictionary<Data, IData>shipData){this.shipData=shipData;foreach(var block in blocks)
{if(block is IMyJumpDrive){jumpDrives.Add(block as IMyJumpDrive);}}}public bool Update(){val=0;minJump=100;maxDistance=0;for(int i=0;i<jumpDrives.Count;i++){if(jumpDrives[i].Enabled){minJump=Math.Min(minJump, jumpDrives[i].GetValue<float>("JumpDistance"));
maxDistance+=2000*(1250000/shipData[Data.Mass].Value);}}maxDistance=MathHelper.Clamp(maxDistance, 0, 2000);Max=maxDistance;val=maxDistance*(minJump/100f);if(val !=Value){Value=val;return true;}return false;
}}
class DataLandingGear:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="Wh";List<IMyLandingGear>landingGears=new List<IMyLandingGear>();
double val;public DataLandingGear(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyLandingGear){landingGears.Add(block as IMyLandingGear);}}Max=landingGears.Count;}public bool Update()
{val=0;for(int i=0;i<landingGears.Count;i++){if(landingGears[i].IsLocked)val++;}if(val !=Value){Value=val;return true;}return false;}}
class DataLift:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=0;public string Unit{get;private set;}="g";IMyShipController controller;
List<IMyThrust>thrusters=new List<IMyThrust>();Dictionary<Data, IData>shipData;public DataLift(IMyShipController controller, List<IMyTerminalBlock>blocks, Dictionary<Data, IData>shipData){this.controller=controller;
this.shipData=shipData;foreach(var block in blocks){if(block is IMyThrust){thrusters.Add(block as IMyThrust);}}}double lift;double thrustSum=0;double thrustTotal=0;Vector3D gravity;Vector3D desiredDirection;
Vector3D thisThrustVec;Vector3D thisThrustVecTotal;public bool Update(){gravity=controller.GetNaturalGravity();desiredDirection=gravity;if(gravity==Vector3D.Zero)desiredDirection=controller.WorldMatrix.Forward;
desiredDirection=Vector3D.Normalize(-desiredDirection);thrustSum=0;thrustTotal=0;for(int i=0;i<thrusters.Count;i++){if(thrusters[i].WorldMatrix.Backward.Dot(desiredDirection)<0)continue;if(thrusters[i].Enabled)
{thisThrustVec=thrusters[i].WorldMatrix.Backward*thrusters[i].MaxEffectiveThrust;thrustSum+=thisThrustVec.Dot(desiredDirection);}thisThrustVecTotal=thrusters[i].WorldMatrix.Backward*thrusters[i].MaxThrust;
thrustTotal+=thisThrustVecTotal.Dot(desiredDirection);}lift=(thrustSum/shipData[Data.Mass].Value)-gravity.Normalize();lift=MathHelper.Clamp(lift/9.81, 0, double.PositiveInfinity);Max=(thrustTotal/shipData[Data.Mass].Value)-gravity.Normalize();
Max=MathHelper.Clamp(Max/9.81, 0, double.PositiveInfinity);if(lift !=Value){Value=lift;return true;}return false;}}
class DataMass:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100000;public string Unit{get;private set;}="kg";IMyShipController Controller;
double val;public DataMass(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){val=Controller.CalculateShipMass().PhysicalMass;if(val !=Value){Value=val;return true;
}}return false;}}
class DataMassCargo:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100000;public string Unit{get;private set;}="kg";IMyShipController Controller;
double val;MyShipMass masses;public DataMassCargo(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){masses=Controller.CalculateShipMass();val=masses.PhysicalMass-masses.BaseMass;
if(masses.PhysicalMass==0){val=0;}if(val !=Value){Value=val;return true;}}return false;}}
class DataNone:IData{public double Value{get;private set;}=0;public double Min{get;private set;}=0;public double Max{get;private set;}=1;public string Unit{get;private set;}="-";public DataNone(){}public bool Update()
{return false;}}
class DataOxygen:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="l";List<IMyGasTank>tanks=new List<IMyGasTank>();
double val;double total;public DataOxygen(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyGasTank){if(block.BlockDefinition.SubtypeId=="" || block.BlockDefinition.SubtypeId.Contains("Oxygen"))
{tanks.Add(block as IMyGasTank);total+=tanks[tanks.Count-1].Capacity;}}}}public bool Update(){val=0;for(int i=0;i<tanks.Count;i++){val+=tanks[i].FilledRatio*tanks[i].Capacity;}if(val !=Value){Value=val;
return true;}return false;}}
class DataPowerUsage:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=1;public string Unit{get;private set;}="W";List<IMyPowerProducer>producers=new List<IMyPowerProducer>();
double val;double total;public DataPowerUsage(List<IMyTerminalBlock>blocks, string typeID){foreach(var block in blocks){if(block is IMyPowerProducer && block.BlockDefinition.TypeIdString==typeID){producers.Add(block as IMyPowerProducer);
total+=producers[producers.Count-1].MaxOutput*1000000;}}Max=total;}public DataPowerUsage(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyPowerProducer){producers.Add(block as IMyPowerProducer);
total+=producers[producers.Count-1].MaxOutput*1000000;}}Max=total;}public bool Update(){val=0;for(int i=0;i<producers.Count;i++){val+=producers[i].CurrentOutput*1000000;}if(val !=Value){Value=val;return true;
}return false;}}
class DataPulse:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="t";int counter=0;bool up=true;
public DataPulse(){}public bool Update(){if(counter>=Max)up=false;else if(counter<=Min)up=true;if(up)counter+=2;else counter-=2;Value=counter;return true;}}
class DataRuler:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=200;public string Unit{get;private set;}="m";List<IMyCameraBlock>cameras=new List<IMyCameraBlock>();
double val;public DataRuler(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyCameraBlock && block.CustomName.Contains("FFI")){cameras.Add(block as IMyCameraBlock);cameras[cameras.Count-1].EnableRaycast=true;
}}Max=cameras.Count;}double distance;public bool Update(){val=double.PositiveInfinity;for(int i=0;i<cameras.Count;i++){if(cameras[i].Enabled){MyDetectedEntityInfo info=cameras[i].Raycast(200.1, 0, 0);if(info.HitPosition.HasValue)
{distance=Vector3D.Distance(cameras[i].GetPosition(), info.HitPosition.Value);if(distance<val)val=distance;}}}if(val !=Value){Value=val;return true;}return false;}}
class DataSeated:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=0;public string Unit{get;private set;}="#";List<IMyShipController>controllers=new List<IMyShipController>();
double val;public DataSeated(List<IMyTerminalBlock>blocks){foreach(var block in blocks){if(block is IMyShipController && !(block is IMyRemoteControl)){controllers.Add(block as IMyShipController);}}Max=controllers.Count;
}public bool Update(){val=0;for(int i=0;i<controllers.Count;i++){if(controllers[i].IsUnderControl)val++;}if(val !=Value){Value=val;return true;}return false;}}
class DataSpeed:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=100;public string Unit{get;private set;}="m/s";private double newValue=0;
IMyShipController Controller;public DataSpeed(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){newValue=Controller.GetShipSpeed();if(newValue !=Value){Value=newValue;
return true;}}return false;}}
class DataStoppingDistance:IData{public double Value{get;private set;}public double Min{get;private set;}=0;public double Max{get;private set;}=200;public string Unit{get;private set;}="m";List<IMyThrust>thrusters=new List<IMyThrust>();
IMyShipController controller;Dictionary<Data, IData>shipData;double val;public DataStoppingDistance(IMyShipController controller, List<IMyTerminalBlock>blocks, Dictionary<Data, IData>shipData){this.controller=controller;
this.shipData=shipData;foreach(var block in blocks){if(block is IMyThrust){thrusters.Add(block as IMyThrust);}}Max=thrusters.Count;}double force;Vector3D grav;Vector3D vel;double gravInHeading;public bool Update()
{grav=controller.GetNaturalGravity();vel=controller.GetShipVelocities().LinearVelocity;gravInHeading=Vector3D.Dot(grav, Vector3D.Normalize(vel));force=ForceInDirection(Vector3D.Normalize(vel), thrusters);
val=StoppingDistance(shipData[Data.Mass].Value, force, vel.Length(), gravInHeading);if(val !=Value){Value=val;return true;}return false;}double StoppingDistance(double mass, double force, double velocity, double gravity)
{double acceleration=force/mass-gravity;double distance=(velocity*velocity)/(2*acceleration);return distance;}double thrustSum=0;Vector3D desiredDirection;Vector3D thisThrustVec;double ForceInDirection(Vector3D direction, List<IMyThrust>thrusters)
{desiredDirection=direction;desiredDirection=Vector3D.Normalize(-desiredDirection);thrustSum=0;for(int i=0;i<thrusters.Count;i++){if(thrusters[i].WorldMatrix.Backward.Dot(desiredDirection)<0)continue;thisThrustVec=thrusters[i].WorldMatrix.Backward*thrusters[i].MaxEffectiveThrust;
thrustSum+=thisThrustVec.Dot(desiredDirection);}return thrustSum;}}
class DataVerticalSpeed:IData{public double Value{get;private set;}public double Min{get;private set;}=-100;public double Max{get;private set;}=100;public string Unit{get;private set;}="m/s";private double newValue=0;
IMyShipController Controller;public DataVerticalSpeed(IMyShipController controller){Controller=controller;}public bool Update(){if(Controller !=null){newValue=-Vector3.Dot(Controller.GetShipVelocities().LinearVelocity,
Vector3D.Normalize(Controller.GetNaturalGravity()));if(newValue !=Value){Value=newValue;return true;}}return false;}}
class FixedWidthText{private List<string>Text;public int Width{get;private set;}public FixedWidthText(int width=40){Text=new List<string>();Width=width;}public void Clear(){Text.Clear();}public void Append(string t)
{Text[Text.Count-1]+=t;}public void AppendLine(){Text.Add("");}public void AppendLine(string t){Text.Add(t);}public void AppendMultilineString(string t){Text.AddArray(t.Split('\n'));}public void Combine(List<string>input)
{Text.AddRange(input);}public List<string>GetRaw(){return Text;}public override string ToString(){return GetText(Width);}public string GetText(){return GetText(Width);}public string GetText(int lineWidth)
{string finalText="";foreach(var line in Text){string rest=line;if(rest.Length>lineWidth){while(rest.Length>lineWidth){string part=rest.Substring(0, lineWidth);rest=rest.Substring(lineWidth);for(int i=part.Length-1;i>0;i--)
{if(part[i]==' '){finalText+=part.Substring(0, i)+"\n";rest=part.Substring(i+1)+rest;break;}}}}finalText+=rest+"\n";}return finalText;}public static string Adjust(string text, int width){string rest=text;
string output="";if(rest.Length>width){while(rest.Length>width){string part=rest.Substring(0, width);rest=rest.Substring(width);for(int i=part.Length-1;i>0;i--){if(part[i]==' '){output+=part.Substring(0, i)+"\n";
rest=part.Substring(i+1)+rest;break;}}}}output+=rest;return output;}}
[Flags]enum Data{None=0,Pulse=1,Speed=2,VerticalSpeed=4,Altitude=8,AltitudeSea=16,Mass=32,MassCargo=64,Gravity=128,Dampeners=256,Hydrogen=512,Oxygen=1024,BatteryCharge=2048,BatteryUsage=4096,JumpDriveCharge=8192,
SolarUsage=16384,ReactorUsage=32768,HydrogenTime=65636,EngineUsage=131272,PowerUsage=262544,WindUsage=525088,JumpDriveDistance=1050176,Lift=2100352,LandingGears=4200704,Connectors=8401408,Seated=16802816,
InMainSeat=33605632,Ruler=67211264,StoppingDistance=134422528,Handbrake=268845056}interface IData{double Value{get;}double Min{get;}double Max{get;}string Unit{get;}bool Update();}
enum Meter{None,Text,Value,Bar,Sprite,Meter,HalfMeter,ThreeQuaterMeter,LineGraph,Action}interface IMeter{void Draw(MySpriteDrawFrame frame, Data dataChanged);}
class Ini{enum Header{None,FFI,Meter}public bool NoErrors{get;private set;}=true;public string Processed{get;private set;}public List<SkinDefinition>SkinDataList{get;private set;}=new List<SkinDefinition>();
IEnumerator<bool>IniStateMachine;List<string>lines;SkinDefinition skinData=null;MeterDefinition meter=null;Header header=Header.None;int surfaceCount;List<string>sprites;bool finnishedIni=false;bool inject=false;
string injectKey="";Dictionary<string, string>preloads;public Ini(string input, int surfaceCount, List<string>sprites, Dictionary<string, string>preloads){this.surfaceCount=surfaceCount;this.sprites=sprites;
this.preloads=preloads;lines=input.Split('\n').ToList();IniStateMachine=ParseIni();}public bool ParserDone(){if(!finnishedIni){if(IniStateMachine !=null){if(!IniStateMachine.MoveNext()|| !IniStateMachine.Current)
{IniStateMachine.Dispose();IniStateMachine=null;}}return false;}else{return true;}}private IEnumerator<bool>ParseIni(){for(int i=0;i<lines.Count;i++){string trimmed=lines[i].Trim();if(string.IsNullOrWhiteSpace(lines[i])|| trimmed.StartsWith("---"))
{if(inject){inject=false;Inject(i, injectKey);}header=Header.None;continue;}if(trimmed.StartsWith(";")){continue;}if(lines[i].StartsWith("[")){trimmed=trimmed.Trim(new char[]{'[', ']'});if(trimmed.StartsWith("FFI"))
{if(lines.Count>i+1){if(lines[i+1].ToLower().StartsWith("screen")){header=Header.FFI;if(skinData !=null){SkinDataList.Add(skinData);if(meter !=null){skinData.meters.Add(meter);}}skinData=new SkinDefinition();
meter=null;}else{header=Header.Meter;if(meter !=null && skinData !=null){skinData.meters.Add(meter);}meter=new MeterDefinition();}}else{lines[i]+=" ;! Can't have a header withouth a body.";NoErrors=false;
}}else{header=Header.None;}continue;}if(header==Header.None)continue;int index=lines[i].IndexOf(";!");if(index !=-1){lines[i]=lines[i].Substring(0, index);lines[i]=lines[i].TrimEnd();}index=trimmed.IndexOf(';');
if(index !=-1){trimmed=trimmed.Substring(0, index);}var keyvalue=trimmed.Split(new char[]{'='}, 2);if(keyvalue.Length !=2){lines[i]+=" ;! Couldn't split into 'key = value'.";NoErrors=false;continue;}keyvalue[0]=keyvalue[0].Trim().ToLower();
keyvalue[1]=keyvalue[1].Trim();int inum=0;double dnum=0;float fnum=0;string[] parts;if(header==Header.FFI){switch(keyvalue[0]){case "screen":if(int.TryParse(keyvalue[1], out inum)){inum-=1;if(inum<0){lines[i]+=" ;! Invalid screen number. Starts at 1.";
NoErrors=false;}else if(inum>=surfaceCount){lines[i]+=$" ;! Invalid screen number. Only {surfaceCount} screens avalible.";NoErrors=false;}else{skinData.screenId=inum;}}else{lines[i]+=" ;! Did not understand, it should be a nuber.";
NoErrors=false;}break;case "background":if(char.IsDigit(keyvalue[1][0])){Color color;if(TryParseRGBColor(keyvalue[1], out color)){color.A=255;skinData.backgroundColor=color;skinData.backgroundColorSet=true;
}else{lines[i]+=" ;! Couldnt read RGB color. Needs to be 1-3 comma separated values, ranging from 0-255. E.g: 102, 51, 153 is purple. You can also use Hex.";NoErrors=false;}}else if(keyvalue[1][0]=='#')
{Color color;if(TryParseHexColor(keyvalue[1], out color)){color.A=255;skinData.backgroundColor=color;skinData.backgroundColorSet=true;}else{lines[i]+=" ;! Couldn't read hex color. Needs to be 3 or 6 characters preceeded by #. E.g: #663399 is purple. ";
NoErrors=false;}}else{if(sprites.Contains(keyvalue[1])){skinData.background=keyvalue[1];}else{lines[i]+=" ;! No background with that name. Use an RGB color or one of these: ";for(int y=0;y<sprites.Count;y++)
{lines[i]+=sprites[y]+", ";}NoErrors=false;}}break;case "color":if(char.IsDigit(keyvalue[1][0])){Color color;if(TryParseRGBColor(keyvalue[1], out color)){skinData.color=color;skinData.colorSet=true;}else
{lines[i]+=" ;! Couldnt read RGB[A] color. Needs to be 1-3 comma separated values, ranging from 0-255. E.g: 102, 51, 153 is purple. You can also use Hex.";NoErrors=false;}}else if(keyvalue[1][0]=='#'){
Color color;if(TryParseHexColor(keyvalue[1], out color)){skinData.color=color;skinData.colorSet=true;}else{lines[i]+=" ;! Couldn't read hex color. Needs to be 3, 4, 6, or 8 characters preceeded by #. E.g: #663399 is purple. ";
NoErrors=false;}}else{lines[i]+=" ;! Couldn't read color. Use RGB or hex. E.g: 102, 51, 153  or #663399 is purple.";NoErrors=false;}break;case "inject":case "preload":if(preloads.ContainsKey(keyvalue[1].ToLower()))
{inject=true;injectKey=keyvalue[1].ToLower();lines[i]=";"+lines[i];}else{lines[i]+=" ;! No preload with that name. Avalible are:";foreach(var key in preloads.Keys){lines[i]+=" "+key+",";}lines[i].TrimEnd(',');
lines[i]+=".";NoErrors=false;}break;default:lines[i]+=" ;! Unknown key <"+keyvalue[0]+">";NoErrors=false;continue;}}else if(header==Header.Meter){switch(keyvalue[0]){case "type":switch(keyvalue[1].ToLower())
{case "text":meter.type=Meter.Text;break;case "value":meter.type=Meter.Value;break;case "bar":meter.type=Meter.Bar;break;case "meter":meter.type=Meter.Meter;break;case "half meter":meter.type=Meter.HalfMeter;
break;case "quarter meter":case "quater meter":meter.type=Meter.ThreeQuaterMeter;break;case "line graph":meter.type=Meter.LineGraph;break;case "sprite":meter.type=Meter.Sprite;break;case "action":meter.type=Meter.Action;
break;default:lines[i]+=" ;! Didn't understand type. Avalible types: text, value, bar, meter, half meter, quarter meter, sprite, and action.";NoErrors=false;break;}break;case "data":if(meter.type==Meter.None)
{lines[i]+=" ;! Can't proceed. You need to set type before data.";NoErrors=false;}else{switch(keyvalue[1].ToLower()){case "pulse":meter.data=Data.Pulse;break;case "speed":meter.data=Data.Speed;break;case "vertical speed":
meter.data=Data.VerticalSpeed;break;case "altitude":case "altitude land":meter.data=Data.Altitude;break;case "altitude sea":meter.data=Data.Altitude;break;case "mass":meter.data=Data.Mass;break;case "cargo mass":
meter.data=Data.MassCargo;break;case "gravity":meter.data=Data.Gravity;break;case "dampeners":meter.data=Data.Dampeners;break;case "hydrogen":meter.data=Data.Hydrogen;break;case "oxygen":meter.data=Data.Oxygen;
break;case "battery charge":case "battery level":meter.data=Data.BatteryCharge;break;case "battery usage":meter.data=Data.BatteryUsage;break;case "jumpdrive charge":case "jumpdrive level":meter.data=Data.JumpDriveCharge;
break;case "solar usage":meter.data=Data.SolarUsage;break;case "reactor usage":meter.data=Data.ReactorUsage;break;case "hydrogen time":meter.data=Data.HydrogenTime;break;case "hydrogen engine usage":meter.data=Data.EngineUsage;
break;case "power usage":meter.data=Data.PowerUsage;break;case "wind usage":meter.data=Data.WindUsage;break;case "jumpdrive distance":meter.data=Data.JumpDriveDistance;break;case "lift":meter.data=Data.Lift;
break;case "landing gears":meter.data=Data.LandingGears;break;case "connectors":meter.data=Data.Connectors;break;case "seated":meter.data=Data.Seated;break;case "in main seat":meter.data=Data.InMainSeat;
break;case "ruler":meter.data=Data.Ruler;break;case "stopping distance":meter.data=Data.StoppingDistance;break;case "handbrake":meter.data=Data.Handbrake;break;default:lines[i]+=" ;! Didn't understand data. Avalible: pulse, speed, vertical speed, altitude, altitude sea, mass, cargo mass, gravity, lift, dampeners, oxygen, hydrogen, hydrogen time, jumpdrive distance, jumpdrive charge, battery charge, battery usage, solar usage, reactor usage, engine usage, wind usage, power usage, landing gears, connectors, seated, in main seat, ruler, stopping distance.";
NoErrors=false;break;}}break;case "text":if(meter.type==Meter.Text){meter.textData=keyvalue[1];}break;case "sprite":if(meter.type==Meter.Sprite){if(sprites.Contains(keyvalue[1])){meter.textData=keyvalue[1];
}else{if(keyvalue[1].ToLower()=="square"){meter.textData="SquareSimple";}else{lines[i]+=" ;! No sprite with that name. Use one of these: Square, ";for(int y=0;y<sprites.Count;y++){lines[i]+=sprites[y]+", ";
}NoErrors=false;}}}break;case "action":if(meter.type==Meter.Action){meter.textData=keyvalue[1];}break;case "position":parts=keyvalue[1].Split(',');if(parts.Length==2){meter.posType.X=ParseVectorType(parts[0]);
meter.posType.Y=ParseVectorType(parts[1]);parts[0]=parts[0].Trim('%', 'w', 'h');parts[1]=parts[1].Trim('%', 'w', 'h');if(float.TryParse(parts[0], out fnum)){Vector2 vec=Vector2.Zero;vec.X=fnum;if(float.TryParse(parts[1], out fnum))
{vec.Y=fnum;meter.position=vec;}else{lines[i]+=" ;! Didn't understand value, make sure it's a number. Use %, w, or h to make your value a percentage of the screen's minimum dimention, width or height.";
NoErrors=false;}}else{lines[i]+=" ;! Didn't understand value, make sure it's a number. Use %, w, or h to make your value a percentage of the screen's minimum dimention, width or height.";NoErrors=false;
}}else{lines[i]+=" ;! Wrong amount of values. Should be 2 (x and y), separated by a comma.";NoErrors=false;}break;case "size":parts=keyvalue[1].Split(',');if(parts.Length==2){meter.sizeType.X=ParseVectorType(parts[0]);
meter.sizeType.Y=ParseVectorType(parts[1]);parts[0]=parts[0].Trim('%', 'w', 'h');parts[1]=parts[1].Trim('%', 'w', 'h');if(float.TryParse(parts[0], out fnum)){meter.size.X=fnum;if(float.TryParse(parts[1], out fnum))
{if(meter.type==Meter.Meter || meter.type==Meter.HalfMeter || meter.type==Meter.ThreeQuaterMeter){meter.size.Y=meter.size.X;meter.sizeType.Y=meter.sizeType.X;if(fnum>1 || fnum<0){lines[i]+=" ;! Specal case, here the second value is the line thickness and has to be between 0 and 1. Using default 0.3.";
NoErrors=false;meter.stroke=0.3f;}else{meter.stroke=fnum;}}else{meter.size.Y=fnum;}}else{lines[i]+=" ;! Didn't understand value, make sure it's a number. Use %, w, or h to make your value a percentage of the screen's minimum dimention, width or height.";
NoErrors=false;}}else{lines[i]+=" ;! Didn't understand value, make sure it's a number. Use %, w, or h to make your value a percentage of the screen's minimum dimention, width or height.";NoErrors=false;
}}else if(parts.Length==1){meter.sizeType.X=ParseVectorType(parts[0]);meter.sizeType.Y=meter.sizeType.X;parts[0]=parts[0].Trim('%', 'w', 'h');if(float.TryParse(parts[0], out fnum)){meter.size.X=fnum;meter.size.Y=fnum;
}else{lines[i]+=" ;! Didn't understand value, make sure it's a number.";NoErrors=false;}}else{lines[i]+=" ;! Wrong amount of values. Should be 1 for text and 1 or 2 for everything else (x and y, separated by a comma).";
NoErrors=false;}break;case "min":if(double.TryParse(keyvalue[1], out dnum)){meter.min=dnum;}else{lines[i]+=" ;! Couldn't parse number.";NoErrors=false;}break;case "max":if(double.TryParse(keyvalue[1], out dnum))
{meter.max=dnum;}else{lines[i]+=" ;! Couldn't parse number.";NoErrors=false;}break;case "unit":parts=keyvalue[1].Split(',');for(int j=0;j<parts.Length;j++){parts[j]=parts[j].Trim();if(parts[j].Length>1)
{switch(parts[j].ToLower()){case "show":meter.showUnit=true;break;case "default":meter.unit=Units.Default;break;case "auto":meter.unit=Units.Auto;break;default:lines[i]+=" ;! Input '"+parts[j]+"' wasn't understood. To change the value, use: auto, %, k, M, G, or T. To show the unit (kg, W, m/s etc..) add 'show'. E.g. 'show, %' will show the value as percent with a %-sign appended.";
NoErrors=false;break;}}else if(parts[j].Length==1){switch(parts[j]){case "":meter.unit=Units.Auto;break;case "%":meter.unit=Units.Percent;break;case "k":case "K":meter.unit=Units.Kilo;break;case "M":meter.unit=Units.Mega;
break;case "G":meter.unit=Units.Giga;break;case "T":meter.unit=Units.Tera;break;default:lines[i]+=" ;! Input '"+parts[j]+"' wasn't understood. To change the value, use: auto, %, k, M, G, or T. To show the unit (kg, W, m/s etc..) add 'show'. E.g. 'show, %' will show the value as percent wiht a % sign-appended.";
NoErrors=false;break;}}else{lines[i]+=" ;! Value empty? To change the value, use: auto, %, k, M, G, or T. To show the unit (kg, W, m/s etc..) add 'show'. E.g. 'show, %' will show the value as percent wiht a % sign-appended.";
NoErrors=false;}}break;case "decimals":if(int.TryParse(keyvalue[1], out inum)){if(inum==0){meter.decimalFormat="{0:"+keyvalue[1]+"}";}else if(inum>0 && inum<10){meter.decimalFormat="{0:0."+new string('0', Math.Abs(inum))+"}";
}else if(inum<0 && inum>-10){meter.decimalFormat="{0:"+new string('0', Math.Abs(inum))+"}";}else{lines[i]+=" ;! That's just ridiculous.. ;)";NoErrors=false;}}else{if(CheckNumberFormat(keyvalue[1])){meter.decimalFormat="{0:"+keyvalue[1]+"}";
}else{lines[i]+=" ;! Invalid number format. Use whole number (positive for decimals, negative for digits) or use advanced number formats: Use 0 for fixed digits/decimals, and # for optional. E.g: value: 13.37, format: 0.#, result: 13.8. Or format: 000.000, reuslt: 013.370.";
NoErrors=false;}}break;case "rotation":if(float.TryParse(keyvalue[1], out fnum)){meter.rotation=MathHelper.ToRadians(fnum);}else{lines[i]+=" ;! Didn't understand value, make sure it's a number.";NoErrors=false;
}break;case "color":if(char.IsDigit(keyvalue[1][0])){Color color;if(TryParseRGBColor(keyvalue[1], out color)){meter.color=color;meter.colorSet=true;}else{lines[i]+=" ;! Couldnt read RGB[A] color. Needs to be 1-3 comma separated values, ranging from 0-255. E.g: 102, 51, 153 is purple. You can also use Hex.";
NoErrors=false;}}else if(keyvalue[1][0]=='#'){Color color;if(TryParseHexColor(keyvalue[1], out color)){meter.color=color;meter.colorSet=true;}else{lines[i]+=" ;! Couldn't read hex color. Needs to be 3, 4, 6, or 8 characters preceeded by #. E.g: #663399 is purple. ";
NoErrors=false;}}else{lines[i]+=" ;! Couldn't read color. Use RGB or hex. E.g: 102, 51, 153  or #663399 is purple.";NoErrors=false;}break;case "background":if(char.IsDigit(keyvalue[1][0])){Color color;
if(TryParseRGBColor(keyvalue[1], out color)){meter.background=color;meter.backgroundSet=true;}else{lines[i]+=" ;! Couldnt read RGB[A] color. Needs to be 1-3 comma separated values, ranging from 0-255. E.g: 102, 51, 153 is purple. You can also use Hex.";
NoErrors=false;}}else if(keyvalue[1][0]=='#'){Color color;if(TryParseHexColor(keyvalue[1], out color)){meter.background=color;meter.backgroundSet=true;}else{lines[i]+=" ;! Couldn't read hex color. Needs to be 3, 4, 6, or 8 characters preceeded by #. E.g: #663399 is purple. ";
NoErrors=false;}}else{lines[i]+=" ;! Couldn't read color. Use RGB or hex. E.g: 102, 51, 153  or #663399 is purple.";NoErrors=false;}break;case "anchor":switch(keyvalue[1].ToLower()){case "center":meter.anchor=Anchor.Center;
break;case "left":meter.anchor=Anchor.Left;break;case "right":meter.anchor=Anchor.Right;break;default:lines[i]+=" ;! Unknown anchor point. Use left, center, or right.";NoErrors=false;break;}break;case "hide":
case "condition":parts=keyvalue[1].Split(',');if(parts.Length>0){var chars="><=!".ToCharArray();for(int j=0;j<parts.Length;j++){parts[j]=parts[j].Trim();int end=parts[j].LastIndexOfAny(chars);if(parts[j].Length>end+1)
{if(double.TryParse(parts[j].Substring(end+1).Trim(), out dnum)){if(end==0){switch(parts[j][0]){case '>':meter.conditions.Add(Condition.Greater);meter.condVals.Add(dnum);break;case '<':meter.conditions.Add(Condition.Less);
meter.condVals.Add(dnum);break;case '=':meter.conditions.Add(Condition.Equal);meter.condVals.Add(dnum);break;default:lines[i]+=" ;! Can't read condtion. Use one of these operators and a number: (> greater), (>= greater or equal), (< less), (<= less or equal), (== equal), (!= not equal). Writing '>= 50' means it will only be drawn if the value is greater than or equal to 50";
NoErrors=false;break;}}else if(end==1){switch(parts[j].Substring(0, 2)){case ">=":meter.conditions.Add(Condition.GreaterEqual);meter.condVals.Add(dnum);break;case "<=":meter.conditions.Add(Condition.LessEqual);
meter.condVals.Add(dnum);break;case "==":meter.conditions.Add(Condition.Equal);meter.condVals.Add(dnum);break;case "!=":meter.conditions.Add(Condition.Not);meter.condVals.Add(dnum);break;default:lines[i]+=" ;! Can't read condtion. Use one of these operators and a number: (> greater), (>= greater or equal), (< less), (<= less or equal), (== equal), (!= not equal). Writing '>= 50' means it will only be drawn if the value is greater than or equal to 50";
NoErrors=false;break;}}else{lines[i]+=" ;! Invalid condition. Use one of these operators and a number: (> greater), (>= greater or equal), (< less), (<= less or equal), (== equal), (!= not equal). Writing '>= 50' means it will only be drawn if the value is greater than or equal to 50";
NoErrors=false;}}else{lines[i]+=" ;! Can't read this. Use one of these operators and a number: (> greater), (>= greater or equal), (< less), (<= less or equal), (== equal), (!= not equal). Writing '>= 50' means it will only be drawn if the value is greater than or equal to 50";
NoErrors=false;}}else{lines[i]+=" ;! Can't read this. Use one of these operators and a number: (> greater), (>= greater or equal), (< less), (<= less or equal), (== equal), (!= not equal). Writing '>= 50' means it will only be drawn if the value is greater than or equal to 50";
NoErrors=false;}}}else{lines[i]+=" ;! Can't read this. Use one of these operators and a number: (> greater), (>= greater or equal), (< less), (<= less or equal), (== equal), (!= not equal). Writing '>= 50' means it will only be drawn if the value is greater than or equal to 50";
NoErrors=false;}break;case "block":parts=keyvalue[1].Split(',');if(parts.Length>0){for(int j=0;j<parts.Length;j++){parts[j]=parts[j].Trim().Trim('"');meter.blocks.Add(parts[j]);}}else{lines[i] += ";! No block? Use block name, and/or*group name*in asterics. Separete more than one with comma.";
NoErrors = false;}break;default:lines[i] += ";! Unknown key";NoErrors = false;continue;}}if (i == lines.Count - 1 && inject){inject = false;Inject(i+1, injectKey);header = Header.None;}}if (meter != null && skinData != null)
{skinData.meters.Add(meter);}if (skinData != null){SkinDataList.Add(skinData);}Processed = string.Join("\n", lines);finnishedIni = true;yield return true;}void Inject(int lineIndex, string preload){lines.InsertRange(lineIndex, preloads[preload].Split('\n'));
}static bool TryParseRGBColor(string text, out Color color){color = Color.White;var parts = text.Split(',');byte val = 0;if (parts.Length == 1){if (byte.TryParse(parts[0].Trim(), out val)){color.R = val;
color.G = val;color.B = val;}else return false;}else if (parts.Length == 2){if (byte.TryParse(parts[0].Trim(), out val)){color.R = val;color.G = val;color.B = val;}else return false;if (byte.TryParse(parts[1].Trim(), out val))
{color.A = val;}else return false;}else if (parts.Length == 3){if (byte.TryParse(parts[0].Trim(), out val)){color.R = val;}else return false;if (byte.TryParse(parts[1].Trim(), out val)){color.G = val;}
else return false;if (byte.TryParse(parts[2].Trim(), out val)){color.B = val;}else return false;}else if (parts.Length == 4){if (byte.TryParse(parts[0].Trim(), out val)){color.R = val;}else return false;
if (byte.TryParse(parts[1].Trim(), out val)){color.G = val;}else return false;if (byte.TryParse(parts[2].Trim(), out val)){color.B = val;}else return false;if (byte.TryParse(parts[3].Trim(), out val)){
color.A = val;}else return false;}else return false;return true;}static bool TryParseHexColor(string text, out Color color){color = Color.White;text = text.TrimStart('#').Trim();if (text.Length == 3){color.R = (byte)(Convert.ToInt16(text.Substring(0, 1), 16) * 16);
color.G = (byte)(Convert.ToInt16(text.Substring(1, 1), 16) * 16);color.B = (byte)(Convert.ToInt16(text.Substring(2, 1), 16) * 16);}else if (text.Length == 4){color.R = (byte)(Convert.ToInt16(text.Substring(0, 1), 16) * 16);
color.G = (byte)(Convert.ToInt16(text.Substring(1, 1), 16) * 16);color.B = (byte)(Convert.ToInt16(text.Substring(2, 1), 16) * 16);color.A = (byte)(Convert.ToInt16(text.Substring(3, 1), 16) * 16);}else if (text.Length == 6)
{color.R = Convert.ToByte(text.Substring(0, 2), 16);color.G = Convert.ToByte(text.Substring(2, 2), 16);color.B = Convert.ToByte(text.Substring(4, 2), 16);}else if (text.Length == 8){color.R = Convert.ToByte(text.Substring(0, 2), 16);
color.G = Convert.ToByte(text.Substring(2, 2), 16);color.B = Convert.ToByte(text.Substring(4, 2), 16);color.A = Convert.ToByte(text.Substring(6, 2), 16);}else return false;return true;}static bool CheckNumberFormat(string input)
{string whitelist = "0#,.";foreach (char c in input){if (whitelist.IndexOf(c) == -1)return false;}return true;}static VectorType ParseVectorType(string input){if (input.Contains('%')){return VectorType.ViewMin;
}else if (input.Contains('w')){return VectorType.ViewWidth;}else if (input.Contains('h')){return VectorType.ViewHeight;}return VectorType.Pixel;}public static bool MeetsConditions(List<Condition> conditions, double value, List<double> conditionValues)
{for (int i = 0; i < conditions.Count; i++){if (TryCondition(conditions[i], value, conditionValues[i])){return true;}}return false;}public static double AdjustToUnit(MeterDefinition def, Dictionary<Data, IData> shipData, double total, bool UseDataMinMax)
{double val = shipData[def.data].Value;if (def.unit == Units.Percent){if (UseDataMinMax){total = shipData[def.data].Max - shipData[def.data].Min;}val = shipData[def.data].Value / total * 100;}else if (def.unit == Units.Default)
{val = shipData[def.data].Value;}else{val = ConvertTo(def.unit, shipData[def.data].Value);}return val;}public static bool TryCondition(Condition condition, double left, double right){switch (condition)
{case Condition.Greater:return left > right;case Condition.Less:return left < right;case Condition.GreaterEqual:return left >= right;case Condition.LessEqual:return left <= right;case Condition.Equal:return left == right;
case Condition.Not:return left != right;}return false;}public static double ConvertTo(Units unit, double value){switch (unit){case Units.Auto:double abs = Math.Abs(value);if (abs >= 1000000000000) return ConvertTo(Units.Tera, value);
if (abs >= 1000000000) return ConvertTo(Units.Giga, value);if (abs >= 1000000) return ConvertTo(Units.Mega, value);if (abs >= 1000) return ConvertTo(Units.Kilo, value);return value;case Units.Default:return value;
case Units.Kilo:return value * 0.001;case Units.Mega:return value * 0.000001;case Units.Giga:return value * 0.000000001;case Units.Tera:return value * 0.000000000001;}return value;}}enum Condition{None,
Greater,Less,GreaterEqual,LessEqual,Equal,Not}enum Units{None,Default,Auto,Percent,Kilo,Mega,Giga,Tera}class SkinDefinition{public int screenId;public Color color;public bool colorSet;public string background;
public Color backgroundColor;public bool backgroundColorSet;public List<MeterDefinition> meters = new List<MeterDefinition>();}struct Vector2Type{public VectorType X;public VectorType Y;}enum VectorType
{Pixel,ViewWidth,ViewHeight,ViewMin}class MeterDefinition{public Meter type;public Data data;public string textData;public Vector2 position = Vector2.Zero;public Vector2Type posType;public Vector2 size = Vector2.Zero;
public Vector2Type sizeType;public float stroke = 0.3f;public Color color = Color.White;public bool colorSet = false;public Color background = Color.White;public bool backgroundSet = false;public double min;
public double max;public Units unit = Units.Default;public bool showUnit;public string decimalFormat = "{0:0.##}";public Anchor anchor = Anchor.Center;public float rotation = 0;public List<Condition> conditions = new List<Condition>();
public List<double> condVals = new List<double>();public List<string> blocks = new List<string>();}
bool debug=false;public TimeSpan Time{get;private set;}IEnumerator<bool>InitStateMachine;IEnumerator<bool>DrawStateMachine;bool DoDraw=false;byte Initialized=0;List<Skin>Skins=new List<Skin>();Dictionary<Data, IData>ShipData=new Dictionary<Data, IData>();
long Count10=0;IMyShipController Controller=null;int MaxInstructionsPerTick=0;Data DataChanged=0;List<IMyTerminalBlock>blocks;List<string>InitProblemBlockNames=new List<string>();IMyTextSurface pbSurface;
SurfaceMath pbSM;Dictionary<string, string>Preloads;Profiler profiler;public Program(){if(Me.SurfaceCount>0){pbSurface=Me.GetSurface(0);pbSM=new SurfaceMath(pbSurface);pbSurface.ContentType=ContentType.SCRIPT;
pbSurface.Script="";if(pbSurface.ScriptBackgroundColor !=problemBG)defaultBG=pbSurface.ScriptBackgroundColor;}InitPreloads();MaxInstructionsPerTick=Runtime.MaxInstructionCount/4;Runtime.UpdateFrequency=UpdateFrequency.Update10 | UpdateFrequency.Update1;
InitStateMachine=Init();}public void Main(string argument, UpdateType updateType){Time=Time+Runtime.TimeSinceLastRun;if(argument=="update"){Initialized=0;InitStateMachine=Init();}else if(argument=="profiler")
{profiler=new Profiler(Me, 600, 10);}if((updateType & UpdateType.Update1)!=0){PrintDetailedInfo();}if((updateType & UpdateType.Update10)!=0){Count10++;if(!debug)DrawOutsidePBLcd();}if(Initialized==0){if(InitStateMachine !=null)
{if(!InitStateMachine.MoveNext()|| !InitStateMachine.Current){InitStateMachine.Dispose();InitStateMachine=null;}}}else{if((updateType & UpdateType.Update10)!=0){DataChanged=0;foreach(var data in ShipData)
{if(data.Value.Update()){DataChanged |=data.Key;}}DoDraw=true;if(Initialized==1){DataChanged=0;DataChanged=~DataChanged;Initialized++;}}if((updateType & UpdateType.Update1)!=0){if(DrawStateMachine !=null)
{if(!DrawStateMachine.MoveNext()|| !DrawStateMachine.Current){DrawStateMachine.Dispose();DrawStateMachine=null;}}}}if(profiler !=null){Echo("Profiler: "+profiler.Update(Runtime.LastRunTimeMs).ToString("P")+"\nAvr: "+profiler.Avrage.ToString("n4")+"ms, Peak: "+profiler.Peak.ToString("n4")+"ms");
}}private IEnumerator<bool>Draw(){while(true){if(DoDraw){foreach(var skin in Skins){if(OverInstructionLimit())yield return true;skin.Draw(DataChanged);}DoDraw=false;}yield return true;}}private IEnumerator<bool>Init()
{Skins.Clear();ShipData.Clear();InitProblemBlockNames.Clear();blocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(blocks, x=>x.IsSameConstructAs(Me));var sprites=new List<string>();foreach(var block in blocks)
{if(block is IMyShipController){var contr=block as IMyShipController;if(contr.CanControlShip){if(Controller==null){Controller=contr;}if(contr.IsMainCockpit){Controller=contr;}}}}foreach(var block in blocks)
{var surfaceProvider=block as IMyTextSurfaceProvider;if(surfaceProvider !=null && surfaceProvider.SurfaceCount>0){if(sprites.Count==0){surfaceProvider.GetSurface(0).GetSprites(sprites);}Ini ini=new Ini(block.CustomData, surfaceProvider.SurfaceCount, sprites, Preloads);
while(true){if(ini.ParserDone()==true){break;}else{if(OverInstructionLimit())yield return true;}}if(!ini.NoErrors){InitProblemBlockNames.Add(block.CustomName);}block.CustomData=ini.Processed;foreach(var skinDef in ini.SkinDataList)
{if(skinDef.screenId<surfaceProvider.SurfaceCount){var surface=surfaceProvider.GetSurface(skinDef.screenId);var sm=new SurfaceMath(surface);var meters=new List<IMeter>();foreach(var meter in skinDef.meters)
{if(meter.type==Meter.Text){RegisterDataPoint(meter.data);meters.Add(new MeterText(sm, meter, ShipData));}else if(meter.type==Meter.Value){RegisterDataPoint(meter.data);meters.Add(new MeterValue(sm, meter, ShipData));
}else if(meter.type==Meter.Bar){RegisterDataPoint(meter.data);meters.Add(new MeterBar(sm, meter, ShipData));}else if(meter.type==Meter.LineGraph){RegisterDataPoint(meter.data);meters.Add(new MeterLineGraph(sm, meter, ShipData));
}else if(meter.type==Meter.Meter){RegisterDataPoint(meter.data);meters.Add(new MeterMeter(sm, meter, ShipData,(skinDef.backgroundColorSet ? skinDef.backgroundColor:surface.ScriptBackgroundColor)));}else if(meter.type==Meter.HalfMeter)
{RegisterDataPoint(meter.data);meters.Add(new MeterHalfMeter(sm, meter, ShipData,(skinDef.backgroundColorSet ? skinDef.backgroundColor:surface.ScriptBackgroundColor)));}else if(meter.type==Meter.ThreeQuaterMeter)
{RegisterDataPoint(meter.data);meters.Add(new MeterThreeQuaterMeter(sm, meter, ShipData,(skinDef.backgroundColorSet ? skinDef.backgroundColor:surface.ScriptBackgroundColor)));}else if(meter.type==Meter.Sprite)
{RegisterDataPoint(meter.data);meters.Add(new MeterSprite(sm, meter, ShipData));}else if(meter.type==Meter.Action){RegisterDataPoint(meter.data);meters.Add(new MeterAction(sm, meter, ShipData, blocks, GridTerminalSystem, Me));
}}Skins.Add(new Skin(this, surface, sm, block, skinDef, meters));}}}}DrawStateMachine=Draw();Initialized++;yield return false;}void RegisterDataPoint(Data data){if(!ShipData.ContainsKey(data)){switch(data){case Data.None:ShipData.Add(data, new DataNone());break;case Data.Pulse:
ShipData.Add(data, new DataPulse());break;case Data.Speed:ShipData.Add(data, new DataSpeed(Controller));break;case Data.VerticalSpeed:ShipData.Add(data, new DataVerticalSpeed(Controller));break;case Data.Altitude:
ShipData.Add(data, new DataAltitude(Controller));break;case Data.AltitudeSea:ShipData.Add(data, new DataAltitudeSea(Controller));break;case Data.Mass:ShipData.Add(data, new DataMass(Controller));break;
case Data.MassCargo:ShipData.Add(data, new DataMassCargo(Controller));break;case Data.Gravity:ShipData.Add(data, new DataGravity(Controller));break;case Data.Dampeners:ShipData.Add(data, new DataDampeners(Controller));
break;case Data.Hydrogen:ShipData.Add(data, new DataHydrogen(blocks));break;case Data.Oxygen:ShipData.Add(data, new DataOxygen(blocks));break;case Data.BatteryCharge:ShipData.Add(data, new DataBatteryCharge(blocks));
break;case Data.BatteryUsage:ShipData.Add(data, new DataPowerUsage(blocks, "MyObjectBuilder_BatteryBlock"));break;case Data.JumpDriveCharge:ShipData.Add(data, new DataJumpDriveCharge(blocks));break;case Data.SolarUsage:
ShipData.Add(data, new DataPowerUsage(blocks, "MyObjectBuilder_SolarPanel"));break;case Data.ReactorUsage:ShipData.Add(data, new DataPowerUsage(blocks, "MyObjectBuilder_Reactor"));break;case Data.HydrogenTime:
RegisterDataPoint(Data.Hydrogen);ShipData.Add(data, new DataHydrogenTime(ShipData, this));break;case Data.EngineUsage:ShipData.Add(data, new DataPowerUsage(blocks, "MyObjectBuilder_HydrogenEngine"));break;
case Data.PowerUsage:ShipData.Add(data, new DataPowerUsage(blocks));break;case Data.WindUsage:ShipData.Add(data, new DataPowerUsage(blocks, "MyObjectBuilder_WindTurbine"));break;case Data.JumpDriveDistance:
RegisterDataPoint(Data.Mass);ShipData.Add(data, new DataJumpDriveDistance(blocks, ShipData));break;case Data.Lift:RegisterDataPoint(Data.Mass);ShipData.Add(data, new DataLift(Controller, blocks, ShipData));
break;case Data.LandingGears:ShipData.Add(data, new DataLandingGear(blocks));break;case Data.Connectors:ShipData.Add(data, new DataConnectors(blocks));break;case Data.Seated:ShipData.Add(data, new DataSeated(blocks));
break;case Data.InMainSeat:ShipData.Add(data, new DataInSeat(Controller));break;case Data.Ruler:ShipData.Add(data, new DataRuler(blocks));break;case Data.StoppingDistance:RegisterDataPoint(Data.Mass);RegisterDataPoint(Data.Speed);
ShipData.Add(data, new DataStoppingDistance(Controller, blocks, ShipData));break;case Data.Handbrake:ShipData.Add(data, new DataHandbrake(Controller));break;}}}public bool OverInstructionLimit(){return Runtime.CurrentInstructionCount>MaxInstructionsPerTick;
}FixedWidthText detailedInfo=new FixedWidthText(40);void PrintDetailedInfo(){detailedInfo.Clear();detailedInfo.AppendLine("Blarg's Fancy Flight Info");if(Initialized<2){detailedInfo.AppendLine("Initializing...");
detailedInfo.Append(new string('.',(int)(Count10%16)/4));detailedInfo.AppendLine("Please wait.");}else{detailedInfo.AppendLine("Running");detailedInfo.Append(new string('.',(int)(Count10%16)/4));detailedInfo.AppendLine();
if(InitProblemBlockNames.Count>0){detailedInfo.AppendLine("Problem(s) in Custom Data of:");for(int i=0;i<InitProblemBlockNames.Count;i++){detailedInfo.AppendLine("• "+InitProblemBlockNames[i]);}}else{detailedInfo.AppendLine("No problems detected.");
}}Echo(detailedInfo.ToString());}Color pbSquare=new Color(Color.White, 0.1f);Color defaultBG=new Color(0, 88, 151);Color problemBG=new Color(151, 88, 0);float[] heartBeat={1, 1.3f, 1.5f, 1.3f, 1, 1.3f, 1.5f, 1.3f, 1, 1, 1, 1};
int heartBeatIndex=0;void DrawOutsidePBLcd(){if(pbSurface==null)return;using(var frame=pbSurface.DrawFrame()){Vector2 headerPos=new Vector2(pbSM.Center.X, pbSM.Center.Y-pbSM.Size.Y*0.333f);MySprite sprite;
sprite=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:pbSquare);sprite.Size=new Vector2(pbSM.Size.X, pbSM.Size.Y*0.333f);sprite.Position=headerPos;frame.Add(sprite);sprite=new MySprite(SpriteType.TEXTURE, "Grid", color:pbSurface.ScriptForegroundColor);
sprite.Size=pbSM.BGSize*2;frame.Add(sprite);string status="";string problems="";if(Initialized<2){status="Initializing...\nPlease wait.";}else{if(InitProblemBlockNames.Count>0){status="Problem(s) with:";
for(int i=0;i<InitProblemBlockNames.Count;i++){problems+=InitProblemBlockNames[i]+"\n";}pbSurface.ScriptBackgroundColor=problemBG;}else{status="Running";pbSurface.ScriptBackgroundColor=defaultBG;heartBeatIndex++;
sprite=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:pbSquare);sprite.Size=Vector2.One*(pbSM.SmallestSize*0.15f)*heartBeat[heartBeatIndex%heartBeat.Length];sprite.Position=new Vector2(pbSM.Center.X, pbSM.Center.Y+pbSM.Size.Y*0.165f);
frame.Add(sprite);}}MySprite textSprite;textSprite=MySprite.CreateText("Blargmode's\nFancy Flight Info", "Debug", pbSurface.ScriptForegroundColor, 1f, TextAlignment.CENTER);textSprite.Position=new Vector2(headerPos.X, headerPos.Y-pbSM.TextHeight(1, 2)*0.5f);
frame.Add(textSprite);textSprite=MySprite.CreateText(status, "Debug", pbSurface.ScriptForegroundColor, 1f, TextAlignment.CENTER);textSprite.Position=new Vector2(headerPos.X, headerPos.Y+pbSM.Size.Y*0.165f);
frame.Add(textSprite);textSprite=MySprite.CreateText(problems, "Debug", pbSurface.ScriptForegroundColor, 0.7f, TextAlignment.CENTER);textSprite.Position=new Vector2(headerPos.X, headerPos.Y+pbSM.Size.Y*0.165f+pbSM.TextHeight(1, 1));
frame.Add(textSprite);}}void InitPreloads(){Preloads=new Dictionary<string, string>(){{"speed", "\n[FFI_Speedometer_Meter]\ntype=quater meter\ndata=speed\nposition=0, 10%\nsize=50%, 0.1\nbackground=#3af\n\n[FFI_Speedometer_Value]\ntype=value\ndata=speed\nposition=0, 10h\nsize=25%\ndecimals=-2\n\n[FFI_Speedometer_Text]\ntype=text\ntext=m/s\nposition=0, -10h\ncolor=#66bfff\nsize=13%\n\n\n[FFI_VSpeed_ArrowUp]\ntype=sprite\nsprite=Triangle\nposition=-43w, 42h\nsize=6%\ncolor=#3af\n\n[FFI_VSpeed_ArrowDown]\ntype=sprite\nsprite=Triangle\nposition=-43w, -42h\nsize=6%\nrotation=180\ncolor=#3af\n\n[FFI_VSpeed_PositiveBar]\ntype=bar\ndata=vertical speed\nposition=-43w, 2.5h\nsize=39.1h, 2.5w\nanchor=left\nrotation=-90\nmin=0\nmax=100\nbackground=#3af\n\n[FFI_VSpeed_NegativeBar]\ntype=bar\ndata=vertical speed\nposition=-43w, -2.5h\nsize=39.1h, 2.5w\nanchor=left\nrotation=90\nmin=-100\nmax=0\nbackground=#3af\n\n[FFI_VSpeed_CenterCircle]\ntype=sprite\nsprite=Circle\nposition=-43w, 0\nsize=7%\ncolor=#3af\n\n[FFI_VSpeed_ValueBackground]\ntype=sprite\nsprite=Square\nposition=-25w, -35h\nsize=25w, 14h\ncolor=#3af\n\n[FFI_VSpeed_Value]\ntype=value\ndata=vertical speed\nposition=-25w, -35h\nsize=13%\ndecimals=-1\n\n[FFI_Altitude_ValueBackgrund]\ntype=sprite\nSprite=square\nposition=25w, -35h\nsize=25w, 14h\ncolor=#3af\n\n[FFI_Altitude_Value]\ntype=value\ndata=altitude\nposition=25w, -35h\nsize=13%\ndecimals=-1\n\n[FFI_Altitude_ArrowUp]\ntype=sprite\nsprite=Triangle\nposition=43w, 42h\nsize=6%\ncolor=#3af\n\n[FFI_Altitude_Bar2000]\ntype=bar\ndata=altitude\nposition=43w, -39h\nsize=79h, 2.5w\nanchor=left\nrotation=-90\nmax=2000\nhide=< 20\nbackground=#3af\n\n[FFI_Altitude_Bar20]\ntype=bar\ndata=altitude\nposition=43w, -39h\nsize=79h, 2.5w\nanchor=left\nrotation=-90\nmax=20\ncolor=#e33\nbackground=#3af\nhide=>= 20\n\n[FFI_Altitude_CenterCircle]\ntype=sprite\nsprite=Circle\nposition=43w, -42h\nsize=6%\ncolor=#3af\n"},
{"propulsion", "\n[FFI_Hydrogen_Background]\ntype=sprite\nsprite=square\nsize=45w, 45h\nposition=-24w, -24h\ncolor=#3af\n\n[FFI_Hydrogen_Background_red]\ntype=sprite\nsprite=square\nsize=45w, 45h\nposition=-24w, -24h\ncolor=#e33\ndata = hydrogen\nunit = %\nhide=> 25\n\n[FFI_Hydrogen_Icon]\ntype=sprite\nsprite=IconHydrogen\nsize=15%\nposition=-24w, -24h\ncolor=255, 150\n\n[FFI_Hydrogen_Graph]\ntype=line graph\ndata=hydrogen\nsize=45w, 45h\nposition=-24w, -24h\n\n\n[FFI_Power_Background]\ntype=sprite\nsprite=square\nsize=45w, 45h\nposition=-24w, 24h\ncolor=#3af\n\n[FFI_Power_Icon]\ntype=sprite\nsprite=IconEnergy\nsize=20%\nposition=-24w, 24h\ncolor=255, 150\n\n[FFI_Power_Graph]\ntype=line graph\ndata=power usage\nsize=45w, 45h\nposition=-24w, 24h\n\n[FFI_Power_Graph]\ntype=line graph\ndata=battery charge\nsize=45w, 45h\nposition=-24w, 24h\ncolor=255, 50\n\n\n[FFI_Lift_Outline]\ntype=sprite\nsprite=SquareHollow\nposition=33w, 39h\nsize=25w, 15h\n\n[FFI_Lift_Value_Ok]\ntype=value\ndata=lift\nsize=14%\nposition=33w, 39h\ncolor=#66ff66\nhide=<= 1\nunit=show\n\n[FFI_Lift_Value_Warning]\ntype=value\ndata=lift\nsize=14%\nposition=33w, 39h\ncolor=#ff6666\nhide=> 1\nunit=show\n\n[FFI_Lift_Text]\ntype=text\ntext=Lift\nsize=14%\nanchor=left\nposition=1w, 39h\ncolor=#66bfff\n\n\n[FFI_Grav_Outline]\ntype=sprite\nsprite=SquareHollow\nposition=33w, 20h\nsize=25w, 15h\n\n[FFI_Grav_Value]\ntype=value\ndata=gravity\nsize=14%\nposition=33w, 20h\nunit=show\n\n[FFI_Grav_Text]\ntype=text\ntext=Grav\nsize=14%\nanchor=left\nposition=1w, 20h\ncolor=#66bfff\n\n\n[FFI_HydrogenTime_Outline]\ntype=sprite\nsprite=SquareHollow\nposition=23.5w, -39h\nsize=44w, 15h\n\n[FFI_HydrogenTime_Value]\ntype=value\ndata=hydrogen time\nsize=14%\nposition=23.5w, -39h\nunit=auto, show\n\n[FFI_HydrogenTime_Text]\ntype=text\ntext=H2 Time\nsize=14%\nposition=23.5w, -25h\ncolor=#66bfff\n\n\n[FFI_CargoWeight_Outline]\ntype=sprite\nsprite=SquareHollow\nposition=23.5w, -10h\nsize=44w, 15h\n\n[FFI_CargoWeight_Value]\ntype=value\ndata=cargo mass\nsize=14%\nposition=23.5w, -10h\nunit=auto, show\n\n[FFI_CargoWeight_Text]\ntype=text\ntext=Cargo\nsize=14%\nposition=23.5w, 6h\ncolor=#66bfff\n"},
{"land and connect", "\n[FFI_LandingGears1]\ntype=sprite\nsprite=Square\nposition=-25w, 0\nsize=46w, 92h\ncolor=#3af\n\n[FFI_LandingGears2]\ntype=sprite\nsprite=Square\nposition=25w, 0\nsize=46w, 92h\ncolor=#3af\n\n[FFI_LandingGears3]\ntype=sprite\nsprite=SemiCircle\nrotation=180\nposition=-23w, 15%\nsize=20%\ncolor=255\n\n[FFI_LandingGears4]\ntype=sprite\nsprite=Square\nrotation=110\nanchor=left\nposition=-20w, 10%\nsize=13%, 8%\ncolor=255\n\n[FFI_LandingGears5]\ntype=sprite\nsprite=SemiCircle\nposition=-23w, -10%\nsize=20%\ncolor=255\n\n[FFI_LandingGears6]\ntype=sprite\nsprite=Square\nposition=-25w, -10%\nsize=28%, 5%\ncolor=255\n\n[FFI_LandingGears_ColoredBar_Locked]\ntype=sprite\nsprite=Square\nposition=-25w, -14%\nsize=30%, 3%\ncolor=#66ff66\n\n[FFI_LandingGears_ColoredBar_Unlocked]\ntype=sprite\nsprite=Square\nposition=-25w, -14%\nsize=30%, 3%\ncolor=#e33\ndata=landing gears\nhide=> 0\n\n[FFI_LandingGears_Distance_Value]\ntype=value\ndata=ruler\nposition=-25w, -25%\nunit=show\ndecimals=1\n\n\n[FFI_Connector1]\ntype=sprite\nsprite=SemiCircle\nrotation=180\nposition=25w, -8%\nsize=20%, 16%\n\n[FFI_Connector2]\ntype=sprite\nsprite=Square\nposition=25w, 0%\nsize=20%\n\n[FFI_Connector3]\ntype=sprite\nsprite=SemiCircle\nposition=25w, 8%\nsize=20%, 16%\n\n[FFI_Connector_ColoredBar_Locked]\ntype=sprite\nsprite=Circle\nposition=25w, 0%\nsize=19%\ncolor=#66ff66\n\n[FFI_Connector_ColoredBar_Unlocked]\ntype=sprite\nsprite=Circle\nposition=25w, 0%\nsize=19%\ncolor=#e33\ndata=connectors\nhide=> 0\n\n[FFI_Connector_InnerCircle]\ntype=sprite\nsprite=Circle\nposition=25w, 0%\nsize=13%\ncolor=150\n"},
{"space rover", "\n;========================== Fuel\n[FFI H2 time background triangle]\ntype = sprite\nsprite = Triangle\nrotation = 180\nposition = 0, -25h\nsize = 150h\ncolor = 254, 94, 0\n\n[FFI H2 time value]\ntype = value\ndata = hydrogen time\nsize = 20h\nposition = 0, 25h\nunit = show\n\n[FFI H2 bar]\ntype = bar\ndata = hydrogen\nsize = 62h, 10h \nposition = 55h, -2h\ncolor = 254, 94, 0\nbackground = 50\nanchor = left\nrotation = -60\n\n[FFI Battery bar]\ntype = bar\ndata = battery charge\nsize = 62h, 10h \nposition = -55h, -2h\ncolor = 0, 94, 254\nbackground = 50\nanchor = left\nrotation = -120\n\n[FFI H2 time background square]\ntype = sprite\nsprite = Square\nposition = 0, -25h\nsize = 125h, 50h\ncolor = 0\n\n[FFI H2 icon]\ntype = sprite\nsprite = IconHydrogen\nsize = 14h \nposition = 55h, -8h\ncolor = 255, 100\n\n[FFI Battery icon]\ntype = sprite\nsprite = IconEnergy\nsize = 14h \nposition = -55h, -8h\ncolor = 255, 100\n\n;========================== Left bars\n[FFI Speed bar]\ntype = bar\ndata = speed\nsize = 6.7w, 7h;\ncolor = 0, 94, 254\nbackground = 50\nposition = -50w, 24h\nanchor = left\n\n[FFI V-speed bar]\ntype = bar\ndata = vertical speed\nsize = 10w, 7h;\ncolor = 0, 94, 254\nbackground = 50\nposition = -50w, -1h\nanchor = left\n\n[FFI V-speed bar]\ntype = bar\ndata = vertical speed\nsize = 10w, 7h;\ncolor = 254, 94, 94\nposition = -50w, -1h\nanchor = left\nrotation = 180\nmin = -100\nmax = 0\n\n;========================== Black triangle\n\n[FFI Right angle triangle]\ntype = sprite\nsprite = Triangle\nrotation = 180\nposition = -36w, 0\nsize = 100h\ncolor = 0\n\n;========================== Left text\n[FFI Speed value]\ntype = value\ndata = speed\nsize = 20h\nposition = -41w, 25h\nanchor = left\nunit = show, auto\ndecimals = 0\n\n[FFI v-Speed value]\ntype = value\ndata = vertical speed\nsize = 20h\nposition = -37w, 0\nanchor = left\nunit = show, auto\ndecimals = 0\n\n;========================== Right bars\n[FFI Handbrake checkbox]\ntype = bar\ndata = landing gears\nsize = 10w, 7h;\ncolor = 254, 94, 0\nbackground = 50\nposition = 50w, 24h\nanchor = right\n\n[FFI Landing gears checkbox]\ntype = bar\ndata = handbrake\nsize = 12w, 7h;\ncolor = 254, 94, 0\nbackground = 50\nposition = 50w, -1h\nanchor = right\n\n[FFI Lift checkbox]\ntype = bar\ndata = lift\nsize = 14w, 7h;\ncolor = 254, 94, 0\nbackground = 50\nposition = 50w, -26h\nanchor = right\nmin = 0\nmax = 1\n\n;========================== Black triangle\n\n[FFI Right angle triangle]\ntype = sprite\nsprite = Triangle\nrotation = 180\nposition = 36w, 0\nsize = 100h\ncolor = 0\n\n;========================== Right text\n[FFI Landing gears text]\ntype = text\ntext = L-gear\nsize = 20h\nanchor = right\nposition = 41w, 25h\n\n[FFI Handbrake text]\ntype = text\ntext = E-brake\nsize = 20h\nanchor = right\nposition = 37w, 0\n\n[FFI Lift  text]\ntype = text\ntext = Thrusters\nsize = 20h\nanchor = right\nposition = 34w, -25h\ndata = lift\nhide = > 0\n\n[FFI Lift val]\ntype = value\ndata = lift\nsize = 20h\nanchor = right\nposition = 34w, -25h\nhide = == 0\nunit = show\n"}
};}
class MeterAction:IMeter{MeterDefinition def;Dictionary<Data, IData>shipData;List<IMyTerminalBlock>blocks=new List<IMyTerminalBlock>();bool conditionMet=true;bool UseDataMinMax=false;double total=0;double val=0;
public MeterAction(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData, List<IMyTerminalBlock>blocks, IMyGridTerminalSystem gts, IMyProgrammableBlock me){this.def=def;this.shipData=shipData;
if(def.textData=="")def.textData="OnOff";if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}foreach(string text in def.blocks){if(text.StartsWith("*")&& text.EndsWith("*")){var group=gts.GetBlockGroupWithName(text.Trim('*'));
if(group !=null){var groupBlocks=new List<IMyTerminalBlock>();group.GetBlocks(groupBlocks);foreach(var block in groupBlocks){if(block.IsSameConstructAs(me))this.blocks.Add(block);}}}else{foreach(var block in blocks)
{if(block.CustomName==text){this.blocks.Add(block);break;}}}}}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if((dataChanged & def.data)!=0){val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);
if(Ini.MeetsConditions(def.conditions, val, def.condVals)){if(!conditionMet)Execute();conditionMet=true;}else{conditionMet=false;}}}private void Execute(){try{foreach(var block in blocks)block.ApplyAction(def.textData);
}finally{}}}
class MeterBar:IMeter{MySprite sprite;MySprite background;SurfaceMath sm;MeterDefinition def;Dictionary<Data, IData>shipData;double total=0;Vector2 spriteSize=Vector2.Zero;bool UseDataMinMax=false;double val;
public MeterBar(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData){this.sm=sm;sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;def.position+=sm.Center;if(def.min==0 && def.max==0)
{UseDataMinMax=true;}else{total=def.max-def.min;}sprite=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:def.color);sprite.Position=sm.AdjustToRotation(sm.AdjustToAnchor(def.anchor, def.position, def.size), def.position, def.rotation);
spriteSize.Y=def.size.Y;sprite.RotationOrScale=def.rotation;if(def.backgroundSet){background=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:def.background);background.Position=sm.AdjustToRotation(sm.AdjustToAnchor(def.anchor, def.position, def.size), def.position, def.rotation);
background.Size=def.size;background.RotationOrScale=def.rotation;}}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if(UseDataMinMax){def.min=shipData[def.data].Min;def.max=shipData[def.data].Max;
total=def.max-def.min;}val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;if((dataChanged & def.data)!=0){spriteSize.X=(float)Math.Abs(MathHelper.Clamp(shipData[def.data].Value, def.min, def.max)/total*def.size.X);
sprite.Position=sm.AdjustToRotation(sm.AdjustToAnchor(def.anchor, def.position, spriteSize), def.position, def.rotation);sprite.Size=spriteSize;if(def.backgroundSet)frame.Add(background);frame.Add(sprite);
}else{sprite.Size=spriteSize;if(def.backgroundSet)frame.Add(background);frame.Add(sprite);}}}
class MeterHalfMeter:IMeter{MySprite full;MySprite semi;MySprite box;MySprite inner;SurfaceMath sm;MeterDefinition def;Dictionary<Data, IData>shipData;double total=0;Color background;bool UseDataMinMax=false;
double val;public MeterHalfMeter(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData, Color background){this.sm=sm;sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;this.background=background;
def.position+=sm.Center;if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}float innerSize=MathHelper.Clamp(1-def.stroke, 0 , 1);def.size.Y=def.size.X;full=new MySprite(SpriteType.TEXTURE, "Circle", def.position, color:def.color);
semi=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, def.size);box=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:background);inner=new MySprite(SpriteType.TEXTURE, "Circle", def.position, def.size*innerSize, color:background);
if(def.backgroundSet){semi.Color=def.background;full.Size=def.size;}else{semi.Color=background;full.Size=def.size*0.97f;}box.RotationOrScale= def.rotation;box.Size=new Vector2(def.size.X, def.size.Y*0.5f);
box.Position=sm.AdjustToRotation(new Vector2(def.position.X, def.position.Y+def.size.Y*0.25f), def.position, def.rotation);}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if(UseDataMinMax)
{def.min=shipData[def.data].Min;def.max=shipData[def.data].Max;total=def.max-def.min;}val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;
if((dataChanged & def.data)!=0){semi.RotationOrScale=MathHelper.ToRadians(MathHelper.Clamp((float)(shipData[def.data].Value/total*180f), 0, 180))+def.rotation;frame.Add(full);frame.Add(semi);frame.Add(inner);
frame.Add(box);}else{frame.Add(full);frame.Add(semi);frame.Add(inner);frame.Add(box);}}}
class MeterLineGraph:IMeter{const int LINES=10;const float SECTION=0.1f;MySprite[] sprites=new MySprite[LINES];MySprite background;SurfaceMath sm;MeterDefinition def;Dictionary<Data, IData>shipData;double total=0;
double[] values=new double[LINES];int valueIndex=0;float thickness=5;Vector2 pos;bool UseDataMinMax=false;double val;public MeterLineGraph(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData)
{this.sm=sm;sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;def.position+=sm.Center;if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}for(int i=0;i<LINES;i++){
sprites[i]=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:def.color);}pos=def.position;pos.X-=def.size.X*0.5f-(def.size.X*SECTION*0.5f);pos.Y+=def.size.Y*0.5f-(thickness*0.5f);def.size.Y-=thickness;
valueIndex=LINES-1;if(def.backgroundSet){background=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:def.background);background.Position=sm.AdjustToRotation(sm.AdjustToAnchor(def.anchor, def.position, def.size), def.position, def.rotation);
background.Size=def.size;background.RotationOrScale=def.rotation;}}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if(UseDataMinMax){def.min=shipData[def.data].Min;def.max=shipData[def.data].Max;
total=def.max-def.min;}val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;if((dataChanged & def.data)!=0){values[valueIndex]=shipData[def.data].Value;
if(def.backgroundSet)frame.Add(background);int prev=LINES-1;int value=(valueIndex+1)%LINES;Vector2 prevPos=sm.AdjustToRotation(new Vector2((pos.X),(float)(pos.Y-values[value]/total*def.size.Y)), def.position, def.rotation);
for(int i=0;i<LINES;i++){value=(valueIndex+i+1)%LINES;Vector2 newPos=sm.AdjustToRotation(new Vector2((pos.X+def.size.X*SECTION*i),(float)(pos.Y-values[value]/total*def.size.Y)), def.position, def.rotation);
sprites[i].Position=Vector2.Lerp(newPos, prevPos, 0.5f);sprites[i].Size=new Vector2(Vector2.Distance(prevPos, newPos), thickness);sprites[i].RotationOrScale=(float)Math.Atan2(newPos.Y-prevPos.Y, newPos.X-prevPos.X);
if(values[value]<=def.max && value>=def.min)frame.Add(sprites[i]);prevPos=newPos;prev=i;}valueIndex++;if(valueIndex>=LINES)valueIndex=0;}else{if(def.backgroundSet)frame.Add(background);for(int i=0;i<LINES;i++)
{frame.Add(sprites[i]);}}}}
class MeterMeter:IMeter{MySprite full;MySprite semi;MySprite top;MySprite bottom;MySprite inner;SurfaceMath sm;MeterDefinition def;Dictionary<Data, IData>shipData;double total=0;Color background;bool UseDataMinMax=false;
double val;public MeterMeter(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData, Color background){this.sm=sm;sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;this.background=background;
def.position+=sm.Center;if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}float innerSize=MathHelper.Clamp(1-def.stroke, 0, 1);def.size.Y=def.size.X;full=new MySprite(SpriteType.TEXTURE, "Circle", def.position, color:def.color);
semi=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, def.size);top=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, color:def.color);bottom=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, def.size);
inner=new MySprite(SpriteType.TEXTURE, "Circle", def.position, def.size*innerSize, color:background);if(def.backgroundSet){semi.Color=def.background;bottom.Color=def.background;full.Size=def.size;top.Size=def.size;
}else{semi.Color=background;bottom.Color=background;full.Size=def.size*0.97f;top.Size=def.size*0.97f;}top.RotationOrScale=def.rotation;bottom.RotationOrScale=MathHelper.ToRadians(180f)+def.rotation;}public void Draw(MySpriteDrawFrame frame, Data dataChanged)
{if(UseDataMinMax){def.min=shipData[def.data].Min;def.max=shipData[def.data].Max;total=def.max-def.min;}val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;
if((dataChanged & def.data)!=0){val=MathHelper.Clamp(shipData[def.data].Value, def.min, def.max)/total;semi.RotationOrScale=(float)MathHelper.ToRadians(val*360f)+def.rotation;frame.Add(full);if(val>0.5)
{frame.Add(semi);frame.Add(top);}else{frame.Add(semi);frame.Add(bottom);}frame.Add(inner);}else{frame.Add(full);if(val>0.5){frame.Add(semi);frame.Add(top);}else{frame.Add(semi);frame.Add(bottom);}frame.Add(inner);
}}}
class MeterSprite:IMeter{MySprite sprite;MeterDefinition def;Dictionary<Data, IData>shipData;bool UseDataMinMax=false;double total=0;double val=0;public MeterSprite(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData)
{sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;def.position+=sm.Center;if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}sprite=new MySprite(SpriteType.TEXTURE, def.textData, color:def.color);
sprite.Size=def.size;sprite.Position=sm.AdjustToRotation(sm.AdjustToAnchor(def.anchor, def.position, def.size), def.position, def.rotation);sprite.RotationOrScale=def.rotation;}public void Draw(MySpriteDrawFrame frame, Data dataChanged)
{if(def.data !=Data.None){val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;}frame.Add(sprite);}}
class MeterText:IMeter{MySprite sprite;MeterDefinition def;Dictionary<Data, IData>shipData;bool UseDataMinMax=false;double total=0;double val=0;public MeterText(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData)
{sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}if(def.size.X==0)def.size.X=1;def.position+=sm.Center;def.position.Y-=sm.TextHeight(def.size.X)*0.5f;
sprite=MySprite.CreateText(def.textData, "Debug", def.color, def.size.X);sprite.Position=def.position;switch(def.anchor){case Anchor.Left:sprite.Alignment=TextAlignment.LEFT;break;case Anchor.Right:sprite.Alignment=TextAlignment.RIGHT;
break;case Anchor.Center:default:sprite.Alignment=TextAlignment.CENTER;break;}}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if(def.data !=Data.None){val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);
if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;}frame.Add(sprite);}}
class MeterThreeQuaterMeter:IMeter{MySprite full;MySprite semi;MySprite top;MySprite bottom;MySprite inner;MySprite triangle;SurfaceMath sm;MeterDefinition def;Dictionary<Data, IData>shipData;double total=0;
Color background;bool UseDataMinMax=false;double val;public MeterThreeQuaterMeter(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData, Color background){this.sm=sm;sm.PostionInterpreter(def);
this.def=def;this.shipData=shipData;this.background=background;def.position+=sm.Center;if(def.min==0 && def.max==0){UseDataMinMax=true;}else{total=def.max-def.min;}float innerSize=MathHelper.Clamp(1-def.stroke, 0, 1);
def.size.Y=def.size.X;full=new MySprite(SpriteType.TEXTURE, "Circle", def.position, color:def.color);semi=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, def.size);top=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, color:def.color);
bottom=new MySprite(SpriteType.TEXTURE, "SemiCircle", def.position, def.size);inner=new MySprite(SpriteType.TEXTURE, "Circle", def.position, def.size*innerSize, color:background);triangle=new MySprite(SpriteType.TEXTURE, "RightTriangle", def.position, def.size*innerSize, color:background);
triangle.RotationOrScale=MathHelper.ToRadians(135)+def.rotation;triangle.Position=sm.AdjustToRotation(new Vector2(def.position.X, def.position.Y+def.size.Y*0.51f), def.position, def.rotation);if(def.backgroundSet)
{semi.Color=def.background;bottom.Color=def.background;full.Size=def.size;top.Size=def.size;}else{semi.Color=background;bottom.Color=background;full.Size=def.size*0.97f;top.Size=def.size*0.97f;}top.RotationOrScale=def.rotation+MathHelper.ToRadians(-90);
bottom.RotationOrScale=MathHelper.ToRadians(180-90)+def.rotation;}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if(UseDataMinMax){def.min=shipData[def.data].Min;def.max=shipData[def.data].Max;
total=def.max-def.min;}val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;if((dataChanged & def.data)!=0){val=MathHelper.Clamp(shipData[def.data].Value, def.min, def.max)/total;
semi.RotationOrScale=(float)MathHelper.ToRadians((val*270)-46f)+def.rotation;frame.Add(full);if(val>0.5){frame.Add(semi);frame.Add(top);}else{frame.Add(semi);frame.Add(bottom);}frame.Add(inner);frame.Add(triangle);
}else{frame.Add(full);if(val>0.5){frame.Add(semi);frame.Add(top);}else{frame.Add(semi);frame.Add(bottom);}frame.Add(inner);frame.Add(triangle);}}}
class MeterValue:IMeter{MySprite sprite;MeterDefinition def;Dictionary<Data, IData>shipData;double total=0;string unit="";double abs=0;double val;bool UseDataMinMax=false;public MeterValue(SurfaceMath sm, MeterDefinition def, Dictionary<Data, IData>shipData)
{sm.PostionInterpreter(def);this.def=def;this.shipData=shipData;if(def.size.X==0)def.size.X=1;def.position+=sm.Center;def.position.Y-=sm.TextHeight(def.size.X)*0.5f;if(def.min==0 && def.max==0){UseDataMinMax=true;
}else{total=def.max-def.min;}sprite=MySprite.CreateText(def.textData, "Debug", def.color, def.size.X);sprite.Position=def.position;switch(def.anchor){case Anchor.Left:sprite.Alignment=TextAlignment.LEFT;
break;case Anchor.Right:sprite.Alignment=TextAlignment.RIGHT;break;case Anchor.Center:default:sprite.Alignment=TextAlignment.CENTER;break;}if(def.showUnit){switch(def.unit){case Units.Percent:unit="%";
break;case Units.Kilo:unit="k";break;case Units.Mega:unit="M";break;case Units.Giga:unit="G";break;case Units.Tera:unit="T";break;}}}public void Draw(MySpriteDrawFrame frame, Data dataChanged){if(def.data !=Data.None)
{val=Ini.AdjustToUnit(def, shipData, total, UseDataMinMax);if(Ini.MeetsConditions(def.conditions, val, def.condVals))return;}if((dataChanged & def.data)!=0){if(def.unit==Units.Percent){if(UseDataMinMax)total=shipData[def.data].Max-shipData[def.data].Min;
val=shipData[def.data].Value/total*100;if(double.IsNaN(val))sprite.Data="--";else if(double.IsInfinity(val))sprite.Data="••";else sprite.Data=string.Format(def.decimalFormat, val);if(def.showUnit)sprite.Data+=unit;
}else if(def.unit==Units.Auto && def.data==Data.HydrogenTime){if(double.IsNaN(shipData[def.data].Value))sprite.Data="--";else if(double.IsInfinity(shipData[def.data].Value))sprite.Data="••";else{TimeSpan time=TimeSpan.FromSeconds(shipData[def.data].Value);
if(def.showUnit){if(time.TotalDays>=1){sprite.Data=string.Format("{0,2}d {1,2}h",(long)time.TotalDays,(long)time.Hours);}else if(time.TotalHours>=1){sprite.Data=string.Format("{0,2}h {1,2}m",(long)time.TotalHours,(long)time.Minutes);
}else{sprite.Data=string.Format("{0,2}m {1,2}s",(long)time.TotalMinutes,(long)time.Seconds);}}else{sprite.Data=string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}",(long)time.TotalHours,(long)time.Minutes,(long)time.Seconds);
}}}else{if(double.IsNaN(shipData[def.data].Value))sprite.Data="--";else if(double.IsInfinity(shipData[def.data].Value))sprite.Data="••";else sprite.Data=string.Format(def.decimalFormat, Ini.ConvertTo(def.unit, shipData[def.data].Value));
if(def.showUnit){if(def.unit==Units.Auto){abs=Math.Abs(shipData[def.data].Value);if(abs>=1000000000000)unit="T";else if(abs>=1000000000)unit="G";else if(abs>=1000000)unit="M";else if(abs>=1000)unit="k";
else unit="";}sprite.Data+=unit+shipData[def.data].Unit;}}frame.Add(sprite);}else{frame.Add(sprite);}}}
class Profiler{double[] lastRuntimes;long count=0;int sampleSize;IMyProgrammableBlock Me;bool hasPrinted=false;public double Avrage{get;private set;}public double Peak{get;private set;}public Profiler(IMyProgrammableBlock Me, int sampleSize=60, int waitCycles=0)
{this.Me=Me;this.sampleSize=sampleSize;lastRuntimes=new double[sampleSize];count-=waitCycles;}public float Update(double lastRuntimeMs){if(count>=sampleSize){if(!hasPrinted){Avrage=lastRuntimes.Average();
Peak=lastRuntimes.Max();hasPrinted=true;Me.CustomData=string.Join("\n", lastRuntimes.Select(p=>p.ToString()));}return 1f;}else if(count<0){count++;return 0f;}lastRuntimes[count]=lastRuntimeMs;count++;return(float)count/sampleSize;
}}
class Skin{Program P;IMyTextSurface Surface;IMyTerminalBlock Block;List<IMeter>Meters;SurfaceMath SM;SkinDefinition SkinDef;Data DataInUse;public Skin(Program P, IMyTextSurface surface, SurfaceMath surfaceMath, IMyTerminalBlock block, SkinDefinition skinDefinition, List<IMeter>meters)
{this.P=P;Surface=surface;SM=surfaceMath;Block=block;SkinDef=skinDefinition;Meters=meters;foreach(var meter in SkinDef.meters){DataInUse |=meter.data;}Surface.ContentType=ContentType.SCRIPT;Surface.Script="";
if(skinDefinition.backgroundColorSet)Surface.ScriptBackgroundColor=skinDefinition.backgroundColor;}public void Draw(Data dataChanged){if(Meters.Count==0){using(var frame=Surface.DrawFrame()){MySprite sprite;
sprite=new MySprite(SpriteType.TEXTURE, "UVChecker", color:Color.White);sprite.Size=SM.BGSize;sprite.Position=SM.Center;frame.Add(sprite);sprite=new MySprite(SpriteType.TEXTURE, "SquareSimple", color:Color.Red);
sprite.Size=new Vector2(110, 110);sprite.Position=SM.Center;frame.Add(sprite);MySprite textSprite;textSprite=MySprite.CreateText($"{SkinDef.screenId+1}", "Debug", Color.White, 3f, TextAlignment.CENTER);
Vector2 center=SM.Center;center.Y-=SM.TextHeight(3f)*0.5f;center.Y+=12f;textSprite.Position=center;frame.Add(textSprite);textSprite=MySprite.CreateText("Screen", "Debug", Color.White, 1f, TextAlignment.CENTER);
center=SM.Center;center.Y-=SM.TextHeight(4f)*0.5f;center.Y+=12f;textSprite.Position=center;frame.Add(textSprite);textSprite=MySprite.CreateText(Surface.SurfaceSize.X.ToString("#.#")+" x "+Surface.SurfaceSize.Y.ToString("#.#"), "Debug", Color.White, 1f, TextAlignment.LEFT);
textSprite.Position=SM.TopLeft;frame.Add(textSprite);}return;}if(true ||(DataInUse & dataChanged)!=0){using(var frame=Surface.DrawFrame()){if(SkinDef.background !=null){MySprite sprite;sprite=new MySprite(SpriteType.TEXTURE, SkinDef.background);
if(SkinDef.colorSet){sprite.Color=SkinDef.color;}else{sprite.Color=Surface.ScriptForegroundColor;}sprite.Size=SM.BGSize;sprite.Position=SM.Center;frame.Add(sprite);}foreach(var meter in Meters){meter.Draw(frame, dataChanged);
}}}}}
enum Anchor{Center,Left,Right}class SurfaceMath{public Vector2 Size;public Vector2 TopLeft;public Vector2 BGSize;public Vector2 Center;public float SmallestSize;public SurfaceMath(IMyTextSurface surface)
{if(surface.SurfaceSize.X>surface.SurfaceSize.Y){BGSize=new Vector2(surface.SurfaceSize.X, surface.SurfaceSize.X);SmallestSize=surface.SurfaceSize.Y;}else{BGSize=new Vector2(surface.SurfaceSize.Y, surface.SurfaceSize.Y);
SmallestSize=surface.SurfaceSize.X;}Size=surface.SurfaceSize;TopLeft=(surface.TextureSize-surface.SurfaceSize)*0.5f;Center=surface.TextureSize*0.5f;}public void PostionInterpreter(MeterDefinition def){
switch(def.posType.X){case VectorType.ViewWidth:def.position.X=Size.X*def.position.X*0.01f;break;case VectorType.ViewHeight:def.position.X=Size.Y*def.position.X*0.01f;break;case VectorType.ViewMin:def.position.X=SmallestSize*def.position.X*0.01f;
break;}switch(def.posType.Y){case VectorType.ViewWidth:def.position.Y=Size.X*def.position.Y*0.01f;break;case VectorType.ViewHeight:def.position.Y=Size.Y*def.position.Y*0.01f;break;case VectorType.ViewMin:
def.position.Y=SmallestSize*def.position.Y*0.01f;break;}def.position.Y=def.position.Y*-1;if(def.type==Meter.Text || def.type==Meter.Value){switch(def.sizeType.X){case VectorType.ViewWidth:def.size.X=Size.X/30.6f*def.size.X*0.01f;
break;case VectorType.ViewHeight:def.size.X=Size.Y/30.6f*def.size.X*0.01f;break;case VectorType.ViewMin:def.size.X=SmallestSize/30.6f*def.size.X*0.01f;break;}}else{switch(def.sizeType.X){case VectorType.ViewWidth:
def.size.X=Size.X*def.size.X*0.01f;break;case VectorType.ViewHeight:def.size.X=Size.Y*def.size.X*0.01f;break;case VectorType.ViewMin:def.size.X=SmallestSize*def.size.X*0.01f;break;}switch(def.sizeType.Y)
{case VectorType.ViewWidth:def.size.Y=Size.X*def.size.Y*0.01f;break;case VectorType.ViewHeight:def.size.Y=Size.Y*def.size.Y*0.01f;break;case VectorType.ViewMin:def.size.Y=SmallestSize*def.size.Y*0.01f;
break;}}}public float TextHeight(float scale, string text){int count=1;foreach(char c in text)if(c=='\n')count++;return count*scale*30.6f;}public float TextHeight(float scale, int lines=1){return lines*scale*30.6f;
}public Vector2 AdjustToAnchor(Anchor anchor, Vector2 position, Vector2 size){switch(anchor){case Anchor.Left:position.X=position.X+size.X*0.5f;break;case Anchor.Right:position.X=position.X-size.X*0.5f;
break;}return position;}public Vector2 AdjustToRotation(Vector2 position, Vector2 rotateAround, float rotation){var vec=Vector2.Zero;vec.X=(float)(Math.Cos(rotation)*(position.X-rotateAround.X)-Math.Sin(rotation)*(position.Y-rotateAround.Y)+rotateAround.X);
vec.Y=(float)(Math.Sin(rotation)*(position.X-rotateAround.X)+Math.Cos(rotation)*(position.Y-rotateAround.Y)+rotateAround.Y);return vec;}}
