/****************

 Easy Play ver 1.33 - SE gameplay automation script by Survival Ready
 https://steamcommunity.com/profiles/76561199069720721/myworkshopfiles/
 
*/

string language = ""; // localization

bool debug = false;     // debug mode
string conLCD = "";     // concole LCD block name

string igcTag  = "EasyPlay"; // IGC channel name
string igcData = "";
string igcQue  = "";
IMyBroadcastListener igcListener;

double scanRange = 15000; // default scan range for Raycast (m)
int scanSize = 0;         // scan targen size precision

double invrescan = 5;   // sec. rescan inventory during request
bool invempty = true;   // show 0 items amount in group[*] list
bool invforce = false;  // force rescan after blocks inventory
DateTime invscan = DateTime.Now;

List<string> cStore;    // current Storage
string cBlock = "";     // current @block name from storage
string rBlock = "";     // request @block name Main(arg) || @block command
int cLine = 0;          // num current line from storage

int tCount = 0;         // token count in current line
string tLast = "";      // last token in current line
string tShot = "";      // used for short name Props & Actions
int sBlock = 0;         // search blocks count
int tOf = 0;            // token "of"

string vBlock = "";     // var block name 
List<string> varInt = new List<string>{}; // vars in @Vars block
List<string> varExt = new List<string>{}; // vars in request block(...)

int fPrec = 4;          // float precision
char[] ssp = new char[]{' '};
string sepRen = "#";
string sepMath = "()+-/*";
string numbers = "0123456789";
string sepLogic = "=|!=|==|<|<<|>|>>|?|!?|??";
string surToken  = "fontsize|fontface|textalign|fontcolor|padding|bgcolor|bgrotate|image|text|script|surface";
string sysToken  = "if|else|when|of|at|to|on";
static Random random = new Random();

int delayTime = 0;
bool delayReset = true;
double delayWhen = 0;
TimeSpan delayMsec;

string oldArgs = "";
string oldCode = "";

Dictionary<string,Dictionary<int,List<string>>> code = new Dictionary<string,Dictionary<int,List<string>>>();
Dictionary<string,double[]> invent = new Dictionary<string,double[]>();
Dictionary<string,string> bcode = new Dictionary<string,string>();
Dictionary<string,string> lng = new Dictionary<string,string>();
Dictionary<int,string> sys = new Dictionary<int,string>() 
{
{100, "en" },
{101, "Properties" },
{102, "Actions" },
{103, "Block" },
{104, "Grid" },
{105, "Name" },
{106, "Small" },
{107, "Large" },
{108, "Ship" },
{301, "Block #1 must be a Controller for property #2" },
{302, "Controller #1 does not have a #2 value" },
{303, "No active Remote Control block of grid" },
{304, "The LCD #1 does not exist." },
{305, "Failed to run Programm Block #1(#2)" },
{306, "The Programm Block #1 does not exist" },
{307, "No active radio antennas for SEND" },
{308, "You can't run itself" },
{309, "No data to write" },
{310, "Can't execute command MOVE" },
{311, "Console LCD not found" },
{312, "Group name #1 not found" },
{313, "Block name #1 not found" },
{314, "Block filter #1 is empty" },
{315, "There aren't active controllers"},
{316, "Can't execute command ROTATE" },
{317, "Block #1 has no action #2" },
{318, "Block #1 has no property #2" },
{319, "Unknow command for #1"},
{320, "Unknow data format #1"},
{321, "Group condition not available for Cond()"},
{322, "Block #1 is not a #2"},
{401, "Unbalansed () brackets in #1"},
{402, "Unbalansed {} brackets"},
{403, "Unclosed \" quotes" },
{404, "No code in CustomData" },
{405, "The code @block #1 not found" },
{406, "The code @block #1 is empty" },
{407, "Incorrect use of the separator TO" },
{408, "Code block name #1 has spaces or spec chars" },
{409, "Enter the @block name to execute" },
{410, "Arguments must be in (round brackets)" },
{411, "Empty block name" },
{412, "Attempt devide by Zero in Math()" },
{413, "#1 is not a variable or parameter" },
{414, "Variables block not found" },
{415, "Var #1 is wrong name" },
{416, "Misuse IF/WHEN/ELSE" },
{417, "Brace \"{\" should be used after IF" },
{418, "Variable #1 not set in vars block" },
{419, "Bad operand #1 found in Math()" },
{420, "Incorrect RUN command" },
{421, "Incorrect init of a variable" },
{422, "Can't turn #1 at #2 with speed #3" },
{423, "Code block #1 already exists" },
{424, "Can't move #1 at #2 with speed #3" },
{425, "Wrong Cond() at #1" },
};

bool checkSource(string raw)
{
  if(raw.IndexOf("\\\"") != -1) raw = raw.Replace("\\\"","¤");
  if((raw.Count(f => f=='"') % 2) > 0) return Console(403);
  
  var data = new List<string>(raw.Split('"'));
  var tstr = new List<string>(); vBlock = "";
  
  int q = 0, i = 0, j = 0; string line = "", bn = "", bb = ""; raw = "";

  for(i=0; i < data.Count; ++i) {
    if((i % 2) > 0) {
      tstr.Add(data[i].Replace("\n","¶").Replace("\\N","¶")); data[i] = "§"+(q++)+"§";
    }
    raw += data[i];
  }

  raw += "\n@#{\n}\n"; data = new List<string>(raw.Split('\n')); q = 0;

  for(i=0; i < data.Count; ++i)
  {
     line = data[i].Trim(); if(line == "" || line[0] == '*') continue; 
     
     bool bo = (line[line.Length-1] == '{');
     bool bc = (line[0] == '}'); 
     q += (bo?1:0) - (bc?1:0);
     
     if(line[0] == '@' && bo) {
       if(bn != "") {
         if(vBlock == "" && bn.ToLower().StartsWith("var")) vBlock = bn;
         bb = bb.Substring(0,bb.Length-2);
         bb = multiLine("math",bb);
         bb = multiLine("cond",bb);
         
         while(bb.IndexOf("§"+j+"§") != -1) {
           bb = bb.Replace("§"+j+"§","\""+tstr[j]+"\""); 
           if(++j > tstr.Count) break;
         }
         bcode.Add(bn,bb); bb = "";
       }
       bn = line.Substring(1).Trim('{').Trim(); 
       if(findChar(bn," "+sepMath+sepRen) && bn != "#") 
         return Console(408,bn); continue;

     } else if(line.Length > 1) {
       if(bc) line = "}\n"+line.Substring(1).Trim();
       if(bo) line = line.Trim('{')+"\n{";
     }
     bb += line+"\n";
  }
  if(q != 0) return Console(402); return true;
}

string multiLine(string s, string t) 
{
  int q = 0; string l = "";
  
  while((q = t.IndexOf(s,q,StringComparison.OrdinalIgnoreCase)) > 0) {
    int i = t.IndexOf(")\n",q); 
    if(i > 0) {
      l = t.Substring(q, i-q+1);
      if(l.Replace("(","").Length != l.Replace(")","").Length) { 
        Console(401,l.Replace("\n"," ")); return "";
      } t = t.Replace(l,l.Replace("\n"," "));
    } else break; q = i;
  }
  return t;
}  

bool Localization() 
{
  IMyTextSurface sur; sur = (IMyTextSurface)Me.GetSurface(1);
  StringBuilder b = new StringBuilder(); sur.ReadText(b);
  
  lng = new Dictionary<string, string>(); bool isy = true;
  
  string[] data = b.ToString().Trim('\n').Trim().Split('\n');
  if(data.Count() == 0 || data[0] != "[sys]") return false;

  for(int i=1; i < data.Count(); ++i) 
  {
    if(!data[i].StartsWith("[")) {
      string[] kv = data[i].Split('=');
      
      if(kv.Count() == 2) {
        if(isy) {
          int k = int.Parse(kv[0].Trim());
          if(k > 0){ if(sys.ContainsKey(k)) sys[k] = kv[1].Trim(); }
        } else {  
          lng[kv[0].Trim()] = kv[1].Trim();
        }  
      }  
    } else isy = false;
  }
  return true;
}  

void Reset() {
  bcode.Clear(); code.Clear(); varInt.Clear(); varExt.Clear(); oldCode = ""; oldArgs = ""; igcQue = "";
}  

void Stop() { 
  Storage = ""; delayReset = true; 
}

public Program()
{
  igcListener=IGC.RegisterBroadcastListener(igcTag);
  igcListener.SetMessageCallback(igcTag); 
}

public void Main (string argument, UpdateType updateSource)
{
  int i = 0; cLine = 0; cBlock = "";  sBlock = 0;

  if((updateSource & UpdateType.IGC) > 0) 
  { 
    while(igcListener.HasPendingMessage) {
      MyIGCMessage mes = igcListener.AcceptMessage();
      if(igcData != "") igcQue += igcData+"§"; else igcQue = "";
      if(mes.Tag == igcTag) igcData = mes.Data.ToString();
    }
    if((i = igcData.IndexOf("@")) != -1)
      argument = igcData.Substring(i+1);
    else return;
  }
  
  if(argument.Length > 0)
  {
    if(argument.ToUpper() == "RESET") {
      Stop(); Reset(); Console(0,""); return;
    // try run other @block during execiting
    } else if(Storage != "") return;
    if(debug) Console(0,"");
  }
  
  if(sys[100] != language) Localization();
    
  string newCode = Me.CustomData; if(newCode.Trim() == "") { Console(404); Reset(); return; }

  if(code.Count == 0 || oldCode.Length != newCode.Length || !string.Equals(oldCode, newCode, StringComparison.Ordinal)) 
  {
    DateTime sT = DateTime.Now;
    
    if(!checkSource(newCode)) { Reset(); return; } if(!codeDictionary()) { Reset(); return; } oldCode = Me.CustomData;

    // vBlock set in checkSource
    if(vBlock != "") {
      List<string> vlist = new List<string>{}; varInt = new List<string>{};

      for(i = 0; i < code[vBlock].Count; i++) {
        varInt.Add(code[vBlock][i][0]); vlist.Add(code[vBlock][i][0]);
      }
      
      vlist.Sort((x,y) => x.Length.CompareTo(y.Length));
      while(vlist.Count > 0) {
        string b = vlist[0]; vlist.RemoveAt(0);
        var n = vlist.FirstOrDefault(s => s.StartsWith(b));
        if(n != null || isWord(b,sysToken)) { 
          Console(415,n==null?b:$"{b}\" part of \"{n}"); Reset(); return; 
        }
      }  
    }
    if(debug) Console(0,$"Recompile[{this.Runtime.CurrentInstructionCount}] {((TimeSpan)(DateTime.Now - sT)).TotalMilliseconds:n1} ms\n"); 
  }

  if(!parseArguments(argument)) return; 
  
  if(!code.ContainsKey(rBlock)) { Console(405,rBlock); Reset(); return; }
  cStore = new List<string>(Storage.Split('§')); cLine = int.Parse(cStore[1]);
  cBlock = cStore[0]; if((i = cBlock.IndexOf("(")) != -1) cBlock = cBlock.Substring(0,i);
  try { tCount = code[rBlock][cLine-1].Count; } catch { Console(406,cBlock); Reset(); return; }
  tShot = code[rBlock][cLine-1][0]; tLast = code[rBlock][cLine-1][tCount-1];

  string cmd = tShot.ToLower(); tOf = hasToken("of");

  if( cmd == "{" || cmd == "}") { Next(); }
  else if (cmd[0] == '@' || cmd == "exec") { HandleExec(); }
  else if (isWord(cmd,"stop|end") && tLast == "") { Stop(); }
  else if (cmd == "if") { IfWhen("if"); }
  else if (cmd == "when") { IfWhen("when"); }
  else if (cmd == "else") { Next(braceCode()+1); }
  else if (cmd == "delay") { HandleDelay(); }
  else if (cmd == "value") { HandleValue(); }
  else if (cmd == "rename") { HandleRename(); }
  else if (cmd == "move") { HandleMove(); }
  else if (cmd == "send") { HandleIGC(); }
  else if (cmd == "fly") { HandleFly(); }
  else if (cmd == "set") { HandleSet(); }
  else if (cmd.StartsWith("show")) { HandleShow(cmd); }
  else if (cmd.StartsWith("run")) { HandleRun(cmd); }
  else if (cmd.StartsWith("scan")) { HandleScan(cmd); }
  else if (cmd.StartsWith("data")) { HandleCustomData(cmd.Substring(4)); }
  else if (cmd.StartsWith("token")) { HandleToken("",tShot); }
  else if (cmd.StartsWith("write")) { HandleWrite(cmd.Substring(5)); }
  else if (isWord(cmd, "rotate|shortrotate")) { HandleRotate(cmd); }
  else if ((tOf > 0) && (hasToken("=") > tOf)) { HandleProperty(cmd); }
  else if (tCount > 1) { HandleActions(cmd); }
  else { Next(); }
}

bool parseArguments(string arg)
{
  if(Storage.Length == 0 && arg.Length == 0) { return false; }

  if(Storage.IndexOf('§') == -1) Storage = (arg + "§1");
  arg = Storage.Substring(0,Storage.IndexOf('§')); int i = 0;

  if(arg.Length != 0 && oldArgs != arg)
  {
    if(arg.Count(f => f=='(') != arg.Count(f => f==')')) return Console(410);
    
    if((i = arg.IndexOf('(')) == -1) {
      rBlock = arg.Trim(); varExt = new List<string>();
    } else {
      rBlock = arg.Substring(0, i).Trim();
      varExt = new List<string>(arg.Substring(i+1).Trim(')').Split(','));
      for(i=0; i < varExt.Count; i++) { varExt[i] = varExt[i].Trim(); }
    }
    oldArgs = arg;
  }
  return true;
}

