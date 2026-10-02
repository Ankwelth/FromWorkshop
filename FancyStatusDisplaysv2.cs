//  ****************************************************************************************************************************************
const string ver="FSD Version 37y";
//  ****************************************************************************************************************************************
//  Config
//  ****************************************************************************************************************************************

string[] Tag  = {
// The main control keywords
"ShowStats",        // [lcdTag] Tag the script reacts to - write it to the custom data of your screen/cockpit
"EndStats",            // [EndTag] Tag to end the command list (optional)
"Panel",                 // [Panel_Tag] Tag to define which panel of block is addressed

// Tags to control the block selection process
"Verbatim",            // [VerbatimTag] Tag to require exact Name match
"NoSubGrids",       // [Opt_Tag05] Tag to exclude subgrids
"OnlySubGrids",    // [Opt_Tag06] Tag to only show blocks on subgrids
"NoGroups",           // [Opt_Tag07] Tag to skip groups in the detection

// Tags for alternative display types
"WideBar",            // [WidebarTag] Tag to display a large chargebar instead of each block individually
"SmallBar",           // [SmallbarTag] Tag to display a small chargebar instead of each block individually
"TallBar",               // [TallbarTag] Tag to display a large vertical chargebar instead of each block individually
"ShortBar",            // [ShortbarTag] Tag to display a small vertical chargebar instead of each block individually
"SingleIcon",         // [SingleIconTag] Tag to display only a single icon instead of each block individually
"NoIcons",             // [NoIconTag] Tag to display only the headlines without any graphics

// Tags to modify the allignment of the elements
"IconCount=",        // [CountTag] Tag to define the number of icons to be displayed
"NoLineBreak",     // [Opt_Tag08] Tag to prevent linebreaks
"NoCR",                 // [Opt_Tag11] Tag to force all the elements in one line
"EndLine",             // [Opt_Tag12] Tag to stop adding elements to the right

// Tags to additional elements on or off
"AddSize",             // [Opt_Tag02] Tag to display size informations
"Health",                // [Opt_Tag03] Tag to display additional healthbar
"AddInfo",              // [Opt_Tag01] Tag to display additional informations
"AltInfo=",              // [AltInfoTag] Tag to replace additional informations
"AltSymbol=",       // [AltSymTag] Tag to replace the symbol 
"NoPercentage",   // [Opt_Tag14] Tag to turn off the percentage display

// Tags to modify the colors
"SymbolColor",     // [Col_sym] Tag to recolor the Symbol
"TextColor",           // [Col_txt] Tag to recolor the texts
"PercentColor",     // [Col_per] Tag to recolor the percent number
"FrameColor",        // [Col_frm] Tag to recolor the frame
"BarColor",            // [Col_bar] Tag to recolor the fill level bar
"InfoColor",            // [Col_opt] Tag to recolor the optional Info Text
"IconColor",           // [Col_ico] Legacy to recolor the Symbol

// Tags to repositioning elements
"Gap=",                  // [GapTag] Tag to display a gap between two blocks
"Position(",            // [PositionTag] Tag to position the next blocks

// Tags to modify element sizes
"Fontsize=",          // [FSizeTag] Tag to define the Fontsize of the headline
"Scale=",               // [ScaleTag] Tag to define the Scalefactor of the Icons
"Length=",             // [LengthTag] Tag to define an alternative length of a chargebar

// Tags to change the look of the bargraphs
"DigitalBar",          // [Bar_Type01] Tag to switch off the last partial bar
"AnalogBar",         // [Bar_Type02] Tag to turn the bargraphs into a single analog bar
"SolidBar",            // [Bar_Type03] Tag to turn the bargraphs into a single solid bar
"Invert",                 // [Opt_Tag15] Tag to invert the colors of the bargraphs

// Tags to adjust the values are calculated
"Optional",             // [Opt_Tag00] Tag to display either power or inventory if a block has both (in case of battery it's load percentage or input/output)
"Filter(",                 // [FilterTag] Tag for filtering which items should be counted
"Reference=",        // [ReferenceTag] Tag to define an alternative reference value
"AltCalc",               // [Opt_Tag13] Tag to switch alterntive pecentage calculation

// Tags to insert graphics
"Sprite:",                // [SpriteTag] Tag to display a custom texture sprite
"GFXStart(",           // [gfxStart] Tag to indicate the start of a graphic sprite
"GFXEnd",              // [gfxEnd] Tag to indicate the end of a graphic sprite

// Tags to insert texts
"Clone:",                // [CloneTag] Tag to clone the contend of a display
"Text:",                  // [textTag] Tag to display a custom text

// Tags to adjust how texts are displayed
"Left",                     // [AlignTag00] Tag to display text left aligned
"Center",                // [AlignTag01] Tag to display text center aligned
"Right" ,                 // [AlignTag02] Tag to display text right aligned
"NoHeadline",        // [Opt_Tag04] Tag to display the blocks without headline
"NoScrolling",        // [Opt_Tag09] Tag to prevent scrolling
"NoNames",           // [Opt_Tag10] Tag to display only the headlines without the names
"ScrollSpeed=",    // [scrlspdTag] Tag to define the scrollspeed of the headline

// Tags to handle conditional elements
"Getvalue",            // [ValueTag] Tag to define which value should used for comparision
"Condition(",          // [ConditionTag] Tag to define the range to be met as condition

// Tags to set general options
"FSD options:",     // [Def_Tag] Tag to switch the default options in the CustomData of the programming block
"NoSubgridLCDs", // [NoSubgridLCDTag] Tag to exclude subgrid LCDs
"NoStatus",           // [NoStatusTag] Tag to not to display FSD status on the PB
"Framerate=",        // [FpM_Tag] Tag to set the frame rate in the CustomData of the programming block
"Layoutrate=",       // [LpM_Tag] Tag to set the layout change rate in the CustomData of the programming block
"Layout",                // [Layout_Tag] Prefix tag for Layout commands
"Sequence(",         // [Sequence_Tag] Tag define the screen layout sequence in the CustomData of the programming block
"Fastmode"           // [Fast_Tag] Tag to activate the fastmode in the CustomData of the programming block
};

// The Separators. Used to indicate where the option keyword part begins
char[] seperators = {'\n' , ',' , ':'};

int FpM = 60; //How often the displays will be updated (In frames per minute)
int LpM = 12;  //How often the display layouts switch  (In changes per minute)
int SsD = 10;  //Default Scroll Speed

// The Default COLORS. Any RGB-Color could be used from Black new Color(0, 0, 0) to White new Color(255, 255, 255)
Color frameColorIsNotThere = new Color(30, 30, 30); //color if block is not there
Color frameColorIsUnfunctional = Color.DarkRed;       //color if block is not funktional
Color frameColorIsFunctional = new Color(0, 70, 70);        //color if block is funktional but not working
Color frameColorIsWorking = Color.Cyan;          //color if block is working
Color frameColorWideBar = Color.Gray;      //color wide bar
Color headlineColor = Color.White;      //headline color
Color symbolColor = Color.White;      //symbols on single blocks
Color optioncolor = Color.Silver;      //optional information on single blocks
Color percentDisplayColor = Color.White;      //fillstate percentvalue on single blocks

//  ****************************************************************************************************************************************
//  Config  End - Do not change anything below
//  ****************************************************************************************************************************************

Program() {
  GeneratePatterns();
  tag_sort = new int[] {Opt_Tag00,Opt_Tag01,Opt_Tag02,Opt_Tag03,Opt_Tag04,Opt_Tag05,Opt_Tag06,Opt_Tag07,Opt_Tag08,Opt_Tag09,Opt_Tag10,Opt_Tag11,Opt_Tag12,Opt_Tag13,Opt_Tag14,Opt_Tag15};
  for (int i=VerbatimTag;i<Def_Tag;i++) Tag[i]=TL(Tag[i]);
  for (int i=NoSubgridLCDTag;i<Tag.Length;i++) Tag[i]=TL(Tag[i]);
  if (!Me.CustomName.EndsWith("[FSD]")) Me.CustomName += " [FSD]";
  if (Me.CustomData.Contains(Tag[Def_Tag])) setdefault();
  if (!nostatus) {
    var stat=Me.GetSurface(0); 
    stat.Font="Debug";
    stat.TextPadding=0.0f;
    stat.ContentType=ContentType.TEXT_AND_IMAGE;
    stat.Alignment=TextAlignment.CENTER;
    stat.FontColor=Color.White;
    stat.FontSize=1.3f;
  }
  cs = scrollpositions.GetUpperBound(0) + 1;
  ce = scrollpositions.GetUpperBound(1) + 1;
  Runtime.UpdateFrequency = speed;
  pnl_tag = "\n"+Tag[Panel_Tag]+" ";
  warnings = "";
}
void Save() {}

void Main(string preargument, UpdateType updateType) {
  int surfaceIndex;
  if (Storage == "offline" && preargument == "") argument = "shutdown"; else argument = TL(preargument);
  if (argument==(Tag[Layout_Tag]+"+")) layoutswitch(+1);
  else if (argument==(Tag[Layout_Tag]+"-")) layoutswitch(-1);
  else {
    if ((LayoutCount>1)&&(LpM>0))  {
      execCounter2++;
      if (execCounter2 >= TpM/LpM) layoutswitch(+1);
    }
  }
  if (Me.CustomData.Contains(Tag[Def_Tag])&&(oldCustomData!=Me.CustomData)) setdefault();
  if (nosubgridLCDs) {
    ignoredGrids.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(surfaceProviders, (p => p.CustomData.Contains(Tag[lcdTag]) && p.CubeGrid == Me.CubeGrid));
  } else {
    DetectIgnoredGrids();
    GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(surfaceProviders, p => p.CustomData.Contains(Tag[lcdTag]));
  }
  if (startdisp==0) {
    if  (FpM < 1) FpM = 1;
    execCounter1++;
    if (execCounter1 >= TpM/FpM) execCounter1 = 0;
    if ((argument=="") && (execCounter1!=0)) return;
    screenIndex = 0;
  }
  dispnr = 0;
  timeout = false;
  permanent = 0;
  foreach (var block in surfaceProviders) {
    bool skipBlock = false;
    foreach (var grid in ignoredGrids) {if (block.CubeGrid == grid) skipBlock = true;}
    if (dispnr<startdisp) {dispnr++;continue;}
    if ((Runtime.CurrentInstructionCount+highload)>Runtime.MaxInstructionCount) {
      timeout = true;
      startdisp = dispnr;
      Runtime.UpdateFrequency = UpdateFrequency.Update1;
    }
    if (skipBlock || timeout) break;
    dispnr++;
    var provider = block as IMyTextSurfaceProvider;
    Displayname = block.CustomName;
    if (provider.SurfaceCount==0) {
      warnings+="The CustomData field of the block:\n\""+Displayname+"\"\ncontains the trigger word:\n\""+Tag[lcdTag]+"\"\nbut doesn't provide any textpanels\n\n";
      continue;
    }
    StartTag=Tag[lcdTag]+" "+Layout;
    if (!("\n"+block.CustomData).Contains("\n"+StartTag)) StartTag = Tag[lcdTag];
    temp_txt = teil(block.CustomData,block.CustomData.IndexOf(StartTag)).Trim();
    if (temp_txt.IndexOfAny(seperators)<0) continue;
    while (temp_txt.StartsWith(Tag[lcdTag])) temp_txt = teil(temp_txt,temp_txt.IndexOfAny(seperators)).Trim();
    if (temp_txt.Contains(Tag[EndTag])) temp_txt = temp_txt.Remove(temp_txt.IndexOf(Tag[EndTag]));
    temp_txt = temp_txt.Insert(0,"\n").Replace(TL(pnl_tag),pnl_tag).Replace(Tag[Panel_Tag].ToUpper(),pnl_tag);
    sections = temp_txt.Split(new[] {pnl_tag}, StringSplitOptions.None);
    for (int i = 0; i < sections.Length; i++) {
      lines = sections[i].Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
      if (lines.Length == 0) continue;
      surfaceIndex = 0;
      if ((sections.Length > 1) && (int.TryParse(lines.First(), out surfaceIndex))) sArgs = lines.Skip(1).ToArray(); else sArgs = lines;
      if (surfaceIndex >= provider.SurfaceCount) surfaceIndex = 0;
      PrepareSurface(block, surfaceIndex);
      DrawScreen();
      frame.Dispose();
      screenIndex++;
    }
  }
  string stat_txt = "";
  rotatecount %= 4;
  switch (argument) {
    case "version":
      Echo("Version: "+ver);
    break;
    case "shutdown":
      Storage = "offline";
      Runtime.UpdateFrequency = UpdateFrequency.None;
      stat_txt = "             << Notice >>\nThis F.S.D. entity is shut down.\nRun this programming block\nwith the argument \"powerup\"\nto restart it again.";
    break;
    case "powerup":
      Storage = "online";
      Runtime.UpdateFrequency = speed;
      stat_txt = "FSD is booting";
      Echo(" ");
    break;
    case "refresh":
      Runtime.UpdateFrequency = speed;
      stat_txt = "FSD is refreshing the displays";
    break;
    case "":
      stat_txt = "   << F.S.D. is active >>\n"+rotator[rotatecount]+"\nThis programming block is\nhandling " + plSing("LCD Panel", screenIndex) + "\nat the moment.";
    break;
    default:
      LpM = intparse(argument,Tag[LpM_Tag],0,LpM);
      FpM = intparse(argument,Tag[FpM_Tag],0,FpM);
      Layout = intparse(argument,Tag[Layout_Tag]+"=",0,Layout);
      SsD = intparse(argument,Tag[scrlspdTag],0,SsD);
    break;
  }
  if (!timeout) {
    Echo(stat_txt);
    if (startdisp > 0) warnings+="high Programmable Block load\n\n";
    if (warnings!="") {
      Echo("\n         << F.S.D. Warnings >>\n");
      Echo(warnings);
    }
    startdisp = 0;
    warnings = "";
    Runtime.UpdateFrequency = speed;
    rotatecount++;
  }
  if (!nostatus) Me.GetSurface(0).WriteText("\n\n"+stat_txt.Trim());
}

