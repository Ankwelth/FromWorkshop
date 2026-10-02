/* 
         * РЛС "Сапфир"
         * Версия 1.7
         * Дальнейшая эволюция РЛС "Изумруд"
         * 
         * Создано в Н.С.К.С для внутреннего пользования. Должна комбинироваться с модулем активной РЛС. 
         */

        // --Названия блоков
        string RCN = "УДТ"; // Название блока ДУ
        string RCN2 = "БУС"; // Название блока ДУ для сканирования
        string LCDN = "Дисплей РЛС"; // добавление цифры в название: 1 - инфо целей, 2 - дисплей РЛС, 3 - дисплей ошибок,
        // 4 - дисплей прицела, 5 - дисплей дальномера, 6 - дисплей автомата ТЛ, 7 - дисплей диагностики.
        string CGN = "Камеры РЛС"; // Название группы камер
        string TGN = "Турели РЛС"; // Название группы турелей
        string PBN = "ПБ"; // Тег для подключаемых ПБ
        string dec1 = "РЛС ''Сапфир''";
        string TGC = "Управляемые турели"; // Группа контролируемых турелей
        string CCN = "Центральный компьютер"; // Название борт. компьютера
        string PRN = "Гол. прицел"; // Проекторы прицела, 1 - обычный, 2 - поиск цели
        string PRN2 = "Второй Гол. прицел"; // Проекторы прицела доп. блока для сканирования, 1 - обычный, 2 - поиск цели
        string RTag = "САПФИР1"; // Тег радара (радиосвязь)
        string DN = "Динамик тревоги"; // Название динамика для тревог
        string DN2 = "Динамик системы ПРО"; // Название динамика для системы ПРО
        // --Детализация РЛС
        // Дисплей РЛС
        double Center = 10.5; // Номер центральной строки, делим на 2 кол-во строк.
        string R1 = ""; // Указатель союзника
        string R2 = ""; // Указатель противника
        string R3 = ""; // Указатель выбранной цели
        // Дисплей целей
        string R4 = ""; // Линейка 
        string R5 = ""; // Маркер главной цели
        string R6 = ""; // Маркер союзника
        string R7 = ""; // Маркер противника
        string R8 = ""; // Маркер переданной цели
        string gyrotag = "ГРК"; // Тег гироскопа
        string AIBlockName = "Блок ИИ Нападения РЛС"; // Название блока наступательного ИИ || Name of offensive AI block
        string AIBlockName2 = "Блок ИИ Перемещения РЛС"; // Название блока перемещения ИИ || Name of moving AI block

        List<string> RGrap = new List<string>();
        // Можно добавлять доп. строки
        Program()
        {
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            RGrap.Add("");
            // Кусок кода
            listener = IGC.RegisterBroadcastListener(RTag);
            listener.SetMessageCallback(RTag);
            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
            SF1 = SF;
            UF1 = UF;
            TF1 = TF;
            t1 = t;
            IMyBlockGroup G;
            CC = GridTerminalSystem.GetBlockWithName(CCN) as IMyProgrammableBlock;
            RC = GridTerminalSystem.GetBlockWithName(RCN) as IMyShipController;
            RSC = GridTerminalSystem.GetBlockWithName(RCN2) as IMyShipController;
            D = GridTerminalSystem.GetBlockWithName(DN) as IMySoundBlock;
            D2 = GridTerminalSystem.GetBlockWithName(DN2) as IMySoundBlock;
            G = GridTerminalSystem.GetBlockGroupWithName(CGN);
            if (G != null) G.GetBlocksOfType(Cameras);
            G = GridTerminalSystem.GetBlockGroupWithName(TGN);
            if (G != null) G.GetBlocksOfType(Turrets);
            G = GridTerminalSystem.GetBlockGroupWithName(TGC);
            if (G != null) G.GetBlocksOfType(CTurrets);
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType(gyros);
            GridTerminalSystem.GetBlocks(blocks);
            foreach (IMyTerminalBlock block in blocks)
            {
                if (block.CustomName.Contains(LCDN)) LCDs.Add(block as IMyTextPanel);
                if (block.CustomName.Contains(PRN)) PRjs.Add(block as IMyProjector);
                if (block.CustomName.Contains(PRN2)) PRjs.Add(block as IMyProjector);
                if (block.CustomName.Contains(PBN)) PBs.Add(block as IMyProgrammableBlock);
            }
            blocks.Clear();
            foreach (IMyCameraBlock cam in Cameras)
            {
                cam.EnableRaycast = true;
            }
            // Конец куска кода
        }
        // --Переменные boolean
        bool Radio = true; // Обмен информацией с другими РЛС
        bool TurretsScan = false; // Турель оперируемая игроком указывает направление сканирования
        bool MouseSelector = false;
        bool TargetUpd = true; // Обновление целей камерами
        bool FriendlyOff = false; // Дружественный огонь
        bool DirectedTargeting = false; // Целеуказание ПБ напрямую через аргументы
        bool SimpleGraphic = true; // Режим упрощенной графики
        bool OnlyForDirScan = true; // Скан только фронтовыми камерами
        bool UseAllTurrets = true;
        bool UseAllGyros = true;
        // --Переменные double и float
        float RQ = 0.800F; // Разрешение РЛС
        double Q = 10; // Коэфф. углубления при повторном сканировании (запас расстояния в случае деформации цели)
        double UF = 1; // Кол-во тиков до следующего обновления целей камерами. Существенно влияет на надёжность ведения.
        double TF = 25.5; // Кол-во тиков до следующего выстрела лучами в активном режиме
        double MaxRange = 4000; // Макс. дистанция   
        // ИК-автомат
        bool IR = true; // Автомат ТЛ
        bool SmartIR = false; // Подключены ПРО ракеты?
        double IS = 10; // Мин. размер ракеты
        double tspeed = 50; // Мин. скорость ракеты
        double trange = 2000; // Макс. дальность ракеты
        double minrange = 1000; // Мин. дальность ракеты (для пуска противоракеты)
        double tcoeff = 2; // Чувствительность 
        int launch_time = 500; // Через сколько вызвов кидать следующую ракету
        int missiles_count = 1; // Сколько ракет запускать
        int t = 3; // Таймер пуска ТЛ
        string launch_timer_name = "Пуск ЗУР-30";
        string launch_computer_name = "ЦП ЗУР-30";
        // упр. турели
        double TRange = 800;
        double TSpeed = 400;
        double SafetyDist = 0.86; // Зона запрета на дружественный огонь (сфера вокруг корабля)
        // фильтр
        double minsize = 10;
        // --Переменные int
        int UCount = 10; // Кол-во лучей попытки повторного поиска
        int MaxCount = 6; // Макс. число целей ведомых камерами
        int SF = 150; // Как часто РЛС будет обновлять свои блоки. -1 для отключения.
        int C = 5; // Делитель скорости
        // балл. компьютер
        int WRange = 800;
        int WSpeed = 300; // Гатлинг - 400 м\с, ракеты - 200(300) м\с.
        // углы поиска утерянной цели
        int minangle = -25;
        int maxangle = 25;
        int RF = 25; // Частота отчиски списка целей (если нет камер)
        int FF = 5; // Кол-во лучей при сканировании
        // Сканирование блоками ИИ
        bool ai_scanning = true;
        static int ai_scan_freq = 40; // Частота поиска целей блоком ИИ || Scanning via AI-block frequency 
        //
        /*******************************************************/
        bool Scan = false;
        bool SuppScan = false;
        bool Active = false;
        bool Offset = false;
        bool GyroUse = false;
        bool BallisticMode = true;
        /**/
        IMyBroadcastListener listener;
        public string IsMode(bool Mode)
        {

            if (Mode == true)
            {
                if (!Scan) return "Активный режим";
                else return "Активный режим, идёт захват цели";
            }
            if (Mode == false)
            {
                if (!Scan) return "Пассивный режим";
                else return "Пассивный режим, идёт захват цели";
            }

            return "";
        }
        public string IsWorking(string w)
        {
            if (w == "") return "";
            else return "";
        }
        public string IsAIMode(bool w)
        {
            if (w) return "включен";
            else return "выключен";
        }
        public string Targetor(MyRelationsBetweenPlayerAndBlock relation)
        {
            if (relation == MyRelationsBetweenPlayerAndBlock.Enemies) return R7;
            if (relation == MyRelationsBetweenPlayerAndBlock.Friends) return R6;
            return "";
        }
        /**/
        IMyShipController RC;
        IMyShipController RSC;
        IMyProgrammableBlock CC;
        IMyProgrammableBlock MC;
        IMyOffensiveCombatBlock AIBlock_offensive;
        IMyFlightMovementBlock AIBlock_move;
        IMySoundBlock D;
        IMySoundBlock D2;
        IMyTimerBlock launch_timer;
        List<IMyTextPanel> LCDs = new List<IMyTextPanel>();
        List<IMyCameraBlock> Cameras = new List<IMyCameraBlock>();
        List<IMyLargeTurretBase> Turrets = new List<IMyLargeTurretBase>();
        List<IMyProgrammableBlock> PBs = new List<IMyProgrammableBlock>();
        List<IMyLargeTurretBase> CTurrets = new List<IMyLargeTurretBase>();
        List<IMyProjector> PRjs = new List<IMyProjector>();
        List<IMyGyro> gyros = new List<IMyGyro>();
        List<MyDetectedEntityInfo> Targets = new List<MyDetectedEntityInfo>();
        List<MyDetectedEntityInfo> Enemy_missiles = new List<MyDetectedEntityInfo>();
        List<MyDetectedEntityInfo> Launch_Sequency = new List<MyDetectedEntityInfo>();
        List<Vector3> acc = new List<Vector3>();
        StringBuilder sb = new StringBuilder();
        string wd = "";
        MyDetectedEntityInfo MainTarget;
        Vector3 MainAcc;
        Random rand = new Random();
        int SF1 = 0;
        int launch_time1 = 0;
        int RF1 = 0;
        int t1 = 0;
        double UF1 = 0;
        double TF1 = 0;
        int ai_scan_freq_code = 0;
        void Main(String args)
        {
            bool exist = false;
            if (true)
            {
                if (SF1 == 0)
                {
                    Turrets.Clear();
                    CTurrets.Clear();
                    Cameras.Clear();
                    LCDs.Clear();
                    PRjs.Clear();
                    PBs.Clear();
                    gyros.Clear();
                    IMyBlockGroup G;
                    AIBlock_offensive = GridTerminalSystem.GetBlockWithName(AIBlockName) as IMyOffensiveCombatBlock;
                    AIBlock_move = GridTerminalSystem.GetBlockWithName(AIBlockName2) as IMyFlightMovementBlock;
                    MC = GridTerminalSystem.GetBlockWithName(launch_computer_name) as IMyProgrammableBlock;
                    CC = GridTerminalSystem.GetBlockWithName(CCN) as IMyProgrammableBlock;
                    RC = GridTerminalSystem.GetBlockWithName(RCN) as IMyRemoteControl;
                    RSC = GridTerminalSystem.GetBlockWithName(RCN2) as IMyRemoteControl;
                    D = GridTerminalSystem.GetBlockWithName(DN) as IMySoundBlock;
                    launch_timer = GridTerminalSystem.GetBlockWithName(launch_timer_name) as IMyTimerBlock;
                    G = GridTerminalSystem.GetBlockGroupWithName(CGN);
                    if (G != null) G.GetBlocksOfType(Cameras);
                    G = GridTerminalSystem.GetBlockGroupWithName(TGN);
                    if (G != null) G.GetBlocksOfType(Turrets);
                    G = GridTerminalSystem.GetBlockGroupWithName(TGC);
                    if (G != null) G.GetBlocksOfType(CTurrets);
                    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                    if (UseAllTurrets) GridTerminalSystem.GetBlocksOfType(Turrets);
                    GridTerminalSystem.GetBlocksOfType(gyros);
                    GridTerminalSystem.GetBlocks(blocks);
                    foreach (IMyTerminalBlock block in blocks)
                    {
                        if (block.CustomName.Contains(LCDN)) LCDs.Add(block as IMyTextPanel);
                        if (block.CustomName.Contains(PRN) || block.CustomName.Contains(PRN2)) PRjs.Add(block as IMyProjector);
                        if (block.CustomName.Contains(PBN)) PBs.Add(block as IMyProgrammableBlock);
                    }
                    blocks.Clear();
                    foreach (IMyCameraBlock cam in Cameras)
                    {
                        cam.EnableRaycast = true;
                    }
                    SF1 = SF;
                }
                List<int> numbers = new List<int>();
                // Поиск целей с помощью блока ИИ
                if (AIBlock_offensive != null && AIBlock_move != null && AIBlock_offensive.IsFunctional && AIBlock_move.IsFunctional)
                {
                    if (ai_scanning)
                    {
                        ai_scan_freq_code--;
                        AIBlock_move.MinimalAltitude = 0;
                        AIBlock_move.PrecisionMode = false;
                        AIBlock_move.SpeedLimit = 100;
                        AIBlock_move.AlignToPGravity = false;
                        AIBlock_move.CollisionAvoidance = false;

                        AIBlock_offensive.UpdateTargetInterval = ai_scan_freq;
                        AIBlock_offensive.SearchEnemyComponent.TargetingLockOptions = MyGridTargetingRelationFiltering.Enemy;
                        AIBlock_offensive.SelectedAttackPattern = 3;
                        AIBlock_offensive.SetValue<long>("OffensiveCombatIntercept_GuidanceType", 0);
                        AIBlock_offensive.SetValueBool("OffensiveCombatIntercept_OverrideCollisionAvoidance", true);


                        if (ai_scan_freq_code < 1)
                        {
                            ai_scan_freq_code = ai_scan_freq;
                            if (AIBlock_move.Enabled)
                            {
                                if (AIBlock_move.CurrentWaypoint != null)
                                {
                                    IMyAutopilotWaypoint waypoint = AIBlock_move.CurrentWaypoint;
                                    Vector3D Pos = new Vector3D(waypoint.Matrix.GetRow(3).X, waypoint.Matrix.GetRow(3).Y, waypoint.Matrix.GetRow(3).Z);
                                    MyDetectedEntityInfo find_target = new MyDetectedEntityInfo(-1, "AI_Found_Ent", MyDetectedEntityType.Unknown, Pos, new MatrixD(), Pos, MyRelationsBetweenPlayerAndBlock.Enemies, new BoundingBoxD(), 0);
                                    Targets.Add(find_target);
                                    AIBlock_move.Enabled = false;
                                    AIBlock_move.ApplyAction("ActivateBehavior_Off", null);
                                    ai_scan_freq_code = ai_scan_freq;
                                }
                                AIBlock_offensive.Enabled = true;
                                AIBlock_offensive.ApplyAction("ActivateBehavior_On", null);
                            }
                            else
                            {
                                {
                                    AIBlock_move.Enabled = true;
                                    AIBlock_move.ApplyAction("ActivateBehavior_On", null);
                                    AIBlock_offensive.ApplyAction("ActivateBehavior_Off", null);
                                }
                            }
                        }
                    }
                    else
                    {
                        AIBlock_move.Enabled = false;
                        AIBlock_move.ApplyAction("ActivateBehavior_Off", null);
                        AIBlock_offensive.Enabled = false;
                        AIBlock_offensive.ApplyAction("ActivateBehavior_Off", null);
                    }

                }
                if (!TargetUpd || Cameras.Count == 0) { RF1--; if (RF1 == 0) { Targets.Clear(); RF1 = RF; acc.Clear(); } }
                for (int i = Turrets.Count - 1; i >= 0; i--)
                {
                    MyDetectedEntityInfo target = Turrets[i].GetTargetedEntity();
                    if (target.IsEmpty()) continue;
                    if (Targets.Count == 0) { if (target.BoundingBox.Volume >= minsize && target.Type != MyDetectedEntityType.Meteor && target.Type != MyDetectedEntityType.Missile && target.Type != MyDetectedEntityType.CharacterHuman && target.Type != MyDetectedEntityType.CharacterOther) { Targets.Add(target); acc.Add(target.Velocity); } continue; }
                    for (int i1 = Targets.Count - 1; i1 >= 0; i1--)
                    {
                        if (target.EntityId == Targets[i1].EntityId) { Targets[i1] = target; numbers.Add(i1); break; }
                        if (i1 == 0) { if (target.BoundingBox.Volume >= minsize) { Targets.Add(target); acc.Add(target.Velocity); numbers.Add(0); } }
                    }
                }
                if (TargetUpd && UF1 <= 0)
                {
                    int c = 0;
                    for (int i1 = Targets.Count - 1; i1 >= 0 && c <= MaxCount; i1--)
                    {
                        c++;
                        if (numbers.Contains(i1) || (Targets[i1].Position - RC.GetPosition()).Length() > MaxRange) continue;
                        MyDetectedEntityInfo target = Targets[i1];
                        for (int i = Cameras.Count - 1; i >= 0; i--)
                        {
                            IMyCameraBlock cam = Cameras[i];
                            if (!cam.CanScan(target.Position) && i > 1) continue;
                            MyDetectedEntityInfo target1;
                            Vector3D dir;
                            if (!Offset || target.HitPosition == null)
                            {
                                dir = new Vector3D(target.Position + Vector3D.Normalize(target.Position - cam.GetPosition()) * Q + target.Velocity / C + (acc[i1] - target.Velocity));
                            }
                            else
                            {
                                dir = new Vector3D((Vector3D)target.HitPosition + Vector3D.Normalize((Vector3D)target.HitPosition - cam.GetPosition()) * Q + target.Velocity / C + (acc[i1] - target.Velocity));
                            }
                            target1 = cam.Raycast(dir);
                            if ((target1.EntityId == Me.CubeGrid.EntityId || target1.Relationship == MyRelationsBetweenPlayerAndBlock.Owner) && i > 0) continue;
                            if (target.EntityId == target1.EntityId) { Targets[i1] = target1; break; }
                            else
                            {

                                int q = UCount;
                                for (int i2 = i - 1; i2 >= 0 && q >= 0; i2--)
                                {
                                    cam = Cameras[i2];
                                    Vector3D v = new Vector3D(target.Position.X + rand.Next(minangle, maxangle), target.Position.Y + rand.Next(minangle, maxangle), target.Position.Z + rand.Next(minangle, maxangle));
                                    v += target.Velocity / C + (acc[i1] - target.Velocity);
                                    if (i2 == i - 1) v = dir;
                                    if (!cam.CanScan(v) && q > 0 && i2 > 0) continue;
                                    q--;
                                    target1 = cam.Raycast(v);
                                    if (target1.EntityId == target.EntityId) { Targets[i1] = target1; break; }
                                    if (q == 0 || i2 == 0) { if (D != null) D.Play(); Targets.RemoveAt(i1); acc.RemoveAt(i1); break; }
                                }
                                break;

                            }
                        }
                    }
                    UF1 += UF;
                }
                numbers.Clear();
                if (SuppScan)
                {
                    int FF1 = FF;
                    for (int i = Cameras.Count - 1; i >= 0 && FF1 >= 0; i--)
                    {
                        IMyCameraBlock cam = Cameras[i];
                        Vector3D v = RSC.GetPosition() + RSC.WorldMatrix.Forward * MaxRange;
                        if (!Offset) v = new Vector3D(v.X + rand.Next(-i, i), v.Y + rand.Next(-i, i), v.Z + rand.Next(-i, i));

                        if ((cam.WorldMatrix.Forward == RSC.WorldMatrix.Forward || !OnlyForDirScan))
                        {
                            numbers.Add(i);
                            MyDetectedEntityInfo target = cam.Raycast(v);
                            FF1--;
                            if (target.IsEmpty() || target.EntityId == Me.CubeGrid.EntityId) continue;
                            exist = false;
                            for (int i1 = Targets.Count - 1; i1 >= 0; i1--)
                            {
                                if (Targets[i1].EntityId == target.EntityId && Offset) { Targets.RemoveAt(i1); acc.RemoveAt(i1); break; }
                                if (Targets[i1].EntityId == target.EntityId) { exist = true; break; }
                            }
                            if (!exist)
                            {
                                if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Owner || target.Type == MyDetectedEntityType.Planet || target.Type == MyDetectedEntityType.Asteroid) continue;
                                if (target.BoundingBox.Volume >= minsize && (target.Relationship != MyRelationsBetweenPlayerAndBlock.Friends || !FriendlyOff)) { Targets.Add(target); acc.Add(target.Velocity); }
                                SuppScan = false;
                                break;
                            }
                            if (Offset) break;
                        }
                    }
                }
                for (int i = PRjs.Count - 1; i >= 0; i--)
                {
                    IMyProjector p = PRjs[i];
                    if (p.CustomName.Contains("1") && p.CustomName.Contains(PRN2) && !SuppScan) p.Enabled = true;
                    if (p.CustomName.Contains("2") && p.CustomName.Contains(PRN2) && !SuppScan) p.Enabled = false;
                    if (p.CustomName.Contains("2") && p.CustomName.Contains(PRN2) && SuppScan) p.Enabled = true;
                    if (p.CustomName.Contains("1") && p.CustomName.Contains(PRN2) && SuppScan) p.Enabled = false;
                }
                if (Scan)
                {
                    int FF1 = FF;
                    for (int i = Cameras.Count - 1; i >= 0 && FF1 >= 0; i--)
                    {
                        IMyCameraBlock cam = Cameras[i];
                        Vector3D v = RC.GetPosition() + RC.WorldMatrix.Forward * MaxRange;
                        if (TurretsScan)
                        {
                            for (int i1 = Turrets.Count - 1; i1 >= 0; i1--)
                            {
                                IMyLargeTurretBase turret = Turrets[i1];
                                if (turret.IsUnderControl) 
                                {
                                    Vector3D turretDirection;
                                    Vector3D.CreateFromAzimuthAndElevation(turret.Azimuth, turret.Elevation, out turretDirection);
                                    turretDirection = Vector3D.TransformNormal(turretDirection, turret.WorldMatrix);

                                    // Преобразуем вектор из локальной системы координат турели в мировую
                                    v = turret.WorldMatrix.Translation + Vector3D.Normalize(turretDirection) * MaxRange;
                                    break;
                                }
                            }
                        }
                        if (!Offset) v = new Vector3D(v.X + rand.Next(-i, i), v.Y + rand.Next(-i, i), v.Z + rand.Next(-i, i));
                        
                        if (cam.CanScan(MaxRange) && (cam.WorldMatrix.Forward == RC.WorldMatrix.Forward || !OnlyForDirScan))
                        {
                            numbers.Add(i);
                            MyDetectedEntityInfo target = cam.Raycast(v);
                            FF1--;
                            if (target.IsEmpty() || target.EntityId == Me.CubeGrid.EntityId) continue;
                            exist = false;
                            for (int i1 = Targets.Count - 1; i1 >= 0; i1--)
                            {
                                if (Targets[i1].EntityId == target.EntityId && Offset) { Targets.RemoveAt(i1); acc.RemoveAt(i1); break; }
                                if (Targets[i1].EntityId == target.EntityId) { exist = true; break; }
                            }
                            if (!exist)
                            {
                                if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Owner || target.Type == MyDetectedEntityType.Planet || target.Type == MyDetectedEntityType.Asteroid) continue;
                                if (target.BoundingBox.Volume >= minsize && (target.Relationship != MyRelationsBetweenPlayerAndBlock.Friends || !FriendlyOff)) { Targets.Add(target); acc.Add(target.Velocity); }
                                Scan = false;
                                break;
                            }
                            if (Offset) break;
                        }
                    }
                }
                for (int i = PRjs.Count - 1; i >= 0; i--)
                {
                    IMyProjector p = PRjs[i];
                    if (p.CustomName.Contains("1") && !p.CustomName.Contains(PRN2) && !Scan) p.Enabled = true;
                    if (p.CustomName.Contains("2") && !p.CustomName.Contains(PRN2) && !Scan) p.Enabled = false;
                    if (p.CustomName.Contains("2") && !p.CustomName.Contains(PRN2) && Scan) p.Enabled = true;
                    if (p.CustomName.Contains("1") && !p.CustomName.Contains(PRN2) && Scan) p.Enabled = false;
                }
                if (Active)
                {
                    for (int i = Cameras.Count - 1; i >= 0 && TF1 <= 0; i--)
                    {
                        if (numbers.Contains(i) || !Cameras[i].CanScan(MaxRange)) continue;
                        int t = rand.Next(-45, 45);
                        int t1 = rand.Next(-45, 45);
                        IMyCameraBlock cam = Cameras[i];
                        cam.Raycast(MaxRange, t, t1);
                        if (i == 0) TF1 += TF;
                    }
                }
                numbers.Clear();
                wd = IsWorking(wd);
                double charge = 0;
                for (int i = Cameras.Count - 1; i >= 0; i--)
                {
                    charge += Cameras[i].AvailableScanRange / 1000 / Cameras.Count;
                }
                for (int i = Targets.Count - 1; i >= 0; i--)
                {
                    if (Targets[i].EntityId == MainTarget.EntityId) { MainTarget = Targets[i]; break; }
                }
                sb.Append("РЛС " + dec1 + " Н.С.К.С " + wd);
                sb.AppendLine();
                sb.Append("Режим: " + IsMode(Active));
                sb.AppendLine();
                sb.Append("Скан ИИ: " + IsMode(Active));
                sb.AppendLine();
                sb.Append("Всего целей: " + Targets.Count);
                sb.AppendLine();
                sb.Append("Заряд о/к (срзнач): " + (float)charge + " км");
                sb.AppendLine();
                charge = 0;
                for (int i = Cameras.Count - 1; i >= 0; i--)
                {
                    if (i == Cameras.Count - 1) { charge = Cameras[i].AvailableScanRange / 1000; continue; }
                    if ((Cameras[i].AvailableScanRange / 1000) < charge) charge = Cameras[i].AvailableScanRange / 1000;
                }
                sb.Append("Заряд о/к (минимум): " + (float)charge + " км");
                sb.AppendLine();
                if (Offset) { sb.Append("ТОЧЕЧНОЕ ЦЕЛЕУКАЗАНИЕ"); sb.AppendLine(); }
                if (GyroUse) { sb.Append("АВТОМАТИЧЕСКОЕ ПРИЦЕЛИВАНИЕ"); sb.AppendLine(); if (gyros.Count == 0) { sb.Append("ГИРОСКОПЫ НЕ НАЙДЕНЫ!"); sb.AppendLine(); } }
                sb.Append(R4);
                sb.AppendLine();
                if (Targets.Count > 0)
                {
                    sb.Append("Название цели: " + MainTarget.Name + " " + R5);
                    sb.AppendLine();
                    sb.Append("Объём цели: " + MainTarget.BoundingBox.Volume);
                    sb.AppendLine();
                    sb.Append("Скорость цели: " + (float)MainTarget.Velocity.Length() + " м/c");
                    sb.AppendLine();
                    sb.Append("Тип цели: " + MainTarget.Type + " " + Targetor(MainTarget.Relationship));
                    sb.AppendLine();
                    sb.Append("Расстояние до цели: " + (float)(RC.GetPosition() - MainTarget.Position).Length() + " м");
                    sb.AppendLine();
                    sb.Append("Ускорение: " + (float)(MainAcc - MainTarget.Velocity).Length() + " м/c");
                    sb.AppendLine();
                    sb.Append(">");
                    sb.AppendLine();
                }
                for (int i = Targets.Count - 1; i >= 0; i--)
                {
                    MyDetectedEntityInfo target = Targets[i];
                    if (target.EntityId == MainTarget.EntityId) continue;
                    sb.Append("Название цели: " + target.Name + " " + Targetor(target.Relationship));
                    sb.AppendLine();
                    sb.Append("Расстояние до цели: " + (float)(RC.GetPosition() - target.Position).Length() + " м");
                    sb.AppendLine();
                    sb.Append("Скорость цели: " + (float)target.Velocity.Length());
                    sb.AppendLine();
                    sb.Append(">");
                    sb.AppendLine();
                }
                if (Radio)
                {
                    for (int i = Targets.Count - 1; i >= 0; i--)
                    {
                        MyDetectedEntityInfo target = Targets[i];
                        string targetpos = target.Position.ToString() + "b";
                        string targetvel = target.Velocity.ToString() + "b";
                        string targetrelate = target.Relationship.ToString() + "b";
                        string targetsize = target.BoundingBox.Max.ToString() + "b";
                        string targetsize1 = target.BoundingBox.Min.ToString() + "b";
                        string targetname = target.Name.ToString() + "b";
                        string targetid = target.EntityId.ToString() + "b";
                        string targettype = target.Type.ToString() + "b";
                        string targets = targetpos + targetvel + targetrelate + targetsize + targetname + targetid + targettype + targetsize1;
                        IGC.SendBroadcastMessage(RTag, targets, (TransmissionDistance)MaxRange);
                    }
                    while (listener.HasPendingMessage)
                    {
                        object TrTarget = listener.AcceptMessage().Data;
                        string[] targetinfo = null;
                        char b = 'b';
                        targetinfo = TrTarget.ToString().Split(b);
                        if (targetinfo != null)
                        {
                            Vector3D position;
                            Vector3D.TryParse(targetinfo[0], out position);
                            Vector3D veloc;
                            Vector3D.TryParse(targetinfo[1], out veloc);
                            string name = targetinfo[4];
                            long targetid;
                            long.TryParse(targetinfo[5], out targetid);
                            MyDetectedEntityType targettype;
                            Enum.TryParse(targetinfo[6], out targettype);
                            Vector3D max;
                            Vector3D.TryParse(targetinfo[3], out max);
                            Vector3D min;
                            Vector3D.TryParse(targetinfo[7], out min);
                            MyDetectedEntityInfo target1;
                            if (targetid != 0) { target1 = new MyDetectedEntityInfo(targetid, name, targettype, position, new MatrixD(), veloc, MyRelationsBetweenPlayerAndBlock.Enemies, new BoundingBoxD(min, max), new long()); }
                            else continue;
                            bool et = false;
                            for (int i = Targets.Count - 1; i >= 0; i--)
                            {
                                if (Targets[i].EntityId == targetid) { et = true; break; }
                            }
                            if (!et)
                            {
                                Targets.Add(target1);
                                if (target1.EntityId == MainTarget.EntityId) continue;
                                sb.Append("Название цели: " + target1.Name + " " + R8);
                                sb.AppendLine();
                                sb.Append("Расстояние до цели: " + (float)(RC.GetPosition() - target1.Position).Length() + " м");
                                sb.AppendLine();
                                sb.Append("Скорость цели: " + (float)target1.Velocity.Length());
                                sb.AppendLine();
                            }
                        }
                    }
                }
                for (int i = Targets.Count - 1; i >= 0; i--)
                {
                    if (Targets[i].EntityId == MainTarget.EntityId) { MainTarget = Targets[i]; break; }
                }
                if (MainTarget.IsEmpty() && Targets.Count > 0) MainTarget = Targets[0];
                if (MouseSelector && Targets.Count > 1)
                {
                    for (int z = Targets.Count - 1; z >= 1; z--)
                    {
                        Vector3D Direction2 = Targets[z].Position - RC.GetPosition();
                        Vector3D NeedFDirection2 = RC.WorldMatrix.Forward - Vector3D.Normalize(Direction2);
                        Vector3D Direction21 = Targets[z - 1].Position - RC.GetPosition();
                        Vector3D NeedFDirection21 = RC.WorldMatrix.Forward - Vector3D.Normalize(Direction21);
                        for (int q = Targets.Count - 1; q >= 0; q--)
                        {
                            if (NeedFDirection2.Length() <= NeedFDirection21.Length()) MainTarget = Targets[z];
                            else MainTarget = Targets[z - 1];
                        }
                    }
                }
                MainAcc -= MainTarget.Velocity;
                if (Targets.Count > 0)
                    for (int i = Targets.Count - 1; i >= 0; i--)
                    {
                        if (Targets[i].EntityId == MainTarget.EntityId) break;
                        else if (i == 0) MainTarget = new MyDetectedEntityInfo();
                    }
                else MainTarget = new MyDetectedEntityInfo();
                for (int i = LCDs.Count - 1; i >= 0; i--)
                {
                    IMyTextPanel l = LCDs[i];
                    if (!l.CustomName.Contains("1")) continue;
                    l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                    l.Font = "Monospace";
                    l.TextPadding = 0;
                    l.WriteText(sb.ToString());
                }
                sb.Clear();
                /*Графика*/
                List<string> g = new List<string>();
                if (Targets.Count > 0)
                {
                    g.AddList(RGrap);
                    for (int i = Targets.Count - 1; i >= 0; i--)
                    {
                        MyDetectedEntityInfo n = Targets[i];
                        Vector3D EnemyPos = n.Position;
                        Vector3D OurPos = RC.CubeGrid.GetPosition();
                        Vector3D Direction = EnemyPos - OurPos;
                        if (Direction.Length() > MaxRange) continue;
                        double VertMapDir = Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(Direction)) * Direction.Length();
                        double HorRast = Vector3D.Dot(RC.WorldMatrix.Left, Vector3D.Normalize(Direction)) * Direction.Length();
                        if (VertMapDir > MaxRange) VertMapDir = MaxRange;
                        if (VertMapDir < -MaxRange) VertMapDir = -MaxRange;
                        if (HorRast > MaxRange) HorRast = MaxRange;
                        if (HorRast < -MaxRange) HorRast = -MaxRange;
                        int Hor = 0;
                        int Vert = 0;
                        double VertNeed = Math.Abs(VertMapDir / MaxRange * 100);
                        double HorNeed = Math.Abs(HorRast / MaxRange * 100);
                        /* максимум - 100%, т.е. граница дисплея*/
                        int j = (int)(100 / Center);
                        if (VertMapDir < 0)
                        {
                            for (Vert = (int)(Center); VertNeed >= j; VertNeed -= j)
                            {
                                Vert++;
                            }
                        }
                        if (VertMapDir > 0)
                        {
                            for (Vert = (int)(Center - 1); VertNeed >= j; VertNeed -= j)
                            {
                                Vert--;
                            }
                        }
                        int Vert1 = Vert;
                        if (Vert1 > g.Count - 1) Vert1 = g.Count - 1;
                        if (Vert > g.Count - 1) Vert = g.Count - 1;
                        if (Vert < 0) Vert = 0;
                        if (Vert1 < 0) Vert1 = 0;
                        if (HorRast > 0)
                        {
                            for (Hor = g[Vert].Length / 2; HorNeed >= g[Vert].Length / 2; HorNeed -= g[Vert].Length / 2)
                            {
                                Hor--;
                            }
                        }
                        if (HorRast < 0)
                        {
                            for (Hor = g[Vert].Length / 2; HorNeed >= g[Vert].Length / 2; HorNeed -= g[Vert].Length / 2)
                            {
                                Hor++;
                            }
                        }
                        if (Hor < 0) Hor = 0;
                        if (Hor > g[Vert].Length - 1) Hor = g[Vert].Length - 1;
                        g[Vert1] = g[Vert1].Remove(Hor, 1);
                        if (n.EntityId == MainTarget.EntityId) { g[Vert1] = g[Vert1].Insert(Hor, R3); continue; }
                        if (n.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies) { g[Vert1] = g[Vert1].Insert(Hor, R2); continue; }
                        if (n.Relationship == MyRelationsBetweenPlayerAndBlock.Friends) { g[Vert1] = g[Vert1].Insert(Hor, R1); continue; }
                        g[Vert1] = g[Vert1].Insert(Hor, "");
                    }
                    for (int i = 0; i < g.Count; i++)
                    {
                        sb.Append(g[i]);
                        sb.AppendLine();
                    }
                }
                else
                    for (int i = 0; i < RGrap.Count; i++)
                    {
                        sb.Append(RGrap[i]);
                        sb.AppendLine();
                    }
                g.Clear();
                /**/
                for (int i = LCDs.Count - 1; i >= 0; i--)
                {
                    IMyTextPanel l = LCDs[i];
                    if (!l.CustomName.Contains("2")) continue;
                    l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                    l.FontSize = RQ;
                    l.Font = "Monospace";
                    l.TextPadding = 0;
                    l.WriteText(sb.ToString());
                }
                sb.Clear();
                /*Диагностика*/
                sb.Append("Ошибки и уведомления РЛС: ");
                sb.AppendLine();
                sb.Append(R4);
                sb.AppendLine();
                if (Turrets.Count == 0) { sb.Append("Нет турелей для локации на малых расстояниях " + ""); sb.AppendLine(); }
                if (Cameras.Count == 0) { sb.Append("Нет камер для локации на больших расстояниях " + ""); sb.AppendLine(); }
                if (Turrets.Count < 1 && Cameras.Count < 1) { sb.Append("Нет камер или турелей для поиска целей, видны только передаваемые цели " + ""); sb.AppendLine(); }
                if (!Radio) { sb.Append("Радиосвязь отключена "); if (Turrets.Count < 1 && Cameras.Count < 1) sb.Append(""); else sb.Append(""); sb.AppendLine(); }
                if (LCDs.Count < 2) { sb.Append("Не обнаружены или недостаточно дисплеев. " + ""); sb.AppendLine(); }
                Echo(sb.ToString());
                if (Cameras.Count < MaxRange / 1000) { sb.Append("Камер слишком мало для указанного расстояния, заряд будет убывать " + ""); }
                if (TF < 15) { sb.Append("Активное сканирование проводится достаточно часто, игра может лагать."); sb.AppendLine(); }
                if (MouseSelector) { sb.Append("Выбор цели производится по близости к указателю корабля. /nВы не можете выбрать цель по дисплею " + ""); sb.AppendLine(); }
                for (int i = Targets.Count - 1; i >= 0; i--)
                {
                    if (Targets[i].BoundingBox.Volume == minsize) { sb.Append("Цель " + Targets[i].Name + " на грани фильтра."); sb.AppendLine(); sb.Append("При повреждении цели наведение может быть потеряно " + ""); sb.AppendLine(); }
                    if (Targets[i].BoundingBox.Size.Length() < 20) { sb.Append("Цель в определенных проекциях очень мала. /nРиск потерять наведение " + ""); sb.AppendLine(); }
                }
                if (Targets.Count > MaxCount - 2) { sb.Append("Будет или уже достигнут лимит ведомых целей камерами. " + ""); sb.AppendLine(); }
                /**/
                for (int i = LCDs.Count - 1; i >= 0; i--)
                {
                    IMyTextPanel l = LCDs[i];
                    if (!l.CustomName.Contains("3")) continue;
                    l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                    l.Font = "Monospace";
                    l.TextPadding = 0;
                    l.WriteText(sb.ToString());
                }
                sb.Clear();
                if (!MainTarget.IsEmpty())
                {
                    Vector3D OurPos1 = RC.GetPosition();
                    Vector3D ShootVector;
                    Vector3D EnemyPos;
                    if (!Offset || MainTarget.HitPosition == null) EnemyPos = MainTarget.Position;
                    else EnemyPos = (Vector3D)MainTarget.HitPosition;
                    Vector3D Direction = EnemyPos - OurPos1;
                    ShootVector = MainTarget.Position + (Vector3D)MainTarget.Velocity * (Direction.Length() / WSpeed) - RC.GetPosition();
                    ShootVector -= RC.GetShipVelocities().LinearVelocity * (Direction.Length() / WSpeed);
                    if (!SimpleGraphic)
                    {
                        g.Add("");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                         ");
                        g.Add("                        ");
                        g.Add("                        "); /* центр */
                        g.Add("                         ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("                          ");
                        g.Add("");
                        if (Offset && MainTarget.HitPosition != null) Direction = (Vector3D)MainTarget.HitPosition + Vector3D.Normalize((Vector3D)MainTarget.HitPosition - OurPos1) * 0.05 - OurPos1;
                        double NeedR = Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(Direction)) / Direction.Length() * 1000;
                        double NeedU = Vector3D.Dot(RC.WorldMatrix.Up, Vector3D.Normalize(Direction)) / Direction.Length() * 1000;
                        if (NeedR > 0) NeedR += g[1].Length / 2;
                        if (NeedR < 0) NeedR = g[1].Length / 2 - Math.Abs(NeedR);
                        if (NeedR < 0) NeedR = 0;
                        int i;
                        if (NeedU >= 0)
                        {
                            for (i = g.Count / 2; i > 0 && NeedU > 0; i--)
                            {
                                NeedU--;
                            }
                        }
                        else
                            for (i = g.Count / 2; i < g.Count - 1 && NeedU < 0; i++)
                            {
                                NeedU++;
                            }
                        if (NeedR > g[i].Length - 1) NeedR = g[i].Length - 1;
                        g[i] = g[i].Remove((int)NeedR, 1);
                        g[i] = g[i].Insert((int)NeedR, "");
                        NeedR = Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 5000;
                        NeedU = Vector3D.Dot(RC.WorldMatrix.Up, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 3000;
                        if (NeedR > 0) NeedR += g[1].Length / 2;
                        if (NeedR < 0) NeedR = g[1].Length / 2 + NeedR;
                        if (NeedR < 0) NeedR = 0;
                        if (NeedU >= 0)
                        {
                            for (i = g.Count / 2; i > 0 && NeedU > 0; i--)
                            {
                                NeedU--;
                            }
                        }
                        else
                            for (i = g.Count / 2; i < g.Count - 1 && NeedU < 0; i++)
                            {
                                NeedU++;
                            }
                        if (NeedR > g[i].Length - 1) NeedR = g[i].Length - 1;
                        g[i] = g[i].Remove((int)NeedR, 1);
                        g[i] = g[i].Insert((int)NeedR, "");
                        for (int i1 = 0; i1 < g.Count; i1++)
                        {
                            sb.Append(g[i1]);
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        List<string> Up = new List<string>();
                        List<string> Down = new List<string>();
                        Up.Clear();
                        Down.Clear();
                        string T1 = "--------------|--------------";
                        string T2 = "--------------|--------------";
                        string T3 = "--------------|--------------";
                        string T4 = "--------------|--------------";
                        string T5 = "--------------*--------------"; /* центр */
                        string T6 = "--------------|--------------";
                        string T7 = "--------------|--------------";
                        string T8 = "--------------|--------------";
                        string T9 = "--------------|--------------";
                        Up.Add(T1);
                        Up.Add(T2);
                        Up.Add(T3);
                        Up.Add(T4);
                        Down.Add(T6);
                        Down.Add(T7);
                        Down.Add(T8);
                        Down.Add(T9);
                        if (RC.GetShipSpeed() <= 0 && MainTarget.Velocity.Length() <= 0) ShootVector = Direction;
                        double NeedR = Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100;
                        double NeedU = Vector3D.Dot(RC.WorldMatrix.Up, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100;
                        /*выводим нужный доворот */
                        int VertSymbols = 14;

                        for (int Need = Math.Abs((int)NeedR) / 13; Need >= 0; Need--)
                        {
                            if (NeedR < 0)
                            {
                                for (int HorSymbols = 10; HorSymbols >= 0; HorSymbols--, Need--)
                                {
                                    T5 = T5.Remove(HorSymbols, 1);
                                    T5 = T5.Insert(HorSymbols, "<");
                                    if (Need <= 0) break;
                                }
                            }
                            else
                            {
                                for (int HorSymbols = 15; HorSymbols <= 28; HorSymbols++, Need--)
                                {
                                    T5 = T5.Remove(HorSymbols, 1);
                                    T5 = T5.Insert(HorSymbols, ">");
                                    if (Need <= 0) break;
                                }
                            }

                        }
                        for (int Need = Math.Abs((int)NeedU) / 45; Need >= 0; Need--)
                        {
                            if (NeedU > 0)
                            {
                                for (int FirstCol = 3; FirstCol >= 0; FirstCol--, Need--)
                                {
                                    Up[FirstCol] = Up[FirstCol].Remove(VertSymbols, 1);
                                    Up[FirstCol] = Up[FirstCol].Insert(VertSymbols, "^");
                                    if (Need <= 0) break;
                                }
                            }
                            else
                            {
                                for (int TwoCol = 0; TwoCol <= 3; TwoCol++, Need--)
                                {
                                    Down[TwoCol] = Down[TwoCol].Remove(VertSymbols, 1);
                                    Down[TwoCol] = Down[TwoCol].Insert(VertSymbols, "v");
                                    if (Need <= 0) break;
                                }

                            }

                        }

                        sb.Clear();
                        foreach (string n in Up)
                        {
                            sb.Append(n);
                            sb.AppendLine();
                        }
                        sb.Append(T5);
                        sb.AppendLine();
                        foreach (string n in Down)
                        {
                            sb.Append(n);
                            sb.AppendLine();
                        }
                    }
                    if (GyroUse)
                    {
                        foreach (IMyGyro gyro in gyros)
                        {
                            if (!gyro.CustomName.Contains(gyrotag) && !UseAllGyros) continue;
                            gyro.Enabled = true;
                            Vector3D ShootVector2 = ShootVector;
                            if (BallisticMode && RC.GetNaturalGravity().Length() > 0)
                            {
                                ShootVector2 = MainTarget.Position - RC.GetPosition() + ((Vector3D)MainTarget.Velocity - RC.GetShipVelocities().LinearVelocity) * (Direction.Length() / WSpeed) / (RC.GetNaturalGravity() * Direction.Length());
                                // if (Gravity && Check_Gravity) { vector = pos - orient.GetPosition() + ((Vector3D)(Target.Velocity - (Vector3I)RC.GetShipVelocities().LinearVelocity) * (Distance / ShellSpeed1)) / RC.GetNaturalGravity(); }
                                //ShootVector2 -= RC.GetShipVelocities().LinearVelocity * (Direction.Length() / WSpeed);
                            }
                            float gForward = (float)(Vector3D.Dot(gyro.WorldMatrix.Forward, Vector3D.Normalize(ShootVector2)) / ShootVector2.Length() * 100);
                            float gRight = (float)(Vector3D.Dot(gyro.WorldMatrix.Right, Vector3D.Normalize(ShootVector2)) / ShootVector2.Length() * 100);
                            float gUp = (float)(Vector3D.Dot(gyro.WorldMatrix.Up, Vector3D.Normalize(ShootVector2)) / ShootVector2.Length() * 100);
                            gyro.GyroOverride = true;
                            if (gyro.WorldMatrix.Forward == RC.WorldMatrix.Forward)
                            {
                                gyro.Pitch = (float)(Math.Atan2(-gUp, gForward) * 3.14);
                                gyro.Yaw = (float)(Math.Atan2(gRight, gForward) * 3.14);
                            }
                            if (gyro.WorldMatrix.Backward == RC.WorldMatrix.Forward)
                            {
                                gyro.Pitch = (float)(Math.Atan2(gUp, -gForward) * 3.14);
                                gyro.Yaw = (float)(Math.Atan2(-gRight, -gForward) * 3.14);
                            }
                            if (gyro.WorldMatrix.Right == RC.WorldMatrix.Forward)
                            {
                                gyro.Roll = (float)(-Math.Atan2(gUp, gRight) * 3.14);
                                gyro.Yaw = (float)(-Math.Atan2(gForward, gRight) * 3.14);
                            }
                            if (gyro.WorldMatrix.Left == RC.WorldMatrix.Forward)
                            {
                                gyro.Roll = (float)(Math.Atan2(gUp, -gRight) * 3.14);
                                gyro.Yaw = (float)(Math.Atan2(gForward, -gRight) * 3.14);
                            }
                            if (gyro.WorldMatrix.Up == RC.WorldMatrix.Forward)
                            {
                                gyro.Pitch = (float)(Math.Atan2(gForward, gUp) * 3.14);
                                gyro.Roll = (float)(Math.Atan2(gRight, gUp) * 3.14);
                            }
                            if (gyro.WorldMatrix.Down == RC.WorldMatrix.Forward)
                            {
                                gyro.Pitch = (float)(-Math.Atan2(gForward, gUp) * 1);
                                gyro.Roll = (float)(-Math.Atan2(gRight, gUp) * 1);
                            }
                        }
                    }
                    for (int i1 = LCDs.Count - 1; i1 >= 0; i1--)
                    {
                        IMyTextPanel l = LCDs[i1];
                        if (!l.CustomName.Contains("4")) continue;
                        l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                        l.Font = "Monospace";
                        l.FontSize = 1;
                        l.TextPadding = 0;
                        if (SimpleGraphic) l.FontSize = 2;
                        if (SimpleGraphic) l.Font = "Debug";
                        l.WriteText(sb.ToString());
                    }
                    sb.Clear();
                    sb.Append("ДИСТ. ДО ЦЕЛИ: " + (float)ShootVector.Length());
                    if (ShootVector.Length() <= WRange) sb.Append(" |+|");
                    for (int i1 = LCDs.Count - 1; i1 >= 0; i1--)
                    {
                        IMyTextPanel l = LCDs[i1];
                        if (!l.CustomName.Contains("5")) continue;
                        l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                        l.TextPadding = 0;
                        l.WriteText(sb.ToString());
                    }
                    sb.Clear();
                }
                g.Clear();
                for (int i = CTurrets.Count - 1; i >= 0; i--)
                {
                    IMyLargeTurretBase turret = CTurrets[i];
                    if (!turret.CanControl) continue;
                    if (!MainTarget.IsEmpty())
                    {
                        Vector3D d = MainTarget.Position - RC.GetPosition();
                        Vector3D pos = MainTarget.Position;
                        if (Offset) pos = (Vector3D)MainTarget.HitPosition;
                        Vector3D v = pos + (MainTarget.Velocity + MainAcc - RC.GetShipVelocities().LinearVelocity) * (d.Length() / TSpeed);
                        turret.SetTarget(v);
                        Vector3D A;
                        Vector3D.CreateFromAzimuthAndElevation(-turret.Azimuth, -turret.Elevation, out A);
                        Vector3D medir = Me.CubeGrid.GetPosition() - RC.GetPosition();
                        Vector3D crossvector = Vector3D.Reject(A, Vector3D.Normalize(medir));
                        bool confirm = true;
                        int step = 1; // Шаг проверки
                        Vector3D dir;
                        // Создаем вектор на основе азимута и элевейшн
                        Vector3D.CreateFromAzimuthAndElevation(turret.Azimuth, turret.Elevation, out dir);
                        dir = Vector3D.TransformNormal(dir, turret.WorldMatrix);
                        // Преобразуем вектор из локальной системы координат турели в мировую
                        for (double distance = 5; distance < 200; distance += step)
                        {
                            // Расчет позиции вдоль линии визирования (мировые координаты)
                            Vector3D checkPosition = turret.GetPosition() + Vector3D.Normalize(dir);
                            // Преобразуем мировые координаты в локальные координаты относительно CubeGrid
                            Vector3D localPosition = Me.CubeGrid.WorldToGridInteger(checkPosition);
                            // Преобразуем в целочисленные координаты (которые используются в CubeExists)
                            Vector3I gridPosition = Vector3I.Round(localPosition);  // Округляем позицию до целого
                            if (Me.CubeGrid.CubeExists(gridPosition))
                            {
                                // Если блок существует на пути, прекращаем проверку
                                confirm = false;
                                break;
                            }


                        }
                        if (confirm && turret.IsAimed && d.Length() <= TRange && (crossvector.Length() > SafetyDist || (medir * -A).Length() > (medir * A).Length()))
                        {
                            turret.ApplyAction("Shoot_On");
                        }
                        else turret.ApplyAction("Shoot_Off");
                    }
                    else { turret.ApplyAction("Shoot_Off"); turret.ResetTargetingToDefault(); }
                }
                Enemy_missiles.Clear();
                if (IR && CC != null)
                {
                    long tgid = 0;
                    sb.Append("Автомат противоракетной обороны РЛС " + wd);
                    sb.AppendLine();
                    bool a = false;
                    bool w = false;
                    for (int i = Targets.Count - 1; i >= 0; i--)
                    {
                        MyDetectedEntityInfo target = Targets[i];
                        if (target.BoundingBox.Volume < IS || target.Velocity.Length() < tspeed) continue;
                        Vector3D dir = target.Position - RC.GetPosition();
                        if (dir.Length() > trange) continue;
                        Vector3D myvel = RC.GetShipVelocities().LinearVelocity;
                        Vector3D Sv = dir + myvel * dir.Length();
                        Sv = -Vector3D.Reflect(Sv, Vector3D.Normalize(target.Velocity));
                        if (Vector3D.Dot(Sv, Vector3D.Normalize(dir)) > 0) continue;
                        Vector3D tg = Vector3D.Reject(target.Velocity, Vector3D.Normalize(Sv));
                        if (tg.Length() < tcoeff && tg.Length() > 0)
                        {
                            if (!Enemy_missiles.Contains(target)) Enemy_missiles.Add(target);
                            a = true;
                            if (!w)
                            {
                                w = true;
                                tgid = target.EntityId;
                                sb.Append("КОРАБЛЬ ПОД УГРОЗОЙ ПОПАДАНИЯ");
                                sb.AppendLine();
                                if (D != null) D.Play();
                            }
                            sb.Append("Цель " + target.Name + " представляет угрозу");
                            sb.AppendLine();
                            sb.Append("Время до столкновения: " + dir / target.Velocity.Length() + " с.");
                            sb.AppendLine();
                            sb.Append("Объём цели: " + target.BoundingBox.Volume + " м.");
                            sb.AppendLine();
                            sb.Append("Скорость при столкновении: " + (target.Velocity - myvel).Length() + " м/c");
                            sb.AppendLine();
                            sb.Append("=========================");
                            sb.AppendLine();
                            D2.Play();
                        }
                    }
                    if (!a) t1 = t;
                    else t1--;
                    if (t1 == 0) { t1 = t; if (!SmartIR) CC.TryRun("IRALARM"); D2.Play(); }

                    for (int i = LCDs.Count - 1; i >= 0; i--)
                    {
                        IMyTextPanel l = LCDs[i];
                        if (!l.CustomName.Contains("6")) continue;
                        l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                        l.TextPadding = 0;
                        l.WriteText(sb.ToString());
                    }
                    sb.Clear();
                }
                if (SmartIR)
                {
                    launch_time1--;
                    if (launch_time < 1)
                    for (int i = 0; i < Enemy_missiles.Count; i++)
                    {
                        MyDetectedEntityInfo missile = Enemy_missiles[i];
                        if (!Launch_Sequency.Contains(missile))
                        {
                            Launch_Sequency.Add(missile);
                            if (launch_timer != null) launch_timer.Trigger();
                                if (MC != null) for (int z = 0; z < missiles_count; z++) MC.TryRun("ATTACK@" + missile.EntityId.ToString());
                                D2.Play();
                        }
                    }
                    if (launch_time1 < 1 && Enemy_missiles.Count > 0) { launch_time1 = launch_time; Enemy_missiles.Clear(); } 
                }
                sb.Append("Диагностика РЛС");
                sb.AppendLine("_");
                sb.Append("Турели РЛС: " + Turrets.Count);
                sb.AppendLine();
                sb.Append("Проекторов: " + PRjs.Count);
                sb.AppendLine();
                sb.Append("Контролируемые турели: " + CTurrets.Count);
                sb.AppendLine();
                sb.Append("Обновление целей: " + TargetUpd);
                sb.AppendLine();
                sb.Append("Дружественный огонь: " + FriendlyOff);
                sb.AppendLine();
                sb.Append("Возможность сканирования турелью: " + TurretsScan);
                sb.AppendLine();
                sb.Append("Макс. дальность: " + MaxRange);
                sb.AppendLine();
                sb.Append("Количество камер: " + Cameras.Count);
                sb.AppendLine();
                sb.Append("Есть б. компьютер?" + CC != null);
                sb.AppendLine();
                sb.Append("Есть радиосвязь? " + Radio);
                sb.AppendLine();
                sb.Append("Подключённых ПБ: " + PBs.Count);
                sb.AppendLine();
                sb.Append("Подключённых дисплеев: " + LCDs.Count);
                sb.AppendLine();
                sb.Append("Размер этого корабля: " + RC.CubeGrid.GridSize);
                for (int i = LCDs.Count - 1; i >= 0; i--)
                {
                    IMyTextPanel l = LCDs[i];
                    if (!l.CustomName.Contains("7")) continue;
                    l.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
                    l.TextPadding = 0;
                    l.WriteText(sb.ToString());
                }
                sb.Clear();
                if (DirectedTargeting)
                    for (int i = PBs.Count - 1; i >= 0; i--)
                    {
                        IMyProgrammableBlock pb = PBs[i];
                        if (Targets.Count > 0)
                        {
                            if (!Offset || MainTarget.HitPosition == null) pb.TryRun(MainTarget.Position + "b" + (Vector3D)MainTarget.Velocity + "b" + MainTarget.BoundingBox.Min + "b" + MainTarget.BoundingBox.Max + "b" + MainTarget.EntityId + "b" + "TTARGET" + "b" + MainTarget.Orientation.M11 + ":" + MainTarget.Orientation.M12 + ":" + MainTarget.Orientation.M13 + ":" + MainTarget.Orientation.M14 + ":" + MainTarget.Orientation.M21 + ":" + MainTarget.Orientation.M22 + ":" + MainTarget.Orientation.M23 + ":" + MainTarget.Orientation.M24 + ":" + MainTarget.Orientation.M31 + ":" + MainTarget.Orientation.M32 + ":" + MainTarget.Orientation.M33 + ":" + MainTarget.Orientation.M34 + ":" + MainTarget.Orientation.M41 + ":" + MainTarget.Orientation.M42 + ":" + MainTarget.Orientation.M43 + ":" + MainTarget.Orientation.M44);
                            else pb.TryRun((Vector3D)MainTarget.HitPosition + "b" + (Vector3D)MainTarget.Velocity + "b" + MainTarget.BoundingBox.Min + "b" + MainTarget.BoundingBox.Max + "b" + MainTarget.EntityId + "b" + "TTARGET" + "b" + MainTarget.Orientation.M11 + ":" + MainTarget.Orientation.M12 + ":" + MainTarget.Orientation.M13 + ":" + MainTarget.Orientation.M14 + ":" + MainTarget.Orientation.M21 + ":" + MainTarget.Orientation.M22 + ":" + MainTarget.Orientation.M23 + ":" + MainTarget.Orientation.M24 + ":" + MainTarget.Orientation.M31 + ":" + MainTarget.Orientation.M32 + ":" + MainTarget.Orientation.M33 + ":" + MainTarget.Orientation.M34 + ":" + MainTarget.Orientation.M41 + ":" + MainTarget.Orientation.M42 + ":" + MainTarget.Orientation.M43 + ":" + MainTarget.Orientation.M44);
                            for (int i1 = 0; i1 < Targets.Count; i1++)
                            {
                                MyDetectedEntityInfo target = Targets[i1];
                                if (!Offset || target.HitPosition == null) pb.TryRun(target.Position + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
                                else pb.TryRun((Vector3D)target.HitPosition + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
                            }
                        }
                    }
                if (Targets.Count > 0)
                {
                    sb.Append(Targets.Count);
                    sb.Append("@");
                    if (!Offset || MainTarget.HitPosition == null) sb.Append(MainTarget.Position + "b" + (Vector3D)MainTarget.Velocity + "b" + MainTarget.BoundingBox.Min + "b" + MainTarget.BoundingBox.Max + "b" + MainTarget.EntityId + "b" + "TTARGET" + "b" + MainTarget.Orientation.M11 + ":" + MainTarget.Orientation.M12 + ":" + MainTarget.Orientation.M13 + ":" + MainTarget.Orientation.M14 + ":" + MainTarget.Orientation.M21 + ":" + MainTarget.Orientation.M22 + ":" + MainTarget.Orientation.M23 + ":" + MainTarget.Orientation.M24 + ":" + MainTarget.Orientation.M31 + ":" + MainTarget.Orientation.M32 + ":" + MainTarget.Orientation.M33 + ":" + MainTarget.Orientation.M34 + ":" + MainTarget.Orientation.M41 + ":" + MainTarget.Orientation.M42 + ":" + MainTarget.Orientation.M43 + ":" + MainTarget.Orientation.M44);
                    else sb.Append((Vector3D)MainTarget.HitPosition + "b" + (Vector3D)MainTarget.Velocity + "b" + MainTarget.BoundingBox.Min + "b" + MainTarget.BoundingBox.Max + "b" + MainTarget.EntityId + "b" + "TTARGET" + "b" + MainTarget.Orientation.M11 + ":" + MainTarget.Orientation.M12 + ":" + MainTarget.Orientation.M13 + ":" + MainTarget.Orientation.M14 + ":" + MainTarget.Orientation.M21 + ":" + MainTarget.Orientation.M22 + ":" + MainTarget.Orientation.M23 + ":" + MainTarget.Orientation.M24 + ":" + MainTarget.Orientation.M31 + ":" + MainTarget.Orientation.M32 + ":" + MainTarget.Orientation.M33 + ":" + MainTarget.Orientation.M34 + ":" + MainTarget.Orientation.M41 + ":" + MainTarget.Orientation.M42 + ":" + MainTarget.Orientation.M43 + ":" + MainTarget.Orientation.M44);
                    if (Targets.Count > 1) sb.Append("@");
                    for (int i1 = 0; i1 < Targets.Count; i1++)
                    {
                        MyDetectedEntityInfo target = Targets[i1];
                        if (MainTarget.EntityId == target.EntityId) continue;
                        if (!Offset || target.HitPosition == null) sb.Append(target.Position + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
                        else sb.Append((Vector3D)target.HitPosition + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
                        if (i1 != Targets.Count - 1) sb.Append("@");
                    }
                }
                Me.CustomData = sb.ToString();
                if (Enemy_missiles.Count > 0)
                {
                    sb.Append(Enemy_missiles.Count);
                    sb.Append("@");
                    for (int i1 = 0; i1 < Targets.Count; i1++)
                    {
                        MyDetectedEntityInfo target = Targets[i1];
                        if (!Offset || target.HitPosition == null) sb.Append(target.Position + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
                        else sb.Append((Vector3D)target.HitPosition + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
                        if (i1 != Targets.Count - 1) sb.Append("@");
                    }
                }
                foreach (IMyTextPanel lcd in LCDs)
                {
                    if (lcd.CustomName == LCDN + " 6") lcd.CustomData = sb.ToString();
                }
                sb.Clear();
                for (int i = Targets.Count - 1; i >= 0; i--)
                {
                    acc[i] = Targets[i].Velocity;
                }
                if (args.EndsWith("RTARGETR"))
                {
                    string[] targetinfo = null;
                    char b = 'b';
                    targetinfo = args.ToString().Split(b);
                    if (targetinfo != null)
                    {
                        Vector3D position;
                        Vector3D.TryParse(targetinfo[0], out position);
                        Vector3D veloc;
                        Vector3D.TryParse(targetinfo[1], out veloc);
                        string name = targetinfo[4];
                        long targetid;
                        long.TryParse(targetinfo[5], out targetid);
                        MyDetectedEntityType targettype;
                        Enum.TryParse(targetinfo[6], out targettype);
                        Vector3D max;
                        Vector3D.TryParse(targetinfo[3], out max);
                        Vector3D min;
                        Vector3D.TryParse(targetinfo[7], out min);
                        MyDetectedEntityInfo target1 = new MyDetectedEntityInfo();
                        if (targetid != 0)
                        {
                            target1 = new MyDetectedEntityInfo(targetid, name, targettype, position, new MatrixD(), veloc, MyRelationsBetweenPlayerAndBlock.Enemies, new BoundingBoxD(min, max), new long());
                            bool et = false;
                            for (int i = Targets.Count - 1; i >= 0; i--)
                            {
                                if (Targets[i].EntityId == targetid) { et = true; break; }
                            }
                            if (!et) { Targets.Add(target1); acc.Add(target1.Velocity); }
                        }
                    }
                }
                switch (args)
                {
                    case "RESET": Targets.Clear(); acc.Clear(); break;
                    case "SCAN": Scan = !Scan; break;
                    case "SUPPORTSCAN": SuppScan = !SuppScan; break;
                    case "RESETONE": Targets.Remove(MainTarget); acc.Remove(MainAcc); MainAcc = new Vector3(); MainTarget = new MyDetectedEntityInfo(); ; break;
                    case "MODE": Active = !Active; break;
                    case "RADIO": Radio = !Radio; break;
                    case "TARGET":
                        if (!MouseSelector)
                        {
                            for (int i = 0; i < Targets.Count; i++)
                            {
                                if (Targets[i].EntityId == MainTarget.EntityId)
                                {
                                    if (Targets.Count > i + 1) MainTarget = Targets[i + 1];
                                    else MainTarget = Targets[0];
                                    break;
                                }
                            }
                        }; break;
                    case "OFFSET": Offset = !Offset; break;
                    case "MOUSE": MouseSelector = !MouseSelector; break;
                    case "FRIEND": FriendlyOff = !FriendlyOff; break;
                    case "IR": IR = !IR; break;
                    case "UPD": TargetUpd = !TargetUpd; break;
                    case "GYROUSE": { GyroUse = !GyroUse; foreach (IMyGyro gyro in gyros) if (gyro.CustomName.Contains(gyrotag) || UseAllGyros) gyro.GyroOverride = false; } break;
                    case "SMARTIR": SmartIR = !SmartIR; break;
                    case "NOGYROS": GyroUse = false; break;
                    case "AISCANNING": ai_scanning = !ai_scanning; break;
                }
                MainAcc = MainTarget.Velocity;
                UF1--;
                SF1--;
                TF1--;
            }
            //else Echo("ВВЕДИТЕ ПАРОЛЬ");
            //if (args == "DFDWS#%#FSDFEW") dp = true;
        }

        //