
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}


public void Main(string argument, UpdateType updateSource)
{
    // ---- CHANGE ROTOR NAMES HERE

    string AzTargetRotorName = "Azimuth";   // The azimuth rotor that you control
    string AzSyncGroupName = "AzSync";      // The azimuth rotor(s) group name that match with the target rotor

    string ElTargetRotorName = "Elevation";   // The elevation rotor that you control
    string ElSyncGroupName = "ElSync";      // The elevation rotor(s) group name that match with the target rotor

    // Set up your AzSync and ElSync groups and the custom data format will generate in each rotor.
    // --------- Custom Data Settings ----------

    // Angle Offset: The angle offset in degrees. Useful for making sure all turrets point in the right direction
    // Direction: If you need to invert the direction (e.g. opposite elevation) set this to -1.
    // Proportional Gain: How sensitive the syncing is. If you need more responsiveness increase it. If you have
    //                               oscillation issues decrease it.
    // RPM Cap: Puts a limit on how fast the rotor can turn while syncing.


    // ---- DONT TOUCH BELOW HERE OR YOU KILL IT


    IMyMotorStator AzTargetRotor;
    AzTargetRotor = GridTerminalSystem.GetBlockWithName(AzTargetRotorName) as IMyMotorStator;
    if (AzTargetRotor == null)
    {
        Echo("AzTarget Rotor Missing [REQUIRED]");
    }

    IMyBlockGroup AzSyncRotors = GridTerminalSystem.GetBlockGroupWithName(AzSyncGroupName);
    if (AzSyncRotors == null)
    {
        Echo("AzSync Group Missing [REQUIRED]");
    }
    else
    {
        List<IMyMotorStator> AzSync = new List<IMyMotorStator>();
        AzSyncRotors.GetBlocksOfType<IMyMotorStator>(AzSync);

        int AzCount = AzSync.Count;

        if ((AzSyncRotors != null) && (AzTargetRotor != null))
        {

            for (int i = 0; i < AzCount; i++)
            {
                if ((AzSync[i].CustomData).Length < 5 || AzSync[i].CustomData == null)
                {
                    AzSync[i].CustomData = "Angle Offset: 0;" + System.Environment.NewLine + "Direction: 1;" + System.Environment.NewLine + "Proportional Gain: 200;" + System.Environment.NewLine + "RPM Speed Cap: 60;"; ;
                }
                float AngleOffset = 0;
                float Direction = 1;
                float ProportionalGain = 200;
                float RPMCap = 60;
                // Grab settings from custom data
                try
                {
                    string CustomData = AzSync[i].CustomData;

                    char[] delims = new[] { '\r', '\n' };
                    string[] SettingsArray = CustomData.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                    int Pos1 = SettingsArray[0].IndexOf(": ") + 2;
                    int Pos2 = SettingsArray[0].IndexOf(";");
                    AngleOffset = float.Parse(SettingsArray[0].Substring(Pos1, Pos2 - Pos1));

                    Pos1 = SettingsArray[1].IndexOf(": ") + 2;
                    Pos2 = SettingsArray[1].IndexOf(";");
                    Direction = float.Parse(SettingsArray[1].Substring(Pos1, Pos2 - Pos1));

                    Pos1 = SettingsArray[2].IndexOf(": ") + 2;
                    Pos2 = SettingsArray[2].IndexOf(";");
                    ProportionalGain = float.Parse(SettingsArray[2].Substring(Pos1, Pos2 - Pos1));

                    Pos1 = SettingsArray[3].IndexOf(": ") + 2;
                    Pos2 = SettingsArray[3].IndexOf(";");
                    RPMCap = float.Parse(SettingsArray[3].Substring(Pos1, Pos2 - Pos1));
                }
                catch (Exception)
                {
                    Echo("Custom data error with rotor " + AzSync[i].CustomName + "Using default values");
                }



                // Angle Offset
                float AzTargetAngle = AzTargetRotor.Angle;
                AzTargetAngle = AzTargetAngle + (AngleOffset * 0.0174533f);

                // Direction
                if (Direction == -1)
                {
                    AzTargetAngle = (MathHelper.TwoPi - AzTargetAngle);
                }

                // Sync calc
                float Azdifference = (AzTargetAngle - AzSync[i].Angle);

                Azdifference %= MathHelper.TwoPi; // Make sure that we are less than -360 deg and 360 deg by wrapping it around and getting the remainder

                if (Azdifference > MathHelper.Pi)
                {
                    Azdifference = -MathHelper.TwoPi + Azdifference;
                }
                else if (Azdifference < -MathHelper.Pi)
                {
                    Azdifference = MathHelper.TwoPi + Azdifference;
                }
                float AzStabVelocity = ProportionalGain * Azdifference;

                // RPM Cap
                if (AzStabVelocity > RPMCap)
                {
                    AzStabVelocity = RPMCap;
                }
                if (AzStabVelocity < (RPMCap * -1))
                {
                    AzStabVelocity = (-1 * RPMCap);
                }
                AzSync[i].TargetVelocityRPM = AzStabVelocity;
            }
        }
    }
        //------------------------------



        IMyMotorStator ElTargetRotor;
        ElTargetRotor = GridTerminalSystem.GetBlockWithName(ElTargetRotorName) as IMyMotorStator;
        if (ElTargetRotor == null)
        {
            Echo("ElTarget Rotor Missing [REQUIRED]");
        }

        IMyBlockGroup ElSyncRotors = GridTerminalSystem.GetBlockGroupWithName(ElSyncGroupName);
        if (ElSyncRotors == null)
        {
            Echo("ElSync Group Missing [REQUIRED]");
        }
        else
        {
            List<IMyMotorStator> ElSync = new List<IMyMotorStator>();
            ElSyncRotors.GetBlocksOfType<IMyMotorStator>(ElSync);

            int ElCount = ElSync.Count;

            if ((ElSyncRotors != null) && (ElTargetRotor != null))
            {

                for (int i = 0; i < ElCount; i++)
                {
                    if ((ElSync[i].CustomData).Length < 5 || ElSync[i].CustomData == null)
                    {
                        ElSync[i].CustomData = "Angle Offset: 0;" + System.Environment.NewLine + "Direction: 1;" + System.Environment.NewLine + "Proportional Gain: 200;" + System.Environment.NewLine + "RPM Speed Cap: 60;"; ;
                    }
                    float AngleOffset = 0;
                    float Direction = 1;
                    float ProportionalGain = 200;
                    float RPMCap = 60;
                    // Grab settings from custom data
                    try
                    {
                        string CustomData = ElSync[i].CustomData;

                        char[] delims = new[] { '\r', '\n' };
                        string[] SettingsArray = CustomData.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        int Pos1 = SettingsArray[0].IndexOf(": ") + 2;
                        int Pos2 = SettingsArray[0].IndexOf(";");
                        AngleOffset = float.Parse(SettingsArray[0].Substring(Pos1, Pos2 - Pos1));

                        Pos1 = SettingsArray[1].IndexOf(": ") + 2;
                        Pos2 = SettingsArray[1].IndexOf(";");
                        Direction = float.Parse(SettingsArray[1].Substring(Pos1, Pos2 - Pos1));

                        Pos1 = SettingsArray[2].IndexOf(": ") + 2;
                        Pos2 = SettingsArray[2].IndexOf(";");
                        ProportionalGain = float.Parse(SettingsArray[2].Substring(Pos1, Pos2 - Pos1));

                        Pos1 = SettingsArray[3].IndexOf(": ") + 2;
                        Pos2 = SettingsArray[3].IndexOf(";");
                        RPMCap = float.Parse(SettingsArray[3].Substring(Pos1, Pos2 - Pos1));
                    }
                    catch (Exception)
                    {
                        Echo("Custom data error with rotor " + ElSync[i].CustomName + "Using default values");
                    }



                    // Angle Offset
                    float ElTargetAngle = ElTargetRotor.Angle;
                    ElTargetAngle = ElTargetAngle + (AngleOffset * 0.0174533f);

                    // Direction
                    if (Direction == -1)
                    {
                        ElTargetAngle = (MathHelper.TwoPi - ElTargetAngle);
                    }

                    // Sync calc
                    float Eldifference = (ElTargetAngle - ElSync[i].Angle);

                    Eldifference %= MathHelper.TwoPi; // Make sure that we are less than -360 deg and 360 deg by wrapping it around and getting the remainder

                    if (Eldifference > MathHelper.Pi)
                    {
                        Eldifference = -MathHelper.TwoPi + Eldifference;
                    }
                    else if (Eldifference < -MathHelper.Pi)
                    {
                        Eldifference = MathHelper.TwoPi + Eldifference;
                    }
                    float ElStabVelocity = ProportionalGain * Eldifference;

                    // RPM Cap
                    if (ElStabVelocity > RPMCap)
                    {
                        ElStabVelocity = RPMCap;
                    }
                    if (ElStabVelocity < (RPMCap * -1))
                    {
                        ElStabVelocity = (-1 * RPMCap);
                    }
                    ElSync[i].TargetVelocityRPM = ElStabVelocity;
                }
            }
        }
    }