public bool codeDictionary()
{
  code.Clear(); string block = "";

  // string tokens include "¤" = \" (slashed quote) and "¶" = \n (new line)
  foreach (KeyValuePair<string, string> x in bcode)
  {
    var lineDict = new Dictionary<int, List<string>>{};
    
    int pos = 0, dictKey = 0, err = 0; block = x.Key; 
    
    if(code.ContainsKey(block)) return Console(423,"@"+block); 

    while(pos < x.Value.Length-1)
    {
      int startPos = pos; int endPos = x.Value.IndexOf('\n', pos);
      string line = x.Value.Substring(startPos, endPos - startPos).Trim();
      var write = new List<string>(); int i = line.IndexOf('"'); string tstr = "";

      if(i != -1) {
        tstr = line.Substring(i);
        write = new List<string>(line.Substring(0,i-1).Split(ssp,StringSplitOptions.RemoveEmptyEntries)); write.Add(tstr);
      } else {
        write = new List<string>(line.Split(ssp, StringSplitOptions.RemoveEmptyEntries));
      }

      tstr = write[0].ToUpper();

      if(tstr[0] == '@' || tstr == "EXEC") {
        write.Clear(); 
        if(tstr == "EXEC") { line = line.Substring(4).Trim(); write.Add("exec"); }
        write.Add(line);
        
      } else if(tstr[0] == '{') {

        if(!isWord(lineDict[dictKey-1][0],"IF|ELSE")) { err = 417; break; }

      // --- block @vars
      } else if(isWord(block,vBlock)) {
        
        if(write.Count < 3 || write[1] != "=") { err = 418; break; }
        tstr = line.Substring(line.IndexOf("=")+1).Trim();
        write.RemoveRange(2,write.Count-2); varParse(tstr,ref write);

      // --- inner commands
      } else if(isWord(tstr,"ECHO|CONSOLE")) {
        write.Insert(0,"to"); write.Insert(0,"Write"); 
        
      } else if(tstr == "VALUE" || tstr[0] == '\\') {

        if(tstr[0] == '\\') {
          write[0] = write[0].Substring(1); 
          write.Insert(0,"of"); write.Insert(0,"Value"); 
        }
          
        if((i = hasToken("=",write)) == 3 && write.Count() > 3) {
          tstr = line.Substring(line.IndexOf("=")+1).Trim();
          write.RemoveRange(++i,write.Count-i); write.Add(tstr);
        }

      } else if(tstr == "SHOW") {

        if(write.Count > 1) {
          write.RemoveRange(2,write.Count-2);
          write[1] = line.Substring(write[0].Length).Trim();
        }

      } else if(isWord(write[0],"RUN|RUNALL|RUNWITHDEFAULTARGUMENT")) {

        int sp = line.IndexOf(" "); i = hasToken("=",write);
        if(sp < 0) { err = 420; break; }

        tstr = (isWord(write[0],"RUNALL")?"RunAll":"Run");
        write.Clear(); write.Add(tstr);

        if(i > 0) {
          i = line.IndexOf("=");
          tstr = line.Substring(i+1).Trim();
          write.Add(line.Substring(sp,i-sp).Trim());
          write.Add("="); write.Add(tstr.Trim('"'));

        } else if((i = line.IndexOf("(")) > 0) {
          tstr = line.Substring(i+1).Trim();
          write.Add(line.Substring(sp,i-sp).Trim());
          write.Add("="); write.Add(tstr.Trim(')'));

        } else {
          write.Add(line.Substring(sp).Trim());
          if(sp > 4) { write.Add("="); write.Add("defarg"); }
        }

      // --- parse logic
      } else if(isWord(write[0],"IF|WHEN|ELSE")) {

        if(write.Count > 1)
        {
          string sep = sepLogic + "|if|else|when", s = ""; 
          var t = write.ToList(); write.Clear();

          for(int j=0; j<t.Count; ++j)
          {
            if(isWord(t[j],sep)) {
              if(s != ""){ write.Add(s.Trim()); }
              write.Add(t[j].ToLower().Trim()); s = "";

            } else if(t[j].ToUpper() == "OF") {
              if(s != "") write.Add(s.Trim()); write.Add("of"); s = "";

            } else if(t[j][0] == '\\' && j < (t.Count-1)) {
              write.Add("Value"); write.Add("of"); 
              write.Add(t[j].Substring(1));
               
            } else {
              s += t[j]+" ";
            }
          }
          if(s != "") write.Add(s.Trim().Trim('"'));
          
          if(!isWord(write[write.Count-2],sepLogic)) { err = 416; break; }

        } else {
          write[0] = write[0].ToLower();
        }

        
      // --- set property or "at" action for multiwords action name
      } else if((hasToken("OF",write) > 0) || (hasToken("TO",write) > 0) || (hasToken("AT",write) > 0)) {
      
        string sep = "of|to|at|=|rotate|shortrotate|rename|move", s = "";
        var t = write.ToList(); write.Clear();

        for(int j=0; j<t.Count; ++j)
        {
          if(isWord(t[j],sep)) {
            if(s != "") write.Add(s.Trim());
            write.Add(t[j].Trim().ToLower()); s = "";
          } else {
            s += t[j]+" ";
          }
        }
        if(s != "") write.Add(s.Trim().Trim('"'));

        if((i = hasToken("AT",write)) != -1 && !isWord(write[0],"rotate|shortrotate|move|fly")) write.RemoveAt(i);

      // --- action
      } else if(tstr != "}") {
        var t = write.ToList(); string s = ""; write.Clear();
        for(int j=0; j<t.Count; ++j) { if(j == 0 || t[j] == "=") write.Add(t[j]); else s += t[j]+" "; }
        write.Add(s.Trim());
      }

      lineDict.Add(dictKey++, write); pos = endPos + 1;

    } // while

    if(err > 0) { cBlock = block; cLine = 0; return Console(err); }

    code.Add(block, lineDict);
  } // while

  bcode.Clear(); return true;
}

int braceCode(string pair = "{}")
{
  int i = cLine+1, c = 1; string s = pair[0].ToString(), e = pair[1].ToString();

  while(i < code[cBlock].Count) {
    if(code[cBlock][i][0] == s) c++; else if(code[cBlock][i][0] == e) c--; if(c == 0) return (i+1); ++i;
  }
  return 0;
}

void Next(int line = 0, bool cascad = false)
{   
  if(line == 0) line = cLine + 1; if(debug && !cascad) outDebug();

  if(line > code[cBlock].Count) cLine = code[cBlock].Count;

  string s = "";

  if(cLine < code[cBlock].Count) {
    s = $"{cStore[0]}§{line}";
    for(int i=2; i < cStore.Count; i++) s += $"§{cStore[i]}";
    Storage = s; Main("",UpdateType.Update1);
  } else {
    if(cStore.Count > 2) {
      s = $"{cStore[2]}";
      for(int i=3; i < cStore.Count; i++) s += $"§{cStore[i]}";
      Storage = s; Main("",UpdateType.Update1);
    } else {
      Storage = ""; if(igcQue == "") return;
      int i = igcQue.IndexOf("§"); igcData = igcQue.Substring(0,i);
      igcQue = igcQue.Substring(i+1); i = igcData.IndexOf("@");
      
      if(i != -1) { 
        Main(igcData.Substring(i+1),UpdateType.Update1);
      }
    }
  }
}

void IfWhen(string oper)
{
  string tstr;
  string ops = codeToken(-2);
  string get = codeToken(-5);
  string item = codeToken(-3);
  bool iswhen = (oper == "when");
  
  string reqVal = varInsert(tLast);
  var actVals = new List<string>{};

  if(isWord(get,"NUMBER")) {
    var blocks = new List<IMyTerminalBlock>();
    blockSearch(item, ref blocks); actVals.Add(blocks.Count.ToString());

  } else if(isWord(get,"VALUE")) {
    if((tstr = varInsert("\\"+item)) == item) { Console(413,item); return; }
    actVals.Add(tstr);

  } else {
    actVals = GetSignals(item, get);
  }
  
  if(isWord(reqVal,"TRUE")) {
    reqVal = "True";
  } else if(isWord(reqVal,"FALSE")) {
    reqVal = "False";
  }
  reqVal = valPrefix(reqVal);
  
  float reqf, actf = 0; 
  int ret = 0, i = reqVal.IndexOf("."), p = 0; 
  bool isf = float.TryParse(reqVal, out reqf);
  
  if(isf && i != -1) p = reqVal.Substring(i).Length-1;
  
  reqf = valParseFloat(reqVal,p); 
  
  for(i = 0; i < actVals.Count; i++)
  {
    actVals[i] = actVals[i].Trim();

    actf = valParseFloat(actVals[i],p);

    switch(ops) {
      case "=":
      case "==":
        ret += (isf ? (actf == reqf) : actVals[i] == reqVal) ? 1:0; break;
      case "!=":
        ret += (isf ? (actf != reqf) : actVals[i] != reqVal) ? 1:0; break;
      case "<":
      case "<<":
        ret += (actf < reqf) ? 1:0; break;
      case ">":
      case ">>":
        ret += (actf > reqf) ? 1:0; break;
      case "!?":
        ret += (actVals[i].ToLower().Contains(reqVal.ToLower())) ? 0:1; break;
      case "?":
      case "??":
        ret += (actVals[i].ToLower().Contains(reqVal.ToLower())) ? 1:0; break;
    }
    if((ops == "!=" || ops.Length == 1) && ret > 0) { ret = actVals.Count; break; }
  }
  
  if((actVals.Count > 0) && (ret == actVals.Count)) {
    
    if(iswhen) {
      delayWhen = TimeSpan.Parse(Convert.ToString(delayMsec)).TotalSeconds;; 
      if(debug) Console(0, $"@{cBlock} : When ({sBlock}) {delayWhen:n2} sec\n"); 
      delayReset = true; delayMsec = new System.TimeSpan(); 
    } 
    Next();

  } else if(iswhen) {
    
    if(delayReset) {
      delayReset = false; delayMsec = new System.TimeSpan(0);
    } else {
      delayMsec += Runtime.TimeSinceLastRun;
    }
    if(debug) Echo($"@{cBlock} When\n{delayMsec} sec");  
    if(!Me.Enabled){ Stop(); return; } Cycle();
    
  } else if(oper == "if") {
    IfCascade();
  }
}

void IfCascade()
{
  int endOfIf = braceCode();

  if(code[cBlock].Count == endOfIf) {
     if(debug) outDebug(); Next(endOfIf, true);

  } else if(isWord(code[cBlock][endOfIf][0], "ELSE")) {

    if(code[cBlock][endOfIf].Count > 1 && isWord(code[cBlock][endOfIf][1], "IF")) {
      if(debug) outDebug(); cLine = endOfIf + 1; tLast = codeToken(-1); IfWhen("if");
    } else {
      if(debug){ outDebug(); Console(0,$"@{cBlock} : else\n"); } Next(endOfIf+3, true);
    }

  } else {
    if(debug) outDebug(); Next(endOfIf+1, true);
  }
}

void blockSearch(string name, ref List<IMyTerminalBlock> sel, int err4 = 0)
{
  sBlock = 0; if(name == "") { Console(411); return; } 
  
  var list = new List<IMyTerminalBlock>(); name = varInsert(name);
  char n0 = name[0]; bool sn = ("?#!".IndexOf(n0) != -1);
  int i = 0, err = (sn?314:313); if(err4 > 0) err = err4;

  // "on MyShip/MyGrid/AllGrid" as part on block/group name
  string grid = "myship", flt = ""; if(sn) name = name.Substring(1);
  string[] w = name.ToLower().Split(ssp,StringSplitOptions.RemoveEmptyEntries);
    
  if((i = Array.IndexOf(w,"on")) != -1) {
    w = String.Join(" ",w,0,i).Split(ssp); 
    i = name.IndexOf(" on ", StringComparison.OrdinalIgnoreCase);
    grid = name.Substring(i+4).ToLower().Trim(); name = name.Substring(0,i).Trim();
  }
    
  if(n0 == '(' && name[name.Length-1] == ')') {
    
    name = name.TrimStart('(').TrimEnd(')');
    IMyBlockGroup glist = GridTerminalSystem.GetBlockGroupWithName(name);
    if(glist == null) { Console(312,name); return; } glist.GetBlocks(list);

    if(grid != "myship") {
      foreach(var b in list) {
        if(grid == "mygrid") {
          if(b.CubeGrid == Me.CubeGrid) sel.Add(b);
        } else if(grid == "allgrid") {  
          if(b.IsSameConstructAs(Me)) sel.Add(b);
        }
      }  
    } else{
      sel = list;
    }  
    
  } else {
    
    flt = (sn && n0 == '?' ? w[0] : name);
    
    if(grid == "mygrid") {
      GridTerminalSystem.SearchBlocksOfName(flt, list, b => b.CubeGrid == Me.CubeGrid);
    } else if(grid == "allgrid") {  
      GridTerminalSystem.SearchBlocksOfName(flt, list, b => b.IsSameConstructAs(Me));
    } else if(grid != "myship") {  
      GridTerminalSystem.SearchBlocksOfName(flt, list, b => b.CubeGrid.CustomName.ToLower() == grid);
    } else {  
      GridTerminalSystem.SearchBlocksOfName(flt, list);
    }
      
    foreach(var b in list)
    {
      bool add = false; i = 0;

      switch(n0) {
      case '?': // contain words
        string[] c = b.CustomName.ToLower().Split(ssp,StringSplitOptions.RemoveEmptyEntries);
        
        for(int j=0; j < w.Count(); ++j) {
          for(int k=0; k < c.Count(); ++k) {
            if(c[k].StartsWith(w[i])) { ++i; break; }
          }
        }
        add = (i == w.Count()); break;
      case '#': // start from exact
        if(b.CustomName.StartsWith(name)) add = true; break;
      case '!': // exact name
        if(b.CustomName == name) add = true; break;
      default:
         add = true; break;
      }
      if(add) sel.Add(b);
    }
  }
  
  if((sBlock = sel.Count) == 0) Console(err,(!sn?"":$"{n0}")+name);
}

