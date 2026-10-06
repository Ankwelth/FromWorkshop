#region VSCode
#if DEBUG
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace AutoGyro11
{
    public sealed class Program : MyGridProgram
    {
#endif
        #endregion

        IMyTextSurface myLCD = null;
        public void LCD(string text = null)
        {
            if (myLCD == null)
            {
                myLCD = Me.GetSurface(0);
                myLCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                myLCD.FontSize = 4;
                myLCD.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
            }
            if (text == null)
                myLCD.WriteText("", false);
            else
                myLCD.WriteText(text + "\n", true);
        }

        private double round0(double d)
        {
            return Math.Abs(d) < 0.01 ? 0 : d;
        }

        private Vector3D round0(Vector3D v)
        {
            return v.Length() < 0.01 ? (0 * v) : v;
        }

        /*
         *  AutoGyro - keeps a ship oriented so you can manoeuvre on a single engine.
         *  
         *  In space, the script will rotate your ship to oppose your current velocity. In gravity the script will
         *  align with gravity and velocity to allow you to hoved in place. Movement keys will allow level flight -
         *  speed is limited at lower altitudes.
         *
         *  1. ensure reference controller/gyro are called "Cockpit" and "Gyroscope" or change names in AutoGyro()
         *  2. add: `Runtime.UpdateFrequency |= UpdateFrequency.Update10;` to Program()
         *  3. add: `AutoGyro(args, updateSource);` to Main()
         *  4. add: `Gyroscope - override controls on/off` to toolbar
         *  
         */

        /*
            TODO:
            - add target velocity to launchControl
            - suicideBurn just turns on dampeners before we hit the ground...
              (hopefully in time to prevent a crash)
        */

        public void AutoGyro(string args, UpdateType updateSource)
        {
            // only run every 10 tics
            if ((updateSource & UpdateType.Update10) == 0)
                return;

            Echo("AutoGyro");
            Echo(" - get blocks by name");
            IMyShipController ctrl = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;
            IMyGyro           gyro = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;

            Echo(" - calculate target vector");
            Vector3D worldRV;

            if (ctrl.RotationIndicator.Length() != 0 || ctrl.RollIndicator != 0)
            {
                // pilot input (rotation/roll) has priority
                Vector3D ctrlRV = new Vector3D(ctrl.RotationIndicator, ctrl.RollIndicator);
                worldRV = Vector3D.TransformNormal(ctrlRV, ctrl.WorldMatrix);
                LCD($"AG:rot");
            }
            else if (ctrl.MoveIndicator.Y != 0) // up or down thrust pauses autogyro
            {
                // pilot input (up/down movement) also has priority
                worldRV = new Vector3D(0, 0, 0);
                LCD($"AG:u/d");
            }
            else
            {
                // calculate our target vector: new down direction to counter gravity/velocity relative to ctrl
                Vector3D gravity = ctrl.GetNaturalGravity();
                Vector3D gravNorm = Vector3D.Normalize(gravity);
                Vector3D velocity = ctrl.GetShipVelocities().LinearVelocity;
                Vector3D sideways = velocity - velocity.Dot(gravNorm) * gravNorm;  // velocity orthogonal to gravity

                if (ctrl.MoveIndicator.Length() > 0)
                {
                    // sideways can increaseup to max, if pressing in that direction.
                    Vector3D input = Vector3D.TransformNormal(ctrl.MoveIndicator, ctrl.WorldMatrix);
                    input = round0(input - input.Dot(gravNorm) * gravNorm);

                    double elevation;
                    ctrl.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation);
                    // set maximum velocity based on altitude: 3m/s at ground level up to 100m/s as altitude increases.
                    double maxV = 3 + 97 * elevation / Math.Sqrt(elevation * elevation + 1000000);
                    // this could just be 3+10*elevation?
                    sideways -= input * maxV;
                }

                Vector3D target = (gravity.Length() < 0.01) ? velocity : gravity + sideways;
                QuaternionD QRV = QuaternionD.CreateFromTwoVectors(target, ctrl.WorldMatrix.Down);

                if (gravity.Length() > 0)
                {
                    // TODO: remove yaw component (rotation perpendicular to gravity vector)?

                    // this should work (but doesn't)
                    // 1. calculate plane perpendicular to gravity
                    // 2. project target onto plane = tp <-- target - target.dot(gravity) * gravity.
                    //Vector3D tp = target - target.Dot(gravNorm) * gravNorm;
                    // 3. get angle between down and normal(tp) = θ
                    //double a = Math.Acos(Vector3D.Normalize(worldDown).Dot(Vector3D.Normalize(tp)));
                    //QuaternionD Qz = new QuaternionD(Math.Cos(a / 2), Math.Sin(a / 2) * gravNorm.X, Math.Sin(a / 2) * gravNorm.Y, Math.Sin(a / 2) * gravNorm.Z);
                    // 4. qz = [cos(θ/2), sin(θ/2)rx,sin(θ/2)ry,sin(θ/2)rz]
                    //QuaternionD Qz = QuaternionD.CreateFromTwoVectors(target, gravNorm);
                    //QRV = Qz * QRV;
                }

                Vector3D axis;
                double   angle;
                QRV.GetAxisAngle(out axis, out angle);
                worldRV = axis * Math.Log(1 + round0(angle), 2); // apply log to angle for dampening effect
                LCD($"AG:{angle:0}°");
            }

            Echo(" - apply to gyro");  // convert to gyro reference frame & apply
            Vector3D gyroRV = Vector3D.TransformNormal(worldRV, MatrixD.Transpose(gyro.WorldMatrix));
            gyro.Pitch = (float)gyroRV.X;
            gyro.Yaw   = (float)gyroRV.Y;
            gyro.Roll  = (float)gyroRV.Z;

            // Programming Block Panel Display
            //Echo(" - LCD Display");
            //IMyTextSurface mesurface0 = Me.GetSurface(0);
            //mesurface0.ContentType    = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
            //mesurface0.FontSize       = 3;
            //mesurface0.Alignment      = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
            //mesurface0.WriteText(String.Join("\n", $"maxV:{strafe_speed:0.##} ang:{angle:0.##} |V|:{velocity.Length():0.##} |s|:{sideways.Length():0.##}".Split(' ')));
        }

        /*
         * LaunchControl - maintain target ascent speed to conserve fuel and enable dampeners in orbit.
         * 
         * 1. ensure reference controller is called "Cockpit" or change name in LaunchControl()
         * 2. add: `Runtime.UpdateFrequency |= UpdateFrequency.Update10;` to Program()
         * 3. add: `LaunchControl(args, updateSource);` to Main()
         * 4. launch with args "launch <targetSpeed>", cancel with "launch 0", "abort", or enable dampeners
         * 
         */

        double targetSpeed = 0;  // This variable tracks target launch speed (0 == inactive)

        public void LaunchControl(string args, UpdateType updateSource)
        {
            Echo("LaunchControl");
            Echo($" - targetSpeed {targetSpeed}");
            if (args.StartsWith("launch "))
                targetSpeed = double.Parse(args.Substring(7));
            else if (args == "abort")
                targetSpeed = 0;
            else if (targetSpeed == 0 || (updateSource & UpdateType.Update10) == 0)
                return;

            Echo(" - get blocks by name");
            IMyShipController ctrl = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;

            Echo(" - calculations");
            double mass    = ctrl.CalculateShipMass().PhysicalMass;
            double speed   = ctrl.GetShipVelocities().LinearVelocity.Dot(ctrl.WorldMatrix.Up);  // speed parallel to thrust
            double gravity = ctrl.GetNaturalGravity().Dot(ctrl.WorldMatrix.Down);  // gravity parallel to thrust (m/ss)
            double thrust  = 0;  // max effective thrust (N).

            // get thrusters
            Base6Directions.Direction dir = Base6Directions.GetFlippedDirection(ctrl.Orientation.Up);
            List<IMyThrust> thrusters = new List<IMyThrust> { };
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters,
                x => x.Orientation.Forward == dir && x.CubeGrid == ctrl.CubeGrid);
            Echo($" - got {thrusters.Count} Thrusters");
            foreach (IMyThrust thruster in thrusters)
                thrust += thruster.MaxEffectiveThrust;

            if (args.StartsWith("launch ") && targetSpeed != 0) // init!
            {
                ctrl.DampenersOverride = false;
            }
            
            if (ctrl.DampenersOverride == true  // pilot enabled dampeners
                || targetSpeed == 0             // script called with "launch 0" or "abort"
                || speed == 0                   // we've landed??
                || gravity == 0)                // we made it!

            {
                // cancel
                ctrl.DampenersOverride = true;
                targetSpeed = 0;
                foreach (IMyThrust thruster in thrusters)
                    thruster.ThrustOverridePercentage = 0;
                return;
            }

            Echo(" - calculate and apply thrust");
            double delta = targetSpeed - speed;
            double accelleration = gravity + delta;  // assuming we want to accellerate to targetSpeed in 1 second.
            double force = mass * accelleration;
            float thrustOverride = (float) Math.Min(force / thrust, 1.0);

            foreach (IMyThrust thruster in thrusters)
                thruster.ThrustOverridePercentage = thrustOverride;

            LCD($"LC:{targetSpeed:0}m/s");
        }

        /*
         * AntiSplat - engages dampeners to prevent lithobraking / rapid unscheduled disassembly.
         * 
         * 1. ensure reference controller is called "Cockpit" or change name in AntiSplat()
         * 2. add: `Runtime.UpdateFrequency |= UpdateFrequency.Update10;` to Program()
         * 3. add: `AntiSplat(args, updateSource);` to Main()
         * 
         */

        public void AntiSplat(string args, UpdateType updateSource)
        {
            if ((updateSource & UpdateType.Update10) == 0)
                return;

            Echo("AntiSplat");
            Echo(" - get blocks by name");
            IMyShipController ctrl = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyShipController;

            Vector3D velocity  = ctrl.GetShipVelocities().LinearVelocity;
            Vector3D direction = Vector3D.Normalize(velocity);
            double speed       = velocity.Length();
            double gravity     = ctrl.GetNaturalGravity().Dot(direction); // in direction of velocity.

            if (ctrl.DampenersOverride || velocity.Length() == 0 || gravity  <= 0 || targetSpeed < 0)
                return; // nothing to do

            double mass = ctrl.CalculateShipMass().PhysicalMass;
            double maxThrust = 0;  // max effective thrust (N) in direction of velocity.

            // get thrusters
            List<IMyThrust> thrusters = new List<IMyThrust> { };
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, x => x.CubeGrid == ctrl.CubeGrid);
            Echo($" - got {thrusters.Count} Thrusters");
            foreach (IMyThrust thruster in thrusters)
            {
                double retroThrust = new Vector3D(thruster.GridThrustDirection).Dot(direction) * (-1);
                if (retroThrust > 0)  // retroThrust should be in -1..1
                    maxThrust += thruster.MaxEffectiveThrust * retroThrust;
            }

            double decelleration = maxThrust / mass - gravity;  // max decelleration.
            double time_to_stop  = speed / decelleration;
            double dist_to_stop  = speed / 2 * time_to_stop;
            double elevation;
            ctrl.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation);
            if (elevation - dist_to_stop < 10)
                ctrl.DampenersOverride = true;

            LCD($"AS:{dist_to_stop}m");
        }

        public Program()
        {
            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
        }


        public void Main(string args, UpdateType updateSource)
        {
            LCD();
            AutoGyro(args, updateSource);
            LaunchControl(args, updateSource);
            AntiSplat(args, updateSource);
        }




        #region VSCodeFooter
#if DEBUG
    }
}
#endif
#endregion
          