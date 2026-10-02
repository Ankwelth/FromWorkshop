/*
 * Wico Modular Communications Manager
 * 
 * Manages antenna ranges and connections via IGC with remote drones.
 */


WicoIGC _wicoIGC;
WicoBlockMaster _wicoBlockMaster;
WicoElapsedTime _wicoElapsedTime;


WicoControl _wicoControl;
Communications _communications;
ModeAttention _modeAttention;

IFF _iff;

Displays _displays;

void ModuleProgramInit()
{
    moduleList += "\nCommunications Manager";

    _wicoIGC = new WicoIGC(this);

    _wicoBlockMaster = new WicoBlockMaster(this);
    _wicoBlockMaster.LoadLocalGrid();

    _wicoControl = new WicoControl(this, _wicoIGC);
    _wicoElapsedTime = new WicoElapsedTime(this, _wicoControl);

    _iff = new IFF(this, _wicoIGC, _wicoElapsedTime);
    _displays = new Displays(this, _wicoBlockMaster, _wicoElapsedTime);
    _communications = new Communications(this, _wicoBlockMaster, _wicoElapsedTime, _wicoIGC, _displays);
    _modeAttention = new ModeAttention(this, _wicoControl, _communications);
}

public void ModulePreMain(string argument, UpdateType updateSource)
{
    if (_wicoControl != null)
        _wicoControl.AnnounceState();
}

public void ModulePostMain(UpdateType updateSource)
{
    if (bInitDone)
    {
        _displays.EchoInfo();
        _wicoControl.WantSlow();
    }

    Runtime.UpdateFrequency = _wicoControl.GenerateUpdate();
    Echo("LastRun=" + LastRunMs.ToString("0.00") + "ms Max=" + MaxRunMs.ToString("0.00") + "ms");
    EchoInstructions();
}

public void ModulePostInit()
{
    if (_wicoControl != null)
        _wicoControl.ModeAfterInit(SaveIni);

}

public class Antennas
{

    bool bGotAntennaName = false;
    public string AntennaName;

    protected List<IMyRadioAntenna> antennaList = new List<IMyRadioAntenna>();
    protected List<IMyLaserAntenna> antennaLList = new List<IMyLaserAntenna>();

    protected List<IMyBeacon> beaconList = new List<IMyBeacon>();

    protected Program _program;
    protected WicoBlockMaster _wicoBlockMaster;

    public Antennas(Program program, WicoBlockMaster wicoBlockMaster)
    {
        _program = program;
        _wicoBlockMaster = wicoBlockMaster;

        _wicoBlockMaster.AddLocalBlockHandler(BlockParseHandler);
        _wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);
    }

public void BlockParseHandler(IMyTerminalBlock tb)
    {
        if (tb is IMyRadioAntenna)
        {
            if (tb.CustomName.Contains("unused") || tb.CustomData.Contains("unused"))
                return;
            antennaList.Add(tb as IMyRadioAntenna);
            if (!bGotAntennaName)
            {
                AntennaName = "Wico " + tb.CustomName.Split('!')[0].Trim();
                bGotAntennaName = true;
            }
        }
        if (tb is IMyLaserAntenna)
        {
            antennaLList.Add(tb as IMyLaserAntenna);
        }
        if (tb is IMyBeacon)
        {
            beaconList.Add(tb as IMyBeacon);
        }
    }
    void LocalGridChangedHandler()
    {
        antennaList.Clear();
        antennaLList.Clear();
        bGotAntennaName = false;
        AntennaName = "";
    }

public void SetLowPower(bool bAll = false)
    {
        bool bFirst = true;
        foreach (var a in antennaList)
        {
            a.Radius = 200;
            if (bFirst || bAll)
            {
                bFirst = false;
                a.Enabled = true;
            }
        }
    }

public void SetRadius(float fRadius = 200, bool bAll = false)
    {
        bool bFirst = true;
        foreach (var a1 in antennaList)
        {
            if (bFirst || bAll)
            {
                bFirst = false;
                a1.Radius = fRadius;
                a1.Enabled = true;
            }
            if (!bAll) return;
        }
    }

public Vector3D GetPosition()
    {
        foreach (var a1 in antennaList)
        {
            return a1.GetPosition();
        }
        Vector3D vNone = new Vector3D();
        return vNone;
    }

protected float fAntennaDesiredRange = float.MaxValue;

public void SetMaxPower(bool bAll = false, float desiredRange = float.MaxValue)
    {
        if (desiredRange < 200) desiredRange = 200;
        fAntennaDesiredRange = desiredRange;

        SetDesiredPower(bAll);
    }

    public void SetDesiredPower(bool bAll = false)
    {
        bool bFirst = true;
        foreach (var a in antennaList)
        {
            if (bFirst || bAll)
            {
                bFirst = false;
                float maxPower = a.GetMaximum<float>("Radius");
                if (fAntennaDesiredRange < maxPower) maxPower = fAntennaDesiredRange;
                a.Radius = maxPower;
                a.Enabled = true;
            }
            if (!bAll) return;
        }
    }

public int AntennaCount()
    {
        return (antennaList.Count);
    }

    public void SetAnnouncement(string sMessage)
    {
        if(beaconList.Count>0)
        {
            IMyBeacon beacon = beaconList[0];
            beacon.Enabled = true;
            beacon.HudText = sMessage;
            return;
        }
        IMyRadioAntenna theAntenna = null;
        foreach(var antenna in antennaList)
        {
            if(theAntenna==null || (antenna.Enabled && antenna.Radius>theAntenna.Radius))
            {
                theAntenna = antenna;
                continue;
            }
        }
        if(theAntenna!=null)
        {
            theAntenna.Enabled = true;
            theAntenna.HudText = sMessage;
        }
    }

    public void ClearAnnouncement()
    {
        foreach(var beacon in beaconList)
        {
            if(beacon.Enabled)
            {
                beacon.Enabled = false;
                beacon.HudText = "";

            }
        }
        foreach(var antenna in antennaList)
        {
            antenna.HudText = "";
        }
    }

}


public class Communications : Antennas
{



    WicoElapsedTime _wicoElapsedTime;
    WicoIGC _wicoIGC;
    Displays _displays;

    const string CommunicationsDisplayTag = "COMMUNICATIONS";
    const string RemoteShipsDisplayTag = "REMOTESHIPS";
    const string RelayShipsDisplayTag = "RELAYSHIPS";

    public string sRemoteAnnounce = "COM_IAMHERE";
    public string sComIGCTag = "COM?";
    public string sLaserConRequestIGCTag = "LASERCON?";
    public string sLaserIGCTag = "LASERCON";
    public string sRelayRequestTag = "RELAY?";

    double AnnounceSeconds = 10;
    const string COMIFFTIMER = "IGCIFFTIMER";
    long _EntityId;

double AreaOfControl = 500000;

    enum GridRole { unknown, SilentShip, RadioOnlyShip, LaserOnlyShip, LaserRadioShip, MultipleLaserShip, MultipleLaserRadioShip, SilentBase, RadioOnlyBase, LaserOnlyBase, LaserRadioBase };

    bool bDefaultsSet = false;
bool bAreaController = true;
bool bRelay = true;

    GridRole _gridRole;

    public Communications(Program program, WicoBlockMaster wicoBlockMaster, WicoElapsedTime wicoElapsedTime, WicoIGC wicoIGC, Displays displays
        ): base(program,wicoBlockMaster)
    {
        _program = program;
        _wicoBlockMaster = wicoBlockMaster;
        _wicoElapsedTime = wicoElapsedTime;
        _wicoIGC = wicoIGC;
        _displays = displays;

        _EntityId = program.Me.CubeGrid.EntityId;

        _program.moduleName += " Communications";
        _program.moduleList += "\nCommunications V4.2n";

        _program.AddUpdateHandler(UpdateHandler);
        _program.AddTriggerHandler(ProcessTrigger);

        _program.AddLoadHandler(LoadHandler);
        _program.AddSaveHandler(SaveHandler);

        _wicoBlockMaster.AddLocalBlockHandler(BlockParseHandler);
        _wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);

        _program.AddPostInitHandler(PostInitHandler());

        _program.AddMainHandler(MainHandler);

        _wicoIGC.AddPublicHandler(sRemoteAnnounce, BroadcastHandler);
        _wicoIGC.AddPublicHandler(sComIGCTag, BroadcastHandler);
        _wicoIGC.AddPublicHandler(sRelayRequestTag, BroadcastHandler);

        _wicoIGC.AddUnicastHandler( BroadcastHandler);

        _displays.AddSurfaceHandler(CommunicationsDisplayTag, SurfaceHandler);
        _displays.AddSurfaceHandler(RemoteShipsDisplayTag, SurfaceHandler);
        _displays.AddSurfaceHandler(RelayShipsDisplayTag, SurfaceHandler);

        AnnounceSeconds = _program.CustomDataIni.Get(_program.OurName, "WicoComAnnounceSeconds").ToDouble(AnnounceSeconds);
        _program.CustomDataIni.Set(_program.OurName, "WicoComAnnounceSeconds", AnnounceSeconds);

        if(_program.CustomDataIni.ContainsKey(_program.OurName, "AreaController"))
        {
            bAreaController = _program.CustomDataIni.Get(_program.OurName, "AreaController").ToBoolean(bAreaController);
            bRelay = _program.CustomDataIni.Get(_program.OurName, "Relay").ToBoolean(bRelay);
            bDefaultsSet = true;
        }

        if (AnnounceSeconds > 0)
        {
            wicoElapsedTime.AddTimer(COMIFFTIMER, AnnounceSeconds, ElapsedTimehandler);
            wicoElapsedTime.StartTimer(COMIFFTIMER);
        }

    }
    public void ElapsedTimehandler(string s)
    {
        if (s == COMIFFTIMER)
        {
            Announce();
        }
    }
    StringBuilder sbMessages=new StringBuilder(200);
    public void Announce(long targetID=0)
    {
        IMyShipController ship = _wicoBlockMaster.GetMainController();
        IMyTerminalBlock tb = ship;
        if (tb == null)
            tb = _program.Me;

        sbMessages.Clear();
        sbMessages.AppendLine(_program.Me.CubeGrid.EntityId.ToString());
        sbMessages.AppendLine(_wicoBlockMaster.GetShipName().ToString());
        sbMessages.AppendLine(_program.Vector3DToString(tb.GetPosition()));
        sbMessages.AppendLine(_program.Vector3DToString(_wicoBlockMaster.GetShipVelocity()));
        sbMessages.AppendLine(((int)_gridRole).ToString());
        sbMessages.AppendLine(bAreaController.ToString());
        sbMessages.AppendLine(bRelay.ToString());

        sbMessages.AppendLine(antennaLList.Count.ToString());
        foreach(var laser in antennaLList)
        {
            sbMessages.AppendLine(laser.CustomName);
            sbMessages.AppendLine(_program.Vector3DToString(laser.GetPosition()));
        }

        string message;
        message = sbMessages.ToString();
        if(targetID>0)
        {
            _program.IGC.SendUnicastMessage(targetID, sRemoteAnnounce, message);
        }
        else _program.IGC.SendBroadcastMessage(sRemoteAnnounce, message);
    }

    int numberLocalThrust = 0;