string blockStatus(IMyTerminalBlock block, bool raw = false)
{
  if(!block.IsWorking) return (block.IsFunctional?"Offline":"Damage"); string s = "";
  
  if(block is IMyLandingGear) {
    return Convert.ToString((block as IMyLandingGear).LockMode);

  } else if(block is IMyShipConnector) {
    IMyShipConnector con = block as IMyShipConnector; s = Convert.ToString(con.Status);
    if(s == "Connected") {
      con = con.OtherConnector; if(!raw) s += $" ({con.CubeGrid.CustomName})";
    }    
    return s;
  } else if(block is IMyAssembler) {
    var b = block as IMyAssembler;
    s = (b.IsProducing ? "Producing" : "Idle");
    if(s == "Idle" && !b.IsQueueEmpty) s = "Stopped";
    if(!raw) s += " ("+b.Mode+")"; return s;
    
  } else if(block is IMyRefinery) {
    return ((block as IMyRefinery).IsProducing ? "Producing" : "Idle");
    
  } else if(block is IMyMotorStator) {
    return ((block as IMyMotorStator).IsAttached ? "Attached" : "Detached");
    
  } else if(block is IMyPistonBase) {
    return Convert.ToString((block as IMyPistonBase).Status);

  } else if(block is IMyDoor) {
    return Convert.ToString((block as IMyDoor).Status);

  } else if(block is IMyProjector) {
    var b = block as IMyProjector; 
    var l = new List<IMyTerminalBlock>(); GridTerminalSystem.GetBlocks(l);
    int t = b.TotalBlocks, f = l.Count(), rb = b.RemainingBlocks, ra = b.RemainingArmorBlocks, rc = 0;
    
    var blocks = b.RemainingBlocksPerType;
    foreach (var item in blocks) {
      s = item.ToString().Trim('[').Trim(']'); string[] info = s.Split('/')[1].Split(',');
      if(info[0].IndexOf("Armor") > 0){ ++ra; --rb; } else if(info[0].IndexOf("Conveyor") > 0){ ++rc; ++ra; }
    }
    s = Convert.ToString(Math.Round((float)rb/(float)t*100,1));
    
    // 1=Blueprint:2=Total:3=Functional:4=Can weld:5=Remain Non-Armor:6=Remain Conveyor:7=Remain Armor:8=% damage
    if(!raw) s = $"{b.IsProjecting}:{t}:{f}:{b.BuildableBlocksCount}:{rb-ra}:{rc}:{ra}:{s}"; return s; 

  } else if(block is IMyJumpDrive) {
    var b = block as IMyJumpDrive;
    s = Math.Round(b.JumpDistanceMeters/1000,0).ToString();
    if(!raw) s += $" km ({b.Status})"; return s;
    
  } else if(block is IMyAirVent) {
    return Convert.ToString((block as IMyAirVent).Status);
    
  } else if(block is IMyLaserAntenna) {
    return Convert.ToString((block as IMyLaserAntenna).Status);

  } else if(block is IMyBatteryBlock) {
    var b = block as IMyBatteryBlock; 
    s = Math.Round(b.CurrentStoredPower/b.MaxStoredPower*100).ToString();
    if(!raw) s += " ("+b.ChargeMode+")"; return s;

  } else if(block is IMyConveyorSorter) {
    return Convert.ToString((block as IMyConveyorSorter).Mode);

  } else if(block is IMyShipMergeBlock) {
    return (block as IMyShipMergeBlock).IsConnected ? "Merged" : "Idle";
    
  } else if(block is IMySensorBlock) {
    var b = block as IMySensorBlock; 
    var e = new List<MyDetectedEntityInfo>(); b.DetectedEntities(e);
    return (e.Count() > 0 ? e[0].Name : "Idle");
    
  } else if(block is IMySolarPanel) {
    var b = block as IMySolarPanel; s = Math.Round(b.CurrentOutput*1000).ToString();
    if(!raw) s += " ("+Math.Round(b.CurrentOutput/b.MaxOutput*100,0).ToString()+"%)";
    return s;

  } else if("MyWind".IndexOf(block.GetType().ToString()) > 0) {
    var b = block as IMyPowerProducer; s = Math.Round(b.CurrentOutput*1000).ToString();
    if(!raw) s += " ("+Math.Round(b.CurrentOutput/b.MaxOutput*100,0).ToString()+"%)";
    return s;
    
  } else if(block is IMyTimerBlock) {
    var b = block as IMyTimerBlock; if(b.IsCountingDown) return "Countdown";
    
  } else if(block is IMyGasTank) {
    var b = block as IMyGasTank; s = $"{Math.Round(b.FilledRatio*100,1)}";
    if(!raw) s += " (Stock"+(b.Stockpile ? "On":"Off")+")"; return s;

  } else if(block is IMyTurretControlBlock) {
    var b = block as IMyTurretControlBlock;
    return (b.HasTarget ? (b.IsAimed ? "Aimed":"Target") : "Idle");
    
  } else if(block is IMyLargeTurretBase) {
    var b = block as IMyLargeTurretBase;
    s = (b.HasTarget ? (b.IsAimed ? "Aimed":"Target") : "Idle");

    if(!raw) {
      var items = new List<MyInventoryItem>();
      b.GetInventory(0).GetItems(items); double am = 0;
    
      foreach(var i in items) {
        if(i.Amount > 0) am += Math.Round((float)i.Amount,0);
      }
      s += $" ({am})";
    }
    return s;
    
  } else if(block is IMyUserControllableGun) {
    var b = block as IMyUserControllableGun;
    s = (b.IsWorking ? "Ready" : "Idle");

    if(!raw) {
      var items = new List<MyInventoryItem>();
      b.GetInventory(0).GetItems(items); double am = 0;
    
      foreach(var i in items) {
        if(i.Amount > 0) am += Math.Round((float)i.Amount,0);
      }
      s += $" ({am})";
    }
    return s;
  }
  
  return "Active";
}

string HandleToken(string name = "", string tok = "") 
{
  char s = (tok.ToLower().IndexOf("line") > 0?'¶':':'); 
  bool set = (name == ""); if(set) name = codeToken(tOf+1);
  if(!varInt.Contains(name)) { Console(418,name); return ""; }
  
  string v = varInsert("\\"+name);
  if(s == '¶' && v.IndexOf(s) == -1) s = '\n'; string[] t = v.Split(s); 
  int i = tok.IndexOf("["); if(i == -1) return (v == ""?"0":$"{t.Count()}");
  
  tok = varInsert(getWord(tok,"[]")); i = tok.IndexOf('"'); tok = tok.Trim(); 
  float f; bool num = (float.TryParse(tok, out f) && i == -1);
  
  if(!num) {
    i = Array.IndexOf(t,tok.Trim('"').Trim()); if(!set) return $"{i+1}";
  } else {  
    i = valParseInt(tok); if(i > t.Count() || i == 0) i = 1; 
  }
  
  if(set) {
    var list = new List<string>{}; 
    list.Add(name); list.Add("="); varParse(varInsert(tLast), ref list, true);
    t[i-1] = list[list.Count()-1]; list[list.Count()-1] = string.Join(s+"",t);
    code[vBlock][varInt.IndexOf(name)] = list; Next();
  } 
  return t[i-1];
}

List<string> GetSignals(string block, string prop, bool slist = false)
{
  // empty signals for error block name
  var signals = new List<string>{}; if(block.Trim() == "") return signals;

  string low = prop.ToLower(); bool grp = (block.IndexOf("(") != -1); int i = 0, sur = 0; 

  if(block.ToLower() == "script") {  
    signals.Add(sysProperty(varInsert(low))); return signals;

  } else if(low.StartsWith("token")) {
    signals.Add(HandleToken(block, prop)); return signals;    
    
  } else if(low.StartsWith("inv")) {
    signals.Add(shipInventory(block, prop)); return signals;    
    
  } if(isWord(block,"myship|mygrid|allgrid")) {
    signals.Add(shipProperty(prop,"",block)); return signals;
  }  
  
  var list = new List<IMyTerminalBlock>{}; string type = ""; int items = 0; 

  if(isWord(prop,surToken)) {
    sur = valParseInt(block,"[]"); if(block.IndexOf("["+sur+"]")!=-1) block = block.Replace("["+sur+"]","");
  }

  blockSearch(block, ref list); items = list.Count;
  
  if(items == 0){ signals.Add(low == "number" ? "0" : ""); return signals; }
   
  if(low == "number") { 
    signals.Add(items.ToString()); return signals;

  } else if(low.StartsWith("data")) {
    signals.Add(low == "data" ? list[0].CustomData:varCustomData(list[i],prop)); return signals;
    
  } else if((list[0] is IMyShipController) && !(list[0] is IMyCryoChamber) && !isWord(prop,surToken) && !grp) {
    signals.Add(shipProperty(prop, block)); return signals;
  }

  for(i=0; i < items; i++)
  {
    string signal = ""; bool empty = false;

    if(low.StartsWith("status")) {
      signal = blockStatus(list[i], low != "status");
      
    } else if(low == "angle" && list[i] is IMyMotorStator) {
      signal = Convert.ToString(Math.Round(((list[i] as IMyMotorStator).Angle * 180 / Math.PI) % 360,fPrec));

    } else if(low == "position" && list[i] is IMyPistonBase) {
      signal = Convert.ToString((list[i] as IMyPistonBase).CurrentPosition);

    } else if(low == "sound" && list[i] is IMySoundBlock) {
      signal = (list[i] as IMySoundBlock).SelectedSound; empty = true;
      
    } else if(low == "jump" && list[i] is IMyJumpDrive) {
      signal = Convert.ToString((list[i] as IMyJumpDrive).JumpDistanceMeters);
      
    } else if(low == "ai") {
      signal = aiPattern(list[i]);
      
    } else if(isWord(low, surToken)) {

      IMyTextSurface surf = getSurface(list[i],sur);

      switch(low) {
        case "fontsize":
          signal = Convert.ToString(surf.FontSize); break;
        case "textalign":
          signal = Convert.ToString(surf.Alignment); break;
        case "fontcolor":
          if(Convert.ToString(surf.ContentType) == "SCRIPT")
            signal = colorStr(surf.ScriptForegroundColor);
          else  
            signal = colorStr(surf.FontColor); 
          break;
        case "bgcolor":
          if(Convert.ToString(surf.ContentType) == "SCRIPT")
            signal = colorStr(surf.ScriptBackgroundColor);
          else
            signal = colorStr(surf.BackgroundColor); 
          break;
        case "fontface":
          signal = surf.Font; break;
        case "padding":
          signal = Convert.ToString(surf.TextPadding); break;
        case "bgrotate":
          signal = Convert.ToString(list[i].GetValue<float>("Rotate")); break;
        case "image":
          signal = surf.CurrentlyShownImage; break;
        case "text":
          StringBuilder b = new StringBuilder(); 
          surf.ReadText(b); signal = b.ToString();
          break;
        case "script":
          signal = surf.Script; break;
        case "surface":
          signal = surfaceShape(surf); break;
      }
      if(signal == null) signal = ""; empty = true;

    } else {
      signal = blockProp(list[i], prop, ref type); 
      if(signal == "") signal = blockInfo(list[i], prop);
    }

    if(signal == "" && !empty) Console(318,$"{list[i].CustomName}|{prop}");

    // translate for status list = "Status Oxygen of MyGrid"
    if(slist && lng.Count > 0) {
      int j = signal.IndexOf("(");
      if(lng.ContainsKey(signal)) {
        signal = lng[signal];
      } else if(j > 0) {
        string k = signal.Substring(0,j-1).Trim(); 
        if(lng.ContainsKey(k)) signal = signal.Replace(k,lng[k]);
      }  
    }
    if(slist) signal = $"{list[i].CustomName}: {signal}:{list[i].EntityId}"; 
    signals.Add(signal);
  }
  
  return signals;
}

List<string> varValue(string varName)
{
  var values = new List<string>{};
  
  if(vBlock != "") {
    int v = varInt.IndexOf(varName), i = (code[vBlock][v].Count-1); 
      
    if(i == 4 && hasToken("of",code[vBlock][v]) > 0) {
      values = GetSignals(code[vBlock][v][4], code[vBlock][v][2]);
    } else {
      bool c = code[vBlock][v][i] == ")";
      
      if(c && hasToken("math",code[vBlock][v]) == 2)
        values.Add(mathCalc(code[vBlock][v]));

      else if(c && hasToken("cond",code[vBlock][v]) == 2)
        values.Add(condCalc(code[vBlock][v]));
      
      else 
        values.Add(code[vBlock][v][2]);
    } 
  } else {
    values.Add(""); Console(414);
  }
  
  return values;
}

