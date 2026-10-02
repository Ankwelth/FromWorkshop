// Space Engineers Piston step extend / retract script
// =========================================
// Author: the_maegges
// Version: 1.0.2
// Description: 
// This script automatically moves pistons in a given piston group stepwise.
// Based on the operation handed over with the run argument (Extend or Retract) the script finds 
// the next piston in the group, which hasn't fulle retracted/expanded and applies the defined velocity
// then it will monitor the piston's position until it has moved the defined distance where it will stop.
// if the piston has been fully extended / retracted, the script will use the next available piston until all pistons
// have been fully retracted / extended.


// Set this variable to match your piston group name
string pistonGroupName = "DownwardPistons";

// Speed in m/s (as you would set it in Terminal)
float stepVelocity = 0.25f;

// Distance to travel per step
float stepDistance = 2.0f;

// Start of Code, do not change variables below
// ==========================================
float currentPistonStart = 0.0f;
float currentPistonTarget = 0.0f;
List<IMyPistonBase> Pistons = null;
List<PistonSequenceStep> Sequence = null;
int sequenceIndex = 0;
PistonSequenceStep currentStep = null;
DateTime delayStart = DateTime.MinValue;
IMyPistonBase CurrentPiston = null;
bool isInErrorState = false;
bool delayActive = false;
bool doingWork = false;
string errorMessage = string.Empty;
Operations operation = Operations.None;

