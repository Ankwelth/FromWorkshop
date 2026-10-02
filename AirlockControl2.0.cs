public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
string[] data;

//called when button pressed or on update
public void Main(string argument)
{
    if (!argument.Equals(""))
    {
        Object[] newStatus = SplitArgs(argument);
        if ((int)newStatus[0] != -1)
        {
            data = ReadCustomData();
            data = updateData(data, (int)newStatus[0], (string)newStatus[1]);
            Me.CustomData = joinCustomData(data);
        }
    }
    else
    {
        data = ReadCustomData();
        updateDoors();
        Me.CustomData = joinCustomData(data);
    }
}
//reads the button input arguments, then splits them and returns the new door state
public Object[] SplitArgs(string arguments)
{
    string[] splitargs = arguments.Split('\"');
    //when split like this, index 0 == ""
    int newStatus = -1;
    
    if (splitargs.Length == 3)
    {
        splitargs[2] = splitargs[2].Replace(" ", string.Empty);
        if (string.Equals(splitargs[2], "in"))
        {
            newStatus = 3;
        }
        if (string.Equals(splitargs[2], "out"))
        {
            newStatus = 1;
        }
    }
    else
    {
        Echo("Can't read arguments. Are they in the Right Format?\n\"Airlock Name\" out/in\nor\n\"all\" out/in");
        return new Object[2]{-1, ""};
    }
    Object[] status = new Object[2]{newStatus, splitargs[1]};
    return status;
}
//updates the parsed data with the new door status for the given airlock
public string[] updateData(string[] olddata, int newStatus, string targetAirlock)
{
    for (int i = 0; i < olddata.Length; i++)
    {
        if (i % 5 == 4)
        {
            try
            {
                string thisAirlock = olddata[i-4].Replace("\n",string.Empty);
                if (thisAirlock.Equals(targetAirlock) || targetAirlock.Equals("all"))
                {
                    int status = Int32.Parse(olddata[i]);
                    status = newStatus;
                    olddata[i] = status.ToString();
                }
            }
            catch (FormatException)
            {
                Echo("it goofed, can't read airlock status");
                break;
            }
        }
    }
    return olddata;
}
//updates all airlocks
public void updateDoors()
{
    for (var i = 0; i < data.Length; i++)
    {
        if (i % 5 == 4)
        {
            try
            {
                int status = Int32.Parse(data[i]);
                status = updateThisDoor(i - 4, status);
                data[i] = status.ToString();
            }
            catch (FormatException)
            {
                Echo("it goofed, can't read airlock status");
                return;
            }
        }
    }
}
//updates the specific airlock
public int updateThisDoor(int l, int currentStatus)
{
    IMyDoor doorout = GridTerminalSystem.GetBlockWithName(data[l + 1]) as IMyDoor;
    IMyDoor doorin = GridTerminalSystem.GetBlockWithName(data[l + 2]) as IMyDoor;
    IMyAirVent airvent = GridTerminalSystem.GetBlockWithName(data[l + 3]) as IMyAirVent;
    //1 is starting cycle out
    if (currentStatus == 1)
    {
        if ((int)doorin.Status < 2)
        {
            doorin.CloseDoor();
        }
        else if ((int)doorin.Status == 3)
        {
            doorin.Enabled = false;
            airvent.Depressurize = true;
            if (airvent.GetOxygenLevel() <= 0.025)
            {
                doorout.Enabled = true;
                doorout.OpenDoor();
                currentStatus = 2;
            }
        }
        //3 is cycling in
    }
    else if (currentStatus == 3)
    {
        if ((int)doorout.Status < 2)
        {
            doorout.CloseDoor();
        }
        else if ((int)doorout.Status == 3)
        {
            doorout.Enabled = false;
            airvent.Depressurize = false;
            if (airvent.GetOxygenLevel() >= 1)
            {
                doorin.Enabled = true;
                doorin.OpenDoor();
                currentStatus = 0;
            }
        }
    }
    return currentStatus;
}
//reads the blocks custom data and parses it out
public string[] ReadCustomData()
{
    string data = Me.CustomData;
    string[] parsedData = data.Split(',');

    return parsedData;
}
//takes the parsed data and shoves it back into one string to be saved back to the block
public static string joinCustomData(string[] arraydata)
{
    string newdata = string.Join(",", arraydata);
    return newdata;
}