string condCalc(List<string> cond)
{
  var _cond = cond.ToList(); int i = 0; bool ret = true; string yes = "", no = "", log = "&"; 

  while(i < _cond.Count && (i = _cond.IndexOf("of",i)) > -1)
  {
    if(_cond[i-1].ToLower() == "value") { 
      Console(425,"value"); return "";
    } else {
      var sig = GetSignals(_cond[i+1], _cond[i-1]);
      _cond[i] = (sig.Count == 0 ? "\"\"" : sig[0]);
    } 
    _cond.RemoveAt(i+1); _cond.RemoveAt(i-1);
  }

  i = 0; foreach(string s in _cond) { if(s == ":") i++; }
  
  if(i != 2) { Console(425,":"); return ""; }
  
  i = hasToken(":",_cond); yes = _cond[i+1].Trim().Trim('"'); _cond.RemoveRange(i,2);
  i = hasToken(":",_cond); no = _cond[i+1].Trim().Trim('"'); _cond.RemoveRange(i,2);

  _cond.RemoveRange(0,4); _cond.RemoveAt(_cond.Count-1); i = 0;
  
  while(_cond.Count >= 2) // string in condition include " & ¶ simbols
  {
    float v1=0, v2=0; string ops = _cond[1].Trim(); bool res=true; int p=0;
    
    if(sepLogic.IndexOf(ops) < 0) { Console(425,ops); return ""; }
    if(ops.Length > 1 && ops != "!=") Console(321);
    
    _cond[0] = _cond[0].Trim('"'); bool isf = float.TryParse(_cond[0], out v1);
    _cond[2] = _cond[2].Trim('"'); isf = isf && float.TryParse(_cond[2], out v2);

    if(isf) {
      i = _cond[0].IndexOf("."); if(i != -1) p = _cond[0].Length-i-1;
      i = _cond[2].IndexOf("."); if(i != -1) p = Math.Max(p,_cond[2].Length-i-1); else p = 0;
      v1 = (float)Math.Round(v1,p); v2 = (float)Math.Round(v2,p);
    }
    
    switch(ops) {
      case "=":
        res = (isf ? (v1 == v2) : _cond[0] == _cond[2]); break;
      case "!=":
        res = (isf ? (v1 != v2) : _cond[0] != _cond[2]); break;
      case "<":
        res = (v1 < v2); break;
      case ">":
        res = (v1 > v2); break;
      case "!?":
        res = (!_cond[0].ToLower().Contains(_cond[2].ToLower())); break;
      case "?":
        res = (_cond[0].ToLower().Contains(_cond[2].ToLower())); break;
    }
    if(log == "&") ret = ret && res; else ret = ret || res;

    _cond.RemoveRange(0,3); if(_cond.Count < 4) break; 
    log = _cond[0].Trim(); _cond.RemoveAt(0);

    if(!findChar(log,"&|")) { Console(425,"and/or"); return ""; }
  }
  
  return (ret ? yes : no).Replace('§',':').Replace("¶","\n");
}

string mathCalc(List<string> math)
{
  var _math = math.ToList(); int i = 0; 
  string neg = "", sep = sepMath.Substring(2); 

  while(i < _math.Count && (i = _math.IndexOf("of",i)) > -1)
  {
    var sig = GetSignals(_math[i+1], _math[i-1]);
    _math[i] = (sig.Count == 0 ? "0" : sig[0]);
    _math.RemoveAt(i+1); _math.RemoveAt(i-1);
  }

  _math.RemoveRange(0,3); var list = new List<string>{};

  // search empty & nagtive values && doubled ops
  for(i = 0; i < _math.Count; ++i) {
    if(_math[i] == "") {
      _math[i] = "0";
    } else if(_math[i] == "-") {
      if($"({sep}".IndexOf(_math[i-1]) != -1){ neg = "-"; continue; }
    } else if(sep.IndexOf(_math[i]) != -1 && sep.IndexOf(_math[i-1]) != -1) {  
      continue;
    } else if(neg == "-" && _math[i] == "(") {  
      list.Add(_math[i]); continue;
    } 
    list.Add(neg+_math[i]); neg = "";
  }  
  
  sep = sepMath + numbers + ".";
  Stack<string> ops = new Stack<string>();
  Stack<double> vals = new Stack<double>();
  
  for(i = 0; i < list.Count; ++i)
  {
    if(!isWord(list[i],"abs|int")) {
      foreach(char c in list[i]) {
        if(sep.IndexOf(c) < 0) {
          list[i] = valParseNum(list[i]); break;
        }  
      }
    }
    string s = list[i].ToLower();
    
    if (s.Equals("(")){}
    else if (s.Equals("+")) ops.Push(s);
    else if (s.Equals("-")) ops.Push(s);
    else if (s.Equals("*")) ops.Push(s);
    else if (s.Equals("/")) ops.Push(s);
    else if (s.Equals("abs")) ops.Push(s);
    else if (s.Equals("int")) ops.Push(s);
    else if (s.Equals(")"))
    {
      int count = ops.Count;
      while (count > 0)
      {
        string op = ops.Pop(); double v = vals.Pop();
        
        if (op.Equals("+")) v = vals.Pop() + v;
        else if (op.Equals("-")) v = vals.Pop() - v;
        else if (op.Equals("*")) v = vals.Pop() * v;
        else if (op.Equals("/")) { if(v == 0) { Console(412); return "0"; } v = vals.Pop()/v; }
        else if (op.Equals("abs")) { v = Math.Abs(v); }
        else if (op.Equals("int")) { v = Math.Round(v,0); }
        vals.Push(v); count--;
      }
    } else {
      try { vals.Push(Double.Parse(s)); } catch { Console(419,s); return "0"; }  
    }

  }
  return Math.Round(vals.Pop(),fPrec).ToString();
}

void HandleDelay()
{
  var values = new List<string>{}; values.Add(varInsert(tLast));

  delayTime = valParseInt(values[0]); if(delayTime < 1) delayTime = 1;

  if(delayReset) {
    delayReset = false; delayMsec = new System.TimeSpan(0);
  } else {
    delayMsec += Runtime.TimeSinceLastRun;
  }

  if(delayMsec < TimeSpan.FromMilliseconds(delayTime)) {
    if(debug) Echo($"@{cBlock} Delay\n{delayMsec}"); Cycle(); return;
  }

  if(debug) Console(0, $"@{cBlock} : Delay {Convert.ToString(delayMsec).Substring(0,13)} sec\n");
  delayMsec = new TimeSpan(); delayReset = true; Next();
}

bool HandleExec()
{
  string atb = varInsert(tLast), s = ""; int i = atb.IndexOf("@");
  if(i == -1) return Console(405,atb); atb = atb.Substring(i+1); 
  s = atb; if((i = atb.IndexOf("(")) > 0) s = atb.Substring(0,i).Trim();
  
  if(!code.ContainsKey(s)) return Console(405,s); if(debug) outDebug();
  
  if(cLine < code[cBlock].Count) 
  {
    if(atb != cStore[0]) {
      s = $"{atb}§1§{cStore[0]}§{cLine+1}";
    } else {
      s = $"{cStore[0]}§1";
    }
  } else {
    s = $"{atb}§1";
  }
  
  for(i=2; i < cStore.Count; i++) s += $"§{cStore[i]}";
  Storage = s; Main("", UpdateType.Update1); return true;
}

void HandleShow(string run)
{
  string name = varInsert(tLast), get = run.Substring(4).Trim('[').Trim(']');
  var list = new List<IMyTerminalBlock>{}; blockSearch(name, ref list);
  int i = list.Count; if(i == 0){ Stop(); return; }

  // MyObjectBuilder_ExtendedPistonBase/LargePistonBase
  string def = blockDefine(list[0].EntityId), grid = list[0].CubeGrid.GridSize.ToString();
  string[] show = { $"{sys[103]} \"{list[0].CustomName}\"","","","",""};
         
  show[1] = $"{sys[105]}: {def}\n{sys[104]}: {sys[grid[0] == '0'?106:107]}\n{sys[108]}: {list[0].CubeGrid.CustomName}";

  def = list[0].DetailedInfo; if(def != "") show[2]= $"{def}"; if(get == "") show[3] = $"{sys[101]}\n\n";
  var props = new List<ITerminalProperty>(); list[0].GetProperties(props); 

  for(i = 0; i < props.Count; i++) {
    def = "";
    if(props[i].TypeName == "Boolean") 
      def = (get == "" ? "True/False" : Convert.ToString(list[0].GetValue<Boolean>(props[i].Id)));
    else if(props[i].TypeName == "Single") 
      def = (get == "" ? "Number" : Convert.ToString(list[0].GetValue<float>(props[i].Id)));
    else if(props[i].TypeName == "Color") 
      def = (get == "" ? "Color" : colorStr(list[0].GetValue<Color>(props[i].Id)));
    
    if(def != "") show[3] += $"{props[i].Id} = {def}\n";
  }

  var acts = new List<ITerminalAction>(); list[0].GetActions(acts);
  if(get == "") show[4] = $"{sys[102]}\n\n"; for(i = 0; i < acts.Count; i++) show[4] += acts[i].Id + '\n';

  def = String.Join("\n\n",show); 
  if(get != "") {
    if(get[0] == 'i') def = show[2];
    else if(get[0] == 'p') def = show[3];
    else if(get[0] == 'a') def = show[4];
  }
  
  if(debug) Echo(def); Console(0, def); Next();
}

void HandleScan(string run)
{
  if(delayReset) {
    
    double range; var lidar = new List<IMyCameraBlock>();
    GridTerminalSystem.GetBlocksOfType(lidar, b => b.CustomName == varInsert(codeToken(2)));
    
    if(lidar.Count > 0) {
     
      string o = run.Substring(4).Trim('[').Trim(']');
      if(o != "") o = o.Substring(0,1); bool li = false; 
      
      try { 
        li = lidar[0].EnableRaycast; if(!li) { lidar[0].EnableRaycast = true; } 
      } catch { Console(322,$"{lidar[0].CustomName}|Camera"); Next(); return; }

      try { range = Convert.ToDouble(varInsert(tLast)); } catch { range = scanRange; }
       
      if(!li){ 
        delayTime = lidar[0].TimeUntilScan(range); delayReset = false; 
        delayMsec = new System.TimeSpan(0); Cycle(); return; 
      }
      
      MyDetectedEntityInfo info = lidar[0].Raycast(range);
      
      if(!info.IsEmpty()) { 
        
        run = $"{Math.Round(Vector3D.Distance(info.BoundingBox.Min, info.BoundingBox.Max)/2,scanSize)}";
        
        switch(o) {
         case "e": o = $"{info.EntityId}"; break;
         case "n": o = $"{info.Name}"; break;
         case "r": o = $"{info.Relationship}"; break;
         case "s": o = run; break;
         case "t": o = $"{info.Type}"; break;
         case "v": o = info.Velocity.Length().ToString("0.00"); break;
         default:  o = info.EntityId+":"+info.Name+":"+info.Relationship+":"+
                       run+":"+info.Type+":"+info.Velocity.Length().ToString("0.00"); break;
        }

        if(info.HitPosition.HasValue) {
          run = $"GPS:{o}:{vec2str(info.HitPosition.Value)}:";
        } else {
          run = $"GPS:{o}:{vec2str(info.Position)}:";
        }
      } else run = ""; lidar[0].CustomData = run;
    } else {  
      Console(313,$"{varInsert(codeToken(2))}"); Next(); return;
    }
    
  } else {  

    delayMsec += Runtime.TimeSinceLastRun;

    if(delayMsec < TimeSpan.FromMilliseconds(delayTime)) {
      if(debug) Echo($"@{cBlock} Raycast\n{delayMsec}"); Cycle(); return;
    }

    if(debug) Console(0, $"@{cBlock} : Raycast {Convert.ToString(delayMsec).Substring(0,13)} sec\n");
    delayMsec = new TimeSpan(); delayReset = true; HandleScan(run);
  }
  
  Next();  
}

void HandleFly()
{
  string gps = tLast, name = "EPFly", low; int i = hasToken("AT"); float at = 0; 
  
  if(i > 0) { at = valParseFloat(varInsert(tLast)); gps = codeToken(i-1); }
  
  gps = varInsert(gps.Trim(':')); low = gps.ToLower();

  var cb = new List<IMyRemoteControl>();
  
  if(isWord(codeToken(1),"OF") && !isWord(codeToken(2),"TO")) {
    
    var list = new List<IMyTerminalBlock>();
    blockSearch(codeToken(2), ref list);
    
    for(i=0; i < list.Count; ++i) {
      if(list[i] is IMyRemoteControl) {
        var rem = (IMyRemoteControl)list[i];
        if(rem.IsSameConstructAs(Me) && rem.CanControlShip) {
          cb.Add(rem); break;
        }  
      }  
    }  
  } else {  
    GridTerminalSystem.GetBlocksOfType(cb, b => b.IsSameConstructAs(Me) && b.CanControlShip);
  }
  
  if(cb.Count > 0) 
  {
    if(low == "clear") {
      cb[0].ClearWaypoints();
      
    } else if(low == "start") {
      cb[0].SetAutoPilotEnabled(true);
      
    } else if(low == "stop") {
      cb[0].SetAutoPilotEnabled(false);
      
    } else if(gps.IndexOf(":") > 0) {  
      Vector3D dist = str2vec(gps);
      
      if(dist != Vector3D.Zero) {
        if(low.StartsWith("gps")) {
           name = gps.Split(':')[1];
        }  
        
        cb[0].ClearWaypoints(); 
        cb[0].Direction = 0;
        cb[0].SpeedLimit = (at>100 || at<0?0:at);
        cb[0].FlightMode = FlightMode.OneWay; 
        cb[0].SetCollisionAvoidance(true);
        cb[0].SetDockingMode(true);
        cb[0].AddWaypoint(dist, name);
        
        double d = Math.Round(Vector3D.Distance(cb[0].CenterOfMass,dist)/1000,3);
        tShot = $"Fly to {name} on {d} km";
      } else {
        Console(320,"GPS");
      } 
    } else {
      Console(319,"FLY");
    }
    
  } else {  
    Console(303);
  }
  Next();
}

