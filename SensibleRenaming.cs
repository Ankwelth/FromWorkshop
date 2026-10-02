/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

readonly SensibleNamer sn;
readonly ProjectorManager pm;

public Program()
{
    sn = new SensibleNamer(GridTerminalSystem, Me, Echo);
    pm = new ProjectorManager(GridTerminalSystem, Me, Echo);
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument == "align proj")
    {
        pm.Main();
    }
    else
    {
        sn.Main(argument, updateSource);
    }
}

}
internal class ProjectorManager
{
    readonly List<IMyProjector> projectors = new List<IMyProjector>();
    readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    IMyGridTerminalSystem GridTerminalSystem { get; set; }
    readonly IMyTerminalBlock Me;
    readonly Action<string> Echo;

    public ProjectorManager(IMyGridTerminalSystem gridTerminalSystem, IMyTerminalBlock Me, Action<string> E)
    {
        GridTerminalSystem = gridTerminalSystem;
        this.Me = Me;
        Echo = E;
    }

    internal void Main()
    {
        blocks.Clear();
        GridTerminalSystem.GetBlocksOfType(blocks, x => Me.IsSameConstructAs(x));
        Vector3I lowDim = blocks[0].Position; //A Pb means Block 0 exists
        GridTerminalSystem.GetBlocksOfType(projectors);
        if (projectors.Count == 0)
        {
            Echo("No projector found.");
        }
        if (projectors.Count > 1)
        {
            Echo("Too many projectors found.");
        }
        else
        {
            AutoAlign(lowDim);
        }
    }

    private void AutoAlign(Vector3I lowDim)
    {
        var pr = projectors[0];

        var vi = pr.Position - lowDim;
        MatrixI rot = MatrixI.CreateRotation(projectors[0].Orientation.Forward, projectors[0].Orientation.Up, Base6Directions.Direction.Forward, Base6Directions.Direction.Up);
        pr.ProjectionOffset = Vector3I.Transform(vi, rot);

        var prot = new Vector3I(0, 0, 0);
        if (pr.Orientation.Forward == Base6Directions.Direction.Right)
        {
            prot.X = 1;
            if (pr.Orientation.Up == Base6Directions.Direction.Up)
            {
                prot.Z = 0;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Forward)
            {
                prot.Z = -1;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Down)
            {
                prot.Z = 2;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Backward)
            {
                prot.Z = 1;
            }
        }
        else if (pr.Orientation.Forward == Base6Directions.Direction.Backward)
        {
            prot.X = 2;
            if (pr.Orientation.Up == Base6Directions.Direction.Up)
            {
                prot.Z = 0;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Left)
            {
                prot.Z = 1;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Down)
            {
                prot.Z = 2;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Right)
            {
                prot.Z = -1;
            }
        }
        else if (pr.Orientation.Forward == Base6Directions.Direction.Left)
        {
            prot.X = -1;
            if (pr.Orientation.Up == Base6Directions.Direction.Up)
            {
                prot.Z = 0;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Forward)
            {
                prot.Z = 1;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Down)
            {
                prot.Z = 2;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Backward)
            {
                prot.Z = -1;
            }
        }
        else if (pr.Orientation.Forward == Base6Directions.Direction.Forward)
        {
            //prot.X = 0;
            if (pr.Orientation.Up == Base6Directions.Direction.Up)
            {
                prot.Z = 0;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Left)
            {
                prot.Z = -1;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Down)
            {
                prot.Z = 2;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Right)
            {
                prot.Z = 1;
            }
        }
        else if (pr.Orientation.Forward == Base6Directions.Direction.Up)
        {
            //prot.X = 0;
            prot.Y = 1;
            if (pr.Orientation.Up == Base6Directions.Direction.Forward)
            {
                prot.Z = 2;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Left)
            {
                prot.Z = -1;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Backward)
            {
                prot.Z = 0;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Right)
            {
                prot.Z = 1;
            }
        }
        else if (pr.Orientation.Forward == Base6Directions.Direction.Down)
        {
            //prot.X = 0;
            prot.Y = -1;
            if (pr.Orientation.Up == Base6Directions.Direction.Forward)
            {
                prot.Z = 0;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Left)
            {
                prot.Z = -1;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Backward)
            {
                prot.Z = 2;
            }
            else if (pr.Orientation.Up == Base6Directions.Direction.Right)
            {
                prot.Z = 1;
            }
        }
        pr.ProjectionRotation = prot;

        pr.UpdateOffsetAndRotation();
    }
}

internal class RenameStrategy
{
    readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    readonly List<IMyRemoteControl> controllers = new List<IMyRemoteControl>();
    readonly Dictionary<string, List<IMyTerminalBlock>> blockDict = new Dictionary<string, List<IMyTerminalBlock>>();
    readonly IMyTerminalBlock Me;
    IMyRemoteControl front;


    IMyGridTerminalSystem GridTerminalSystem { get; set; }


    public RenameStrategy(IMyGridTerminalSystem gridTerminalSystem, IMyTerminalBlock Me, Action<string> E)
    {
        GridTerminalSystem = gridTerminalSystem;
        this.Me = Me;
    }

