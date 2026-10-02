// Docking Station 2.0 - install in the STATION programmable block. C# 6.
// Annuncia solo i connettori non agganciati dello stesso costrutto.
// Requires a working antenna and the ship's NETWORK value. RELOAD refreshes blocks.
const string NETWORK = "DOCKING_HOME_V2";
const double LEASE_SECONDS = 5;
double now, broadcastAt, reloadAt;
long sequence;
List<IMyShipConnector> ports = new List<IMyShipConnector>();
Dictionary<long, Lease> leases = new Dictionary<long, Lease>();
Dictionary<long, double> freeSince = new Dictionary<long, double>();
class Lease { public long Owner; public string Token; public double Until; }

public Program() { Runtime.UpdateFrequency = UpdateFrequency.Update10; Reload(); }
public void Save() { }
void Reload()
{
    GridTerminalSystem.GetBlocksOfType(ports, b => b.IsSameConstructAs(Me));
    reloadAt = now + 10;
}
public void Main(string argument, UpdateType updateSource)
{
    now += Math.Max(0, Runtime.TimeSinceLastRun.TotalSeconds);
    if (argument.Trim().ToUpperInvariant() == "RELOAD" || now >= reloadAt) Reload();
    foreach (var p in ports)
    {
        if (p.Closed || p.Status != MyShipConnectorStatus.Unconnected) freeSince.Remove(p.EntityId);
        else if (!freeSince.ContainsKey(p.EntityId)) freeSince[p.EntityId] = now;
    }
    int budget = 40;
    while (IGC.UnicastListener.HasPendingMessage && budget-- > 0)
    {
        var msg = IGC.UnicastListener.AcceptMessage();
        if (msg.Tag != NETWORK || !(msg.Data is string)) continue;
        string[] f = ((string)msg.Data).Split('|');
        long id;
        if (f.Length != 3 || !long.TryParse(f[1], out id) || f[2].Length > 80) continue;
        Lease lease;
        leases.TryGetValue(id, out lease);
        if (f[0] == "RELEASE")
        {
            if (lease != null && lease.Owner == msg.Source && lease.Token == f[2]) leases.Remove(id);
            continue;
        }
        if (f[0] != "RESERVE") continue;
        IMyShipConnector port = ports.Find(b => b.EntityId == id && !b.Closed);
        double since;
        bool renewal = lease != null && lease.Until > now && lease.Owner == msg.Source && lease.Token == f[2];
        // After restarting, wait for any previous reservations to expire.
        bool grant = now >= LEASE_SECONDS && port != null && port.IsWorking && port.CubeGrid.IsStatic
            && port.Status != MyShipConnectorStatus.Connected
            && (renewal || (port.Status == MyShipConnectorStatus.Unconnected && freeSince.TryGetValue(id, out since) && now - since >= 5))
            && (lease == null || lease.Until <= now || (lease.Owner == msg.Source && lease.Token == f[2]));
        if (grant) leases[id] = new Lease { Owner = msg.Source, Token = f[2], Until = now + LEASE_SECONDS };
        IGC.SendUnicastMessage(msg.Source, NETWORK, (grant ? "GRANT|" : "DENY|") + id + "|" + f[2]);
    }
    if (now >= broadcastAt)
    {
        broadcastAt = now + 0.5;
        sequence++;
        foreach (var p in ports)
        {
            if (p.Closed) continue;
            Lease lease;
            long owner = leases.TryGetValue(p.EntityId, out lease) && lease.Until > now ? lease.Owner : 0;
            double since;
            bool visible = p.Status == MyShipConnectorStatus.Unconnected && freeSince.TryGetValue(p.EntityId, out since) && now - since >= 5 && owner == 0;
            if (!visible) IGC.SendBroadcastMessage(NETWORK, "HIDE|" + p.EntityId, TransmissionDistance.TransmissionDistanceMax);
            if (!visible && (owner == 0 || p.Status == MyShipConnectorStatus.Connected)) continue;
            // The connector face is separate from the block center.
            Vector3D face = p.GetPosition() + p.WorldMatrix.Forward * FaceOffset(p);
            string packet = "PORT|" + p.EntityId + "|" + p.CubeGrid.EntityId + "|" + sequence
                + "|" + Clean(Me.CubeGrid.CustomName) + "|" + Clean(p.CustomName)
                + "|" + Vec(face) + "|" + Vec(p.WorldMatrix.Forward) + "|" + Vec(p.WorldMatrix.Up)
                + "|" + (p.IsWorking && p.CubeGrid.IsStatic ? "1" : "0")
                + "|" + (int)p.Status + "|" + owner;
            if (visible) IGC.SendBroadcastMessage(NETWORK, packet, TransmissionDistance.TransmissionDistanceMax);
            else IGC.SendUnicastMessage(owner, NETWORK, packet);
        }
    }
    Echo("DOCKING STATION 2.0\nNetwork: " + NETWORK + "\nPorts: " + ports.Count);
    foreach (var p in ports)
        if (!p.Closed) Echo(p.CustomName + " : " + p.Status + (p.CubeGrid.IsStatic ? "" : " (mobile: broadcast only)"));
}
string Clean(string s) { return s.Replace('|', '/').Replace('\n', ' ').Replace('\r', ' '); }
string Vec(Vector3D v) { return Num(v.X) + ";" + Num(v.Y) + ";" + Num(v.Z); }
string Num(double n) { return n.ToString("R", System.Globalization.CultureInfo.InvariantCulture); }
double FaceOffset(IMyShipConnector p)
{
    // Front face of the block's occupied volume. Override for nonstandard models:
    // Connector Custom Data: DockFaceOffset=1.25 (meters from center, along Forward).
    foreach (string line in p.CustomData.Split('\n'))
    {
        double value;
        if (line.Trim().StartsWith("DockFaceOffset=", StringComparison.OrdinalIgnoreCase)
            && double.TryParse(line.Trim().Substring(15), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out value)
            && !double.IsNaN(value) && !double.IsInfinity(value) && value >= 0 && value <= 10) return value;
    }
    Vector3D size = (Vector3D)(p.Max - p.Min + Vector3I.One) * p.CubeGrid.GridSize;
    Vector3D f = Vector3D.TransformNormal(p.WorldMatrix.Forward, MatrixD.Transpose(p.CubeGrid.WorldMatrix));
    return (Math.Abs(f.X) * size.X + Math.Abs(f.Y) * size.Y + Math.Abs(f.Z) * size.Z) * 0.5;
}
