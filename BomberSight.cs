/*
  
  Bomber Sight Script ver 1.2 - Indicates the impact point of the bomb through the camera
  
  Authors: Survival Ready - steamcommunity.com/profiles/76561199069720721/myworkshopfiles/
  TheManInTheIronPailMask - steamcommunity.com/profiles/76561198968339026/myworkshopfiles/
  
  Programm Block (PB) paramaters (case insenitive):

  SIGHT        - switch sight control mode Manual/Auto
  SIGHT Manual - set sight control mode to Manual
  SIGHT Auto   - set sight control mode to Auto
 
  LOCK         - lock target through the camera view
  LOCK X:Y:Z   - lock target at specified coordinates (you can use the SE GPS tag format)
  
  DROP N       - detach N mergers, without N = detach one merger
  
  Note: recompile the script after changing the configuration of blocks in Terminal
  
*/

public string prefix = "Sight";   // prefix for script control blocks
public double maxSpeed = 104.4;   // free fall world speed limit

// sight hinge set
public double sightOffset = 0;   // sight shift from drop point in meters: >0 forward; <0 backward
public bool clockWise = true;     // true = hinge rotation set along course of ship; false = against

// drop mode 
public bool dropAuto = false;     // false = manual drop; true = autodrop by camera raycast
public double maxRange = 3000;    // max target detection range in meters
public double maxRoll = 3;        // max roll angle attack in °, no autodrop if greater, 0 = any
public double maxPitch = 10;      // max pitch angle attack in °, no autodrop if greater, 0 = any
public string[] aimTarget = 
  {"Enemies","Neutral","Owner",}; // remove extra targets in "" (including comma)

// out telemetry on screen
public bool outScreen = true;     // false = off
public string outHead = "-= Bomb Sight =-\n\n";

// flight control: Remote Control, Timer or "Program Block name(command)". Exact block name required
public string flyControl = "";

////////////// DO NOT CHANGE ANYTHING BEYOND THIS POINT ///////////////

IMyShipController shipControl = null;
IMyMotorAdvancedStator sightHinge = null;
IMyTurretControlBlock sightTurret = null;
IMyTimerBlock dropTimer = null;
IMyRemoteControl autoPilot = null;

public List<IMyShipMergeBlock> mergers;
public List<IMyCameraBlock> cameras;
public List<IMyTextSurface> surface;

Vector3D target = Vector3D.Zero;

public Program()
{
    surface = new List<IMyTextSurface>();
    cameras = new List<IMyCameraBlock>();
    mergers = new List<IMyShipMergeBlock>();
    
    var list = new List<IMyTerminalBlock>();
      GridTerminalSystem.SearchBlocksOfName(prefix, list);
      
    foreach(var b in list) 
    {
      if(b is IMyShipController && shipControl == null) {
        shipControl = (IMyShipController)b;
        if(!shipControl.CanControlShip) shipControl = null;
        
      } else if(b is IMyMotorAdvancedStator && sightHinge == null) {
        sightHinge = (IMyMotorAdvancedStator)b;
      
      } else if(b is IMyCameraBlock) {
        var c = (IMyCameraBlock)b;
        cameras.Add(c); c.EnableRaycast = true;

      } else if(b is IMyShipMergeBlock) {
        var m = (IMyShipMergeBlock)b; mergers.Add(m);
        
      } else if(b is IMyTimerBlock && dropTimer == null) {
        dropTimer = (IMyTimerBlock)b;
        
      } else if(b is IMyTurretControlBlock && sightTurret == null) {
        sightTurret = (IMyTurretControlBlock)b;
        
      } else if(b is IMyTextPanel && outScreen) {  
        var p = (IMyTextPanel)b;
        if(!p.Enabled) p.Enabled = true;
        surface.Add((IMyTextSurface)p);
      }
      
      if(outScreen && b is IMyCockpit) {
        var c = (IMyCockpit)b; int i;
        
        if(c.SurfaceCount > 0) {
          for(i = 1; i < 7; i++) {
            if(c.CustomName.Contains($"_{i}")) {
              try {
                surface.Add((IMyTextSurface)c.GetSurface(i-1));
              } catch {
                continue;
              }
            }  
          }  
        }  
      }  
    }

    var morder = mergers.OrderBy(i => i.CustomName);
    foreach(IMyShipMergeBlock m in morder) mergers.Add(m); 
    mergers.RemoveRange(0, mergers.Count/2); 
    
    if(Me.CustomName.Contains(prefix)) {
      surface.Add((IMyTextSurface)Me.GetSurface(0));
    }  
    
    string err = "";
    
    if(shipControl == null) 
    {
      List<IMyShipController> ctrl = new List<IMyShipController>(); 
      GridTerminalSystem.GetBlocksOfType(ctrl, c => c.CanControlShip == true);
      if(ctrl.Count > 0) shipControl = ctrl[0]; else err = "Controller not found\n";
    }

    if(sightHinge == null) err += "Sight: Hinge not found\n";
    
    if(cameras.Count == 0) err += "Sight: Camera not found\n";

    if(err == "" ) {
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
    } else {  
      Runtime.UpdateFrequency = UpdateFrequency.None;
    }
    
    Echo(err);
}

