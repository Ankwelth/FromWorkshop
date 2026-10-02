/*******************************************************************************************************************
 * [PAM]-E.L.A. - Path Auto Miner Exclusive Local Area
 *
 * Author:            Mythos
 * Version:          1.0.0 (2019-11-08)
 *
 * URL: https://steamcommunity.com/sharedfiles/filedetails/?id=1908865520
 *
 * This is a hangar utility script for the great [PAM] by Keks
 * PAM: https://steamcommunity.com/sharedfiles/filedetails/?id=1507646929
 *******************************************************************************************************************/

/*******************************************************************************************************************
 * One PAM-Controller is needed to command drones. All drones must use PAM and broadcast 
 * to that controller, with broadcast-name = grid-name (see PAM guide for setup). One 
 * PAM-Controller could be reused for multiple PAM-ELAs.
 *******************************************************************************************************************/
const string controllerTag = "[PAM-Controller]";

/*******************************************************************************************************************
 * A sensor is needed to detect drones entering the exclusive area. In a hangar you 
 * probably have such a sensor controlling the hangar doors. The area can be extended to
 * use multiple sensors that will be combined into one large exlusive area. They need to
 * have the same [PAM-ELA] tag in their name then.
 * The sensor must be configured to detect ships (size depends on your drones) but NOT
 * detect subgrids. A drone will be considered a subgrid after connection, and the sensor
 * should consider the area to be free then.
 * If you want to control multiple areas / hangars allowing for one active drone in each,
 * you'll need to set up one programmable block with PAM-ELA for each hangar. They will
 * need unique sensor and panel tags like [PAM-ELA-1], [PAM-ELA-2] or similar.
 *******************************************************************************************************************/
const string sensorTag = "[PAM-ELA]";

/*******************************************************************************************************************
 * The list of requests to enter the area can be shown on any LCD panel with this tag in
 * it's name. Using indexed text sufaces like on the PB or in a cockpit is also possible,
 * the tag therefor has to look like [PAM-ELA:#] with # being the text surface index
 * starting at 1.
 *******************************************************************************************************************/
const string panelTag = "[PAM-ELA]";

/*******************************************************************************************************************
 * There is always only one active drone allowed to move in the exclusive area. Other
 * drones will be sent the STOP command via the PAM-Controller. When a stopped drone
 * becomes top of the reuqest list, it will be activated via the CONT command. If it does
 * not react and free the area (maybe because the job was done), after <resendContAfter> 
 * minutes it will be sent CONT again every minute. After <sendHomeAfter> minutes however,
 * it will be sent the HOMEPOS command, finally clearing the area.
 *******************************************************************************************************************/
const int resendContAfter = 1; // minutes
const int sendHomeAfter = 3; // minutes



/*------------------------------------------ please do not change below -------------------------------------------*/

List<IMyProgrammableBlock> controllers = new List<IMyProgrammableBlock>();
List<IMySensorBlock> sensors = new List<IMySensorBlock>();
List<IMyTextSurface> panels = new List<IMyTextSurface>();
List<MyDetectedEntityInfo> entities = new List<MyDetectedEntityInfo>();
List<string> requests = new List<string>();
System.DateTime request0Active = System.DateTime.Now;
int request0Minutes = 0;
const string line = "—————————————————";

public Program()
{
  Runtime.UpdateFrequency = UpdateFrequency.Update10;

  string[] stored = Storage.Split(';');
  requests.AddRange(stored);

  ScanGrid();
}

public void Save()
{
  Storage = string.Join(";", requests);
}

