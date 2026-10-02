/*
 * R e a d m e
 * -----------
 * 
 * Put down a programmable block and a remote control and enable auto pilot on the remote control
 */
G c=new G(2,0,.1,1);string d="=|=";List<IMyMotorSuspension>e=new List<IMyMotorSuspension>();IMyRemoteControl f;
IMySensorBlock g;Program(){var h=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(h);e=h.FindAll(i=>i.IsSameConstructAs(Me)&&
i is IMyMotorSuspension).Select(i=>i as IMyMotorSuspension).ToList();f=h.Find(i=>i.IsSameConstructAs(Me)&&i is
IMyRemoteControl)as IMyRemoteControl;g=h.Find(i=>i.IsSameConstructAs(Me)&&i is IMySensorBlock)as IMySensorBlock;Runtime.UpdateFrequency=
UpdateFrequency.Update1;}double j=0;void Main(string k,UpdateType l){if(!f.IsAutoPilotEnabled){foreach(IMyMotorSuspension Z in e){Z.
SetValue<Single>("Steer override",0);Z.SetValue<float>("Propulsion override",0);}}if((j+=Runtime.TimeSinceLastRun.TotalSeconds)>
.25){d+=d.FirstOrDefault();d=d.Substring(1);j=0;}Echo(
$"{d} Rover Autopilot {(f.IsAutoPilotEnabled?"active":"inactive")} {new string(d.Reverse().ToArray())}");if(!f.IsAutoPilotEnabled){foreach(IMyMotorSuspension Z in e){Z.SetValue<float>("Steer override",0);Z.SetValue<float>(
"Propulsion override",0);}return;}double m=Math.Sqrt(Math.Pow(f.CurrentWaypoint.Coords.X-f.GetPosition().X,2)+Math.Pow(f.CurrentWaypoint.
Coords.Z-f.GetPosition().Z,2));if(m<5){var n=new List<MyWaypointInfo>();f.GetWaypointInfo(n);n.Add(n.FirstOrDefault());n.
RemoveAt(0);if(n.Count==0){f.SetAutoPilotEnabled(false);return;}f.ClearWaypoints();foreach(MyWaypointInfo o in n)f.AddWaypoint(o
);f.SetAutoPilotEnabled(true);}Vector3D p=f.CurrentWaypoint.Coords;Vector3D q=(p-f.GetPosition());if(f.GetValueBool(
"CollisionAvoidance")){var r=f.WorldAABB.Inflate(2).Include(f.GetPosition()+f.WorldMatrix.Forward*15).Include(f.GetPosition()+f.WorldMatrix.
Right*5).Include(f.GetPosition()+f.WorldMatrix.Left*5);var s=new List<MyDetectedEntityInfo>();g.DetectedEntities(s);var u=s.
Find(i=>r.Intersects(i.BoundingBox));if(!u.IsEmpty()){var v=u.BoundingBox.TransformFast(f.WorldMatrix);var w=v.GetCorners().
ToList().OrderByDescending(i=>Vector3D.DistanceSquared(i,f.GetPosition())).FirstOrDefault();q+=w;}}Vector3D z=Vector3D.
Normalize(Vector3D.Transform(f.GetPosition()+Vector3D.Normalize(q),MatrixD.Invert(f.WorldMatrix)));double y=c.W(f.SpeedLimit-f.
GetShipSpeed(),Runtime.TimeSinceLastRun.TotalSeconds);y=MathHelper.Clamp(y,0,1);int b=1;if(f.WorldMatrix.Forward.Dot(Vector3D.
Normalize(p-f.GetPosition()))<-.85)b=-1;foreach(IMyMotorSuspension Z in e){Z.Brake=false;float B=Math.Sign(Math.Round(Vector3D.
Dot(Z.WorldMatrix.Forward,f.WorldMatrix.Up),2))*Math.Sign(Vector3D.Dot(Z.GetPosition()-D(),f.WorldMatrix.Forward))*b;float
C=-Math.Sign(Math.Round(Vector3D.Dot(Z.WorldMatrix.Up,f.WorldMatrix.Right),2))*b;Z.SetValue<float>("Steer override",B*(
float)(z.X/z.Length()));Z.SetValue<float>("Propulsion override",(float)(y*.1)*C);}}Vector3D D(){var E=Vector3D.Zero;foreach(
IMyMotorSuspension F in e)E+=F.GetPosition();return E/(e.Count);}
}class G{double H=0;double I=0;double J=0;double K=0;double L=0;double M=0;double N=0;bool O=true;public double P{get;
private set;}public G(double Q,double R,double S,double A){H=Q;I=R;J=S;K=A;L=1/K;}protected virtual double T(double U,double V,
double A){return V+U*A;}public double W(double X){var Y=(X-N)*L;if(O){Y=0;O=false;}M=T(X,M,K);N=X;this.P=H*X+I*M+J*Y;return
this.P;}public double W(double X,double A){if(A!=K){K=A;L=1/K;}return W(X);}public void a(){M=0;N=0;O=true;}