public Program()
{
	Pistons = new List<IMyPistonBase>();
	Echo("Initializing Pistons");
	IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(pistonGroupName);
	if (blockGroup != null)
	{
		blockGroup.GetBlocksOfType<IMyPistonBase>(Pistons, block => block is IMyPistonBase);
		if (Pistons != null)
		{
			Echo(string.Format("Found {0} Pistons", Pistons.Count()));
		}
	}
	GetPistonSequence();
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
	if (!String.IsNullOrEmpty(argument))
	{
		if (argument.ToLower() == "extend")
		{
			operation = Operations.Extend;
		}
		else if (argument.ToLower() == "retract")
		{
			operation = Operations.Retract;
		}
		else if (argument.ToLower() == "seq_once")
		{
			operation = Operations.SequenceOnce;
		}
		else if (argument.ToLower() == "seq_repeat")
		{
			operation = Operations.SequenceRepeat;
		}
		else if (argument.ToLower() == "stop")
		{
			operation = Operations.Stopped;
			if (CurrentPiston != null)
			{
				CurrentPiston.Velocity = 0.0f;
				CurrentPiston = null;
				sequenceIndex = 0;
			}
		}
		
		if ((operation == Operations.SequenceOnce || operation == Operations.SequenceRepeat) && (Sequence == null || Sequence.Count() == 0))
		{
			isInErrorState = true;
			errorMessage = "Cannot run in sequence mode without valid sequence. \nPlease check your custom data and recompile the script.";
		}
	}
	Echo("Operation: " + operation.ToString());

	if (delayActive == true)
	{
		Echo("Delay active");
		TimeSpan delaytime = DateTime.Now - delayStart;
		if ((delaytime.TotalMilliseconds / 1000.0f) <= currentStep.Delay)
		{
			Echo("Delayed since: " + delayStart);
			Echo("Set delay: " + currentStep.Delay);
			Echo("Elapsed: " + delaytime);
			return;
		}
		else
		{
			delayActive = false;
			delayStart = DateTime.MinValue;
		}
	}
	if (isInErrorState)
	{
		Echo("== ERROR ==");
		if (!string.IsNullOrEmpty(errorMessage))
			Echo(errorMessage);
		return;
	}
	
	if (operation == Operations.Stopped)
		return;
	if (Pistons != null && Pistons.Count() > 0)
	{	
		if (!doingWork || CurrentPiston == null)
			CurrentPiston = GetNextPiston(operation, stepDistance) ?? CurrentPiston ?? null;
		if ((operation == Operations.Extend || operation == Operations.Retract) && CurrentPiston == null)
		{
			Echo(string.Format("{0}", operation == Operations.Extend ? "All Pistons have been extended" : 
				operation == Operations.Retract ? "All Pistons have been retracted" : "No operation"));
			return;
		}
		if (operation == Operations.Extend)
		{			
			if (!doingWork && (CurrentPiston.Status == PistonStatus.Stopped || CurrentPiston.Status == PistonStatus.Retracted))
			{
				CurrentPiston.Velocity = stepVelocity;
				currentPistonStart = CurrentPiston.CurrentPosition;
				currentPistonTarget = TruncateFloat(currentPistonStart + stepDistance);
				doingWork = true;
			}
			if (CurrentPiston.Status == PistonStatus.Extended || TruncateFloat(CurrentPiston.CurrentPosition) >= TruncateFloat(currentPistonTarget))
			{
				CurrentPiston.Velocity = 0.0f;
				operation = Operations.None;
				CurrentPiston = null;
				doingWork = false;
			}
		}
		else if (operation == Operations.Retract)
		{		
			if (!doingWork && (CurrentPiston.Status == PistonStatus.Stopped || CurrentPiston.Status == PistonStatus.Extended))
			{
				CurrentPiston.Velocity = stepVelocity * (-1);
				currentPistonStart = TruncateFloat(CurrentPiston.CurrentPosition);
				currentPistonTarget = TruncateFloat(currentPistonStart - stepDistance);
				doingWork = true;
			}

			if (CurrentPiston.Status == PistonStatus.Retracted || TruncateFloat(CurrentPiston.CurrentPosition) <= TruncateFloat(currentPistonTarget))
			{
				CurrentPiston.Velocity = 0.0f;
				operation = Operations.None;
				CurrentPiston = null;
				doingWork = false;
			}
		}
		else if (operation == Operations.SequenceOnce || operation == Operations.SequenceRepeat)
		{
			if (Sequence == null || Sequence.Count() < 1)
			{
				isInErrorState = true;
				errorMessage += "Sequence is empty";
				return;
			}
			
			currentStep = Sequence[sequenceIndex];
			Echo("Seq Step Index: " + sequenceIndex);
			Echo("Seq Step Velocity: " + currentStep.StepVelocity);
			Echo("Seq Step Distance: " + currentStep.StepDistance);
			Echo("Seq Step Delay: " + currentStep.Delay + "\n\n");
			if (currentStep.StepVelocity > 0.0f)
			{
				if (!doingWork || CurrentPiston == null)
					CurrentPiston = GetNextPiston(Operations.Extend, currentStep.StepDistance) ?? CurrentPiston ?? null;
				if (!doingWork && (CurrentPiston.Status == PistonStatus.Stopped || CurrentPiston.Status == PistonStatus.Retracted))
				{
					CurrentPiston.Velocity = currentStep.StepVelocity;
					currentPistonStart = TruncateFloat(CurrentPiston.CurrentPosition);
					currentPistonTarget = TruncateFloat(currentPistonStart) + currentStep.StepDistance;
					doingWork = true;
				}

				if (CurrentPiston.Status == PistonStatus.Extended || TruncateFloat(CurrentPiston.CurrentPosition) >= TruncateFloat(currentPistonTarget))
				{
					CurrentPiston.Velocity = 0.0f;
					CurrentPiston = null;
					doingWork = false;
					if (currentStep.Delay > 0.0f)
					{
						delayStart = DateTime.Now;
						delayActive = true;
					}
					if (sequenceIndex + 1 < Sequence.Count())
					{
						sequenceIndex++;
					}
					else
					{
						if (operation == Operations.SequenceRepeat)
						{
							sequenceIndex = 0;
						}
						else 
						{
							operation = Operations.None;
						}
					}
				}
			}
			else if (currentStep.StepVelocity < 0.0f)
			{
				if (!doingWork || CurrentPiston == null)
					CurrentPiston = GetNextPiston(Operations.Retract, currentStep.StepDistance) ?? CurrentPiston ?? null;
				if (!doingWork && (CurrentPiston.Status == PistonStatus.Stopped || CurrentPiston.Status == PistonStatus.Extended))
				{
					CurrentPiston.Velocity = currentStep.StepVelocity;
					currentPistonStart = TruncateFloat(CurrentPiston.CurrentPosition);
					currentPistonTarget = TruncateFloat(currentPistonStart - currentStep.StepDistance);
					doingWork = true;
				}

				if (CurrentPiston.Status == PistonStatus.Retracted || CurrentPiston.CurrentPosition <= TruncateFloat(currentPistonTarget))
				{
					CurrentPiston.Velocity = 0.0f;
					CurrentPiston = null;
					doingWork = false;
					if (currentStep.Delay > 0.0f)
					{
						delayStart = DateTime.Now;
						delayActive = true;
					}
					if (sequenceIndex + 1 < Sequence.Count())
					{
						sequenceIndex++;
					}
					else
					{
						if (operation == Operations.SequenceRepeat)
						{
							sequenceIndex = 0;
						}
						else 
						{
							operation = Operations.None;
						}
					}
				}
			}
			
		}
		// Output on 
		if (CurrentPiston != null)
		{
			Echo("Piston Name: " + CurrentPiston.CustomName);
			Echo("Start: " + currentPistonStart.ToString());
			Echo("Piston Status: " + CurrentPiston.Status.ToString());	
			Echo("Piston MinPos: " + CurrentPiston.MinLimit);
			Echo("Piston CurrentPos: " + CurrentPiston.CurrentPosition);
			Echo("Piston MaxPos: " + CurrentPiston.MaxLimit);
			Echo("Piston Velocity: " + CurrentPiston.Velocity);
			Echo("Target: " + currentPistonTarget.ToString());	
		}
	}
	else
	{
		isInErrorState = true;
		errorMessage += "Piston group '"+ pistonGroupName +"' was not found or does not contain any pistons.";
	}
}

