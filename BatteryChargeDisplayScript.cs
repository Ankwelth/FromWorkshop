#region program
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        bool flash = false;

        int c = 0;
        int flashtime = 10;

        public void Main(string str, UpdateType updateType)
        {
            float remaining = 0;
            float cod = 0;
            float prcnt = 0;
            ScanForBatteries(out remaining, out cod, out prcnt);


            var trem = CalculateTime(remaining);
            string pon = "Fully depleted in";
            if (cod > .5) { pon = "Fully recharged in"; }
            string outs = pon + "\n" + trem.Key + " " + trem.Value.ToString();

            Echo(outs);

            int screens = 0;

            blocks.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, (x => x is IMyTextSurfaceProvider && x.IsSameConstructAs(Me)));


            foreach (IMyTerminalBlock aa in blocks)
            {
                var a = aa as IMyTextSurfaceProvider;

                if (a == null || (a != null && a.SurfaceCount <= 0))
                {
                    continue;
                }

                string data = aa.CustomData.ToLower();
                int index = 0;
                if (data.Contains("battery:"))
                {
                    var pos = data.IndexOf("battery:") + ("battery:".Length);

                    var endpos = data.IndexOf(";", pos);
                    int.TryParse(data.Substring(pos, endpos - pos), out index);
                    index = Math.Min(a.SurfaceCount - 1, Math.Max(0, index));
                }else if (data.Contains("battery;"))
                {
                    index = 0;
                }
                else
                {
                    continue;
                }
                var surface = a.GetSurface(index);
                if (surface == null)
                {
                    continue;
                }
                DrawFrame(surface, outs, prcnt);
                screens++;
            }
            Echo("Updated " + screens.ToString() + " displays.");
            if (c > flashtime)
            {
                c = 0;
                flash = !flash;
            }
            else
            {
                if ((updateType & UpdateType.Update100) == UpdateType.Update100)
                {
                    c = c + 100;
                }
                else if ((updateType & UpdateType.Update10) == UpdateType.Update10)
                {
                    c = c + 10;
                }
                else
                {
                    c++;
                }
            }
        }

        private void ScanForBatteries(out float remaining, out float cod, out float prcnt)
        {
            batteries.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, (x => x.IsSameConstructAs(Me)));
            float max = 0, cur = 0;
            int tot = 0;
            float dis = 0, inp = 0;
            int charging = 0;
            float lowrem = 0, highrem = 0;
            int chtot = 0;
            float CommonRechargeMult = 0.8f;
            foreach (IMyBatteryBlock battery in batteries)
            {

                cur += battery.CurrentStoredPower;
                max += battery.MaxStoredPower;
                float flow = battery.CurrentInput * (CommonRechargeMult) - battery.CurrentOutput; // Retrieve Multiplier?
                float _rem = GetRemaining(battery.MaxStoredPower,battery.CurrentStoredPower, flow); // Multiplier is also avaiable, however not sure how to get it.
                if (flow > 0)
                {
                    charging++;
                    highrem = Math.Max(highrem, _rem);
                    chtot++;
                }
                else if (flow < 0)
                {
                    lowrem = Math.Max(lowrem, _rem);
                    chtot++;
                }

                tot++;

            }

            float avgcur = cur / tot, avgmax = max / tot;
            cod = charging / (float)chtot;
            float powerflow = (inp - dis);
            remaining = (cod > .5) ? highrem : lowrem;
            prcnt = avgcur / avgmax;
        }

        private float GetRemaining(float max, float cur, float powerflow)
        {
            float remaining;
            if (powerflow > 0)
            {
                remaining = (((max - cur) / (Math.Abs(powerflow) / 3600f)));
            }
            else if (powerflow < 0)
            {
                remaining = (cur / (Math.Abs(powerflow) / 3600f));
            }
            else
            {
                remaining = 0;
            }
            return (remaining);
        }

        private void DrawFrame(IMyTextSurface surface, string value, float percent)
        {
            int i = 0;
            MySpriteDrawFrame frame = surface.DrawFrame();
            var builder = new StringBuilder(value);
            float fontScale = 1f;
            for (fontScale = 1f; fontScale >= 0.1f; fontScale -= .1f)
            {
                if (surface.MeasureStringInPixels(builder, "Monospace", fontScale).X < surface.SurfaceSize.X)
                {
                    break;
                }
            }
            if (flash)
            {
                MySprite empty = MySprite.CreateText("_", "Monospace", Color.Transparent);
                frame.Add(empty);
            }

            var lineheight = Math.Max((surface.SurfaceSize.X * 0.05f), (surface.MeasureStringInPixels(new StringBuilder("|"), "Monospace", fontScale).Y) + 5) * 2;
            Vector2 SurfaceZero = (surface.TextureSize - surface.SurfaceSize) / 2;
            var sp = SurfaceZero + (surface.SurfaceSize * 0.5f) - new Vector2(surface.SurfaceSize.X * 0.375f, 0);
            sp.Y = (SurfaceZero.Y + (surface.SurfaceSize.Y / 2) - (((lineheight / 2) * 1) / 2)) + (i * (lineheight));


            var lineheightvec = new Vector2(0, lineheight / 2) * .5f;
            var progouter = MySprite.CreateSprite("SquareSimple", sp - lineheightvec, new Vector2((surface.SurfaceSize.X * 0.75f) * percent, lineheight / 2.5f));
            var proginner = MySprite.CreateSprite("SquareSimple", sp - lineheightvec, new Vector2((surface.SurfaceSize.X * 0.75f), lineheight / 2.5f));
            proginner.Alignment = TextAlignment.LEFT;
            progouter.Alignment = TextAlignment.LEFT;
            proginner.Color = new Color(1f, 1f, 1f, .25f);



            string text = value;
            var txt = MySprite.CreateText(text, "Monospace", surface.ScriptForegroundColor, fontScale, TextAlignment.CENTER);
            Vector2 sp2 = new Vector2((surface.SurfaceSize.X / 2), sp.Y);
            txt.Position = sp2;
            //txt.Position = new Vector2(0, lineheight * 0.75f);
            if (percent > 1 - 0.8)
            {
                progouter.Color = Color.Green;
            }
            else if (percent > 1 - 0.89375)
            {
                progouter.Color = Color.Orange;
            }
            else if (percent > 1 - 0.9875)
            {
                progouter.Color = Color.Red;
            }
            else
            {
                if (flash)
                    progouter.Color = Color.DarkRed;
                else
                    progouter.Color = Color.Red;
            }

            frame.Add(proginner);
            frame.Add(progouter);

            frame.Add(txt);
            //frame.Add(MySprite.CreateText(surface.TextureSize.ToString() + "\n" + surface.SurfaceSize.ToString(), "Monospaced", Color.White));
            frame.Dispose();

        }

        public enum TimeType
        {
            Seconds, Minutes, Hours, Days, Years
        }

        public KeyValuePair<double, TimeType> CalculateTime(float TimeInSeconds)
        {
            float outsn = TimeInSeconds;
            TimeType type = TimeType.Seconds;
            if (outsn > 60)
            {
                outsn = outsn / 60f;

                type = TimeType.Minutes;
                if (TimeInSeconds / 60 > 60)
                {

                    outsn = outsn / 60f;

                    type = TimeType.Hours;
                    if (TimeInSeconds / 60 / 60 > 24)
                    {

                        outsn = outsn / 24f;

                        type = TimeType.Days;
                        if (TimeInSeconds / 60 / 60 / 24 > 365)
                        {

                            outsn = outsn / 365f;

                            type = TimeType.Years;

                        }
                    }
                }
            }
            return (new KeyValuePair<double, TimeType>(Math.Round(outsn), type));
        }

        private struct MathInfo
        {

        }

        #endregion