public void Main(string args)
{
    int fwc = -1; // first working camera
    
    for(int i = 0; i < cameras.Count; i++) {
      if(cameras[i].IsWorking){ fwc = i; break; }
    }  
  
    if(!sightHinge.IsWorking || fwc < 0) {
      Runtime.UpdateFrequency = UpdateFrequency.None; 
      outText("Sight damaged",1); return;
    }  

    if(args.Length > 0) 
    {
      string[] arg = args.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);
      
      switch(arg[0].ToLower()) {
        case "sight":
          if(arg.Count() == 1) {
            dropAuto = !dropAuto; 
          } else if(arg[1] == "auto") {  
            dropAuto = true;
          } else {  
            dropAuto = false;
          }
        
          if(!dropAuto) {
            target = Vector3D.Zero;
            if(autoPilot != null && autoPilot.IsAutoPilotEnabled) { 
              autoPilot.SetAutoPilotEnabled(false); autoPilot.ClearWaypoints();
            }  
          }  
          break;
        
        case "drop":
          if(mergers.Count > 0) {
            int i = 1; if(arg.Count() == 2) if(!int.TryParse(arg[1], out i)) i = 1;
            
            foreach(IMyShipMergeBlock m in mergers) {
               if(m.IsWorking && m.IsConnected) {
                 m.Enabled = false; if(--i == 0) break;
               }  
            }
          }
          break;
          
        case "lock":
          if(arg.Count() == 1) {
            target = Vector3D.Zero; MyDetectedEntityInfo info = cameras[fwc].Raycast(maxRange);
            
            if(isTarget(info)) {
               target = (info.HitPosition.HasValue ? info.HitPosition.Value : info.Position);
               cameras[fwc].CustomData = "GPS:Sight:"+vec2str(target); dropAuto = true;
            }
          } else {  
            target = str2vec(arg[1]);
          }
         
          if(target == Vector3D.Zero) break;
         
          if(flyControl != "") 
          {
            string[] fly = flyControl.Split('(');
            IMyTerminalBlock b = GridTerminalSystem.GetBlockWithName(fly[0]);

            if(b is IMyTimerBlock) {
              IMyTimerBlock t = (IMyTimerBlock)b; t.Trigger();
             
            } else if(b is IMyProgrammableBlock) {
              IMyProgrammableBlock p = (IMyProgrammableBlock)b;
              p.TryRun(fly.Count()==1?"":fly[1].Trim(')'));
             
            } else if(b is IMyRemoteControl){
              autoPilot = (IMyRemoteControl)b;
            }
          }
          break;
      }  
    }  

    if(sightTurret != null) {
      if(sightTurret.AIEnabled) {
        outText("Sight under control",1); return;
      }  
    }  

    double altitude; dropAuto = (dropAuto && dropTimer != null);
    
    if(!shipControl.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude)) {
      outText("No planet gravity",1); return;
    }  
    
    MatrixD gridOrientation = shipControl.WorldMatrix;
    Vector3D shipVelocity = shipControl.GetShipVelocities().LinearVelocity;
    Vector3D gravity = shipControl.GetNaturalGravity();
    
    Vector3D up = Vector3D.Negate(gravity);
    Vector3D localDown = new Vector3D(gridOrientation.GetDirectionVector(Base6Directions.Direction.Down));
    Vector3D localForward = new Vector3D(gridOrientation.GetDirectionVector(Base6Directions.Direction.Forward));
    Vector3D left = new Vector3D(gridOrientation.GetDirectionVector(Base6Directions.Direction.Left));

    //Define GNF coordinates
    Vector3D forward;
    if (shipVelocity.Length() != 0) {
        forward = Vector3D.ProjectOnPlane(ref shipVelocity, ref gravity);
    } else if (Math.Abs(Vector3D.Dot(Vector3D.Normalize(localForward), Vector3D.Normalize(gravity))) != 1){
        forward = localForward;
    } else {
        forward = Vector3D.Negate(localDown);
    }
    
    Vector3D normal = Vector3D.Cross(forward, gravity);

    //GNF transformation matrix
    MatrixD GNF = new MatrixD();
    GNF.Down = Vector3D.Normalize(gravity);
    GNF.Left = Vector3D.Normalize(normal);
    GNF.Forward = Vector3D.Normalize(forward);

    //Calculate bomb travel distance
    double vForward = forward.Length();
    double horizontalDistanceTraveled = 0;
    
    double vCourse  = Math.Round(shipVelocity.Dot(shipControl.WorldMatrix.GetDirectionVector(Base6Directions.Direction.Forward)),2);
    double vAngular = Math.Round(shipControl.GetShipVelocities().AngularVelocity.Length(),2);
    double pitchRot = Math.Round(Math.Abs(shipControl.RotationIndicator.X),2);

    double vUp = 0;
    if (shipVelocity.Length() != 0) {
        vUp = Vector3D.ProjectOnVector(ref shipVelocity, ref up).Length() * Vector3D.Dot(Vector3D.Normalize(VRageMath.Vector3D.ProjectOnVector(ref shipVelocity, ref up)), Vector3D.Normalize(up));
    }
    
    // condition for recalculate bomb path (how get pitch speed?)
    bool recalc = (Math.Abs(vCourse) > 0 || Math.Abs(Math.Round(vUp,1)) > 0) || (vAngular > 0 && pitchRot > 0.01);
    
    double offsetAngle = 0;
    double currentAngleH = 0;
    double ftime = (-vUp + Math.Sqrt(Math.Pow(vUp, 2) + 2 * gravity.Length() * altitude))/gravity.Length();   // falling time
    double fcorr = (Math.Sqrt(altitude * gravity.Length() * 2) < maxSpeed ? (vForward/25)*(Math.PI/180) : 0); // low altitude
    
    if (recalc) horizontalDistanceTraveled = ftime * vForward;
    
    // I don't like goto, but it's short 
    if(sightTurret != null) {
      if(sightTurret.IsUnderControl) goto OutText;
    }  
    
    Vector3D towardTargetGNF = new Vector3D(0, -altitude, -horizontalDistanceTraveled);
    Vector3D towardTarget = Vector3D.TransformNormal(Vector3D.Normalize(towardTargetGNF), GNF);

    // fix SE issue
    if(towardTarget.ToString().Contains("NaN")) towardTarget = localDown;
    
    double theta = Vector3D.Angle(localDown, towardTarget) * Math.Sign(Vector3D.Dot(towardTarget, localForward));
    if (Math.Abs(theta) > Math.PI/2.0) theta = Math.PI/2.0;

    // calculate sight offset
    if(sightOffset != 0) {
      offsetAngle = (90-(Math.Atan(altitude/Math.Abs(sightOffset))*(180/Math.PI))) * (Math.PI/180);
      if(sightOffset > 0) offsetAngle = 0-offsetAngle;
    }  
    
    currentAngleH = (sightHinge.Angle - offsetAngle - fcorr) * (clockWise ? 1 : -1);

    if(Math.Abs(theta - currentAngleH) < 0.01 || !recalc) {
      sightHinge.TargetVelocityRad = 0;
    } else {
      sightHinge.TargetVelocityRad = (float)((theta - currentAngleH) * (clockWise ? 1 : -1));
    }

    OutText:
    
    // auto drop by raycast
    double rollAngle = Math.Round(GNF.Down.Dot(shipControl.WorldMatrix.Left)*(180/Math.PI),0);
    double pitchAngle = Math.Round(GNF.Down.Dot(shipControl.WorldMatrix.Forward)*(180/Math.PI),0);

    string text = "", 
           angles = "",
           mode = "Manual", 
           status = "Idle";
    
    if((Math.Abs(rollAngle) <= maxRoll || maxRoll == 0)) angles = $"{rollAngle}°";
    if((Math.Abs(pitchAngle) <= maxPitch || maxPitch == 0)) angles += $"/{pitchAngle}°"; 
    if(angles.IndexOf("/") <= 0) angles = "Critical";
    
    if(cameras.Count > 0 && dropAuto)
    {
      mode = "Auto"; status = "Search";
      
      if( angles != "Critical" )
      {
        if(target == Vector3D.Zero) 
        {
          foreach(IMyCameraBlock c in cameras) 
          {
            if(sightTurret != null && sightTurret.IsUnderControl) break;

            if(c.IsWorking) { 
              MyDetectedEntityInfo info = c.Raycast(maxRange);
              if(isTarget(info)) { dropBomb(fwc); break; }
            }
          }
        } else {
          Vector3D planet = Vector3D.Zero; shipControl.TryGetPlanetPosition(out planet);
          Vector3D drop = (target + Vector3D.Normalize(target - planet) * altitude);
          double d = Vector3D.Distance(sightHinge.GetPosition(),drop) - horizontalDistanceTraveled + sightOffset;
          
          if(autoPilot != null && !autoPilot.IsAutoPilotEnabled) flyAuto(autoPilot, drop);

          if(d <= 0 || d.ToString() == "NaN") { 
            dropBomb(fwc);
          } else {   
            status = $"{(int)d} m";
          }   
        }
      }
    }

    text = $"Drop status  : {mode}\n"+
           $"Ship angles  : {angles}\n"+
           $"Hinge angle  : {Math.Round(sightHinge.Angle*(180/Math.PI),2)}°\n"+
           $"Sight target  : {status}\n";
           
    if(mergers.Count > 0) {
      int i = 0;
      foreach(IMyShipMergeBlock m in mergers) {
          if(m.IsWorking && m.IsConnected) ++i;
      }
      text += $"Bombs left   : {i}";
    }  

    Echo(text); if(outScreen) outText(text);
}