public void ScanGrid()
{
  controllers.Clear();
  sensors.Clear();
  panels.Clear();
  
  List<IMyTerminalBlock> list = new List<IMyTerminalBlock>();
  
  GridTerminalSystem.SearchBlocksOfName(controllerTag, list, x => x is IMyProgrammableBlock && x.IsSameConstructAs(Me));
  foreach (var item in list)
  {
    controllers.Add(item as IMyProgrammableBlock);
  }

  GridTerminalSystem.SearchBlocksOfName(sensorTag, list, x => x is IMySensorBlock && x.IsSameConstructAs(Me));
  foreach (var item in list)
  {
    sensors.Add(item as IMySensorBlock);
  }

  GridTerminalSystem.SearchBlocksOfName(panelTag, list, x => x is IMyTextSurface && x.IsSameConstructAs(Me));
  foreach (var item in list)
  {
    panels.Add(item as IMyTextSurface);
  }

  StringBuilder tagBuilder = new StringBuilder(panelTag);
  tagBuilder[panelTag.Length - 1] = ':';
  string tag = tagBuilder.ToString();
  GridTerminalSystem.SearchBlocksOfName(tag, list, x => x is IMyTextSurfaceProvider && x.IsSameConstructAs(Me));
  foreach (var item in list)
  {
    IMyTextSurfaceProvider provider = item as IMyTextSurfaceProvider;

    string s = item.CustomName;
    int start = s.IndexOf(tag) + tag.Length;
    int end  = s.IndexOf(panelTag[panelTag.Length - 1], start);

    int surface;
    if (int.TryParse(s.Substring(start, end - start), out surface))
    {
      if (surface > 0 && surface <= provider.SurfaceCount)
      {
        panels.Add(provider.GetSurface(surface - 1));
      }
    }
  }
}

public void Main(string argument, UpdateType updateSource)
{
  PanelClear();
  Print("[PAM]-E.L.A.");
  Print(line);
  Echo($"Controllers found: {controllers.Count}" + (controllers.Count != 1 ? " WARNING!" : ""));
  Echo($"Sensors found: {sensors.Count}" + (sensors.Count < 1 ? " WARNING!" : ""));
  Echo($"Panels found: {panels.Count}");
  Echo(line);

  bool foundRequest0 = false;

  foreach (var sensor in sensors)
  {
    sensor.DetectedEntities(entities);
  
    if (entities.Count > 1)
    {
      entities.Sort(delegate(MyDetectedEntityInfo x, MyDetectedEntityInfo y)
      {
        return x.Name.CompareTo(y.Name);
      });
    }

    foreach (var entity in entities)
    {
      Request(entity.Name);
      
      if (requests.Count > 0 && entity.Name == requests[0])
      {
        foundRequest0 = true;
      }
    }
  }
  System.TimeSpan span = System.DateTime.Now - request0Active;
  double seconds = span.TotalSeconds;

  Print($"Exclusive local area requests: {requests.Count}");
  foreach (string request in requests)
  {
    if (requests.Count > 0 && request == requests[0])
    {
      Print($"> {request} (go {request0Minutes}:{seconds:00})");
    }
    else
    {
      Print($"  {request} (wait)");
    }
  }

  if (foundRequest0)
  {
    if (span.TotalMinutes > 1)
    {
      request0Active = System.DateTime.Now;
      request0Minutes++;

      if (request0Minutes >= sendHomeAfter)
      {
        SendHome(requests[0]);
      }
      else if (request0Minutes >= resendContAfter)
      {
        SendCont(requests[0]);
      }
    }
  }
  else
  {
    if (requests.Count > 0)
    {
      requests.RemoveAt(0);
    }
    if (requests.Count > 0)
    {
      SendCont(requests[0]);
      request0Active = System.DateTime.Now;
      request0Minutes = 0;
    }
  }
  
  Echo(line);
  Echo($"Instructions: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}");
}

public void Request(string s)
{
  if (!requests.Contains(s))
  {
    if (requests.Count > 0)
    {
      SendStop(s);
    }
    else
    {
      request0Active = System.DateTime.Now;
      request0Minutes = 0;
    }
    requests.Add(s);
  }
}

public void SendStop(string s)
{
  if (controllers.Count > 0)
  {
    controllers[0].TryRun($"SEND {s}:STOP");
  }
}

public void SendCont(string s)
{
  if (controllers.Count > 0)
  {
    controllers[0].TryRun($"SEND {s}:CONT");
  }
}

public void SendHome(string s)
{
  if (controllers.Count > 0)
  {
    controllers[0].TryRun($"SEND {s}:HOMEPOS");
  }
}

public void PanelClear()
{
  foreach (var panel in panels)
  {
    panel.WriteText("", false); // clear
    panel.ContentType = ContentType.TEXT_AND_IMAGE;
  }
}

public void Print(string s)
{
  Echo(s);
  foreach (var panel in panels)
  {
    panel.WriteText($"{s}\n", true);
  }
}