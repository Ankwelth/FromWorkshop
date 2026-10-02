
// Block Renaming Script by Isy
// =======================
// Version: 1.6.4
// Date: 2023-08-28

// Guide: https://steamcommunity.com/sharedfiles/filedetails/?id=2066808115

// =======================================================================================
//                                                                            --- Configuration ---
// =======================================================================================

// Define the character that will be added between names and numbers when sorting or adding strings.
// Default (whitespace): char spaceCharacter = ' ';
const char spaceCharacter = ' ';

// Add an additional space character after/before addfront/addback?
bool extraSpace = true;

// Use custom number length?
// By default, the script will determine automatically, how long your numbers should be by getting the total
// amount of blocks of your current filter. If you don't like that, enable this option and adjust your wanted length.
// The length of a number is the amount of digits and will be filled with zeros, if needed.
// Example: Automatic mode in a set of 1000 blocks would produce: 0001, 0010, 0100, 1000
// Example: A custom length of 3 in the same set would produce: 001, 010, 100, 1000
bool useCustomNumberLength = false;
int customNumberLength = 0;


// --- Modifier Keywords ---
// =======================================================================================

// Keyword for sorting after performing another operation
const string sortKeyword = "!sort";

// Keyword for test mode (changes will only be shown but not executed)
const string testKeyword = "!test";

// Keyword for help (can be added to any command to see the in-script help, example: 'rename,!help')
const string helpKeyword = "!help";


// --- LCD Panels ---
// =======================================================================================

// Keword for LCD output
// Everything the script writes to the terminal, will also be written on the LCD containing the keyword.
// The keyword will be transformed to my universal [IsyLCD] keyword, once the script has recognized it. That way,
// it's also possible to show the contents on block LCDs. The screen can be changed in the custom data (see guide).
string mainLCDKeyword = "[IBR-main]";

// Default screen font, fontsize and padding, when a screen is first initialized. Fonts: "Debug" or "Monospace"
string defaultFont = "Debug";
float defaultFontSize = 0.6f;
float defaultPadding = 2f;


// =======================================================================================
//                                                                      --- End of Configuration ---
//                                                        Don't change anything beyond this point!
// =======================================================================================