private void dropBomb(int wc) 
{
  dropTimer.Trigger(); dropAuto = false; target = Vector3D.Zero; cameras[wc].CustomData = "";
}

private bool isTarget(MyDetectedEntityInfo i) 
{
  return (!i.IsEmpty() && Array.Exists(aimTarget, e => e == i.Relationship.ToString()) && 
         i.Type.ToString().IndexOf("Grid") > 0 && i.EntityId != Me.CubeGrid.EntityId);
}

private void outText(string text = "", int echo = 0) 
{
  if(echo > 0) Echo(text);
  
  foreach(var s in surface) {
    s.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    s.WriteText(outHead+text);
  }  
}  

private void flyAuto(IMyRemoteControl rc, Vector3D dp) 
{
  if(rc.IsWorking && rc.CanControlShip) 
  {
    rc.ClearWaypoints();
    rc.FlightMode = FlightMode.OneWay;
    rc.Direction = 0;
    
    rc.SetCollisionAvoidance(false);
    rc.AddWaypoint(dp, prefix);
    rc.SetAutoPilotEnabled(true);
  }
}  

private string vec2str(Vector3D v) 
{
  return v.GetDim(0) + ":" + v.GetDim(1) + ":" + v.GetDim(2);
}

private Vector3D str2vec(string coord) 
{ 
  string[] c = coord.Trim(':').Split(':'); int i = c.Count();
  double x, y, z; 
  
  if(c.Count() > 2) {  
    while(i-- > 0) {
      if(double.TryParse(c[i].Trim(), out z)) {
        if(double.TryParse(c[i-1].Trim(), out y)) {
          if(double.TryParse(c[i-2].Trim(), out x)) 
             return new Vector3D(x,y,z);
        }  
      }  
    }  
  } return Vector3D.Zero;
}