void HandleIGC() 
{
  string text = varInsert(tLast); int i = 0;

  if(text.ToLower() == "clear") {
    igcData = "";
  } else if(text.ToLower() == "wipe") {
    igcData = ""; igcQue = "";
  } else {
    List<IMyRadioAntenna> ant = new List<IMyRadioAntenna>();
  
    if(isWord(codeToken(1),"TO") && codeToken(2) != "=") 
    {
      var list = new List<IMyTerminalBlock>();
      blockSearch(codeToken(2), ref list);
      
      for(i=0; i < list.Count; ++i) {
        if(list[i] is IMyRadioAntenna) ant.Add(list[i] as IMyRadioAntenna);
      }  
    } else {
      GridTerminalSystem.GetBlocksOfType(ant, b => b.IsSameConstructAs(Me)); 
    }

    if(ant.Count > 0) {
      for(i=0; i < ant.Count; ++i) {
        if(ant[i].Enabled && ant[i].IsBroadcasting) {
          tShot = $"Send to {ant[i].CustomName} \"{text}\" at {ant[i].Radius} m";
          IGC.SendBroadcastMessage(igcTag,text); break;
        }
      }
      if(i == ant.Count) Console(307);
    } else
      Console(307);  
  }  
  Next();
}  

void HandleRun(string run)
{
  var list = new List<IMyTerminalBlock>{};
  string arg = (tCount == 2 ? "" : varInsert(tLast));
  
  blockSearch(codeToken(1), ref list); 

  if(list.Count() > 0) {
    foreach(IMyTerminalBlock b in list) {
      if(b is IMyProgrammableBlock) {
        IMyProgrammableBlock pb = b as IMyProgrammableBlock;
        
        if(pb.CustomName == Me.CustomName) {  
          Console(308); break;
        } else if(arg == "defarg") {
          pb.ApplyAction("RunWithDefaultArgument");
        } else if(!pb.TryRun(arg)) {
          Console(305,$"{codeToken(1)}|{arg}");
        }

      }
      if(run.IndexOf("all") == -1) break;
    }
  } else {  
    Console(306,$"{codeToken(1)}");
  }
  Next();
}

void HandleCustomData(string run) 
{
  if(tCount < 5 || codeToken(1) != "of"){ Console(309); Next(); return; }
  
  var list = new List<IMyTerminalBlock>{}; int i = 0;
  string text = valInsert(varInsert(tLast)), key = ""; 

  if(text.IndexOf('¶') != -1) text = text.TrimStart('¶').Replace("¶","\n");
  if(text.IndexOf('¤') != -1) text = text.Replace("¤","\"");
  if(text.IndexOf("\\N") != -1) text = text.Replace("\\N","\n");
  
  blockSearch(codeToken(2), ref list); 
  
  if(list.Count > 0) 
  {
    if((i = run.IndexOf('[')) == 0) {
      key = varInsert(codeToken(0).Substring(5).Trim(']').Trim());
    }
    
    if(run == "add") {  
      list[0].CustomData += text;
    } else if(run == "line") {  
      list[0].CustomData += (list[0].CustomData == ""?"":"\n")+text;
    } else {
      list[0].CustomData = varCustomData(list[0],key,text);
    }  
  }
  Next();
}  

void HandleMove() 
{
  // Move Piston to 7 at 1
  if(tCount < 6) { Console(310); Stop(); return; }

  float to = valParseFloat(varInsert(codeToken(3)),2);
  float at = valParseFloat(varInsert(codeToken(5)),1);

  var list = new List<IMyTerminalBlock>{};
  var names = new List<string>();

  blockSearch(codeToken(1), ref list); 
  
  if(list.Count > 0) 
  {
    for(int i = 0; i < list.Count; i++)
    {
      if(list[i] is IMyPistonBase) {
        IMyPistonBase piston = list[i] as IMyPistonBase;
        float min = piston.LowestPosition, 
              max = piston.HighestPosition,
              cur = piston.CurrentPosition;
              
        if(to < min || to > max || at > piston.MaxVelocity) {
          Next(); Console(424); return;
        }  
        
        at = Math.Abs(at) * (cur < to ? 1:-1);
        
        if(at < 0) { piston.MaxLimit = max; piston.MinLimit = to; } 
        else { piston.MaxLimit = to; piston.MinLimit = min; }
        piston.Velocity = at;
      }  
    }    
  }
  Next();
}  

void HandleRotate(string run)
{
  // Rotate Rotor to 30 at 10
  if(tCount < 6) { Console(316); Stop(); return; }

  int to = valParseInt(varInsert(codeToken(3)));
  float at = valParseFloat(varInsert(codeToken(5)),0);

  var list = new List<IMyTerminalBlock>{};
  var names = new List<string>();

  blockSearch(codeToken(1), ref list); 
  
  if(list.Count > 0) 
  {
    for(int i = 0; i < list.Count; i++) names.Add(list[i].CustomName);

    var motor = new List<IMyMotorStator>();
    GridTerminalSystem.GetBlocksOfType(motor);

    for(int i = 0; i < motor.Count; i++)
    {
      if(hasToken(motor[i].CustomName,names) != -1)
      {
        bool h = motor[i].BlockDefinition.ToString().ToLower().Contains("hinge");
        int too = Math.Abs(to), ang = 30; float att = at; if(at == 0) break;
        if(motor[i].CubeGrid.GridSize < 1) ang = 60;

        if((h && too > 90) || (!h && too > 360) || Math.Abs(at) > ang) {
          Console(422,$"{codeToken(1)}|{to}|{at}"); return;
        }
        
        if(run == "shortrotate" || h) {
          ang = (int)(motor[i].Angle * 180 / Math.PI) % 360; int rang = ang;

          if(ang < 0) ang += 360; if(to < 0) too = to + 360;
          att = ((too > ang) && (Math.Abs(too-ang) < 180) ? 1:-1) * Math.Abs(att);
          if(att < 0 && rang < 0 && Math.Abs(too-ang) == Math.Abs(ang-to)) att = 0-att;
        }

        if(att > 0) {
          motor[i].LowerLimitDeg = (h ? -90f:-361f);
          motor[i].UpperLimitDeg = to;
        } else if (att < 0){
          motor[i].LowerLimitDeg = to;
          motor[i].UpperLimitDeg = (h ? 90f:361f);
        }
        if(motor[i].RotorLock) motor[i].RotorLock = false;
        motor[i].TargetVelocityRPM = att;
      }
    }
  }
  Next();
}

void HandleRename()
{
  var list = new List<IMyTerminalBlock>{};

  string setTo = varInsert(tLast), block = codeToken(1); 
  
  bool allgrid = isWord(block,"ALLGRID"), 
       mygrid = isWord(block,"MYGRID"), 
       myship = isWord(block,"MYSHIP");
  
  if(allgrid || mygrid || myship) {
    GridTerminalSystem.GetBlocks(list);
  } else {
    blockSearch(varInsert(block), ref list);
  }
  
  if(list.Count == 0){ Next(); return; } int num = 0; 
  
  bool view = (setTo.IndexOf("~") == 0); setTo = setTo.Trim('~').Trim(); 
  if(setTo[0] != '#') setTo="#"+setTo; string[] wild = setTo.Split(sepRen[0]);
  
  for(int i=0; i < list.Count; ++i) 
  {
    if(allgrid && !list[i].IsSameConstructAs(Me)) continue;
    if(mygrid && list[i].CubeGrid != Me.CubeGrid) continue; 
    
    int p = 0; setTo = "";
    
    for(int j=1; j < wild.Count(); ++j) 
    {
      string s = wild[j];
      
      switch(s.Substring(0,1).ToLower()) {
        case "*":
          if((p = wild[j].IndexOf("-")) != -1) {
            s = list[i].CustomName.Replace(wild[j].Substring(p+1).Trim(),"");
          } else {  
            s = s.Replace("*",list[i].CustomName.Trim());
          } 
          break;
          
        case "n":
          p = valParseInt(s.Substring(1));
          if(p > 0){ ++num; s = num.ToString("D"+p); } break;
          
        case "r":
          p = valParseInt(s.Substring(1));
          if(p > 0) s = RandomNum(p); break;
          
        case "d":
          if(isWord(s,"DEF")) {
             s = blockDefine(list[i].EntityId);
          } break; 
      }
      setTo += s.Trim()+(wild[j].LastIndexOf(' ') == (wild[j].Length-1)?" ":"");
    }
    if(view) {
      Echo("Rename:\n"+(myship?Me.CubeGrid.CustomName:list[i].CustomName)+$"\n{setTo}\n");
    } else {  
      if(myship) Me.CubeGrid.CustomName = setTo; else list[i].CustomName = setTo;
    }
    if(myship) break;
  }
  Next();
}

void HandleWrite(string run)
{
  if(tCount < 4 || codeToken(1) != "to"){ Console(309); Next(); return; }
  
  string text = valInsert(varInsert(tLast)); 
  if(tCount == 4 && text == "=") text = "";

  if(text.IndexOf('¶') != -1) text = text.Replace("¶","\n");
  if(text.IndexOf('¤') != -1) text = text.Replace("¤","\"");

  string panel = varInsert(codeToken(2)); int i = 0;

  if(isWord(panel,"ECHO")) {
    Echo(text.Trim('"'));
  } else if(isWord(panel,"CONSOLE")) {   
    Console(0,text.Trim('"')+"\n"); 
  } else {
    int ssur = valParseInt(panel,"[]"); 

    if((i = panel.IndexOf("[")) > 0){ panel = panel.Replace($"[{ssur}]",""); }
    var list = new List<IMyTerminalBlock>{}; blockSearch(panel, ref list);

    if(list.Count > 0) 
    { 
      if((i = run.IndexOf("menu")) != -1) {
        int sel = valParseInt(varInsert(run),"()"); 
        string[] opt = text.Split('\n'); text = "";
        
        for(int j = 0; j < opt.Count(); ++j) {
          text += (j == (sel-1)?">":"  ")+" "+opt[j]+"\n";
        }
        run = run.Substring(0,i);
      } 
      
      for(i = 0; i < list.Count; i++) {
        var sur = getSurface(list[i],ssur);
        try {
          if(run == "script") {
            sur.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
            sur.Script = text; continue;
          } else {
            sur.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
          }  
        } catch {  
          Console(304,$"{list[i].CustomName}"); continue;
        }
        
        if(run == "add") {
          sur.WriteText(text,true);
        } else if(run == "line") {
          sur.WriteText('\n'+text,true);
        } else {
          sur.WriteText(text);
        }
      }
    } else 
      Console(304,$"{panel}");
  }
  Next();
}

void HandleValue()
{
  string v = codeToken(2), vlast = tLast; 
  if(!varInt.Contains(v)) { Console(418,v); return; }
  var list = new List<string>{}; list.Add(v); list.Add("=");
  if(tLast.ToLower().IndexOf("value of") == 0) {
    vlast = varValue(tLast.Substring(8).Trim())[0];
    list.Add(varValue(vlast)[0]);
  } else {
    varParse(varInsert(vlast,tLast.ToLower().StartsWith("cond")), ref list, true);
  }
  code[vBlock][varInt.IndexOf(v)] = list; 
  tShot += $" \\{v}"; Next();
}

string HandleActions(string run)
{
  tShot = tShot.Replace("-"," ");
  
  var list = new List<IMyTerminalBlock>{};
  string block = varInsert(codeToken(1)); 
  int rblock = valParseInt(run,"[]");
  int exact = tShot.IndexOf(" ");
  bool app = false; 
  
  blockSearch(block, ref list);
  
  if(rblock > 0) run = run.Replace($"[{rblock}]","");
  
  if(exact > 0)run = tShot.Substring(0,exact).ToLower();
    
  for(int i=0; i < list.Count; i++)
  {
    var acts = new List<ITerminalAction>{};
    list[i].GetActions(acts);

    for(int j=0; j < acts.Count; ++j) {
       if(acts[j].Id.ToLower().StartsWith(run)) {
         if(exact > 0 & acts[j].Id != tShot) continue;
         tShot = acts[j].Id; app = true;
         list[i].ApplyAction(tShot); break;
       }
    }
    
    if(rblock > 0) { if(--rblock == 0) break; }
  }
  
  if(list.Count < 2 && !app) Console(317,$"{block}|{tShot}");

  Next(); return tShot;
}

void HandleSet () 
{
  int i = tLast.IndexOf(" on ", StringComparison.OrdinalIgnoreCase);
  if(i < 0) { Console(320,"SET"); Next(); return; }

  string bs = varInsert(tLast.Substring(0,i).Trim()), 
         bw = varInsert(tLast.Substring(i+4).Trim());
         
  var set  = new List<IMyTerminalBlock>{};
  var list = new List<IMyTerminalBlock>{};
  var prop = new List<ITerminalProperty>();
  
  blockSearch(bs, ref list); if(list.Count == 0){ Next(); return; }
  
  if(bw.IndexOf("\"") == -1) {
    blockSearch("!"+bw, ref set); if(set.Count == 0){ Next(); return; }

    set[0].GetProperties(prop);
    
    for(i = 0; i < list.Count; i++) {
      if(list[i].BlockDefinition.TypeIdString == set[0].BlockDefinition.TypeIdString) {
        for(int j = 0; j < prop.Count; j++)
        {
          string id = prop[j].Id;
          
          if(prop[j].TypeName == "Single")
            list[i].SetValueFloat(id, set[0].GetValue<float>(id));
          else if(prop[j].TypeName == "Boolean")
            list[i].SetValueBool(id, set[0].GetValue<Boolean>(id));
          else if(prop[j].TypeName == "Color")
            list[i].SetValueColor(id, set[0].GetValue<Color>(id));
          else if(prop[j].TypeName == "Int64")
            list[i].SetValue(id, set[0].GetValue<Int64>(id));
        }
      }
    }
  } else {
    string[] text = bw.Trim('"').Replace("\n","¶").Replace("\\N","¶").Split('¶');
    var pid = new List<string>{}; list[0].GetProperties(prop);
    for(i = 0; i < prop.Count; i++) pid.Add(prop[i].Id);
    
    foreach(string l in text) 
    {
      if(l.Trim() != "") {
        string[] kv = l.Split('='); kv[0] = kv[0].Trim(); 
        int x = pid.IndexOf(kv[0]); bool ai = isStart("ai",kv[0].ToLower());
        if(kv.Length > 1 && !ai && x == -1) { Console(318,$"{bs}|{kv[0]}"); break; }

        for(i = 0; i < list.Count; i++) 
        {
          if(kv.Length > 1) {
            kv[1] = kv[1].Trim();
            try {    
              if(prop[x].TypeName == "Boolean")
                list[i].SetValueBool(kv[0], bool.Parse(kv[1]));
              else if(prop[x].TypeName == "Color")
                list[i].SetValueColor(kv[0], valColor(kv[1]));
              else if(prop[x].TypeName == "Single")
                list[i].SetValueFloat(kv[0], valParseFloat(kv[1]));
              else if(prop[x].TypeName == "Int64")
                list[i].SetValue(kv[0], valParseInt(kv[1]));
            } catch {
              int err = 1; if(ai) err = (aiPattern(list[i],kv[1]) == "" ? 1 : 0);
              if(err > 0){ Console(320,$"SET {kv[0]}={kv[1]}"); Next(); return; }
            }
          } else {  
            try {
              list[i].ApplyAction(kv[0]);
            } catch {
              Console(317,$"{bs}|{kv[0]}"); Next(); return;
            }  
          }
        }
      }
    }
  }
  
  Next();
}

