int totalSteps = 20;
int stepsTaken = 0;
bool completed = false;

float[,] maxPandAngle;

List<IMyMotorStator> rotors = new List<IMyMotorStator>();

public void Main(string state)
{
    if(state == "restart")
    {
        stepsTaken = 0;
        completed = false;
    }

    if(!completed)
    {
        rotors.Clear();
        GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors);
    
        for(int i = rotors.Count - 1; i > -1; i--)
        {
            if(rotors[i].CustomName.Contains("SolarRotor"))
            {
                if(stepsTaken == 0)
                {
                    rotors[i].SetValue<float>("LowerLimit", float.NegativeInfinity);
                    rotors[i].SetValue<float>("UpperLimit", float.PositiveInfinity);
                    rotors[i].SetValueFloat("Velocity", 1f);
                }
            }
            else
            {
                rotors.Remove(rotors[i]);
            }
        }

        if(stepsTaken ==  0)
        {
            maxPandAngle = new float[rotors.Count, 2];
        }
        else
        {
            for(int i = 0; i < rotors.Count; i++)
            {
                if(stepsTaken <= totalSteps)
                {
                    IMySolarPanel currentPanel = GridTerminalSystem.GetBlockWithName(rotors[i].CustomData) as IMySolarPanel;
                    string[] detail = (currentPanel.DetailedInfo.Split('\n',' '));
                    float power = float.Parse(detail[5]);

                    if(power > maxPandAngle[i, 0])
                    {
                        maxPandAngle[i, 0] = power;
                        maxPandAngle[i, 1] = rotors[i].Angle * 57.2957f;
                    }
                }
                else
                {
                    rotors[i].SetValue<float>("LowerLimit", maxPandAngle[i, 1]);
                    rotors[i].SetValue<float>("UpperLimit", maxPandAngle[i, 1]);
                    rotors[i].SetValueFloat("Velocity", -5f);

                    completed = true;
                }
            }
        }
    }

    stepsTaken++;
}