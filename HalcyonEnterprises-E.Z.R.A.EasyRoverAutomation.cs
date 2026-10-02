class EZRoverAutomation
{
	const float LightIntensityLo = 1f;
	const float LightIntensityHi = 5f;
	const int LightBlinkInterval = 20;
	const float SpeedLimitLo = 80f;
	const float SpeedLimitHi = 180f;
	const float WheelPowerLo = 50f;
	const float WheelPowerHi = 100f;
	const float WheelPowerDelta = 1f;
	const float GyroRollCoeff = 1f;
	Color LightColorTurn = new Color(255,204,0);
	Color LightColorStop = new Color(255,0,0);
	Color LightColorBack = new Color(255,255,255);
	
	int blink;

	Program Parent;
	List<IMyShipController> Controllers;
	List<IMyLightingBlock> Lights;
	List<IMyMotorSuspension> Wheels;
	List<IMyGyro> Gyros;
	
	public EZRoverAutomation(Program parentProgram)
	{
		Parent = parentProgram;
	}
	
	public void Update()
	{
		Controllers = new List<IMyShipController>();
		Parent.GridTerminalSystem.GetBlocksOfType(Controllers, block => block.CubeGrid == Parent.Me.CubeGrid);
		if (Controllers.Count > 0)
		{
			Vector3 axes = Vector3.Zero;
			float roll = 0f;
			foreach (var Controller in Controllers)
			{
				if (Controller.IsUnderControl)
				{
					axes = Controller.MoveIndicator;
					roll = Controller.RollIndicator;
					blink = axes.X != 0f && blink < LightBlinkInterval ? blink + 1 : 0;
					Wheels = new List<IMyMotorSuspension>();
					Parent.GridTerminalSystem.GetBlocksOfType(Wheels, 
						block => block.CubeGrid == Parent.Me.CubeGrid && HasTags(block, "ezra"));
					foreach (var Wheel in Wheels)
					{
						Wheel.SetValueFloat("Speed Limit", (axes.Y < 0 && HasTags(Wheel, "fast")) ? SpeedLimitHi : SpeedLimitLo);
						if (axes.Z != 0 && Wheel.Power < ((axes.Y < 0 && HasTags(Wheel, "fast")) ? WheelPowerHi : WheelPowerLo))
							Wheel.Power += WheelPowerDelta;
						else
							Wheel.Power -= WheelPowerDelta;
					}
					Gyros = new List<IMyGyro>();
					Parent.GridTerminalSystem.GetBlocksOfType(Gyros, 
						block => block.CubeGrid == Parent.Me.CubeGrid && HasTags(block, "ezra"));
					foreach (var Gyro in Gyros)
					{
						Gyro.GyroOverride = roll != 0f ? true : false;
						if (Gyro.Roll != roll * GyroRollCoeff)
							Gyro.Roll = roll * GyroRollCoeff;
					}
					Lights = new List<IMyLightingBlock>();
					Parent.GridTerminalSystem.GetBlocksOfType(Lights, 
						block => block.CubeGrid == Parent.Me.CubeGrid && HasTags(block, "ezra"));
					foreach (var Light in Lights)
					{
						if (HasTags(Light, "left"))
						{
							if (axes.X < 0f)
							{
								if (blink == 1)
									Light.Intensity = Light.Intensity == LightIntensityLo ? LightIntensityHi : LightIntensityLo;
							}
							else
								Light.Intensity = LightIntensityLo;
							if (HasTags(Light, "color"))
								Light.Color = LightColorTurn;
						}
						if (HasTags(Light, "right"))
						{
							if (axes.X > 0f)
							{
								if (blink == 1)
									Light.Intensity = Light.Intensity == LightIntensityLo ? LightIntensityHi : LightIntensityLo;
							}
							else
								Light.Intensity = LightIntensityLo;
							if (HasTags(Light, "color"))
								Light.Color = LightColorTurn;
						}
						if (HasTags(Light, "stop back"))
						{
							Light.Intensity = axes.Y > 0f || axes.Z > 0 ? LightIntensityHi : LightIntensityLo;
							if (HasTags(Light, "color"))
								Light.Color = axes.Z > 0f ? LightColorBack : LightColorStop;
						}
						else if (HasTags(Light, "stop"))
						{
							Light.Intensity = axes.Y > 0f ? LightIntensityHi : LightIntensityLo;
							if (HasTags(Light, "color"))
								Light.Color = LightColorStop;
						}
						else if (HasTags(Light, "back"))
						{
							Light.Enabled = axes.Z > 0f ? true : false;
							if (HasTags(Light, "color"))
								Light.Color = LightColorBack;
						}
					}
					break;
				}
			}
		}
	}

	public bool HasTags(IMyTerminalBlock block, string tags)
	{
		
		foreach (var tag in tags.Split(' '))
		{
			if (block.CustomData.IndexOf(tag,StringComparison.OrdinalIgnoreCase) < 0)
				return false;
		}
		return true;
	}
}

EZRoverAutomation EZRA;
public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update1;
	EZRA = new EZRoverAutomation(this);
}

public void Main(string args, UpdateType updateSource)
{
	EZRA.Update();
}