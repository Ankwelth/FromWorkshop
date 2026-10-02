/*
         * Экспериментальная графическая оболочка для РЛС "Сапфир" и прочих подсистем
         * Версия 1.01
         * 
         * Создана Н.С.К.С
         */

        string PB_Radar_Name = "ЦП РЛС ''Ион''";
        string LCDN_original = "Дисплей РЛС"; // добавление цифры в название: 1 - инфо целей, 2 - дисплей РЛС, 3 - дисплей ошибок
        string LCDN = "Дисплей ГО"; // использование своих дисплеев вместо перехвата дисплеев. ГО - Графическая Оболочка. 1 - дисплей РЛС, 2 - дисплей готовности ракет.
        string orient = "УДТ"; // Блок-ориентир
        string orient_missile = "УДР"; // Блоки-ориентиры ракет
        string projector_names = "Проектор АЗ"; // Названия проекторов автоматов заряжания ракет
        string fuel_tanks_names = "Бак "; // Названия топливных баков ракет

        double MaxRange = 5000; // Максимальная дальность цели
        

        bool radar_graphic_intercept = false;
        bool Hollow_circles = false;


        List<IMyTextPanel> lcds_radar = new List<IMyTextPanel>();
        List<IMyTextPanel> lcds = new List<IMyTextPanel>();
        List<IMyShipController> missiles = new List<IMyShipController>();
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        List<MyDetectedEntityInfo> Targets = new List<MyDetectedEntityInfo>();
        List<MyDetectedEntityInfo> enemy_missiles = new List<MyDetectedEntityInfo>();
        List<IMyProjector> missile_projectors = new List<IMyProjector>();

        MyDetectedEntityInfo Target;
        IMyShipController RC;
        IMyProgrammableBlock Radar_CPU;

        int Refresh = 25;
        int Refresh1 = 0;

        // Настройки графики 
        float target_size = 5; // Размер цели на дисплее
        Color color_center = new Color(0, 255, 0); // Центр
        Color color_circle_1 = new Color(0, 150, 0, 255); // Размер внутреннего круга
        Color color_circle_2 = new Color(0, 100, 0, 255); // Размер среднего круга
        Color color_circle_3 = new Color(0, 50, 0, 255); // Размер внешнего круга

        Color color_text = new Color(0, 255, 100); // Цвет текста

        Color color_target = new Color(255, 255, 0); // Цвет выбранной цели
        Color color_enemy = new Color(255, 0, 0); // Цвет противника
        Color color_friend = new Color(0, 255, 0); // Цвет союзника
        Color color_neutral = new Color(0, 0, 255); // Цвет нейтрала
        Color color_missile = new Color(80, 100, 30); // Цвет ракеты
        Color color_missile_enemy = new Color(255, 0, 0); // Цвет вражеской ракеты

        Color color_ready_missile = new Color(0, 255, 0);
        Color color_unready_missile = new Color(255, 0, 0);
        Color color_fueled_missile = new Color(0, 255, 0);
        Color color_unfueled_missile = new Color(255, 160, 0);


        Color color_background = new Color(0, 0, 0); // Цвет фона

        string small_ship_icon = "SquareHollow";
        string big_ship_icon = "Circle";
        string missile_enemy_icon = "Triangle";
        string missile_own_icon = "Triangle";
        string unknown_icon = "SemiCircle";

        //

        Program()
        {
            GridTerminalSystem.GetBlocks(blocks);
            RC = GridTerminalSystem.GetBlockWithName(orient) as IMyShipController;
            Radar_CPU = GridTerminalSystem.GetBlockWithName(PB_Radar_Name) as IMyProgrammableBlock;
            foreach (IMyTerminalBlock block in blocks)
            {
                if (block.CustomName.Contains(LCDN)) lcds.Add(block as IMyTextPanel);
                if (block.CustomName.Contains(LCDN_original)) lcds_radar.Add(block as IMyTextPanel);
                if (block.CustomName.Contains(orient_missile)) missiles.Add(block as IMyShipController);
                if (block.CustomName.Contains(projector_names)) missile_projectors.Add(block as IMyProjector);
            }
            blocks.Clear();

            Runtime.UpdateFrequency |= UpdateFrequency.Update10;
        }

        void Main(string args)
        {
            Refresh1--;

            // Рисование РЛС
            //
            foreach (IMyTextPanel LCD in lcds)
            {
                if (LCD.CustomName != LCDN + " 1") continue;
                Radar_graphic_draw(LCD);
            }
            foreach (IMyTextPanel LCD in lcds)
            {
                if (LCD.CustomName != LCDN + " 2") continue;
                Missile_check_draw(LCD);
            }
            if (radar_graphic_intercept)
                foreach (IMyTextPanel LCD in lcds_radar)
                {
                    if (LCD.CustomName.Contains(" 2"))
                    Radar_graphic_draw(LCD);
                }
            //
            //


            // Запись списка целей и обновление
            //
            if (Radar_CPU != null && Radar_CPU.Enabled && Refresh1 < 0)
            {
                Refresh1 = Refresh;
                GridTerminalSystem.GetBlocks(blocks);
                foreach (IMyTerminalBlock block in blocks)
                {
                    if (block.CustomName.Contains(orient_missile) && !missiles.Contains(block as IMyRemoteControl)) missiles.Add(block as IMyRemoteControl);
                }
                blocks.Clear();
                for (int i = missiles.Count - 1; i >= 0; i--)
                {
                    IMyShipController missile = missiles[i];
                    if (!missile.IsFunctional) missiles.Remove(missile);
                }


                Targets.Clear();
                int q = 0;
                string[] rinfo = Radar_CPU.CustomData.Split('@');
                int.TryParse(rinfo[0], out q);

                if (q > 0)
                {
                    string[] info = rinfo[1].Split('b');
                    Vector3D Pos;
                    Vector3D.TryParse(info[0], out Pos);
                    Vector3D Veloc;
                    Vector3D.TryParse(info[1], out Veloc);
                    Vector3D min;
                    Vector3D max;
                    Vector3D.TryParse(info[2], out min);
                    Vector3D.TryParse(info[3], out max);
                    BoundingBoxD Box = new BoundingBoxD(min, max);
                    long id;
                    long.TryParse(info[4], out id);
                    Target = new MyDetectedEntityInfo(id, "TargetedEnt", MyDetectedEntityType.LargeGrid, Pos, new MatrixD(), Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
                    if (Targets.Count > 0 && Target.EntityId != 0)
                    {
                        bool exist = false;
                        foreach (MyDetectedEntityInfo target in Targets)
                        {
                            if (target.EntityId == Target.EntityId) { exist = true; break; }
                        }
                        if (!exist) Targets.Add(Target);
                        if (exist)
                        {
                            foreach (MyDetectedEntityInfo target in Targets)
                            {
                                if (target.EntityId == id) { Targets.Remove(target); Targets.Add(Target); break; }
                            }
                        }
                    }
                    else if (Target.EntityId != 0) Targets.Add(Target);

                    for (int i = 2; i <= q; i++)
                    {
                        info = rinfo[i].Split('b');
                        Vector3D.TryParse(info[0], out Pos);
                        Vector3D.TryParse(info[1], out Veloc);
                        Vector3D.TryParse(info[2], out min);
                        Vector3D.TryParse(info[3], out max);
                        Box = new BoundingBoxD(min, max);
                        long.TryParse(info[4], out id);
                        MyDetectedEntityInfo Darget = new MyDetectedEntityInfo(id, "TargetedEnt", MyDetectedEntityType.LargeGrid, Pos, new MatrixD(), Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
                        if (Targets.Count > 0 && Darget.EntityId != 0)
                        {
                            bool exist = false;
                            foreach (MyDetectedEntityInfo target in Targets)
                            {
                                if (target.EntityId == Darget.EntityId) { exist = true; break; }
                            }
                            if (!exist) Targets.Add(Darget);
                            if (exist)
                            {
                                foreach (MyDetectedEntityInfo target in Targets)
                                {
                                    if (target.EntityId == id) { Targets.Remove(target); Targets.Add(Darget); break; }
                                }
                            }
                        }
                        else if (Darget.EntityId != 0) Targets.Add(Darget);
                    }
                }



                foreach (IMyTextPanel lcd in lcds_radar)
                {
                    if (lcd.CustomName != LCDN_original + " 6") continue;
                    enemy_missiles.Clear();
                    q = 0;
                    rinfo = lcd.CustomData.Split('@');
                    int.TryParse(rinfo[0], out q);

                    if (q > 0)
                    {
                        string[] info = rinfo[1].Split('b');
                        Vector3D Pos;
                        Vector3D.TryParse(info[0], out Pos);
                        Vector3D Veloc;
                        Vector3D.TryParse(info[1], out Veloc);
                        Vector3D min;
                        Vector3D max;
                        Vector3D.TryParse(info[2], out min);
                        Vector3D.TryParse(info[3], out max);
                        BoundingBoxD Box = new BoundingBoxD(min, max);
                        long id;
                        long.TryParse(info[4], out id);
                        Target = new MyDetectedEntityInfo(id, "TargetedEnt", MyDetectedEntityType.LargeGrid, Pos, new MatrixD(), Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
                        if (Targets.Count > 0 && Target.EntityId != 0)
                        {
                            bool exist = false;
                            foreach (MyDetectedEntityInfo target in Targets)
                            {
                                if (target.EntityId == Target.EntityId) { exist = true; break; }
                            }
                            if (!exist) Targets.Add(Target);
                            if (exist)
                            {
                                foreach (MyDetectedEntityInfo target in Targets)
                                {
                                    if (target.EntityId == id) { Targets.Remove(target); Targets.Add(Target); break; }
                                }
                            }
                        }
                        else if (Target.EntityId != 0) Targets.Add(Target);

                        for (int i = 2; i <= q; i++)
                        {
                            info = rinfo[i].Split('b');
                            Vector3D.TryParse(info[0], out Pos);
                            Vector3D.TryParse(info[1], out Veloc);
                            Vector3D.TryParse(info[2], out min);
                            Vector3D.TryParse(info[3], out max);
                            Box = new BoundingBoxD(min, max);
                            long.TryParse(info[4], out id);
                            MyDetectedEntityInfo Darget = new MyDetectedEntityInfo(id, "TargetedEnt", MyDetectedEntityType.LargeGrid, Pos, new MatrixD(), Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
                            if (Targets.Count > 0 && Darget.EntityId != 0)
                            {
                                bool exist = false;
                                foreach (MyDetectedEntityInfo target in Targets)
                                {
                                    if (target.EntityId == Darget.EntityId) { exist = true; break; }
                                }
                                if (!exist) Targets.Add(Darget);
                                if (exist)
                                {
                                    foreach (MyDetectedEntityInfo target in Targets)
                                    {
                                        if (target.EntityId == id) { Targets.Remove(target); Targets.Add(Darget); break; }
                                    }
                                }
                            }
                            else if (Darget.EntityId != 0) Targets.Add(Darget);
                        }
                    }

                    break;
                }



            }
            //
            //
        }


        void Radar_graphic_draw(IMyTextPanel LCD)
        {
            MySprite circle;
            double v1;
            double v2;
            float sizex;
            float sizey;
            double maxdist;
            double maxdist_coeff;

            IMyTextSurface surf;
            surf = LCD;
            surf.ContentType = ContentType.SCRIPT;
            surf.ScriptBackgroundColor = color_background;

            using (var frame = surf.DrawFrame())
            {
                // Рисунок РЛС

                string texture_radar = "CircleHollow";
                if (!Hollow_circles) texture_radar = "Circle";

                sizex = surf.SurfaceSize.Y / 1;
                sizey = surf.SurfaceSize.Y / 1;
                circle = new MySprite(SpriteType.TEXTURE, texture_radar, size: new Vector2(sizex, sizey), color: color_circle_3);
                circle.Position = surf.SurfaceSize / 2f;
                frame.Add(circle);
                sizex = surf.SurfaceSize.Y / 2;
                sizey = surf.SurfaceSize.Y / 2;
                circle = new MySprite(SpriteType.TEXTURE, texture_radar, size: new Vector2(sizex, sizey), color: color_circle_2);
                circle.Position = surf.SurfaceSize / 2f;
                frame.Add(circle);
                sizex = surf.SurfaceSize.Y / 3;
                sizey = surf.SurfaceSize.Y / 3;
                circle = new MySprite(SpriteType.TEXTURE, texture_radar, size: new Vector2(sizex, sizey), color: color_circle_1);
                circle.Position = surf.SurfaceSize / 2f;
                frame.Add(circle);

                MySprite sprite = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(target_size * 2, target_size * 2), color: color_center); 
                sprite.Position = new Vector2(surf.SurfaceSize.X / 2, surf.SurfaceSize.Y / 2);
                frame.Add(sprite);
                sprite = MySprite.CreateText(((int)(MaxRange / 1)).ToString(), "Monospace", color_text, 1f, TextAlignment.CENTER);
                sprite.Position = new Vector2(surf.SurfaceSize.X / 2, 20);
                frame.Add(sprite);
                sprite = MySprite.CreateText(((int)(MaxRange / 2)).ToString(), "Monospace", color_text, 1f, TextAlignment.CENTER);
                sprite.Position = new Vector2(surf.SurfaceSize.X / 2, surf.SurfaceSize.Y / 1.31f);
                frame.Add(sprite);
                sprite = MySprite.CreateText(((int)(MaxRange / 3)).ToString(), "Monospace", color_text, 1f, TextAlignment.CENTER);
                sprite.Position = new Vector2(surf.SurfaceSize.X / 2, surf.SurfaceSize.Y / 1.5f);
                frame.Add(sprite);




                // Рисование целей
                for (int i = 0; i < Targets.Count; i++)
                {
                    MyDetectedEntityInfo target = Targets[i];
                    maxdist = surf.SurfaceSize.X;
                    maxdist_coeff = maxdist / MaxRange;

                    Vector3D dir = target.Position - RC.GetPosition();
                    double vertdir = maxdist / 2;
                    vertdir -= Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(dir)) * dir.Length() / 2 * maxdist_coeff;
                    double hordir = maxdist / 2;
                    hordir += Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(dir)) * dir.Length() / 2 * maxdist_coeff;
                    Echo((Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(dir)) * dir.Length() * maxdist_coeff).ToString());
                    Echo(dir.Length().ToString());
                    if (vertdir < 0) vertdir = 0;
                    if (vertdir > maxdist) vertdir = maxdist;
                    if (hordir > maxdist) hordir = maxdist;
                    if (hordir < 0) hordir = 0;
                    //
                    Echo(vertdir.ToString());
                    Echo(hordir.ToString());

                    string texture = "Circle";
                    if (target.Type == MyDetectedEntityType.LargeGrid) texture = big_ship_icon;
                    else
                    if (target.Type == MyDetectedEntityType.SmallGrid) texture = small_ship_icon;
                    else
                    texture = unknown_icon;

                    //if (IsThisAMissile(target)) texture = missile_enemy_icon;

                    if (i == 0)
                    {
                        circle = new MySprite(SpriteType.TEXTURE, texture, size: new Vector2(target_size, target_size), color: color_target);
                        circle.Position = new Vector2((float)hordir, (float)vertdir);
                        frame.Add(circle);
                    }
                    else
                    if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies)
                    {
                        circle = new MySprite(SpriteType.TEXTURE, texture, size: new Vector2(target_size, target_size), color: color_enemy);
                        circle.Position = new Vector2((float)hordir, (float)vertdir);
                        frame.Add(circle);
                    }
                    else
                    if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Friends || target.Relationship == MyRelationsBetweenPlayerAndBlock.Owner)
                    {
                        circle = new MySprite(SpriteType.TEXTURE, texture, size: new Vector2(target_size, target_size), color: color_friend);
                        circle.Position = new Vector2((float)hordir, (float)vertdir);
                        frame.Add(circle);
                    }
                    else
                    {
                        circle = new MySprite(SpriteType.TEXTURE, texture, size: new Vector2(target_size, target_size), color: color_neutral);
                        circle.Position = new Vector2((float)hordir, (float)vertdir);
                        frame.Add(circle);
                    }
                }

                // Рисование вражеских ракет
                for (int i = 0; i < enemy_missiles.Count; i++)
                {
                    MyDetectedEntityInfo target = enemy_missiles[i];
                    maxdist = surf.SurfaceSize.X;
                    maxdist_coeff = maxdist / MaxRange;

                    Vector3D dir = target.Position - RC.GetPosition();
                    double vertdir = maxdist / 2;
                    vertdir -= Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(dir)) * dir.Length() / 2 * maxdist_coeff;
                    double hordir = maxdist / 2;
                    hordir += Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(dir)) * dir.Length() / 2 * maxdist_coeff;
                    Echo((Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(dir)) * dir.Length() * maxdist_coeff).ToString());
                    Echo(dir.Length().ToString());
                    if (vertdir < 0) vertdir = 0;
                    if (vertdir > maxdist) vertdir = maxdist;
                    if (hordir > maxdist) hordir = maxdist;
                    if (hordir < 0) hordir = 0;
                    //
                    Echo(vertdir.ToString());
                    Echo(hordir.ToString());

                    string texture = missile_enemy_icon;
 
                    circle = new MySprite(SpriteType.TEXTURE, texture, size: new Vector2(target_size, target_size), color: color_missile_enemy);
                    circle.Position = new Vector2((float)hordir, (float)vertdir);
                    frame.Add(circle);
                    
                }


                // Рисование собственных ракет
                for (int i = 0; i < missiles.Count; i++)
                {
                    IMyShipController missile = missiles[i];
                    maxdist = surf.SurfaceSize.X;
                    maxdist_coeff = maxdist / MaxRange;

                    Vector3D dir = missile.GetPosition() - RC.GetPosition();
                    if (dir.Length() < 200 || !missile.IsFunctional) continue;

                    double vertdir = maxdist / 2;
                    vertdir -= Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(dir)) * dir.Length() / 2 * maxdist_coeff;
                    double hordir = maxdist / 2;
                    hordir += Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(dir)) * dir.Length() / 2 * maxdist_coeff;
                    Echo((Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(dir)) * dir.Length() * maxdist_coeff).ToString());
                    Echo(dir.Length().ToString());
                    if (vertdir < 0) vertdir = 0;
                    if (vertdir > maxdist) vertdir = maxdist;
                    if (hordir > maxdist) hordir = maxdist;
                    if (hordir < 0) hordir = 0;
                    //
                    Echo(vertdir.ToString());
                    Echo(hordir.ToString());

                    string texture = missile_own_icon;

                     circle = new MySprite(SpriteType.TEXTURE, texture, size: new Vector2(target_size, target_size), color: color_missile);
                     circle.Position = new Vector2((float)hordir, (float)vertdir);
                     frame.Add(circle);
                    
                }
            }
        }


        void Missile_check_draw(IMyTextPanel LCD)
        {
            MySprite rocket;
            MySprite gui;
            double maxdist;
            double maxdist_coeff;

            IMyTextSurface surf;
            surf = LCD;
            surf.ContentType = ContentType.SCRIPT;
            surf.ScriptBackgroundColor = color_background;

            using (var frame = surf.DrawFrame())
            {
                gui = new MySprite(SpriteType.TEXTURE, "SquareHollow", size: new Vector2(surf.SurfaceSize.X / 2, surf.SurfaceSize.Y / 3), color: color_center);
                gui.Position = new Vector2(surf.SurfaceSize.X / 2, surf.SurfaceSize.Y / 1.8f);
                frame.Add(gui);
                gui = MySprite.CreateText("ГОТОВНОСТЬ РАКЕТ:", "Monospace", color_text, 0.6f, TextAlignment.CENTER);
                gui.Position = new Vector2(surf.SurfaceSize.X / 2, surf.SurfaceSize.Y / 2.2f);
                frame.Add(gui);

                float x_pos = surf.SurfaceSize.X / missile_projectors.Count + 1;
                float y_pos = surf.SurfaceSize.Y / 1;


                if (missile_projectors.Count > 1)
                {
                    for (int i = 0; i < missile_projectors.Count; i++)
                    {
                        IMyProjector missile_projector = missile_projectors[i];
                        bool ready = false;
                        bool fueled = false;
                        if (missile_projector.RemainingBlocks < 1) ready = true;
                        if (ready)
                        {
                            fueled = true;
                            GridTerminalSystem.GetBlocks(blocks);
                            foreach(IMyTerminalBlock tank in blocks)
                            {
                                if (!tank.CustomName.Contains(fuel_tanks_names)) continue;
                                bool brk = true;
                                foreach (IMyProjector prj in missile_projectors) if ((prj.GetPosition() - tank.GetPosition()).Length() < 20) brk = false;
                                if (brk) continue;
                                IMyGasTank gas = tank as IMyGasTank;
                                if (gas.Capacity != gas.FilledRatio) { fueled = false; break; }
                            }
                        }
                        Color color;
                        if (ready) color = color_ready_missile;
                        else color = color_unready_missile;
                        rocket = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(surf.SurfaceSize.X / 15, surf.SurfaceSize.Y / 5), color: color);
                        rocket.Position = new Vector2((surf.SurfaceSize.X / missile_projectors.Count + 1) * i + 50, y_pos / 1f);
                        frame.Add(rocket);
                        if (fueled) color = color_fueled_missile;
                        else color = color_unfueled_missile;
                        rocket = new MySprite(SpriteType.TEXTURE, "Circle", size: new Vector2(surf.SurfaceSize.X / 17, surf.SurfaceSize.Y / 3), color: color);
                        rocket.Position = new Vector2((surf.SurfaceSize.X / missile_projectors.Count + 1) * i + 50, y_pos / 0.8f);
                        frame.Add(rocket);
                        gui = MySprite.CreateText(missile_projector.CustomName.Remove(0, projector_names.Length), "Monospace", color_text, 0.3f, TextAlignment.CENTER);
                        gui.Position = new Vector2((surf.SurfaceSize.X / missile_projectors.Count + 1) * i + 50, y_pos / 1.2f);
                        frame.Add(gui);
                    }
                }

                //rocket = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(target_size * 2, target_size * 2), color: color_center);
                //frame.Add(rocket);
            }
        }


        bool IsThisAMissile(MyDetectedEntityInfo target, double missile_size, double danger_speed, double danger_range, double target_coeff)
        {
            // missile_size = 10
            // target_speed = 50
            // danger_range = 1000
            // target_coeff = 2 

            if (target.BoundingBox.Volume < missile_size || target.Velocity.Length() < danger_speed) return false;
            Vector3D dir = target.Position - RC.GetPosition();
            if (dir.Length() > danger_range) return false;
            Vector3D myvel = RC.GetShipVelocities().LinearVelocity;
            Vector3D Sv = dir + myvel * dir.Length();
            Sv = -Vector3D.Reflect(Sv, Vector3D.Normalize(target.Velocity));
            if (Vector3D.Dot(Sv, Vector3D.Normalize(dir)) > 0) return false;
            Vector3D tg = Vector3D.Reject(target.Velocity, Vector3D.Normalize(Sv));
            if (tg.Length() < target_coeff && tg.Length() > 0)
            return true;
            else
            return false;
        }