void HandleProperty(string prop)
{
  var setTos = new List<string>{}; setTos.Add(varInsert(tLast)); int sur = 0; 
  string setTo = setTos[0], item = varInsert(codeToken(tOf+1)), type = ""; 
  
  if(isWord(prop,surToken)) {
    sur = valParseInt(item,"[]");
    if(item.IndexOf($"[{sur}]") != -1) item = item.Replace($"[{sur}]","");
  } else if(item.ToLower() == "myship") {
    item = shipProperty(prop,item); // get name of first controller
  }

  var list = new List<IMyTerminalBlock>{}; blockSearch(item, ref list);

  for(int i = 0; i < list.Count; i++)
  {
    if(isWord(prop,"SOUND")) {
      try {
        var b = list[i] as IMySoundBlock; b.SelectedSound = setTo;
      } catch {
        Console(319,list[i].CustomName);
      }
      
    } else if(isWord(prop,"JUMP")) {
      try {
        var b = list[i] as IMyJumpDrive; float d = (valParseFloat(setTo)*1000);
        if(d < b.MinJumpDistanceMeters || d > b.MaxJumpDistanceMeters) 
           d = b.MinJumpDistanceMeters; b.JumpDistanceMeters = d;
      } catch {
        Console(319,list[i].CustomName);
      }

    } else if(isWord(prop,"AI")) {
      if(aiPattern(list[i],setTo) == "") Console(319,list[i].CustomName);
      
    } else if(isWord(prop,surToken)) {
      
      IMyTextSurface surf = getSurface(list[i],sur); ContentType c = surf.ContentType;
      if(Convert.ToString(surf.ContentType) == "NONE") {
        surf.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
      }  

      switch(prop.ToLower()) {
        case "fontsize":
          surf.FontSize = valParseFloat(setTo); break;
        case "textalign":
          if(setTo.ToLower() == "center")
            surf.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
          else if(setTo.ToLower() == "right")
            surf.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.RIGHT;
          else
            surf.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
          break;
        case "fontcolor":
          if(Convert.ToString(surf.ContentType) == "SCRIPT")
            surf.ScriptForegroundColor = valColor(setTo);
          else  
            surf.FontColor = valColor(setTo); 
          break;
        case "bgcolor":
          if(Convert.ToString(surf.ContentType) == "SCRIPT")
            surf.ScriptBackgroundColor = valColor(setTo);
          else   
            surf.BackgroundColor = valColor(setTo); 
          break;
        case "fontface":
          surf.Font = setTo; break;
        case "padding":
          surf.TextPadding = valParseFloat(setTo); break;
        case "bgrotate":
          float r = valParseFloat(setTo);
          list[i].SetValueFloat("Rotate",r-(r % 90)); break;
        case "image":
          surf.ClearImagesFromSelection();
          if(setTo != "") surf.AddImageToSelection(setTo); break;
        case "text":
          surf.WriteText(setTo.Replace("¶","\n")); break;
        case "surface":
          surfaceShape(surf,setTo); break;
        case "script":
          surf.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
          c = surf.ContentType; surf.Script = setTo; break;
      }
      if(c != surf.ContentType) surf.ContentType = c;
      
    // full property name tShot set in blockProp()
    } else if(blockProp(list[i], prop, ref type) != "") {

      if(type == "boolean") {
        try { 
          bool b = bool.Parse(setTo); 
          if(tShot.ToLower() == "handbrake" && list[i] is IMyShipController) {
            var c = (IMyShipController)list[i]; c.HandBrake = b;
          } else {  
            list[i].SetValueBool(tShot, b);
          }
        } catch { Console(320,$"{setTo}"); Next(); return; }
        
      } else if(type == "color") {
        list[i].SetValueColor(tShot, valColor(setTo));
        
      } else {  
        if(isWord(tShot,"OVERRIDE") && setTo.IndexOf("%") > 0) {
          float per = valParseFloat(setTo.Replace("%",""));
          if(list[i] is IMyThrust && per >= 0 && per <= 100) {
            IMyThrust t = (IMyThrust)list[i];
            t.ThrustOverridePercentage = per/100;
          }
        } else {
          list[i].SetValueFloat(tShot, valParseFloat(setTo));
        }
      }

    } else {
      Console(318,$"{codeToken(tOf+1)}|{codeToken(0)}"); Next(); return;
    }
  }
  Next();
}

string sysProperty(string low)
{
  TimeSpan span = DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0));

  if(isStart("sync",low)) {
    int sec = (int)span.TotalSeconds, mod = valParseInt(low,"[]");
    if(mod == 0 && low.IndexOf("[") == -1) return Convert.ToString(delayWhen);
    return mod == 0 ? Convert.ToString(sec):Convert.ToString(sec % mod);

  } else if(isStart("rand",low)) {
    int rnd = valParseInt(low,"[]"); return RandomNum(rnd == 0 ? 3:rnd);
    
  } else if(low == "utc") {
    return Convert.ToString((int)span.TotalSeconds);
    
  } else if(low == "lng") {
    return sys[100];
  }
  return "";  
}

string shipProperty(string prop, string block = "", string grid = "") 
{
  var cont = new List<IMyShipController>();
  GridTerminalSystem.GetBlocksOfType<IMyShipController>(cont, b => b.IsSameConstructAs(Me) && b.CanControlShip);
  if(cont.Count == 0){ Console(315); return ""; }
  
  var term = new List<IMyTerminalBlock>(); string low = prop.ToLower(), type = "";
  
  if(block != "") {
    if(block.ToLower() == "myship") return cont[0].CustomName; IMyShipController cc = null;
    foreach(IMyShipController c in cont){ if(block == c.CustomName) { cc = c; break; } }
    if(cc == null){ Console(301,$"{block}|{prop}"); return ""; } cont.Clear(); cont.Add(cc);
  }

  foreach(IMyShipController c in cont) term.Add(GridTerminalSystem.GetBlockWithId(c.EntityId)); 
  
  if(low == "send") {
    return igcData;
  
  } else if(low.StartsWith("time")) {
     var s = DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString(); 
     int i = prop.IndexOf("["); if(i > 0) s = DateTime.Now.ToString(prop.Substring(i+1)).Trim(']'); 
     return(s);

  } else if(low.StartsWith("dist")) {
      int c = low.IndexOf(" "); if(c != -1) low = low.Substring(c).Trim(); 
      Vector3D dist = str2vec(varInsert(low)); if(dist == Vector3D.Zero) return "0";
      return Math.Round(Vector3D.Distance(cont[0].CenterOfMass,dist),0).ToString();
     
   } else if(isStart("oper",low)) {
     int i = 0; string r = "False";

     if(block != "") {
       r = Convert.ToString(cont[0].IsUnderControl);
     } else {
       for(i=0; i < cont.Count; i++) {
         if(cont[i].IsUnderControl){ r = cont[i].CustomName; break; }
       }
     }
      
     if(low.EndsWith("axis")) {
       r = (cont[i].MoveIndicator.Length() == 0 ? "False" : "True");
     }  
       
     return(r);

  // Status BlockName of Grid
  } else if(low.StartsWith("status")) {
     string[] t = low.Split(ssp); var o = low.Substring(6).Trim('[').Trim(']'); 
     if(t.Count() == 1) return "Active"; // recall from shipController()
     var sig = GetSignals(prop.Substring(t[0].Length).Trim(),t[0],true); 
     if(sig.Count == 1 && sig[0] == "") return ""; // block | group not found
     if(!o.StartsWith(" ")) o = o.Substring(0,1); else sig.Sort((a, b) => a.CompareTo(b));

     float tf = 0, ti = 0; low = ""; grid = grid.ToLower(); 
     
     foreach(string s in sig) {
       float f; t = s.Split(':'); t[1] = t[1].Trim();
       var b = GridTerminalSystem.GetBlockWithId(long.Parse(t[2]));
        
       if(grid == "mygrid" && b.CubeGrid != Me.CubeGrid) continue;
       if(grid == "allgrid" && !b.IsSameConstructAs(Me)) continue;

       if(o == "t" || o == "m") {
         try { f = float.Parse(t[1]); tf+=f; ti+=1f; } catch { }
       } else if(o == "a") {
         ti += 1f; if(t[1] == "Offline" || t[1] == "Damage") tf += 1f;
       } else {  
         low += t[0]+": "+t[1]+"\n"; 
       } 
     }
     
     switch(o) {
       case "t": return Math.Round(tf,fPrec).ToString();
       case "m": return (ti == 0?"0":Math.Round(tf/ti,fPrec).ToString());
       case "a": return $"{ti-tf}/{ti}"; default: return low.Trim('\n');
     }  
    
  } else if(isStart("gravity",low)) {

    if(isStart("nat",low)) {
      Vector3D vec = cont[0].GetNaturalGravity();
      double grav = Math.Sqrt(Math.Pow(vec.X,2)+Math.Pow(vec.Y,2)+Math.Pow(vec.Z,2));
      return(Convert.ToString(vec.Length() == 0 ? 0 : Math.Round(grav/10,2)));

    } else if(isStart("art",low)) {
      Vector3D vec = cont[0].GetArtificialGravity();
      double grav = Math.Sqrt(Math.Pow(vec.X,2)+Math.Pow(vec.Y,2)+Math.Pow(vec.Z,2));
      return(Convert.ToString(vec.Length() == 0 ? 0 : Math.Round(grav/10,2)));

    } else {
      Vector3D vec = cont[0].GetTotalGravity();
      double grav = Math.Sqrt(Math.Pow(vec.X,2)+Math.Pow(vec.Y,2)+Math.Pow(vec.Z,2));
      return(Convert.ToString(vec.Length() == 0 ? 0 : Math.Round(grav/10,2)));
    }

  } else if(isStart("planet",low)) {
    if(cont[0].GetNaturalGravity().Length() == 0) return "-1";
    
    if(isStart("pos",low)) {
      var pos = new Vector3D(); 
      cont[0].TryGetPlanetPosition(out pos);
      return(vec2str(pos));

    } else if(isStart("ele",low)) {
      var elev = new double();
      var planetElev = new MyPlanetElevation();
      cont[0].TryGetPlanetElevation(planetElev, out elev);
      return(Convert.ToString(elev));

    } else if(isStart("alt",low)) {
      var alti = new double();
      cont[0].TryGetPlanetElevation(MyPlanetElevation.Surface, out alti);
      return(Convert.ToString(alti));
    } else {
      return "-1";  
    }

  } else if(isStart("speed",low)) {

      if(isStart("lin",low)) {
        return(Convert.ToString(cont[0].GetShipVelocities().LinearVelocity.Length()));
      } else if(isStart("ang",low)) {
        return(Convert.ToString(cont[0].GetShipVelocities().AngularVelocity.Length()));
      } else {
        return(Convert.ToString(cont[0].GetShipSpeed()));
      }

  } else if(isStart("mass",low)) {

      if(isStart("bas",low)) {
        return(Convert.ToString(cont[0].CalculateShipMass().BaseMass));
      } else if(isStart("phy",low)) {
        return(Convert.ToString(cont[0].CalculateShipMass().PhysicalMass));
      } else {
        return(Convert.ToString(cont[0].CalculateShipMass().TotalMass));
      }

  } else if(isStart("grid",low)) {

      if(isStart("nam",low)) {
        return(cont[0].CubeGrid.CustomName);
        
      } else if(isStart("pos",low)) {
          return vec2str(cont[0].CenterOfMass);
        
      } else if(isStart("mar",low)) {
        string s = igcData; float d = 0;
        int p = valParseInt(low,"[]"); 
        d = (p > 0 ? (float)p:gridSize(term[0], "max"));
      
        // marging along vector from IGC
        if((p = s.IndexOf("GPS:")) != -1) {
          s = s.Substring(p); p = s.LastIndexOf(":#");
          s = (p > 0 ? s.Substring(0,p+2) : "u");
        } else s = "u"; 
        
        return vec2str(new Vector3D(vecMargin(term[0],d,s)));

      } else if(isStart("park",low)) {
        var con = new List<IMyShipConnector>();
        GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(con, b => b.IsSameConstructAs(Me));
        foreach(IMyShipConnector c in con){ 
          if(Convert.ToString(c.Status) == "Connected") {
            return($"{c.OtherConnector.CubeGrid.CustomName}:{c.OtherConnector.CustomName}");
          } 
        } return("");
        
      } else if(isStart("size",low)) {
        return ("hwl".IndexOf(low[low.Length-1]) == -1 ? 0 :
                gridSize(term[0],low[low.Length-1]+"")).ToString();
          
      }
      
   } else {
      low = blockProp(term[0], prop, ref type); if(low != "") return(low);
      low = blockInfo(term[0], prop); if(low != "") return(low);
   }

  if(block == "") block = "MyShip"; { Console(302, $"{block}|{prop}"); }

  return("");
}