void DetectIgnoredGrids() {
  List<IMyProgrammableBlock> pbs = new List<IMyProgrammableBlock>();
  ignoredGrids.Clear();
  GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(pbs, x => x.CustomName.Contains("[FSD]") && !(x.CubeGrid == Me.CubeGrid));
  foreach (var pb in pbs) ignoredGrids.Add(pb.CubeGrid);
}

void DrawScreen() {
  Colors[ICO] = Color.Black;
  pos = LineStartpos = viewport.Position + new Vector2(2.88f, 2.88f);
  string[] groupAndOptions;
  string l_arg, U_arg="";
  float gfx_res = 0.1f;
  bool continueScreen = true;
  lastDraw = nothing;
  lastLength = 0;
  int optionpos,texttagpos,spritetagpos;
  headlineIndex = 0;
  if (Storage == "offline" || argument == "shutdown" || argument == "refresh") {
    AddTextSprite(pos, " ", "Debug", 1, Color.Black, L_align);
    frame.Dispose();
    return;
  }
  get_sprite = false;
  gfxSprite.Clear();
  foreach (var arg in sArgs) {
    if (get_sprite) {
      if (TL(arg).TrimStart(new[] { ':', ',' }) == TL(Tag[gfxEnd])) {
        mod_col(ref Colors[TXT]);
        AddTextSprite(gfxpos, gfxSprite.ToString(), "Monospace", gfx_res, Colors[TXT], L_align);
        LineStartpos.Y = gfxpos.Y + surface.MeasureStringInPixels(gfxSprite, "Monospace", gfx_res).Y;
        last_fsize = 0f;
        get_sprite = false;
        gfxSprite.Clear();
      } else {
        gfxSprite.AppendLine(arg);
      }
      continue;
    }
    LineStartpos.X = viewport.X + 2.88f;
    optionpos = arg.IndexOfAny(new char[] { ',', ':' });
    containertype = multiicon;
    added_txt = sym3 = opt_txt2 = filter = "";
    clone_txt.Clear();
    scale = min_length = 1f;
    bar_type = normal;
    iconcount = spritetagpos = clone = -1;
    set_length = reference = percent = fontsize = 0f;
    verti = horiz = perm_flag = false;
    for(int i=0;i<paints.Length;i++) paints[i] = false;
    Colors[SYM] = symbolColor;
    Colors[TXT] = headlineColor;
    Colors[PER] = percentDisplayColor;
    Colors[OPT] = optioncolor;
    scrl_spd = SsD;
    std_length = viewport.Width - LineStartpos.X;
    if (optionpos >= 0) {
      U_arg = teil(arg, optionpos + 1);
      l_arg = TL(U_arg);
      spritetagpos = l_arg.IndexOf(TL(Tag[SpriteTag]));
      texttagpos = l_arg.IndexOf(TL(Tag[textTag]));
      if (texttagpos >= 0) {
        added_txt = teil(U_arg, texttagpos + Tag[textTag].Length);
        U_arg = U_arg.Remove(texttagpos);
        l_arg = TL(U_arg);
      }
      if (have(l_arg,Tag[ConditionTag])) {
        var conditions = getBetween(l_arg, TL(Tag[ConditionTag]), ")").Replace(">=","↑").Replace("<=","↓").Replace("<>","↔").Replace("!=","↔").Trim().Split(',').ToArray();
        bool and = true ,or = false , result = false;
        float val = 0;
        var cArgs = conditions.Skip(1).ToArray();
        foreach (var cArg in cArgs) {
          if (Char.IsSymbol(cArg[0]) && (!float.TryParse(cArg.Substring(1),out val))) continue;
          switch (cArg[0]) {
            case '↑': result = (permanent>= val); break;
            case '↓': result = (permanent<= val); break;
            case '↔': result = (permanent!= val); break;
            case '=': result = (permanent== val); break;
            case '>': result = (permanent > val); break;
            case '<': result = (permanent < val); break;
          }
          and &= result;
          or |= result;
        }
        if ((conditions.First()=="and") && and) continue;
        else if ((conditions.First()=="or") && or) continue;
      }
      if (have(l_arg,Tag[gfxStart])) {
        if (getVector(l_arg, Tag[gfxStart], out tempvector, out gfx_res)) {
          gfxpos = tempvector + viewport.Position;
          get_sprite = true;
          gfxSprite.Clear();
        }
      }
      if (have(l_arg,Tag[PositionTag])) {
        if (getVector(l_arg, Tag[PositionTag], out tempvector, out pos.X)) {
          pos = LineStartpos = tempvector + viewport.Position;
          lastDraw = nothing;
          continueScreen = true;
        }
      }
      if (have(l_arg,Tag[FilterTag])) filter = getBetween(l_arg,TL(Tag[FilterTag]),")");
      textpos = get_align(l_arg,textposdefault);
      opt_chc = wordparse(opt_def,l_arg);

      for(int i=Bar_Type01;i<=Bar_Type03;i++) {
        if(have(l_arg,Tag[i])) {
          bar_type = i-Bar_Type00;
          break;
        }
      }
      if (have(l_arg,Tag[Col_ico])) l_arg = l_arg.Replace(TL(Tag[Col_ico]), TL(Tag[Col_sym]));
      for(int i=0;i<paints.Length;i++) {
        if (have(l_arg,Tag[Col_sym+i])) {
          paints[i] = getcolors(l_arg, Tag[Col_sym+i], out Colors[i]);
        }
      }
      if (!paints[SYM]) Colors[SYM] = symbolColor;
      if (!paints[TXT]) Colors[TXT] = headlineColor;
      if (!paints[PER]) Colors[PER] = percentDisplayColor;
      if (!paints[OPT]) Colors[OPT] = optioncolor;
      if (have(l_arg,Tag[AltSymTag])) sym3 = getval(U_arg, Tag[AltSymTag],true);
      if (have(l_arg,Tag[AltInfoTag])) opt_txt2 = getval(U_arg, Tag[AltInfoTag],true);
      scrl_spd = intparse(l_arg, Tag[scrlspdTag], 0, SsD);
      clone = intparse(l_arg,Tag[CloneTag], 0, -1);
      iconcount =  intparse(l_arg, Tag[CountTag], 0, -1);
      fontsize = floatparse(l_arg, Tag[FSizeTag], 0.1f, 0);
      scale = floatparse(l_arg, Tag[ScaleTag], 0.3f, 1);
      set_length = floatparse(l_arg, Tag[LengthTag], 0.1f, 0);
      reference = floatparse(l_arg, Tag[ReferenceTag], 0, 0);
      std_length = viewport.Width - LineStartpos.X;
      perm_flag = have(l_arg,Tag[ValueTag]);
      if (have(l_arg,Tag[WidebarTag])) {
        horiz = true;
        containertype = widebar;
        std_length = 501.12f;
        min_length = 188.8f;
      }
      else if (have(l_arg,Tag[SmallbarTag])) {
        horiz = true;
        containertype = smallbar;
        std_length = 241.92f;
        min_length = 83.04f;
      }
      else if (have(l_arg,Tag[TallbarTag])) {
        verti = true;
        containertype = tallbar;
        std_length = 501.12f;
        min_length = 123.12f;
      }
      else if (have(l_arg,Tag[ShortbarTag])) {
        verti = true;
        containertype = shortbar;
        std_length = 239.04f;
        min_length = 89.04f;
      }
      else if (have(l_arg,Tag[SingleIconTag])) {
        containertype = singleicon;
      }
      else if (have(l_arg,Tag[NoIconTag])||(clone>=0)) {
        horiz = true;
        containertype = textline;
      }
      if (horiz||verti) scale=1;
      l_arg = teil(arg , 0, optionpos);
    } else {
      opt_chc = opt_def;
      added_txt = "";
      l_arg = arg;
    }
    if (set_length < min_length) set_length = std_length;
    groupAndOptions = l_arg.Trim().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
    if (groupAndOptions.Count()==0) containertype = textline;
    if (continueScreen) {
      continueScreen = CycleDrawBlocks(groupAndOptions);
      lastDraw = containertype;
      if (opt_chc[12]) lastDraw+=100;
      lastscale = scale;
      lastLength = set_length;
    }
    if (perm_flag) permanent = percent;
    if (spritetagpos >= 0) {
      var sprite = new MySprite();
      if (getsprite(teil(U_arg, spritetagpos + Tag[SpriteTag].Length), out sprite, out LineStartpos.Y)) frame.Add(sprite);
        last_fsize = 0f;
    }
  }
}

