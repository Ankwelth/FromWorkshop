private readonly GPSInterfaceV3Ini gps;
private GPSInterfaceV3Ini.Future<bool> active;
private GPSInterfaceV3Ini.Future<int> gpsHash;
private GPSInterfaceV3Ini.Future<int> editHash;
private GPSInterfaceV3Ini.Future<List<GPSInterfaceV3Ini.GPS_Entry>> list;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    gps = new GPSInterfaceV3Ini(Me);
}

void Main()
{
    gps.Update();
    if (gpsHash == null)
    {
        if (active == null)
        {
            active = gps.IsActive();
        }
        else if (!active.Finished())
        {
            Echo("Do you call \"gps.Update()\"?");
            Echo("Is an antenna on the grid?");
            Echo("Do you have access to the antenna?");
            Echo("");
            Echo("Is the GPS Ingame API active?");
            Echo("Download from Steam Workshop:");
            Echo("http://steamcommunity.com/sharedfiles/filedetails/?id=1272904212");
            active = null;
        }
        else
        {
            active = null;
            gps.Clear();
            gpsHash = gps.AddGps("I am here", Me.CubeGrid.GridIntegerToWorld(Me.Position), "Position of the test scripts pb");
        }
    }
    else if (gpsHash.Finished())
    {
        if (gpsHash.Failed())
        {
            Echo("Failed to create gps marker: " + gpsHash.GetError());
        }
        else
        if (editHash != null && editHash.Finished() && editHash.Result() == -1)
        {
            gps.Clear();
            gpsHash = gps.AddGps("I am here", Me.CubeGrid.GridIntegerToWorld(Me.Position), "Position of the test scripts pb");
            editHash = null;
            Echo("Gps marker created (again)");
        }
        else if (editHash == null || editHash.Finished())
        {
            if (list != null && list.Finished())
            {
                Echo("All GPS-Entries (created by " + gps.GetInterfaceBlock().CustomName + ")");
                list.Result().Select(e => string.Format("{0}: {1},{2},{3}", e.Name, e.X, e.Y, e.Z)).ToList().ForEach(s => Echo(s));
            }
            editHash = gps.EditGps(editHash == null ? gpsHash.Result() : editHash.Result(), Me.CubeGrid.GridIntegerToWorld(Me.Position));
            list = gps.List();
            Echo("Gps marker moved");
        }
        else
        {
            Echo("Moving gps marker");
        }
    }
    else
    {
        Echo("Creating gps marker");
    }
    gps.Commit();
}

//--------------------------------------------------------------------- 
sealed class GPSInterfaceV3Ini
{
    private readonly IMyTerminalBlock TerminalBlock;
    private readonly List<IFuture> Active = new List<IFuture>();
    private readonly List<IFuture> Scheduled = new List<IFuture>();
    private bool Dirty = true;

    public GPSInterfaceV3Ini(IMyTerminalBlock terminalBlock)
    {
        TerminalBlock = terminalBlock;
    }

    public IMyTerminalBlock GetInterfaceBlock() => TerminalBlock;