new void BlockParseHandler(IMyTerminalBlock tb)
    {
        if (tb is IMyThrust)
        {
            numberLocalThrust++;
        }
    }
    void LocalGridChangedHandler()
    {
        numberLocalThrust = 0;
    }

    StringBuilder sbComNotices = new StringBuilder(200);
    StringBuilder sbComInfo = new StringBuilder(200);
    StringBuilder sbRSNotices = new StringBuilder(200);
    StringBuilder sbRSInfo = new StringBuilder(200);

    public void SurfaceHandler(string tag, IMyTextSurface tsurface, int ActionType)
    {
        if (tag == CommunicationsDisplayTag)
        {
            if (ActionType == Displays.DODRAW)
            {
                if (tsurface.SurfaceSize.Y < 256)
                {
                    tsurface.WriteText(sbComInfo);
                }
                else
                {
                    tsurface.WriteText(sbComInfo);
                    tsurface.WriteText(sbComNotices, true);
                }
            }
            else if (ActionType == Displays.SETUPDRAW)
            {
                tsurface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                tsurface.WriteText("");
                if (tsurface.SurfaceSize.Y < 256)
                {
                    tsurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
                    tsurface.FontSize = 3;
                }
                else
                {
                    tsurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
                    tsurface.FontSize = 2f;
                }
            }
            else if (ActionType == Displays.CLEARDISPLAY)
            {
                tsurface.WriteText("");
            }
        }
        else if (tag == RemoteShipsDisplayTag)
        {
            if (ActionType == Displays.DODRAW)
            {
                if (tsurface.SurfaceSize.Y < 256)
                {
                    tsurface.WriteText(sbRSInfo);
                }
                else
                {
                    tsurface.WriteText(sbRSInfo);
                    tsurface.WriteText(sbRSNotices, true);
                }
            }
            else if (ActionType == Displays.SETUPDRAW)
            {
                tsurface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                if (tsurface.SurfaceSize.Y < 256)
                {
                    tsurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
                    tsurface.FontSize = 3.5f;
                }
                else
                {
                    tsurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
                    tsurface.FontSize = 1.75f;
                }
            }
            else if (ActionType == Displays.CLEARDISPLAY)
            {
                tsurface.WriteText("");
            }
        }
    }

    void LoadHandler(MyIni Ini)
    {
    }

    void SaveHandler(MyIni Ini)
    {
    }
    public IEnumerator<bool> PostInitHandler()
    {
        if(numberLocalThrust>0)
        {
            if(antennaLList.Count<1)
            {
                if (antennaList.Count < 1)
                {
                    _gridRole = GridRole.SilentShip;
                    if(!bDefaultsSet)
                    {
                        bRelay = false;
                        bAreaController = false;
                    }
                }
                else _gridRole = GridRole.RadioOnlyShip;
            }
            else
            {
                if(antennaLList.Count >1)
                {
                    if (antennaList.Count < 1) _gridRole = GridRole.MultipleLaserShip;
                    else _gridRole = GridRole.MultipleLaserRadioShip;
                }
                else if(antennaLList.Count>0)
                {
                    if (antennaList.Count < 1) _gridRole = GridRole.LaserOnlyShip;
                    else _gridRole = GridRole.LaserRadioShip;
                }
            }

        }
        else
        {
            if (antennaLList.Count < 1)
            {
                if (antennaList.Count < 1)
                {
                    _gridRole = GridRole.SilentBase;
                    if (!bDefaultsSet)
                    {
                        bRelay = false;
                        bAreaController = false;
                    }
                }
                else _gridRole = GridRole.RadioOnlyBase;
            }
            else
            {
                if (antennaLList.Count > 1)
                {
                    if (antennaList.Count < 1)
                    {
                        _gridRole = GridRole.LaserOnlyBase;
                        if (!bDefaultsSet)
                        {
                            bAreaController = false;
                        }
                    }
                    else _gridRole = GridRole.LaserRadioBase;
                }
                else if (antennaLList.Count > 0)
                {
                    if (antennaList.Count < 1)
                    {
                        if (!bDefaultsSet)
                        {
                            bRelay = false;
                            bAreaController = false;
                        }
                        _gridRole = GridRole.LaserOnlyBase;
                    }
                    else
                    {
                        _gridRole = GridRole.LaserRadioBase;
                    }

                }
            }
        }
        if (!bDefaultsSet)
        {
            _program.CustomDataIni.Set(_program.OurName, "AreaController", bAreaController);
            _program.CustomDataIni.Set(_program.OurName, "Relay", bRelay);
            bDefaultsSet = true;
        }
        _program.CustomDataChanged();
        Announce();

        _program.IGC.SendBroadcastMessage(sRelayRequestTag, "");

        yield return true;
    }

public void ProcessTrigger(string sArgument, MyCommandLine myCommandLine, UpdateType updateSource)
    {
        string[] varArgs = sArgument.Trim().Split(';');

        for (int iArg = 0; iArg < varArgs.Length; iArg++)
        {
            string[] args = varArgs[iArg].Trim().Split(' ');

        }
        if (myCommandLine != null)
        {
            if (myCommandLine.Argument(0) == "godock")
            {
            }
        }
    }

    void UpdateHandler(UpdateType updateSource)
    {

        if ((updateSource & UpdateType.Update100) > 0)
        {

            sbRSInfo.Clear();
            sbRSNotices.Clear();

            float farthestShip = 0;
            float nearestAreaController = float.MaxValue;
            string interestingName = "";
            bool bAreaControllerFound = false;



            foreach (var remoteship in RemoteShips)
            {


                if (remoteship.ageMs < 10 * 1000)
                {
                    Vector3D shipPosition = remoteship.Position;
                    double estimatedrange = 0;
                    if(!remoteship.Velocity.IsZero())
                    {
                        Vector3D estimatedShipPosition=shipPosition + remoteship.Velocity * 2*AnnounceSeconds;
                        estimatedrange = (_wicoBlockMaster.CenterOfMass() - estimatedShipPosition).Length();
                    }
                    double range = (_wicoBlockMaster.CenterOfMass() - shipPosition).Length();
                    if (estimatedrange > range)
                        range = estimatedrange;
                    if (range > float.MaxValue) continue;

                    if(bAreaController)
                    {
                        if (range > farthestShip && range <= AreaOfControl)
                        {
                            farthestShip = (float)range;
                            interestingName = remoteship.sName;
                        }
                    }
                    else
                    {
                        if (remoteship.IsAreaController)
                        {
                            bAreaControllerFound = true;
                            if (range < nearestAreaController)
                            {
                                nearestAreaController = (float)range;
                                interestingName = remoteship.sName;
                            }
                        }
                    }
                }
            }
            if(RemoteShips.Count>0)
            {
                if (bAreaController)
                {
                    fAntennaDesiredRange = farthestShip + 420;
                    if (fAntennaDesiredRange > nearestAreaController)
                        fAntennaDesiredRange = nearestAreaController + 1000;
                }
                else
                {
                    fAntennaDesiredRange = nearestAreaController + 200 + (float)(_wicoBlockMaster.GetShipSpeed()* AnnounceSeconds);
                }

            }
            if(!bAreaController)
            {

                if(!bAreaControllerFound)
                {
                    sbRSInfo.AppendLine("No local area controller");
                }
                else
                {
                    sbRSInfo.AppendLine("Closest Area Controller");
                    sbRSInfo.AppendLine(" "+ interestingName);
                    sbRSInfo.AppendLine(" " + _program.niceDoubleMeters(nearestAreaController));
                }
            }
            else
            {
                if(RemoteShips.Count>0)
                {
                    sbRSInfo.AppendLine("Farthest Ship");
                    sbRSInfo.AppendLine(" "+ interestingName);
                    sbRSInfo.AppendLine(" " + _program.niceDoubleMeters(farthestShip));
                }
            }
            sbRSNotices.AppendLine("");

            foreach (var remoteship in RemoteShips)
            {
                string sAge = " ";
                if (remoteship.ageMs > 2 * 1000)
                {
                    if (remoteship.ageMs > 10 * 1000)
                        sAge = "!";
                    else sAge = "*";
                }
                if (remoteship.IsAreaController)
                    sAge += "A";
                else sAge += " ";
                if (remoteship.IsRelay)
                    sAge += "R";
                else sAge += " ";
                sbRSNotices.AppendLine(sAge + remoteship.sName);
                sbRSNotices.AppendLine(" " + _program.niceDoubleMeters((_wicoBlockMaster.CenterOfMass() - remoteship.Position).Length()));
            }

            foreach (var laser in antennaLList)
            {
                if (laser.Status == MyLaserAntennaStatus.Idle)
                {
                    laser.Enabled = false;
                }

            }
            IMyRadioAntenna bestAntenna = null;
            foreach (var ant in antennaList)
            {
                if (bestAntenna == null)
                {
                    if(ant.IsFunctional)
                        bestAntenna = ant;
                }
                else
                {
                    if (ant.IsFunctional)
                    {
                        if(ant.Radius>bestAntenna.Radius)
                        {
                            bestAntenna = ant;
                        }
                    }
                }
            }
            if(bestAntenna!=null)
            {

                if(RemoteShips.Count > 0)
                {
                    bestAntenna.Radius = fAntennaDesiredRange;
                }
                foreach (var ant in antennaList)
                {
                    if (ant != bestAntenna)
                    {
                        if(ant.Enabled)
                            ant.Enabled = false;
                    }
                    else
                    {
                        if (bestAntenna.Enabled != true)
                            bestAntenna.Enabled = true;
                        sbComInfo.AppendLine("Best Ant=" + bestAntenna.CustomName);
                    }
                }
            }

            sbComInfo.Clear();
            sbComNotices.Clear();
            sbComInfo.AppendLine("Communications");
            if (bAreaController) sbComNotices.AppendLine("I am area controller");
            if (bRelay) sbComNotices.AppendLine("I am a relay");

            sbComNotices.AppendLine(antennaList.Count + " Radio Antennas");
            sbComNotices.AppendLine(antennaLList.Count + " Laser Antennas");
            foreach (var radio in antennaList)
            {
                sbComInfo.AppendLine(radio.CustomName + " (" + radio.Radius.ToString("N0") + "M)");
            }
            foreach (var laser in antennaLList)
            {
                sbComInfo.AppendLine(laser.CustomName + " (" + laser.Range.ToString("N0") + "M)");
            }

        }
    }

    void BroadcastHandler(MyIGCMessage msg)
    {

        if (msg.Tag == sComIGCTag)
        {
            _program.Echo("Com Request");
            string sMessage = (string)msg.Data;
            string[] aMessage = sMessage.Trim().Split('\n');
            long incomingID = 0;
            bool pOK = false;
            pOK = long.TryParse(aMessage[0], out incomingID);
        }
        else if (msg.Tag == sRelayRequestTag)
        {
            _program.Echo("Relay Request");
            if (bRelay)
                Announce(msg.Source);
        }
        else if (msg.Tag==sRemoteAnnounce)
        {
            string sMessage = (string)msg.Data;
            string[] aMessage = sMessage.Trim().Split('\n');

            long Shipid;
            string ShipName;
            Vector3D ShipPosition;
            Vector3D Velocity;
            GridRole role;
            int LaserAntennaCount;

            int iTemp;

            int iLine = 0;

            long.TryParse(aMessage[iLine++], out Shipid);
            ShipName = aMessage[iLine++];

            ShipPosition = _program.StringToVector3D(aMessage[iLine++]);

            Velocity= _program.StringToVector3D(aMessage[iLine++]);

            int.TryParse(aMessage[iLine++], out iTemp);
            role = (GridRole)iTemp;

            bool IsAreaController;
            bool.TryParse(aMessage[iLine++], out IsAreaController);
            bool IsRelay;
            bool.TryParse(aMessage[iLine++], out IsRelay);


            int.TryParse(aMessage[iLine++], out LaserAntennaCount);

            List<RemoteLaser> remoteLasers = new List<RemoteLaser>();

            for(int i=0;i<LaserAntennaCount;i++)
            {
                string LaserName = aMessage[iLine++];
                Vector3D LaserPosition = _program.StringToVector3D(aMessage[iLine++]);
                RemoteLaser remote = new RemoteLaser();
                remote.Name = LaserName;
                remote.Position = LaserPosition;
                remoteLasers.Add(remote);
            }

            bool bFound = false;
            foreach(var remoteShip in RemoteShips)
            {
                if(remoteShip.ShipID == Shipid)
                {
                    remoteShip.sName = ShipName;
                    remoteShip.Position = ShipPosition;
                    remoteShip.Velocity = Velocity;
                    remoteShip.role = role;
                    remoteShip.IsAreaController = IsAreaController;
                    remoteShip.IsRelay = IsRelay;
                    remoteShip.NumberLasers = LaserAntennaCount;
                    remoteShip.RemoteLasers = remoteLasers;
                    remoteShip.ageMs = 0;
                    bFound = true;
                    break;
                }

            }
            if(!bFound)
            {
                RemoteShip remoteShip = new RemoteShip();
                remoteShip.ShipID = Shipid;
                remoteShip.sName = ShipName;
                remoteShip.Position = ShipPosition;
                remoteShip.Velocity = Velocity;
                remoteShip.role = role;
                remoteShip.IsAreaController = IsAreaController;
                remoteShip.IsRelay = IsRelay;
                remoteShip.NumberLasers = LaserAntennaCount;
                remoteShip.RemoteLasers = remoteLasers;
                remoteShip.ageMs = 0;
                RemoteShips.Add(remoteShip);
            }
        }
    }

    void MainHandler(UpdateType updateSource)
    {
        foreach(var remoteship in RemoteShips)
        {
            remoteship.ageMs += _program.Runtime.TimeSinceLastRun.TotalMilliseconds;
        }
    }

    List<RemoteLaser> RemoteLasers = new List<RemoteLaser>();
    List<RemoteShip> RemoteShips = new List<RemoteShip>();

    class RemoteLaser
    {
        public string Name;
        public Vector3D Position;
    }

    class RemoteShip
    {
        public long ShipID;
        public string sName;
        public double ageMs;
        public Vector3D Position;
        public Vector3D Velocity;
        public GridRole role;
        public bool IsAreaController;
        public bool IsRelay;
        public int NumberLasers;
        public List<RemoteLaser> RemoteLasers;
    }

}