bool CycleDrawBlocks(string[] groupOrBlocknames) {
  Vector2 posContainerGroup = nulvec;
  percent = 0;
  float integrity = 0;
  bool continueBlocks = true;
  ClearGroupArrays();
  Group.Clear();
  headline.Clear();
  bool same = (containertype == lastDraw);
  lastDraw%=100;
  if (same && opt_chc[11]) lastDraw = nothing;
  switch (lastDraw) {
    case widebar:
      if (!same || (pos.X + lastLength) >= viewport.Width) {LineStartpos.Y += 72; pos = LineStartpos;}
    break;
    case smallbar:
      if (!same || (pos.X + lastLength) >= viewport.Width) {LineStartpos.Y += 54; pos = LineStartpos;}
    break;
    case tallbar:
      if (!same || (lastLength != set_length) || (pos.X + 72) >= viewport.Width) {LineStartpos.Y += lastLength + 2.88f; pos = LineStartpos;}
    break;
    case shortbar:
      if (!same || (lastLength != set_length) || (pos.X + 62) >= viewport.Width) {LineStartpos.Y += lastLength + 2.88f; pos = LineStartpos;}
    break;
    case singleicon:
      if (!same || (pos.X + 72*lastscale) >= viewport.Width || !opt_chc[4]) {LineStartpos.Y += 95*lastscale; pos = LineStartpos;}
    break;
    case multiicon:
      LineStartpos.Y += 95*lastscale; pos = LineStartpos;
    break;
    case textline:
      LineStartpos.Y += (28.75f*last_fsize); pos = LineStartpos;
    break;
    default: break;
  }
  if (fontsize==0f) {
    if (containertype==widebar || containertype==tallbar) fontsize=1.3f; else fontsize=0.8f;
  }
  float offset = 28.75f*fontsize;
  if ((containertype == multiicon || containertype == singleicon) && (!opt_chc[4] || added_txt != "")) {
    LineStartpos.Y += offset;
    posContainerGroup = pos = LineStartpos;
  }
  foreach (string entry in groupOrBlocknames) {
    string name = entry.Trim();
    iconcounter = 0;
    if (iconcount < 0) {
      if (containertype == multiicon && name.EndsWith("#")) {
        iconcounter = name.Length - name.TrimEnd('#').Length;
        name = name.TrimEnd('#');
      }
    } else {
      iconcounter = iconcount;
    }
    verbatim = verbatimdefault || (name.StartsWith("\"") && name.EndsWith("\""));
    if (verbatim) name = name.Trim('"');
    var foundgroup = GridTerminalSystem.GetBlockGroupWithName(name);
    if (foundgroup != null && !opt_chc[7]) {
      if (groupOrBlocknames.Length == 1 && added_txt == "" && !opt_chc[10] && !opt_chc[4]) {
        headline.Append(foundgroup.Name + ": ");
      }
      foundgroup.GetBlocks(Group, (x => (x.CubeGrid == Me.CubeGrid && opt_chc[5]) || (!opt_chc[5] && !opt_chc[6]) || (x.CubeGrid != Me.CubeGrid && opt_chc[6])));
    } else {
      Group.Clear();
    }
    if (Group.Count == 0) {
      GridTerminalSystem.SearchBlocksOfName(name, Group, (x => (x.CubeGrid == Me.CubeGrid && opt_chc[5]) || (!opt_chc[5] && !opt_chc[6]) || (x.CubeGrid != Me.CubeGrid && opt_chc[6])));
    }
    for (int i = 0; i < iconcounter; i++) Group.Add(Me);
    if (TL(name).StartsWith(TL(Tag[GapTag].TrimEnd('=')))) {
      int gap = intparse(TL(name), Tag[GapTag], 0, -1);
      gap =  intparse(TL(name), Tag[GapTag].TrimEnd('='), 0, gap);
      if (gap>0) {
        pos.X += gap;
        Group.Clear();
      }
    }
    else if (Group.Count < 1) warnings+="No group or block with the name:\n\""+name+"\"\nfound as listed in the to-do list of:\n\""+Displayname+"\"\n\n";
    bool continuegroup = true;
    int place_used = 0;
    foreach (var block in Group) {
      if (verbatim && block != Me && block.CustomName != name) continue;
      if (!GetBlockProperties(block, ref integrity)) continue;
      if (iconcounter > 0 && place_used >= iconcounter) continue;
      if (!continuegroup || containertype != multiicon) continue;
      if (containertype == multiicon) p_abs();
      if ((pos.X + 72*scale) > viewport.Width && !opt_chc[11]) {
        if (!opt_chc[8]) {
          if ((pos.Y + 170*scale) > viewport.Bottom) {
            continuegroup = false;
          } else {
            LineStartpos.Y += 95*scale;
            pos = LineStartpos;
          }
        } else {
          continuegroup = false;
        }
      }
      if (continuegroup) {
        if (pattern == patternNone) percent = 0;
        SetBarSettings();
        integrityLevelColor  = var_col(1 - (float)Math.Pow(integrity,3));
        DrawObject(pos, integrity);
        pos.X += 72*scale;
        place_used++;
      }
    }
  }
  if (sumAll[0] + sumBlock == 0) {
    if (containertype == multiicon) {LineStartpos.Y -= offset; pos = LineStartpos;}
    containertype = textline;
  } else {
    float f_pct = 0;
    int cnt= 0 ;
    if (opt_chc[13]) {
      if (sumPwr[0]>0) {f_pct += (sumPwr[1] / set_ref(sumPwr[0],0.001f)); cnt++;}
      if (sumChg[0]>0) {f_pct += (sumChg[1] / set_ref(sumChg[0],0.001f)); cnt++;}
      if (sumCar[0]>0) {f_pct += (sumCar[1] / set_ref(sumCar[0])); cnt++;}
      if (sumVol[0]>0) {f_pct += (sumVol[1] / set_ref(sumVol[0])); cnt++;}
      if (sumTht[0]>0) {f_pct += (sumTht[1] / sumTht[0]); cnt++;}
      if (cnt>0) {f_pct = 100 * f_pct / cnt;}
    }
    if (sumAll[0]>0 && cnt == 0) f_pct = sumAll[1] / sumAll[0];
    percent = f_pct;
    p_abs();
  }
  if ((sumAll[0] + sumBlock > 0) && (containertype!=multiicon)) SetBarSettings();
  switch (containertype) {
    case widebar:
      scrollrange = set_length-28.8f;
      posHeadline = pos + new Vector2(14.4f, 36.4f-(16.63f*fontsize));
      fieldpos = pos + new Vector2(14.4f, 12);
      fieldsize = new Vector2(scrollrange, 49);
      DrawObject(pos);
      pos.X += set_length + 2.88f;
    break;
    case smallbar:
      scrollrange = set_length-23.04f;
      posHeadline = pos + new Vector2(11.52f, 28.36f-(17.95f*fontsize));
      fieldpos = pos + new Vector2(11.52f, 9);
      fieldsize = new Vector2(scrollrange, 33);
      DrawObject(pos);
      pos.X += set_length + 2.88f;
    break;
    case tallbar:
      scrollrange = set_length-123.12f;
      posHeadline = pos + new Vector2(36, 109);
      fieldpos = pos + new Vector2(12, 109);
      fieldsize = new Vector2(48, scrollrange);
      DrawObject(pos);
      pos.X += 75;
    break;
    case shortbar:
      scrollrange = set_length-90.48f;
      posHeadline = pos + new Vector2(30.5f, 80);
      fieldpos = pos + new Vector2(10, 80);
      fieldsize = new Vector2(41, scrollrange);
      DrawObject(pos);
      pos.X += 64;
    break;
    case singleicon:
      posHeadline = fieldpos = posContainerGroup + new Vector2(0, -offset);
      scrollrange = set_length;
      fieldsize = new Vector2(scrollrange,offset);
      DrawObject(pos);
      pos.X += 72*scale;
    break;
    case multiicon:
      posHeadline = fieldpos = posContainerGroup + new Vector2(0, -offset);
      scrollrange = set_length;
      fieldsize = new Vector2(scrollrange,offset);
    break;
    case textline:
      posHeadline = fieldpos = LineStartpos;
      scrollrange = set_length;
      fieldsize = new Vector2(scrollrange,offset);
      if ((pos.Y + offset) >= viewport.Height) continueBlocks = false;
    break;
  }
  if (containertype!=textline) {
    if (pos.Y > viewport.Height) continueBlocks = false;
  }
  headline.Append(added_txt);
  if (!opt_chc[4]) {
    if (opt_chc[10]) {
      StopSep = StartSep = "";
    } else {
      if (verti) {
        StartSep = "["; StopSep = "]";
      } else {
        StartSep = "[ "; StopSep = " ]";
      }
    }
    if (sumBat[0] > 0) {
      headline.Append(hlstart("Battery", sumBat[0]));
      if (opt_chc[0]) headline.Append("In " + TruncateUnit(sumBat[1], "watt") + "Out ");
      else headline.Append(f_percent(sumBat));
      headline.Append(TruncateUnit(sumBat[2], "whr") + StopSep);
    }
    if (sumReactor[0] > 0) {
      headline.Append(hlstart("Reactor", sumReactor[0]));
      if (opt_chc[0]) headline.Append("Output " + f_percent(sumReactor) + TruncateUnit(sumReactor[2], "watt") + StopSep);
      else headline.Append(TruncateUnit(sumReactor[2], "kg") + "Uranium " + StopSep);
    }
    if (sumGasgen[0] > 0) {
      headline.Append(hlstart("Hydrogen Engine", sumGasgen[0]));
      if (opt_chc[0]) headline.Append("Output " + f_percent(sumGasgen) + TruncateUnit(sumGasgen[2], "watt") + StopSep);
      else headline.Append(f_percent(sumGasgen) + TruncateUnit(sumGasgen[2], "lit") + StopSep);
    }
    if (sumHydro[0] > 0) {
      headline.Append(hlstart("Hydrogen Tank", sumHydro[0]));
      if (opt_chc[0]) headline.Append(f_percent(sumHydro) + TruncateUnit(sumHydro[2], "c_l") + StopSep);
      else headline.Append(f_percent(sumHydro) + TruncateUnit(sumHydro[2], "lit") + StopSep);
    }
    if (sumOxy[0] > 0) {
      headline.Append(hlstart("Oxygen Tank", sumOxy[0]));
      if (opt_chc[0]) headline.Append(f_percent(sumOxy) + TruncateUnit(sumOxy[2], "c_l") + StopSep);
      else headline.Append(f_percent(sumOxy) + TruncateUnit(sumOxy[2], "lit") + StopSep);
    }
    if (sumRefinery[0] > 0) {
      headline.Append(hlstart("Refinery", sumRefinery[0]));
      if (opt_chc[0]) headline.Append("Ingots " + f_percent(sumRefinery) + TruncateUnit(sumRefinery[2], "c_l") + StopSep);
      else headline.Append("Ores " + f_percent(sumRefinery) + TruncateUnit(sumRefinery[2], "c_l") + StopSep);
    }
    if (sumAssembler[0] > 0) {
      headline.Append(hlstart("Assembler", sumAssembler[0]));
      if (opt_chc[0]) headline.Append("Items Inventory " + f_percent(sumAssembler) + "Item Completion: " + Math.Round(sumAssembler[2] * 100 / sumAssembler[0], 0) + "% " + StopSep);
      else headline.Append("Ingot Inventory " + f_percent(sumAssembler) + "Production Queue: " + plSing(" Item", sumAssembler[2]) + StopSep);
    }
    if (sumDoor[0] > 0) {
      headline.Append(hlstart("Door", sumDoor[0]) + f_percent(sumDoor));
      if (opt_chc[0]) headline.Append("open" + StopSep);
      else headline.Append("closed" + StopSep);
    }
    if (sumRotor[0] > 0) {
      headline.Append(hlstart("Rotor", sumRotor[0]) + f_percent(sumRotor));
      if (opt_chc[0]) headline.Append("to the right" + StopSep);
      else headline.Append("to the left" + StopSep);
    }
    if (sumHinge[0] > 0) {
      headline.Append(hlstart("Hinge", sumHinge[0]) + f_percent(sumHinge));
      if (opt_chc[0]) headline.Append("to the right" + StopSep);
      else headline.Append("to the left" + StopSep);
    }
    if (sumCockpit[0] > 0) {
      headline.Append(hlstart("Cockpit", sumCockpit[0]) + f_percent(sumCockpit));
      if (opt_chc[0]) headline.Append("occupied" + StopSep);
      else headline.Append("Storage" + TruncateUnit(sumCockpit[2], "c_l") + StopSep);
    }
    if (sumConnectors[0] > 0) {
      headline.Append(hlstart("Connector", sumConnectors[0]));
      if (opt_chc[0]) headline.Append((sumConnectors[1]-sumConnectors[2])/100 + " connected / " + sumConnectors[2]/50 + " ready but open" + StopSep);
      else headline.Append(f_percent(sumConnectors) + "Storage" + TruncateUnit(sumConnectors[2], "c_l") + StopSep);
    }
    if (sumJdrives[0] > 0) headline.Append(hlstart("Jumpdrive", sumJdrives[0]) + f_percent(sumJdrives) + TruncateUnit(sumJdrives[2], "whr") + StopSep);
    if (sumCargo[0] > 0) headline.Append(hlstart("Container", sumCargo[0]) + f_percent(sumCargo) + TruncateUnit(sumCargo[2], "c_l") + StopSep);
    if (sumPara[0] > 0) headline.Append(hlstart("Parachute Crate", sumPara[0]) + f_percent(sumPara) + TruncateUnit(sumPara[2], "c_l") + StopSep);
    if (sumSorter[0] > 0) headline.Append(hlstart("Sorter", sumSorter[0]) + f_percent(sumSorter) + TruncateUnit(sumSorter[2], "c_l") + StopSep);
    if (sumCollectors[0] > 0) headline.Append(hlstart("Collector", sumCollectors[0]) + f_percent(sumCollectors) + TruncateUnit(sumCollectors[2], "c_l") + StopSep);
    if (sumDrills[0] > 0) headline.Append(hlstart("Drill", sumDrills[0]) + f_percent(sumDrills) + TruncateUnit(sumDrills[2], "c_l") + StopSep);
    if (sumWelder[0] > 0) headline.Append(hlstart("Welder", sumWelder[0]) + f_percent(sumWelder) + TruncateUnit(sumWelder[2], "c_l") + StopSep);
    if (sumGrinder[0] > 0) headline.Append(hlstart("Grinder", sumGrinder[0]) + f_percent(sumGrinder) + TruncateUnit(sumGrinder[2], "c_l") + StopSep);
    if (sumEShield[0] > 0) headline.Append(hlstart("Shield Generator", sumEShield[0]) + f_percent(sumEShield) + "Required Input: " + eShieldInput + " " + StopSep);
    if (sumShield[0] > 0) headline.Append(hlstart("Shield Controller", sumShield[0]) + Math.Round(sumShield[1], 0) + "% Overheated: " + Math.Round(sumShield[2], 0) + "% " + StopSep);
    if (sumMissileTurret[0] > 0) headline.Append(hlstart("Missile Turret", sumMissileTurret[0]) + f_percent(sumMissileTurret) + plSing(" Missile", sumMissileTurret[2]) + " " + StopSep);
    if (sumGatlingTurret[0] > 0) headline.Append(hlstart("Gatling Turret", sumGatlingTurret[0]) + f_percent(sumGatlingTurret) + plSing(" Unit", sumGatlingTurret[2]) + " " + StopSep);
    if (sumMissileLauncher[0] > 0) headline.Append(hlstart("Rocket Launcher", sumMissileLauncher[0]) + f_percent(sumMissileLauncher) + plSing(" Rocket", sumMissileLauncher[2]) + " " + StopSep);
    if (sumGatling[0] > 0) headline.Append(hlstart("Gatling Gun", sumGatling[0]) + f_percent(sumGatling) + plSing(" Unit", sumGatling[2]) + " " + StopSep);
    if (sumThrust[0] > 0) headline.Append(hlstart("Thruster", sumThrust[0]) + f_percent(sumThrust) + "Eff. " + Math.Round(sumThrust[2] * 100 / sumThrust[0], 0) + "% Override" + StopSep);
    if (sumSolar[0] > 0) headline.Append(hlstart("Solar Panel", sumSolar[0]) + "Output " + f_percent(sumSolar) + TruncateUnit(sumSolar[2], "watt") + StopSep);
    if (sumWindmill[0] > 0) headline.Append(hlstart("Wind Turbine", sumWindmill[0]) + "Output " + f_percent(sumWindmill) + TruncateUnit(sumWindmill[2], "watt") + StopSep);
    if (sumO2Gen[0] > 0) headline.Append(hlstart("O2H2 Generator", sumO2Gen[0]) + f_percent(sumO2Gen) + TruncateUnit(sumO2Gen[2], "c_l") + StopSep);
    if (sumProjector[0] > 0) headline.Append(hlstart("Projector", sumProjector[0]) + f_percent(sumProjector) + "Remaining: " + plSing(" Block", sumProjector[2]) + StopSep);
    if (sumVent[0] > 0) headline.Append(hlstart("Air Vent", sumVent[0]) + f_percent(sumVent) + "Pressure" + StopSep);
    if (sumPiston[0] > 0) headline.Append(hlstart("Piston", sumPiston[0]) + f_percent(sumPiston) + "extended" + StopSep);
    if (sumBlock > 0) headline.Append(hlstart("unidentified Block", sumBlock) + StopSep);
  }
  if (paints[TXT]) mod_col(ref Colors[TXT]);
  if (clone_txt.Length==0) {
    DrawHeadline();
    last_fsize = fontsize;
  } else {
    AddTextSprite(LineStartpos , clone_txt.ToString(), clone_font, fontsize, Colors[TXT], textpos);
    last_fsize = sb_size(clone_txt, "Y")/28.75f;
  }
  return continueBlocks;
}