    public void Update()
    {
        if (IsOutReady())
        {
            int start = -1;
            int end = -1;
            bool error = false;


            string[] lines = TerminalBlock.CustomData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            int offset = 1;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0 || line.StartsWith(";"))
                {
                    if (start != -1)
                    {
                        offset++;
                    }
                    continue;
                }
                if (start != -1)
                {
                    if (line.StartsWith("["))
                    {
                        break;
                    }
                    end = i;
                    int index = i - start - offset;
                    int eq = line.IndexOf('=');
                    if (eq == -1)
                    {
                        error = true;
                        break;
                    }
                    if (index < Active.Count)
                    {
                        Active[index].Handle(line.Substring(eq + 1).Trim());
                    }
                }
                if (line.Equals("[Gps-Scripting:OUT_ini]"))
                {
                    start = i;
                    end = i;
                }
            }
            Active.Clear();
            if (error)
            {
                throw new Exception("Ini parser error");
            }
            TerminalBlock.CustomData = string.Join("\r\n", lines.Take(start).Concat(lines.Skip(end + 1)));
            Dirty = false;
        }
        else if (!IsInReady())
        {
            Dirty = false;
        }
    }

    public void Commit()
    {
        if (!Dirty && !IsInReady() && Scheduled.Count > 0)
        {
            StringBuilder sb = new StringBuilder("\r\n[Gps-Scripting:IN_ini]");
            int l = 0;
            foreach (IFuture future in Scheduled)
            {
                sb.Append("\r\nl").Append(l).Append('=').Append(future.GetInLine());
                Active.Add(future);
                l++;
            }
            TerminalBlock.CustomData += sb.ToString();

            Scheduled.Clear();
            Dirty = true;
        }
    }

    private bool IsInReady() => TerminalBlock.CustomData.Contains("[Gps-Scripting:IN_ini]");
    private bool IsOutReady() => TerminalBlock.CustomData.Contains("[Gps-Scripting:OUT_ini]");

    private void Command(IFuture future) => Scheduled.Add(future);

    public Future<bool> IsActive()
    {
        Future<bool> r = new FutureBoolean("Active?");
        Command(r);
        return r;
    }

    public Future<int> AddGps(string name, Vector3D position, string description = "", bool persistent = false, bool showOnHud = true)
    {
        Future<int> r = new FutureInt(string.Format("Add:{0}:{1}:{2}:{3}:{4}:{5}:{6}", name, description.Replace("\r\n", "\\r\\n").Replace("\n", "\\n"), position.X, position.Y, position.Z, persistent ? "Y" : "N", showOnHud ? "Y" : "N"));
        Command(r);
        return r;
    }

    public Future<int> EditGps(int gpsHash, Vector3D position)
    {
        Future<int> r = new FutureInt(string.Format("Edit:{0}:{1}:{2}:{3}", gpsHash, position.X, position.Y, position.Z));
        Command(r);
        return r;
    }

    public Future<List<GPS_Entry>> List()
    {
        Future<List<GPS_Entry>> r = new FutureListGPS_Entry(string.Format("List"));
        Command(r);
        return r;
    }

    public Future<bool> RemoveGps(int gpsHash)
    {
        Future<bool> r = new FutureBoolean(string.Format("Remove:{0}", gpsHash));
        Command(r);
        return r;
    }

    public Future<bool> Clear()
    {
        Future<bool> r = new FutureBoolean("Clear");
        Command(r);
        return r;
    }

    interface IFuture
    {
        string GetInLine();
        void Handle(string line);
    }
    public abstract class Future<T> : IFuture
    {
        private string cache;
        private T result;
        readonly string InLine;
        private string error;

        protected Future(string inLine)
        {
            InLine = inLine;
        }

        public string GetInLine() => InLine;

        public void Handle(string line)
        {
            cache = line;
            try
            {
                result = Handle0(line, out error);
            }
            catch (Exception e)
            {
                error = e.Message;
            }
        }

        protected abstract T Handle0(string line, out string error);

        public T Result()
        {
            if (Failed())
            {
                throw new Exception("Future failed: " + error);
            }
            if (Finished())
            {
                return result;
            }
            throw new Exception("Future not finished");
        }

        public bool Finished() => cache != null;
        public bool Failed() => error != null;

        public string GetError() => error;
    }
    private class FutureBoolean : Future<bool>
    {
        public FutureBoolean(string inLine) : base(inLine) { }
        protected override bool Handle0(string line, out string error)
        {
            error = null;
            return line.Equals("Ok");
        }
    }

    private class FutureInt : Future<int>
    {
        public FutureInt(string inLine) : base(inLine) { }
        protected override int Handle0(string line, out string error)
        {
            try
            {
                error = null;
                return int.Parse(line);
            }
            catch (ArgumentException)
            {
                error = line;
                return -1;
            }
        }
    }

    private class FutureListGPS_Entry : Future<List<GPS_Entry>>
    {
        public FutureListGPS_Entry(string inLine) : base(inLine) { }
        protected override List<GPS_Entry> Handle0(string line, out string error)
        {
            error = null;
            return line.Split(new string[] { "::" }, StringSplitOptions.None).Where(e => e.Length > 0).Select(i => new GPS_Entry(i)).ToList();
        }
    }

    public sealed class GPS_Entry
    {
        public readonly string Name;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public GPS_Entry(string item)
        {
            string[] items = item.Split(new char[] { ':' }).Select(e => e.Trim()).ToArray();
            Name = items[0];
            X = double.Parse(items[1]);
            Y = double.Parse(items[2]);
            Z = double.Parse(items[3]);
        }
    }
}