public class Displays
{

    const string DisplayCheckTimer = "DisplayCheck";
    const double DisplayInterval = 0.5;

    public const int CLEARDISPLAY = 0;
    public const int DODRAW= 1;
    public const int SETUPDRAW = 99;

    bool _Debug = false;

    List<IMyTerminalBlock> _SurfaceProviders = new List<IMyTerminalBlock>();
    List<WicoDisplay> _wicoDisplays = new List<WicoDisplay>();

    public class WicoDisplay
    {
        public string tag;
        List<IMyTextSurface> _surfaces;
        List<Action<string,IMyTextSurface, int>> SurfaceDrawHandlers = new List<Action<string,IMyTextSurface, int>>();

        public WicoDisplay(List<IMyTerminalBlock> lsp, string Tag)
        {
            tag = Tag;
            _surfaces = new List<IMyTextSurface>();
            SurfaceDrawHandlers = new List<Action<string,IMyTextSurface, int>>();
        }

        public void ResetSurfaces()
        {
            _surfaces.Clear();
        }

        public void OfferSurface(IMyTerminalBlock tb)
        {
            if(tb.CustomName.Contains(tag))
            {
                var tsp = tb as IMyTextSurfaceProvider;
                var x=tsp.SurfaceCount;
                var tsurface = tsp.GetSurface(0);
                if(tsurface!=null)
                    _surfaces.Add(tsurface);
            }
        }

        public void OfferHandler(Action<string,IMyTextSurface, int> handler)
        {
            if (!SurfaceDrawHandlers.Contains(handler))
                SurfaceDrawHandlers.Add(handler);
        }

        public void CallHandlers(int ActionType)
        {
            foreach(var handler in SurfaceDrawHandlers)
            {
                foreach(var surface in _surfaces)
                {
                    handler(tag,surface, ActionType);
                }
            }
        }
    }

    Program _program;
    WicoBlockMaster _wicoBlockMaster;
    WicoElapsedTime _wicoElapsedTime;

    readonly string WicoDisplaySection = "WicoDisplay";

    public Displays(Program program, WicoBlockMaster wicoBlockMaster, WicoElapsedTime wicoElapsedTime)
    {
        _program = program;
        _wicoBlockMaster = wicoBlockMaster;
        _wicoElapsedTime = wicoElapsedTime;

        _wicoBlockMaster.AddLocalBlockHandler(BlockParseHandler);
        _wicoBlockMaster.AddLocalBlockChangedHandler(LocalGridChangedHandler);

        _program.AddPostInitHandler(PostInitHandler());

        _Debug = _program.CustomDataIni.Get(WicoDisplaySection, "Debug").ToBoolean(_Debug);
        _program.CustomDataIni.Set(WicoDisplaySection, "Debug", _Debug);

        _wicoElapsedTime.AddTimer(DisplayCheckTimer, DisplayInterval, ElapsedTimerHandler);
        _wicoElapsedTime.StartTimer(DisplayCheckTimer);
    }

    public IEnumerator<bool> PostInitHandler()
    {
        foreach(var surface in _wicoDisplays)
        {
            foreach(var tb in _SurfaceProviders)
            {
                surface.OfferSurface(tb);
            }
        }
        foreach(var surface in _wicoDisplays)
        {
            surface.CallHandlers(SETUPDRAW);
        }
        yield return false;
    }

void BlockParseHandler(IMyTerminalBlock tb)
    {
        if (tb is IMyTextPanel)
        {
            _SurfaceProviders.Add(tb);
        }
    }

    void LocalGridChangedHandler()
    {
        _SurfaceProviders.Clear();
        foreach (var display in _wicoDisplays)
        {
            display.ResetSurfaces();
        }
    }

void ElapsedTimerHandler(string timerName)
    {
        foreach(var display in _wicoDisplays)
        {
            if(_Debug) _program.Echo("Display:" + display.tag);
            display.CallHandlers(DODRAW);
        }
    }

public bool AddSurfaceHandler(string tag, Action<string,IMyTextSurface, int> handler)
    {
        if (handler == null)
            _program.Echo("handler is NULL!");

        bool bFound = false;
        WicoDisplay FoundDisplay=null;
        foreach(var display in _wicoDisplays)
        {
            if(display.tag==tag)
            {
                FoundDisplay = display;
                bFound = true;
                break;
            }
        }
        if (!bFound)
        {
            FoundDisplay = new WicoDisplay(_SurfaceProviders, tag);
            FoundDisplay.OfferHandler(handler);
            _wicoDisplays.Add(FoundDisplay);
        }
        else
        {
            FoundDisplay.OfferHandler(handler);
        }

        return true;
    }

    public void ClearDisplays(string tag)
    {
        foreach (var display in _wicoDisplays)
        {
            if (display.tag == tag)
            {
                display.CallHandlers(CLEARDISPLAY);
            }
        }
    }

    public void EchoInfo()
    {
        _program.Echo("Displays:");
        _program.Echo(" " +_wicoDisplays.Count + " DisplayTypes");
        foreach(var display in _wicoDisplays)
        {
            _program.Echo(" " + display.tag);
        }
        _program.Echo(" " + _SurfaceProviders.Count + " Surface Providers");
    }

}

public class WicoElapsedTime
{
    readonly Program _program;
    readonly WicoUpdates _wicoUpdates;

    bool _bDebug = false;

    string wicoETString = "WicoET";

    public WicoElapsedTime(Program program, WicoUpdates wicoUpdates)
    {
        _program = program;
        _wicoUpdates = wicoUpdates;

        _program.AddMainHandler(CheckTimers);

        _bDebug = _program.CustomDataIni.Get(wicoETString, "Debug").ToBoolean(_bDebug);
        _program.CustomDataIni.Set(wicoETString, "Debug", _bDebug);
    }

    List<ElapsedTimers> TimerList = new List<ElapsedTimers>();

    class ElapsedTimers
    {
        public string sName;
        public double dWaitSeconds;
        public double dElapsedSeconds;
        public bool bActive;
        public bool AutoRestart;
        public Action<string> handler;
    }

    public bool AddTimer(string sName, double dDefaultWaitSeconds = 1, Action<string> handler = null, bool AutoRestart = true)
    {
        ElapsedTimers et = new ElapsedTimers
        {
            sName = sName
            , dWaitSeconds = dDefaultWaitSeconds
            , dElapsedSeconds = -1
            , bActive = false
            , AutoRestart = AutoRestart
            , handler = handler
        };

        foreach (var et1 in TimerList)
        {
            if(et1.sName==sName)
            {
                et1.dWaitSeconds = dDefaultWaitSeconds;
                et1.dElapsedSeconds = -1;
                et1.AutoRestart = AutoRestart;
                et1.handler = handler;
                return false;
            }
        }
        if (bCheckingTimers)
            _program.Echo("ERROR: Adding while checking");
        TimerList.Add(et);

        return true;
    }

    public bool StartTimer(string sName)
    {

        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                et.bActive = true;
                return true;
            }
        }
        return false;
    }

    public bool StopTimer(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                et.bActive = false;
                return true;
            }
        }
        return false;
    }