    public void NameEverything(string gridName)
    {
        blocks.Clear();
        blockDict.Clear();
        GridTerminalSystem.GetBlocksOfType(blocks, x => Me.IsSameConstructAs(x));

        GridTerminalSystem.GetBlocksOfType(controllers, x => Me.IsSameConstructAs(x));
        front = controllers[0];
        MatrixI rot = MatrixI.CreateRotation(front.Orientation.Forward, front.Orientation.Up, Base6Directions.Direction.Forward, Base6Directions.Direction.Up);
        Vector3I com = front.CubeGrid.WorldToGridInteger(front.CenterOfMass);

        foreach (var block in blocks)
        {
            if (!blockDict.ContainsKey(block.DefinitionDisplayNameText))
            {
                blockDict.Add(block.DefinitionDisplayNameText, new List<IMyTerminalBlock>());
            }
            blockDict[block.DefinitionDisplayNameText].Add(block);
        }

        foreach (var pair in blockDict)
        {
            var list = pair.Value;

            list.Sort((x,y) => Vector3I.Transform(x.Position - com, rot).Z.CompareTo(Vector3I.Transform(y.Position - com, rot).Z));

            int leadingZeros = list.Count.ToString().Length;
            var format = new string('0', leadingZeros);
            for (int i = 0; i < list.Count; ++i)
            {
                var block = list[i];
                var formattedNum = $" {(i + 1).ToString(format)}";
                if (list.Count == 1) formattedNum = "";
                //block.CustomName = $"{gridName}-{block.DefinitionDisplayNameText}{formattedNum} {PreserveTags(block.CustomName)}";
                var tags = PreserveTags(block.CustomName);

                bool po = Vector3I.Transform(block.Position - com, rot).X < 0;
                var side = po ? "Po" : "Sb";
                if (list.Count == 1) side = "";
                block.CustomName = $"{gridName} {Renamer(block.DefinitionDisplayNameText)} {side}{formattedNum}{tags}";
            }
        }
    }

    private string Renamer(string name)
    {
        var result = name;
        result = result.Replace("[", "");
        result = result.Replace("]", "");
        result = result.Replace("Programmable Block", "PB");
        result = result.Replace("Remote Control", "RC");
        result = result.Replace("Atmospheric", "Atmo");
        return result;
    }

    public static string PreserveTags(string oldText)
    {
        string result = "";
        int ind = oldText.IndexOf('[');
        while (ind >= 0)
        {
            var nextBrace = oldText.Substring(ind);
            int len = nextBrace.IndexOf(']');
            result += " " + nextBrace.Substring(0, len + 1);
            if (len < 0 || len + 1 == nextBrace.Length) break;
            oldText = nextBrace.Substring(len + 1);
            ind = oldText.IndexOf('[');
        }
        result = result.TrimEnd(' ');
        return result;
    }
}

public class SensibleNamer
{
    // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
    readonly List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
    readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    readonly List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    readonly List<IMyShipController> contr = new List<IMyShipController>();
    RenameStrategy strategy;

    IMyGridTerminalSystem GridTerminalSystem { get; set; }
    readonly IMyTerminalBlock Me;
    readonly Action<string> Echo;

    public SensibleNamer(IMyGridTerminalSystem gridTerminalSystem, IMyTerminalBlock Me, Action<string> E)
    {
        GridTerminalSystem = gridTerminalSystem;
        this.Me = Me;
        Echo = E;
        GridTerminalSystem.GetBlockGroups(blockGroups);
        Echo("Run without any arguments to rename.\n" +
            "Run with 'align proj' to auto align repair projector.\n");
        strategy = new RenameStrategy(gridTerminalSystem, Me, E);
    }

    public void Main(string argument, UpdateType updateSource)
    {
        string gridName = "";
        gridName = GetGridAbbr(gridName);

        strategy.NameEverything(gridName);
        InitConnectors();
        HideStuff();
        HideEverythingInAGroup();
    }

    private void HideStuff()
    {
        blocks.Clear();
        GridTerminalSystem.GetBlocksOfType(blocks, x => Me.IsSameConstructAs(x));
        foreach (var block in blocks)
        {
            if (block.CustomName.Contains("Cargo")
                || block.CustomName.Contains("Gyro")
                 || block.CustomName.Contains("Thruster")
                 || block.CustomName.Contains("Armory")
                 || block.CustomName.Contains("Solar")
                 || block.CustomName.Contains("Turbine")
                 || block.CustomName.Contains("Locker")
                 || block.CustomName.Contains("Bed")
                 || block.CustomName.Contains("Desk")
                 || block.CustomName.Contains("Bathroom")
                 || block.CustomName.Contains("Battery")
                  || block.CustomName.Contains("Door"))
            {
                block.ShowInTerminal = false;
                block.ShowInToolbarConfig = false;
            }
        }

        foreach (var block in blocks)
        {
            if (block.CustomName.Contains("Light")
                || block.CustomName.Contains("LCD")
                )
            {
                block.ShowInToolbarConfig = false;
            }
        }
    }


    private void HideEverythingInAGroup()
    {
        foreach (var group in blockGroups)
        {
            blocks.Clear();
            group.GetBlocks(blocks, x => Me.IsSameConstructAs(x));
            foreach (var block in blocks)
            {
                block.ShowInTerminal = false;
                block.ShowInToolbarConfig = false;
            }
        }
    }

    private void InitConnectors()
    {
        blocks.Clear();
        GridTerminalSystem.GetBlocksOfType(connectors, x => Me.IsSameConstructAs(x));
        foreach (var connector in connectors)
        {
            connector.PullStrength = 0.0f;
        }
    }

    private string GetGridAbbr(string gridName)
    {
        GridTerminalSystem.GetBlocksOfType(contr, x => Me.IsSameConstructAs(x));
        if (contr.Count > 0)
        {
            var cn = contr[0].CubeGrid.CustomName;
            if (cn.Contains(" "))
            {
                var results = cn.Split(' ');
                foreach (var c in results)
                {
                    gridName += c[0];
                }
            }
            else
            {
                if (cn.Length > 2)
                {
                    gridName += $"{cn[0]}{cn[1]}{cn[cn.Length - 1]}";
                }
            }
        }

        return gridName;
    }