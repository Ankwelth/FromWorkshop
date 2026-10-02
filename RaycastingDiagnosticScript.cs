// Space Engineers Raycasting Diagnostic Script
// By Arguswastaken

// To use this script, add a camera to your grid (probably facing forwards) and make sure to name it "RDS Raycast Camera".
// Change the scanRange field below to the raycast distance you prefer to use. 2000 meters is the default setting.
// Then recompile the script to complete the setup. After that, point the camera at any entity and run the script.
// If it finds a target it will give you information about it in the script output, including the entity type (like small or large grid),
// its display name and position, and relationship to you.

// I created this for obtaining the exact coordinates of enemy bases of which I have line of sight, so I could turn them into
// personal GPS signals so that attacking them is easier and other scripts are able to use the precise location.

// Distance to scan, in meters (e.g. 2000 = 2km)
private int scanRange = 2000;


// ------------------------------------------------------------------------


private IMyCameraBlock camera;
private MyDetectedEntityInfo info;

public Program()
{
    camera = GridTerminalSystem.GetBlockWithName("RDS Raycast Camera") as IMyCameraBlock;
    camera.EnableRaycast = true;
}

public void Main(string arguments)
{
    if (!camera.CanScan(scanRange)) return;

    info = camera.Raycast(2000, 0, 0);
    if (info.HitPosition.HasValue) Echo(CollateTargetInfo(info));
    else Echo("Raycast did not find target.");
}

private string CollateTargetInfo(MyDetectedEntityInfo target)
{
    return $"Camera Raycast Target Information\n\nType: {target.Type}\n" +
    $"Display Name: {target.Name}\n" +
    $"Relationship: {target.Relationship.ToString()}\n" +
    $"Position: {target.Position.ToString()}";
}