public bool ResetTimer(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                et.dElapsedSeconds =-1;
                et.bActive = false;
                return true;
            }
        }
        return false;

    }

public bool RestartTimer(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                et.dElapsedSeconds = 0;
                et.bActive = true;
                return true;
            }
        }
        return false;

    }

public bool IsExpired(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                if (!et.bActive) return false;
                if (et.dElapsedSeconds < 0) return true;
                if (et.dElapsedSeconds > et.dWaitSeconds) return true;
                else return false;
            }
        }
        return true;
    }
public bool IsInActiveOrExpired(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                if (!et.bActive) return true;
                if (et.dElapsedSeconds < 0) return true;
                if (et.dElapsedSeconds > et.dWaitSeconds) return true;
                else return false;
            }
        }
        return true;
    }

    public bool IsActive(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                if (et.bActive) return true;
            }
        }
        return false;

    }
    public bool GetTime(string sName, out double Elapsed, out double Wait)
    {
        Elapsed = -1;
        Wait = -1;
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                Elapsed = et.dElapsedSeconds;
                Wait = et.dWaitSeconds;
                return true;
            }
        }
        return false;
    }
    public double GetElapsed(string sName)
    {
        foreach (var et in TimerList)
        {
            if (et.sName == sName)
            {
                return et.dElapsedSeconds;
            }
        }
        return -1;
    }

    bool bCheckingTimers = false;
public void CheckTimers(UpdateType updateSource)
    {
        bCheckingTimers = true;
        foreach (var et in TimerList)
        {
            if (_bDebug) _program.Echo("Timer:" + et.sName + " Active="+ et.bActive+" " + et.dElapsedSeconds.ToString("0.00") + "/" + et.dWaitSeconds.ToString("0.00"));
            if (et.bActive)
            {
                if (et.dElapsedSeconds >= 0)
                {
                    et.dElapsedSeconds += _program.Runtime.TimeSinceLastRun.TotalMilliseconds/1000;
                }
                if (et.dElapsedSeconds > et.dWaitSeconds)
                {
                    if (et.handler != null)
                    {
                        et.handler(et.sName);
                        if (et.AutoRestart)
                            et.dElapsedSeconds = 0;
                    }
                }
                if(et.dElapsedSeconds<0)
                {
                    if (et.AutoRestart)
                        et.dElapsedSeconds = 0;
                }
                _wicoUpdates.WantSlow();
            }
        }
        bCheckingTimers = false;
    }

    public void SetDebug(bool bDebug=false)
    {
        _bDebug = bDebug;
    }
}

public class WicoIGC
{
    IMyUnicastListener _unicastListener;

List<Action<MyIGCMessage>> _unicastMessageHandlers = new List<Action<MyIGCMessage>>();

List<Action<MyIGCMessage>> _broadcastMessageHandlers = new List<Action<MyIGCMessage>>();
List<IMyBroadcastListener> _broadcastChannels = new List<IMyBroadcastListener>();

    Program _program;
    bool _debug = false;
    IMyTextPanel _debugTextPanel;

    string WicoIGCSection = "WicoIGC";
public WicoIGC(Program myProgram)
    {
        _program = myProgram;

        _program.AddMainHandler(ProcessIGCMessages);

        _debug = _program.CustomDataIni.Get(WicoIGCSection, "Debug").ToBoolean(_debug);
        _program.CustomDataIni.Set(WicoIGCSection, "Debug", _debug);

        _debugTextPanel = _program.GridTerminalSystem.GetBlockWithName("IGC Report") as IMyTextPanel;
        if (_debug) _debugTextPanel?.WriteText("");
    }

public bool AddPublicHandler(string channelTag, Action<MyIGCMessage> handler, bool setCallback = true)
    {
        IMyBroadcastListener publicChannel;
        publicChannel = _program.IGC.RegisterBroadcastListener(channelTag);
        if (setCallback) publicChannel.SetMessageCallback(channelTag);

        if(!_broadcastMessageHandlers.Contains(handler))
            _broadcastMessageHandlers.Add(handler);

        if(!_broadcastChannels.Contains(publicChannel))
           _broadcastChannels.Add(publicChannel);

        return true;
    }

public bool AddUnicastHandler(Action<MyIGCMessage> handler)
    {
        _unicastListener = _program.IGC.UnicastListener;
        _unicastListener.SetMessageCallback("UNICAST");
        if(!_unicastMessageHandlers.Contains(handler))
            _unicastMessageHandlers.Add(handler);
        return true;

    }
public void ProcessIGCMessages(UpdateType updateSource)
    {
        bool bFoundMessages = false;
        if (_debug) _program.Echo(_broadcastChannels.Count.ToString() + " broadcast channels");
        if (_debug) _program.Echo(_broadcastMessageHandlers.Count.ToString() + " broadcast message handlers");
        if (_debug) _program.Echo(_unicastMessageHandlers.Count.ToString() + " unicast message handlers");


        do
        {
            bFoundMessages = false;
            foreach (var channel in _broadcastChannels)
            {
                if (channel.HasPendingMessage)
                {
                    bFoundMessages = true;
                    var msg = channel.AcceptMessage();
                    if (_debug)
                    {
                        _program.Echo("Broadcast received. TAG:" + msg.Tag);
                        _debugTextPanel?.WriteText("IGC:" +msg.Tag+" SRC:"+msg.Source.ToString("X")+"\n",true);
                    }
                    foreach (var handler in _broadcastMessageHandlers)
                    {
                        if (_debug) _program.Echo("Calling handler");
                        handler(msg);
                    }
                    if (_debug) _program.Echo("Broadcast Handlers completed");
                }
            }
        } while (bFoundMessages);

        if (_unicastListener != null)
        {
            if (_debug) _program.Echo("Unicast check");

            do
            {
                bFoundMessages = false;

                if (_unicastListener.HasPendingMessage)
                {
                    bFoundMessages = true;
                    var msg = _unicastListener.AcceptMessage();
                    if (_debug) _program.Echo("Unicast received. TAG:" + msg.Tag);
                    foreach (var handler in _unicastMessageHandlers)
                    {
                        if (_debug) _program.Echo(" Unicast Handler");
                        handler(msg);
                    }
                    if (_debug) _program.Echo("Broadcast Handlers completed");
                }
            } while (bFoundMessages);
            if (_debug) _program.Echo("Unicast check completed");
        }

    }

public void SetDebug(bool debug)
    {
        _debug = debug;
        if (_debug) _debugTextPanel?.WriteText("");
    }
}

public class IFF
{
    const string IGCIFFMessage = "IGC_IFF_MSG";

    const string IGCIFFTimer = "IGCIFFTIMER";

    readonly WicoIGC _wicoIGC;
    WicoElapsedTime _wicoElapsedTime;
    long _EntityId;
    Program _program;

    double AnnounceSeconds = 1;

    public IFF(Program program, WicoIGC wicoIGC, WicoElapsedTime wicoElapsedTime)
    {

        _wicoIGC = wicoIGC;
        _wicoElapsedTime = wicoElapsedTime;
        _EntityId = program.Me.CubeGrid.EntityId;
        _program = program;

        AnnounceSeconds = _program.CustomDataIni.Get(_program.OurName, "IFFAnnounceSeconds").ToDouble(AnnounceSeconds);
        _program.CustomDataIni.Set(_program.OurName, "IFFAnnounceSeconds", AnnounceSeconds);

        if (AnnounceSeconds > 0)
        {
            wicoElapsedTime.AddTimer(IGCIFFTimer, AnnounceSeconds, ElapsedTimehandler);
            wicoElapsedTime.StartTimer(IGCIFFTimer);
        }
    }
    public void ElapsedTimehandler(string s)
    {
        if (s == IGCIFFTimer)
        {
            Announce();
        }
    }
    public void AnnounceEnemy(long EntityID, Vector3D position, Double radius)
    {
        MyTuple<byte, long, Vector3D, double> msg;
        msg.Item1 = 1;
        msg.Item2 = EntityID;
        msg.Item3 = position;
        msg.Item4 = radius;
        _program.IGC.SendBroadcastMessage(IGCIFFMessage, msg);
    }
    public void Announce()
    {
        MyTuple<byte, long, Vector3D, double> msg;
        msg.Item1 = 2;
        msg.Item2 = _EntityId;
        msg.Item3 = _program.Me.CubeGrid.GetPosition();
        msg.Item4 = _program.Me.CubeGrid.WorldVolume.Radius;
        _program.IGC.SendBroadcastMessage(IGCIFFMessage, msg);
    }
}

public class ModeAttention
{
    private Program _program;
    private WicoControl _wicoControl;
    private Antennas _antennas;

    public ModeAttention(Program program, WicoControl wc
        , Antennas antennas
        )
    {
        _program = program;
        _wicoControl = wc;
        _antennas = antennas;

        _program.moduleName += " Att";
        _program.moduleList += "\nAttention V4.2k";

        _program.AddUpdateHandler(UpdateHandler);
        _program.AddTriggerHandler(ProcessTrigger);

        _wicoControl.AddModeInitHandler(ModeInitHandler);
        _wicoControl.AddControlChangeHandler(ModeChangeHandler);
    }
public void ModeChangeHandler(int fromMode, int fromState, int toMode, int toState)
    {
        if (
            fromMode == WicoControl.MODE_MINE
            || fromMode == WicoControl.MODE_BORESINGLE
            )
        {
            _antennas.ClearAnnouncement();
        }
        if (
            toMode == WicoControl.MODE_ATTENTION
            )
        {
            _wicoControl.WantOnce();
        }
    }
void ModeInitHandler()
    {
        int iMode = _wicoControl.IMode;
        int iState = _wicoControl.IState;

        if (
            iState == WicoControl.MODE_ATTENTION
            )
        {
            _wicoControl.WantFast();
        }
    }
public void ProcessTrigger(string sArgument, MyCommandLine myCommandLine, UpdateType updateSource)
    {
        string[] varArgs = sArgument.Trim().Split(';');

        for (int iArg = 0; iArg < varArgs.Length; iArg++)
        {
            string[] args = varArgs[iArg].Trim().Split(' ');

        }
        if (myCommandLine != null)
        {
        }
    }

    void UpdateHandler(UpdateType updateSource)
    {
        int iMode = _wicoControl.IMode;
        int iState = _wicoControl.IState;

        if (iMode == WicoControl.MODE_ATTENTION) { doModeAttention(); return; }
    }

    void doModeAttention()
    {
        _program.Echo("Mode Attention!");
        int iMode = _wicoControl.IMode;
        int iState = _wicoControl.IState;
        switch (iState)
        {
            case 0:
                _wicoControl.SetState(10);
                break;
        }
    }
}



