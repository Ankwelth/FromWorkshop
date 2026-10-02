public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument != "hit")
    {
        return;
    }

    var camera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
    if (camera == null)
    {
        Echo("cam not found");
        return;
    }

    if (!camera.EnableRaycast)
    {
        camera.EnableRaycast = true;
        Echo("Raycast...");
        return;
    }

    if (camera.CanScan(1000))
    {
        var info = camera.Raycast(3000);
        if (!info.IsEmpty() && info.HitPosition.HasValue)
        {
            var hitPos = info.HitPosition.Value;
            string gps = $"GPS:Marker:{hitPos.X:F2}:{hitPos.Y:F2}:{hitPos.Z:F2}:#FFF17575:";
            Me.CustomData = gps;
            Echo("Custom Data:");
            Echo(gps);
        }
        else
        {
            Echo("n");
        }
    }
}