void DrawHeadline() {
  frame.Add(new MySprite(SpriteType.CLIP_RECT,"",fieldpos,fieldsize));
  if (screenIndex < cs && headlineIndex < ce) scrollindex = scrollpositions[screenIndex, headlineIndex]; else scrollindex = 0;
  StringBuilder headline_mod = new StringBuilder();
  string dim = "X", outtxt;
  bool scrolling = !opt_chc[9];
  if (verti) {
    for (int i = 0; i < headline.Length; i++) headline_mod.Append(headline[i]+"\n");
    dim = "Y";
  } else {
    headline_mod = headline;
  }
  scrolling &= (scrollindex > 0 || sb_size(headline_mod,dim) > (scrollrange + sb_size(new StringBuilder("\n"),dim)));
  float second_pos = scrl_spd*(int)((sb_size(headline_mod,dim)+(scrollrange/2))/scrl_spd);
  outtxt = headline_mod.ToString();
  if (verti) {
    AddTextSprite(posHeadline - new Vector2(0, scrollindex) , outtxt, "Debug", fontsize, Colors[TXT], C_align);
    if (scrolling) AddTextSprite(posHeadline + new Vector2(0,(second_pos - scrollindex)) , outtxt, "Debug", fontsize, Colors[TXT], C_align);
  } else {
    TextAlignment align = textpos;
    if (scrolling) {posHeadline.X -= scrollindex; align = L_align;}
    else if (textpos == R_align) posHeadline.X += scrollrange;
    else if (textpos == C_align) posHeadline.X += (scrollrange / 2);
    AddTextSprite(posHeadline , outtxt, "Debug", fontsize, Colors[TXT], align);
    if (scrolling) {
      posHeadline.X+=second_pos;
      AddTextSprite(posHeadline , outtxt, "Debug", fontsize, Colors[TXT], align);
    }
  }
  if ((scrollindex + scrl_spd) >= second_pos || !scrolling) scrollindex = 0; else scrollindex+=scrl_spd;
  frame.Add(MySprite.CreateClearClipRect());
  headline.Clear();
  if (!opt_chc[9] && screenIndex < cs && headlineIndex < ce) {
    scrollpositions[screenIndex, headlineIndex] = scrollindex;
    headlineIndex++;
  }
}

float sb_size(StringBuilder sb, string dim = "X") {
  var t_vec = surface.MeasureStringInPixels(sb, "Debug", fontsize);
  if (dim=="X") return t_vec.X; else return t_vec.Y;
}

string hlstart(string str, float number) {
  if (opt_chc[10]) return StartSep;
  else return StartSep + plSing(str, number) + ": ";
}

string plSing(string str, float number) {
  if (number != 1) {
    if (str.EndsWith("ry")) return number + " " + str.Replace("ry", "ries");
    else return number + " " + str + "s";
  }
  else return number + " " + str;
}

string TruncateUnit(float number, string unit) {
  string str = "";
  if (unit == "whr") str = calculateunits(number, " T", " G", " M", " K", 107.4162075f, 10) + "Wh";
  if (unit == "lit") str = calculateunits(number / 1000, "G ", "M ", "K ", " ", 107.4162075f, 9) + "L";
  if (unit == "c_l") str = calculateunits(number, "G ", "M ", "K ", " ", 107.4162075f, 9) + "L";
  if (unit == "watt") str = calculateunits(number, " T", " G", " M", " K", 107.4162075f, 9) + "W";
  if (unit == "kg") str = calculateunits(number, "K ton", " ton", " Kg", " g", 133.8810875f, 11);
  return str + "  ";
}

string calculateunits(float number, string sign1, string sign2, string sign3, string sign4, float width, int len) {
  StringBuilder sb = new StringBuilder();
  if (number >= 1000000) sb.Append(Math.Round((number / 1000000f), 2) + sign1);
  else if (number >= 1000) sb.Append(Math.Round((number / 1000f), 2) + sign2);
  else if (number >= 1) sb.Append(Math.Round(number, 2) + sign3);
  else sb.Append(Math.Round((number * 1000f), 2) + sign4);
  return adjust_length(width, len, sb);
}

void p_abs() {
  if (percent<0) {
    percent = clamp(-percent);
    invertColor = !invertColor;
  }
}

string f_percent(float[] werte) {
  StringBuilder sb = new StringBuilder();
  sb.AppendFormat("{0:N0}% ", TestForNaN(werte[1]/werte[0]));
  return adjust_length(61.491895f, 5, sb);
}

String adjust_length(float soll, int len, StringBuilder sb) {
  int temp_i;
  if (verti) {
    temp_i = Math.Max(len-sb.Length,0);
  } else {
    temp_i = (int)Math.Max(((soll*fontsize) - sb_size(sb)) / sb_size(new StringBuilder(" ")),0f);
  }
  sb.Insert(0,new String(' ',temp_i));
  return sb.ToString();
}