private List<Action<string,MyCommandLine, UpdateType>> UpdateTriggerHandlers = new List<Action<string,MyCommandLine, UpdateType>>();
private List<Action<UpdateType>> UpdateUpdateHandlers = new List<Action<UpdateType>>();

private MyCommandLine myCommandLine = new MyCommandLine();

private List<Action<MyIni>> SaveHandlers = new List<Action<MyIni>>();
private List<Action<MyIni>> LoadHandlers = new List<Action<MyIni>>();

private List<Action<bool>> ResetMotionHandlers = new List<Action<bool>>();

private List<IEnumerator<bool>> PostInitHandlers = new List<IEnumerator<bool>>();

private List<Action<UpdateType>> MainHandlers = new List<Action<UpdateType>>();

private MyIni SaveIni = new MyIni();
private MyIni CustomDataIni = new MyIni();

readonly UpdateType utTriggers = UpdateType.Terminal | UpdateType.Trigger | UpdateType.Mod | UpdateType.Script;
readonly UpdateType utUpdates = UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100 | UpdateType.Once;


bool bUsePBSurfaces = true;
IMyTextSurface mesurface0;
IMyTextSurface mesurface1;

bool bAllowPBRename = true;

string OurName = "Wico Modular";
string moduleName = "";

string moduleList = "";
string sVersion = " 4.2b";


string sMasterReporting = "";

public Program()
{

    if(Me.TerminalRunArgument=="--clear")
    {
        Me.CustomData = "";
        Storage = "";
    }
    MyIniParseResult result;
    if (!CustomDataIni.TryParse(Me.CustomData, out result))
    {
        Me.CustomData = "";
        CustomDataIni.Clear();
        Echo(result.ToString());
    }

    if (!SaveIni.TryParse(Storage, out result))
    {
        Storage = "";
        SaveIni.Clear();
        Echo(result.ToString());
    }

    CheckNewEntity();

    LoadDefaults();

    ModuleProgramInit();

    Runtime.UpdateFrequency |= UpdateFrequency.Once;

    OldEcho = Echo;
    Echo = MyEcho;

    PBSurfaceInit();

    if (bAllowPBRename && !Me.CustomName.Contains(moduleName))
        Me.CustomName = "PB" +moduleName;

    if (!Me.Enabled)
    {
        OldEcho("I am turned OFF!");
    }
}

void PBSurfaceInit()
{
    if (bUsePBSurfaces)
    {
        if (Me.SurfaceCount > 0)
        {
            mesurface0 = Me.GetSurface(0);
            mesurface0.ContentType = ContentType.TEXT_AND_IMAGE;
            mesurface0.WriteText(OurName + sVersion + "\n" + moduleList);
            mesurface0.FontSize = 1.3f;
            mesurface0.Alignment = TextAlignment.CENTER;
        }

        if (Me.SurfaceCount > 1)
        {
            mesurface1 = Me.GetSurface(1);
            mesurface1.ContentType = ContentType.TEXT_AND_IMAGE;
            mesurface1.WriteText("Version: " + sVersion);
            mesurface0.Alignment = TextAlignment.CENTER;
            mesurface1.TextPadding = 0.25f;
            mesurface1.FontSize = 3.5f;
        }
    }

}

void CheckNewEntity()
{
    long meentityid = 0;
    SaveIni.Get(OurName + sVersion, "MEENITYID").TryGetInt64(out meentityid);
    if (meentityid != Me.EntityId)
    {
        ErrorLog("New instance:Resetting Storage");
        Storage = "";
        SaveIni.Clear();
    }
    SaveIni.Set(OurName + sVersion, "MEENITYID", Me.EntityId);
}

void LoadDefaults()
{
    bAddDate = CustomDataIni.Get(OurName, "DebugAddDate").ToBoolean(bAddDate);
    CustomDataIni.Set(OurName, "DebugAddDate", bAddDate);
    bAddLogCount = CustomDataIni.Get(OurName, "DebugAddLogCount").ToBoolean(bAddLogCount);
    CustomDataIni.Set(OurName, "DebugAddLogCount", bAddLogCount);
    bAddRunCount = CustomDataIni.Get(OurName, "DebugAddRunCount").ToBoolean(bAddRunCount);
    CustomDataIni.Set(OurName, "DebugAddRunCount", bAddRunCount);

    bAllowPBRename = CustomDataIni.Get(OurName, "AllowPBRename").ToBoolean(bAllowPBRename);
    CustomDataIni.Set(OurName, "AllowPBRename", bAllowPBRename);

    bUsePBSurfaces = CustomDataIni.Get(OurName, "UsePBSurfaces").ToBoolean(bUsePBSurfaces);
    CustomDataIni.Set(OurName, "UsePBSurfaces", bUsePBSurfaces);

    bEchoOn = CustomDataIni.Get(OurName, "EchoOn").ToBoolean(bEchoOn);
    CustomDataIni.Set(OurName, "EchoOn", bEchoOn);
}

bool bEchoOn = true;

Action<string> OldEcho;
void MyEcho(string output)
{
    if (bEchoOn) OldEcho(output);
}

public void Save()
{
    foreach (var handler in SaveHandlers)
    {
        handler(SaveIni);
    }
    Storage = SaveIni.ToString();
}

void AddSaveHandler(Action<MyIni> handler)
{
    if (!SaveHandlers.Contains(handler))
        SaveHandlers.Add(handler);
}

void AddLoadHandler(Action<MyIni> handler)
{
    if (!LoadHandlers.Contains(handler))
        LoadHandlers.Add(handler);
}

bool HandleLoad(MyIni theIni)
{
    foreach (var handler in LoadHandlers)
    {
        handler(SaveIni);
    }
    return false;
}

void HandleMain(UpdateType updateSource)
{
    foreach(var handler in MainHandlers)
    {
        handler(updateSource);
    }
}

void AddUpdateHandler(Action<UpdateType> handler)
{
    if (!UpdateUpdateHandlers.Contains(handler))
        UpdateUpdateHandlers.Add(handler);
}

void AddTriggerHandler(Action<string,MyCommandLine, UpdateType> handler)
{
    if (!UpdateTriggerHandlers.Contains(handler))
        UpdateTriggerHandlers.Add(handler);
}

void AddResetMotionHandler(Action<bool> handler)
{
    if (!ResetMotionHandlers.Contains(handler))
        ResetMotionHandlers.Add(handler);
}

void ResetMotion(bool bNoDrills=false)
{
    foreach (var handler in ResetMotionHandlers)
    {
        handler(bNoDrills);
    }
}

void AddPostInitHandler(IEnumerator<bool> handler)
{
    if (!PostInitHandlers.Contains(handler))
        PostInitHandlers.Add(handler);
}

void AddMainHandler(Action<UpdateType> handler)
{
    if (!MainHandlers.Contains(handler))
        MainHandlers.Add(handler);
}

int postInitIterator = 0;
bool PostInit()
{
    for (; postInitIterator < PostInitHandlers.Count;postInitIterator++)
    {
        if(PostInitHandlers[postInitIterator].MoveNext())
        {
            return true;
        }
        else
        {
            PostInitHandlers[postInitIterator].Dispose();
        }
    }
    return false;
}

bool bCustomDataNeedsSave = false;
double LastRunMs = 0;
double MaxRunMs = 0;
long runCount = 0;
public void Main(string argument, UpdateType updateSource)
{
    runCount++;
    if (bInitDone && !bLastWasInit)
    {
        LastRunMs = Runtime.LastRunTimeMs;
        if (LastRunMs > MaxRunMs)
            MaxRunMs = LastRunMs;
    }
    if (moduleList != "")
    {
        Echo(OurName + sVersion + moduleList);
    }
    ModulePreMain(argument, updateSource);

    if (!bInitDone)
    {
        if (!WicoLocalInit())
        {
            Echo("Init Incomplete.  Trying again");

            Runtime.UpdateFrequency = UpdateFrequency.Once;
            return;
        }

    }
    else
    {
    }
    if ((updateSource & (utTriggers)) > 0)
    {
        MyCommandLine useCommandLine = null;
        if (myCommandLine.TryParse(argument))
        {
            useCommandLine = myCommandLine;
        }
        bool bProcessed = false;

        {
            if (argument == "save")
            {
                Save();
                ErrorLog("After Save storage=");
                ErrorLog(Storage);
                bProcessed = true;
            }

        }
        if (!bProcessed)
        {
            foreach (var handler in UpdateTriggerHandlers)
            {
                handler(argument, useCommandLine, updateSource);
            }
        }
    }
    if ((updateSource & (utUpdates)) > 0)
    {
        _wicoControl.ResetUpdates();
        foreach (var handler in UpdateUpdateHandlers)
        {
            handler(updateSource);
        }
    }

    if (sMasterReporting!="") Echo("Reporting:\n"+sMasterReporting);
    if (sMasterReporting.Length > 1024*2)
    {
        sMasterReporting="---\n"+sMasterReporting.Remove(0,1024);
    }

    HandleMain(updateSource);

    ModulePostMain(updateSource);

    if (bInitDone) bLastWasInit = false;

    if (bCustomDataNeedsSave)
    {
        bCustomDataNeedsSave = false;
        Me.CustomData = CustomDataIni.ToString();
    }
}

long logcount = 0;
bool bAddDate = false;
bool bAddLogCount = false;
bool bAddRunCount = false;
public void ErrorLog(string str)
{
    if (bAddDate) str = System.DateTime.Now.ToLongTimeString() + ":" + str; ;
    if (bAddLogCount) str = logcount++.ToString() + ":" + str;
    if (bAddRunCount) str = runCount.ToString() + ":" + str;
    sMasterReporting += "\n"+str;
}

bool bInitDone = false;
bool bLastWasInit = true;
int InitStage = 0;
bool WicoLocalInit()
{
    if (bInitDone)
    {
        return true;
    }
    if (InitStage < 1)
    {
        HandleLoad(SaveIni);
        InitStage++;
    }
    if (InitStage < 2)
    {
        if (PostInit())
            return false;
        InitStage++;
    }
    bInitDone = true;
    bLastWasInit = true;

    if (InitStage < 3)
    {
        ModulePostInit();
        if (_wicoControl != null)
            _wicoControl.ModeAfterInit(SaveIni);
        InitStage++;
    }

    Me.CustomData = CustomDataIni.ToString();

    return bInitDone;
}

void WicoInitReset()
{
    bInitDone = false;
    InitStage = 0;
}

void CustomDataChanged()
{
    bCustomDataNeedsSave = true;
}

public string niceDoubleMeters(double thed)
{
    string nice = "";
    if (thed > 1000)
    {
        nice = thed.ToString("N0") + "km";
    }
    else if (thed > 100)
    {
        nice = thed.ToString("0") + "m";
    }
    else if (thed > 10)
    {
        nice = thed.ToString("0.0") + "m";
    }
    else
    {
        nice = thed.ToString("0.000") + "m";
    }
    return nice;
}
public string Vector3DToString(Vector3D v)
{
    string s;
    s = v.X.ToString("0.00") + ":" + v.Y.ToString("0.00") + ":" + v.Z.ToString("0.00");
    return s;
}

