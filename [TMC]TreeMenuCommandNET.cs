// TreeMenuCommand (c) cheerkin
		// https://steamcommunity.com/sharedfiles/filedetails/?id=2789500371


		string Ver = "1.0.213";
		public static class Config
		{
			public static readonly string DefaultTextPanelName = "menu-panel"; // Menu tree printout
			public static readonly string OutputTextPanelName = "menu-output"; // Output display (target PB echo dump, idea by Etamit)
			public static readonly int MenuScreenIndex = -1; // index of cockpit screen in case menu-panel is absent
			public static readonly int OutputScreenIndex = 2; // index of cockpit screen in case menu-panel is absent
			public static readonly char AliasDelimiter = ';';
			public static readonly string AliasKey = "DisplayName";
			public static readonly string TimersMenuTopNodeName = "Timers";
			public static readonly string IgcIdPlaceholder = "{igc.me}";

			public static readonly float InterfaceScaleLargeGrid = 1.4f;
			public static readonly float InterfaceScaleSmallGrid = 1.2f;
			public static readonly float InterfaceScaleInternal = 0.6f;

			public static readonly bool AutoChangeSeatControlGyros = true;
			public static readonly Color MainColor = new Color(r: 255, g: 195, b: 110);
			public static readonly float MouseSpeedMult = 1f;
			public static readonly bool MouseFlipY = false;
			public static readonly string EscapeMenuItem = "..."; // Set to empty "" if you want it to disappear

			public static readonly string OP_BackgroundFileName = "bg.opa";
			public static readonly string AR_PanelName = "ar-panel";

			public static readonly bool DrawQuickActionNumbers = true;
		}

		public Program()
		{
			E.Init(Echo, GridTerminalSystem, Me);

			menuController = new MenuController();
			var pbs = new List<IMyProgrammableBlock>();
			GridTerminalSystem.GetBlocksOfType(pbs, x => x.IsSameConstructAs(Me));
			igcN = new IGCNetwork(IGC, pbs, menuController, Me.CubeGrid.CustomName, Me.CubeGrid.EntityId);
			menuController.igcN = igcN;

			Runtime.UpdateFrequency = UpdateFrequency.Once;

			var gr = GetThisBlockGroup(Me);
			if (gr == null)
			{
				Runtime.UpdateFrequency = UpdateFrequency.None;
				Echo("Can't find hardware group containing this PB, stopping now.");
			}
			else
			{
				var tools = GetGroupBlocks<IMyShipToolBase>(gr);
				toolActivator = tools.FirstOrDefault();

				var ctrls = GetGroupBlocks<IMyShipController>(gr);
				UserCtrlListener = new InputListener(ctrls, toolActivator, () => runCount);

				float textScale;
				InitSurface(gr, out textScale);

				soundClickFeedback = GetGroupBlocks<IMySoundBlock>(gr).FirstOrDefault();

				Action<string, string> localHandler = (rootId, cmd) =>
				{
					if (rootId == "Timer triggers")
					{
						localTimers.First(t => t.CustomName == cmd)?.Trigger();
					}
					else
					{
						var pb = pbs.FirstOrDefault(b => b.CustomName.Contains(rootId));
						if (pb != null)
						{
							E.DebugLog($"Found PB named '{pb.CustomName}', trying to run with argument '{cmd}'");
							var res = pb.TryRun(cmd);
							E.DebugLog($"Run result: {res}");
							surfaceForOutput?.WriteText(pb.DetailedInfo);
							if (res)
								soundClickFeedback?.Play();
						}
						else
						{
							bool res;
							if (ogs != null && ogs.terminalBlocks.Any())
								res = ParseAndExecuteTCommand(cmd, ogs.terminalBlocks);
							else
								res = ParseAndExecuteTCommand(cmd);
							if (res)
								soundClickFeedback?.Play();
						}
					}
				};

				igcN.SetLocalHandler(localHandler);

				if (string.IsNullOrEmpty(Me.CustomData))
					this.menuCommander = new TreeMenuCommander(igcN.HandleCommand);
				else
					this.menuCommander = new TreeMenuCommander(Me.CustomData, igcN.HandleCommand, localHandler, IGC.Me.ToString());

				gr.GetBlocksOfType(localTimers);
				var timersNode = new TreeNode<MenuItem>("Timer triggers", new MenuItem(Config.TimersMenuTopNodeName));
				foreach (var t in localTimers)
				{
					timersNode.Add(new TreeNode<MenuItem>(t.CustomName));
				}
				menuCommander.Root.Add(timersNode);

				var transponders = new List<IMyTransponder>();
				gr.GetBlocksOfType(transponders);
				var tp = transponders.FirstOrDefault();
				if (tp != null)
				{
					E.DebugLog($"Starting watching transponder {tp.CustomName}");
					TW = new TransponderWatch(tp, localHandler, menuCommander.Root.ID);
					var tpData = menuCommander.Root.FirstOrDefault(n => n.ID.ToLower() == "transponder");
					if (tpData != null)
					{
						Runtime.UpdateFrequency = UpdateFrequency.Update10;
						foreach (var binding in tpData)
						{
							var p = binding.Payload;
							var parts = binding.ID.Split(':');
							TW.TpBindings.Add(int.Parse(parts[0]), string.Join(":", parts.Skip(1)));
							E.DebugLog($"Added binding to channel '{parts[0]}' command: {string.Join(":", parts.Skip(1))}");
						}
						tpData.Remove();
					}

				}

				menuController.LocalModel = menuCommander;
				menuController.IsLocal = true;

				menuCommander.Options();

				if (surfaceForMenu != null)
				{
					orbitalPainterImporter = new OrbitalPainterImporter(IGC, surfaceForMenu.SurfaceSize);
					guiH = new GuiHandler(igcN, surfaceForMenu, menuController, textScale, orbitalPainterImporter);
					if (surfaceForMenu is IMyTextPanel)
					{
						var sb = new List<IMySensorBlock>();
						gr.GetBlocksOfType(sb);
						if (sb.Any())
						{
							handsFreeActivator = new HandsFreeActivator(sb, (IMyTextPanel)surfaceForMenu);
						}
					}

					shellHandler = new ShellHandler(IGC, "tmc_net", surfaceForMenu.TextureSize);
					shellHandler.AddHandler("GetTmcView", () => guiH.GetFrameSprites());
				}

				var conns = new List<IMyShipConnector>();
				gr.GetBlocksOfType(conns);
				if (conns.Any())
				{
					ogs = new OtherGridScanner(conns.Single(), Runtime);
					Runtime.UpdateFrequency = UpdateFrequency.Update1;
				}
			}
		}

		IMySoundBlock soundClickFeedback;
		List<IMyTimerBlock> localTimers = new List<IMyTimerBlock>();
		IMyTextSurface surfaceForMenu;
		IMyTextSurface surfaceForOutput;
		IMyTextSurface arSurface;
		IMyShipToolBase toolActivator;
		private void InitSurface(IMyBlockGroup g, out float textScale)
		{
			var panels = GetGroupBlocks<IMyTextPanel>(g);

			var mainLcd = panels.FirstOrDefault(x => x.CustomName.Contains(Config.DefaultTextPanelName)) ?? panels.FirstOrDefault();
			if (mainLcd != null)
			{
				textScale = mainLcd.CubeGrid.GridSizeEnum == MyCubeSize.Large ? Config.InterfaceScaleLargeGrid : Config.InterfaceScaleSmallGrid;
				E.DebugLog($"Menu display found: {mainLcd.CustomName}, using scale: {textScale:f2}");
				surfaceForMenu = mainLcd;
			}
			else
			{
				surfaceForMenu = UserCtrlListener.GetCockpitSurface(Config.MenuScreenIndex);
				if (surfaceForMenu != null)
				{
					textScale = Config.InterfaceScaleInternal;
					E.DebugLog($"Using cockpit screen: {Config.MenuScreenIndex}, using scale: {textScale:f2}");
				}
				else
					textScale = -1;
			}

			if (surfaceForMenu != null)
			{
				surfaceForMenu.ContentType = ContentType.SCRIPT;
				surfaceForMenu.ScriptBackgroundColor = Color.Transparent;
				surfaceForMenu.Script = "";
			}
			else
			{
				surfaceForMenu = Me.GetSurface(0);
				E.DebugLog($"Resorting to PB built-in screen output. Using scale 'InterfaceScaleInternal' ({Config.InterfaceScaleInternal})");
				textScale = Config.InterfaceScaleInternal;
			}

			//--- Output Display ---
			surfaceForOutput = panels.FirstOrDefault(x => x.CustomName.Contains(Config.OutputTextPanelName)) ?? panels.Skip(1).FirstOrDefault();
			if (surfaceForOutput != null)
			{
				E.DebugLog($"Output display found: {((IMyTerminalBlock)surfaceForOutput).CustomName}");
			}
			else
			{
				surfaceForOutput = UserCtrlListener.GetCockpitSurface(Config.OutputScreenIndex);
			}
			if (surfaceForOutput != null)
			{
				surfaceForOutput.ContentType = ContentType.TEXT_AND_IMAGE;
				E.DebugLog($"Output goes to {surfaceForOutput.DisplayName}");
			}

			arSurface = panels.FirstOrDefault(x => x.CustomName.Contains(Config.AR_PanelName));
			if (arSurface != null)
			{
				arSurface.ContentType = ContentType.SCRIPT;
				arSurface.Script = "";
				arSurface.ScriptBackgroundColor = Color.Transparent;
			}

			//--- Output Display end ---
		}

		private TreeMenuCommander menuCommander;

		int runCount;
		bool captureControls = false;
		static string lastKey = "";
		bool needsUpdate = true;
		int inputIdleTickCounter;

		List<MyIGCMessage> uniMsgs = new List<MyIGCMessage>();

		private void Main(string arg)
		{
			runCount++;
			E.T++;
			E.Echo($"Version: {Ver}");
			E.Echo($"Run count: {runCount}");

			uniMsgs.Clear();
			while (IGC.UnicastListener.HasPendingMessage)
			{
				uniMsgs.Add(IGC.UnicastListener.AcceptMessage());
			}

			if (ogs != null)
			{
				E.Echo($"ogs.outputX: {ogs.terminalBlocks.Count}");
				E.Echo($"ogs.InstCount: {ogs.InstCount}");
				ogs.Handle(runCount);
			}

			shellHandler.HandleRequests(uniMsgs);
			orbitalPainterImporter.HandleRequests(uniMsgs);

			try
			{
				igcN.HandleTick(uniMsgs);
				E.Echo("IGC peers: ");
				E.Echo(string.Join("\n", igcN.Peers));

				TW?.HandleTp();

				if (surfaceForMenu != null)
				{
					UserCtrlListener.PrepareBeforeTick(uniMsgs);
				}

				if (!string.IsNullOrEmpty(arg))
				{
					if (arg.Length == 1)
					{
						int quickActionNum;
						if (int.TryParse(arg, out quickActionNum))
						{
							menuController.Enter(quickActionNum - 1);
						}
					}

					if (arg == "cc")
					{
						captureControls = !captureControls;
						if (Config.AutoChangeSeatControlGyros)
							UserCtrlListener.GetControlledCockpit()?.SetValueBool("ControlGyros", !captureControls);
						Runtime.UpdateFrequency = captureControls || (TW != null) ? UpdateFrequency.Update1 : UpdateFrequency.Once;
					}

					if (arg == "sens")
					{
						captureControls = true;
						Runtime.UpdateFrequency = UpdateFrequency.Update1;
					}

					if (arg.Contains("q"))
					{
						if (ogs != null && ogs.terminalBlocks.Any())
							ParseAndExecuteTCommand(arg.Substring(arg.IndexOf(':') + 1), ogs.terminalBlocks);
						else
							ParseAndExecuteTCommand(arg.Substring(arg.IndexOf(':') + 1));
					}

					switch (arg)
					{
						case "up":
							{
								lastKey = "up";
								menuController.Up();
								break;
							}
						case "esc":
							{
								lastKey = "esc";
								menuController.Escape();
								break;
							}
						case "down":
							{
								lastKey = "down";
								menuController.Down();
								break;
							}
						case "enter":
							{
								lastKey = "enter";
								menuController.Enter();
								break;
							}
						case "ctn":
							{
								lastKey = "ctn";
								igcN.NextContext();
								break;
							}
						case "ctp":
							{
								lastKey = "ctp";
								igcN.PrevContext();
								break;
							}
					}
				}

				if (surfaceForMenu != null)
				{
					UserCtrlListener.PrepareBeforeTick(uniMsgs);

					bool click = false;
					bool sensePlayer = false;
					bool uiAction = false;

					Vector2 projectedCursorPosition = Vector2.Zero;
					if (handsFreeActivator != null)
					{
						bool Q = false;
						bool E = false;
						var rel = MyRelationsBetweenPlayerAndBlock.Owner;
						sensePlayer = handsFreeActivator.IsInteractive(ref projectedCursorPosition, ref E, ref Q, ref rel);
						uiAction = sensePlayer;
						click = E;
						if (Q)
							menuController.Escape();
					}

					var surfOffset = new RectangleF(
						(surfaceForMenu.TextureSize - surfaceForMenu.SurfaceSize) / 2f, surfaceForMenu.SurfaceSize
					);

					var controlledStation = UserCtrlListener.GetControlledCockpit();

					Vector2 cursorPosition;
					if (captureControls && (controlledStation != null))
					{
						var r = UserCtrlListener.GetRot();
						if (r.LengthSquared() > 0)
						{
							if (Config.MouseFlipY)
								r.X *= -1;
							var adj = UserCtrlListener.Cumulative + r * Config.MouseSpeedMult;
							adj.X = Math.Min(Math.Abs(adj.X), surfaceForMenu.SurfaceSize.Y / 2f) * Math.Sign(adj.X);
							adj.Y = Math.Min(Math.Abs(adj.Y), surfaceForMenu.SurfaceSize.X / 2f) * Math.Sign(adj.Y);
							UserCtrlListener.Cumulative = adj;
							uiAction = true;
						}
					}
					else if (UserCtrlListener.ReceivedUserActionsFromIGC)
					{
						var ovr = Vector2.Zero;
						UserCtrlListener.GetCursorOverride(ref ovr);
						var adj = new Vector2(ovr.X * surfaceForMenu.TextureSize.X / 2, ovr.Y * surfaceForMenu.TextureSize.Y / 2);
						UserCtrlListener.Cumulative.X = adj.Y;
						UserCtrlListener.Cumulative.Y = adj.X;
						uiAction = true;
					}

					if ((captureControls && (controlledStation != null)) || UserCtrlListener.ReceivedUserActionsFromIGC)
					{
						if (UserCtrlListener.KeyReleased("e") || UserCtrlListener.KeyReleased("tool"))
						{
							lastKey = "E";
							click = true;
							uiAction = true;
						}
						else if (UserCtrlListener.KeyReleased("q"))
						{
							lastKey = "Q";
							menuController.Escape();
							uiAction = true;
						}
					}

					if (sensePlayer)
						cursorPosition = projectedCursorPosition;
					else
						cursorPosition = surfOffset.Position + new Vector2(UserCtrlListener.Cumulative.Y, UserCtrlListener.Cumulative.X) + surfOffset.Size / 2;

					if (needsUpdate)
					{
						guiH.Handle(click, cursorPosition, captureControls || UserCtrlListener.ReceivedUserActionsFromIGC);
						surfaceForMenu.ContentType = ContentType.SCRIPT;
						surfaceForMenu.ScriptBackgroundColor = Color.Transparent;
						surfaceForMenu.Script = "";
					}

					if (!uiAction && (inputIdleTickCounter++ > 60))
					{
						inputIdleTickCounter = 0;
						needsUpdate = false;
					}
					else
					{
						needsUpdate = true;
					}

					if (arSurface != null && arSurface is IMyTextPanel)
					{
						var c = UserCtrlListener.GetControlledCockpit();
						if (c != null)
						{
							igcN.PollPeerPositions();
							using (var frame = arSurface.DrawFrame())
							{
								foreach (var peerPos in igcN.PeerMeta)
								{
									if (peerPos.Value.Pos != Vector3D.Zero)
									{
										Vector2 proj;
										if (ProjectOnScreen(peerPos.Value.Pos, (IMyTextPanel)arSurface, c.WorldMatrix.Translation, out proj))
										{
											var lockSpr = new MySprite(SpriteType.TEXTURE, "SquareHollow", size: new Vector2(40f, 40f), color: Color.Goldenrod);
											if (igcN.GetSelectedPeerAddr() == peerPos.Key)
												lockSpr.RotationOrScale = runCount / 100f;
											lockSpr.Position = proj;
											frame.Add(lockSpr);
										}
									}
								}
							}
							arSurface.ContentType = ContentType.TEXT_AND_IMAGE;
							arSurface.ContentType = ContentType.SCRIPT;
						}
					}
				}
			}
			catch (Exception ex)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("LocalModel SelectedId: " + menuController.LocalModel.CurrentNode.ID);
				sb.AppendLine("RemoteSelectedId: " + menuController.RemoteSelectedId);
				sb.AppendLine($"Last key: {lastKey}");
				sb.AppendLine(ex.ToString());
				Runtime.UpdateFrequency = UpdateFrequency.None;
				E.Echo(sb.ToString());
				E.DebugLog(sb.ToString());
				return;
			}
		}

		List<T> GetBsOfType<T>(Func<T, bool> pred, List<T> cache = null) where T : class, IMyTerminalBlock
		{
			var b = new List<T>();
			if (cache == null)
				GridTerminalSystem.GetBlocksOfType(b, pred);
			else
				b = cache.Where(pred).ToList();
			return b;
		}

		bool ParseAndExecuteTCommand(string commands, List<IMyTerminalBlock> cache = null)
		{
			// foreach:IType:Action
			// first:name:Action
			// group:name:Action
			// report:name
			//var b = new List<IMyTerminalBlock>();
			var cmds = commands.Split(new[] { "],[" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			if (cmds.Count > 1)
			{
				cmds = cmds.Select(s => s.Trim('[', ']')).ToList();
			}
			bool result = false;

			foreach (var cmd in cmds)
			{
				var parts = cmd.Split(':');
				// first:name:PropertyId:Value
				// first:a-hud-dog:set-value:sadas:10
				string propertyName = null;
				if (parts.Length > 3)
					propertyName = parts[2];

				E.DebugLog($"{cmd} cache count: {cache?.Count}");

				if (cmd.StartsWith("report") && (parts.Length > 1))
				{
					//GridTerminalSystem.GetBlocksOfType(b, x => x.CustomName.Contains(parts[1]));
					var b = GetBsOfType<IMyTerminalBlock>(x => x.CustomName.Contains(parts[1]), cache);
					foreach (var x in b)
					{
						E.DebugLog($"Type: {x.GetType().Name}");
						var acts = new List<ITerminalAction>();
						x.GetActions(acts);
						foreach (var a in acts)
						{
							E.DebugLog($"Action: {a.Id} ({a.Name})");
						}
						var props = new List<ITerminalProperty>();
						x.GetProperties(props);
						foreach (var a in props)
						{
							E.DebugLog($"Property: {a.Id} ({a.TypeName})");
						}
						result = true;
					}
				}
				else if (cmd.StartsWith("foreach") && (parts.Length > 2))
				{
					// foreach:MyBatteryBlock:OnOff_Off
					var b = GetBsOfType<IMyTerminalBlock>(x => parts[1].Contains(x.GetType().Name), cache);
					foreach (var block in b)
					{
						if (!string.IsNullOrEmpty(propertyName))
						{
							if ((propertyName == "Run") && (block is IMyProgrammableBlock))
								result |= TryRunPb(block, string.Join(":", parts.Skip(3)));
							else if (propertyName == "CustomData")
								result |= TrySetText(parts[3], (s, append) => block.CustomData = append ? block.CustomData + s : s);
							else if ((propertyName == "PublicText") && (block is IMyTextPanel))
								result |= TrySetText(parts[3], (s, append) => ((IMyTextPanel)block).WriteText(s, append));
							else
								result |= TrySetProperty(block, propertyName, parts[3]);
						}
						else
							result |= TryRunAction(block, parts[2]);
					}
				}
				else if (cmd.StartsWith("first") && (parts.Length > 2))
				{
					//GridTerminalSystem.GetBlocksOfType(b, x => x.CustomName.Contains(parts[1]));
					var b = GetBsOfType<IMyTerminalBlock>(x => x.CustomName.Contains(parts[1]), cache);
					var firstB = b.FirstOrDefault();
					if (firstB != null)
					{
						if (!string.IsNullOrEmpty(propertyName))
						{
							if ((propertyName == "Run") && (firstB is IMyProgrammableBlock))
								result |= TryRunPb(firstB, string.Join(":", parts.Skip(3)));
							else if (propertyName == "CustomData")
								result |= TrySetText(parts[3], (s, append) => firstB.CustomData = append ? firstB.CustomData + s : s);
							else if ((propertyName == "PublicText") && (firstB is IMyTextPanel))
								result |= TrySetText(parts[3], (s, append) => ((IMyTextPanel)firstB).WriteText(s, append));
							else
								result |= TrySetProperty(firstB, propertyName, parts[3]);
						}
						else
							result |= TryRunAction(firstB, parts[2]);
					}
				}
				else if (cmd.StartsWith("group") && (parts.Length > 2))
				{
					var g = GridTerminalSystem.GetBlockGroupWithName(parts[1]);
					if (g != null)
					{
						var b = new List<IMyTerminalBlock>();
						g.GetBlocks(b);

						foreach (var block in b)
						{
							if (!string.IsNullOrEmpty(propertyName))
							{
								if ((propertyName == "Run") && (block is IMyProgrammableBlock))
									result |= TryRunPb(block, string.Join(":", parts.Skip(3)));
								else
									result |= TrySetProperty(block, propertyName, parts[3]);
							}
							else
								result |= TryRunAction(block, parts[2]);
						}
					}
				}
			}

			return result;
		}

		bool TryRunPb(IMyTerminalBlock x, string arg)
		{
			if (x is IMyProgrammableBlock)
			{
				bool res = ((IMyProgrammableBlock)x).TryRun(arg);
				E.DebugLog($"Programmable block '{x.CustomName}' run result :{res}");
				return res;
			}
			return false;
		}
		bool TryRunAction(IMyTerminalBlock x, string action)
		{
			if (x.HasAction(action))
			{
				x.ApplyAction(action);
				E.DebugLog($"Ran action '{action}' on '{x.CustomName}'");
				return true;
			}
			else
				E.DebugLog($"No action '{action}' found for '{x.CustomName}'");
			return false;
		}
		bool TrySetProperty(IMyTerminalBlock x, string property, string value)
		{
			bool cumulative = false;
			if (value.StartsWith("$"))
			{
				cumulative = true;
				value = value.TrimStart('$');
			}
			var prop = x.GetProperty(property);
			if (prop != null)
			{
				if (prop.Is<Color>())
				{
					var rgb = value.Trim('#');
					try
					{
						var c = new Color(
								int.Parse(rgb.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
								int.Parse(rgb.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
								int.Parse(rgb.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)
							);
						x.SetValue(property, c);
						return true;
					}
					catch
					{
						E.DebugLog("Failed to parce Color value. Needs to be hex rgb, e.g. #085FA6");
					}
				}
				//x.SetValueColor(property, new Color() )
				if (prop.Is<float>())
				{
					var current = prop.AsFloat().GetValue(x);
					float v;
					if (float.TryParse(value, out v))
					{
						x.SetValue(property, cumulative ? current + v : v);
						return true;
					}
				}
				if (prop.Is<bool>())
				{
					var current = prop.AsBool().GetValue(x);
					bool v;
					if (cumulative)
						x.SetValue(property, !current);
					else if (bool.TryParse(value, out v))
						x.SetValue(property, v);
					return true;
				}
			}
			return false;
		}
		bool TrySetText(string value, Action<string, bool> writer)
		{
			bool cumulative = false;
			value = value.Replace("\\n", "\n");
			if (value.StartsWith("$"))
			{
				cumulative = true;
				value = value.TrimStart('$');
			}
			writer(value, cumulative);
			return true;
		}

		HandsFreeActivator handsFreeActivator;
		class HandsFreeActivator
		{
			List<IMySensorBlock> srs = new List<IMySensorBlock>();
			//List<IMyTextPanel> pnls = new List<IMyTextPanel>();
			IMyTextPanel mainPanel;

			public HandsFreeActivator(List<IMySensorBlock> sensorBlocks, IMyTextPanel panel)
			{
				srs = sensorBlocks;
				mainPanel = panel;
				srs.ForEach(x => x.DetectPlayers = true);
			}

			List<MyDetectedEntityInfo> entities = new List<MyDetectedEntityInfo>();
			public bool IsInteractive(ref Vector2 cursor, ref bool E, ref bool Q, ref MyRelationsBetweenPlayerAndBlock myRelations)
			{
				if (srs.Any())
				{
					foreach (var sens in srs)
					{
						sens.DetectedEntities(entities);
						foreach (var entity in entities)
						{
							if (entity.Type == MyDetectedEntityType.CharacterHuman)
							{
								myRelations = entity.Relationship;
								var povRef = entity.Position + entity.Orientation.Up * 0.75f + entity.Orientation.Forward * 0.1f;
								var res = entity.Orientation;
								res.Translation = povRef;

								if (ProjectOnScreen(res.Translation + res.Forward * 1000, mainPanel, res.Translation, out cursor, true))
								{
									//return CheckShake(entity.Position, entity.Orientation);
									CheckRollCW(entity.Orientation, ref E, ref Q);
								}
								return true; // only 1st player is considered
							}
						}
					}
				}
				return false;
			}

			void CheckRollCW(MatrixD playerOrientation, ref bool E, ref bool Q)
			{
				var playerPos = playerOrientation.Translation;

				if (!lastUpNorm.HasValue)
				{
					lastUpNorm = playerOrientation.Up;
				}
				else
				{
					if ((lastPos - playerPos).Length() < 0.1) // won't work for moving grid-player system
					{
						if (lastUpNorm != playerOrientation.Up) // rolling
						{
							var p = Vector3D.Normalize(Vector3D.Cross(lastUpNorm.Value, playerOrientation.Up));
							lastUpNorm = playerOrientation.Up;
							lastPos = playerPos;

							var dtNow = DateTime.Now;
							if ((dtNow - lastActivationStamp).TotalSeconds < 0.5f)
								return;

							if (Vector3D.Dot(p, playerOrientation.Forward) > 0.98)
							{
								E = true;
								lastActivationStamp = dtNow;
							}
							if ((Vector3D.Dot(p, playerOrientation.Backward) > 0.98))
							{
								Q = true;
								lastActivationStamp = dtNow;
							}

						}
					}
					lastUpNorm = playerOrientation.Up;
					lastPos = playerPos;
				}
			}

			public bool? CW;
			public string CW_log;
			Vector3D? lastUpNorm;
			Vector3D lastPos;
			DateTime lastActivationStamp;
			public List<bool> sequence = new List<bool>();
			public bool CheckShake(Vector3D playerPos, MatrixD playerOrientation)
			{
				CW_log = "";
				if (!lastUpNorm.HasValue)
				{
					lastUpNorm = playerOrientation.Up;
				}
				else
				{
					if ((lastPos - playerPos).Length() < 0.1)
					//if (lastPos == playerPos)
					{
						CW_log += "dPos: " + (lastPos - playerPos).Length().ToString("f2");
						if (lastUpNorm != playerOrientation.Up) // rolling
						{
							var p = Vector3D.Normalize(Vector3D.Cross(lastUpNorm.Value, playerOrientation.Up));
							CW_log += " dot: " + Vector3D.Dot(p, playerOrientation.Backward).ToString("f2");
							if (Vector3D.Dot(p, playerOrientation.Forward) > 0.98)
							{
								CW = true;
							}
							if ((Vector3D.Dot(p, playerOrientation.Backward) > 0.98))
							{
								CW = false;
							}
						}
						else
						{
							if (CW.HasValue)
								sequence.Add(CW.Value);
							CW = null;

							var pattern = new bool[] { true, false, true };
							if (sequence.Count >= pattern.Length)
							{
								bool match = true;
								for (int n = 0; n < pattern.Length; n++)
								{
									match = pattern[n] == sequence[n];
								}
								sequence.Clear();
								return match;
							}
						}
					}
					else
					{
						sequence.Clear();
					}
				}

				lastUpNorm = playerOrientation.Up;
				lastPos = playerPos;
				return false;
			}

		}

		GuiHandler guiH;
		public class GuiHandler
		{
			IGCNetwork _igcN;
			IMyTextSurface _surfaceForMenu;
			MenuController _menuController;
			int textHeightPx;
			float _textScale;
			OrbitalPainterImporter _opi;

			public GuiHandler(IGCNetwork igcN, IMyTextSurface surfaceForMenu, MenuController menuController, float textScale, OrbitalPainterImporter opi)
			{
				_igcN = igcN;
				_surfaceForMenu = surfaceForMenu;
				_menuController = menuController;
				_textScale = textScale;
				textHeightPx = (int)surfaceForMenu.MeasureStringInPixels(new StringBuilder("HeightMeasure"), "Debug", textScale).Y;
				_opi = opi;
			}

			List<MySprite> lastFrameSprites = new List<MySprite>();
			List<ClickableArea> areas = new List<ClickableArea>();
			public void Handle(bool click, Vector2 cursorPosition, bool drawCursor)
			{
				var surfOffset = new RectangleF(
						(_surfaceForMenu.TextureSize - _surfaceForMenu.SurfaceSize) / 2f,
						_surfaceForMenu.SurfaceSize
					);

				/*
				MySprite ovrState = MySprite.CreateText(sb.ToString(), "Debug", new Color(1f), 0.6f, TextAlignment.LEFT);
				//ovrState.Position = new Vector2(centerX, surface.SurfaceSize.Y / 5f);
				ovrState.Position = new Vector2(surfaceForMenu.SurfaceSize.X / 4f, surfaceForMenu.SurfaceSize.Y / 4f);
				frame.Add(ovrState);
				*/

				// building items
				areas.Clear();
				int n = 0;
				var w = _surfaceForMenu.TextureSize.X * 0.8f;
				var h = textHeightPx * 1.2f;

				var currentNode = _menuController.GetItemsForView();

				if (!string.IsNullOrEmpty(Config.EscapeMenuItem) && (currentNode.Parent != null))
				{
					var bPosBack = surfOffset.Position + new Vector2(_surfaceForMenu.SurfaceSize.X / 2f, h + h * n);
					var spriteBback = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: Color.Transparent);
					var btnBack = new ClickableArea(bPosBack, new Vector2(w, h), spriteBback, _menuController.Escape);
					btnBack.AddCaption(new Vector2(10, textHeightPx / 2), Config.EscapeMenuItem, _textScale);
					areas.Add(btnBack);
					n++;
				}

				int index = 0;
				foreach (var node in currentNode)
				{
					Vector2 itemPosition = surfOffset.Position + new Vector2(_surfaceForMenu.SurfaceSize.X / 2f, h + h * n);

					Action onClick = () => _menuController.Enter(node.ID);
					Action onHover = () => _menuController.SelectID(node.ID);

					ClickableArea clickableArea = new ClickableArea(itemPosition, new Vector2(w, h),
						new MySprite(SpriteType.TEXTURE, "SquareSimple", color: Color.Transparent), onClick, onHover);

					var caption = node.Payload?.DisplayName ?? node.ID;
					// intended for toolbar 1-9 numbers
					if (!drawCursor && Config.DrawQuickActionNumbers && (++index < 10))
						caption = $"{index}: {caption}";
					if ((_menuController.IsLocal && _menuController.LocalModel.SelectedID == node.ID) ||
							(!_menuController.IsLocal && _menuController.RemoteSelectedId == node.ID))
						caption = ">" + caption;

					clickableArea.AddCaption(new Vector2(10, textHeightPx / 2), caption, _textScale);

					areas.Add(clickableArea);
					n++;
				}

				var bPosPrev = surfOffset.Position + new Vector2(_surfaceForMenu.SurfaceSize.X / 20f, _surfaceForMenu.SurfaceSize.Y - 30);
				var spriteBprev = new MySprite(SpriteType.TEXTURE, "Triangle", color: new Color(1f));
				spriteBprev.RotationOrScale = -(float)Math.PI / 2f;
				ClickableArea clickableAreaBprev = new ClickableArea(bPosPrev, new Vector2(20, 20), spriteBprev, _igcN.PrevContext);
				areas.Add(clickableAreaBprev);

				var bPosNext = surfOffset.Position + new Vector2(_surfaceForMenu.SurfaceSize.X * 0.95f, _surfaceForMenu.SurfaceSize.Y - 30);
				var spriteBnext = new MySprite(SpriteType.TEXTURE, "Triangle", color: new Color(1f));
				spriteBnext.RotationOrScale = (float)Math.PI / 2f;
				ClickableArea clickableAreaBnext = new ClickableArea(bPosNext, new Vector2(20, 20), spriteBnext, _igcN.NextContext);
				areas.Add(clickableAreaBnext);

				// building frame
				lastFrameSprites.Clear();
				lastFrameSprites.AddRange(_opi.GetBackground());

				string cap;
				if (_igcN.currentContext == 0)
					cap = "Local";
				else
				{
					var cCont = _igcN.Peers[_igcN.currentContext - 1];
					cap = _igcN.PeerMeta.ContainsKey(cCont) ? _igcN.PeerMeta[cCont].Name : cCont.ToString();
				}
				MySprite bottom = MySprite.CreateText(cap, "Debug", Config.MainColor, _textScale, TextAlignment.CENTER);
				bottom.Position = surfOffset.Position + new Vector2(_surfaceForMenu.SurfaceSize.X / 2f, _surfaceForMenu.SurfaceSize.Y - 30 - textHeightPx / 2f);
				lastFrameSprites.Add(bottom);

				//bottom.Data = "LKey: " + lastKey;
				//bottom.Alignment = TextAlignment.LEFT;
				//bottom.Position = surfOffset.Position + new Vector2(_surfaceForMenu.SurfaceSize.X / 20f, _surfaceForMenu.SurfaceSize.Y - 55);
				//frame.Add(bottom);

				foreach (var clickableArea in areas)
				{
					if (drawCursor && clickableArea.CheckHover(cursorPosition))
					{
						if (click)
						{
							clickableArea.HandleClick();
						}
					}

					lastFrameSprites.AddRange(clickableArea.BuildSprites());
				}

				if (drawCursor)
				{
					MySprite cursor = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(7f, 10f), color: new Color(1f));
					cursor.RotationOrScale = 6f;
					cursor.Position = cursorPosition;
					lastFrameSprites.Add(cursor);
				}

				using (var frame = _surfaceForMenu.DrawFrame())
				{
					frame.AddRange(lastFrameSprites);
				}

				_surfaceForMenu.ContentType = ContentType.TEXT_AND_IMAGE;
				_surfaceForMenu.ContentType = ContentType.SCRIPT;
			}

			public List<MySprite> GetFrameSprites()
			{
				return lastFrameSprites;
			}
		}

		static bool ProjectOnScreen(Vector3D worldPos, IMyTextPanel surface, Vector3D povRef, out Vector2 res, bool ignoreBounds = false)
		{
			var pos = worldPos;
			var povR = povRef;
			var sWm = surface.WorldMatrix;

			bool isLargeGrid = surface.CubeGrid.GridSizeEnum == MyCubeSize.Large;
			//var pxScale = Vector2.One * surface.TextureSize.Y / ((isLargeGrid ? 2.5f : 0.5f) * 0.87f); // Transparent
			var pxScale = Vector2.One * surface.TextureSize.Y / (isLargeGrid ? 2.5f : 0.5f); // wide
			//var screenCenter = sWm.Translation + sWm.Forward * (isLargeGrid ? 2.45f : 0.49f) / 2f; // Transparent
			//E.DebugLog($"GPS:sWm.Translation:{sWm.Translation.X}:{sWm.Translation.Y}:{sWm.Translation.Z}:");
			var screenCenter = sWm.Translation + sWm.Forward * (isLargeGrid ? 2.45f : 0.49f) / 2f; // wide
			var screenPlane = new PlaneD(screenCenter, sWm.Forward);

			var ray1 = new RayD(pos, Vector3D.Normalize(povR - pos));
			var inters = ray1.Intersects(screenPlane);

			if (inters.HasValue && (Vector3D.Dot(ray1.Direction, -screenPlane.Normal) > 0))
			{
				float centerX = surface.TextureSize.X / 2f;
				float centerY = surface.TextureSize.Y / 2f;

				var hudIntersPos = ray1.Position + Vector3D.Normalize(ray1.Direction) * inters.Value;
				if (ignoreBounds || (hudIntersPos - screenCenter).Length() < (isLargeGrid ? 1.25f : 0.25f) * 1.41)
				{
					var localPhud = Vector3D.TransformNormal(hudIntersPos - sWm.Translation, MatrixD.Transpose(sWm));
					Vector2 c = new Vector2((float)localPhud.X, (float)localPhud.Y);
					c *= pxScale;

					res = new Vector2(centerX + c.X, centerY - c.Y);
					return true;
				}
			}
			res = Vector2.Zero;
			return false;
		}

		public class ClickableArea
		{
			Vector2 min, max, center;
			MySprite sprite;
			Action clickHandler;
			Action hoverHandler;

			AreaCaption areaCaption;
			class AreaCaption
			{
				public string Text;
				public Vector2 Position;
				public Color Color;
				public float Scale;
			}

			public ClickableArea(Vector2 pos, Vector2 diag, MySprite areaSprite, Action clickHandler = null, Action hoverHandler = null)
			{
				center = pos;
				min = pos - diag / 2;
				max = pos + diag / 2;
				areaSprite.Position = pos;
				areaSprite.Size = diag;
				sprite = areaSprite;
				this.clickHandler = clickHandler;
				this.hoverHandler = hoverHandler;
			}

			public void AddCaption(Vector2 localPosition, string text, float scale)
			{
				areaCaption = new AreaCaption()
				{
					Color = Config.MainColor,
					Text = text,
					Position = new Vector2(min.X + localPosition.X, center.Y - localPosition.Y),
					Scale = scale
				};
			}

			public void HandleClick()
			{
				clickHandler?.Invoke();
			}

			public bool CheckHover(Vector2 cursorPosition)
			{
				bool res = (cursorPosition.X > min.X) && (cursorPosition.X < max.X)
							&& (cursorPosition.Y > min.Y) && (cursorPosition.Y < max.Y);

				if (res)
				{
					//var nC = MySprite.CreateText(node.DisplayName ?? node.ID, "Debug", Config.mainColor, 0.6f, TextAlignment.LEFT)
					if (areaCaption != null)
					{
						areaCaption.Color = Color.Black;
					}
					sprite.Color = Config.MainColor;
					hoverHandler?.Invoke();
				}

				return res;
			}
			public IEnumerable<MySprite> BuildSprites()
			{
				yield return sprite;
				if (areaCaption != null)
				{
					var tS = MySprite.CreateText(areaCaption.Text, "Debug", areaCaption.Color, areaCaption.Scale, TextAlignment.LEFT);
					tS.Position = areaCaption.Position;
					yield return tS;
				}
			}
		}

		public class PeerMeta
		{
			public string Name;
			public long GridId;
			public Vector3D Pos;
		}

		IGCNetwork igcN;
		public class IGCNetwork
		{
			public List<long> Peers = new List<long>();
			public Dictionary<long, PeerMeta> PeerMeta = new Dictionary<long, PeerMeta>();
			public int currentContext { get; private set; }
			Action<string, string> localHandler;
			List<IMyProgrammableBlock> localPbs;
			bool initialized;
			IMyIntergridCommunicationSystem igc;
			MenuController controller;
			public string Name;
			long _gridId;

			public IGCNetwork(IMyIntergridCommunicationSystem igc, List<IMyProgrammableBlock> localPbs, MenuController controller, string name, long gridId)
			{
				this.igc = igc;
				this.localPbs = localPbs;
				this.controller = controller;
				Name = name;
				_gridId = gridId;
			}

			public void SetLocalHandler(Action<string, string> localHandler)
			{
				this.localHandler = localHandler;
			}

			IMyBroadcastListener mcChannelListener;
			public void HandleTick(List<MyIGCMessage> unicasts)
			{
				if (!initialized)
				{
					PingPeers();
					mcChannelListener = igc.RegisterBroadcastListener("menucommand.channel");
					mcChannelListener.SetMessageCallback();
					igc.UnicastListener.SetMessageCallback();
					initialized = true;
				}

				foreach (var msg in unicasts)
				{
					if (!msg.Tag.Contains("menucommand.get-position") && !msg.Tag.Contains("shell"))
						E.DebugLog($"Received unicast '{msg.Tag}'");

					if (msg.Tag == "apck-handshake")
					{
						igc.SendUnicastMessage(msg.Source, "apck-handshake-reply", "TMC");
					}
					if (msg.Tag == "menucommand.set-context")
					{
						SetContext((long)msg.Data);
					}
					if (msg.Tag == "menucommand.handshake.reply")
					{
						var addr = msg.Source;
						if (!Peers.Contains(addr))
						{
							var d = (MyTuple<long, string, Vector3D>)msg.Data;
							Peers.Add(addr);
							if (!PeerMeta.ContainsKey(addr))
								PeerMeta[addr] = new PeerMeta() { GridId = d.Item1, Name = d.Item2, Pos = d.Item3 };
						}
					}
					if (msg.Tag == "menucommand.get-position")
					{
						igc.SendUnicastMessage(msg.Source, "menucommand.get-position.reply", localPbs.First().GetPosition());
					}
					if (msg.Tag == "menucommand.get-position.reply")
					{
						if (PeerMeta.ContainsKey(msg.Source))
						{
							PeerMeta[msg.Source].Pos = (Vector3D)msg.Data;
						}
					}

					if (msg.Tag.StartsWith("menucommand.exec."))
					{
						var pbNamePart = msg.Tag.ToString().Split('.').Last();

						var pb = localPbs.FirstOrDefault(b => b.CustomName.Contains(pbNamePart));
						if (pb != null)
						{
							E.Echo(pbNamePart + " run result: " + pb.TryRun(msg.Data.ToString()) + "\nDetailedInfo:\n" + pb.DetailedInfo);
						}
					}
					if (msg.Tag == "menucommand.enter")
					{
						var nodeId = msg.Data.ToString();
						controller.Enter(nodeId);

						igc.SendUnicastMessage(msg.Source,
								"menucommand.enter.reply",
								controller.GetItemsForView().Select(n => new MyTuple<string, string>(n.Payload?.DisplayName, n.ID)).ToImmutableArray());
					}
					if (msg.Tag == "menucommand.escape")
					{
						controller.Escape();

						igc.SendUnicastMessage(msg.Source,
								"menucommand.enter.reply",
								controller.GetItemsForView().Select(n => new MyTuple<string, string>(n.Payload?.DisplayName, n.ID)).ToImmutableArray());
					}
					if ((msg.Tag == "menucommand.enter.reply") || (msg.Tag == "menucommand.escape.reply"))
					{
						var options = (ImmutableArray<MyTuple<string, string>>)msg.Data;
						controller.RemoteModel = new TreeNode<MenuItem>("", new MenuItem(""));// { options.Select(s => new TreeNode(s)) };
						foreach (var o in options)
						{
							controller.RemoteModel.Add(new TreeNode<MenuItem>(o.Item2, new MenuItem(o.Item1)));
						}
						controller.RemoteSelectedId = controller.RemoteModel.FirstOrDefault()?.ID;
					}
					// usage: CustomData -> command:get-toggles:{igc.me}
					if (msg.Tag.StartsWith("menucommand.get-commands.reply"))
					{
						var options = (ImmutableArray<MyTuple<string, string>>)msg.Data;
						var callbackId = msg.Tag.Split(new string[] { ".get-commands.reply:" }, StringSplitOptions.None)[1];

						var cn = controller.LocalModel.CurrentNode;
						Func<TreeNode<MenuItem>, TreeNode<MenuItem>> getTop = null;
						getTop = (tn) => tn.Parent.ID == "root" ? tn : getTop(tn.Parent);
						var top = cn.ID == "root" ? cn : getTop(cn);

						cn = controller.LocalModel.Root;

						TreeNode<MenuItem> res = null;
						var node = cn.FindDescendant(callbackId, ref res);
						controller.LocalModel.CurrentNode = node;

						foreach (var o in options)
						{
							controller.LocalModel.CurrentNode.Add(new TreeNode<MenuItem>(o.Item2, new MenuItem(o.Item1)));
						}

						controller.LocalModel.SelectedID = controller.LocalModel.CurrentNode.First().ID;
					}

				}

				while (mcChannelListener.HasPendingMessage)
				{
					var msg = mcChannelListener.AcceptMessage();
					E.DebugLog($"Received broadcast '{msg.Tag}'");
					if (msg.Data is long)
					{
						var addr = (long)msg.Data;
						if (!Peers.Contains(addr))
							Peers.Add(addr);
						igc.SendUnicastMessage(addr, "menucommand.handshake.reply", new MyTuple<long, string, Vector3D>(_gridId, Name, Vector3D.Zero));
					}
				}
			}

			public void HandleCommand(string pbName, string cmd)
			{
				if (currentContext == 0)
				{
					localHandler(pbName, cmd);
				}
				else
				{
					var target = Peers[currentContext - 1];

					if ((target != 0) && igc.IsEndpointReachable(target))
					{
						igc.SendUnicastMessage(target, "menucommand.exec." + pbName, cmd);
					}
				}
			}

			public long GetSelectedPeerAddr()
			{
				if (currentContext != 0)
				{
					var target = Peers[currentContext - 1];
					return target;
				}
				return 0;
			}

			void SendUnicastCommandToPeer(string commandName, string data)
			{
				var target = GetSelectedPeerAddr();
				if ((target != 0) && igc.IsEndpointReachable(target))
				{
					igc.SendUnicastMessage(target, commandName, data);
				}
			}

			public void SendEnterCommand(string nodeId)
			{
				SendUnicastCommandToPeer("menucommand.enter", nodeId);
			}

			public void SendEscapeCommand()
			{
				SendUnicastCommandToPeer("menucommand.escape", "");
			}

			public void SetContext(long id)
			{
				if (PeerMeta.Any(p => p.Value.GridId == id))
				{
					var newPeerMeta = PeerMeta.First(p => p.Value.GridId == id);
					E.DebugLog($"Forced context to {newPeerMeta.Key} {newPeerMeta.Value.Name}");
					currentContext = Peers.IndexOf(newPeerMeta.Key) + 1;
					controller.IsLocal = false;
					SendEnterCommand("");
				}
			}

			public void NextContext()
			{
				if (!Peers.Any())
				{
					PingPeers();
					return;
				}
				currentContext++;
				if (currentContext > Peers.Count)
					currentContext = 0;
				if (currentContext != 0)
				{
					controller.IsLocal = false;
					SendEnterCommand("");
				}
				else
				{
					controller.IsLocal = true;
					controller.Enter("");
				}
			}
			public void PrevContext()
			{
				if (!Peers.Any())
				{
					PingPeers();
					return;
				}
				currentContext--;
				if (currentContext < 0)
					currentContext = Peers.Count;
				if (currentContext != 0)
				{
					controller.IsLocal = false;
					SendEnterCommand("");
				}
				else
				{
					controller.IsLocal = true;
					controller.Enter("");
				}
			}
			public void PingPeers()
			{
				igc.SendBroadcastMessage("menucommand.channel", igc.Me, TransmissionDistance.AntennaRelay);
			}

			public void PollPeerPositions()
			{
				foreach (var p in Peers)
					igc.SendUnicastMessage(p, "menucommand.get-position", "");
			}
		}

		MenuController menuController;
		public class MenuController
		{
			public TreeMenuCommander LocalModel;
			public bool IsLocal;
			public IGCNetwork igcN;

			public TreeNode<MenuItem> RemoteModel;
			public string RemoteSelectedId = "";

			public void SelectID(string id)
			{
				if (IsLocal)
					LocalModel.SelectedID = id;
				else
					RemoteSelectedId = id;
			}

			public void Enter(int index)
			{
				var m = IsLocal ? LocalModel.CurrentNode : RemoteModel;
				if (m.Count > index)
				{
					Enter(m.Skip(index).First().ID);
				}
			}

			public void Enter(string id)
			{
				if (IsLocal)
				{
					if (!string.IsNullOrEmpty(id))
					{
						LocalModel.SelectedID = id;
						LocalModel.Enter();
					}
					else
					{
						LocalModel.CurrentNode = LocalModel.Root;
					}
				}
				else
				{
					// igcN send "controller.enter" currentNodeId
					igcN.SendEnterCommand(id);
				}
			}

			public void Enter()
			{
				if (IsLocal)
					LocalModel.Enter();
				else
					Enter(RemoteSelectedId);
			}

			public void Up()
			{
				if (IsLocal)
				{
					LocalModel.Up();
				}
				else
				{
					var newSelectedNode = RemoteModel.Reverse().SkipWhile(n => n.ID != RemoteSelectedId).Skip(1).FirstOrDefault() ?? RemoteModel.LastOrDefault();
					if (newSelectedNode != null)
					{
						RemoteSelectedId = newSelectedNode.ID;
					}
				}
			}

			public void Down()
			{
				if (IsLocal)
				{
					LocalModel.Down();
				}
				else
				{
					var newSelectedNode = RemoteModel.SkipWhile(n => n.ID != RemoteSelectedId).Skip(1).FirstOrDefault() ?? RemoteModel.FirstOrDefault();
					if (newSelectedNode != null)
					{
						RemoteSelectedId = newSelectedNode.ID;
					}
				}
			}

			public void Escape()
			{
				if (IsLocal)
					LocalModel.Esc();
				else
				{
					igcN.SendEscapeCommand();
				}
			}

			public TreeNode<MenuItem> GetItemsForView()
			{
				//return new TreeNode("", "") { new TreeNode("sadads", "nam1"), new TreeNode("sadads", "name2") };
				if (IsLocal)
					return LocalModel.CurrentNode;
				else
				{
					if (RemoteModel != null)
						return RemoteModel;
					else
						return LocalModel.CurrentNode;
				}
			}
		}

		public class TreeMenuCommander
		{
			public TreeNode<MenuItem> Root { get; private set; }
			public TreeNode<MenuItem> CurrentNode { get; set; }
			public string SelectedID;

			private Action<string, string> handler;

			public TreeMenuCommander(Action<string, string> handler)
			{
				this.handler = handler;
				this.Root = new TreeNode<MenuItem>("root");
				this.CurrentNode = Root;
			}

			public TreeMenuCommander(string fromCData, Action<string, string> generalHandler, Action<string, string> localHadler, string igcMe)
			{
				this.handler = generalHandler;
				this.Root = new TreeNode<MenuItem>("root");
				this.CurrentNode = Root;
				var lines = fromCData.Split('\n');
				var recentParents = new Dictionary<int, TreeNode<MenuItem>>();

				foreach (var line in lines)
				{
					int indLevel = line.Length - line.TrimStart().Length;
					string aliasSubstring, cmdString;
					TreeNode<MenuItem> node;
					var nodeText = line.TrimStart();

					if (indLevel == 0)
					{
						if (nodeText.Contains(Config.AliasKey))
						{
							ParseAlias(nodeText, out cmdString, out aliasSubstring);
							node = new TreeNode<MenuItem>(cmdString, new MenuItem(aliasSubstring));
						}
						else
						{
							node = new TreeNode<MenuItem>(nodeText);
						}
						Root.Add(node);
						recentParents[0] = node;
					}
					else
					{
						Action<string, string> handler = null;

						if (nodeText.Contains(Config.IgcIdPlaceholder))
						{
							nodeText = nodeText.Replace(Config.IgcIdPlaceholder, igcMe);
							handler = localHadler;
						}

						if (nodeText.Contains(Config.AliasKey))
						{
							ParseAlias(nodeText, out cmdString, out aliasSubstring);
							node = new TreeNode<MenuItem>(cmdString, new MenuItem(aliasSubstring, localHadler));
						}
						else
						{
							node = new TreeNode<MenuItem>(nodeText, localHadler == null ? null : new MenuItem(nodeText, localHadler));
						}
						recentParents[indLevel - 1].Add(node);
						recentParents[indLevel] = node;
					}
				}
			}

			void ParseAlias(string nodeText, out string cmd, out string alias)
			{
				var aliasSubstring = nodeText.Split(Config.AliasDelimiter)[0];
				cmd = nodeText.Substring(aliasSubstring.Length + 1);
				alias = aliasSubstring.Substring(aliasSubstring.IndexOf('=') + 1);
			}

			public TreeMenuCommander Options()
			{
				if (SelectedID == null)
				{
					SelectedID = Enumerable.First(CurrentNode).ID;
				}
				return this;
			}

			public TreeMenuCommander Enter()
			{
				// item we've just clicked on
				var selectedChild = this.CurrentNode.GetChild(this.SelectedID);

				Func<TreeNode<MenuItem>, TreeNode<MenuItem>> getTop = null;
				getTop = (tn) => tn.Parent.ID == "root" ? tn : getTop(tn.Parent);
				var top = CurrentNode.ID == "root" ? CurrentNode : getTop(CurrentNode);


				if (selectedChild.Count == 0)
				{
					handler(top.ID, SelectedID);
					return this;
				}
				//else
					//selectedChild.Payload?.OnClick(top.ID, SelectedID); // why?

				this.CurrentNode = selectedChild;
				this.SelectedID = Enumerable.First(selectedChild).ID;
				this.Options();
				return this;
			}
			public TreeMenuCommander Esc()
			{
				if (CurrentNode.Parent == null)
					return this;
				CurrentNode = CurrentNode.Parent;
				SelectedID = Enumerable.First(CurrentNode).ID;
				Options();
				return this;
			}
			public TreeMenuCommander Down()
			{
				var newSelectedNode = CurrentNode.SkipWhile(n => n.ID != SelectedID).Skip(1).FirstOrDefault() ?? CurrentNode.FirstOrDefault();
				if (newSelectedNode != null)
				{
					SelectedID = newSelectedNode.ID;
				}
				Options();
				return this;
			}
			public TreeMenuCommander Up()
			{
				var newSelectedNode = CurrentNode.Reverse().SkipWhile(n => n.ID != SelectedID).Skip(1).FirstOrDefault() ?? CurrentNode.LastOrDefault();
				if (newSelectedNode != null)
				{
					SelectedID = newSelectedNode.ID;
				}
				Options();
				return this;
			}
		}

		public class MenuItem
		{
			public string DisplayName { get; set; }

			private Action<string, string> handler;

			public MenuItem(string displayName = null, Action<string, string> onClick = null)
			{
				DisplayName = displayName;
				handler = onClick;
			}

			public void OnClick(string nodeId, string cmd)
			{
				handler?.Invoke(nodeId, cmd);
			}
		}

		public class TreeNode<T> : IEnumerable<TreeNode<T>> where T : class
		{
			private readonly Dictionary<string, TreeNode<T>> _children =
												new Dictionary<string, TreeNode<T>>();

			public readonly string ID;
			public readonly T Payload;

			public TreeNode<T> Parent { get; private set; }

			public TreeNode(string id, T payload = null)
			{
				this.ID = id;
				this.Payload = payload;
			}

			public TreeNode<T> GetChild(string id)
			{
				return this._children[id];
			}

			public IEnumerable<TreeNode<T>> GetDescendants(string id)
			{
				foreach (var c in _children)
				{
					if (c.Key == id)
						yield return c.Value;
					else
						foreach (var x in c.Value.GetDescendants(id))
							yield return x;
				}
			}

			public TreeNode<T> FindDescendant(string id, ref TreeNode<T> result)
			{
				if (id == this.ID)
					return this;
				else
					foreach (var c in _children)
					{
						result = c.Value.FindDescendant(id, ref result);
					}

				return result;
			}

			public void Add(TreeNode<T> item)
			{
				if (item.Parent != null)
				{
					item.Parent._children.Remove(item.ID);
				}

				if (_children.ContainsKey(item.ID))
					_children.Remove(item.ID);

				item.Parent = this;
				this._children.Add(item.ID, item);
			}

			public void Remove()
			{
				if (Parent != null)
				{
					Parent._children.Remove(this.ID);
				}
			}

			public IEnumerator<TreeNode<T>> GetEnumerator()
			{
				return this._children.Values.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public int Count
			{
				get { return this._children.Count; }
			}
		}

		IMyBlockGroup GetThisBlockGroup(IMyTerminalBlock block)
		{
			List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
			GridTerminalSystem.GetBlockGroups(groups);
			return groups.Where(g =>
			{
				var bs = new List<IMyTerminalBlock>();
				g.GetBlocksOfType(bs);
				return bs.Contains(block);
			}).FirstOrDefault();
		}
		List<T> GetGroupBlocks<T>(IMyBlockGroup g, Func<IMyTerminalBlock, bool> p = null) where T : class, IMyTerminalBlock
		{
			List<T> bs = new List<T>();
			g.GetBlocksOfType(bs, p);
			return bs;
		}

		InputListener UserCtrlListener;
		public class InputListener
		{
			public Vector2 Cumulative = new Vector2();
			List<IMyShipController> ctrls;
			Func<int> tickGetter;
			public IMyShipController GetControlledCockpit()
			{
				return _activeController;
			}

			IMyShipController _activeController;
			Vector3 _moveInd;
			Vector2 _rotInd;
			float _rollInd;

			IMyShipToolBase _toolActivator;

			public bool ReceivedUserActionsFromIGC { get; private set; }
			Vector2 _proxyCursorOverride;
			public void PrepareBeforeTick(List<MyIGCMessage> uniMsgs)
			{
				_activeController = ctrls.Where(c => c.IsUnderControl).FirstOrDefault();

				if (_activeController != null)
				{
					_moveInd = _activeController.MoveIndicator;
					_moveInd.X = -_moveInd.X;
					_moveInd.Z = -_moveInd.Z;
				}
				else
					_moveInd = Vector3.Zero;

				_rollInd = _activeController?.RollIndicator ?? 0f;
				_rotInd = _activeController?.RotationIndicator ?? Vector2.Zero;

				ReceivedUserActionsFromIGC = false;
				foreach (var msg in uniMsgs)
				{
					if (msg.Tag == "shell.hid")
					{
						
						ReceivedUserActionsFromIGC = true;
						var d = (MyTuple<Vector2, bool, bool>)msg.Data;
						//E.DebugLog($"uiProxyEvent: {d.Item2} : {d.Item3}");
						if (d.Item3)
							_rollInd = 1;
						if (d.Item2)
							_rollInd = -1;
						_proxyCursorOverride = d.Item1;
					}
				}
			}

			public bool GetCursorOverride(ref Vector2 c)
			{
				if (ReceivedUserActionsFromIGC)
				{
					c = _proxyCursorOverride;
					return true;
				}
				return false;
			}

			public IMyTextSurface GetCockpitSurface(int screenIndex)
			{
				IMyTextSurfaceProvider c = _activeController as IMyCockpit;
				if (c == null)
					c = ctrls.Where(x => x is IMyTextSurfaceProvider).FirstOrDefault() as IMyTextSurfaceProvider;
				if (c != null)
				{
					if (c.SurfaceCount > screenIndex)
					{
						return c.GetSurface(screenIndex);
					}
				}

				return null;
			}

			public Vector3 GetVector()
			{
				return _moveInd;
			}
			public Vector2 GetRot()
			{
				return _rotInd;
			}
			public float GetRoll()
			{
				return _rollInd;
			}

			class InputHistory
			{
				public string KeyName;
				public int LastKeyDownStamp;
				public int State;
			}

			List<InputHistory> h;
			public InputListener(List<IMyShipController> ctrlToConsider, IMyShipToolBase toolActivator, Func<int> tickGetter)
			{
				ctrls = ctrlToConsider;
				this.tickGetter = tickGetter;
				_toolActivator = toolActivator;

				h = new List<InputHistory>();
				h.Add(new InputHistory { KeyName = "spacebar" });
				h.Add(new InputHistory { KeyName = "c" });
				h.Add(new InputHistory { KeyName = "e" });
				h.Add(new InputHistory { KeyName = "q" });

				h.Add(new InputHistory { KeyName = "w" });
				h.Add(new InputHistory { KeyName = "s" });
				h.Add(new InputHistory { KeyName = "a" });
				h.Add(new InputHistory { KeyName = "d" });

				h.Add(new InputHistory { KeyName = "tool" });
			}

			public bool CheckKeyDown(string keyName)
			{
				if (_activeController != null || ReceivedUserActionsFromIGC)
				{
					bool isKeyDown = false;
					if ((keyName == "spacebar") && (_moveInd.Y > 0))
						isKeyDown = true;
					if ((keyName == "c") && (_moveInd.Y < 0))
						isKeyDown = true;
					if ((keyName == "e") && (_rollInd > 0))
						isKeyDown = true;
					if ((keyName == "q") && (_rollInd < 0))
						isKeyDown = true;
					if ((keyName == "w") && (_moveInd.Z < 0))
						isKeyDown = true;
					if ((keyName == "s") && (_moveInd.Z > 0))
						isKeyDown = true;
					if ((keyName == "a") && (_moveInd.X < 0))
						isKeyDown = true;
					if ((keyName == "d") && (_moveInd.X > 0))
						isKeyDown = true;
					if ((keyName == "tool") && (_toolActivator?.IsActivated == true))
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

		TransponderWatch TW;
		public class TransponderWatch
		{
			IMyTransponder _tpon;
			int? _lastCh;
			int? _initCh;
			public Dictionary<int, string> TpBindings = new Dictionary<int, string>();
			Action<string, string> _localHandler;
			string _rootId;

			public TransponderWatch(IMyTransponder tpon, Action<string, string> localHandler, string rootId)
			{
				_tpon = tpon;
				_localHandler = localHandler;
				_rootId = rootId;
			}

			public void HandleTp()
			{
				if (_lastCh == null)
				{
					int freeChannel = 100;
					do
					{
						if (!TpBindings.ContainsKey(freeChannel))
						{
							_tpon.Channel = freeChannel;
							break;
						}
					} while (--freeChannel > 0);
				}
				else if (_lastCh != _tpon.Channel && TpBindings.ContainsKey(_tpon.Channel))
				{
					_localHandler(_rootId, TpBindings[_tpon.Channel]);
				}
				_lastCh = _tpon.Channel;
			}
		}

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

			static string buff = "";
			public static void DebugToPanel(string s)
			{
				buff += s + "\n";
			}
			public static void DebugLog(string s)
			{
				l.WriteText($"{T}: {s}\n", true);
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

		OrbitalPainterImporter orbitalPainterImporter;
		public class OrbitalPainterImporter
		{
			List<MySprite> _back = new List<MySprite>();
			IMyIntergridCommunicationSystem _igc;
			Vector2 _vpSize;
			public OrbitalPainterImporter(IMyIntergridCommunicationSystem igc, Vector2 vpSize)
			{
				_igc = igc;
				_vpSize = vpSize;
			}

			public void HandleRequests(List<MyIGCMessage> unicasts)
			{
				if (!string.IsNullOrEmpty(Config.OP_BackgroundFileName) && _back.Count == 0)
				{
					foreach (var msg in unicasts)
					{
						if (msg.Tag.Contains("shell.sprites"))
						{
							var d = (MyTuple<string, ImmutableArray<MyTuple<int, string, Vector2, Vector2, float, Vector4>>>)msg.Data;
							_back.Clear();
							foreach (var s in d.Item2)
							{
								var data = s;
								SpriteType spriteType = data.Item1 == -1 ? SpriteType.TEXTURE : SpriteType.TEXT; // Item1 is Alignment enum, -1 means it's not a text
								var sprite = new MySprite(spriteType, data.Item2, size: data.Item3, color: new Color(1f));

								var sz = sprite.Size.Value;
								sprite.Size = new Vector2(sz.X > 1 ? sz.X : sz.X * _vpSize.X, sz.Y > 1 ? sz.Y : sz.Y * _vpSize.Y);

								sprite.Position = data.Item4;
								var posInPort = new Vector2(sprite.Position.Value.X * _vpSize.X / 2f, sprite.Position.Value.Y * _vpSize.Y / 2f);
								sprite.Position = _vpSize / 2f + posInPort;

								sprite.RotationOrScale = data.Item5;
								sprite.Color = new Color(data.Item6);
								if (data.Item1 >= 0)
									sprite.Alignment = (TextAlignment)data.Item1;
								_back.Add(sprite);
							}
						}
					}

					if (_back.Count == 0)
						_igc.SendBroadcastMessage("op:get-pic", Config.OP_BackgroundFileName);
				}
			}

			public List<MySprite> GetBackground()
			{
				return _back;
			}
		}

		OtherGridScanner ogs;
		class OtherGridScanner
		{
			IMyShipConnector _conn;
			IMyGridProgramRuntimeInfo _runtimeInfo;

			public OtherGridScanner(IMyShipConnector connector, IMyGridProgramRuntimeInfo runtimeInfo)
			{
				_conn = connector;
				_runtimeInfo = runtimeInfo;
			}

			IMyShipConnector _otherConn;
			public bool Finished = true;

			public void Handle(int tick)
			{
				// connected state causes enemy grid to bug out and not give all its' fat blocks
				//if (_conn.Status == MyShipConnectorStatus.Connected)
				//{
					if (_otherConn != _conn.OtherConnector)
					{
						_otherConn = _conn.OtherConnector;
						E.DebugLog("OtherGridScanner new connection!");
						StartScan(_otherConn.CubeGrid);
					}
				//}

				if (!Finished)
				{
					var grid = _otherConn.CubeGrid;
					foreach (var p in Vector3I.EnumerateRange(grid.Min, grid.Max))
					{
						//E.DebugLog($"check {p}");
						var fb = grid.GetCubeBlock(p)?.FatBlock;
						if (fb is IMyTerminalBlock)
						{
							var tb = (IMyTerminalBlock)fb;
							terminalBlocks.Add(tb);
							var def = tb.BlockDefinition.TypeId + "///" + tb.BlockDefinition.SubtypeId;
							if (!definitionsDistinct.Contains(def))
							{
								definitionsDistinct.Add(def);
								E.DebugLog(def);
							}
						}
					}
					Finished = true;
					terminalBlocks = terminalBlocks.Distinct().ToList();
					InstCount = _runtimeInfo.CurrentInstructionCount;
				}
			}

			void StartScan(IMyCubeGrid grid)
			{
				terminalBlocks.Clear();
				Finished = false;
				InstCount = 0;
			}

			public List<IMyTerminalBlock> terminalBlocks = new List<IMyTerminalBlock>();
			public List<string> definitionsDistinct = new List<string>();
			public int InstCount;
		}