bool GetBlockProperties(IMyTerminalBlock block, ref float integrity) {
  invertColor = false;
  bool isValidBlock = true;
  string blocktype = "";
  string subtype = block.BlockDefinition.SubtypeName;
  int Ref_inv;
  bsize1 = bsize2 = opt_txt1 = sym2 = drawpercent = "";
  if (subtype.Contains("DSControl")) {
    sumShield[0]++; sumAll[0]++;
    float.TryParse(getBetween(block.CustomInfo, " (", "%)"), out percent);
    sumShield[1] = percent; sumAll[1] += percent;
    float.TryParse(getBetween(block.CustomInfo, "[Over Heated]: ", "%"), out sumShield[2]);
    sym1 = sym_Shield;
    pattern = patternShield;
  }
  else if (subtype.Contains("ShieldGenerator")) {
    if (!block.CustomName.Contains(":")) block.CustomName = block.CustomName + ":";
    sumEShield[0]++; sumAll[0]++;
    percent = TestForNaN(100 / EnergyShield.MaxHitpoints(block) * EnergyShield.CurrentHitpoints(block));
    sumEShield[1] = percent; sumAll[1] += percent;
    eShieldInput = EnergyShield.RequiredInput(block);
    sym1 = sym_Shield;
    pattern = patternShield;
  }
  else if (block == Me) {
    sym2 = "Cross";
    pattern = patternNone;
    opt_txt1 = "UNKNOWN";
    drawpercent = "???";
    percent = 0;
    bsize1 = bsize2 = " ";
  } else {
    blocktype = teil(block.ToString(),2, block.ToString().IndexOf('{')-2).Trim();
  }
  switch (blocktype) {
    case "BatteryBlock":
      get_size(block,12,1);
      var bat = block as IMyBatteryBlock;
      float maxstore, maxout, maxin, store, output, input;
      maxstore = set_ref(bat.MaxStoredPower,0.001f);
      maxout = set_ref(bat.MaxOutput,0.001f);
      maxin = set_ref(bat.MaxInput,0.001f);
      store = bat.CurrentStoredPower;
      input = bat.CurrentInput;
      output = bat.CurrentOutput;
      if (opt_chc[0]) {
        opt_txt1 = "Load";
        invertColor = true;
        percent = clamp(100 * output / maxout) - clamp(100 * input / maxin);
        sumBat[1] += input;
        sumBat[2] += output;
        sumPwr[0] += maxout;
        sumPwr[1] += output - input;
      } else {
        opt_txt1 = "Charge";
        store = bat.CurrentStoredPower;
        percent = clamp(100 * store / maxstore);
        sumBat[1] += percent;
        sumBat[2] += store;
        sumChg[0] += bat.MaxStoredPower;
        sumChg[1] += store;
      }
      sumAll[1] += percent;
      sumBat[0]++;
      sumAll[0]++;
      sym2 = "IconEnergy";
      pattern = patternBattery;
    break;
    case "Reactor":
      get_size(block,8,8);
      if (opt_chc[0]) {
        opt_txt1 = "Load";
        invertColor = true;
        PowerOutput2Arr(block, sumReactor, ref percent);
      } else {
        opt_txt1 = "Fuel";
        CargoVol2Arr(block, sumReactor, ref percent, 1);
      }
      sym1 = sym_Reactor;
      pattern = patternReactor;
    break;
    case "HydrogenEngine":
      if (opt_chc[0]) {
        opt_txt1 = "Load";
        invertColor = true;
        PowerOutput2Arr(block, sumGasgen, ref percent);
      } else {
        float MaxVol,CurVol;
        opt_txt1 = block.DetailedInfo.Replace('л','L')+"L)";
        float.TryParse(getBetween(opt_txt1, " (", "L/"), out CurVol);
        float.TryParse(getBetween(opt_txt1, "L/", "L)"), out MaxVol);
        opt_txt1 = "Fuel";
        sumVol[0] += CurVol;
        sumVol[1] += MaxVol;
        MaxVol = set_ref(MaxVol);
        sumGasgen[0]++;
        sumAll[0]++;
        percent = clamp(100 * CurVol / MaxVol);
        sumGasgen[1] += percent;
        sumAll[1] += percent;
        sumGasgen[2] += CurVol;
      }
      sym1 = sym_Engine;
      pattern = patternGasgen;
    break;
    case "CargoContainer":
      get_size(block,14,8);
      if (subtype=="SmallBlockModularContainer") bsize1="X";
      isValidBlock = CargoVol2Arr(block, sumCargo, ref percent);
      sym1 = sym_Cargo;
      opt_txt1 = "Fill";
      pattern = patternCargo;
      invertColor = true;
    break;
    case "Cockpit":
    case "CryoChamber":
      if (opt_chc[0]) {
        var cp = block as IMyCockpit;
        if (cp.IsUnderControl) {percent = 100; opt_txt1 = "occupied";} else {percent = 0; opt_txt1 = "empty";}
        sumCockpit[1] += percent;
        sumAll[1] += percent;
        sumCockpit[0]++;
        sumAll[0]++;
      } else {
        isValidBlock = CargoVol2Arr(block, sumCockpit, ref percent);
        opt_txt1 = "Filled";
      }
      invertColor = true;
      sym1 = sym_Cockpit;
      pattern = patternCargo;
    break;
    case "GasTank":
      if (subtype.Contains("Hydro")) {
        get_size(block,16,8);
        if (opt_chc[0]) {
          isValidBlock = CargoVol2Arr(block, sumHydro, ref percent);
        } else {
          TankVolume2Arr(block, sumHydro, ref percent);
        }
        sym2 = "IconHydrogen";
      } else {
  
        if (opt_chc[0]) {
          isValidBlock = CargoVol2Arr(block, sumOxy, ref percent);
        } else {
          TankVolume2Arr(block, sumOxy, ref percent);
        }
        sym2 = "IconOxygen";
      }
      if (opt_chc[0]) opt_txt1 = "Cargo"; else opt_txt1 = "Tank";
      pattern = patternTank;
    break;
    case "ShipConnector":
      get_size(block,12,1);
      opt_txt1 = "Fill";
      pattern = patternConnector;
      if (opt_chc[0]) {
        var cn = block as IMyShipConnector;
        if(cn.Status == MyShipConnectorStatus.Connected) {
          percent = 100;
          opt_txt1 = "locked";
        } else if(cn.Status == MyShipConnectorStatus.Connectable) {
          percent = 50;
          opt_txt1 = "ready";
          sumConnectors[2] += 50;
        } else {
          percent = 0;
          opt_txt1 = "open";
        }
        sumConnectors[1] += percent;
        sumAll[1] += percent;
        sumConnectors[0]++;
        sumAll[0]++;
      } else {
        isValidBlock = CargoVol2Arr(block, sumConnectors, ref percent);
        invertColor = true;
      }
      sym1 = sym_Connector;
    break;
    case "Collector":
      isValidBlock = CargoVol2Arr(block, sumCollectors, ref percent);
      opt_txt1 = "Fill";
      sym1 = sym_Collector;
      pattern = patternConnector;
      invertColor = true;
    break;
    case "ShipDrill":
      isValidBlock = CargoVol2Arr(block, sumDrills, ref percent);
      sym1 = sym_Drill;
      opt_txt1 = "Fill";
      pattern = patternTool;
      invertColor = true;
    break;
    case "JumpDrive":
      var jumpdrive = block as IMyJumpDrive;
      sumJdrives[0]++;
      sumAll[0]++;
      percent = 100 * jumpdrive.CurrentStoredPower / jumpdrive.MaxStoredPower;
      sumJdrives[1] += percent;
      sumAll[1] += percent;
      sumJdrives[2] += jumpdrive.CurrentStoredPower;
      sumChg[0] += jumpdrive.CurrentStoredPower;
      sumChg[1] += jumpdrive.MaxStoredPower;
      sym1 = sym_Jumpdrive;
      opt_txt1 = "Charge";
      pattern = patternJumpdrive;
    break;
    case "Thrust":
      get_size(block,10,10);
      var thruster = block as IMyThrust;
      sumThrust[0]++;
      sumAll[0]++;
      sumTht[0] += thruster.MaxEffectiveThrust;
      sumTht[1] += thruster.CurrentThrust;
      percent = clamp(100 * thruster.CurrentThrust / thruster.MaxEffectiveThrust);
      sumThrust[1] += percent;
      sumAll[1] += percent;
      sumThrust[2] += thruster.ThrustOverridePercentage;
      sym1 = sym_Thruster;
      opt_txt1 = "Thrst";
      pattern = patternThruster;
    break;
    case "SolarPanel":
     get_size(block);
      PowerOutput2Arr(block, sumSolar, ref percent);
      sym1 = sym_Solar;
      opt_txt1 = "Load";
      invertColor = true;
      pattern = patternSolar;
    break;
    case "WindTurbine":
      PowerOutput2Arr(block, sumWindmill, ref percent);
      sym1 = sym_Windmill;
      opt_txt1 = "Load";
      invertColor = true;
      pattern = patternWindmill;
    break;
    case "GasGenerator":
      isValidBlock = CargoVol2Arr(block, sumO2Gen, ref percent);
      sym1 = sym_Ice;
      opt_txt1 = "Fill";
      pattern = patternGasgen;
    break;
    case "ShipGrinder":
      isValidBlock = CargoVol2Arr(block, sumGrinder, ref percent);
      sym1 = sym_Grinder;
      opt_txt1 = "Fill";
      pattern = patternTool;
      invertColor = true;
    break;
    case "ShipWelder":
      isValidBlock = CargoVol2Arr(block, sumWelder, ref percent);
      sym1 = sym_Welder;
      opt_txt1 = "Fill";
      pattern = patternTool;
      invertColor = true;
    break;
    case "Refinery":
      get_size(block,8,8);
      Ref_inv = 0;
      if (!opt_chc[0]) {
        opt_txt1 = "Ores";
      } else {
        opt_txt1 = "Ingots";
        Ref_inv = 1;
      }
      isValidBlock = CargoVol2Arr(block, sumRefinery, ref percent, 0, Ref_inv);
      sym1 = sym_Refinery;
      pattern = patternRefinery;
      invertColor = true;
    break;
    case "Assembler":
      get_size(block,0,3);
      Ref_inv = 0;
      var Assblock = block as IMyAssembler;
      if (!opt_chc[0]) {
        opt_txt1 = "Ingots";
        List<MyProductionItem> queue = new List<MyProductionItem>();
        Assblock.GetQueue(queue);
        foreach (var item in queue) {sumAssembler[2] += ((float)item.Amount);}
      } else {
        opt_txt1 = "Items";
        sumAssembler[2] += Assblock.CurrentProgress;
        Ref_inv = 1;
      }
      isValidBlock = CargoVol2Arr(block, sumAssembler, ref percent, 2, Ref_inv);
      sym1 = sym_Assembler;
      pattern = patternAssembler;
      invertColor = true;
    break;
    case "LargeMissileTurret":
      CargoVol2Arr(block, sumMissileTurret, ref percent, 1);
      sym1 = sym_Rocket;
      opt_txt1 = "Ammo";
      pattern = patternTurret;
    break;
    case "LargeGatlingTurret":
    case "LargeInteriorTurret":
      get_size(block,20,8);
     CargoVol2Arr(block, sumGatlingTurret, ref percent, 1);
      sym1 = sym_Bullet;
      opt_txt1 = "Ammo";
      pattern = patternTurret;
    break;
    case "SmallGatlingGun":
      CargoVol2Arr(block, sumGatling, ref percent, 1);
      sym1 = sym_Bullet;
      opt_txt1 = "Ammo";
      pattern = patternTurret;
    break;
    case "SmallMissileLauncher":
    case "SmallMissileLauncherReload":
      get_size(block,10,0);
      CargoVol2Arr(block, sumMissileLauncher, ref percent, 1);
      sym1 = sym_Rocket;
      opt_txt1 = "Ammo";
      pattern = patternTurret;
    break;
    case "SpaceProjector":
      var projector = block as IMyProjector;
      var total = projector.TotalBlocks;
      sumProjector[0]++;
      sumAll[0]++;
      if (total != 0) percent = 100 * (total - projector.RemainingBlocks) / total;
      sumProjector[1] += percent;
      sumAll[1] += percent;
      sumProjector[2] += projector.RemainingBlocks;
      sym1 = sym_Projector;
      opt_txt1 = "Done";
      pattern = patternProjector;
    break;
    case "AirVent":
      var vent = block as IMyAirVent;
      sumVent[0]++;
      sumAll[0]++;
      percent = (float)Math.Round(vent.GetOxygenLevel() * 100);
      sumVent[1] += percent;
      sumAll[1] += percent;
      sym1 = sym_Vent;
      opt_txt1 = "Press";
      pattern = patternVent;
    break;
    case "Parachute":
      isValidBlock = CargoVol2Arr(block, sumPara, ref percent);
      sym1 = sym_Para;
      opt_txt1 = "Fill";
      pattern = patternCargo;
    break;
    case "ConveyorSorter":
      get_size(block,10,0);
      isValidBlock = CargoVol2Arr(block, sumSorter, ref percent);
      sym1 = sym_Sorter;
      opt_txt1 = "Fill";
      pattern = patternConnector;
      invertColor = true;
    break;
    case "AirtightHangarDoor":
    case "Door":
    case "AirtightSlideDoor":
      var door = block as IMyDoor;
      sumDoor[0]++;
      sumAll[0]++;
      percent = (float)Math.Round(door.OpenRatio * 100);
      opt_txt1 = "open";
      invertColor = true;
      if (!opt_chc[0]) {
        opt_txt1 = "closed";
        percent=100-percent;
        invertColor = false;
      }
      sumDoor[1] += percent;
      sumAll[1] += percent;
      sym1 = sym_Door;
      pattern = patternJumpdrive;
    break;
    case "MotorAdvancedStator":
    case "MotorStator":
      get_size(block,14,0);
      var rotor = block as IMyMotorStator;
      opt_txt1 = "left";
      float max_ang=rotor.UpperLimitRad;
      float min_ang=rotor.LowerLimitRad;
      float angle=rotor.Angle;
      if (max_ang>rad || min_ang<-rad) {
        percent = 100 * ((angle+rad)%rad)/ rad;
      } else {
        percent = 100 * (angle - min_ang) / (max_ang - min_ang);
      }
      percent = clamp(percent);
      if (opt_chc[0]) {
        opt_txt1 = "right";
        percent=100-percent;
      }
      sumAll[0]++;
      sumAll[1] += percent;
      if (subtype.Contains("Hinge")) {
        sumHinge[0]++;
        sumHinge[1] += percent;
        sym1 = sym_Hinge;
      } else {
        sumRotor[0]++;
        sumRotor[1] += percent;
        sym1 = sym_Rotor;
      }
      pattern = patternTool;
      invertColor = true;
    break;
    case "ExtendedPistonBase":
      var piston = block as IMyPistonBase;
      sumPiston[0]++;
      sumAll[0]++;
      percent = 100 * (piston.CurrentPosition - piston.MinLimit) / (piston.MaxLimit - piston.MinLimit);
      sumPiston[1] += percent;
      sumAll[1] += percent;
      sym1 = sym_Piston;
      pattern = patternTool;
      invertColor = true;
    break;
    case "":
    break;
    default:
      pattern = patternNone;
      sym1 = sym_Block;
      if(opt_chc[3]) {
        sumBlock++;
        drawpercent = "-----";
      }
      else {isValidBlock = false;}
    break;
  }

  if (clone>=0) {
    var screen = block as IMyTextSurfaceProvider;
    if (clone < screen.SurfaceCount) {
      IMyTextSurface content = screen.GetSurface(clone);
      content.ReadText(clone_txt);
      clone_font = content.Font;
    }
  }
  if (isValidBlock) {
    if (bsize1=="") get_size(block);
    if(opt_chc[3]) integrity = TestForNaN(GetMyTerminalBlockHealth(block.CubeGrid.GetCubeBlock(block.Position))); else integrity = 0;
    if(!paints[FRM]) {
      if (block == Me) {Colors[FRM] = frameColorIsNotThere; integrity = 0;}
      else if (block.IsWorking) Colors[FRM] = frameColorIsWorking;
      else if (block.IsFunctional) Colors[FRM] = frameColorIsFunctional;
      else Colors[FRM] = frameColorIsUnfunctional;
    }
  } else {
    blocktype = subtype = bsize1 = bsize2 = opt_txt1 = "";
  }
  return isValidBlock;
}

float GetMyTerminalBlockHealth(IMySlimBlock slimblock) {
  return (slimblock.BuildIntegrity - slimblock.CurrentDamage) / slimblock.MaxIntegrity;
}

void PowerOutput2Arr(IMyTerminalBlock block, float[] arr, ref float percent) {
  var powProd = block as IMyPowerProducer;
  arr[0]++;
  sumAll[0]++;
  float power = powProd.CurrentOutput;
  float maxpwr = set_ref(powProd.MaxOutput,0.001f);
  percent = clamp(100 * power / maxpwr);
  arr[1] += percent;
  sumAll[1] += percent;
  arr[2] += power;
  sumPwr[0] += maxpwr;
  sumPwr[1] += power;
}

bool CargoVol2Arr(IMyTerminalBlock block, float[] arr, ref float percent, int arr2 = 0, int inv = 0) {
  bool result = (block.InventoryCount > inv);
  if (result) {
    arr[0]++;
    sumAll[0]++;
    float volume = 0f;
    if (filter=="") volume = (float)block.GetInventory(inv).CurrentVolume;
    else volume = filtervolume(block.GetInventory(inv));
    float maxvol = set_ref((float)block.GetInventory(inv).MaxVolume);
    percent = clamp(100 * volume / maxvol);
    arr[1] += percent;
    sumAll[1] += percent;
    sumCar[0] += maxvol;
    sumCar[1] += volume;
    if (arr2==0) arr[2] += volume;
    else if (arr2==1) arr[2] += itemCount(block);
  } else {
    percent = 0;
  }
  return result;
}

float filtervolume(IMyInventory inventory) {
  items.Clear();
  inventory.GetItems(items);
  int skips1=1;
  int skips2=0;
  bool and = false;
  bool p_flag,n_flag,e_flag,f_flag,l_flag,count_it=false;
  float result = 0;
  var filters = filter.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
  if(filters.Count()==0) return (float)inventory.CurrentVolume;
  if(filters.Count()==1) skips1=0;
  else if (filters.First()=="and"||filters.First()=="&") and = true;
  foreach (var item in items) {
    string filter3=TL(item.Type.ToString());
    foreach (var filter1 in filters.Skip(skips1)) {
      skips2=0;
      p_flag = n_flag = e_flag = f_flag =  l_flag = count_it = false;
      if (filter1[0]=='+') {skips2=1;p_flag = true;}
      else if (filter1[0]=='-') {skips2=1;p_flag = n_flag = true;}
      if (filter1.Length<=skips2) return result;
      if (filter1.EndsWith("*")) l_flag = true;
      else if (filter1[skips2]=='*') {skips2++;f_flag = true;}
      else if (filter1[skips2]=='=') {skips2++;e_flag = true;}
      string filter2=teil(filter1.TrimEnd('*'),skips2);
      if (filter2=="") return result;
      if (f_flag&&l_flag&&filter3.Contains(filter2)) count_it=true;
      else if (f_flag&&filter3.EndsWith(filter2)) count_it=true;
      else if (l_flag&&filter3.StartsWith(filter2)) count_it=true;
      if (e_flag&&(filter3==filter2)) count_it=true;
      else if (p_flag&&filter3.Contains(filter2)) count_it=true;
      if (n_flag) count_it=!count_it;
      if (count_it!=and) break;
    }
    if (count_it) result += (float)(MyPhysicalInventoryItemExtensions_ModAPI.GetItemInfo(item.Type).Volume*item.Amount);
  }
  return result;
}