public Vector3D StringToVector3D(string s)
{
    string[] coordinates = s.Split(',');
    if (coordinates.Length < 3)
    {
        coordinates = s.Split(':');
    }

    double x, y, z;
    int iCoordinate = 0;
    bool xOk = double.TryParse(coordinates[iCoordinate++].Trim(), out x);
    bool yOk = double.TryParse(coordinates[iCoordinate++].Trim(), out y);
    bool zOk = double.TryParse(coordinates[iCoordinate++].Trim(), out z);
    return new Vector3D(x, y, z);
}

public bool stringToBool(string txt)
{
    txt = txt.Trim().ToLower();
    return (txt == "on" || txt == "true");
}
string toGpsName(string ShipName, string sQual)
{
    string s;
    int iName = ShipName.Length;
    int iQual = sQual.Length;
    if (iName + iQual > 32)
    {
        if (iQual > 31) return "INVALID";
        iName = 32 - iQual;
    }
    s = ShipName.Substring(0, iName) + sQual;
    s.Replace(":", "_");
    s.Replace(";", "_");
    return s;

}
void EchoInstructions(string sBanner = null)
{
    float fper = 0;
    fper = Runtime.CurrentInstructionCount / (float)Runtime.MaxInstructionCount;
    if (sBanner == null) sBanner = "Instructions=";
    Echo(sBanner + " " + (fper * 100).ToString("0.00") + "%");
}



public class WicoBlockMaster
{
    Program _program;
    IMyGridTerminalSystem GridTerminalSystem;

    bool bMeGridOnly = false;

    public WicoBlockMaster(Program program, bool MeGridOnly=false)
    {
        _program = program;

        GridTerminalSystem = _program.GridTerminalSystem;
        bMeGridOnly = MeGridOnly;

        AddLocalBlockHandler(BlockParseHandler);
        AddLocalBlockChangedHandler(LocalGridChangedHandler);

        _program.AddLoadHandler(LoadHandler);
        _program.AddSaveHandler(SaveHandler);

        _program.AddPostInitHandler(LocalBlocksInit());
        _program.AddPostInitHandler(RemoteBlocksInit());

        DesiredMinTravelElevation = (float)_program.CustomDataIni.Get(_program.OurName, "MinTravelElevation").ToDouble(DesiredMinTravelElevation);
        _program.CustomDataIni.Set(_program.OurName, "MinTravelElevation", DesiredMinTravelElevation);

        LoadLocalGrid();
    }
    void LoadHandler(MyIni theINI)
    {
    }
    void SaveHandler(MyIni theINI)
    {
    }
    List<IMyShipController> shipControllers = new List<IMyShipController>();
    private IMyShipController MainShipController;
public void BlockParseHandler(IMyTerminalBlock tb)
    {
        if (tb is IMyShipController)
        {
            shipControllers.Add(tb as IMyShipController);
        }
    }

    public void LocalGridChangedHandler()
    {
        shipdimController = null;
        MainShipController = null;
        shipControllers.Clear();
    }


public bool IsClosed(IMyTerminalBlock block)
    {
        if (block == null || block.WorldMatrix == MatrixD.Identity) return true;
        return !(GridTerminalSystem.GetBlockWithId(block.EntityId) == block);
    }

public IMyRemoteControl GetRemoteControl()
    {
        foreach(var tb in shipControllers)
        {
            if(tb is IMyRemoteControl && tb.IsUnderControl)
            {

                return tb as IMyRemoteControl;
            }
        }
        foreach (var tb in shipControllers)
        {
            if (tb is IMyRemoteControl)
            {
                return tb as IMyRemoteControl;
            }
        }

        return null;
    }

public IMyShipController GetMainController()
    {
        foreach (var tb in shipControllers)
        {
            if (tb.IsUnderControl && tb.CanControlShip)
            {
                MainShipController = tb;
                break;
            }
        }
        if (MainShipController == null)
        {

            foreach (var tb in shipControllers)
            {
                if (tb is IMyRemoteControl && tb.CanControlShip)
                {
                    MainShipController = tb;
                    break;
                }
            }
            if (MainShipController == null)
            {
                foreach (var tb in shipControllers)
                {
                    if (tb is IMyCockpit && tb.CanControlShip)
                    {
                        MainShipController = tb;
                        break;
                    }
                }
            }
            if (MainShipController == null)
            {
                foreach (var tb in shipControllers)
                {
                    if (tb is IMyShipController && tb.CanControlShip)
                    {
                        MainShipController = tb;
                        break;
                    }
                }
            }
            if (MainShipController != null)
            {
                ShipDimensions(MainShipController);
            }
            else
            {
            }
        }

        return MainShipController;

    }

    public Vector3D CenterOfMass()
    {
        Vector3D com=_emptyV3D;
        var shipcontroller = GetMainController();
        if(shipcontroller!=null)
            com= shipcontroller.CenterOfMass;
        else
        {
            com=_program.Me.CubeGrid.GetPosition();
        }
        return com;
    }
    public double GetShipSpeed()
    {
        double shipspeed = -1;
        var shipcontroller = GetMainController();
        if (shipcontroller != null)
            shipspeed = shipcontroller.GetShipSpeed();
        return shipspeed;
    }
    public Vector3D GetShipVelocity()
    {
        Vector3D velocity = _emptyV3D;
        var shipcontroller = GetMainController();
        if (shipcontroller != null)
        {
            MyShipVelocities velocities = shipcontroller.GetShipVelocities();
            velocity = velocities.LinearVelocity;
        }
        return velocity;
    }

    readonly StringBuilder ShipName = new StringBuilder(42);

    public StringBuilder GetShipName()
    {
        ShipName.Clear();
        if (GetMainController() != null)
            ShipName.Append(GetMainController().CubeGrid.CustomName);
        else
            ShipName.Append(_program.Me.CubeGrid.CustomName);
        return ShipName;
    }

    public Vector3D GetNaturalGravity()
    {
        Vector3D vNG = _emptyV3D;
        var shipcontroller = GetMainController();
        if (shipcontroller != null)
            vNG = shipcontroller.GetNaturalGravity();
        return vNG;
    }

    public double GetAllPhysicalMass()
    {
        double effectiveMass = -1;
        effectiveMass = GetPhysicalMass();
        foreach(var grid in remoteCubeGrids)
        {
            bool bGridDone = false;
            foreach(var tb in gtsRemoteBlocks)
            {
                if(tb is IMyShipController && tb.CubeGrid==grid)
                {
                    var sc = tb as IMyShipController;
                    MyShipMass myMass;
                    myMass = sc.CalculateShipMass();
                    effectiveMass += myMass.PhysicalMass;
                    bGridDone = true;
                    break;
                }
            }
            if (bGridDone) break;
        }
        return effectiveMass;
    }

    public double GetPhysicalMass()
    {
        double effectiveMass = -1;
        var shipcontroller = GetMainController();
        if (shipcontroller != null)
        {
            MyShipMass myMass;
            myMass = shipcontroller.CalculateShipMass();
            effectiveMass = myMass.PhysicalMass;
        }
        return effectiveMass;
    }


    public void DisplayInfo()
    {
    }

    List<IMyTerminalBlock> gtsLocalBlocks = new List<IMyTerminalBlock>();
    public long localBlocksCount = 0;

    bool CollectRemote = false;

    List<IMyTerminalBlock> gtsRemoteBlocks = new List<IMyTerminalBlock>();
    long remoteBlocksCount = 0;

    List<IMyCubeGrid> localCubeGrids = new List<IMyCubeGrid>();

    List<IMyCubeGrid> remoteCubeGrids = new List<IMyCubeGrid>();

    List<Action<IMyTerminalBlock>> WicoLocalBlockParseHandlers = new List<Action<IMyTerminalBlock>>();
    List<Action<IMyTerminalBlock>> WicoRemoteBlockParseHandlers = new List<Action<IMyTerminalBlock>>();

    List<Action> WicoLocalBlockChangedHandlers = new List<Action>();
    List<Action> WicoRemoteBlockChangedHandlers = new List<Action>();

    List<Action> WicoLocalBlockDoneParsedHandlers = new List<Action>();
    List<Action> WicoRemoteBlockDoneParsedHandlers = new List<Action>();


    public bool AddLocalBlockHandler(Action<IMyTerminalBlock> handler)
    {
        if (!WicoLocalBlockParseHandlers.Contains(handler))
            WicoLocalBlockParseHandlers.Add(handler);
        return true;
    }
    public void AddLocalBlockChangedHandler(Action handler)
    {
        if (!WicoLocalBlockChangedHandlers.Contains(handler))
            WicoLocalBlockChangedHandlers.Add(handler);
    }

    public void AddLocalBlockParseDone(Action handler)
    {
        if (!WicoLocalBlockDoneParsedHandlers.Contains(handler))
            WicoLocalBlockDoneParsedHandlers.Add(handler);
    }

    public bool AddRemoteBlockHandler(Action<IMyTerminalBlock> handler)
    {
        if (!WicoRemoteBlockParseHandlers.Contains(handler))
            WicoRemoteBlockParseHandlers.Add(handler);
        return true;
    }
    public void AddRemoteBlocChangedHandler(Action handler)
    {
        if (!WicoRemoteBlockChangedHandlers.Contains(handler))
            WicoRemoteBlockChangedHandlers.Add(handler);
    }
    public void AddRemoteBlockParseDone(Action handler)
    {
        if (!WicoRemoteBlockDoneParsedHandlers.Contains(handler))
            WicoRemoteBlockDoneParsedHandlers.Add(handler);
    }
    public void SetMeGridOnly(bool bMeOnly = false)
    {
        bMeGridOnly = bMeOnly;
    }

    public void LoadLocalGrid()
    {
        localCubeGrids.Clear();
        gtsLocalBlocks.Clear();
        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsLocalBlocks, bLocalCheck);
    }

    bool bLocalCheck(IMyTerminalBlock tb)
    {
        bool bValid = true;
        if (!ValidBlock(tb)) return false;
        if (bMeGridOnly)
            bValid = tb.CubeGrid.EntityId == _program.Me.CubeGrid.EntityId;
        else
            bValid = tb.IsSameConstructAs(_program.Me);

        if (bValid)
        {
            if (!localCubeGrids.Contains(tb.CubeGrid))
            {
                localCubeGrids.Add(tb.CubeGrid);
            }
        }
        return bValid;
    }