bool č,Č,ċ,Ċ;string Ď=@"( |"+spaceCharacter+@")\d+\b";List<string>ĉ=new List<string>(){"","","",""};List<
IMyTerminalBlock>ď=new List<IMyTerminalBlock>();List<string>ğ=new List<string>();List<string>ĝ=new List<string>();List<IMyTerminalBlock>
Ĝ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ě=new List<IMyTerminalBlock>();List<IMyCubeGrid>Ě=new List<
IMyCubeGrid>();HashSet<String>ę=new HashSet<String>();List<string>Ę=new List<string>();int ė=0;int Ğ=0;int Ė=0;DateTime ĕ=DateTime.
Now;int Ĕ=0;Program(){ē();L();ī();}void ē(){ğ=new List<string>(){"Isy's Block Renaming Script\n======================\n"};}
void Main(string Ē){Ė++;if(Ĝ.Count>0){Ĺ();if(!Ċ)N("Creating sort list");Ĕ+=Runtime.CurrentInstructionCount;Ĝ.Clear();return;
}if(Ě.Count>0){ĳ();N("Collecting unique names");Ĕ+=Runtime.CurrentInstructionCount;if(Ğ<Ě.Count){return;}else{Ě.Clear();Ğ
=0;return;}}if(ě.Count>0){ŀ();if(ė<ě.Count){N("Renaming blocks: "+ĝ.Count);Ĕ+=Runtime.CurrentInstructionCount;return;}
else{Runtime.UpdateFrequency=UpdateFrequency.None;ě.Clear();ė=0;ī();return;}}ē();ĕ=DateTime.Now;Ė=0;Ĕ=0;Ē=Ē.Replace("*","");
List<string>đ=Ē.Split(',').ToList();đ=đ.Concat(ĉ).ToList();č=false;Č=false;ċ=false;Ċ=false;ď.Clear();ĝ.Clear();if(Ē.Contains
(testKeyword)){č=true;đ.Remove(testKeyword);}if(Ē.Contains(sortKeyword)){Č=true;đ.Remove(sortKeyword);}if(Ē.Contains(
helpKeyword)){Ċ=true;đ.Remove(helpKeyword);}if(đ[0]=="undo")ċ=true;if(đ.Count==0){L();ī();return;}if(!č&&!ċ&&!Ċ&&Ē!=String.Empty)
Storage="";if(Ē==String.Empty){L();}else if(Ċ){L(đ[0]);}else if(đ[0]=="rename"){if(đ[2]!=""){Đ(đ[1],đ[2],đ[3],đ[4]);}else{µ(đ[0
],2);L(đ[0]);}}else if(đ[0]=="replace"){if(đ[2]!=""){ă(đ[1],đ[2],đ[3],đ[4]);}else{µ(đ[0],2);L(đ[0]);}}else if(đ[0]==
"remove"){if(đ[1]!=""){Ć(đ[1],đ[2],đ[3]);}else{µ(đ[0],1);L(đ[0]);}}else if(đ[0]=="removenumbers"){Ĉ(đ[1],đ[2]);}else if(đ[0]==
"defaultname"){ć(đ[1],đ[2]);}else if(đ[0]=="sort"){if(đ[1]!=""){Ġ(đ[1],đ[2]);}else{µ(đ[0],1);L(đ[0]);}}else if(đ[0]=="sortbygrid"){if
(đ[1]!=""){ĸ(đ[1]);}else{µ(đ[0],1);L("sort");}}else if(đ[0]=="autosort"){ķ(đ[1]);if(Ě.Count>0){N("Getting list of grids")
;Ĕ+=Runtime.CurrentInstructionCount;Runtime.UpdateFrequency=UpdateFrequency.Update1;return;}}else if(đ[0]=="addfront"||đ[
0]=="addback"){if(đ[1]!=""){ľ(đ[0],đ[1],đ[2],đ[3]);}else{µ(đ[0],1);L(đ[0]);}}else if(đ[0]=="renamegrid"){if(đ[1]!=""){Ļ(đ
[1],đ[2]);}else{µ(đ[0],1);L(đ[0]);}}else if(đ[0]=="copydata"){if(đ[2]!=""){ĥ(đ[1],đ[2],đ[3]);}else{µ(đ[0],2);L(đ[0]);}}
else if(đ[0]=="deletedata"){if(đ[1]!=""){ģ(đ[1],đ[2]);}else{µ(đ[0],1);L(đ[0]);}}else if(ċ){Ģ();}else{ĝ.Add(
"Error!\nUnknown Command!\n");L();}if(Č){ı(ď);}if(đ[0]=="renamegrid"){ī("grids");}else if(đ[0].Contains("data")){ī("data");}else{ī();}}void Đ(string
O,string À,string u="",string t=""){var s=w(O,u,t);foreach(var p in s){string ą=p.CustomName;string É=Ê(ą);string Ą=À+É;ď
.Add(p);Æ(ą,Ą);Á(p,Ą);}}void ă(string Ă,string ā,string u="",string t=""){var s=w(Ă,u,t);foreach(var p in s){string O=p.
CustomName;string À=O.Replace(Ă,ā);ď.Add(p);Æ(O,À);Á(p,À);}}void Ć(string Ā,string u="",string t=""){var s=w(Ā,u,t);foreach(var p
in s){string O=p.CustomName;StringBuilder À=new StringBuilder(O);À.Replace(Ā+" ","").Replace(" "+Ā,"").Replace(Ā+
spaceCharacter,"").Replace(spaceCharacter+Ā,"");ď.Add(p);Æ(O,À.ToString());Á(p,À.ToString());}}void Ĉ(string u="",string t=""){var s=w
("",u,t);foreach(var p in s){string O=p.CustomName;string À=È(O);ď.Add(p);Æ(O,À);Á(p,À);}}void ć(string u="",string t="")
{var s=w("",u,t);foreach(var p in s){string O=p.CustomName;string À=p.DefinitionDisplayNameText;ď.Add(p);Æ(O,À);Á(p,À);}}
void Ġ(string u,string t=""){var s=w("",u,t);ı(s,true);}void ĸ(string u){var s=w("",u);HashSet<IMyCubeGrid>Ħ=new HashSet<
IMyCubeGrid>();foreach(var p in s){Ħ.Add(p.CubeGrid);}foreach(var t in Ħ){Ġ(u,t.CustomName);}}void ķ(string ĵ=""){if(ĵ!=""){HashSet
<IMyCubeGrid>Ĵ=new HashSet<IMyCubeGrid>();List<IMyTerminalBlock>s=new List<IMyTerminalBlock>();GridTerminalSystem.
GetBlocks(s);foreach(var p in s){if(ĵ=="all"){Ĵ.Add(p.CubeGrid);}else{if(p.CubeGrid.CustomName.Contains(ĵ)){Ĵ.Add(p.CubeGrid);}}}
Ě=Ĵ.ToList();}else{Ě=new List<IMyCubeGrid>(){Me.CubeGrid};}}void ĳ(){ę.Clear();Ę.Clear();GridTerminalSystem.
GetBlocksOfType<IMyTerminalBlock>(Ĝ,Q=>Q.CubeGrid==Ě[Ğ]);Ğ++;foreach(var p in Ĝ){string Ķ=È(p.CustomName);Ę.Add(Ķ);ę.Add(Ķ);}}void Ĺ(){
List<string>ļ=ę.OrderByDescending(v=>v).ToList();foreach(var v in ļ){for(int g=Ę.Count-1;g>=0;g--){if(Ę[g]==v){ě.Add(Ĝ[g]);Ĝ
.RemoveAt(g);Ę.RemoveAt(g);}}ě.Add(null);if(Runtime.CurrentInstructionCount>45000){ĝ.Add("Error!\nAutosort had to be stopped! You have too many different block names. Try another filter or simplify your blocknames first!"
);Runtime.UpdateFrequency=UpdateFrequency.None;Ĝ.Clear();ě.Clear();Ě.Clear();ę.Clear();Ę.Clear();ė=0;Ğ=0;Ċ=true;ī();
return;}}}void ŀ(){List<IMyTerminalBlock>Ŀ=new List<IMyTerminalBlock>();for(int g=ė;g<ě.Count;g++){ė++;if(ě[g]!=null){Ŀ.Add(ě[
g]);}else{break;}}ı(Ŀ,true);}void ľ(string Ľ,string Ł,string u="",string t=""){var s=w("",u,t);foreach(var p in s){string
O=p.CustomName;string À=O;if(Ľ.Contains("addfront")){if(!O.StartsWith(Ł)){À=Ł+(extraSpace?spaceCharacter.ToString():"")+O
;}}if(Ľ.Contains("addback")){if(!O.EndsWith(Ł)){À=O+(extraSpace?spaceCharacter.ToString():"")+Ł;}}ď.Add(p);Æ(O,À);Á(p,À);
}}void Ļ(string À,string ĺ=""){if(ĺ==""){Æ(Me.CubeGrid.CustomName,À);if(!č)Me.CubeGrid.CustomName=À;}else{List<
IMyTerminalBlock>s=new List<IMyTerminalBlock>();List<IMyCubeGrid>Ħ=new List<IMyCubeGrid>();GridTerminalSystem.GetBlocks(s);foreach(var p
in s){if(p.CustomName.Contains(ĺ)&&!Ħ.Contains(p.CubeGrid)){Ħ.Add(p.CubeGrid);Æ(p.CubeGrid.CustomName,À);if(!č)p.CubeGrid.
CustomName=À;}}}}void ĥ(string Ĥ,string u,string t=""){var Ã=w(Ĥ);if(Ã.Count==0){ğ.Add("Source block not found:\n'"+Ĥ+"'\n");
return;}var s=w("",u,t);foreach(var p in s){if(p==Ã)continue;Ä(Ã[0].CustomName,p.CustomName);if(!č)p.CustomData=Ã[0].
CustomData;}}void ģ(string u,string t=""){var s=w("",u,t);foreach(var p in s){Ä("",p.CustomName);if(!č)p.CustomData="";}}void Ģ(){
var ġ=Storage.Split('\n');if(Storage.Length==0){ğ.Add("No saved operations to undo!\n");}else{for(int g=ġ.Length-1;g>=0;g--
){var Ĥ=ġ[g].Split(';');if(Ĥ.Length!=2)continue;long P;if(!long.TryParse(Ĥ[0],out P))continue;IMyTerminalBlock p=
GridTerminalSystem.GetBlockWithId(P);if(p!=null){Æ(p.CustomName,Ĥ[1]);if(!č)p.CustomName=Ĥ[1];}}}}void ı(List<IMyTerminalBlock>İ,bool º=
false){if(İ.Count==0){ĝ.Add("Nothing to sort here..");return;}int į=İ.Count.ToString().Length;if(useCustomNumberLength)į=
customNumberLength;İ.Sort((Į,Q)=>Į.CustomName.CompareTo(Q.CustomName));for(int g=0;g<İ.Count;g++){string O=İ[g].CustomName;string À=O;
string ĭ="";if(İ.Count>1){int Ĭ=(g+1).ToString().Length;ĭ=spaceCharacter+ª('0',į-Ĭ)+(g+1);}À=È(À)+ĭ;Æ(O,À);Á(İ[g],À,º);}}void
ī(string Ī="blocks"){List<string>ĩ=new List<string>(ğ);ĩ=ĩ.Concat(ĝ).ToList();int Ĩ=ĝ.Count;if(!Ċ){if(Ĩ==0)ĩ.Add("No "+Ī+
" found!");if(č){if(Ī=="data"){ĩ.Add("\nTest Mode. No custom data was changed!");}else{ĩ.Add("\nTest Mode. No "+Ī+
" were renamed!");}}else if(ċ){ĩ.Add("\nUndid renaming of "+Ĩ+" "+Ī+"!");}else if(Ī=="data"){ĩ.Add("\nChanged the custom data of "+Ĩ+
" blocks!");}else{ĩ.Add("\nRenamed "+(Č?Ĩ/2:Ĩ)+" "+Ī+"!");}}ĩ.Add("\nThis operation took "+(DateTime.Now-ĕ).TotalMilliseconds+
"ms and "+(Ĕ+Runtime.CurrentInstructionCount)+" instructions!");if(Ė>0)ĩ.Add("The script was restarted "+Ė+
" times to split the load.");string T=String.Join("\n",ĩ);var ħ=I(mainLCDKeyword);for(int g=0;g<ħ.Count;g++){var Ĳ=ħ[g].Í(mainLCDKeyword);foreach(
var ÿ in Ĳ){var U=ÿ.Key;var y=ÿ.Value;if(!U.GetText().EndsWith("\a")){U.Font=defaultFont;U.FontSize=defaultFontSize;U.
TextPadding=defaultPadding;U.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.LEFT;U.ContentType=VRage.Game.GUI.TextPanel.
ContentType.TEXT_AND_IMAGE;}StringBuilder x=new StringBuilder(T);x=U.ö(x,ô:false);U.WriteText(x.Append("\a"));}}if(ĩ.Count>100){for
(int g=0;g<50;g++){Echo(ĩ[g]);}Echo(".\n.\n.");for(int g=ĩ.Count-50;g<ĩ.Count;g++){Echo(ĩ[g]);}}else{Echo(T);}}List<
IMyTerminalBlock>w(string v="",string u="",string t=""){List<IMyTerminalBlock>s=new List<IMyTerminalBlock>();if(u.StartsWith("G:")){var
r=GridTerminalSystem.GetBlockGroupWithName(u.Substring(2));if(r!=null){r.GetBlocksOfType<IMyTerminalBlock>(s,Q=>Q.
CustomName.Contains(v)&&Q.CubeGrid.CustomName.Contains(t));ğ.Add("Filtered by group:\n"+u.Substring(2)+"\n");}else{
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(s,Q=>Q.CustomName.Contains(v)&&Q.CubeGrid.CustomName.Contains(t));ğ.Add(
"Not filtered - Group not found!\n");}}else if(u.StartsWith("T:")){GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(s,Q=>Q.BlockDefinition.ToString().
ToLower().Contains(u.Substring(2).ToLower())&&Q.CustomName.Contains(v)&&Q.CubeGrid.CustomName.Contains(t));if(s.Count!=0){
HashSet<string>q=new HashSet<string>();ğ.Add("Filtered by type:");foreach(var p in s){if(q.Contains(p.BlockDefinition.ToString(
)))continue;q.Add(p.BlockDefinition.ToString());ğ.Add(p.BlockDefinition.TypeId.ToString()+"/\n"+p.BlockDefinition.
SubtypeId.ToString()+"\n");}ğ.Add("");}else{GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(s,Q=>Q.CustomName.Contains(v)&&Q
.CubeGrid.CustomName.Contains(t));ğ.Add("Not filtered - Type not found!\n");}}else{GridTerminalSystem.GetBlocksOfType<
IMyTerminalBlock>(s,Q=>Q.CustomName.Contains(v)&&Q.CustomName.Contains(u)&&Q.CubeGrid.CustomName.Contains(t));}return s;}string ª(char Ì
,int Ë){if(Ë<=0){return"";}return new string(Ì,Ë);}string Ê(string Ç){string É=System.Text.RegularExpressions.Regex.Match
(Ç,Ď).Value;return É==String.Empty?"":É;}string È(string Ç){try{return System.Text.RegularExpressions.Regex.Replace(Ç,Ď,
"");}catch{return Ç;}}void Æ(string O,string À){ĝ.Add((ĝ.Count+1)+". "+O+"\n   -> "+À);}void Ä(string Ã,string Â){if(Ã==""
){ĝ.Add((ĝ.Count+1)+". Data deleted:\n   -> "+Â);}else{ĝ.Add((ĝ.Count+1)+". Data copied: "+Ã+"\n   -> "+Â);}}void Á(
IMyTerminalBlock p,string À,bool º=true){if(!č){if(º)d(p.EntityId,p.CustomName);p.CustomName=À;}}void µ(string Å,int o){ĝ.Add(
"Error!\n'"+Å+"' needs at least "+o+" additional parameter"+(o>1?"s":"")+"!\n");}void d(long P,string O){Storage+=P+";"+O+"\n";}
void N(string M){Echo(ğ[0]+"\n"+M);}void L(string K=""){bool J=false;if(K==""){ĝ.Add("Instructions:\n");J=true;}else{ĝ.Add(
"Usage:\n");}if(K==""||K=="rename"){ĝ.Add("--- Rename ---");ĝ.Add("Rename a block containing OLDNAME:");ĝ.Add(
"rename,OLDNAME,NEWNAME [[,FILTER]] [[,GRID]]\n");J=true;}if(K==""||K=="replace"){ĝ.Add("--- Replace ---");ĝ.Add("Replace a string with another one:");ĝ.Add(
"replace,OLDSTRING,NEWSTRING [[,FILTER]] [[,GRID]]\n");J=true;}if(K==""||K=="remove"){ĝ.Add("--- Remove ---");ĝ.Add("Remove a string:");ĝ.Add(
"remove,STRING [[,FILTER]] [[,GRID]]\n");J=true;}if(K==""||K=="removenumbers"){ĝ.Add("--- Remove Numbers ---");ĝ.Add("Remove numbers from blocknames:");ĝ.Add(
"removenumbers [[,FILTER]] [[,GRID]]\n");J=true;}if(K==""||K=="defaultname"){ĝ.Add("--- Default Name ---");ĝ.Add(
"Sets blocknames to the default, localized name. For numbers, use autosort afterwards:");ĝ.Add("defaultname [[,FILTER]] [[,GRID]]\n");J=true;}if(K==""||K=="sort"){ĝ.Add("--- Sort ---");ĝ.Add(
"Create new continuous numbers:");ĝ.Add("sort,FILTER [[,GRID]]\n");ĝ.Add("Create new numbers based on the grid:");ĝ.Add("sortbygrid,FILTER\n");J=true;}
if(K==""||K=="autosort"){ĝ.Add("--- Autosort ---");ĝ.Add("Autosort all blocks on your grid with automatic numbers:");ĝ.Add
("autosort\n");ĝ.Add("Autosort all blocks on a specific grid:");ĝ.Add("autosort,GRID\n");ĝ.Add(
"Autosort every connected grid:");ĝ.Add("autosort,all\n");J=true;}if(K==""||K=="addfront"||K=="addback"){ĝ.Add("--- Add strings ---");ĝ.Add(
"Add a string at the front or back:");ĝ.Add("addfront,STRING [[,FILTER]] [[,GRID]]");ĝ.Add("addback,STRING [[,FILTER]] [[,GRID]]\n");J=true;}if(K==""||K==
"renamegrid"){ĝ.Add("--- Rename grid ---");ĝ.Add("Rename a grid:");ĝ.Add("renamegrid,NEWNAME [[,BLOCKONGRID]]\n");J=true;}if(K==""||
K=="copydata"){ĝ.Add("--- Copy Custom Data ---");ĝ.Add("Copy the custom data from BLOCK to all blocks matching FILTER:");
ĝ.Add("copydata,BLOCK,FILTER [[,GRID]]\n");J=true;}if(K==""||K=="deletedata"){ĝ.Add("--- Delete Custom Data ---");ĝ.Add(
"Delete the custom data of all blocks matching FILTER:");ĝ.Add("deletedata,FILTER [[,GRID]]\n");J=true;}if(K==""||K=="undo"){ĝ.Add("--- Undo last operation ---");ĝ.Add(
"Mistakes are made. Undo your last operation with this:");ĝ.Add("undo\n");J=true;}if(!J){ĝ.Add("--- Error ---");ĝ.Add("No topic with the given name exists!");}if(J){ĝ.Add(
"To skip parameters, use asterisk *\n");ĝ.Add("FILTER:");ĝ.Add("Can be either a part of a blockname");ĝ.Add("Or a group with the group token 'G:'");ĝ.Add(
"Or a type with the type token 'T:'\n");ĝ.Add("GRID:");ĝ.Add("A gridname filtering blocks that should be used.");ĝ.Add(
"Partial gridnames are also supported.\n");ĝ.Add("Additional parameters:");ĝ.Add("Test before renaming:\n"+testKeyword);ĝ.Add("Sort after renaming:\n"+
sortKeyword);}Ċ=true;}List<IMyTerminalBlock>I(string H,string[]G=null,string F="Debug",float E=0.6f,float D=2f){string C="[IsyLCD]"
;var B=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(B,Q=>Q.IsSameConstructAs(
Me)&&(Q.CustomName.Contains(H)||(Q.CustomName.Contains(C)&&Q.CustomData.Contains(H))));var A=B.FindAll(Q=>Q.CustomName.
Contains(H));foreach(var n in A){n.CustomName=n.CustomName.Replace(H,"").Replace(" "+H,"").TrimEnd(' ');bool l=false;bool k=
false;int j=0;if(n is IMyTextSurface){if(!n.CustomName.Contains(C))l=true;if(!n.CustomData.Contains(H)){k=true;n.CustomData=
"@0 "+H+(G!=null?"\n"+String.Join("\n",G):"");}}else if(n is IMyTextSurfaceProvider){if(!n.CustomName.Contains(C))l=true;int
h=(n as IMyTextSurfaceProvider).SurfaceCount;for(int g=0;g<h;g++){if(!n.CustomData.Contains("@"+g)){k=true;j=g;n.
CustomData+=(n.CustomData==""?"":"\n\n")+"@"+g+" "+H+(G!=null?"\n"+String.Join("\n",G):"");break;}}}else{B.Remove(n);}if(l)n.
CustomName+=" "+C;if(k){var U=(n as IMyTextSurfaceProvider).GetSurface(j);U.Font=F;U.FontSize=E;U.TextPadding=D;U.Alignment=
TextAlignment.LEFT;U.ContentType=ContentType.TEXT_AND_IMAGE;}}return B;}
}public static partial class f{private static Dictionary<char,float>m=new Dictionary<char,float>();public static void e(
string Z,float Y){foreach(char X in Z){m[X]=Y;}}public static void W(){if(m.Count>0)return;e(
"3FKTabdeghknopqsuy£µÝàáâãäåèéêëðñòóôõöøùúûüýþÿāăąďđēĕėęěĝğġģĥħĶķńņňŉōŏőśŝşšŢŤŦũūŭůűųŶŷŸșȚЎЗКЛбдекруцяёђћўџ",18);e("ABDNOQRSÀÁÂÃÄÅÐÑÒÓÔÕÖØĂĄĎĐŃŅŇŌŎŐŔŖŘŚŜŞŠȘЅЊЖф□",22);e("#0245689CXZ¤¥ÇßĆĈĊČŹŻŽƒЁЌАБВДИЙПРСТУХЬ€",20);e(
"￥$&GHPUVY§ÙÚÛÜÞĀĜĞĠĢĤĦŨŪŬŮŰŲОФЦЪЯжы†‡",21);e("！ !I`ijl ¡¨¯´¸ÌÍÎÏìíîïĨĩĪīĮįİıĵĺļľłˆˇ˘˙˚˛˜˝ІЇії‹›∙",9);e("？7?Jcz¢¿çćĉċčĴźżžЃЈЧавийнопсъьѓѕќ",17);e(
"（）：《》，。、；【】(),.1:;[]ft{}·ţťŧț",10);e("+<=>E^~¬±¶ÈÉÊË×÷ĒĔĖĘĚЄЏЕНЭ−",19);e("L_vx«»ĹĻĽĿŁГгзлхчҐ–•",16);e("\"-rª­ºŀŕŗř",11);e("WÆŒŴ—…‰",32);e("'|¦ˉ‘’‚",7)
;e("@©®мшњ",26);e("mw¼ŵЮщ",28);e("/ĳтэє",15);e("\\°“”„",13);e("*²³¹",12);e("¾æœЉ",29);e("%ĲЫ",25);e("MМШ",27);e("½Щ",30);
e("ю",24);e("ј",8);e("љ",23);e("ґ",14);e("™",31);}public static Vector2 V(this IMyTextSurface U,StringBuilder T){W();
Vector2 R=new Vector2();if(U.Font=="Monospace"){float S=U.FontSize;R.X=(float)(T.Length*19.4*S);R.Y=(float)(28.8*S);return R;}
else{float S=(float)(U.FontSize*0.779);foreach(char X in T.ToString()){try{R.X+=m[X]*S;}catch{}}R.Y=(float)(28.8*U.FontSize)
;return R;}}public static float ï(this IMyTextSurface n,StringBuilder T){Vector2 ð=n.V(T);return ð.X;}public static float
ï(this IMyTextSurface n,string T){Vector2 ð=n.V(new StringBuilder(T));return ð.X;}public static float î(this
IMyTextSurface n,char ì){float ë=ï(n,new string(ì,1));return ë;}public static int ê(this IMyTextSurface n){Vector2 é=n.SurfaceSize;
float è=n.TextureSize.Y;if(é.X<512||è!=é.Y)é.Y*=512/è;float í=é.Y*(100-n.TextPadding*2)/100;Vector2 ð=n.V(new StringBuilder(
"T"));return(int)(í/ð.Y);}public static float ø(this IMyTextSurface n){Vector2 é=n.SurfaceSize;float è=n.TextureSize.Y;if(é
.X<512||è!=é.Y)é.X*=512/è;return é.X*(100-n.TextPadding*2)/100;}public static StringBuilder ý(this IMyTextSurface n,char
ü,double û){int ú=(int)(û/î(n,ü));if(ú<0)ú=0;return new StringBuilder().Append(ü,ú);}private static DateTime þ=DateTime.
Now;private static Dictionary<int,List<int>>ù=new Dictionary<int,List<int>>();public static StringBuilder ö(this
IMyTextSurface n,StringBuilder T,int õ=3,bool ô=true,int ó=0){int ò=n.GetHashCode();if(!ù.ContainsKey(ò)){ù[ò]=new List<int>{1,3,õ,0};
}int ñ=ù[ò][0];int ç=ù[ò][1];int Ú=ù[ò][2];int Þ=ù[ò][3];var Ù=T.ToString().TrimEnd('\n').Split('\n');List<string>Ø=new
List<string>();if(ó==0)ó=n.ê();float Ö=n.ø();StringBuilder Õ,Ô=new StringBuilder();for(int g=0;g<Ù.Length;g++){if(g<õ||g<Ú||
Ø.Count-Ú>ó||n.ï(Ù[g])<=Ö){Ø.Add(Ù[g]);}else{try{Ô.Clear();float Ó,Ò;var Ñ=Ù[g].Split(' ');string É=System.Text.
RegularExpressions.Regex.Match(Ù[g],@"\d+(\.|\:)\ ").Value;Õ=n.ý(' ',n.ï(É));foreach(var Ð in Ñ){Ó=n.ï(Ô);Ò=n.ï(Ð);if(Ó+Ò>Ö){Ø.Add(Ô.
ToString());Ô=new StringBuilder(Õ+Ð+" ");}else{Ô.Append(Ð+" ");}}Ø.Add(Ô.ToString());}catch{Ø.Add(Ù[g]);}}}if(ô){if(Ø.Count>ó){
if(DateTime.Now.Second!=Þ){Þ=DateTime.Now.Second;if(ç>0)ç--;if(ç<=0)Ú+=ñ;if(Ú+ó-õ>=Ø.Count&&ç<=0){ñ=-1;ç=3;}if(Ú<=õ&&ç<=0)
{ñ=1;ç=3;}}}else{Ú=õ;ñ=1;ç=3;}ù[ò][0]=ñ;ù[ò][1]=ç;ù[ò][2]=Ú;ù[ò][3]=Þ;}else{Ú=õ;}StringBuilder Ï=new StringBuilder();for(
var Î=0;Î<õ;Î++){Ï.Append(Ø[Î]+"\n");}try{for(var Î=Ú;Î<Ø.Count;Î++){Ï.Append(Ø[Î]+"\n");}}catch{}return Ï;}public static
Dictionary<IMyTextSurface,string>Í(this IMyTerminalBlock p,string H,Dictionary<string,string>Û=null){var æ=new Dictionary<
IMyTextSurface,string>();if(p is IMyTextSurface){æ[p as IMyTextSurface]=p.CustomData;}else if(p is IMyTextSurfaceProvider){var å=
System.Text.RegularExpressions.Regex.Matches(p.CustomData,@"@(\d).*("+H+@")");int ä=(p as IMyTextSurfaceProvider).SurfaceCount
;foreach(System.Text.RegularExpressions.Match ã in å){int â=-1;if(int.TryParse(ã.Groups[1].Value,out â)){if(â>=ä)continue
;string z=p.CustomData;int á=z.IndexOf("@"+â);int à=z.IndexOf("@",á+1)-á;string y=à<=0?z.Substring(á):z.Substring(á,à);æ[
(p as IMyTextSurfaceProvider).GetSurface(â)]=y;}}}return æ;}public static bool ß(this string y,string Ü){var z=y.Replace(
" ","").Split('\n');foreach(var Î in z){if(Î.StartsWith(Ü+"=")){try{return Convert.ToBoolean(Î.Replace(Ü+"=",""));}catch{
return true;}}}return true;}public static string Ý(this string y,string Ü){var z=y.Replace(" ","").Split('\n');foreach(var Î
in z){if(Î.StartsWith(Ü+"=")){return Î.Replace(Ü+"=","");}}return"";}