float itemCount(IMyTerminalBlock block) {
  List<MyInventoryItem> items = new List<MyInventoryItem>();
  block.GetInventory().GetItems(items);
  float count = 0;
  foreach (var item in items) count += item.Amount.RawValue / 1000000;
  return count;
}

void TankVolume2Arr(IMyTerminalBlock block, float[] arr, ref float percent) {
  var tank = block as IMyGasTank;
  if (tank != null) {
    arr[0]++;
    sumAll[0]++;
    float cur_cap = tank.Capacity * (float)tank.FilledRatio;
    if (reference > 0) {
      percent = 100 * cur_cap / reference;
    } else {
      percent = 100 * (float)tank.FilledRatio;
    }
    arr[1] += percent;
    sumAll[1] += percent;
    arr[2] += cur_cap;
    sumVol[0] += tank.Capacity;
    sumVol[1] += cur_cap;
  }
}

float TestForNaN(float number) {
  if (double.IsNaN(number)) number = 0;
  return number;
}

void ClearGroupArrays() {
  ClearArray(sumBat);
  ClearArray(sumJdrives);
  ClearArray(sumCargo);
  ClearArray(sumCockpit);
  ClearArray(sumPara);
  ClearArray(sumSorter);
  ClearArray(sumHydro);
  ClearArray(sumOxy);
  ClearArray(sumVent);
  ClearArray(sumDoor);
  ClearArray(sumCollectors);
  ClearArray(sumConnectors);
  ClearArray(sumDrills);
  ClearArray(sumGrinder);
  ClearArray(sumWelder);
  ClearArray(sumGasgen);
  ClearArray(sumShield);
  ClearArray(sumGatling);
  ClearArray(sumMissileTurret);
  ClearArray(sumGatlingTurret);
  ClearArray(sumMissileLauncher);
  ClearArray(sumThrust);
  ClearArray(sumReactor);
  ClearArray(sumSolar);
  ClearArray(sumWindmill);
  ClearArray(sumO2Gen);
  ClearArray(sumRefinery);
  ClearArray(sumAssembler);
  ClearArray(sumProjector);
  ClearArray(sumEShield);
  ClearArray(sumHinge);
  ClearArray(sumPiston);
  ClearArray(sumRotor);
  ClearArray(sumAll);
  ClearArray(sumPwr);
  ClearArray(sumChg);
  ClearArray(sumCar);
  ClearArray(sumVol);
  ClearArray(sumTht);
  sumBlock=0;
}

void ClearArray(Array arr) {Array.Clear(arr, 0, arr.Length);}

void SetBarSettings() {
  percent = clamp(percent);
  chargeBarOffset = restBarOffset = nulvec;
  int div = 10;
  float v1,v2,v3,v4,v5,v6,v7,v8,mul=9;
  v1=(set_length-20.28f)/10;
  v2=v1-3;
  v3=set_length-11.64f-(v2/2);
  v4=(set_length-21.04f)/10;
  v5=v4-8;
  v6=set_length-14.52f-(v5/2);
  v7=v2*rest;
  v8=v5*rest;
  switch (containertype) {
    case widebar:
      chargebarpos = pos + new Vector2(14, 37);
      chargeBarOffset.X = v4;
      chargeBarSize = new Vector2(v5, 43);
    break;
    case smallbar:
      chargebarpos = pos + new Vector2(12, 27);
      chargeBarOffset.X = v1;
      chargeBarSize = new Vector2(v2, 29);
    break;
    case tallbar:
      chargebarpos = pos + new Vector2(14.5f, v6);
      chargeBarOffset.Y = -v4;
      chargeBarSize = new Vector2(43, v5);
      sym_pos = pos + new Vector2(36, 36);
      optionpos = pos + new Vector2(36, 65);
      numberpos = pos + new Vector2(36, 80);
    break;
    case shortbar:
      chargebarpos = pos + new Vector2(12,v3);
      chargeBarOffset.Y = -v1;
      chargeBarSize = new Vector2(37,v2);
      sym_pos = pos + new Vector2(30.5f, 32);
      optionpos = pos + new Vector2(30.5f, 53);
      numberpos = pos + new Vector2(30.5f, 61);
    break;
    case singleicon:
    case multiicon:
      div = 20;
      mul = 4;
      chargebarpos = pos + new Vector2(10.8f, 75)*scale;
      chargeBarOffset.Y = -14*scale;
      chargeBarSize = new Vector2(44, 9.2f)*scale;
      sym_pos = pos + new Vector2(33, 36)*scale;
      optionpos = pos + new Vector2(33, 54.5f)*scale;
      numberpos = pos + new Vector2(33, 62)*scale;
    break;
    default:
      div = 0;
    break;
  }
  restBarSize = chargeBarSize;
  if (div == 0) {
    barcount = 0;
    rest = 0;
  } else if (bar_type==analog || bar_type==solid) {
    restBarSize.X += chargeBarOffset.X*mul;
    restBarSize.Y += Math.Abs(chargeBarOffset.Y)*mul;
    chargebarpos.Y += chargeBarOffset.Y*mul/2;
    chargeBarSize = restBarSize;
    barcount = 0;
    rest = percent/100;
  } else {
    barcount = (int)(percent/div);
    rest = (percent%div)/div;
  }
  if (bar_type==solid) rest = 1;

  if (bar_type==digital) {barcount+=(int)(rest+0.5f); rest =0;}

  if (chargeBarOffset.Y < 0) {restBarSize.Y*=rest; restBarOffset.Y = chargeBarSize.Y*(1-rest)/2;}
  if (chargeBarOffset.X > 0) {restBarSize.X*=rest;}
  if (drawpercent == "") {drawpercent = String.Format("{0:N0}%",percent);}

  if (invertColor!=opt_chc[15]) {
    Colors[ICO] = var_col(percent/100);
    backg = 0.4f - (Math.Abs(45-percent)*0.02f);
  } else {
    Colors[ICO] = var_col(1-(percent/100));
    backg = 0.4f - (Math.Abs(55-percent)*0.02f);
  }
  if (!paints[BAR]) Colors[BAR] = Colors[ICO]; else backg = 0;
  Colors[ICO].B = (byte)(percent*2.55f);
}

float clamp(float wert,float max=100, float min=0) {return Math.Max(Math.Min(TestForNaN(wert),max),min);}

static string getBetween(string strSource, string strStart, string strEnd) {
  int tag = strStart.Length;
  int Start = 0;
  if (tag>0) Start = strSource.IndexOf(strStart);
  int End = strSource.IndexOf(strEnd, Start + tag);
  if (Start>=0 && End>=0) {
    Start+=tag;
    return teil(strSource, Start, End - Start);
  } else {
    return "";
  }
}

string getval(string strSource, string strStart, bool lit=false) {
  strSource+=" ";
  Char[] chars = new char[] {' ',',',':','\n'};
  if (lit&&have(TL(strSource),strStart+"\"")) {strStart+="\""; chars = new char[] {'"'};}
  int tag = strStart.Length;
  int Start = TL(strSource).IndexOf(TL(strStart));
  int End = strSource.IndexOfAny(chars, Start + tag);
  if (Start>=0 && End>=0) {
    Start+=tag;
    return teil(strSource, Start, End - Start);
  } else {
    return "";
  }
}

float floatparse(string line, string tag, float min, float result) {
  if (have(line,tag)&&(float.TryParse(getval(line, tag), out result))) result=Math.Max(result,min);
  return result;
}

int intparse(string line, string tag, int min, int result) {
  if (have(line,tag)&&(int.TryParse(getval(line, tag), out result))) result=Math.Max(result,min);
  return result;
}

bool have(string a,string b) {return a.Contains(TL(b));}

float set_ref(float regular, float factor=1) {
  if (reference > 0) regular = reference * factor / 1000;
  return regular;
}

void get_size(IMyTerminalBlock block, int small=0,int large=0) {
  int r = 0, s = small, l;
  Vector3I v = Vector3I.Abs(block.Max-block.Min);
  l = (v.X+v.Y+v.Z+1)*2;
  if (block.CubeGrid.GridSize>1) {r = 4;s = large;}
  if (s == 0) r+= 3;
  else if (s < l) r+= 2;
  else if (s == l) r+= 1;
  bsize1 = teil("SMLSML█",r,1);
  bsize2 = teil("████",r,1);
}

void DrawObject(Vector2 pos, float integrity = 0) {
  Color buffer_col = Colors[BAR];
  if (paints[BAR]) mod_col(ref Colors[BAR]);
  if (paints[SYM]) mod_col(ref Colors[SYM]);
  if (paints[PER]) mod_col(ref Colors[PER]);
  if (paints[OPT]) mod_col(ref Colors[OPT]);
  if (paints[FRM]) mod_col(ref Colors[FRM]);
  else if (containertype != multiicon) Colors[FRM] = frameColorWideBar;
  switch (containertype) {
    case tallbar:
      AddFrameSprite(pos, new Vector2(72,set_length), true, Colors[FRM]);
    break;
    case shortbar:
      AddFrameSprite(pos, new Vector2(60.48f,set_length), false, Colors[FRM]);
    break;
    case widebar:
      AddFrameSprite(pos, new Vector2(set_length,72), true, Colors[FRM]);
    break;
    case smallbar:
      AddFrameSprite(pos, new Vector2(set_length,51.84f), false, Colors[FRM]);
    break;
    default:
      for (int i = 0; i < pattern.Count();) {
        AddTextureSprite("SquareSimple", pos+(pattern[i++]*scale), pattern[i++]*scale, Colors[FRM], C_align);
      }
    break;
  }
  if (opt_chc[3] && (containertype == multiicon)) {
    chargeBarSize.X = restBarSize.X = 32*scale;
    var integritypos = pos + new Vector2(48, 79.5f - (integrity * 32.6f))*scale;
    var integritysize = new Vector2(9, 65.2f * integrity)*scale;
    AddTextureSprite("SquareSimple", integritypos, integritysize, integrityLevelColor, L_align);
    AddTextSprite(pos + new Vector2(52.5f, 12)*scale, "+\n+\n+\n+", "Monospace", 0.54f*scale, Color.Black, C_align);
    AddTextSprite(pos + new Vector2(52.5f, 20)*scale, "+\n+\n+\n+", "Monospace", 0.54f*scale, Color.Black, C_align);
  }
  for (int i = 0; i < barcount; i++) {
    AddTextureSprite("SquareSimple", chargebarpos, chargeBarSize, Colors[BAR], L_align);
    chargebarpos += chargeBarOffset;
  }
  if (rest>0) AddTextureSprite("SquareSimple", chargebarpos + restBarOffset, restBarSize, Colors[BAR], L_align);
  Colors[BAR] = buffer_col;
  if (horiz) return;
  if (sym3!="") sym2=sym3;
  if (sym2!="") {
    AddTextureSprite(sym2, sym_pos, new Vector2(39, 35)*scale, Colors[SYM], C_align);
  } else {
    sym_pos-=new Vector2(20.16f,20.16f)*scale;
    for (int i = 0; i < sym1.Count();) {
      AddTextureSprite("SquareSimple", sym_pos+(sym1[i++]*scale), sym1[i++]*scale, Colors[SYM], C_align);
    }
  }
  if (!opt_chc[1]) opt_txt1="";
  if (opt_txt2!="") opt_txt1=opt_txt2;
  if (opt_txt1!="") AddTextSprite(optionpos, opt_txt1, "Debug", 0.4f*scale, Colors[PER], C_align,backg);
  if (opt_chc[2] && (containertype == multiicon)) {
    AddTextSprite(pos + new Vector2(7,9)*scale, bsize1, "Monospace", 0.3f*scale, Colors[OPT], L_align);
    AddTextSprite(pos + new Vector2(59, 9)*scale, bsize2, "Monospace", 0.3f*scale, Colors[OPT], R_align);
    AddTextSprite(pos + new Vector2(7, 74)*scale, bsize2, "Monospace", 0.3f*scale, Colors[OPT], L_align);
    AddTextSprite(pos + new Vector2(59, 74)*scale, bsize1, "Monospace", 0.3f*scale, Colors[OPT], R_align);
  }
  if (!opt_chc[14]) AddTextSprite(numberpos, drawpercent, "Debug", 0.7f*scale, Colors[PER], C_align, backg);
}

