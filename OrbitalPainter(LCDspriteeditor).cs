
		public static class Config
		{
			public static readonly string DefaultTextPanelName = "paint-panel"; // Canvas display
			public static readonly string OutputTextPanelName = "paint-aux"; // Secondary display (color tool, etc)
			public static readonly int MenuScreenIndex = 0; // index of cockpit screen in case paint-panel is absent
			public static readonly int OutputScreenIndex = 2; // index of cockpit screen in case paint-aux is absent
		}

		static class Variables
		{
			static Dictionary<string, object> v = new Dictionary<string, object> {
				{ "float", new Variable<float> { value = 10, parser = s => float.Parse(s) } },
				{ "int", new Variable<int> { value = 5, parser = s => int.Parse(s) } },
				{ "planned-reset", new Variable<bool> { value = false, parser = s => s == "true" } },
				{ "rotate-when-copy", new Variable<bool> { value = false, parser = s => s == "true" } }
			};
			public static void Set(string key, string value) { (v[key] as ISettable).Set(value); }
			public static void Set<T>(string key, T value) { (v[key] as ISettable).Set(value); }
			public static T Get<T>(string key) { return (v[key] as ISettable).Get<T>(); }
			public interface ISettable
			{
				void Set(string v);
				T1 Get<T1>();
				void Set<T1>(T1 v);
			}
			public class Variable<T> : ISettable
			{
				public T value;
				public Func<string, T> parser;
				public void Set(string v) { value = parser(v); }
				public void Set<T1>(T1 v) { value = (T)(object)v; }
				public T1 Get<T1>() { return (T1)(object)value; }
			}
		}

		class Toggle
		{
			static Toggle inst;
			Toggle() { }
			Action<string> onToggleStateChangeHandler;
			Dictionary<string, bool> sw;
			Toggle(Dictionary<string, bool> switches, Action<string> handler)
			{
				onToggleStateChangeHandler = handler;
				sw = switches;
			}

			public static Toggle C => inst;

			public static void Init(Dictionary<string, bool> switches, Action<string> handler)
			{
				if (inst == null)
					inst = new Toggle(switches, handler);
			}

			public void Set(string key, bool value)
			{
				if (sw[key] != value)
					Invert(key);
			}
			public void Invert(string key)
			{
				sw[key] = !sw[key];
				onToggleStateChangeHandler(key);
			}
			public bool Check(string key)
			{
				return sw[key];
			}
		}

		bool pendingInitSequence;
		CommandRegistry commandRegistry;
		public class CommandRegistry
		{
			Dictionary<string, Action<string[]>> commands;
			public CommandRegistry(Dictionary<string, Action<string[]>> commands)
			{
				this.commands = commands;
			}
			public void RunCommand(string id, string[] cmdParts)
			{
				this.commands[id].Invoke(cmdParts);
			}
		}

		int runCount;
		bool StartOfTick(string arg)
		{
			runCount++;
			Echo("Run count: " + runCount);

			if (pendingInitSequence && string.IsNullOrEmpty(arg))
			{
				pendingInitSequence = false;
				arg = string.Join(",", Me.CustomData.Trim('\n').Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(s => !s.StartsWith("//")).Select(s => "[" + s + "]"));
				//SendFeedback(DateTime.Now.ToString("hh:mm:ss") + ": " + Me.CustomName + ": hello there, I've just got initialized. Have a great day and may the profitsssss be with you!", "", true);
			}

			if (!string.IsNullOrEmpty(arg) && arg.Contains(":"))
			{
				var commands = arg.Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('[', ']')).ToList();
				foreach (var c in commands)
				{
					string[] cmdParts = c.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
					if (cmdParts[0] == "command")
					{
						this.commandRegistry.RunCommand(cmdParts[1], cmdParts);
					}
					if (cmdParts[0] == "toggle")
					{
						Toggle.C.Invert(cmdParts[1]);
						E.DebugLog(string.Format("Switching '{0}' to state '{1}'", cmdParts[1], Toggle.C.Check(cmdParts[1])));
					}
				}
				return true;
			}

			return false;
		}

		void EndOfTick()
		{
			Scheduler.C.HandleTick();
			boundPanelsManager.HandleTick();
			//FlushFeedbackBuffer();
		}

		IMyTextSurface surfaceMain;
		IMyTextSurface surfaceAux;
		IMyShipToolBase triggerTool;
		//IMyShipWelder signalWelder;

		void Ctor()
		{
			E.Init(Echo, GridTerminalSystem, Me);

			Action<PersistentState> onLoadComplete = (pstate) =>
			{
				if (!string.IsNullOrEmpty(Me.CustomData))
					pendingInitSequence = true;
				fileHandler = new FileHandler(shellHandler, pstate.storageEntries.Select(x => x.Key).ToList(), pstate, IGC);
			};

			//Me.CustomData = Storage;
			stateWrapper = new StateWrapper(s => Storage = s, onLoadComplete);
			//stateWrapper = new StateWrapper(s => { Storage = s; Me.CustomData = s; }, onLoadComplete);
			if (!stateWrapper.TryLoad(Storage, Runtime))
			{
				E.Echo("State load failed, clearing Storage now");
				stateWrapper.Save();
				Runtime.UpdateFrequency = UpdateFrequency.None;
			}

			List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
			GridTerminalSystem.GetBlockGroups(groups);
			var gr = groups.Where(g => {
				List<IMyProgrammableBlock> pbs = new List<IMyProgrammableBlock>();
				g.GetBlocksOfType(pbs);
				return pbs.Contains(Me);
			}).FirstOrDefault();
			if (gr == null)
			{
				Runtime.UpdateFrequency = UpdateFrequency.None;
				E.Echo("Can't find hardware group containing this PB, stopping now.");
			}
			else
			{
				List<IMyTextPanel> panels = new List<IMyTextPanel>();
				gr.GetBlocksOfType(panels);
				surfaceMain = panels.FirstOrDefault(p => p.CustomName == Config.DefaultTextPanelName);
				surfaceAux = panels.FirstOrDefault(p => p.CustomName == Config.OutputTextPanelName);
				var anyLcd = panels.FirstOrDefault();

				List<IMyShipController> ctrls = new List<IMyShipController>();
				gr.GetBlocksOfType(ctrls, x => x.IsSameConstructAs(Me));

				UserCtrlListener = new InputListener(ctrls, () => runCount);

				IMyTextSurfaceProvider c = UserCtrlListener.GetControlledCockpit() as IMyCockpit;
				if (c == null)
					c = ctrls.Where(ct => ct is IMyCockpit).FirstOrDefault() as IMyCockpit;

				// Priorities: Named screen ?? just any screen ?? cockpit screen.
				if (surfaceMain == null)
				{
					surfaceMain = anyLcd ?? GetCockpitSurface(Config.MenuScreenIndex, c);
				}
				else
				{
					E.Echo($"Main display found: {((IMyTerminalBlock)surfaceMain).CustomName}");
				}

				if (surfaceMain != null)
				{
					surfaceMain.ContentType = ContentType.TEXT_AND_IMAGE;
				}
				else
				{
					throw new Exception("Need something to write to. Surface for menu was not found. Group this PB with either cockpit or LCD.");
				}

				// Priorities: Named screen ?? cockpit screen.
				if (surfaceAux == null)
				{
					surfaceAux = GetCockpitSurface(Config.OutputScreenIndex, c);
				}
				if (surfaceAux != null)
				{
					surfaceAux.ContentType = ContentType.TEXT_AND_IMAGE;
				}

				boundPanelsManager = new BoundPanelsManager(GridTerminalSystem, stateWrapper.PState);

				var myShipTools = new List<IMyShipToolBase>();
				gr.GetBlocksOfType(myShipTools);
				triggerTool = myShipTools.FirstOrDefault();

				shellHandler = new ShellHandler(IGC, "o_painter", surfaceMain.TextureSize);
				shellHandler.AddHandler("GetCurrentFrameSprites", () => {
					return canvas?.GetCurrentFrameSprites();
				});
			}

			Toggle.Init(new Dictionary<string, bool>
				{
					{ "take-control", true },
					{ "s", false },
					{ "play-repeat", false },
					{ "play-repeat-slow", false },
					{ "generate-frames", false },
					{ "prev-onion", true },
					{ "auto", false }
				},
					key =>
					{
						switch (key)
						{
							case "take-control":
								if (Toggle.C.Check("take-control"))
								{
									UserCtrlListener.GetControlledCockpit()?.SetValueBool("ControlGyros", false);
									//UserCtrlTest.GetControlledCockpit()?.SetValueBool("ControlThrusters", false);
								}

								else
								{
									UserCtrlListener.GetControlledCockpit()?.SetValueBool("ControlGyros", true);
									//UserCtrlTest.GetControlledCockpit()?.SetValueBool("ControlThrusters", true);
								}
								break;
							case "s":
								viewProvider.SwapView();
								break;
							case "play-repeat":
								Scheduler.C.Reset();
								Toggle.C.Set("play-repeat-slow", false);
								Toggle.C.Set("generate-frames", false);
								if (Toggle.C.Check("play-repeat"))
								{
									Scheduler.C.After(50).RepeatWhile(() => true).RunCmd(() => canvas.NextFrame());
								}
								break;
							case "play-repeat-slow":
								Scheduler.C.Reset();
								Toggle.C.Set("play-repeat", false);
								Toggle.C.Set("generate-frames", false);
								if (Toggle.C.Check("play-repeat-slow"))
								{
									Scheduler.C.After(2000).RepeatWhile(() => true).RunCmd(() => canvas.NextFrame());
								}
								break;
							case "generate-frames":
								Scheduler.C.Reset();
								Toggle.C.Set("play-repeat-slow", false);
								Toggle.C.Set("play-repeat", false);
								if (Toggle.C.Check("generate-frames"))
								{
									Scheduler.C.After(2000).RepeatWhile(() => true).RunCmd(() => canvas.CreateFrame("frame"));
								}
								break;

						}
					}
				);

			this.commandRegistry = new CommandRegistry(
				new Dictionary<string, Action<string[]>>
					{
						{
							"set-value", (parts) => Variables.Set(parts[2], parts[3])
						},
						{
							"save", (parts) => Save()
						},
						{
							"export", (parts) => Me.GetSurface(0).WriteText(SerializeItem(surfaceMain.TextureSize, canvas.GetFrames()))
						},
						{
							"bind-panel", (parts) => {
								if (!stateWrapper.PState.exportToPanels.ContainsKey(parts[2]))
									stateWrapper.PState.exportToPanels.Add(parts[2], parts[3]);
							}
						},
						{
							"start-panel", (parts) => {
								boundPanelsManager.StartPanelPlayback(parts[2], parts[3], int.Parse(parts[4]));
							}
						},
						{
							"stop-panel", (parts) => {
								boundPanelsManager.StopPanelPlayback(parts[2]);
							}
						},
						{
							"fopen", (parts) => canvas.LoadOrCreate(stateWrapper.PState, parts[2])
						},
						{
							"get-available-screens", (parts) => {
								var pnls = new List<IMyTextPanel>();
								GridTerminalSystem.GetBlocksOfType(pnls);
								IGC.SendUnicastMessage(long.Parse(parts[2]),
										$"menucommand.get-commands.reply:{ string.Join(":", parts.Take(3)) }",
												pnls.Select(n => new MyTuple<string, string>("Bind to " + n.CustomName, 
														$"command:start-panel:{n.CustomName}:{canvas.CurrentItemName}:2")).ToImmutableArray());
							}
						},
						{
							"get-available-files", (parts) => {
								IGC.SendUnicastMessage(long.Parse(parts[2]),
										$"menucommand.get-commands.reply:{ string.Join(":", parts.Take(3)) }",
												stateWrapper.PState.storageEntries.Keys.Select(n => new MyTuple<string, string>(n,
														$"command:fopen:{n}")).ToImmutableArray());
							}
						}

					}
				);
		}

		IMyTextSurface GetCockpitSurface(int screenIndex, IMyTextSurfaceProvider c)
		{
			if (c != null)
			{
				if (c.SurfaceCount > screenIndex)
				{
					return c.GetSurface(screenIndex);
				}
			}

			return null;
		}

		static void AddUniqueItem<T>(T item, IList<T> c) where T : class
		{
			if ((item != null) && !c.Contains(item))
				c.Add(item);
		}

		T GetSingleBlock<T>(Func<IMyTerminalBlock, bool> pred) where T : class
		{
			var blocks = new List<IMyTerminalBlock>();
			GridTerminalSystem.GetBlocksOfType(blocks, b => ((b is T) && pred(b)));
			return blocks.First() as T;
		}


		public Program()
		{
			Runtime.UpdateFrequency = UpdateFrequency.Update1;

			Ctor();
		}

		List<MyIGCMessage> uniMsgs = new List<MyIGCMessage>();
		void Main(string param, UpdateType updateType)
		{
			if (StartOfTick(param))
				return;

			uniMsgs.Clear();
			while (IGC.UnicastListener.HasPendingMessage)
			{
				uniMsgs.Add(IGC.UnicastListener.AcceptMessage());
			}
			shellHandler.HandleRequests(uniMsgs);
			fileHandler?.HandleIGC(uniMsgs);

			if (!stateWrapper.HumbleLoad())
			{
				E.Echo("Loading sprite data..." + (new[] { "\\", "|", "/", "--" }[runCount % 4]));
				return;
			}

			if (canvas == null)
			{
				canvas = new Canvas(stateWrapper.PState, () => UserCtrlListener.GetControlledCockpit()?.CustomData);
				viewProvider = new ViewProvider(canvas, surfaceMain.TextureSize, () => runCount,
						() => (float)canvas.GetCurrentFrameSprites().Count / Runtime.MaxInstructionCount, UserCtrlListener);
			}

			bool drawPointer = Toggle.C.Check("take-control") && (UserCtrlListener.GetControlledCockpit() != null);
			if (!Toggle.C.Check("play-repeat") && !drawPointer)
			{
				EndOfTick();
				return;
			}

			var surface = Toggle.C.Check("s") ? surfaceAux : surfaceMain;

			if (Variables.Get<bool>("planned-reset"))
			{
				Variables.Set("planned-reset", false);
				stateWrapper.ClearPersistentState();
				canvas = new Canvas(stateWrapper.PState, () => UserCtrlListener.GetControlledCockpit()?.CustomData);
				viewProvider = new ViewProvider(canvas, surface.TextureSize, () => runCount, 
						() => (float)canvas.GetCurrentFrameSprites().Count / Runtime.MaxInstructionCount, UserCtrlListener);
			}

			surface.ContentType = ContentType.SCRIPT;
			surface.ScriptBackgroundColor = Color.Transparent;
			surface.Script = "";
			Runtime.UpdateFrequency = UpdateFrequency.Update1;

			if (drawPointer)
			{
				viewProvider.MouseDown = UserCtrlListener.CheckKeyDown("q");
				if (triggerTool != null)
				{
					viewProvider.MouseDown = viewProvider.MouseDown || triggerTool.IsActivated;
				}

				//viewProvider.MouseDown = viewProvider.MouseDown || UserCtrlListener.CheckKeyDown("e", UserCtrlTest.GetControlledCockpit());

				viewProvider.DoubleTap = UserCtrlListener.CheckDoubleTap("e");

				if (UserCtrlListener.CheckKeyDown("w"))
				{
					canvas.CurrentSize++;
				}
				if (UserCtrlListener.CheckKeyDown("s"))
				{
					canvas.CurrentSize--;
					if (canvas.CurrentSize < 0)
						canvas.CurrentSize = 0;
				}

				if (UserCtrlListener.CheckKeyDown("a"))
				{
					canvas.CurrentRotation -= 0.05f;
				}
				if (UserCtrlListener.CheckKeyDown("d"))
				{
					canvas.CurrentRotation += 0.05f;
				}

				if (UserCtrlListener.CheckKeyDown("spacebar"))
				{
					if (canvas.CurrentAlpha < 255)
						canvas.CurrentAlpha++;

				}
				if (UserCtrlListener.CheckKeyDown("c"))
				{
					if (canvas.CurrentAlpha > 1)
						canvas.CurrentAlpha--;
				}

				var r = UserCtrlListener.GetRot();
				UserCtrlListener.Cumulative += r;
			}

			var adj = UserCtrlListener.Cumulative;
			float maxF = 500;
			Func<float, float> constrain = x =>
			{
				x = Math.Min(Math.Abs(x), maxF) * Math.Sign(x);
				return x;
			};

			adj.X = constrain(adj.X);
			adj.Y = constrain(adj.Y);

			UserCtrlListener.Cumulative = adj;
			adj = adj / maxF;

			//////////////

			if (((adj.Y >= 1) || (adj.Y <= -1)) && !viewProvider.MouseDown)
			{
				Toggle.C.Invert("s");
				UserCtrlListener.Cumulative.Y = -Math.Sign(adj.Y) * (maxF - 10);
				return;
			}

			using (var frame = surface.DrawFrame())
			{
				Vector2 cursorPosition = surface.TextureSize / 2f + (new Vector2(10f) / 2f) + new Vector2(surface.TextureSize.X / 2 * adj.Y, surface.TextureSize.Y / 2 * adj.X);

				if (viewProvider.MainView)
				{
					if (Toggle.C.Check("prev-onion"))
					{
						var prefFrSprites = canvas.GetPrevFrameSprites();
						if (prefFrSprites != null)
							frame.AddRange(prefFrSprites.Select(x => { var c = x.Color.Value; c.A = 50; x.Color = c; return x; }));
					}
					frame.AddRange(canvas.GetCurrentFrameSprites());
				}

				viewProvider.CheckHover(cursorPosition);
				frame.AddRange(viewProvider.Build());

				if (drawPointer)
				{
					var cursor = viewProvider.GetCursor(cursorPosition);
					if (viewProvider.MainView)
					{
						canvas.HandleUserAction(viewProvider.MouseDown, UserCtrlListener.KeyReleased("q"), 
							viewProvider.DoubleTap && !viewProvider.DoubleTapWasHandled, ref cursor, UserCtrlListener.GetRot() / 2f);
					}

					frame.Add(cursor);
					/*
					string txct = "";
					txct += "Roll: " + UserCtrlListener.GetRoll().ToString("f2") + "\n";
					txct += "MouseDown: " + viewProvider.MouseDown + "\n";
					txct += "Q down: " + UserCtrlListener.CheckKeyDown("q") + "\n";
					var dbgT = MySprite.CreateText(txct, "Debug", Color.White, 1f);
					dbgT.Position = surfaceMain.TextureSize / 2f;
					frame.Add(dbgT);*/
				}
			}

			surface.ContentType = ContentType.TEXT_AND_IMAGE;
			surface.ContentType = ContentType.SCRIPT;


			/////////////

			EndOfTick();

			E.Echo($"Storage state bytes: {stateWrapper.GetStorageSize()}");

			E.Echo($"CurrentInstructionCount: {Runtime.CurrentInstructionCount}");
			E.Echo("Processed in " + Runtime.LastRunTimeMs.ToString("f3") + " ms");
			lastRunInstructionCount = Runtime.CurrentInstructionCount;
		}

		int lastRunInstructionCount;

		ViewProvider viewProvider;
		class ViewProvider
		{
			UiNode root;
			UiNode view1;
			UiNode view2;
			Canvas canvas;
			MySprite cursor;

			public float Upscale { get; private set; }
			public bool MainView { get; private set; }
			public bool MouseDown { get; set; }
			public bool DoubleTap { get; set; }
			public bool DoubleTapWasHandled { get; private set; }

			public ViewProvider(Canvas canvas, Vector2 textureSize, Func<int> tickGetter, 
					Func<float> progressBarFactorGetter, InputListener inputListener)
			{
				MainView = true;
				this.canvas = canvas;
				Upscale = textureSize.Y > 300 ? 1.5f : 1f;

				view1 = new UiNode(null);
				var mainCa = new ClickableArea(textureSize / 2, textureSize);
				var mainNode = view1.AddChild(new UiNode(mainCa));

				CreateModalWindow(mainNode, textureSize / 2f, textureSize / 4f);

				mainCa.OnHover = (p) =>
				{
					cursor = canvas.GetCursor();
				};

				Vector2 toolPreviewSize = new Vector2(100, 100);
				var toolPreviewSpr = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(0.5f, 0.5f, 0.5f, 0.1f));
				ClickableArea toolPreview = new ClickableArea(new Vector2(textureSize.X / 2, textureSize.Y / 2), textureSize / 4, toolPreviewSpr);
				toolPreview.Visible = false;
				var boxAreaNode = mainNode.AddChild(new UiNode(toolPreview));

				//int xOffset = 10;

				Func<string, Vector2, string, Action, UiNode> createBottomControl = (sprId, pos, caption, clickHandler) =>
				{
					var diag = new Vector2(20, 20) * Upscale;
					var selectBoxSpr = new MySprite(SpriteType.TEXTURE, sprId, color: Color.White);
					ClickableArea clickableAreaBottom = new ClickableArea(pos, diag, selectBoxSpr, clickHandler);
					var boxAreaC = new UiNode(clickableAreaBottom);
					clickableAreaBottom.OnMouseIn += () => clickableAreaBottom.Transform(spr => {
						spr.Size = diag * 1.3f;
						return spr;
					});
					clickableAreaBottom.OnMouseOut += () => clickableAreaBottom.Transform(spr => {
						spr.Size = diag;
						return spr;
					});
					if (!string.IsNullOrEmpty(caption))
					{
						clickableAreaBottom.AddCaption(Vector2.UnitY * 24 * Upscale, "", Color.White, 0.5f * Upscale);
						clickableAreaBottom.OnMouseIn += () => clickableAreaBottom.TransformCaption(cap => {
							cap.Text = caption;
							return cap;
						});
						clickableAreaBottom.OnMouseOut += () => clickableAreaBottom.TransformCaption(cap => {
							cap.Text = "";
							return cap;
						});
					}
					return boxAreaC;
				};

				List<Func<Vector2, UiNode>> fs = new List<Func<Vector2, UiNode>>();

				var wrapperBottom = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(.1f, .1f, .1f, 0.95f));
				wrapperBottom.Position = new Vector2(textureSize.X * 0.4f, textureSize.Y * 0.92f);
				wrapperBottom.Size = new Vector2(textureSize.X * 0.8f, textureSize.Y * 0.16f);
				var activatorBottom = new ClickableArea(wrapperBottom.Position.Value + Vector2.UnitY * wrapperBottom.Size.Value.Y * 0.95f, wrapperBottom.Size.Value);

				int controlInterval = (int)(30 * Upscale);
				var rightBottomMargin = wrapperBottom.Size.Value.X / 2 * 0.82f;

				

				fs.Add(overCenter => createBottomControl("Cross", new Vector2(-rightBottomMargin, 0) + overCenter, "Clear", () => {
					ShowModalWindow("are u sure", () => Variables.Set("planned-reset", true));
				}));

				fs.Add(overCenter => {
					var bc = createBottomControl(@"Textures\FactionLogo\Traders\TraderIcon_3.dds",
							new Vector2(-rightBottomMargin + controlInterval, 0) + overCenter, "Tool",
							() => canvas.NextTool());
					((ClickableArea)bc.Payload).OnHover += (p) =>
					{
						((ClickableArea)bc.Payload).TransformCaption(c => { c.Text = canvas._currentTool.ToString(); return c; });
					};
					return bc;
				});

				string help = "Draw and pick colors: Q or gatling fire\n" +
					"Activate buttons and clone color: double tap E\n" +
					"Rotate brush: A/D\n" +
					"Stretch brush: A/D when on brush panel\n" +
					"Size: W/S\n" +
					"Opacity: C/SPACE\n" +
					$"External screen names: \n  {Config.DefaultTextPanelName}\n  {Config.OutputTextPanelName}\n" +
					"Image to LCD command arg: \n  'command:bind-panel:LCD:FRAME'";
				fs.Add(overCenter => {
					var bc = createBottomControl(@"Textures\FactionLogo\Traders\TraderIcon_5.dds", new Vector2(rightBottomMargin - controlInterval, 0) + overCenter, help, null);
					((ClickableArea)bc.Payload).TransformCaption(c =>
					{
						c.Position = new Vector2(textureSize.X * 0.05f, textureSize.Y * 0.1f);
						c.Scale = 0.5f * Upscale;
						c.Color = Color.White;
						return c;
					});
					return bc;
				});

				fs.Add(overCenter => {
					var bc = createBottomControl("CircleHollow", new Vector2(rightBottomMargin, 0) + overCenter, "Semi/Auto", () => {
							Toggle.C.Invert("auto");
						});
					((ClickableArea)bc.Payload).OnHover += (p) =>
					{
						((ClickableArea)bc.Payload).TransformCaption(c => { c.Text = Toggle.C.Check("auto") ? "Auto" : "Semi"; return c; });
					};
					return bc;
				});

				int barWidth = (int)(wrapperBottom.Size.Value.X * 0.3);
				var compelxityBar = new ProgressBar(wrapperBottom.Position.Value, new Vector2(barWidth, wrapperBottom.Size.Value.Y * 0.5f), 
						progressBarFactorGetter, "complexity");
				//compelxityBar.SetProgress(runtime.CurrentInstructionCount / 50000f);
				var barSprites = new ProgressBarUiElement(compelxityBar);
				var barNode = new UiNode(barSprites);
				fs.Add((x) => barNode);

				CreateAutohideConrolGroup(mainNode, wrapperBottom, activatorBottom, fs);


				var wrapperRight = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(.1f, .1f, .1f, 0.95f));
				fs = new List<Func<Vector2, UiNode>>();
				float distanceFromRightB = textureSize.X * (0.05f * Upscale);
				wrapperRight.Position = new Vector2(textureSize.X - distanceFromRightB, textureSize.Y / 2);
				wrapperRight.Size = new Vector2(distanceFromRightB * 2, textureSize.Y);
				var activatorRight = new ClickableArea(wrapperRight.Position.Value + Vector2.UnitX * (distanceFromRightB * 2 - 15), wrapperRight.Size.Value);

				int yOffset = 0;
				var toolSize = new Vector2(20, 20);
				foreach (int brushID in Enumerable.Range(0, canvas.BrushIDs.Length))
				{
					int offset = yOffset;

					fs.Add(overCenter => {
						var selectBoxPos = overCenter + new Vector2(0, textureSize.Y / 2 - 30 * Upscale - offset);
						MySprite selectBoxSpr = brushID == 0 ? new MySprite(SpriteType.TEXTURE, "SquareHollow", color: Color.White) :
								new MySprite(SpriteType.TEXTURE, canvas.BrushIDs[brushID], color: Color.White);

						ClickableArea clickableAreaColorBox = new ClickableArea(selectBoxPos, toolSize * Upscale, selectBoxSpr, 
							() => {
								canvas.CurrentBrushID = brushID;
								canvas.CurrentStretchFactor = 1f;
							});
						
						if (brushID == 0)
						{
							clickableAreaColorBox.TransformCaption(c =>
							{
								c = new ClickableArea.AreaCaption();
								c.Text = "Tx";
								c.Position = selectBoxPos - toolSize / 2f;
								c.Scale = 1.2f;
								c.Color = Color.White;
								return c;
							});
						}

						var toolAC = new UiNode(clickableAreaColorBox);

						clickableAreaColorBox.OnMouseIn += () => toolPreview.Visible = true;
						clickableAreaColorBox.OnMouseOut += () => toolPreview.Visible = false;
						clickableAreaColorBox.OnHover += (p) =>
						{
							if (inputListener.CheckKeyDown("a"))
							{
								canvas.CurrentStretchFactor /= 1.02f;
							}
							if (inputListener.CheckKeyDown("d"))
							{
								canvas.CurrentStretchFactor *= 1.02f;
							}

							toolPreview.Transform(spr => {
								if (brushID == 0)
								{
									var sTx = MySprite.CreateText("A", "Debug", Color.White, 2f);
									sTx.Position = spr.Position;
									return sTx;
								}
								else
								{
									spr = new MySprite(SpriteType.TEXTURE, canvas.BrushIDs[brushID], color: new Color(0.5f, 0.5f, 0.5f, 0.1f));
									spr.RotationOrScale = tickGetter() / 60f;
									var newSize = new Vector2(toolPreviewSize.X, canvas.CurrentStretchFactor * toolPreviewSize.Y);
									spr.Size = newSize * (textureSize.Y / newSize.Length() / 2f);
									//spr.Data = canvas.BrushIDs[brushID];
									return spr;
								}
							});
						};

						return toolAC;
					});
					yOffset += (int)(30 * Upscale);
				}

				CreateAutohideConrolGroup(mainNode, wrapperRight, activatorRight, fs, () => toolPreview.Visible = false);

				var wrapperTop = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(.1f, .1f, .1f, 0.95f));
				wrapperTop.Position = new Vector2(textureSize.X * 0.35f, textureSize.Y * 0.1f);
				wrapperTop.Size = new Vector2(textureSize.X * 0.9f, textureSize.Y * 0.2f);
				var activatorTop = new ClickableArea(wrapperTop.Position.Value - Vector2.UnitY * wrapperTop.Size.Value.Y * 0.9f, wrapperTop.Size.Value);

				controlInterval = (int)(25 * Upscale);
				var vControlOffset = -wrapperTop.Size.Value.Y * 0.1f;

				fs = new List<Func<Vector2, UiNode>>();
				fs.Add(overCenter => createBottomControl(@"Textures\FactionLogo\Others\OtherIcon_2.dds", new Vector2(-controlInterval * 3, vControlOffset) + overCenter, "Add frame", () => canvas.CreateFrame("frame")));
				fs.Add(overCenter => createBottomControl(@"Textures\FactionLogo\Builders\BuilderIcon_1.dds", new Vector2(-controlInterval * 2, vControlOffset) + overCenter, "Add via copy", () => canvas.CreateFrame("frame", true)));
				fs.Add(overCenter => {
						var c = createBottomControl("AH_PullUp", new Vector2(-controlInterval, vControlOffset) + overCenter, "Prev frame", () => canvas.PrevFrame());
						((ISpritableContainer)c.Payload).Transform(s =>
						{
							s.RotationOrScale = -(float)Math.PI / 2f;
							return s;
						});
						return c;
					});
				fs.Add(overCenter => createBottomControl("Cross", new Vector2(0, vControlOffset) + overCenter, "Delete frame", () => canvas.DeleteFrame()));
				fs.Add(overCenter => {
					var c = createBottomControl("AH_PullUp", new Vector2(controlInterval, vControlOffset) + overCenter, "Next frame", () => canvas.NextFrame());
					((ISpritableContainer)c.Payload).Transform(s =>
					{
						s.RotationOrScale = (float)Math.PI / 2f;
						return s;
					});
					return c;
				});

				fs.Add(overCenter => {
						var text = MySprite.CreateText("Frame", "Debug", Color.White, 0.5f * Upscale);
						text.Position = new Vector2(0, wrapperTop.Size.Value.Y * 0.2f + vControlOffset) + overCenter;
						var ss = new SimpleSpriteContainer(text);
						ss.OnPreRender = t => t.Transform(tx => {
								tx.Data = canvas.GetCurrentFrameName();
								return tx;
						});
						var n = new UiNode(ss);
						return n;
					}
				);

				Action<ISpritableContainer, Color, string> onPreRenderColorChanger = (isc, alternate, toggle) =>
				{
					isc.OnPreRender += ct =>
					{
						isc.Transform(s =>
						{
							s.Color = Toggle.C.Check(toggle) ? alternate : Color.White;
							return s;
						});
					};
				};

				fs.Add(overCenter => {
					var c = createBottomControl(@"Textures\FactionLogo\Builders\BuilderIcon_5.dds", new Vector2(controlInterval * 2, vControlOffset) + overCenter, "Auto Frames",
							() => Toggle.C.Invert("generate-frames"));
					onPreRenderColorChanger((ISpritableContainer)c.Payload, Color.Red, "generate-frames");
					return c;
				});
				fs.Add(overCenter => {
					var c = createBottomControl("AH_BoreSight", new Vector2(controlInterval * 3, vControlOffset) + overCenter, "Step over 2 sec", () => Toggle.C.Invert("play-repeat-slow"));
					onPreRenderColorChanger((ISpritableContainer)c.Payload, Color.Blue, "play-repeat-slow");
					return c;
				});
				fs.Add(overCenter => {
					var c = createBottomControl("Triangle", new Vector2(controlInterval * 4, vControlOffset) + overCenter, "Play", () => Toggle.C.Invert("play-repeat"));
					var isc = (ISpritableContainer)c.Payload;
					isc.Transform(s => {
						s.RotationOrScale = (float)Math.PI / 2f;
						return s;
					});
					onPreRenderColorChanger(isc, Color.Blue, "play-repeat");
					return c;
				});

				CreateAutohideConrolGroup(mainNode, wrapperTop, activatorTop, fs);



				root = view1;

				
				view2 = new UiNode(null);
				var wrapCa = new ClickableArea(textureSize / 2, textureSize);
				var wrap2 = view2.AddChild(new UiNode(wrapCa));

				wrapCa.OnHover = (p) =>
				{
					cursor = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(7f, 10f) * Upscale, color: new Color(1f));
					cursor.RotationOrScale = 6f;
				};
				

				int circleR = (int)(textureSize.X * 0.4);
				int circlePadding = (int)(textureSize.X * 0.1f);
				int triangleThickness = (int)(textureSize.X * 0.07f);

				var circlePos = new Vector2(textureSize.X / 2, textureSize.Y / 2);
				var circColor = new MySprite(SpriteType.TEXTURE, "CircleHollow", color: Color.Red);
				ClickableArea circ = new ClickableArea(circlePos, new Vector2(circleR * 2, circleR * 2), circColor);
				circ.AddCaption(new Vector2(0, -circleR - 20), "", new Color(r: 255, g: 195, b: 110));
				var cArea = new UiNode(circ);
				wrap2.AddChild(cArea);

				Func<Vector2, Vector2, Color, Action, UiNode> createGradientSpot = (pos, diag, color, clickHandler) =>
				{
					var selectBoxPos = pos;
					var selectBoxSpr = new MySprite(SpriteType.TEXTURE, "Circle", color: color);
					ClickableArea ca = new ClickableArea(selectBoxPos, diag, selectBoxSpr, clickHandler);
					var boxAreaC = new UiNode(ca);
					return boxAreaC;
				};

				Vector2 spotSize = new Vector2(triangleThickness, triangleThickness);
				Vector2 vertexA = circlePos + Vector2.UnitX * (circleR - circlePadding - triangleThickness * 0.7f);
				Vector2 vertexB = new Vector2(circlePos.X - (circleR - circlePadding - triangleThickness * 0.7f) * 0.5f, circlePos.Y + (circleR - circlePadding - triangleThickness * 0.7f) * 0.866f);
				Vector2 vertexC = new Vector2(circlePos.X - (circleR - circlePadding - triangleThickness * 0.7f) * 0.5f, circlePos.Y - (circleR - circlePadding - triangleThickness * 0.7f) * 0.866f);

				List<UiNode> gradientSpots = new List<UiNode>();
				var edgeAB = (vertexB - vertexA);
				for (float n = 0; n < (vertexB - vertexA).Length(); n += spotSize.X / 10)
				{
					var gCa = createGradientSpot(vertexA + Vector2.Normalize(edgeAB) * n, spotSize, Color.White, null);
					gradientSpots.Add(gCa);
					wrap2.AddChild(gCa);
				}
				var edgeBC = (vertexC - vertexB);
				for (float n = 0; n < (vertexC - vertexB).Length(); n += spotSize.X / 10)
				{
					var gCa = createGradientSpot(vertexB + Vector2.Normalize(edgeBC) * n, spotSize, Color.White, null);
					gradientSpots.Add(gCa);
					wrap2.AddChild(gCa);
				}
				var edgeCA = (vertexA - vertexC);
				for (float n = 0; n < (vertexA - vertexC).Length(); n += spotSize.X / 10)
				{
					var gCa = createGradientSpot(vertexC + Vector2.Normalize(edgeCA) * n, spotSize, Color.White, null);
					gradientSpots.Add(gCa);
					wrap2.AddChild(gCa);
				}


				ClickableArea hueIndicator = new ClickableArea(circlePos, new Vector2(30), new MySprite(SpriteType.TEXTURE, "Triangle", color: Color.White));
				hueIndicator.Visible = false;
				wrap2.AddChild(new UiNode(hueIndicator));
				ClickableArea shadeSatIndicator = new ClickableArea(circlePos, new Vector2(30), new MySprite(SpriteType.TEXTURE, "CircleHollow", color: Color.White));
				shadeSatIndicator.Visible = false;
				wrap2.AddChild(new UiNode(shadeSatIndicator));


				circ.OnHover = (pos) => circ.Transform(spr => {
					if (!MouseDown)
						return spr;

					var centerToCursor = pos - spr.Position.Value;
					var n = Vector2.Normalize(centerToCursor);
					
					float twoThirds = (float)Math.PI * 2 / 3f;

					Func<Vector2, Vector3> getRGBfromHueRing = (position) =>
					{
						var norm = Vector2.Normalize(position - spr.Position.Value);
						var angle = Math.Atan2(norm.Y, norm.X);
						var rFactor = (1 - Math.Min(Math.Abs(angle), twoThirds) / twoThirds);
						var diff = angle > 0 ? Math.Abs(twoThirds - angle) : Math.PI * 4 / 3 + angle;
						var gFactor = (1 - Math.Min(Math.Abs(diff), twoThirds) / twoThirds);
						var angleB = angle;
						diff = angleB > 0 ? Math.PI * 4 / 3 - angleB : Math.Abs(twoThirds + angle);
						var bFactor = (1 - Math.Min(Math.Abs(diff), twoThirds) / twoThirds);
						return new Vector3(rFactor, gFactor, bFactor);
					};

					Func<Vector2, float> getShade = (position) =>
					{
						var bToCursor = position - vertexB;
						var bDot = Math.Max(0, Vector2.Dot(Vector2.Normalize(bToCursor), Vector2.Normalize(spr.Position.Value - vertexB)));
						var abFactor = bToCursor.Length() * bDot / ((vertexB - circlePos).Length() * 1.5f);
						abFactor = Math.Min(1, abFactor);
						return abFactor;
					};

					Func<Vector2, float> getSaturation = (position) =>
					{
						var aToCursor = position - vertexA;
						var aDot = Math.Max(0, Vector2.Dot(Vector2.Normalize(aToCursor), Vector2.Normalize(spr.Position.Value - vertexA)));
						var acFactor = aToCursor.Length() * aDot / ((vertexA - circlePos).Length() * 1.5f);
						acFactor = Math.Max(0, 1 - acFactor);
						return acFactor;
					};

					Color newColorBase = canvas.CurrentColor;
					if (centerToCursor.Length() > circleR - circlePadding * 0.7f)
					{
						canvas.CurrentColor = new Color(getRGBfromHueRing(pos));

						hueIndicator.Transform(his => {
							his.Position = circlePos + n * circleR;
							var a = n;
							var z = new Vector2(0, -1);
							float angle = (float)(Math.Atan2(z.Y, z.X) - Math.Atan2(a.Y, a.X));
							his.RotationOrScale = -angle;
							hueIndicator.Visible = true;
							return his;
						});

						foreach (var gs in gradientSpots)
						{
							ISpritableContainer ic = gs.Payload as ISpritableContainer;
							ic.Transform(ss => {
								var color = new Color(getRGBfromHueRing(pos));
								var hsv = ColorExtensions.ColorToHSV(color);
								hsv.Y = getSaturation(ss.Position.Value);
								color = ColorExtensions.HSVtoColor(hsv);
								color = color.Shade((float)Math.Pow(getShade(ss.Position.Value), 2));
								//color = color.Shade(getShade(ss.Position.Value));
								ss.Color = color;
								return ss;
							});
						}
					}
					else
					{
						canvas.CurrentColorSaturation = getSaturation(pos);
						canvas.CurrentColorShade = (float)Math.Pow(getShade(pos), 2);

						shadeSatIndicator.Transform(his => {
							his.Position = pos;
							shadeSatIndicator.Visible = true;
							return his;
						});
					}
					
					circ.TransformCaption(cap => {
						cap.Text = $"Saturation:{canvas.CurrentColorSaturation:f2} Shade:{canvas.CurrentColorShade:f2} Alpha:{(canvas.CurrentAlpha / 255f):f2}";  //.ToString("f2");
						return cap;
					});

					var v3 = ColorExtensions.ColorToHSV(canvas.CurrentColor);
					v3.Y = canvas.CurrentColorSaturation;
					var desaturated = ColorExtensions.HSVtoColor(v3);

					spr.Color = desaturated.Shade(canvas.CurrentColorShade);

					//spr.Color = newColor;
					return spr;
				});

			}
			public void SwapView()
			{
				if (root == view1)
					root = view2;
				else
					root = view1;
				MainView = root == view1;
			}

			void CreateModalWindow(UiNode parentContainer, Vector2 pos, Vector2 size)
			{
				var wrapper = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(.1f, .1f, .1f, 0.95f));
				wrapper.Position = pos;
				wrapper.Size = size;

				Vector2 btnSize = new Vector2(85, 40);

				var boxCenter = wrapper.Position.Value;
				var boxDiag = wrapper.Size.Value;

				var box = new ClickableArea(boxCenter, boxDiag, wrapper);
				box.AddCaption(Vector2.UnitX * boxDiag.X / 2f, "Confirmation", Color.White, scale: 1f,  textAlignment: TextAlignment.CENTER);
				box.Visible = false;

				var wndNode = new UiNode(box);

				var cancelSprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: Color.White);
				var cancel = new ClickableArea(new Vector2(boxCenter.X - boxDiag.X / 2.5f, boxCenter.Y + boxDiag.Y / 2.5f), btnSize, cancelSprite);
				cancel.AddCaption(Vector2.UnitX * btnSize / 4f, "Cancel", Color.Black, textAlignment: TextAlignment.CENTER);
				cancel.OnMouseIn += () => cancel.Transform(spr => {
					spr.Size = btnSize * 1.3f;
					return spr;
				});
				cancel.OnMouseOut += () => cancel.Transform(spr => {
					spr.Size = btnSize;
					return spr;
				});
				cancel.Visible = false;
				wndNode.AddChild(new UiNode(cancel));

				var okSprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: Color.White);
				var ok = new ClickableArea(new Vector2(boxCenter.X + boxDiag.X / 2.5f, boxCenter.Y + boxDiag.Y / 2.5f), btnSize, okSprite);
				ok.AddCaption(Vector2.UnitX * btnSize / 4f, "OK", Color.Black, textAlignment: TextAlignment.CENTER);
				ok.OnMouseIn += () => ok.Transform(spr => {
					spr.Size = btnSize * 1.3f;
					return spr;
				});
				ok.OnMouseOut += () => ok.Transform(spr => {
					spr.Size = btnSize;
					return spr;
				});
				ok.Visible = false;
				wndNode.AddChild(new UiNode(ok));
				parentContainer.AddChild(wndNode);

				modalWnd = wndNode;
			}

			UiNode modalWnd;
			void ShowModalWindow(string text, Action okAction, Action cancelAction = null)
			{
				var ca = (ClickableArea)modalWnd.Payload;
				ca.Visible = true;
				ca.TransformCaption(x => { x.Text = text; return x; });

				var cancel = modalWnd.Children[0].Payload as ClickableArea;
				Action cancelHandler = cancelAction;

				var ok = modalWnd.Children[1].Payload as ClickableArea;
				Action okHandler = okAction;

				cancelHandler += () =>
				{
					cancel.Visible = false;
					ok.Visible = false;
					ca.Visible = false;
				};
				cancel.SetClickHandler(cancelHandler);
				cancel.Visible = true;

				okHandler += () =>
				{
					cancel.Visible = false;
					ok.Visible = false;
					ca.Visible = false;
				};
				ok.SetClickHandler(okHandler);
				ok.Visible = true;
			}

			void CreateAutohideConrolGroup(UiNode container, MySprite wrapper, ClickableArea activator, List<Func<Vector2, UiNode>> areaControlsFacs, Action onWrapperMouseOut = null)
			{
				var mainCanvasArea = container;

				var overlayCenter = wrapper.Position.Value;
				var overlayDiag = wrapper.Size.Value;

				//var toolsetAreaSprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(.1f, .1f, .1f, 0.95f));

				ClickableArea toolsetAreaOverlay = new ClickableArea(overlayCenter, overlayDiag, wrapper);
				toolsetAreaOverlay.Visible = false;
				var toolsetAreaOverlayC = new UiNode(toolsetAreaOverlay);
				ClickableArea toolsetArea = new ClickableArea(overlayCenter, overlayDiag);
				var toolsetAreaCtrl = new UiNode(toolsetArea);
				toolsetArea.OnHover = (p) =>
				{
					if (!MouseDown && toolsetAreaOverlay.Visible)
					{
						cursor = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(7f, 10f) * Upscale, color: new Color(1f));
						cursor.RotationOrScale = 6f;
					}
				};
				if (onWrapperMouseOut != null)
					toolsetArea.OnMouseOut += onWrapperMouseOut;

				mainCanvasArea.AddChild(toolsetAreaCtrl);
				mainCanvasArea.AddChild(toolsetAreaOverlayC);
				mainCanvasArea.AddChild(new UiNode(activator));

				//throw new Exception(string.Join("???", areaControlsFacs.Select(s => s(overlayCenter).ClickableArea.GetPosition().ToString())));

				foreach (var fac in areaControlsFacs)
				{
					var toolAC = fac(overlayCenter);

					ISpritableContainer sc = toolAC.Payload as ISpritableContainer;
					sc.Visible = false;

					mainCanvasArea.AddChild(toolAC);

					activator.OnHover += (p) =>
					{
						if (!MouseDown)
						{
							toolsetAreaOverlay.Visible = true;
							sc.Visible = true;
						}
					};

					/*
					toolsetArea.OnMouseIn += () =>
					{
						if (!MouseDown)
						{
							toolsetAreaOverlay.Visible = true;
							sc.Visible = true;
						}
					};*/
					toolsetArea.OnMouseOut += () => sc.Visible = false;
					toolsetArea.OnMouseOut += () => toolsetAreaOverlay.Visible = false;
				}
			}

			public void CheckHover(Vector2 cursorPosition)
			{
				DoubleTapWasHandled = false;

				foreach (var ac in root.GetDescendants().Where(x => (x.Payload is IInteractiveContainer) && (x.Payload is ISpritableContainer) && ((ISpritableContainer)x.Payload).Visible))
				{
					IInteractiveContainer clickableArea = ac.Payload as IInteractiveContainer;

					if (clickableArea.CheckHover(cursorPosition))
					{
						if (!clickableArea.Hover)
						{
							clickableArea.OnMouseIn?.Invoke();
						}
						clickableArea.Hover = true;
						clickableArea.OnHover?.Invoke(cursorPosition);

						if (DoubleTap)
						{
							DoubleTapWasHandled = DoubleTapWasHandled || clickableArea.HandleClick();
						}
					}
					else
					{
						if (clickableArea.Hover)
						{
							clickableArea.OnMouseOut?.Invoke();
						}
						clickableArea.Hover = false;
					}
				}
			}

			public MySprite GetCursor(Vector2 position)
			{
				cursor.Position = position;
				return cursor;
			}

			public IEnumerable<MySprite> Build()
			{
				return root.GetDescendants().Where(x => (x.Payload is ISpritableContainer) && ((ISpritableContainer)x.Payload).Visible)
						.SelectMany(ac => {
								var isc = (ISpritableContainer)ac.Payload;
								isc.OnPreRender?.Invoke(isc);
								return isc.BuildSprites();
							});

				//return root.GetDescendants().SelectMany(ac => ((ISpritableContainer)ac.Payload).BuildSprites());
			}

		}

		public interface ISpritableContainer
		{
			bool Visible { get; set; }
			IEnumerable<MySprite> BuildSprites();
			void Transform(Func<MySprite, MySprite> f);
			Action<ISpritableContainer> OnPreRender { get; set; }
		}
		public interface IInteractiveContainer
		{
			Action OnMouseIn { get; set; }
			Action OnMouseOut { get; set; }
			Action<Vector2> OnHover { get; set; }
			bool Hover { get; set; }
			IEnumerable<MySprite> BuildSprites();
			bool CheckHover(Vector2 cursorPosition);
			bool HandleClick();
		}
		/*
		public class AreaSpriteSet : ISpritableContainer
		{
			public List<MySprite> Sprites = new List<MySprite>();
			public bool Visible { get; set; }
			public IEnumerable<MySprite> BuildSprites()
			{
				return Sprites;
			}

			public void Transform(Func<MySprite, MySprite> f)
			{
				//throw new NotImplementedException();
			}
		}*/

		public class UiNode
		{
			public UiNode Parent;
			public List<UiNode> Children = new List<UiNode>();

			public object Payload;

			public UiNode(object payload)
			{
				Payload = payload;
			}

			public UiNode AddChild(UiNode areaControl)
			{
				Children.Add(areaControl);
				areaControl.Parent = this;
				return areaControl;
			}

			public IEnumerable<UiNode> GetDescendants()
			{
				foreach (var c in Children)
				{
					yield return c;
					foreach (var x in c.GetDescendants())
						yield return x;
				}
			}
		}

		class ClickableArea : ISpritableContainer, IInteractiveContainer
		{

			public Action OnMouseIn { get; set; }
			public Action OnMouseOut { get; set; }
			public Action<Vector2> OnHover { get; set; }
			public Action<ISpritableContainer> OnPreRender { get; set; }
			public bool Visible { get; set; }
			public bool Hover { get; set; }

			Vector2 min, max, center;
			MySprite? sprite;
			Action handler;
			MySprite? highlightSprite;
			public Vector2 GetPosition()
			{
				return center;
			}

			AreaCaption areaCaption;
			public class AreaCaption
			{
				public string Text;
				public Vector2 Position;
				public Color Color;
				public float Scale;
				public TextAlignment Alignment = TextAlignment.LEFT;
			}

			public ClickableArea(Vector2 pos, Vector2 diag, MySprite? areaSprite = null, Action handler = null)
			{
				Visible = true;
				center = pos;
				min = pos - diag / 2;
				max = pos + diag / 2;
				if (areaSprite.HasValue)
				{
					var s = areaSprite.Value;
					s.Position = pos;
					s.Size = diag;
					sprite = s;
				}
				this.handler = handler;
			}

			public void SetClickHandler(Action onClick)
			{
				handler = onClick;
			}

			public void AddCaption(Vector2 localPosition, string text, Color color, float scale = 0.6f, TextAlignment textAlignment = TextAlignment.LEFT)
			{
				areaCaption = new AreaCaption() {
					Color = color, Text = text, Position = new Vector2(min.X + localPosition.X, center.Y - localPosition.Y), Scale = scale, Alignment = textAlignment
				};
			}

			public bool HandleClick()
			{
				if (handler != null)
				{
					handler?.Invoke();
					return true;
				}
				return false;
			}

			public void TransformCaption(Func<AreaCaption, AreaCaption> transform)
			{
				areaCaption = transform(areaCaption);
			}

			public void Transform(Func<MySprite, MySprite> transform)
			{
				if (sprite.HasValue)
					sprite = transform(sprite.Value);
			}

			public bool CheckHover(Vector2 cursorPosition)
			{
				bool res = (cursorPosition.X > min.X) && (cursorPosition.X < max.X)
							&& (cursorPosition.Y > min.Y) && (cursorPosition.Y < max.Y);
				/*
				if (res)
				{
					if (areaCaption != null)
					{
						areaCaption.Text = ">" + areaCaption.Text;
						areaCaption.Color = Color.Black;
					}
					//sprite.Color = new Color(r: 255, g: 195, b: 110);
					var hl = sprite;
					hl.Size *= 1.3f;
					hl.Color = new Color(1f);
					highlightSprite = hl;
				}
				*/
				return res;
			}
			public IEnumerable<MySprite> BuildSprites()
			{
				if (highlightSprite.HasValue)
					yield return highlightSprite.Value;
				if (sprite.HasValue)
					yield return sprite.Value;
				if (areaCaption != null)
				{
					var tS = MySprite.CreateText(areaCaption.Text, "Debug", areaCaption.Color, areaCaption.Scale, areaCaption.Alignment);
					tS.Position = areaCaption.Position;
					yield return tS;
				}
			}
		}


		Canvas canvas;
		class Canvas
		{
			public int CurrentSize { get; set; }
			public float CurrentRotation { get; set; }
			public float CurrentStretchFactor { get; set; }
			public float CurrentColorShade { get; set; }
			public float CurrentColorSaturation { get; set; }
			public Color CurrentColor { get; set; }
			public byte CurrentAlpha { get; set; }

			Frame currentFrame;
			//Dictionary<string, List<MySprite>> sprites = new Dictionary<string, List<MySprite>>();
			LinkedList<Frame> frames = new LinkedList<Frame>();
			public string CurrentItemName;

			public string[] BrushIDs = new[] { "RESERVED", "Circle", "Triangle", "CircleHollow", "SquareSimple", "RightTriangle", "SquareHollow", "Textures\\FactionLogo\\Others\\OtherIcon_4.dds", "Textures\\FactionLogo\\Others\\OtherIcon_22.dds" };
			public int CurrentBrushID { get; set; }

			Func<string> _textToolGetter;

			public enum CanvasTool { Sprite, DeleteLast, Erase, Colorize, Darken, Lighten, Move, Lerp }
			public CanvasTool _currentTool = CanvasTool.Sprite;

			public Canvas(PersistentState persistentState, Func<string> textToolGetter)
			{
				CurrentSize = 10;
				CurrentColorShade = 0.5f;
				CurrentColorSaturation = 1f;
				CurrentStretchFactor = 1f;
				CurrentColor = new Color(1f);
				CurrentAlpha = 255;
				CurrentBrushID = 1;
				if (!persistentState.storageEntries.Any())
					persistentState.storageEntries.Add("Untitled.opa", new LinkedList<Frame>());
				frames = persistentState.storageEntries.First().Value;
				CurrentItemName = persistentState.storageEntries.First().Key;
				if (frames.Count == 0)
					frames.AddFirst(new Frame() { Name = "default", Sprites = new List<MySprite>() });
				currentFrame = frames.First();
				currentFramePtr = frames.First;

				_textToolGetter = textToolGetter;
			}

			public void LoadOrCreate(PersistentState persistentState, string key)
			{
				var se = persistentState.storageEntries;
				if (!se.ContainsKey(key))
					se.Add(key, new LinkedList<Frame>());
				frames = se[key];
				CurrentItemName = key;
				if (frames.Count == 0)
					frames.AddFirst(new Frame() { Name = "default", Sprites = new List<MySprite>() });
				currentFrame = frames.First();
				currentFramePtr = frames.First;
			}

			public void AddSprite(MySprite sprite)
			{
				currentFrame.Sprites.Add(sprite);
			}
			public void RemoveLastSprite()
			{
				if (currentFrame.Sprites.Count > 0)
					currentFrame.Sprites.RemoveAt(currentFrame.Sprites.Count - 1);
			}
			public List<MySprite> GetCurrentFrameSprites()
			{
				return currentFrame.Sprites;
			}
			public List<MySprite> GetPrevFrameSprites()
			{
				var prev = currentFramePtr.Previous;
				if (prev != null)
					return prev.Value.Sprites;
				else
					return null;
			}
			public LinkedList<Frame> GetFrames()
			{
				return frames;
			}
			public void CreateFrame(string tag, bool copyPrev = false)
			{
				Frame newFrame = new Frame();
				if (copyPrev)
				{
					if (Variables.Get<bool>("rotate-when-copy"))
					{
						newFrame.Sprites = currentFrame.Sprites.Select(s => {
							s.Position = rotateAroundPos(new Vector2(250, 250), s.Position.Value);
							return s;
						}).ToList();
					}
					else
						newFrame.Sprites = new List<MySprite>(currentFrame.Sprites);
				}
				else
					newFrame.Sprites = new List<MySprite>();

				//var prev = currentFramePtr.Previous;
				var next = currentFramePtr.Next;
				// created frame while sitting on first (default)
				if (currentFrame.Name == "default")
				{
					newFrame.Name = "frame-1.00";
				}
				else 
				{
					var prevNum = float.Parse(currentFrame.Name.Split(new string[] { "frame" }, StringSplitOptions.None)[1].Trim('-'));
					if (next != null)
					{
						var nextNum = float.Parse(next.Value.Name.Split(new string[] { "frame" }, StringSplitOptions.None)[1].Trim('-'));
						newFrame.Name = "frame-" + (prevNum + (nextNum - prevNum) / 2f).ToString("f2") ;
					}
					else
					{
						newFrame.Name = "frame-" + (prevNum + 1).ToString("f2");
					}
				}

				frames.AddAfter(currentFramePtr, newFrame);
				currentFramePtr = currentFramePtr.Next;
				currentFrame = currentFramePtr.Value;
			}
			public void DeleteFrame()
			{
				if (currentFramePtr != frames.First)
				{
					var prev = currentFramePtr.Previous;
					frames.Remove(currentFramePtr);
					currentFramePtr = prev;
					currentFrame = currentFramePtr.Value;
				}
			}

			LinkedListNode<Frame> currentFramePtr;
			public string GetCurrentFrameName()
			{
				return $"{CurrentItemName}: {currentFrame.Name}";
			}
			public void NextFrame()
			{
				if (currentFramePtr.Next != null)
				{
					currentFramePtr = currentFramePtr.Next;
				}
				else
				{
					currentFramePtr = frames.First;
				}
				currentFrame = currentFramePtr.Value;
			}
			public void PrevFrame()
			{
				if (currentFramePtr.Previous != null)
				{
					currentFramePtr = currentFramePtr.Previous;
				}
				else
				{
					currentFramePtr = frames.Last;
				}
				currentFrame = currentFramePtr.Value;
			}

			internal void PickColor(Vector2 cursorPosition)
			{
				try
				{
					var sprite = currentFrame.Sprites.Reverse<MySprite>()
						.FirstOrDefault(s => Vector2.Distance(cursorPosition, s.Position.Value) - (s.Size ?? Vector2.One * 20).Length() / 2 < 1f);
					var c = sprite.Color;
					if (c != null)
					{
						CurrentColor = c.Value;
						CurrentAlpha = c.Value.A;
					}
				}
				catch (Exception ex)
				{
					E.DebugLog(ex.ToString());
					throw ex;
				}
				
				/*
				var sprite = currentFrame.OrderBy(s => Vector2.Distance(cursorPosition, s.Position.Value));
				if (sprite.Count() > 0)
				{
					var c = sprite.First().Color;
					CurrentColor = c.Value;
					CurrentAlpha = c.Value.A;
				}*/
			}

			internal void DeleteSprites(MySprite cursor)
			{
				// private MyProgrammableBlock.ScriptTerminationReason RunSandboxedProgramActionCore(Action<IMyGridProgram> action, out string response)
				// ScriptOutOfRangeException
				currentFrame.Sprites.RemoveAll(s => Vector2.Distance(cursor.Position.Value, s.Position.Value) - cursor.Size.Value.Length() / 2 
					- (s.Size ?? Vector2.One * 20).Length() / 2 < 1f);
			}

			public MySprite GetCursor()
			{
				MySprite cursor;

				var v3 = ColorExtensions.ColorToHSV(CurrentColor);
				v3.Y = CurrentColorSaturation;
				var desaturated = ColorExtensions.HSVtoColor(v3);
				desaturated = desaturated.Shade(CurrentColorShade);

				desaturated.A = CurrentAlpha;
				if (CurrentBrushID == 0)
				{
					var text = _textToolGetter() ?? "";
					if (string.IsNullOrEmpty(text))
						text = "A";
					cursor = MySprite.CreateText(text, "Debug", Color.White, Math.Min(15f, (float)CurrentSize / 10));
					cursor.Color = desaturated;
					//cursor.Size = new Vector2(1f) * canvas.CurrentSize / 10;
					//cursor.RotationOrScale = canvas.CurrentRotation;
				}
				else
				{
					cursor = new MySprite(SpriteType.TEXTURE, BrushIDs[CurrentBrushID], size: new Vector2(1, 1f / CurrentStretchFactor) * CurrentSize, color: desaturated);
					cursor.RotationOrScale = CurrentRotation;
				}

				return cursor;
			}

			internal void HandleUserAction(bool mDown, bool qReleased, bool dTap, ref MySprite cursor, Vector2 drag)
			{
				if (qReleased && _currentTool == CanvasTool.DeleteLast)
				{
					RemoveLastSprite();
				}
				if (_currentTool == CanvasTool.Move && mDown)
				{
					var move = new Vector2(drag.Y, drag.X);
					OverwriteSpritesUnderCursor(cursor.Position.Value, cursor.Size.Value, s => { s.Position += move; return s; });
				}
				if (_currentTool == CanvasTool.Lerp)
				{
					if (mDown && !lerpStart.HasValue)
					{
						lerpStart = cursor.Position.Value;
					}
					if (lerpStart.HasValue && qReleased)
					{
						lerpEnd = cursor.Position.Value;
						E.DebugLog($"lerpStart: {lerpStart}");
						E.DebugLog($"lerpEnd: {lerpEnd}");
						LerpThroughFrames(cursor.Position.Value, cursor.Size.Value);
						lerpStart = null;
					}
				}

				if ((Toggle.C.Check("auto") && mDown) || (!Toggle.C.Check("auto") && qReleased))
				{
					if (_currentTool == CanvasTool.Erase)
					{
						DeleteSprites(cursor);
					}
					else if (_currentTool == CanvasTool.Sprite)
					{
						if (CurrentBrushID == 0)
						{
							var textSprite = cursor;
							textSprite.Data = _textToolGetter() ?? "";
							AddSprite(textSprite);
						}
						else
							AddSprite(cursor);
					}
					else if (_currentTool == CanvasTool.Colorize)
					{
						var v3 = ColorExtensions.ColorToHSV(CurrentColor);
						v3.Y = CurrentColorSaturation;
						var desaturated = ColorExtensions.HSVtoColor(v3);
						desaturated = desaturated.Shade(CurrentColorShade);
						OverwriteSpritesUnderCursor(cursor.Position.Value, cursor.Size.Value, s => { s.Color = desaturated; return s; });
					}
					else if (_currentTool == CanvasTool.Darken)
					{
						// degrades to grayscale at all 255
						OverwriteSpritesUnderCursor(cursor.Position.Value, cursor.Size.Value, s => { s.Color = Color.Darken(s.Color.Value, 0.005f); return s; });
					}
					else if (_currentTool == CanvasTool.Lighten)
					{
						OverwriteSpritesUnderCursor(cursor.Position.Value, cursor.Size.Value, s => { s.Color = Color.Lighten(s.Color.Value, 0.005f); return s; });
					}
				}

				if (dTap)
				{
					PickColor(cursor.Position.Value);
				}
			}

			Vector2? lerpStart;
			Vector2 lerpEnd;
			void LerpThroughFrames(Vector2 cPos, Vector2 cSize)
			{
				var cursorR = Math.Min(cSize.X, cSize.Y) / 2f;
				Vector2 vec = lerpEnd - lerpStart.Value;
				//var spritesToCopy = currentFrame.Sprites.Where(x => Vector2.Distance(cPos, x.Position.Value) - cursorR - (x.Size ?? Vector2.One * 20).Length() < 1f);
				var spritesToCopy = currentFrame.Sprites;
				E.DebugLog($"spritesToCopy: {spritesToCopy.Count()}");

				var count = 1;
				var node = currentFramePtr;
				while (!ReferenceEquals(node, frames.Last))
				{
					node = node.Next;
					++count;
				}
				E.DebugLog($"frames: {count}");

				var dp = vec / count;
				count = 1;
				E.DebugLog($"dp: {dp}");
				node = currentFramePtr;
				while (!ReferenceEquals(node, frames.Last))
				{
					count++;
					node = node.Next;
					foreach (var s in spritesToCopy)
					{
						var spr = s;
						spr.Position = s.Position.Value + dp * count;
						node.Value.Sprites.Add(spr);
					}
				}
			}

			void OverwriteSpritesUnderCursor(Vector2 cPos, Vector2 cSize, Func<MySprite, MySprite> trans)
			{
				var cursorR = Math.Min(cSize.X, cSize.Y) / 2f;
				for (int n = 0; n < currentFrame.Sprites.Count; n++)
				{
					var s = currentFrame.Sprites[n];
					var spriteSize = s.Size ?? Vector2.One * 20;
					var spriteR = Math.Min(spriteSize.X, spriteSize.Y) / 2f;
					if (Vector2.Distance(cPos, s.Position.Value) - cursorR - spriteR < 1f)
					{
						currentFrame.Sprites[n] = trans(s);
					}
				}
			}

			public void NextTool()
			{
				_currentTool++;
				var last = Enum.GetValues(typeof(CanvasTool)).Cast<CanvasTool>().Last();
				if (_currentTool > last)
					_currentTool = 0;
			}
		}

		class SimpleSpriteContainer : ISpritableContainer
		{
			MySprite[] sprites;
			public bool Visible { get; set; }
			public Action<ISpritableContainer> OnPreRender { get; set; }
			public SimpleSpriteContainer(params MySprite[] sprites)
			{
				this.sprites = sprites;
			}
			public IEnumerable<MySprite> BuildSprites()
			{
				return sprites;
			}
			public void Transform(Func<MySprite, MySprite> f)
			{
				for (int n = 0; n < sprites.Length; n++)
				{
					sprites[n] = f(sprites[n]);
				}
			}
		}


		class ProgressBarUiElement : ISpritableContainer
		{
			public Action<ISpritableContainer> OnPreRender { get; set; }
			bool _vis = true;
			public bool Visible
			{
				get
				{
					return _vis;
				}
				set
				{
					_vis = value;
					bar.Update();
					//bar.SetProgress(0.6f);
				}
			}

			ProgressBar bar;
			public ProgressBarUiElement(ProgressBar progressBar)
			{
				bar = progressBar;
			}

			public IEnumerable<MySprite> BuildSprites()
			{
				return bar.GetSprites();
			}

			public void Transform(Func<MySprite, MySprite> f)
			{
				bar.Update();
			}
		}

		ProgressBar compelxityBar;
		class ProgressBar
		{
			MySprite outer;
			MySprite inner;
			MySprite? caption;
			Func<float> factorGetter;

			public ProgressBar(Vector2 pos, Vector2 size, Func<float> factorGetter, string caption = null)
			{
				this.factorGetter = factorGetter;
				outer = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(1f));
				outer.Position = pos;
				outer.Size = size;
				inner = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: new Color(0.2f));
				//inner.Position = new Vector2(pos.X, pos.Y + (size.Y * 0.05f));
				inner.Position = pos;
				inner.Size = new Vector2(size.X * 0.98f, size.Y * 0.9f);
				if (caption != null)
				{
					var spr = MySprite.CreateText(caption, "Debug", Color.Firebrick, 0.7f, TextAlignment.CENTER);
					spr.Position = new Vector2(pos.X, pos.Y - size.Y - 15f);
					this.caption = spr;
				}
			}

			public void Update()
			{
				SetProgress(factorGetter());
				//SetProgress(runtime.CurrentInstructionCount / 50000f);
			}

			public void SetProgress(float factor)
			{
				var newSize = new Vector2(outer.Size.Value.X * 0.98f, outer.Size.Value.Y * 0.9f);
				var newPos = outer.Position.Value;

				newPos.X = newPos.X - newSize.X / 2 * (1 - factor);
				newSize.X *= factor;

				inner.Position = newPos;
				inner.Size = newSize;
			}

			public IEnumerable<MySprite> GetSprites()
			{
				yield return outer;
				yield return inner;
				if (caption != null)
					yield return caption.Value;
			}
		}

		BoundPanelsManager boundPanelsManager;
		class BoundPanelsManager
		{
			class BoundPanel
			{
				public IMyTextPanel Panel;
				public int Interval;
				public DateTime LastStamp;
				public bool Playback;
				public LinkedList<Frame> Frames;
				public LinkedListNode<Frame> Ptr;
			}

			List<BoundPanel> boundPanels = new List<BoundPanel>();

			IMyGridTerminalSystem _gts;
			PersistentState pstate;
			public BoundPanelsManager(IMyGridTerminalSystem gts, PersistentState persistentState)
			{
				_gts = gts;
				pstate = persistentState;
			}

			public void HandleTick()
			{
				var dt = DateTime.Now;
				foreach (var bp in boundPanels)
				{
					if ((dt - bp.LastStamp).TotalMilliseconds > bp.Interval)
					{
						if (bp.Ptr.Next != null)
							bp.Ptr = bp.Ptr.Next;
						else
							bp.Ptr = bp.Frames.First;
						using (var frame = bp.Panel.DrawFrame())
						{
							//E.Echo(ptr.Value.Name);
							frame.AddRange(bp.Ptr.Value.Sprites);
							bp.LastStamp = dt;
						}
						bp.Panel.ContentType = ContentType.TEXT_AND_IMAGE;
						bp.Panel.ContentType = ContentType.SCRIPT;
						bp.Panel.ScriptBackgroundColor = Color.Transparent;
					}
				}
			}
			public void StartPanelPlayback(string panelTag, string itemName, int updateRate)
			{
				var pnls = new List<IMyTextPanel>();
				_gts.GetBlocksOfType(pnls, x => x.CustomName.Contains(panelTag));
				var surf = pnls.FirstOrDefault();
				if (surf != null && pstate.storageEntries.ContainsKey(itemName))
				{
					boundPanels.RemoveAll(x => x.Panel == surf);
					var bp = new BoundPanel()
					{
						Panel = surf,
						Frames = pstate.storageEntries[itemName],
						Interval = (int)(1f / updateRate * 1000f),
						Playback = true,
						Ptr = pstate.storageEntries[itemName].First,
					};
					boundPanels.Add(bp);
				}
			}
			public void StopPanelPlayback(string panelTag)
			{
				boundPanels.RemoveAll(x => x.Panel.CustomName.Contains(panelTag));
				//var bp = boundPanels.FirstOrDefault(x => x.Panel.CustomName.Contains(panelTag));
				//if (bp != null)
				//bp.Playback = false;
			}
		}

		/// ///////////////////////////////

		public static class E
		{
			static string debugTag = "";
			static Action<string> e;
			static IMyTextSurface l;
			public static int T;
			static IMyGridTerminalSystem g;
			public static void Init(Action<string> echo, IMyGridTerminalSystem t, IMyProgrammableBlock me)
			{
				e = echo;
				g = t;
				l = (g.GetBlockWithName("LCD Debug") as IMyTextPanel) ?? me.GetSurface(1);
				l.ContentType = ContentType.TEXT_AND_IMAGE;
				l.WriteText("");
			}
			public static void Echo(string s) { if ((debugTag == "") || (s.Contains(debugTag))) e(s); }

			public static void DebugLog(string s)
			{
				l.WriteText($"{T}: {s}\n", true);
			}
		}

		InputListener UserCtrlListener;
		class InputListener
		{
			public Vector2 Cumulative = new Vector2();
			List<IMyShipController> ctrls;
			Func<int> tickGetter;
			public IMyShipController GetControlledCockpit()
			{
				return ctrls.Where(c => c.IsUnderControl).FirstOrDefault();
			}

			public Vector3 GetVector()
			{
				Vector3 res = new Vector3();
				var userCtrl = GetControlledCockpit();
				if (userCtrl != null)
				{
					res = userCtrl.MoveIndicator;
					res.X = -res.X;
					res.Z = -res.Z;
				}
				return res;
			}
			public Vector2 GetRot()
			{
				return GetControlledCockpit()?.RotationIndicator ?? Vector2.Zero;
			}
			public float GetRoll()
			{
				return GetControlledCockpit()?.RollIndicator ?? 0f;
			}

			class InputHistory
			{
				public string KeyName;
				public int LastKeyDownStamp;
				public int State;
			}

			List<InputHistory> h;
			public InputListener(List<IMyShipController> ctrlToConsider, Func<int> tickGetter)
			{
				ctrls = ctrlToConsider;
				this.tickGetter = tickGetter;

				h = new List<InputHistory>();
				h.Add(new InputHistory { KeyName = "spacebar" });
				h.Add(new InputHistory { KeyName = "c" });
				h.Add(new InputHistory { KeyName = "e" });
				h.Add(new InputHistory { KeyName = "q" });

				h.Add(new InputHistory { KeyName = "w" });
				h.Add(new InputHistory { KeyName = "s" });
				h.Add(new InputHistory { KeyName = "a" });
				h.Add(new InputHistory { KeyName = "d" });
			}

			public bool CheckKeyDown(string keyName)
			{
				var userCtrl = GetControlledCockpit();
				if (userCtrl != null)
				{
					bool isKeyDown = false;
					if ((keyName == "spacebar") && (userCtrl.MoveIndicator.Y > 0))
						isKeyDown = true;
					if ((keyName == "c") && (userCtrl.MoveIndicator.Y < 0))
						isKeyDown = true;
					if ((keyName == "e") && (userCtrl.RollIndicator > 0))
						isKeyDown = true;
					if ((keyName == "q") && (userCtrl.RollIndicator < 0))
						isKeyDown = true;
					if ((keyName == "w") && (userCtrl.MoveIndicator.Z < 0))
						isKeyDown = true;
					if ((keyName == "s") && (userCtrl.MoveIndicator.Z > 0))
						isKeyDown = true;
					if ((keyName == "a") && (userCtrl.MoveIndicator.X < 0))
						isKeyDown = true;
					if ((keyName == "d") && (userCtrl.MoveIndicator.X > 0))
						isKeyDown = true;

					return isKeyDown;
				}
				return false;
			}

			public bool KeyReleased(string keyName)
			{
				var cState = h.First(h => h.KeyName == keyName);
				if (CheckKeyDown(keyName))
				{
					if (cState.State == 0)
					{
						cState.State = 1;
						cState.LastKeyDownStamp = tickGetter();
					}
					if (cState.State == 2)
					{
						cState.State = 0;
					}
					return false;
				}
				else
				{
					if ((cState.State == 1) || (cState.State == 2))
					{
						cState.State = 0;
						return true;
					}
				}
				return false;
			}
			public bool CheckDoubleTap(string keyName)
			{
				return UpdateKeyState(keyName, tickGetter(), CheckKeyDown(keyName));
			}

			bool UpdateKeyState(string keyName, int currentTick, bool keyDown)
			{
				var cState = h.First(h => h.KeyName == keyName);
				if (keyDown)
				{
					if (cState.State == 0)
					{
						cState.State = 1;
						cState.LastKeyDownStamp = currentTick;
					}
					if (cState.State == 2) // was released less than 30 ticks ago
					{
						cState.State = 0;
						return true;
					}
				}
				else
				{
					if (currentTick - cState.LastKeyDownStamp < 30)
					{
						if (cState.State == 1)
						{
							cState.State = 2;
						}
					}
					else
					{
						cState.State = 0;
					}
				}
				return false;
			}
		}

		public class Frame
		{
			public string Name { get; set; }
			public List<MySprite> Sprites { get; set; }
		}

		static class ColorHelpers
		{
			public static Color ColorFromHSV(double hue, double saturation, double value)
			{
				int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
				double f = hue / 60 - Math.Floor(hue / 60);

				value = value * 255;
				int v = Convert.ToInt32(value);
				int p = Convert.ToInt32(value * (1 - saturation));
				int q = Convert.ToInt32(value * (1 - f * saturation));
				int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

				if (hi == 0)
					return new Color(v, t, p, 255);
				else if (hi == 1)
					return new Color(q, v, p,255);
				else if (hi == 2)			
					return new Color(p, v, t,255);
				else if (hi == 3)			
					return new Color(p, q, v,255);
				else if (hi == 4)			
					return new Color(t, p, v,255);
				else						
					return new Color(v, p, q, 255);
			}
		}


		StateWrapper stateWrapper;
		public class StateWrapper
		{
			public PersistentState PState { get; private set; }
			Action<PersistentState> onLoadComplete;

			int sSize;
			public int GetStorageSize()
			{
				return sSize;
			}

			public void ClearPersistentState()
			{
				var currentState = PState;
				PState = new PersistentState(onLoadComplete);
				//PState.StaticDockOverride = currentState.StaticDockOverride;
			}

			Action<string> stateSaver;
			public StateWrapper(Action<string> stateSaver, Action<PersistentState> onLoadComplete)
			{
				this.onLoadComplete = onLoadComplete;
				this.stateSaver = stateSaver;
			}

			public void Save()
			{
				PState.Save(stateSaver);
			}

			string storage;
			IMyGridProgramRuntimeInfo runtimeInfo;
			public bool TryLoad(string serialized, IMyGridProgramRuntimeInfo runtimeInfo)
			{
				PState = new PersistentState(onLoadComplete);
				try
				{
					storage = serialized;
					this.runtimeInfo = runtimeInfo;
					PState.Load(serialized, runtimeInfo);
					sSize = System.Text.ASCIIEncoding.Unicode.GetByteCount(serialized);
					return true;
				}
				catch { }
				return false;
			}

			public bool HumbleLoad()
			{
				try
				{
					return PState.HumbleLoad(storage, runtimeInfo);
				}
				catch (Exception ex)
				{
					E.Echo("State load failed. Clearing state now. Stopping updates. Restart and draw again.");
					E.Echo(ex.ToString());
					E.Echo(storage);
					ClearPersistentState();
					Save();
					runtimeInfo.UpdateFrequency = UpdateFrequency.None;
					return false;
				}
			}
		}

		public class PersistentState
		{
			public int numberInCircle = 0;

			public Dictionary<string, string> exportToPanels = new Dictionary<string, string>();

			Action<PersistentState> onLoadComplete;

			public PersistentState(Action<PersistentState> onLoadComplete)
			{
				this.onLoadComplete = onLoadComplete;
			}

			T ParseValue<T>(Dictionary<string, string> values, string key)
			{
				string res;
				if (values.TryGetValue(key, out res) && !string.IsNullOrEmpty(res))
				{
					if (typeof(T) == typeof(String))
						return (T)(object)res;
					else if (typeof(T) == typeof(int))
						return (T)(object)int.Parse(res);
					else if (typeof(T) == typeof(float))
						return (T)(object)float.Parse(res);
					else if (typeof(T) == typeof(float?))
						return (T)(object)float.Parse(res);
					else if (typeof(T) == typeof(long?))
						return (T)(object)long.Parse(res);
					else if (typeof(T) == typeof(Vector3D?))
					{
						var d = res.Split(':');
						return (T)(object)new Vector3D(double.Parse(d[0]), double.Parse(d[1]), double.Parse(d[2]));
					}
					else if (typeof(T) == typeof(List<string>))
					{
						return (T)(object)res.Split(',').ToList();
					}
				}
				return default(T);
			}


			public Dictionary<string, LinkedList<Frame>> storageEntries = new Dictionary<string, LinkedList<Frame>>();

			// add deferred
			public string SerializeSprites()
			{
				var lines = new List<string>();
				foreach (var entry in storageEntries)
				{
					foreach (var spriteGroup in entry.Value)
					{
						var strings = spriteGroup.Sprites.Select(spr => $"ColorR={spr.Color.Value.R},ColorG={spr.Color.Value.G},ColorB={spr.Color.Value.B},ColorA={spr.Color.Value.A}," +
								$"Brush={(spr.Type == SpriteType.TEXT ? EscapeTextStr(spr.Data) : spr.Data)},SizeX={(spr.Size ?? Vector2.Zero).X},SizeY={(spr.Size ?? Vector2.Zero).Y},PosX={spr.Position.Value.X},PosY={spr.Position.Value.Y},Rotation={spr.RotationOrScale},Type={spr.Type}");

						lines.Add($"{entry.Key}/Sprites/{spriteGroup.Name}={string.Join("|", strings)}");
					}
				}

				return string.Join("\n", lines);
			}

			public bool Ready;
			public PersistentState Load(string storage, IMyGridProgramRuntimeInfo runtimeInfo)
			{
				if (!string.IsNullOrEmpty(storage))
				{
					E.Echo(storage);
					//throw new Exception();

					// var values = storage.Split('\n').ToDictionary(s => s.Split(new string[] { "\"=\"" }, StringSplitOptions.None)[0], s => s.Split('=')[1]);
					var values = storage.Split('\n').ToDictionary(s => s.Split('=')[0], s => string.Join("=", s.Split('=').Skip(1)));

					sprHumbleLoader = HumbleLoadSprites(values, runtimeInfo).GetEnumerator();

					numberInCircle = ParseValue<int>(values, "numberInCircle");

					if (values.ContainsKey("exportToPanels"))
					{
						exportToPanels = new Dictionary<string, string>();
						var exStr = values["exportToPanels"];
						if (!string.IsNullOrEmpty(exStr))
						{
							var vals = exStr.Split(',').ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);
							foreach (var pair in vals)
							{
								exportToPanels.Add(pair.Key, pair.Value);
							}
						}
					}

					//ParseSprites(values);
				}
				return this;
			}

			public int SpriteCount { get; private set; }
			IEnumerator<bool> sprHumbleLoader;
			public bool HumbleLoad(string storage, IMyGridProgramRuntimeInfo runtimeInfo)
			{
				if (sprHumbleLoader != null)
				{
					if (sprHumbleLoader.MoveNext())
						SpriteCount++;
					else
					{
						sprHumbleLoader.Dispose();
						sprHumbleLoader = null;
						onLoadComplete?.Invoke(this);
						return true;
					}
					return false;
				}
				return true;
			}

			IEnumerable<bool> HumbleLoadSprites(Dictionary<string, string> values, IMyGridProgramRuntimeInfo runtimeInfo)
			{
				// Sprites/def=PosX=111,PosY=222,Type=aaa|PosX=111,PosY=222,Type=aaa
				// Sprites/frame1=PosX=111,PosY=222,Type=aaa|PosX=111,PosY=222,Type=aaa
				foreach (var spriteLine in values.Where(v => v.Key.Contains("Sprites")))
				{
					var parts = spriteLine.Key.Split('/');
					var itemName = parts[0];
					var frameName = spriteLine.Key.Split('/')[2];

					if (!storageEntries.ContainsKey(itemName))
					{
						storageEntries.Add(itemName, new LinkedList<Frame>());
					}

					var frame = new Frame() { Name = frameName, Sprites = new List<MySprite>() };
					storageEntries[itemName].AddLast(frame);

					if (!string.IsNullOrEmpty(spriteLine.Value))
					{
						var entries = spriteLine.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

						foreach (string s1 in entries)
						{
							if (runtimeInfo.CurrentInstructionCount > 10000)
								yield return false;


							//throw new NullReferenceException("s1: " + s1);
							//E.DebugToPanel("s1:" + s1 + "x");
							//E.FlushDebugPanel();

							var vals = s1.Split(',').ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);
							Color c = new Color(int.Parse(vals["ColorR"]), int.Parse(vals["ColorG"]), int.Parse(vals["ColorB"]));
							c.A = byte.Parse(vals["ColorA"]);
							MySprite spr;
							SpriteType type;
							if (Enum.TryParse(vals["Type"], out type) && (type == SpriteType.TEXT))
							{
								spr = MySprite.CreateText(UnescapeTextStr(vals["Brush"]), "Debug", c);
								spr.Size = Vector2.Zero;
							}
							else
							{
								spr = new MySprite(SpriteType.TEXTURE, vals["Brush"], size: new Vector2(float.Parse(vals["SizeX"]), float.Parse(vals["SizeY"])), color: c);
							}
							spr.Position = new Vector2(float.Parse(vals["PosX"]), float.Parse(vals["PosY"]));
							spr.RotationOrScale = float.Parse(vals["Rotation"]);
							frame.Sprites.Add(spr);
						}
					}


				}

				yield return true;
			}

			string EscapeTextStr(string data)
			{
				return data.Replace("\n", "$$n$$").Replace(",", "$$c$$").Replace("|", "$$v$$").Replace("=", "$$e$$").Replace("/", "$$s$$");
			}
			string UnescapeTextStr(string data)
			{
				return data.Replace("$$n$$", "\n").Replace("$$c$$", ",").Replace("$$v$$", "s").Replace("$$e$$", "=").Replace("$$s$$", "/");
			}

			public void Save(Action<string> store)
			{
				E.DebugLog("Serializing to storage:");
				var s = Serialize();
				E.DebugLog(s);
				store(s);
			}

			string Serialize()
			{
				string[] pairs = new string[]
				{
					"exportToPanels=" + string.Join(",", exportToPanels.Select(pair => $"{pair.Key}={pair.Value}")),
					SerializeSprites()
				};
				return string.Join("\n", pairs);
			}

			public override string ToString()
			{
				return Serialize();
			}
		}

		public void Save()
		{
			stateWrapper.Save();
		}

		static Vector2 rotateAroundPos(Vector2 pos, Vector2 victim)
		{
			var angle = 0.1f;

			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);

			// translate point back to origin:
			victim -= pos;

			// rotate point
			float xnew = victim.X * c - victim.Y * s;
			float ynew = victim.X * s + victim.Y * c;

			// translate point back:
			return pos + new Vector2(xnew, ynew);
		}

		string SerializeItem(Vector2 vSize, LinkedList<Frame> framesToExport)
		{
			var entries = new List<string>();
			foreach (var spriteGroup in framesToExport)
			{
				var strings = spriteGroup.Sprites.Select(spr =>
				$"ColorR={spr.Color.Value.R},ColorG={spr.Color.Value.G},ColorB={spr.Color.Value.B},ColorA={spr.Color.Value.A}," +
						$"Brush={spr.Data.Replace(@"\", @"\\")}," +
						$"SizeX={(spr.Size.HasValue ? spr.Size.Value.X / vSize.X : 1f)},SizeY={(spr.Size.HasValue ? spr.Size.Value.Y / vSize.Y : 1f)}," +
						$"PosX={(spr.Position.HasValue ? spr.Position.Value.X / (vSize.X * 0.5f) - 1 : 0)},PosY={(spr.Position.HasValue ? spr.Position.Value.Y / (vSize.Y * 0.5f) - 1 : 0)}," +
						$"Rotation={(spr.Type == SpriteType.TEXT ? 1f / (vSize.X / 2f) : spr.RotationOrScale)},Type={spr.Type}");

				entries.Add($"Sprites/{spriteGroup.Name}={string.Join("|", strings)}");
			}

			return "\"" + string.Join("\\n", entries) + "\"";
		}

		class Scheduler
		{
			static Scheduler inst = new Scheduler();
			Scheduler() { }

			public static Scheduler C
			{
				get
				{
					inst.delayForNextCmd = 0;
					inst.repeatCondition = null;
					return inst;
				}
			}

			class DelayedCommand
			{
				public DateTime TimeStamp;
				//public string Command;
				public Action Command;
				public Func<bool> repeatCondition;
				public long delay;
			}

			Queue<DelayedCommand> q = new Queue<DelayedCommand>();
			long delayForNextCmd;
			Func<bool> repeatCondition;
			public void Reset()
			{
				q.Clear();
				delayForNextCmd = 0;
				repeatCondition = null;
			}

			public Scheduler After(int ms)
			{
				this.delayForNextCmd += ms;
				return this;
			}

			public Scheduler RunCmd(Action cmd)
			{
				q.Enqueue(new DelayedCommand { TimeStamp = DateTime.Now.AddMilliseconds(delayForNextCmd), Command = cmd, repeatCondition = repeatCondition, delay = delayForNextCmd });
				return this;
			}

			public Scheduler RepeatWhile(Func<bool> repeatCondition)
			{
				this.repeatCondition = repeatCondition;
				return this;
			}

			public void HandleTick()
			{
				if (q.Count > 0)
				{
					E.Echo("Scheduled actions count:" + q.Count);
					var c = q.Peek();
					if (c.TimeStamp < DateTime.Now)
					{
						if (c.repeatCondition != null)
						{
							if (c.repeatCondition.Invoke())
							{
								c.Command.Invoke();
								c.TimeStamp = DateTime.Now.AddMilliseconds(c.delay);
							}
							else
							{
								q.Dequeue();
							}
						}
						else
						{
							c.Command.Invoke();
							q.Dequeue();
						}

						//sendFeedback("Executing " + c.Command, "", true);
					}
				}
			}
		}

		FileHandler fileHandler;
		public class FileHandler
		{
			ShellHandler _shellHandler;
			IMyIntergridCommunicationSystem _igc;
			PersistentState _pstate;
			public FileHandler(ShellHandler shellHandler, List<string> availableFiles, PersistentState pstate, IMyIntergridCommunicationSystem igc)
			{
				_shellHandler = shellHandler;
				_igc = igc;
				_pstate = pstate;
				foreach (var s in availableFiles)
				{
					_shellHandler.AddHandler(s, () => pstate.storageEntries[s].First().Sprites);
				}
			}
			List<MyIGCMessage> _unicasts = new List<MyIGCMessage>();
			public void HandleIGC()
			{
				_unicasts.Clear();
				var bc = _igc.RegisterBroadcastListener("op:get-pic");
				if (bc.HasPendingMessage)
				{
					var msg = bc.AcceptMessage();
					if (msg.Tag == "op:get-pic")
					{
						_unicasts.Add(new MyIGCMessage(msg.Data, "shell.get", msg.Source));
					}
				}
				_shellHandler.HandleRequests(_unicasts);
			}

			List<ImmutableArray<MyTuple<int, string, Vector2, Vector2, float, Vector4>>> _fileFrames = 
					new List<ImmutableArray<MyTuple<int, string, Vector2, Vector2, float, Vector4>>>();
			public void HandleIGC(List<MyIGCMessage> unicasts)
			{
				var bc = _igc.RegisterBroadcastListener("request-file");
				if (bc.HasPendingMessage)
					_unicasts.Add(bc.AcceptMessage());
				_unicasts.AddRange(unicasts);

				foreach (var msg in _unicasts)
				{
					if (msg.Tag.Contains("request-file"))
					{
						var fname = (string)msg.Data;
						if (_pstate.storageEntries.Any(x => x.Key == fname))
						{
							var file = _pstate.storageEntries.FirstOrDefault(x => x.Key == fname);
							foreach (var frame in file.Value)
							{
								_fileFrames.Add(_shellHandler.GetIgcSpriteFrame(frame.Sprites));
							}
							if (_fileFrames.Any())
							{
								// ImmutableArray<ImmutableArray<MyTuple<int, string, Vector2, Vector2, float, Vector4>>>
								_igc.SendUnicastMessage(msg.Source, $"op.file:{fname}", _fileFrames.ToImmutableArray());
							}
						}
						E.DebugLog($"Sent '{fname}', frame count: {_fileFrames.Count}");
						_fileFrames.Clear();
					}
				}
				_unicasts.Clear();
			}
		}

		ShellHandler shellHandler;
		public class ShellHandler
		{
			IMyIntergridCommunicationSystem _igc;
			Dictionary<string, Func<string>> _dataGetter = new Dictionary<string, Func<string>>();
			Dictionary<string, Func<List<MySprite>>> _spriteGetter = new Dictionary<string, Func<List<MySprite>>>();
			string HostName;
			Vector2 vSize;

			public int UnicastCtr;
			public ShellHandler(IMyIntergridCommunicationSystem igc, string hostName, Vector2 viewPortSize)
			{
				_igc = igc;
				HostName = hostName;
				vSize = viewPortSize;
			}

			public void AddHandler(string feedTag, Func<string> getter)
			{
				_dataGetter[feedTag] = getter;
			}

			public void AddHandler(string feedTag, Func<List<MySprite>> getter)
			{
				_spriteGetter[feedTag] = getter;
			}

			List<MyTuple<int, string, Vector2, Vector2, float, Vector4>> batch = new List<MyTuple<int, string, Vector2, Vector2, float, Vector4>>();
			public void HandleRequests(List<MyIGCMessage> unicasts)
			{
				foreach (var msg in unicasts)
				{
					if (msg.Tag.Contains("shell.get"))
					{
						var d = (string)msg.Data;
						if (_dataGetter.ContainsKey(d))
						{
							var payload = _dataGetter[d]();
							// one connection support. Null means no update.
							if (payload != null)
							{
								_igc.SendUnicastMessage(msg.Source, "shell.text", new MyTuple<string, string>(d, payload));
								UnicastCtr++;
							}
						}
						else if (_spriteGetter.ContainsKey(d))
						{
							var payload = _spriteGetter[d]();
							// one connection support. Null means no update.
							if (payload != null && payload.Any())
							{
								_igc.SendUnicastMessage(msg.Source, "shell.sprites",
									new MyTuple<string, ImmutableArray<MyTuple<int, string, Vector2, Vector2, float, Vector4>>>(d, GetIgcSpriteFrame(payload)));
								UnicastCtr++;
							}
						}
					}
					else if (msg.Tag == "shell.options")
					{
						// pb custom name, feed <key, type> coll
						// var d = (MyTuple<string, ImmutableArray<MyTuple<string, byte>>>)msg.Data;

						var items = _dataGetter.Keys.Select(x => new MyTuple<string, byte>(x, 2)).ToList();
						items.AddRange(_spriteGetter.Keys.Select(x => new MyTuple<string, byte>(x, 1)));
						_igc.SendUnicastMessage(msg.Source, "shell.options", new MyTuple<string, ImmutableArray<MyTuple<string, byte>>>(HostName, items.ToImmutableArray()));
					}
				}

				E.Echo($"Shell handler unicast counter: {UnicastCtr}");

			}

			public ImmutableArray<MyTuple<int, string, Vector2, Vector2, float, Vector4>> GetIgcSpriteFrame(IEnumerable<MySprite> payload)
			{
				batch.Clear();
				foreach (var s in payload)
					batch.Add(new MyTuple<int, string, Vector2, Vector2, float, Vector4>(
					s.Type == SpriteType.TEXT ? (int)s.Alignment : -1, // text Alignment enum, -1 means it's not a text
					s.Data,
					s.Size.HasValue ? new Vector2(s.Size.Value.X / vSize.X, s.Size.Value.Y / vSize.Y) : Vector2.One,
					s.Position.HasValue ? new Vector2(s.Position.Value.X / (vSize.X * 0.5f) - 1, s.Position.Value.Y / (vSize.Y * 0.5f) - 1) : Vector2.Zero,
					s.RotationOrScale,
					s.Color ?? Color.White
				));
				return batch.ToImmutableArray();
			}
		}

		/*
		struct SpriteViewPortItem
		{
			public string Src;
			public Vector2 PosPx;
			public Vector2 SizePx;
			public List<MySprite> BackSprites;
			public List<List<MySprite>> Frames;
		}

		SpriteViewPortItem testImportedS = new SpriteViewPortItem() { Src = I2, PosPx = new Vector2(512 * 0.5f, 512 * 0.5f), SizePx = new Vector2(512 * 0.45f, 512 * 0.45f), BackSprites = new List<MySprite>() };

		void PrepareImportedSprites()
		{
			var frames = HumbleLoadSprites(I2, testImportedS.PosPx, testImportedS.SizePx);
			if (frames.Count > 0)
				testImportedS.BackSprites = frames[0].Item2;

			testImportedS.Frames = new List<List<MySprite>>();
			foreach (var f in frames.Skip(1))
			{
				testImportedS.Frames.Add(f.Item2);
			}
		}

		const string I2 = "Sprites/default....";

		List<MyTuple<string, List<MySprite>>> HumbleLoadSprites(string src, Vector2 pos, Vector2 rect)
		{
			var str = src.Replace("Hud Dog", Logo);

			var res = new List<MyTuple<string, List<MySprite>>>();

			var values = str.Split('\n').ToDictionary(s => s.Split('=')[0], s => string.Join("=", s.Split('=').Skip(1)));

			// Sprites/def=PosX=111,PosY=222,Type=aaa|PosX=111,PosY=222,Type=aaa
			// Sprites/frame1=PosX=111,PosY=222,Type=aaa|PosX=111,PosY=222,Type=aaa
			foreach (var spriteGroup in values.Where(v => v.Key.Contains("Sprites")))
			{
				var groupName = spriteGroup.Key.Split('/')[1];

				if (!string.IsNullOrEmpty(spriteGroup.Value))
				{
					var sprites = new List<MySprite>();
					res.Add(new MyTuple<string, List<MySprite>>(groupName, sprites));

					var entries = spriteGroup.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string s1 in entries)
					{
						//throw new NullReferenceException("s1: " + s1);
						//E.DebugToPanel("s1:" + s1 + "x");
						//E.FlushDebugPanel();

						var vals = s1.Split(',').ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);
						Color c = new Color(byte.Parse(vals["ColorR"]), byte.Parse(vals["ColorG"]), byte.Parse(vals["ColorB"]), byte.Parse(vals["ColorA"]));
						MySprite spr;
						SpriteType type;
						if (Enum.TryParse(vals["Type"], out type) && (type == SpriteType.TEXT))
						{
							spr = MySprite.CreateText(vals["Brush"], "Debug", c);
						}
						else
						{
							Vector2 sz = new Vector2(float.Parse(vals["SizeX"]), float.Parse(vals["SizeY"]));
							var size = new Vector2(sz.X > 1 ? sz.X : sz.X * rect.X, sz.Y > 1 ? sz.Y : sz.Y * rect.Y);
							spr = new MySprite(SpriteType.TEXTURE, vals["Brush"], size: size, color: c);
						}
						var posInPort = new Vector2(float.Parse(vals["PosX"]) * rect.X / 2f, float.Parse(vals["PosY"]) * rect.Y / 2f);
						spr.Position = pos + posInPort;
						var scale = float.Parse(vals["Rotation"]);
						if (type == SpriteType.TEXT)
							spr.RotationOrScale = scale * (rect.Y);
						else
							spr.RotationOrScale = scale;

						sprites.Add(spr);
					}
				}
			}

			return res;
		}
		*/