string shipInventory(string blocks, string req) 
{
  string[] grp = req.Split(' '); if(grp.Count() < 2) return "";
  
  bool x = grp[0].Trim().ToLower().EndsWith("x"); 
  string low = blocks.ToLower(); req = grp[1].Trim();
  
  bool myship  = low == "myship",  // am[0] 
       allgrid = low == "allgrid", // am[1]
       mygrid  = low == "mygrid";  // am[2] 
  
  invforce = (!myship && !allgrid && !mygrid) || invforce;
  
  if(invent.Count == 0 || (DateTime.Now - invscan).TotalSeconds > invrescan || invforce)
  {
    invscan = DateTime.Now; invforce = (!myship && !allgrid && !mygrid); 
    invent = new Dictionary<string, double[]>(); 
    var list = new List<IMyTerminalBlock>();

    if(allgrid || mygrid || myship) {
      GridTerminalSystem.GetBlocks(list);
    } else {
      blockSearch(blocks, ref list);
    }
    
    foreach(IMyTerminalBlock block in list) 
    {
      if(block.HasInventory) {
        var items = new List<MyInventoryItem>();
        block.GetInventory(0).GetItems(items); 
        
        if(block is IMyProductionBlock) {
          var its = new List<MyInventoryItem>();
          block.GetInventory(1).GetItems(its); 
          foreach(MyInventoryItem i in its) items.Add(i);
        }  
        
        if(items.Count > 0) {
          
          // MyObjectBuilder_Component/Explosives
          foreach(var item in items) {
            if(item.Amount > 0) {
              string key = item.Type.ToString();
              string tp = getWord(key,"_/").ToLower().Substring(0,3);
              string it = key.Substring(key.IndexOf("/")+1);
              double a = Math.Round((float)item.Amount,0); 
              double[] am = new double[] { a, a ,a };
              
              if(!block.IsSameConstructAs(Me)) am[1] = 0; 
              if(block.CubeGrid != Me.CubeGrid) am[2] = 0;
                  
              if(tp == "amm") {
                if(it.IndexOf("x45") > 0) it = "Turret 56x45 mm";
                if(it.IndexOf("x184") > 0) it = "Gatling 25x184 mm";
                if(it.IndexOf("e200") > 0) it = "Missile 200 mm";
                it = it.Replace("_"," ").Replace("Gun","");
                it = it.Replace("Automatic","").Replace(" Mag","Mag");
              } else if(tp == "phy") {  
                tp = "arm"; it = it.Replace("Item","").Trim();
              } else if(!isWord(tp,"com|amm|ore|ing|arm")) {
                tp = "oth" ; it = it.Replace("Item","").Trim();
              }  
               
              key = tp+":"+valTextCamel(it); 
               
              if(invent.ContainsKey(key)) {
                invent[key][0] += am[0]; 
                invent[key][1] += am[1]; 
                invent[key][2] += am[2]; 
              } else {
                invent.Add(key,am);
              }  
            }
          }
        }
      }  
    }
    invent = invent.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
  }
  
  grp = new string[] {"*", req}; string s = ""; 
  
  int idx = 0; if(allgrid) idx = 1; if(mygrid) idx = 2;
    
  if(req.IndexOf("[") != -1) grp = req.Trim(']').Split('[');
  if(grp[0].Length < 3) grp[0] = "*"; else grp[0] = grp[0].Substring(0,3); 
  if(grp[1] == "") grp[1] = "*"; int w = 0; string spp = new string(' ',50);
  if(!isWord(grp[0],"*|com|amm|ore|ing|arm|oth")) return "";
  
  foreach (var p in invent) 
  {
    if(grp[0] != "*" && !p.Key.StartsWith(grp[0]+":")) continue;
    if(grp[1] != "*" && p.Key.IndexOf(grp[1],StringComparison.OrdinalIgnoreCase) == -1) continue;
    
    double i = p.Value[idx]; string val = i.ToString(); if(w < val.Length) w = val.Length;
    string it = p.Key.Substring(4); if(lng.Count > 0 && lng.ContainsKey(it)) it = lng[it];
    string sp = spp.Substring(0,val.Count(f => f=='1')+1);
    
    if(grp[0] == "*" && s != "") {
      s = s.TrimEnd('\n')+$" ({val})\n";
    } else {
      if(!invempty && !x && i == 0 && grp[1] == "*") continue;
      s += (x ? $"{val}\n" : $"{val}{sp}{it}\n");
    }
  }

  if(x) return (s == "" ? "0" : s.TrimEnd('\n')); 
  grp = s.TrimEnd('\n').Split('\n'); s = "";
  
  foreach(var l in grp) {
    int i = l.IndexOf(" ");
    if(i > 0) s += l.Substring(0,i)+spp.Substring(0,(w-i)*2+5)+l.Substring(i+1)+"\n";
  }  
  
  return s.TrimEnd('\n');
}  

float gridSize(IMyTerminalBlock block, string dim = "max") 
{
  float gs = (float)block.CubeGrid.GridSize; Vector3D[] points = new Vector3D[4];

  OrientedBoundingBoxFaces _obbf = new OrientedBoundingBoxFaces(block);
  _obbf.GetFaceCorners(OrientedBoundingBoxFaces.LookupFront, points); // 5 = front
  
  float w = (float)Math.Round((points[0] - points[1]).Length(),1);
  float h = (float)Math.Round((points[0] - points[2]).Length(),1); _obbf.GetFaceCorners(0, points);
  float l = (float)Math.Round((points[0] - points[2]).Length(),1);

  float lb = (l / gs); float wb = (w / gs); float hb = (h / gs); 
  
  if(dim == "l") return l; if(dim == "w") return w; if(dim == "h") return h;
  
  return Math.Max(Math.Max(l,w),h);
}  

string blockDefine(long id)
{
  var block = GridTerminalSystem.GetBlockWithId(id); if(block == null) return "";
  string def = block.BlockDefinition.ToString(); int i = def.IndexOf("_");
  string build = def.Substring(i+1,def.IndexOf("/")-i-1);

  if((i = def.IndexOf("/")) == def.Length-1 || isWord(build,"REACTOR|GYRO")) {
    def = (build == "Gyro" ? "Gyroscope" : build); i = -1;
  }

  def = valTextCamel(def.Substring(i+1));

  if((i = def.IndexOf(" ")) < 0) i = 0;
  else if(!isWord(def.Substring(0,5),"LARGE|SMALL")) i = 0;
  
  def = def.Substring(i).Replace("Ship","");
  if(def.IndexOf("Block") < (def.Length - 6)) def = def.Replace("Block","");
  
  return def.Trim();
}

string blockProp(IMyTerminalBlock block, string req, ref string type)
{
  string val = ""; tShot = tShot.Replace("-"," "); int exact = tShot.IndexOf(" ");
  
  if(exact > 0) req = tShot.Substring(0,exact); req = req.ToLower();
  
  var props = new List<ITerminalProperty>(); block.GetProperties(props);

  for(int i=0; i < props.Count; ++i)
  {
    if(props[i].Id.ToLower().StartsWith(req))
    {
      if(exact > 0 & props[i].Id != tShot) continue;
      
      tShot = props[i].Id; type = props[i].TypeName.ToLower();
      
      if(type == "single")
        return Convert.ToString(Math.Round(block.GetValue<float>(tShot),fPrec));
      else if(type == "boolean")
        return Convert.ToString(block.GetValue<Boolean>(tShot));
      else if(type == "color")
        return colorStr(block.GetValue<Color>(tShot));
      else 
        return Convert.ToString(block.GetValue<String>(tShot));
    }
  }

  type = ""; return val;
}

string blockInfo(IMyTerminalBlock block, string req)
{
  string def = block.DetailedInfo, force = "";
  int i = req.IndexOf('['); if(i == -1) return def;

  req = req.Substring(i+1).TrimEnd(']'); 
  bool f = (req.IndexOf("!") == 0); if(f) req = req.Substring(1);
  int l = valParseInt(req); string[] arr = def.Split('\n'); 
  
  for(i=0; i < arr.Count(); ++i) {
    if(l > 0) {
      if(l == (i+1)) 
        return arr[i].Substring(arr[i].IndexOf(':')+1).Trim();
    } else {
      if(arr[i].IndexOf(req) == 0) {
        force = arr[i].Substring(arr[i].IndexOf(':')+1); 
        if(!f) return force;
      }  
    }
  }
  return (f && force != ""? force : def);
}

string aiPattern(IMyTerminalBlock block, string ai = "") 
{
  string ret = "";
  string[] off = {"CycleOrbit","StayAtRange","HitAndRun","Intersept"};
  string[] def = {"Always","Never","WhenTakingDamage","WhenTargetLocked"};
  string[] bas = {"","FollowPlayer","FollowHome","AutoPilot"};

  if(block is IMyOffensiveCombatBlock) {
    var b = block as IMyOffensiveCombatBlock; ret = off[(int)b.SelectedAttackPattern];
    if(ai != "") { long i = Array.IndexOf(off,ai); if(i != -1) b.SelectedAttackPattern = i; else ret = ""; }  
    
  } else if(block is IMyDefensiveCombatBlock) {
    var b = block as IMyDefensiveCombatBlock; ret = Convert.ToString(b.FleeTrigger);
    if(ai != "") {
      if(Array.IndexOf(def,ai) != -1)
        b.FleeTrigger = (FleeTrigger)Enum.Parse(typeof(FleeTrigger), ai); else ret = "";
    }

  } else if(block is IMyBasicMissionBlock) {
    var b = block as IMyBasicMissionBlock; ret = bas[(int)b.SelectedMissionId];
    if(ai != "") { long i = Array.IndexOf(bas,ai); if(i != -1) b.SelectedMissionId = i; else ret = ""; }  
    
  } else if(block is IMyTurretControlBlock) {
    var b = block as IMyTurretControlBlock; ret = Convert.ToString(b.AIEnabled);
    if(ai != "") b.AIEnabled = (ai.ToLower() == "true");
  }
    
  return ret;
}  

void outDebug(string w = "") 
{
  if(!isWord(tShot,"delay|when|{|}") || w != "") 
  {
     string s = (sBlock > 0 ? $" ({sBlock})" : ""), c = codeToken(0); 
     
     if(isWord(c,sysToken)) {
       if(c == "else" && codeToken(1) == "if") c += " if"; 
       c += " "+(tShot == c || tShot == "else" ? "" : tShot);
     } else {  
       if(tShot != "" && tShot[0] == '\\') c += " "+tShot;
       else c = (c == tShot || tShot == "" ? c : tShot);
     }  
     Console(0,$"@{cBlock} : {c}{s}\n");
  }
}

bool Console(int index = 0, string text = "")
{
  int i = 0; string s = "Compile"; IMyTextSurface surf = null; 
  
  if(index > 0)
  {
    if(index > 400) Stop();

    if(index > 300) {
      string mes = sys[index]; string[] t = text.Split('|');

      for(i=0; i < t.Count(); ++i) {
        mes = mes.Replace("#"+(i+1),(i==0?"\"":"")+t[i]+(i==0?"\"":""));
      }
      if(cBlock != "") s = "@"+cBlock; if(cLine > 0) s = codeToken(0);
      text = $"E{index} > {s}\n{mes}\n"; text = text.Replace("\\n","\n");
    } else {
      text = sys[index];
    }
  }

  if(conLCD != "") 
  {
    s = conLCD; i = valParseInt(s,"[]"); 
    if(s.IndexOf($"[{i}]")!=-1) s = s.Replace($"[{i}]","");
    
        var list = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(s, list);
    
    if(list.Count > 0) {
      surf = getSurface(list[0],i);
    } else {
      text = sys[311]; Echo(text);
    }
  }

  if(surf == null) surf = (IMyTextSurface)Me.GetSurface(0);
  if(debug) surf.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
  surf.WriteText(text, debug && text != ""); return false;
}

string surfaceShape(IMyTextSurface sur, string set = "") 
{
   if(set == "") {
      return "ff="+sur.Font+"|"+
             "fs="+Convert.ToString(sur.FontSize)+"|"+
             "fc="+colorStr(sur.FontColor)+"|"+
             "ta="+Convert.ToString(sur.Alignment)+"|"+
             "tp="+Convert.ToString(sur.TextPadding)+"|"+
             "bc="+colorStr(sur.BackgroundColor);
   } else {
      string[] line = set.Trim('|').Split('|');
      
      for(int i = 0; i < line.Count(); ++i) {
        if(line[i].IndexOf("=") > 0) 
        {
          string[] v = line[i].Split('=');
          switch(v[0]) {
          case "ff":
            sur.Font = v[1]; break;
          case "fs":
            sur.FontSize = valParseFloat(v[1]); break;
          case "fc":
            sur.FontColor = valColor(v[1]); break;
          case "tp":
            sur.TextPadding = valParseFloat(v[1]); break;
          case "bc":
            sur.BackgroundColor = valColor(v[1]); break;
          case "ta":
            if(v[1].ToLower() == "center")
              sur.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
            else if(v[1].ToLower() == "right")
              sur.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.RIGHT;
            else
              sur.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
            break;
          }
        }
      }
   }  
   return "";
}  