void AddFrameSprite(Vector2 pos, Vector2 size, bool fat, Color color) {
  if (fat) {
    AddBoxSprite(pos, size,11.52f,col_cal(color,4));
    pos.X+=2.88f; pos.Y+=2.88f;
    size.X-=5.76f;size.Y-=5.76f;
    AddBoxSprite(pos, size,2.88f,col_cal(color,2));
  } else {
    AddBoxSprite(pos, size,8.64f,col_cal(color,4));
  }
  pos.X+=2.88f; pos.Y+=2.88f;
  size.X-=5.76f;size.Y-=5.76f;
  AddBoxSprite(pos, size,2.88f,color);
}

void AddBoxSprite(Vector2 pos, Vector2 size, float c, Color color) {
  float x=pos.X, y=pos.Y, a=size.X, b=size.Y, d=x+(a/2), e=c/2, f=y+(b/2);
  AddTextureSprite("SquareSimple", new Vector2(d,y+e), new Vector2(a,c), color, C_align);
  AddTextureSprite("SquareSimple", new Vector2(x+e,f), new Vector2(c,b), color, C_align);
  AddTextureSprite("SquareSimple", new Vector2(x+a-e,f), new Vector2(c,b), color, C_align);
  AddTextureSprite("SquareSimple", new Vector2(d,y+b-e), new Vector2(a,c), color, C_align);
}

void AddTextureSprite(string picture, Vector2 pos, Vector2 size, Color color, TextAlignment alignment) {
  frame.Add(new MySprite(SpriteType.TEXTURE,picture,pos,size,color,"",alignment,0));
}

void AddTextSprite(Vector2 pos, string str, string font, float size, Color color, TextAlignment alignment, float bg = 0) {
  if (bg>0) {
    var t_vec=surface.MeasureStringInPixels(new StringBuilder(str), font, size)*new Vector2(1,0.7f);
    AddTextureSprite("SquareSimple", pos+new Vector2(0,t_vec.Y*.8f), t_vec, Color.Black.Alpha(bg), alignment);
  }
  frame.Add(new MySprite(SpriteType.TEXT,str,pos,nulvec,color,font,alignment,size));
}

void PrepareSurface(IMyTerminalBlock block, int i = 0) {
  var provider = block as IMyTextSurfaceProvider;
  if (i >= provider.SurfaceCount) i = 0;
  surface = provider.GetSurface(i);
  pos = surface.SurfaceSize;
  viewport = new RectangleF((surface.TextureSize - pos) / 2, pos);
  float x=0,y=0,w=0,h=0;
  switch (block.BlockDefinition.SubtypeName+i) {
    case "AtmBlock0": x=8; y=0; w=16; h=2; break;
    case "MedicalStation0": x=21; y=5; w=41; h=10; break;
    case "LabEquipment0": x=12; y=20; w=23; h=29; break;
    case "StoreBlock1": x=9; y=2; w=13; h=3; break;
    default: break;
  }
  viewport.X+=x;
  viewport.Y+=y;
  viewport.Width-=w;
  viewport.Height-=h;
  surface.ContentType = ContentType.SCRIPT;
  surface.Script = "";
  surface.ScriptBackgroundColor = new Color(0, 0, 0);
  frame = surface.DrawFrame();
  AddTextureSprite("SquareSimple", viewport.Center, viewport.Size*1.2f, Color.Black, C_align);
  AddTextSprite(new Vector2(viewport.Center.X,-500), "", "Monospace", 100, Color.Black, C_align);
}

void GeneratePatterns() {
  getpattern("OOBJOIDBIOBDUOBDOOJBOUDB" , ref sym_Ice, 1);
  getpattern("OIFBOMHBOQHBOUFBOOBH" , ref sym_Windmill, 1);
  getpattern("OIBDIIBBUIBBKKBBSKBBIODBOOBBUODBKSBBOUBDSSBBIUBBUUBB" , ref sym_Solar, 1);
  getpattern("OGFBOPFCOQJBOWJBKHBCSHBCIKBDUKBDONBEKOBDSOBDGTBEWTBE" , ref sym_Grinder, 1);
  getpattern("OIFBOUFBMKBDQKBDKOBBSOBBIRBCURBC" , ref sym_Refinery, 1);
  getpattern("OIHBOMHBOQHBOUHB" , ref sym_Vent, 1);
  getpattern("RGGBVHCCKIBBSKBBLMGBUSBBRUCBLWGBWLBGILBCGRBGQRBG" , ref sym_Block, 1);
  getpattern("OGDBKIDBSIDBHKCBVKCBOUDDIOBBUOBBKQBBSQBBMJBEQJBEGLBCWLBCOSBF" , ref sym_Para, 1);
  getpattern("OIHBJMCBTMCBMOBBQOBBOQBBOUHBIOBHUOBH" , ref sym_Sorter, 1);
  getpattern("IHBGUHBGHJEEVJEEHICFVICFGLFCWLFCHKGBVKGBOOBBOWDFOXFEOYHB" , ref sym_Reactor);
  getpattern("OGFBOLFCOOBFOSDBLUCBRUCBOWDB" , ref sym_Engine, 1);
  getpattern("OFEDOICELMBCRMBCJPBDTPBDOXIBHUBEVUBE" , ref sym_Thruster, 3);
  getpattern("OJCEJIBBTIBBOKEBIRCCURCCHTBEVTBEOSGBOWIB" , ref sym_Collector, 2);
  getpattern("QDCBMFCBIHCBOJIBONEBOTEBOXIBHPBJLQBEVQBI" , ref sym_Cargo, 3);
  getpattern("OGIBOMGBHIBDVIBDJLBCTLBCOQGBOWIBJRBCTRBCHUBDVUBD" , ref sym_Connector, 2);
  getpattern("OFCBOJEBONGBORIBOVKB" , ref sym_Drill, 3);
  getpattern("LDBDRDBDOHCBDLDBOLCBZLDBHOBCLOBCROBCVOBCDRDBORCBZRDBOVCBLZBDRZBD" , ref sym_Jumpdrive, 3);
  getpattern("OFIBOJEBONEBOREBKVCBSVCBOXEBHMBIVMBILLBDRPBD" , ref sym_Shield, 3);
  getpattern("NLHBNRHBHOBELOBEVOBC" , ref sym_Bullet, 3);
  getpattern("IHCBKJCBOLIBORIBKTCBIVCBHOBEXOBC" , ref sym_Rocket, 3);
  getpattern("LGBBRGBBJLBETLBEHSBFVSBFOOIBOWIB" , ref sym_Welder, 2);
  getpattern("LJBCRJBCOOCFLTBCRTBC" , ref sym_Assembler, 2);
  getpattern("OHIBOLGBOPEBOTGBOXEBLVBDRVBD" , ref sym_Projector, 3);
  getpattern("OGIBLOBBOWIBHOBJVOBJ" , ref sym_Door, 2);
  getpattern("KIEBPKBBRMBBTOBBOTICHOBHIQCFPSBDVSBD" , ref sym_Cockpit, 2);
  getpattern("OHGBIJCBUJCBOOEEXODEOZEBHOBGJUBCLXBDVOBGTUBCRXBD" , ref sym_Hinge, 3);
  getpattern("OHIBOIGCOKEEONIBJVDBTVDBOXGBHRBFLPBJRPBJVRBFAAAA" , ref sym_Rotor, 3);
  getpattern("OFGBOYECLJBFRJBFJSBGOTCHTSBG" , ref sym_Piston, 3);
  getpattern("PEPCmDIDCgCasgCaX8XC" , ref patternBattery);
  getpattern("EEECXDFBqEECXFPBCKCEsKCEDgBSrgBSBgBGtgBGC2CEs2CEE8ECX7PBq8ECX9FB" , ref patternCargo);
  getpattern("XEVCBLBHtLBHDgBargBaB0BIt0BIX8VC" , ref patternTank);
  getpattern("XEXCBgBatgBaDTBHrTBHDfBBrfBBDsBIrsBIX8XC" , ref patternConnector);
  getpattern("XEXCDYBSrYBSBMBCtMBCBUBCtUBCBcBCtcBCBkBCtkBCCyCIsyCIX8XC" , ref patternTool);
  getpattern("XDXBBgBcXFLBtgBcDgBSrgBSX7LBX9XB" , ref patternJumpdrive);
  getpattern("XEXCBgBatgBaDMBCrMBCDgBOrgBOD0BCr0BCX8XC" , ref patternGasgen);
  getpattern("XEVCCICEsICEFHBBpHBBCQCCsQCCCWCCsWCCCgCGsgCGCqCCsqCCCwCCswCCC4CEs4CEF5BBp5BBX8VC" , ref patternShield);
  getpattern("XEXCBgBatgBaDgBSrgBSX8XC" , ref patternTurret);
  getpattern("XEVCBIBEtIBEDVBPrVBPCxCNsxCNI8CCO8CCU8CCa8CCg8CCm8CC" , ref patternThruster);
  getpattern("JEJCXDFBlEJCCQCKsQCKBfBFtfBFCvCLsvCLJ8JCl8JCX9FB" , ref patternReactor);
  getpattern("XEXCDgBargBaX8XC" , ref patternSolar);
  getpattern("XEXCBKBEtKBEDRBFrRBFBZBFtZBFDhBFrhBFBpBFtpBFDxBFrxBFB3BDt3BDX8XC" , ref patternWindmill);
  getpattern("XETCEGCCqGCCCgCasgCaE6CCq6CCX8TC" , ref patternNone);
  getpattern("XEVCBTBPtTBPDpBNrpBNBoBCtoBCBwBCtwBCC6CEs6CEX8TC" , ref patternRefinery);
  getpattern("HEFCXDLBnEFCBIBEtIBEDXBRrXBRBrBPtrBPDrBBrrBBDvBBrvBBDzBBrzBBD3BBr3BBX8XC" , ref patternAssembler);
  getpattern("LEJCjEJCCQCMsQCMFHBBpHBBCvCNsvCNF5BBp5BBL8JCj8JC" , ref patternProjector);
  getpattern("LEJCjEJCBQBMXFDBtQBMDgBargBaBvBNtvBNL8JCX7DBj8JC" , ref patternVent);
}

void getpattern(string pat, ref Vector2[] matr, int brack=0) {
  int j=0;
  switch (brack) {
    case 1: pat+="FCEBXCEBFaEBXaEBCFBEaFBECXBEaXBE"; break;
    case 2: pat+="ECEBYCEBEaEBYaEBBFBEbFBEBXBEbXBE"; break;
    case 3: pat+="EBEBYBEBEbEBYbEBBEBEbEBEBYBEbYBE"; break;
  }
  matr = new Vector2[pat.Length/2];
  for (int i = 0; i < pat.Length;) {
    matr[j++]=getvec(pat[i++],pat[i++])*1.44f;
    matr[j++]=getvec(pat[i++],pat[i++])*2.88f;
  }
}

Vector2 getvec(char a, char b) {return new Vector2(b64dec(a),b64dec(b));}

