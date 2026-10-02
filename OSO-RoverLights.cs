/*****************************************************************************
OSO - Rover Lights
  Stop, Back, Turn lights, & HandBrake.
Author: Old Smuggler
Date: 2022.09.11
*****************************************************************************/
string CockpitName = "Industrial Cockpit";// Cockpit Name.
//----------------------------------------------------------------------------
string StopGroup="Stop lights";// Stop lights Group Name.
string TurnGroup="Turn lights";// Turn lights Group Name.
//============================================================================
IMyShipController Cockpit;
IMyBlockGroup group;
List<IMyLightingBlock> Stop;
List<IMyLightingBlock> Turn;
//============================================================================
public Program(){
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
  Cockpit = GridTerminalSystem.GetBlockWithName(CockpitName) as IMyShipController;
  Stop=new List<IMyLightingBlock>();
  Turn=new List<IMyLightingBlock>();
  
  group = GridTerminalSystem.GetBlockGroupWithName(StopGroup);
  if(group!=null){
    GridTerminalSystem.GetBlockGroupWithName(StopGroup).GetBlocksOfType<IMyLightingBlock>(Stop);}

  group = GridTerminalSystem.GetBlockGroupWithName(TurnGroup);
  if(group!=null){
    GridTerminalSystem.GetBlockGroupWithName(TurnGroup).GetBlocksOfType<IMyLightingBlock>(Turn);}
}
//============================================================================
void Main(){
//----------------------------------------------------------------------------
if (Cockpit.IsUnderControl == false){
  Cockpit.HandBrake=true;}
//----------------------------------------------------------------------------
foreach(var light in Stop){
  if(Cockpit.MoveIndicator.Y>0.4){
    light.Radius=2f;
    light.Intensity=5f;
    light.Falloff=1.3f;
    light.Color=Color.Red;}
  else if(Cockpit.MoveIndicator.Z>0.5){
    light.Radius=5f;
    light.Intensity=5f;
    light.Falloff=1.3f;
    light.Color=Color.White;}
  else{
    light.Radius=1f;
    light.Intensity=1f;
    light.Falloff=0;
    light.Color=Color.DarkRed;}}
//----------------------------------------------------------------------------
foreach(var light in Turn){
  if(light.CustomData.Equals("LEFT",StringComparison.OrdinalIgnoreCase)){
    if(Cockpit.MoveIndicator.X<0)
      light.Enabled=true;
    else
      light.Enabled=false;}
  else if(light.CustomData.Equals("RIGHT",StringComparison.OrdinalIgnoreCase)){
    if(Cockpit.MoveIndicator.X>0)
      light.Enabled=true;
    else
      light.Enabled=false;}}
//----------------------------------------------------------------------------
}
//============================================================================