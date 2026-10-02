/*
 * **Readme**
 * -----------
 * Do you dislike having to search for a button to cycle an airlock?
 * Do you dislike having to make room for buttons to cycle an airlock?
 * Do you simply want to interact with the door and be done with it?
 * 
 * Look no further; this script is buttonless, sensorless, and relatively hassle-free in terms of setup!
 * 
 * 
 * **How To**
 * ----------
 * 
 * Group two doors and an air vent together.
 * Add "#AL" to the name of the exterior door.
 * Recompile the script.
 * 
 * "#AL" can be changed to your preference by modifying the "variable outerDoorTag":
 * ```
 * string outerDoorTag = "My Sweet New Tag";
 * ```
 * 
 * Please feel free to inform me about any problems you encounter or features that are missing.
 */

string outerDoorTag = "#AL";
float autoCloseDelay = 1.5f;

/**
		POTENTIAL TODOS
			- Timeout if cant pressurize/depressurize
			- Airventless mode  aka "simple airlock"
			- Remove group dependency
			- Multiple Doors
			- Multiple Airvents
			- Lights
			- LCD
			- Custom data of PB list airblock group and delays is customizable per airlock



		No Touchy Below
		*/
List<Airlock>airlocks=new List<Airlock>();Utility utility;Dictionary<Airlock.Phase,Action<Airlock>>stateHandlers=new
Dictionary<Airlock.Phase,Action<Airlock>>();Program(){utility=new Utility(this,Me);utility.Print("Airlock Manager (AM)");utility.
Print(utility.version());init();utility.Print($"Managed Airlocks : {airlocks.Count}");Runtime.UpdateFrequency=UpdateFrequency
.Update10;}void init(){InitializeStateHandlers();List<IMyBlockGroup>groups=new List<IMyBlockGroup>();GridTerminalSystem.
GetBlockGroups(groups);foreach(IMyBlockGroup group in groups){if(isGroupAnAirlock(group)){List<IMyTerminalBlock>blocks=new List<
IMyTerminalBlock>();group.GetBlocks(blocks);List<IMyDoor>outerDoor=new List<IMyDoor>();List<IMyDoor>innerDoor=new List<IMyDoor>();List<
IMyAirVent>vent=new List<IMyAirVent>();foreach(IMyTerminalBlock block in blocks){if(!block.IsSameConstructAs(Me)){break;}if(block
is IMyDoor&&block.CustomName.Contains(outerDoorTag)){outerDoor.Add(block as IMyDoor);}else if(block is IMyDoor){innerDoor.
Add(block as IMyDoor);}else if(block is IMyAirVent){vent.Add(block as IMyAirVent);}}if(outerDoor.Count==0||innerDoor.Count
==0||vent.Count==0){utility.Print($"\nError With Group {group.Name} : \n Outer door : {outerDoor.Count}\n Inner door : {innerDoor.Count}\n Vents : {vent.Count}\n"
);Runtime.UpdateFrequency=UpdateFrequency.None;}else{airlocks.Add(new Airlock(this,outerDoor[0],innerDoor[0],vent,utility
));}}}}bool isGroupAnAirlock(IMyBlockGroup group){List<IMyDoor>doors=new List<IMyDoor>();List<IMyAirVent>vents=new List<
IMyAirVent>();group.GetBlocksOfType(doors);group.GetBlocksOfType(vents);return doors.Count>1&&vents.Count>0;}void
InitializeStateHandlers(){stateHandlers[Airlock.Phase.Idle]=HandleIdlePhase;stateHandlers[Airlock.Phase.PressurizationTransition]=
HandlePressurizationTransitionPhase;stateHandlers[Airlock.Phase.ClosingTransition]=HandleClosingTransitionPhase;stateHandlers[Airlock.Phase.CycleDone]=
HandleCycleDonePhase;stateHandlers[Airlock.Phase.Idling]=HandleIdlingPhase;stateHandlers[Airlock.Phase.Timedout]=HandleTimedOut;}void Save()
{}void Main(string argument,UpdateType updateSource){if((updateSource&(UpdateType.Update10))!=0){foreach(Airlock airlock
in airlocks){stateHandlers[airlock.currentPhase](airlock);}}}void HandleIdlePhase(Airlock airlock){if(airlock.
GetDoorStatus(airlock.outerDoor)==DoorStatus.Opening){airlock.DisableBlock(airlock.innerDoor);airlock.Depressurize();airlock.
cycleType=Airlock.CycleType.In;}else if(airlock.GetDoorStatus(airlock.innerDoor)==DoorStatus.Opening){airlock.DisableBlock(
airlock.outerDoor);airlock.Pressurize();airlock.cycleType=Airlock.CycleType.Out;}if(airlock.cycleType!=Airlock.CycleType.None){
airlock.timer=utility.CalculateGameTicks(autoCloseDelay);airlock.currentPhase=Airlock.Phase.ClosingTransition;}}void
HandleClosingTransitionPhase(Airlock airlock){if(airlock.GetDoorStatus(airlock.outerDoor)!=DoorStatus.Closed||airlock.GetDoorStatus(airlock.
innerDoor)!=DoorStatus.Closed){if((airlock.GetDoorStatus(airlock.outerDoor)==DoorStatus.Open||airlock.GetDoorStatus(airlock.
innerDoor)==DoorStatus.Open)&&utility.timeout(ref airlock.timer)){airlock.timer=utility.CalculateGameTicks(autoCloseDelay);
airlock.CloseDoors();}return;}if(airlock.cycleType==Airlock.CycleType.In){airlock.Pressurize();}else if(airlock.cycleType==
Airlock.CycleType.Out){airlock.Depressurize();}airlock.currentPhase=Airlock.Phase.PressurizationTransition;airlock.DisableDoors
();}void HandlePressurizationTransitionPhase(Airlock airlock){if((airlock.cycleType==Airlock.CycleType.In&&airlock.
IsPressurized())||(airlock.cycleType==Airlock.CycleType.Out&&airlock.IsDepressurized())){if(airlock.cycleType==Airlock.CycleType.In){
airlock.OpenDoor(airlock.innerDoor);}else if(airlock.cycleType==Airlock.CycleType.Out){airlock.OpenDoor(airlock.outerDoor);}
airlock.DisableAirvents();airlock.timer=utility.CalculateGameTicks(autoCloseDelay);airlock.currentPhase=Airlock.Phase.CycleDone
;}}void HandleCycleDonePhase(Airlock airlock){if(airlock.GetDoorStatus(airlock.outerDoor)!=DoorStatus.Closed||airlock.
GetDoorStatus(airlock.innerDoor)!=DoorStatus.Closed){if((airlock.GetDoorStatus(airlock.outerDoor)==DoorStatus.Open||airlock.
GetDoorStatus(airlock.innerDoor)==DoorStatus.Open)&&utility.timeout(ref airlock.timer)){airlock.timer=utility.CalculateGameTicks(
autoCloseDelay);airlock.CloseDoors();}return;}airlock.DisableDoors();airlock.Depressurize();airlock.currentPhase=Airlock.Phase.Idling;
}void HandleIdlingPhase(Airlock airlock){if(!airlock.IsDepressurized()){return;}airlock.EnableDoors();airlock.
DisableAirvents();airlock.cycleType=Airlock.CycleType.None;airlock.currentPhase=Airlock.Phase.Idle;}void HandleTimedOut(Airlock airlock
){airlock.TimeoutWarning();}class Airlock{Utility utility;Program program;public IMyDoor outerDoor;public IMyDoor
innerDoor;public List<IMyDoor>doorList;public List<IMyAirVent>ventList;public Phase currentPhase=Phase.Idling;public CycleType
cycleType=CycleType.None;public int timer=0;private int timeoutDelay=4;private int timeOutBlinkAmount=24;private int
timeOutblinkCount=0;public enum CycleType{None,In,Out}public enum Phase{Idle,ClosingTransition,PressurizationTransition,CycleDone,Idling,
Timedout}public Airlock(Program p,IMyDoor outerDoor,IMyDoor innerDoor,List<IMyAirVent>ventList,Utility utility){this.program=p;
this.outerDoor=outerDoor;this.innerDoor=innerDoor;this.ventList=ventList;this.utility=utility;doorList=new List<IMyDoor>(){
innerDoor,innerDoor};timeOutblinkCount=timeOutBlinkAmount;}public DoorStatus GetDoorStatus(IMyDoor door){return door.Status;}
public void OpenDoor(IMyDoor door){EnableBlock(door);door.OpenDoor();}public void CloseDoors(){outerDoor.CloseDoor();innerDoor
.CloseDoor();}public void EnableDoors(){EnableBlock(outerDoor);EnableBlock(innerDoor);}public void DisableDoors(){
DisableBlock(outerDoor);DisableBlock(innerDoor);}public void EnableBlock(IMyFunctionalBlock block){block.Enabled=true;}public void
DisableBlock(IMyFunctionalBlock block){block.Enabled=false;}public void Depressurize(){foreach(IMyAirVent vent in ventList){
EnableBlock(vent);vent.Depressurize=true;}}public void Pressurize(){foreach(IMyAirVent vent in ventList){EnableBlock(vent);vent.
Depressurize=false;}}private bool timeoutActive=false;private bool HasTimedOut(){if(!timeoutActive){timer=utility.CalculateGameTicks
(timeoutDelay);}bool timedOut=utility.timeout(ref timer);timeoutActive=!timedOut;return timedOut;}public bool
IsPressurized(){if(HasTimedOut()){TimeoutWarning();}foreach(IMyAirVent vent in ventList){if(vent.Status!=VentStatus.Pressurized){
return false;}}timeoutActive=false;return true;}public bool IsDepressurized(){if(HasTimedOut()){TimeoutWarning();}double
totalOxygenLevel=0;foreach(IMyAirVent vent in ventList){totalOxygenLevel+=(double)vent.GetOxygenLevel()*100;}bool depressurized=(
totalOxygenLevel/ventList.Count)<1;timeoutActive=!depressurized;return depressurized;}public void DisableAirvents(){foreach(IMyAirVent
vent in ventList){DisableBlock(vent);}}public void EnableAirvents(){foreach(IMyAirVent vent in ventList){EnableBlock(vent);}
}public void TimeoutWarning(){currentPhase=Phase.Timedout;if(!timeoutActive){timer=utility.CalculateGameTicks(0.15);}bool
timedOut=utility.timeout(ref timer);timeoutActive=!timedOut;if(timedOut&&timeOutblinkCount>0){timeOutblinkCount--;timeoutActive=
false;if(timeOutblinkCount%2==0){EnableDoors();}else{DisableDoors();}}else if(timedOut&&timeOutblinkCount==0){timeoutActive=
false;timeOutblinkCount=timeOutBlinkAmount;EnableDoors();DisableAirvents();cycleType=Airlock.CycleType.None;currentPhase=
Airlock.Phase.Idle;}}}class Utility{const string deployTime="2023-09-27 22:45";Program program;IMyProgrammableBlock Me;bool
alreadyPrintedToSurface=false;bool limitedEchoMode=false;public Utility(Program program,IMyProgrammableBlock Me){this.program=program;this.Me=
Me;}public string version(){DateTime dateTime=DateTime.ParseExact(deployTime,"yyyy-MM-dd HH:mm",null);int year=dateTime.
Year%100;int dayOfYear=dateTime.DayOfYear;string timeFormatted=$"{dateTime.Hour:D2}{dateTime.Minute:D2}";string
versionNumber=$"v{year:D2}-{dayOfYear:D3}-{timeFormatted}";return versionNumber;}public void ClearTextSurface(){
alreadyPrintedToSurface=false;}public void PrintToAll(string text,List<IMyTextSurface>surfaces){limitedEchoMode=true;foreach(IMyTextSurface
surface in surfaces){Print(text,surface);limitedEchoMode=false;}}public void Print(string text,IMyTextSurface surface=null){if(
!limitedEchoMode){program.Echo(text);}if(surface!=null){try{Vector2 scale=surface.MeasureStringInPixels(new StringBuilder
(text),surface.Font,1);Vector2 size=surface.SurfaceSize;float newFontSize=(size.X*(1-surface.TextPadding*0.02f))/scale.X;
if(surface.FontSize>newFontSize||!alreadyPrintedToSurface){surface.FontSize=newFontSize;}surface.WriteText(text+'\n',
alreadyPrintedToSurface);alreadyPrintedToSurface=true;}catch(Exception e){program.Echo("Error printing to text surface.");program.Echo(e.
Message);}}}public int ConvertUpdateFrequencyToInt(UpdateFrequency frequency){switch(frequency){case UpdateFrequency.Update1:
return 1;case UpdateFrequency.Update10:return 10;case UpdateFrequency.Update100:return 100;default:return 1;}}public int
CalculateGameTicks(double seconds){int gameTicks=(int)(seconds*ConvertUpdateFrequencyToInt(program.Runtime.UpdateFrequency));return
gameTicks;}public bool timeout(ref int timer){if(timer>0){timer--;}return timer<1;}}