int b64dec(char chr) {return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".IndexOf(chr);}

bool getVector(string option, string tagstart, out Vector2 tempvector, out float gfx_res) {
  tempvector = nulvec;
  gfx_res = 0.1f;
  var xyz = getBetween(option, TL(tagstart), ")").Split(',').ToArray();
  if (xyz.Length == 2) {
    return float.TryParse(xyz[0], out tempvector.X) && float.TryParse(xyz[1], out tempvector.Y);
  }
  else if (xyz.Length == 3) {
    return float.TryParse(xyz[0], out tempvector.X) && float.TryParse(xyz[1], out tempvector.Y) && float.TryParse(xyz[2], out gfx_res);
  }
  else return false;
}

bool getcolors(string option, string tagstart, out Color tempColor) {
  bool valid = true;
  int[] col =new int[4];
  col[3] = 31;
  tempColor = new Color(255, 0, 0, 0);
  var xyz = getBetween(TL(option), TL(tagstart) + "(", ")").Split(',').ToArray();
  if (xyz.Length != 3) return false;
  for(int i=0;i<3;i++) {
    col[3] *=2;
    if (int.TryParse(xyz[i], out col[i])) {
      col[3] +=1;
      continue;
    }
    string temp = xyz[i];
    if (temp[0]=='-') col[i]=12;
    else if (temp[0]!='+') return false;
    if (temp.Length<2) return false;
    switch(temp[1]) {
      case 'r': col[i]+=0; break;
      case 'g': col[i]+=1; break;
      case 'y': col[i]+=2; break;
      case 'v': col[i]+=3; break;
      case 'q': col[i]+=4; break;
      case 'f': col[i]+=5; break;
      case 'x': col[i]+=6; break;
      case 'u': col[i]+=7; break;
      case 's': col[i]+=8; break;
      case 'h': col[i]+=9; break;
      case 'z': col[i]+=10;break;
      case 'w': col[i]+=11;break;
      default: valid = false; break;
    }
  }
  if (valid) tempColor = new Color(col[0],col[1],col[2],col[3]);
  return valid;
}

Color var_col (float prct) {return new Color(prct*2, 2-(prct*2), 0);}

void mod_col(ref Color col) {
  if (col.A == 255) return;
  int[] comp = new int[4];
  comp[0] = col.R;
  comp[1] = col.G;
  comp[2] = col.B;
  comp[3] = 255-col.A;
  for (int i=0;i<3;i++) {
    comp[3]*=2;
    if (comp[3]<8||comp[i]>23) continue;
    var temp = comp[i];
    switch (temp%4) {
      case 0: comp[i] = Colors[ICO].R; break;
      case 1: comp[i] = Colors[ICO].G; break;
      case 2: comp[i] = (Colors[ICO].R+Colors[ICO].G)-255; break;
      case 3: comp[i] = Colors[ICO].B; break;
    }
    if ((temp%12)>=8) comp[i]+=256;
    if ((temp%12)>=4) comp[i]/=2;
    if (temp>=12) comp[i]=255-comp[i];
    comp[3]-=8;
  }
  col = new Color(comp[0],comp[1],comp[2],255);
}

Color col_cal (Color in_col, int fact) {return new Color(in_col.R/fact,in_col.G/fact,in_col.B/fact);}

void setdefault() {
  ClearArray(opt_chc);
  LayoutCount = 0;
  temp_txt = TL(getBetween((Me.CustomData + "\n"), Tag[Def_Tag], "\n")) + " ";
  nosubgridLCDs = have(temp_txt,Tag[NoSubgridLCDTag]);
  verbatimdefault = have(temp_txt,Tag[VerbatimTag]);
  opt_def = wordparse(opt_chc,temp_txt);
  nostatus = have(temp_txt,Tag[NoStatusTag]);
  textposdefault = get_align(temp_txt,L_align);
  SsD = intparse(temp_txt, Tag[scrlspdTag], 0, SsD);
  FpM = intparse(temp_txt, Tag[FpM_Tag], 0, FpM);
  LpM = intparse(temp_txt, Tag[LpM_Tag], 0, LpM);
  if (have(temp_txt,Tag[Fast_Tag])) {
    TpM = 600;
    speed = UpdateFrequency.Update10;
  } else {
    TpM = 60;
    speed = UpdateFrequency.Update100;
  }
  if (temp_txt.Contains(Tag[Sequence_Tag])) {
    sArgs = getBetween(temp_txt, Tag[Sequence_Tag],")").Split(new[] {' ',',','.',':','/'}, StringSplitOptions.RemoveEmptyEntries);
    Layouts = new int[sArgs.Length];
    for (int i = 0; i < sArgs.Length;i++) {
      if (int.TryParse(sArgs[i], out Layouts[LayoutCount])) LayoutCount++;
    }
  }
  Runtime.UpdateFrequency = speed;
  oldCustomData = Me.CustomData;
}

bool[] wordparse(bool[] inv,string text) {
  int len = inv.Length;
  bool[] res = new bool[len];
  for(int i=0;i<len;i++) {
    res[i]=have(text,Tag[tag_sort[i]]) ^ inv[i];
  }
  return res;
}

bool getsprite (string allvars, out MySprite sprite, out float sprite_end) {
  bool success = false;
  string tmpname = "";
  if (allvars[0]=='"') {
    tmpname = getBetween(allvars,"\"","\"").Trim();
    var nameEnd = (allvars+"  ").IndexOf('"',allvars.IndexOf('"')+1);
    if (nameEnd>tmpname.Length) allvars = "@ " + allvars.Substring(nameEnd+1); else allvars = "";
  }
  string[] sprvals = allvars.Split(new char[] {' ',',',':','/','(',')'}, StringSplitOptions.RemoveEmptyEntries);
  Vector2 tmppos = nulvec,tmpsize = nulvec;
  Color sprt_col = new Color(255,255,255);
  float tmprot=0;
  if (sprvals.Length > 7 && sprvals.Length < 10) {
    success = true;
    tmpname = sprvals[0].Replace("@",tmpname);
    success &= float.TryParse(sprvals[1], out tmppos.X);
    success &= float.TryParse(sprvals[2], out tmppos.Y);
    success &= float.TryParse(sprvals[3], out tmpsize.X);
    success &= float.TryParse(sprvals[4], out tmpsize.Y);
    success &= getcolors(("x("+sprvals[5]+","+sprvals[6]+","+sprvals[7]+")"), "x", out sprt_col);
    if (sprvals.Length == 9) success &= float.TryParse(sprvals[8], out tmprot); else tmprot=0;
  }
  tmppos+= viewport.Position;
  mod_col(ref sprt_col);
  sprite = new MySprite() {Type = SpriteType.TEXTURE,Data = tmpname,Position = (tmppos+(tmpsize/2)),Size = tmpsize,Color = sprt_col,Alignment = C_align,RotationOrScale = tmprot};
  sprite_end = tmppos.Y + tmpsize.Y;
  return success;
}

void layoutswitch(int change) {
  if (LayoutCount>1) {
    execCounter1 = 0;
    execCounter2 = 0;
    LayoutIndex += change + LayoutCount;
    LayoutIndex %= LayoutCount;
    Layout = Layouts[LayoutIndex];
  }
}

static string teil(string f_txt, int strt, int len=-1) {
  if ((strt<0)||(strt>=f_txt.Length)||(len==0)) return "";
  if (len<0) return f_txt.Substring(strt);
  else return f_txt.Substring(strt,len);
}

TextAlignment get_align(string text,TextAlignment align) {
  if (have(text,Tag[AlignTag00])) align = L_align;
  else if (have(text,Tag[AlignTag01])) align = C_align;
  else if (have(text,Tag[AlignTag02])) align = R_align;
  return align;
}

String TL(string tag) {
  return tag.ToLower();
}

public class EnergyShield {
  public static float CurrentHitpoints(IMyTerminalBlock block) {
    float val;
    float.TryParse(getBetween(block.CustomName, " (", "/"), out val);
    return val;
  }
  public static float MaxHitpoints(IMyTerminalBlock block) {
    float val;
    float.TryParse(getBetween(block.CustomName, "/", ")"), out val);
    return val;
  }
  public static string RequiredInput(IMyTerminalBlock block) {
    return getBetween(block.DetailedInfo, "\nRequired Input: ", "\n");
  }
}

int highload = 10000; // defining reserved command cycles
int execCounter1 = 0,execCounter2 = 0,rotatecount = 0,containertype,lastDraw,Layout = 0,LayoutCount = 0,LayoutIndex = 0,TpM = 60,screenIndex,iconcount,iconcounter,headlineIndex,barcount,sumBlock, scrl_spd = 10, startdisp=0 ,dispnr=0, scrollindex, cs, ce, clone, bar_type;
int[] Layouts, tag_sort;
int[,] scrollpositions = new int[100,20];
string[] sArgs,lines,sections;
string argument, sym2, sym3, drawpercent, opt_txt1, opt_txt2, Displayname, oldCustomData = "", eShieldInput = "", added_txt = "", temp_txt, StartTag, StartSep, StopSep, bsize1, bsize2, pnl_tag, clone_font, filter, warnings, rotator="|/-\\";
StringBuilder gfxSprite = new StringBuilder(), headline = new StringBuilder(), clone_txt = new StringBuilder();
bool[] opt_def = new bool[16], opt_chc = new bool[16], paints = new bool[6];
bool get_sprite, invertColor, verbatim, verbatimdefault, nosubgridLCDs, nostatus, verti, horiz, timeout, perm_flag;
TextAlignment textpos = L_align, textposdefault = L_align;
Color integrityLevelColor;
Color[] Colors = new Color[7];
Vector2[] pattern, sym1, sym_Assembler, sym_Block, sym_Bullet, sym_Cargo, sym_Collector, sym_Connector, sym_Cockpit, sym_Door, sym_Drill, sym_Engine, sym_Grinder, sym_Hinge, sym_Ice, sym_Jumpdrive, sym_Para, sym_Piston, sym_Projector, sym_Reactor, sym_Refinery, sym_Rocket, sym_Rotor, sym_Shield, sym_Solar, sym_Sorter, sym_Thruster, sym_Vent, sym_Welder, sym_Windmill, patternBattery, patternCargo, patternTank, patternConnector, patternTool, patternJumpdrive, patternGasgen, patternShield, patternTurret, patternThruster, patternReactor, patternSolar, patternWindmill, patternNone, patternRefinery, patternAssembler, patternProjector, patternVent;
float[] sumBat = new float[3], sumJdrives = new float[3], sumCargo = new float[3], sumCockpit = new float[3], sumPara = new float[3], sumSorter = new float[3], sumHydro = new float[3], sumOxy = new float[3], sumDoor = new float[2], sumVent = new float[2], sumCollectors = new float[3], sumConnectors = new float[3], sumDrills = new float[3], sumGrinder = new float[3], sumWelder = new float[3], sumGasgen = new float[3], sumShield = new float[3], sumGatling = new float[3], sumMissileTurret = new float[3], sumGatlingTurret = new float[3], sumMissileLauncher = new float[3], sumThrust = new float[3], sumReactor = new float[3], sumSolar = new float[3], sumWindmill = new float[3], sumO2Gen = new float[3], sumRefinery = new float[3], sumAssembler = new float[3], sumProjector = new float[3], sumEShield = new float[2], sumPiston = new float[2], sumHinge = new float[2], sumRotor = new float[2], sumAll = new float[2] ,sumPwr = new float[2], sumChg = new float[2], sumCar = new float[2], sumVol = new float[2], sumTht = new float[2];
float last_fsize = 0, fontsize = 0, rad=2*(float)Math.PI, backg, rest, scale, lastscale = 1, lastLength, scrollrange =0, set_length, min_length, std_length, reference, percent, permanent;
HashSet<IMyCubeGrid> ignoredGrids = new HashSet<IMyCubeGrid>();
List<IMyTerminalBlock> surfaceProviders = new List<IMyTerminalBlock>(), Group = new List<IMyTerminalBlock>(), subgroup = new List<IMyTerminalBlock>();
List<MyInventoryItem> items = new List<MyInventoryItem>();
IMyTextSurface surface;
RectangleF viewport;
UpdateFrequency speed = UpdateFrequency.Update100;
MySpriteDrawFrame frame = new MySpriteDrawFrame();
Vector2 chargeBarSize, chargeBarOffset, restBarOffset, restBarSize, sym_pos, optionpos, numberpos, LineStartpos, pos, tempvector, chargebarpos, fieldsize, fieldpos, nulvec = new Vector2(0,0), posHeadline, gfxpos;

const TextAlignment L_align = TextAlignment.LEFT;
const TextAlignment C_align = TextAlignment.CENTER;
const TextAlignment R_align = TextAlignment.RIGHT;

const int
lcdTag = 0,
EndTag = 1,
Panel_Tag = 2,
VerbatimTag = 3,
Opt_Tag05 = 4,
Opt_Tag06 = 5,
Opt_Tag07 = 6,
WidebarTag = 7,
SmallbarTag = 8,
TallbarTag = 9,
ShortbarTag = 10,
SingleIconTag = 11,
NoIconTag = 12,
CountTag = 13,
Opt_Tag08 = 14,
Opt_Tag11 = 15,
Opt_Tag12 = 16,
Opt_Tag02 = 17,
Opt_Tag03 = 18,
Opt_Tag01 = 19,
AltInfoTag = 20,
AltSymTag = 21,
Opt_Tag14 = 22,
Col_sym = 23,
Col_ico = 29,
GapTag = 30,
PositionTag = 31,
FSizeTag = 32,
ScaleTag = 33,
LengthTag = 34,
Bar_Type00 = 34,
Bar_Type01 = 35,
Bar_Type02 = 36,
Bar_Type03 = 37,
Opt_Tag15 = 38,
Opt_Tag00 = 39,
FilterTag = 40,
ReferenceTag = 41,
Opt_Tag13 = 42,
SpriteTag = 43,
gfxStart = 44,
gfxEnd = 45,
CloneTag = 46,
textTag = 47,
AlignTag00 = 48,
AlignTag01 = 49,
AlignTag02 = 50,
Opt_Tag04 = 51,
Opt_Tag09 = 52,
Opt_Tag10 = 53,
scrlspdTag = 54,
ValueTag = 55,
ConditionTag = 56,
Def_Tag = 57,
NoSubgridLCDTag = 58,
NoStatusTag = 59,
FpM_Tag = 60,
LpM_Tag = 61,
Layout_Tag = 62,
Sequence_Tag = 63,
Fast_Tag = 64,

SYM = 0,
TXT = 1,
PER = 2,
FRM = 3,
BAR = 4,
OPT = 5,
ICO = 6,

nothing = 0,
smallbar = 1,
widebar = 2,
shortbar = 3,
tallbar = 4,
singleicon = 5,
multiicon = 6,
textline = 7,

normal = 0,
digital = 1,
analog = 2,
solid = 3;