IMyTextSurface getSurface(IMyTerminalBlock block, int scr = 0)
{
  if(block is IMyTextPanel) {
    var b = (IMyTextPanel)block; return (IMyTextSurface)b;

  } else if(block is IMyTextSurfaceProvider){
    var b = (IMyTextSurfaceProvider)block;
    return b.GetSurface(scr>b.SurfaceCount-1?0:scr);

  }
  return (IMyTextSurface)Me.GetSurface(0);
}

void varParse(string val, ref List<string> list, bool getvar = false)
{
  string low = val.ToLower(), s = ""; int i = 0;
  
  if(low.StartsWith("math")) {
    
    if(low.IndexOf("of (") > 0) {
      int x = 0;
      while((i = val.IndexOf("of (",StringComparison.OrdinalIgnoreCase)) > 0) {
        x = val.IndexOf(")",i); 
        s += val.Substring(0,i+3)+"¤"+val.Substring(i+4,x-i-4)+"§";
        val = val.Substring(x+1);
      }  
      val = s+val;
    }  
    
    val = wideSpace(val,sepMath); val = val.Replace("¤","(").Replace("§",")"); list.Add("math");
    var t = new List<string>(val.Split(ssp,StringSplitOptions.RemoveEmptyEntries));

    for(i=1; i < t.Count; ++i)
    {
      if(sepMath.IndexOf(t[i]) != -1) {
        if(s != "") list.Add(getvar ? varGet(s) : s.Trim());
        list.Add(t[i].Trim()); s = "";

      } else if(t[i].ToLower() == "of") {
        if(s != "") list.Add(getvar ? varGet(s) : s.Trim());
        list.Add("of"); s = "";

      } else {
        s += t[i]+" ";
      }
    }
    
  } else if(low.StartsWith("cond")) {

    if(val.IndexOf('"') > 0) {
      bool q = false; val = val.Replace("\"\"","¤¤");

      while((i = val.IndexOf('"')) > 0) {
        if(!q) s += val.Substring(0,i);
        else s += "¤"+val.Substring(0,i).Replace(":","§").Replace(' ','')+"¤";
        val = val.Substring(i+1); q = !q;
      }  
      val = s.Replace('¤','"')+val;
    }
    
    val = wideSpace(val.Replace("¤¤","\"\""),"(:)"); 
    val = val.Substring(7,val.Length-9).Replace("( ","(").Replace(" )",")");
    var t = val.Split(ssp,StringSplitOptions.RemoveEmptyEntries); 
    
    list.Add("cond"); list.Add("("); s = "";
    
    for(i=0; i<t.Count(); ++i)
    {
      if(isWord(t[i],sepLogic+"|:|&||")) {
        if(s != "") list.Add(getvar ? varGet(s) : s.Trim());
        list.Add(t[i].Trim()); s = "";

      } else if(t[i].ToLower() == "of") {
        if(s != "") list.Add(getvar ? varGet(s) : s.Trim());
        list.Add("of"); s = "";

      } else {
        s += t[i].Replace('',' ')+" ";
      }
    }
    list.Add(s.Replace('',' ')); list.Add(")");
    
  } else if((val.IndexOf('"') != 0) && (i = low.IndexOf(" of ")) > 0) {
    list.Add(val.Substring(0,i).Trim()); list.Add("of"); 
    list.Add(val.Substring(i+4).Trim());

  } else {
    if(val.IndexOf('"') == -1) {
      val = valPrefix(val.Trim('"')); 
    } else {
      val = valInsert(val);
    }  
    list.Add(val.Trim('"'));
  }
}

string varGet(string s = "") 
{
  s = s.Trim();
  if(s[0] == '\\') {
    string v = s.Substring(1); if(varInt.Contains(v)){ s = varValue(v)[0]; }
  }
  return s;
}

string varInsert(string text, bool q2 = false)
{
  if(text.IndexOf("\\") != -1) {
    var vars = varInt.ToList(); string n = "";

    for(int i=0; i < varExt.Count; ++i) vars.Add((i+1)+"");

    for(int i=0; i < vars.Count; ++i) {
      string vv = "\\"+vars[i], val = "";
      int v = text.IndexOf(vv);

      if(v != -1) {
        if(i < varInt.Count) {
          val = varValue(varInt[i])[0];
        } else if((i - varInt.Count) >= 0) {
          val = varExt[i-varInt.Count];
        }
        if(q2 && val == "") val = "\"\"";
        text = text.Replace(vv, val);
        n += "\\"+vv.Substring(1)+", ";
      }
    }
    tShot = $"{n.Trim().Trim(',')}";
  }

  return text;
}

// Set (block, color, 0:0:100) / Get (block, data[color])
string varCustomData(IMyTerminalBlock block, params string[] kv) 
{
  string data = block.CustomData, key; int i = kv.Length;
  
  if(i == 0) return data; if(kv[0] == "") return(i < 2 ? data:kv[1]);
  key = (i == 2 ? kv[0] : kv[0].Substring(4).Trim('[').Trim(']'));

  string[] line = data.Split('\n'); char sep = '=';
  
  // read @block { }
  if(key[0] == '@') {
    if((i = data.IndexOf(key)) == -1) return ""; 
    
    data = ""; bool b = false;
    for(i = 0; i < line.Count(); ++i) {
      line[i] = line[i].Trim();
      if(!b) {
        b = (line[i].StartsWith(key) && line[i].EndsWith("{"));
      } else {
        if(line[i] == "}") break; data += line[i]+"\n";
      }
    }
    return data;
  } else if(key[0] == ':') {
    sep = ':'; key = key.Substring(1);
  }

  for(i = 0; i < line.Count(); ++i) 
  {
    if(line[i].Trim().IndexOf(key) == 0) {
      int j = line[i].IndexOf($"{sep}");
      if(j > 0) {
        if(kv.Length == 1) 
          return line[i].Substring(j+1).Trim();
        line[i] = key+$"{sep} "+kv[1]; break;
      }  
    }  
  }
    
  if(kv.Length == 2) {
    // change var or add non exists var
    if(i < line.Count()) 
      data = String.Join("\n",line); 
    else 
      data += ("\n"+key+$" {sep} "+kv[1]).Trim('\n');
  } else {
    data = ""; // get var not found
  }
  
  return data;
}  

string valInsert(string t) 
{
  int bs = t.IndexOf("{"), be = t.IndexOf("}"), 
      of = t.IndexOf(" of ",StringComparison.OrdinalIgnoreCase);
  
  if(bs == -1 || be == -1 || of < bs || of > be) return t;
  
  while(of > bs && of < be) {
    string p = t.Substring(bs+1, be-bs-1);
    of = p.IndexOf(" of ",StringComparison.OrdinalIgnoreCase);
    var s = GetSignals(p.Substring(of+4),p.Substring(0,of)); 
    t = t.Replace("{"+p+"}",s.Count == 0 ? "":s[0]);
    bs = t.IndexOf("{"); be = t.IndexOf("}");
    of = t.IndexOf(" of ",StringComparison.OrdinalIgnoreCase);
  }  

  return t;
}  

int valParseInt(string val, string sep = "")
{
  int ret = 0; float f = 0; if(sep != "") val = getWord(val,sep);
  if(float.TryParse(val, out f)) ret = (int)Math.Round(f,0);
  return ret;
}

float valParseFloat(string val, params int[] prec)
{
  float ret = 0; int p = (prec.Length == 0 ? fPrec : prec[0]);
  if(float.TryParse(val, out ret)) ret = (float)Math.Round(ret,p);
  
  return ret;
}

string valParseNum(string val) {
  int b = -1; string sep = $"{numbers}-.";
  
  for(int i=0; i < val.Length; ++i) {
    bool num = (sep.IndexOf(val[i]) != -1);
    if(b < 0 && num) { b = i; }
    else if(!num && b >= 0) { val = val.Substring(b,i-b); break; }
    else if(val[i] == '.') sep = sep.Replace(".","");
    else if(val[i] == '-') sep = sep.Replace("-","");
  }
  float f; return float.TryParse(val, out f)?val:"0";
}  

string valPrefix(string s) 
{
  string r = s.Replace(" ",""), p = ""; int i = 0;
  
  if(s != "" && numbers.IndexOf(s[0]) != -1) {
    while($" .{numbers}".IndexOf(r[r.Length-1]) == -1) {
      p = r[r.Length-1]+""; r = r.Substring(0,r.Length-1);
    }
    r = r.Trim(); p = p.ToLower();
    if(p == "m") i = (int)(valParseFloat(r)*1000000);
    else if(p == "k") i = (int)(valParseFloat(r)*1000);
    else return s; s = i.ToString();  
  }
  
  return s;
}  

string valTextCamel(string t) 
{
  if(t.Length == 0) return t;
  
  string s = t.ToLower(), w = ""; 
  bool up = (s[0] != t[0]);
  
  for(int i=0; i < t.Length; ++i) {
    if(i > 0) {
      string p = t.Substring(i-1,1); up = (p != p.ToLower());
    }  
    if(s[i] != t[i]) w += (up?"":" "); w += t[i];
  }
  return w.Trim();
}

Color valColor(string c)
{
  string[] a = c.Split(':'); if(a.Count() != 3) a = new string[] {"0","0","0"};
  return(new Color(int.Parse(a[0]), int.Parse(a[1]), int.Parse(a[2])));
}

string colorStr(Color cc) // {R:200 G:100 B:100 A:255}
{
  string c = Convert.ToString(cc), v = ""; int a = 0, b = 0;

  while((a = c.IndexOf(':', a)) != -1 && b < 3) {
    v += c.Substring(a,c.IndexOf(' ', a)-a); ++a; ++b;
  }
  return(v.Trim(':'));
}

bool isWord(string req, string tokens)
{
  tokens = "|"+tokens.ToLower()+"|"; return tokens.Contains("|"+req.ToLower().Trim()+"|");
}

bool isStart(string req, string tokens)
{ 
  string r = req.ToLower();
  string[] w = tokens.ToLower().Split(ssp,StringSplitOptions.RemoveEmptyEntries);
  for(int i=0; i < w.Count(); ++i) { if(w[i].StartsWith(r)) return true; } return false;
}

string getWord(string str, string sep = "") 
{
  if(sep == "") return ""; // sep = pair [] or single |
  int i = 0, j = 0; char s0 = sep[0]; char s1 = sep.Length < 2 ? s0 : sep[1];
  if((i = str.IndexOf(s0)) < 0 || (j = str.IndexOf(s1,i+1)) < 0) return "";
  return str.Substring(i+1,j-i-1);
}

string codeToken(int n)
{
  int i = code[rBlock][cLine-1].Count;
  if(n < 0) { if(Math.Abs(n) <= i) n += i; else return ""; }
  return (n < i ? code[rBlock][cLine - 1][n] : "");
}

int hasToken(string t, List<string> list = null)
{
  if(list == null) list = code[rBlock][cLine - 1];
  return(list.FindIndex(s => s.Equals(t, StringComparison.OrdinalIgnoreCase)));
}

bool findChar(string req, string chr)
{
  StringBuilder b = new StringBuilder(chr);
  for(int i=0; i < b.Length; ++i) { if(req.IndexOf(b[i]) != -1) return true; } return false;
}  

string wideSpace(string t, string s) {
  for(int i=0; i < s.Length; ++i) { t = t.Replace(s.Substring(i,1),$" {s[i]} "); } return t;
}

string shotSpace(string t) {
  var a = t.Split(ssp,StringSplitOptions.RemoveEmptyEntries); return(string.Join(" ",a)); 
}  


string RandomNum(int length) {
  return new string(Enumerable.Repeat(numbers, length).Select(s => s[random.Next(s.Length)]).ToArray());
}

string vec2str(Vector3D v) {
    return v.GetDim(0) + ":" + v.GetDim(1) + ":" + v.GetDim(2);
}

// GPS:# BASE #:-62664.81:-80178.37:-27915.08:#FFE9F175:
Vector3D str2vec(string coord) 
{ 
  double x, y, z; string[] c = coord.Trim(':').Split(':'); int i = c.Count();
  if(c.Count() > 2) {  
    while(i-- > 0) {
      if(double.TryParse(c[i].Trim(), out z)) {
        if(double.TryParse(c[i-1].Trim(), out y)) {
          if(double.TryParse(c[i-2].Trim(), out x)) return new Vector3D(x,y,z);
        }  
      }  
    }  
  } return new Vector3D(0,0,0);
}

// thank for Lazy Kitty
Vector3D vecMargin(IMyTerminalBlock block, float meters = 50f, string dir = "f") 
{ 
  Vector3D v_app; Vector3D v_block;
                           
  if(dir.Length == 1 || !(block is IMyShipController)) {
    v_app = new Vector3D(block.CubeGrid.GridIntegerToWorld(block.Position + 
                Base6Directions.GetIntVector(block.Orientation.Up))); 
    v_block = block.CubeGrid.GridIntegerToWorld(block.Position); 
  } else {
    v_app = str2vec(dir); v_block = (block as IMyShipController).CenterOfMass;
  }
  
  Vector3D v_offset = v_app - v_block;
  Vector3D v_normal = v_offset / Math.Sqrt(v_offset.X*v_offset.X + v_offset.Y*v_offset.Y + v_offset.Z*v_offset.Z);
  return new Vector3D(v_block + v_normal * meters);
}

// Thank for Wicorel
public struct OrientedBoundingBoxFaces
{
  public Vector3D[] Corners;
  public Vector3D Position;
  Vector3D localMax;
  Vector3D localMin;

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
    for(int i = 0; i < PointsLookup[face].Length; i++) {
       points[index++] = Corners[PointsLookup[face][i]];
    }
  }
}

void Cycle() { Runtime.UpdateFrequency = UpdateFrequency.Once; }