public IEnumerator<bool> LocalBlocksInit()
    {
        yield return true;
        float fper = 0;

        if (gtsLocalBlocks.Count < 1)
        {
            LoadLocalGrid();
        }

        fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
        if (fper > 0.75f) yield return true;

        localBlocksCount = gtsLocalBlocks.Count;
        foreach (var tb in gtsLocalBlocks)
        {
            fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
            if (fper > 0.75f)
            {
                yield return true;
            }
            foreach (var handler in WicoLocalBlockParseHandlers)
            {
                fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
                if (fper > 0.75f)
                {
                    yield return true;
                }
                handler(tb);
            }
        }
        fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
        if (fper > 0.75f)
           yield return true;
        foreach(var handler in WicoLocalBlockDoneParsedHandlers)
        {
            handler();
        }
    }
    void LocalBlocksChanged()
    {
        foreach (var handler in WicoLocalBlockChangedHandlers)
        {
            handler();
        }
    }

public IEnumerator<bool> RemoteBlocksInit()
    {
        yield return true;
        float fper = 0;

        gtsRemoteBlocks.Clear();
        remoteCubeGrids.Clear();

        if (CollectRemote)
        {
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsRemoteBlocks, (x1 => !x1.IsSameConstructAs(_program.Me) && ValidBlock(x1)));
            fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
            if (fper > 0.75f) yield return true;

            remoteBlocksCount = gtsRemoteBlocks.Count;
            foreach (var tb in gtsRemoteBlocks)
            {
                if (!remoteCubeGrids.Contains(tb.CubeGrid))
                {
                    remoteCubeGrids.Add(tb.CubeGrid);
                }
                fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
                if (fper > 0.75f)
                {
                    yield return true;
                }
                foreach (var handler in WicoRemoteBlockParseHandlers)
                {
                    fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
                    if (fper > 0.75f)
                    {
                        yield return true;
                    }
                    handler(tb);
                }
            }
            fper = _program.Runtime.CurrentInstructionCount / (float)_program.Runtime.MaxInstructionCount;
            if (fper > 0.75f)
                yield return true;
            foreach (var handler in WicoRemoteBlockDoneParsedHandlers)
            {
                handler();
            }
        }
    }
    void RemoteBlocksChanged()
    {
        foreach (var handler in WicoLocalBlockChangedHandlers)
        {
            handler();
        }
    }

    public void SetCollectRemote(bool bUse=true)
    {
        CollectRemote = bUse;
    }

    public List<IMyTerminalBlock> GetBlocksContains<T>(string Keyword = null) where T : class
    {
        var Output = new List<IMyTerminalBlock>();
        if (gtsLocalBlocks.Count < 1) LocalBlocksInit();

        for (int e1 = 0; e1 < gtsLocalBlocks.Count; e1++)
        {
            if (gtsLocalBlocks[e1] is T
                && Keyword != null && (gtsLocalBlocks[e1].CustomName.Contains(Keyword) || gtsLocalBlocks[e1].CustomData.Contains(Keyword))
                )
            {
                Output.Add(gtsLocalBlocks[e1]);
            }
        }
        return Output;
    }

    List<IMyTerminalBlock> gtsTestBlocks = new List<IMyTerminalBlock>();
public bool CalcLocalGridChange(bool bForceUpdate = false)
    {
        gtsTestBlocks.Clear();
        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsTestBlocks, (x1 => x1.IsSameConstructAs(_program.Me) && ValidBlock(x1)));
        if (localBlocksCount != gtsTestBlocks.Count || bForceUpdate)
        {
            LocalBlocksChanged();
            localBlocksCount = gtsTestBlocks.Count;
            gtsLocalBlocks = gtsTestBlocks;
            foreach (var tb in gtsLocalBlocks)
            {
                foreach (var handler in WicoLocalBlockParseHandlers)
                {
                    handler(tb);
                }
            }
            foreach (var handler in WicoLocalBlockDoneParsedHandlers)
            {
                handler();
            }
            return true;
        }
        return false;
    }

    readonly Vector3D _emptyV3D=new Vector3D();

    bool ValidBlock(IMyTerminalBlock tb)
    {
        if (tb.GetPosition() == _emptyV3D)
        {
            return false;
        }
        else return true;
    }

public bool CalcRemoteGridChange()
    {
        List<IMyTerminalBlock> gtsTestBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gtsTestBlocks, (x1 => !x1.IsSameConstructAs(_program.Me)));
        if (remoteBlocksCount != gtsTestBlocks.Count)
        {
            RemoteBlocksChanged();
            remoteBlocksCount = gtsTestBlocks.Count;
            gtsRemoteBlocks = gtsTestBlocks;
            foreach (var tb in gtsRemoteBlocks)
            {
                foreach (var handler in WicoRemoteBlockParseHandlers)
                {
                    handler(tb);
                }
            }
            foreach (var handler in WicoRemoteBlockDoneParsedHandlers)
            {
                handler();
            }
            return true;
        }
        return false;
    }

    const float SMALL_BLOCK_VOLUME = 0.5f;
    const float LARGE_BLOCK_VOLUME = 2.5f;
    const float SMALL_BLOCK_LENGTH = 0.5f;
    const float LARGE_BLOCK_LENGTH = 2.5f;

    private float _length_blocks, _width_blocks, _height_blocks;
    private double _length, _width, _height;
    public float gridsize;
    private OrientedBoundingBoxFaces _obbf;

    IMyShipController shipdimController;
    void ShipDimensions(IMyShipController orientationBlock)
    {
        shipdimController = orientationBlock;

        if (_program.Me.CubeGrid.GridSizeEnum.ToString().ToLower().Contains("small"))
            gridsize = SMALL_BLOCK_LENGTH;
        else
            gridsize = LARGE_BLOCK_LENGTH;

        _obbf = new OrientedBoundingBoxFaces(orientationBlock);
        Vector3D[] points = new Vector3D[4];
        _obbf.GetFaceCorners(OrientedBoundingBoxFaces.LookupFront, points);
        _width = (points[0] - points[1]).Length();
        _height = (points[0] - points[2]).Length();
        _obbf.GetFaceCorners(0, points);
        _length = (points[0] - points[2]).Length();

        _length_blocks = (float)(_length / gridsize);
        _width_blocks = (float)(_width / gridsize);
        _height_blocks = (float)(_height / gridsize);
    }
    public float LengthInBlocks()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return _length_blocks;
    }
    public double LengthInMeters()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return _length;
    }
    public float WidthInBlocks()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return _width_blocks;
    }
    public double WidthInMeters()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return _width;
    }
    public float HeightInBlocks()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return _height_blocks;
    }
    public double HeightInMeters()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return _height;
    }
    public double LargestSideInMeters()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        double largest = _height;
        if (_length > largest)
            largest = _length;
        if (_width > largest)
            largest = _width;
        return largest;
    }
    public double BlockMultiplier()
    {
        if (shipdimController == null) ShipDimensions(GetMainController());
        return gridsize;
    }

public void BlocksOnOff(List<IMyTerminalBlock> blocks, bool bOn = true)
    {
        foreach (var b in blocks)
        {
            IMyFunctionalBlock f = b as IMyFunctionalBlock;
            if (f == null) continue;
            f.Enabled = bOn;
        }
    }
    public float DesiredMinTravelElevation = 0;

}

public struct OrientedBoundingBoxFaces
{
    public Vector3D[] Corners;
    Vector3D localMax;
    Vector3D localMin;

    public Vector3D Position;

    static int[] PointsLookupRight = { 1, 3, 5, 7 };
    static int[] PointsLookupLeft = { 0, 2, 4, 6 };

    static int[] PointsLookupTop = { 2, 3, 6, 7 };
    static int[] PointsLookupBottom = { 0, 1, 4, 5 };

    static int[] PointsLookupBack = { 4, 5, 6, 7 };
    static int[] PointsLookupFront = { 0, 1, 2, 3 };

    static int[][] PointsLookup = {
PointsLookupRight, PointsLookupLeft,
PointsLookupTop, PointsLookupBottom,
PointsLookupBack, PointsLookupFront
    };
    public const int LookupRight = 0;
    public const int LookupLeft = 1;
    public const int LookupTop = 2;
    public const int LookupBottom = 3;
    public const int LookupBack = 4;
    public const int LookupFront = 5;

    public OrientedBoundingBoxFaces(IMyTerminalBlock block)
    {
        Corners = new Vector3D[8];
        if (block == null)
        {
            Position = new Vector3D();
            localMin = new Vector3D();
            localMax = new Vector3D();
            return;
        }

        localMin = new Vector3D(block.CubeGrid.Min) - new Vector3D(0.5, 0.5, 0.5);
        localMin *= block.CubeGrid.GridSize;
        localMax = new Vector3D(block.CubeGrid.Max) + new Vector3D(0.5, 0.5, 0.5);
        localMax *= block.CubeGrid.GridSize;

        var blockOrient = block.WorldMatrix.GetOrientation();

        var matrix = block.CubeGrid.WorldMatrix.GetOrientation() * MatrixD.Transpose(blockOrient);

        Vector3D.TransformNormal(ref localMin, ref matrix, out localMin);
        Vector3D.TransformNormal(ref localMax, ref matrix, out localMax);

        var tmpMin = Vector3D.Min(localMin, localMax);
        localMax = Vector3D.Max(localMin, localMax);
        localMin = tmpMin;


        var center = block.CubeGrid.GetPosition();

        Vector3D tmp2;
        Vector3D tmp3;
        tmp2 = localMin;
        Vector3D.TransformNormal(ref tmp2, ref blockOrient, out tmp2);
        tmp2 += center;

        tmp3 = localMax;
        Vector3D.TransformNormal(ref tmp3, ref blockOrient, out tmp3);
        tmp3 += center;

        BoundingBox bb = new BoundingBox(tmp2, tmp3);
        Position = bb.Center;


        Vector3D tmp;
        for (int i = 0; i < 8; i++)
        {
            tmp.X = ((i & 1) == 0 ? localMin : localMax).X;
            tmp.Y = ((i & 2) == 0 ? localMin : localMax).Y;
            tmp.Z = ((i & 4) == 0 ? localMin : localMax).Z;
            Vector3D.TransformNormal(ref tmp, ref blockOrient, out tmp);
            tmp += center;
            Corners[i] = tmp;
        }
    }


public void GetFaceCorners(int face, Vector3D[] points, int index = 0)
    {
        face %= PointsLookup.Length;
        for (int i = 0; i < PointsLookup[face].Length; i++)
        {
            points[index++] = Corners[PointsLookup[face][i]];
        }
    }

}