private float TruncateFloat(float current)
{
	string truncated = current.ToString("0.00");
	return float.Parse(truncated);
}

private IMyPistonBase GetNextPiston(Operations operation, float distance)
{
	if (Pistons == null || Pistons.Count() == 0)
		return null;
	IMyPistonBase candidate = null;
	// slightly reduce distance
	distance -= 0.01f;
	switch (operation)
	{
		case Operations.Extend:
			{
				candidate = Pistons.FirstOrDefault(itm => itm.CurrentPosition < itm.MaxLimit
					&& (TruncateFloat(itm.MaxLimit - itm.CurrentPosition) >= distance ));
			}
			break;
		case Operations.Retract:
			{
				candidate = Pistons.FirstOrDefault(itm => itm.CurrentPosition > itm.MinLimit
						&& (TruncateFloat(itm.CurrentPosition - itm.MinLimit) >= distance ));
			}
			break;
		default:
			break;
	}
	return candidate;
}

private void GetPistonSequence()
{
	string customdata = Me.CustomData;
	if (string.IsNullOrEmpty(customdata))
	{
		errorMessage = "Custom Data is empty\n";
		return;
	}
	Sequence = new List<PistonSequenceStep>();
	foreach (string line in customdata.Split('\n'))
	{
		PistonSequenceStep step = PistonSequenceStep.FromCDLine(line, errorMessage);
		if (step != null)
			Sequence.Add(step);
	}
	if (Sequence.Count() == 0)
	{
		isInErrorState = true;
		errorMessage += "Sequence does not contain any valid elements";
	}
}

private class PistonSequenceStep
{
	public PistonSequenceStep(float stepVelocity, float stepDistance, float delay)
	{
		StepVelocity = stepVelocity;
		StepDistance = stepDistance;
		Delay = delay;
	}
	public float StepVelocity {get;set;}
	public float StepDistance {get;set;}
	public float Delay {get;set;}
	
	public static PistonSequenceStep FromCDLine(string line, string errorMessage)
	{
		string[] seqItems = line.Trim().Split('|');
		float stepVelocity = 0.0f;
		float stepDistance = 0.0f;
		float delay = 0.0f;
		if (seqItems.Count() < 2)
		{
			errorMessage += "Wrong format in Custom Data line: " + line;
			return null;
		}
		if (seqItems.Count() == 2)
		{
			if (float.TryParse(seqItems[0],out stepVelocity) && float.TryParse(seqItems[1],out stepDistance))
			{
				if (stepVelocity == 0.0f || stepDistance == 0.0f)
				{
					errorMessage += "Velocity or distance of a step cannot be 0.";
					return null;
				}
				return new PistonSequenceStep(stepVelocity, stepDistance, 0.0f);
			}
			else
			{
				errorMessage += "Could not parse Sequence Entry: " + line;
			}
		}
		else if (seqItems.Count() >= 3)
		{
			if (float.TryParse(seqItems[0],out stepVelocity) && float.TryParse(seqItems[1],out stepDistance) && float.TryParse(seqItems[2],out delay))
			{
				if (stepVelocity == 0.0f || stepDistance == 0.0f)
				{
					errorMessage += "Velocity or distance of a step cannot be 0.";
					return null;
				}
				return new PistonSequenceStep(stepVelocity, stepDistance, delay);
			}	
			else
			{
				errorMessage += "Could not parse Sequence Entry: " + line;
			}
		}
		return null;
	}
}

public enum Operations
{
	None = 0,
	Extend = 1,
	Retract = 2,
	SequenceOnce = 3,
	SequenceRepeat = 4,
	Stopped = 5
}