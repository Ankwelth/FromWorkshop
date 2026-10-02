
    readonly IMyCameraBlock camera;
    Vector3D lastposition = Vector3D.Zero;
    Vector3D position = Vector3D.Zero;
    float speed;
    Vector3D velocity = Vector3D.Zero;
    _console console;
    public Program()
    {
        string customdata = Me.CustomData;
        string outputname = "output";
        string cameraname = "Camera";
        string[] lines = customdata.Split('\n');
        for(int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Split('=')[0].Trim() == "output") outputname = lines[i].Split('=')[1].Trim();
            if (lines[i].Split('=')[0].Trim() == "camera") cameraname = lines[i].Split('=')[1].Trim();
        }
        console = new _console(this,GridTerminalSystem.GetBlockWithName(outputname) as IMyTextPanel);
        camera = GridTerminalSystem.GetBlockWithName(cameraname) as IMyCameraBlock;
        camera.EnableRaycast = true;
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        console.log("Program successfully compiled");
        //textpanel = GridTerminalSystem.GetBlockWithName("JOMAMA") as IMyTextPanel;
        // The constructor, called only once every session and
        // always before any other method is called. Use it to
        // initialize your script. 
        //     
        // The constructor is optional and can be removed if not
        // needed.
        // 
        // It's recommended to set RuntimeInfo.UpdateFrequency 
        // here, which will allow your script to run itself without a 
        // timer block.
    }

    public void Save()
    {
        // Called when the program needs to save its state. Use
        // this method to save your state to the Storage field
        // or some other means. 
        // 
        // This method is optional and can be removed if not
        // needed.
    }

    public void Main(string argument, UpdateType updateSource)
    {
        position = Me.Position;
        if (!lastposition.Equals(Vector3D.Zero))
        {
            double deltaTime = Runtime.TimeSinceLastRun.TotalSeconds;
            speed = (float)(Vector3D.Distance(position, lastposition) / deltaTime);
            Vector3D change = (position - lastposition);
            velocity = new Vector3D(change.X / deltaTime,change.Y/deltaTime,change.Z/deltaTime);
        }
        if (argument != "")
        {
            if (argument[0] == '%') { console.log(argument.Remove(0, 1)); return; }
            if (argument.ToLower() == "$scan") { Scan(); return; }
            if (argument.ToLower() == "$clear") { console.Clear();return; }
        }
        // The main entry point of the script, invoked every time
        // one of the programmable block's Run actions are invoked,
        // or the script updates itself. The updateSource argument
        // describes where the update came from.
        // 
        // The method itself is required, but the arguments above
        // can be removed if not needed.
        console.Write();
    }
    public class _console
    {
        public static byte maxlines = 17;
        public readonly IMyTextPanel output;
        public readonly Program program;
        public System.Collections.Generic.List<string> queue = new System.Collections.Generic.List<string>();
        public _console(Program program,IMyTextPanel output) { 
            this.output = output;
            this.program = program;
        }
        public void log(string text)
        {
            /*
            Echo(line);
            byte lines = (byte)line.Split('\n').Length;
            if (lines == 0 || lines > maxlines) return;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            output.ReadText(stringBuilder);
            string currenttext = stringBuilder.ToString();
            if (currenttext.Split('\n').Length + lines <= maxlines && currenttext == "") output.WriteText(line, false);
            else if (currenttext.Split('\n').Length < maxlines) output.WriteText("\n" + line, true);
            else
            {
                string toBeOutput = "";
                for (int i = 1; i < currenttext.Split('\n').Length; i++) toBeOutput += "\n" + currenttext.Split('\n')[i];
                toBeOutput += "\n" + line;
                output.WriteText(toBeOutput.Remove(0, 1), false);
            }
            output.CustomData += line + "\n";
            */
            queue.AddRange(text.Split('\n'));

        }
        public void Clear()
        {
            output.WriteText("-----Console Cleared--------------------------------------", false);
            output.CustomData = "";
            program.Echo("Cleared");
        }
        public void Write()
        {
            if (queue.Count == 0) return;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            output.ReadText(stringBuilder);
            string[] currentlines = stringBuilder.ToString().Split('\n');
            System.Collections.Generic.List<string> lines = new System.Collections.Generic.List<string>(currentlines);
            lines.AddRange(queue);
            short startIndex = (short)(lines.Count-maxlines);
            if(startIndex < 0) startIndex = 0;
            short lineCount;
            string text = "";
            if (lines.Count <= maxlines) lineCount = (short)lines.Count;
            else lineCount = maxlines;
            for(int i = 0; i < lineCount;i++)
            {
                if (text != "") text += "\n";
                text += lines[i + startIndex];
            }
            output.WriteText(text);
            queue.Clear();
        }
    }
    void Scan()
    {
        MyDetectedEntityInfo entity = camera.Raycast(1000, 0, 0);
        if (entity.Type == MyDetectedEntityType.None) { console.log("No Object Detected"); return; }
        console.log("Detected entity of type " + entity.Type.ToString());
        console.log("With name " + entity.Name);
        console.log("And distance of " + Vector3D.Distance((Vector3D)entity.HitPosition, Me.GetPosition()));
        console.log("And relative velocity of " + Vector3D.Distance(entity.Velocity,velocity));
    }