double CalculateYaw(Vector3D destination, IMyTerminalBlock Origin)
{
    double yawAngle = 0;
    bool facingTarget = false;

    MatrixD refOrientation = GetBlock2WorldTransform(Origin);

    Vector3D vCenter = Origin.GetPosition();
    Vector3D vBack = vCenter + 1.0 * Vector3D.Normalize(refOrientation.Backward);
    Vector3D vRight = vCenter + 1.0 * Vector3D.Normalize(refOrientation.Right);
    Vector3D vLeft = vCenter - 1.0 * Vector3D.Normalize(refOrientation.Right);



    double rightTargetDistance = calculateDistance(vRight, destination);

    double leftTargetDistance = calculateDistance(vLeft, destination);

    double yawLocalDistance = calculateDistance(vRight, vLeft);


    double centerTargetDistance = Vector3D.DistanceSquared(vCenter, destination);
    double backTargetDistance = Vector3D.DistanceSquared(vBack, destination);
    facingTarget = centerTargetDistance < backTargetDistance;

    yawAngle = (leftTargetDistance - rightTargetDistance) / yawLocalDistance;

    if (!facingTarget)
    {
        yawAngle += (yawAngle < 0) ? -1 : 1;
    }
    return yawAngle;
}

double calculateDistance(Vector3D a, Vector3D b)
{
    return Vector3D.Distance(a, b);
}

MatrixD GetGrid2WorldTransform(IMyCubeGrid grid)
{ Vector3D origin = grid.GridIntegerToWorld(new Vector3I(0, 0, 0)); Vector3D plusY = grid.GridIntegerToWorld(new Vector3I(0, 1, 0)) - origin; Vector3D plusZ = grid.GridIntegerToWorld(new Vector3I(0, 0, 1)) - origin; return MatrixD.CreateScale(grid.GridSize) * MatrixD.CreateWorld(origin, -plusZ, plusY); }
MatrixD GetBlock2WorldTransform(IMyCubeBlock blk)
{ Matrix blk2grid; blk.Orientation.GetMatrix(out blk2grid); return blk2grid * MatrixD.CreateTranslation(((Vector3D)new Vector3D(blk.Min + blk.Max)) / 2.0) * GetGrid2WorldTransform(blk.CubeGrid); }


public class WicoControl : WicoUpdates
{
    bool _bControlDebug = false;

    const string MODECHANGETAG = "[WICOMODECHANGE]";
    int _iMode = -1;
    int _iState = -1;

    string ControlSection = "WicoControl";
    public int IMode
    {
        get
        {
            return _iMode;
        }

        set
        {
            SetMode(value);
        }
    }

    public int IState
    {
        get
        {
            return _iState;
        }

        set
        {
            SetState(value);
        }
    }

    List<Action<int, int, int, int>> ControlChangeHandlers = new List<Action<int, int, int, int>>();
    List<Action> ModeAfterInitHandlers = new List<Action>();

    public const int MODE_IDLE = 0;

    public const int MODE_DOCKING = 30;
    public const int MODE_DOCKED = 40;
    public const int MODE_LAUNCH = 50;

    public const int MODE_LAUNCHPREP = 100;
    public const int MODE_ORBITALLAUNCH = 120;
    public const int MODE_DESCENT = 150;
    public const int MODE_ORBITALLAND = 151;
    public const int MODE_HOVER = 170;
    public const int MODE_LANDED = 180;

    public const int MODE_MINE = 500;
    public const int MODE_GOTOORE = 510;
    public const int MODE_BORESINGLE = 520;

    public const int MODE_EXITINGASTEROID = 590;


    public const int MODE_STARTNAV = 600;
    public const int MODE_GOINGTARGET = 650;
    public const int MODE_NAVNEXTTARGET = 670;
    public const int MODE_ARRIVEDTARGET = 699;


    public const int MODE_DOSCANS = 900;

    public const int MODE_UNDERCONSTRUCTION = 1000;

    public const int MODE_ATTACKDRONE = 2000;

    public const int MODE_ATTENTION = 9999;

    StringBuilder sbData = new StringBuilder(100);
    public void SetMode(int theNewMode, int theNewState = 0)
    {
        if (_bControlDebug) _program.ErrorLog("MSet I= " + _iMode + " S=" + theNewState);
        if (_iMode == theNewMode)
            return;

        sbData.Clear();
        sbData.AppendLine(_iMode.ToString());
        sbData.AppendLine(_iState.ToString());
        sbData.AppendLine(theNewMode.ToString());
        sbData.AppendLine(theNewState.ToString());

        SendToAllSubscribers(MODECHANGETAG, sbData.ToString());
        HandleModeChange(_iMode, _iState, theNewMode, theNewState);

        _iMode = theNewMode;
        SetState(theNewState);
        WantOnce();
    }

    public void SetState(int theNewState)
    {
        if (_bControlDebug) _program.ErrorLog("SSet I= "+ _iMode + " S=" + theNewState);

        _iState = theNewState;
    }
    public bool AddControlChangeHandler(Action<int, int, int, int> handler)
    {
        if (!ControlChangeHandlers.Contains(handler))
            ControlChangeHandlers.Add(handler);
        return true;
    }
    void HandleModeChange(int fromMode, int fromState, int toMode, int toState)
    {
        foreach (var handler in ControlChangeHandlers)
        {
            handler(fromMode, fromState, toMode, toState);
        }
    }

    public bool AddModeInitHandler(Action handler)
    {
        if (!ModeAfterInitHandlers.Contains(handler))
            ModeAfterInitHandlers.Add(handler);
        return true;
    }
    new public void ModeAfterInit(MyIni theIni)
    {
        _iState = theIni.Get(ControlSection, "State").ToInt32(_iState);
        _iMode = theIni.Get(ControlSection, "Mode").ToInt32(_iMode);


        foreach (var handler in ModeAfterInitHandlers)
        {
            handler();
        }
    }

    void SaveHandler(MyIni theIni)
    {
        theIni.Set(ControlSection, "Mode", _iMode);
        theIni.Set(ControlSection, "State", _iState);
    }



    WicoIGC _wicoIGC;

    readonly TransmissionDistance localConstructs = TransmissionDistance.CurrentConstruct;
    public WicoControl(Program program, WicoIGC wicoIGC): base(program)
    {
        _program = program;
        _wicoIGC = wicoIGC;

        WicoControlInit();
    }

List<long> _WicoMainSubscribers = new List<long>();
    bool bIAmMain = true;

    readonly string WicoMainTag = "WicoTagMain";
    readonly string YouAreSub = "YOUARESUB";
    readonly string UnicastTagTrigger = "TRIGGER";
    readonly string UnicastAnnounce = "IAMWICO";

    public void WicoControlInit()
    {
        _WicoMainSubscribers.Clear();
        bIAmMain = true;

        _program.IGC.SendBroadcastMessage(WicoMainTag, "Configure", localConstructs);

        _wicoIGC.AddPublicHandler(WicoMainTag, WicoControlMessagehandler, true);
        _wicoIGC.AddUnicastHandler(WicoConfigUnicastListener);

        _program.AddTriggerHandler(ProcessTrigger);

        _bControlDebug = _program.CustomDataIni.Get(_program.OurName, "ControlDebug").ToBoolean(_bControlDebug);
        _program.CustomDataIni.Set(_program.OurName, "ControlDebug", _bControlDebug);

        _program.AddSaveHandler(SaveHandler);

    }
    public bool IamMain()
    {
        return bIAmMain;
    }

public void ProcessTrigger(string sArgument,MyCommandLine myCommandLine, UpdateType updateSource)
    {
        if (myCommandLine != null && myCommandLine.ArgumentCount > 1)
        {
            if (myCommandLine.Argument(0) == "setmode")
            {
                int toMode = 0;
                bool bOK = int.TryParse(myCommandLine.Argument(1), out toMode);
                if (bOK)
                {
                    SetMode(toMode);
                    WantOnce();
                }
            }
        }

    }

    public void SendToAllSubscribers(string tag, string argument)
    {
        foreach (var submodule in _WicoMainSubscribers)
        {
            if (submodule == _program.Me.EntityId) continue;
            _program.IGC.SendUnicastMessage(submodule, tag, argument);
        }
    }

public void WicoControlMessagehandler(MyIGCMessage msg)
    {
        var tag = msg.Tag;

        var src = msg.Source;
        if (tag == WicoMainTag)
        {
            if (msg.Data is string)
            {
                string data = (string)msg.Data;
                if (data == "Configure")
                {
                    _program.IGC.SendUnicastMessage(src, UnicastAnnounce, "");
                }
            }
        }
    }

public void WicoConfigUnicastListener(MyIGCMessage msg)
    {
        var tag = msg.Tag;
        var src = msg.Source;
        if (tag == YouAreSub)
        {
            bIAmMain = false;
        }
        else if (tag == UnicastAnnounce)
        {
            if (_WicoMainSubscribers.Contains(src))
            {
            }
            else
            {
                _WicoMainSubscribers.Add(src);
            }
            bIAmMain = true;
            foreach (var other in _WicoMainSubscribers)
            {
                if (other < _program.Me.EntityId)
                {
                    bIAmMain = false;
                }
            }
        }
        else if (tag == UnicastTagTrigger)
        {
        }
        else if (tag == MODECHANGETAG)
        {
            string[] aLines = ((string)msg.Data).Split('\n');
            int theNewMode = Convert.ToInt32(aLines[2]);
            int theNewState = Convert.ToInt32(aLines[3]);

            if (_iMode != theNewMode)
                HandleModeChange(_iMode, _iState, theNewMode, theNewState);

            _iMode = theNewMode;
            SetState(theNewState);
        }
    }

    public new void AnnounceState()
    {
        if (_bControlDebug)
        {
        }
        if (bIAmMain) _program.Echo("MAIN. Mode=" + IMode.ToString() + " S=" + IState.ToString());
        else _program.Echo("SUB. Mode=" + IMode.ToString() + " S=" + IState.ToString());
    }
}

public class WicoUpdates
{
    protected Program _program;

    public bool _bUpdateDebug = false;

    public WicoUpdates(Program program)
    {
        _program = program;

        _bUpdateDebug = _program.CustomDataIni.Get(_program.OurName, "UpdateDebug").ToBoolean(_bUpdateDebug);
        _program.CustomDataIni.Set(_program.OurName, "UpdateDebug", _bUpdateDebug);
    }

    public float fMaxWorldMps = 100f;

    bool bWantOnce = false;
    bool bWantFast = false;
    bool bWantMedium = false;
    bool bWantSlow = false;

public void ResetUpdates()
    {
        bWantOnce = false;
        bWantFast = false;
        bWantMedium = false;
        bWantSlow = false;
    }

public void WantOnce()
    {
        bWantOnce = true;
    }

public void WantFast()
    {
        bWantFast = true;
    }
public void WantMedium()
    {
        bWantMedium = true;
    }
public void WantSlow()
    {
        bWantSlow = true;
    }

public UpdateFrequency GenerateUpdate()
    {
        UpdateFrequency desired = 0;
        if (bWantOnce) desired |= UpdateFrequency.Once;
        if (bWantFast) desired |= UpdateFrequency.Update1;
        if (bWantMedium) desired |= UpdateFrequency.Update10;
        if (bWantSlow) desired |= UpdateFrequency.Update100;
        return desired;
    }

    public void AnnounceState()
    {
    }

    public void ModeAfterInit(MyIni theIni)
    {
    }
}