/* =======================================================
      ATTENTION! All configuration is now done explicitly through the
      CUSTOM DATA of this Programmable Block. Editing this script may
      cause it to break! Do so at your own risk...
    ======================================================= */

// Defaults
int _efficiencySetting = 3;
string _inventoryGroupName = "Production Group";
string _lcdTag = "[IQueue]";
string _invLCDTag = "[IQ Inv]";
char _multiDisplayDelimeter = ':';
bool _useSpriteMode = true;
bool _useThisGrid = true;
bool _autoResize = true;
bool _autoDecon = true;
bool _showTools = false;
bool _showLegend = true;
bool _showResources = true;
int _scrollLineSkip = 1;

Dictionary<string,MyItemType>ǃ;Dictionary<MyItemType,Resource>Ǆ;Dictionary<Resource,MyItemType>ǅ;Dictionary<string,
MyItemType>ǆ;Dictionary<MyItemType,Ĥ>Ǉ;Dictionary<MyItemType,Ĥ>ǈ;Dictionary<MyItemType,Č>Ǌ;Dictionary<MyCubeSize,Dictionary<string
,ğ>>ǉ;Dictionary<string,float>ǂ=new Dictionary<string,float>();Dictionary<MyItemType,Resource>ǀ=new Dictionary<MyItemType
,Resource>();Dictionary<MyItemType,MyItemType>Ʈ=new Dictionary<MyItemType,MyItemType>();HashSet<IMyAssembler>Ư=new
HashSet<IMyAssembler>();HashSet<MyProductionItem>ư=new HashSet<MyProductionItem>();HashSet<MyDefinitionId>Ʊ=new HashSet<
MyDefinitionId>();List<string>Ʋ=new List<string>();List<string>Ƴ=new List<string>();List<MyIniKey>ƴ=new List<MyIniKey>();List<
IMyTextPanel>Ƶ=new List<IMyTextPanel>();List<œ>ƶ=new List<œ>();List<IMyRefinery>Ʒ=new List<IMyRefinery>();List<IMyAssembler>Ƹ=new
List<IMyAssembler>();List<IMyInventory>ƹ=new List<IMyInventory>();List<IMyCargoContainer>ƺ=new List<IMyCargoContainer>();
List<IMyTerminalBlock>ƻ=new List<IMyTerminalBlock>();List<MyProductionItem>Ƽ=new List<MyProductionItem>();List<
MyInventoryItem>ƽ=new List<MyInventoryItem>();List<IMyUpgradableBlock>ƾ=new List<IMyUpgradableBlock>();List<KeyValuePair<MyItemType,
float>>ƿ=new List<KeyValuePair<MyItemType,float>>();MyIni ů=new MyIni();StringBuilder ƭ=new StringBuilder();StringBuilder ǁ=
new StringBuilder();StringBuilder ǋ=new StringBuilder();StringBuilder ǣ=new StringBuilder();StringBuilder Ǥ=new
StringBuilder();StringBuilder ǥ=new StringBuilder();StringBuilder Ǧ=new StringBuilder();StringBuilder ǧ=new StringBuilder();
StringBuilder Ǩ=new StringBuilder();StringBuilder ǩ=new StringBuilder();StringBuilder Ǫ=new StringBuilder();IEnumerator<bool>Ŭ;
IEnumerator<bool>ǫ;IEnumerator<bool>ǭ;IEnumerator<bool>Ƿ;ũ Ǯ;IMyAssembler ǯ=null;string ǰ=null;string Ǳ=null;int ǲ=0;bool ǳ=false;
bool Ǵ=false;bool ǵ=false;bool Ƕ=false;bool Ǹ=false;bool Ǣ=false;bool ǌ=true;bool Ǡ=false;bool Ǎ=false;bool ǎ=true;bool Ǐ=
true;bool ǐ=true;bool Ǒ;char[]ǒ=new char[1]{','};char[]Ǔ=new char[1]{'='};char[]ǔ=new char[1]{':'};char[]Ǖ=new char[1]{'.'};
char[]ǖ=new char[2]{',','='};const string Ǘ="MyObjectBuilder_BlueprintDefinition/";const string ǘ=
"MyObjectBuilder_GasContainerObject/";const string Ǚ="MyObjectBuilder_OxygenContainerObject/";const string ǚ="Monospace";const string Ǜ="TransparentLCDSmall"
;const float ǜ=18944f/28.8f;const float ǝ=ǜ*2;const float Ǟ=ǜ*0.99375f;const char ǟ='\ue035';const char ǡ='\ue036';const
char Ǭ='\ue038';const char Ƭ='\ue037';const char Ƨ='\ue03a';void ƍ(string w,UpdateType y){l();var Ǝ=(y&UpdateType.Update100)
>0;var Ə=(y&UpdateType.Update10)>0;if(Ǵ&&Ǯ!=null){if(Ǯ.Š>=60&&g<=0.05)Ǎ=true;}else if(++ǲ>=60&&g<=0.05)Ǎ=true;if(Ŭ!=null)
{if(!Ŭ.MoveNext()){Ŭ.Dispose();Ŭ=null;}return;}if(w.Length>0)Ƒ(w);if(Ǹ||(Ǝ&&Ƕ)){if(ǎ&&ǌ){if(!Ɛ)È(ů);Ǳ=n(ů,Ǡ);if(Ǡ)Me.
CustomData=Ǳ;Ƕ=Ǹ=Ǡ=Ɛ=false;return;}else Ǹ=true;}if(Ǵ&&Ə){if(Ǯ==null)Ǯ=new ũ(this);else Ǯ.ē();}else{if(ǭ!=null){if(!ǵ){if(!ǭ.
MoveNext()){ǭ.Dispose();ǭ=null;}ǵ=ǭ.Current;}else if(Ƿ!=null){if(!Ƿ.MoveNext()){Ƿ.Dispose();Ƿ=null;}ǵ=Ƿ.Current;}else ǵ=false;}
if(Ư.Count>0&&ǫ!=null&&Ǎ){if(!ǫ.MoveNext()){ǫ.Dispose();ǫ=null;}}}if(Ǝ){if(!Ǳ.Equals(Me.CustomData,StringComparison.
OrdinalIgnoreCase)){Ƕ=true;Ǡ=true;}c();}}bool Ɛ=false;void Ƒ(string w){Ɛ=false;if(w.Equals("show tools",StringComparison.
OrdinalIgnoreCase)){_showTools=!_showTools;Ɛ=true;}else if(w.Equals("show legend",StringComparison.OrdinalIgnoreCase)){_showLegend=!
_showLegend;Ɛ=true;}else if(w.Equals("show resources",StringComparison.OrdinalIgnoreCase)){_showResources=!_showResources;Ɛ=true;}
else if(w.Equals("auto resize",StringComparison.OrdinalIgnoreCase)){_autoResize=!_autoResize;Ɛ=true;}else if(w.Equals(
"enable coop",StringComparison.OrdinalIgnoreCase)){ǐ=!ǐ;Ɛ=true;}else if(w.ToLower().Contains("line skip")){if(int.TryParse(w.Split(ǔ)
[1],out _scrollLineSkip)){_scrollLineSkip=Math.Max(1,_scrollLineSkip);Ɛ=true;}}Ǹ=Ǡ=Ɛ;}IEnumerator<bool>ƒ(){while(true){ǳ=
true;foreach(var Ú in Ư){if(Ú.Ĭ(this))continue;var â=Ú.InputInventory;IMyInventory Ɠ;if(â.IsFull||(float)â.CurrentVolume/(
float)â.MaxVolume>0.9f)Ɠ=â;else Ɠ=Ú.OutputInventory;for(int Y=ƺ.Count-1;Y>=0;Y--){if(Ɠ.ItemCount==0)break;var ì=ƺ[Y];if(ì.Ĭ(
this)){ƺ.RemoveAtFast(Y);continue;}var ą=ì.GetInventory(0);if(ą.IsFull)continue;while(Ɠ.ItemCount>0){if(ą.IsFull||Ɠ.
ItemCount==0)break;int Ɣ=Ɠ.ItemCount-1;if(!Ɠ.IsItemAt(Ɣ))break;ą.TransferItemFrom(Ɠ,Ɣ,null,true,Ɠ.GetItemAt(Ɣ)?.Amount);ǲ=0;if(Ǯ
!=null)Ǯ.Š=0;Ǎ=false;yield return true;}}yield return true;}Ư.Clear();ǳ=false;yield return true;}}void ƕ(œ Ɩ,float Ɨ,int ƙ
,int Ƙ,StringSegment Ƃ,List<string>ƃ){var ż=(Ƙ-20)/2;ƭ.Clear().Append(' ').Append('-',Math.Max(0,ż-1)).Append(
" Inventory Mgr Issues ").Append('-',Math.Max(0,Ƙ-20-ż-1)).Append('\n');int Ž=0;if(Ƃ.Text.Length==0){Ɩ.ŕ.ClearImagesFromSelection();ƭ.Append(
$" {ǡ.ToString()}None");Ž=20;if(Ɩ.Ř)Ɨ=Ɩ.ŕ.CubeGrid.GridSize>1?2.5f:1.5f;else Ɨ=0.7f;}else{Ɩ.ŕ.ClearImagesFromSelection();if(Ǣ)Ɩ.ŕ.
AddImageToSelection("Danger");ƙ-=2;if(ƙ>=ƃ.Count)Ɩ.Ŏ=0;for(int Y=0;Y<ƙ;Y++){if(Y>=ƃ.Count)break;var ž=(Ɩ.Ŏ+Y)%ƃ.Count;var ſ=ƃ[ž];if(ſ.
Length>Ž)Ž=ſ.Length;ƭ.Append(' ').Append(ǟ).Append(ſ).Append('\n');}Ɩ.Ŏ+=Math.Max(1,Math.Min(ƙ,_scrollLineSkip));if(Ɩ.Ŏ>ƃ.
Count)Ɩ.Ŏ=0;}if(_autoResize&&(Ž>Ƙ||Ɨ<0.7f)){float Ũ;if(Ɩ.Ř)Ũ=Ɩ.ŕ.CubeGrid.GridSize>1?2.5f:1.5f;else Ũ=0.7f;Ɨ=Math.Max(0.5f,
Math.Min(Ũ,Ɨ*Ƙ/Ž));}Ȗ(Ɩ.ŕ,Ɨ,ƭ);}void ƀ(œ Ɓ,StringSegment Ƃ,List<string>ƃ){var Ƅ=Ɓ.Ś;var ƅ=Ƅ.ŋ;var Ɔ=Ƅ.ō;var Ƈ=Ƅ.Ō;var ŏ=Ƅ.Ń;
var ƈ=ŏ.DrawFrame();var Ɖ=Math.Min(Ƅ.ŋ.X,512f)/512f;var Ɗ=ƅ-1;var Ƌ=Ƈ.Y;Vector2 Ż,ƌ;Ɖ*=(Ɓ.Ř||Ɓ.ř)?0.5f:0.7f;if(ȳ%2==0)ɍ(ƈ);
Ʉ(ŏ);ȸ(ȶ,ref Ɔ,ref Ɗ,ref Ⱦ,ref ƈ);if(Ƃ.Text.Length>0&&Ǣ){Ɗ*=0.8f;ȸ("Danger",ref Ɔ,ref Ɗ,ref Ȱ,ref ƈ);}ȴ.Clear().Append(
"M");Ż=ŏ.MeasureStringInPixels(ȴ,ȵ,Ɖ);ƌ=new Vector2(Ɔ.X,Ƈ.Y);Ⱥ("Inventory Manager Issues",ȵ,ref Ɖ,ref ƌ,ref ƈ,ref ȯ);Ɗ=new
Vector2(ƅ.X-10,2);ƌ=new Vector2(Ɔ.X,Ƈ.Y+Ż.Y+2);ȸ(ȶ,ref ƌ,ref Ɗ,ref ȯ,ref ƈ);Ɗ=new Vector2(Math.Min(Ż.X,Ż.Y))*0.9f;if(Ƃ.Text.
Length==0){ƌ=new Vector2(Ƈ.X+5,Ƈ.Y+Ż.Y+4)+Ż*0.5f;ȸ(ȷ,ref ƌ,ref Ɗ,ref Ȳ,ref ƈ,MathHelper.PiOver2);ƌ=new Vector2(Ƈ.X+Ɗ.X+10,Ƈ.Y+
Ż.Y+2);Ⱥ("None",ȵ,ref Ɖ,ref ƌ,ref ƈ,ref ȯ,TextAlignment.LEFT);ƈ.Dispose();return;}var ƚ=ƅ.Y-Ż.Y-4;var ƣ=ƃ.Count*Ż.Y;Ƌ+=Ż.
Y+2;if(ƣ<=ƚ)Ɓ.Ŏ=0;for(int Y=0;Y<ƃ.Count;Y++){if(Ƌ+Ż.Y>Ƈ.Y+ƅ.Y){if(Y<ƃ.Count-1)Ɓ.Ŏ+=_scrollLineSkip;if(Ɓ.Ŏ>ƃ.Count)Ɓ.Ŏ=0;
break;}var ž=(Ɓ.Ŏ+Y)%ƃ.Count;var Ƥ=ƃ[ž];ƌ=new Vector2(Ƈ.X+5,Ƌ+0.5f)+Ż*0.5f;ȸ(ȷ,ref ƌ,ref Ɗ,ref ȼ,ref ƈ,MathHelper.PiOver2);ƌ=
new Vector2(Ƈ.X+Ɗ.X+10,Ƌ);Ⱥ(Ƥ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref ȯ,TextAlignment.LEFT);Ƌ+=Ż.Y;}ƈ.Dispose();}IEnumerator<bool>ƥ(){while
(true){ǌ=false;StringSegment Ƃ=new StringSegment();if(Ǵ&&Ǯ!=null){Ƃ=new StringSegment(Ǯ.ş??"");Ƃ.GetLines(Ƴ);Ǣ=!Ǣ;}
foreach(var Ʀ in ɇ)ɉ.Push(Ʀ);foreach(var Ʀ in Ɇ)ɉ.Push(Ʀ);foreach(var Ʀ in Ʌ)ɉ.Push(Ʀ);Ʌ.Clear();ɇ.Clear();Ɇ.Clear();Ɉ.Clear();
yield return true;Ɉ.Add(Ɋ);int Ė=0;foreach(var s in Ǌ.Values){if(s.Ĳ==0||(!_showTools&&s.ħ==ItemType.TOOL))continue;s.ĵ=false
;foreach(var Í in ư){if(Í.BlueprintId==s.ĸ){s.ĵ=true;break;}}s.Ķ=!s.ĵ&&Ʊ.Contains(s.ĸ);float Ɲ=s.Ğ;float ƨ=s.Ĳ;float Ʃ=Ɲ/
ƨ;var ƪ=ȋ(Ʃ);var Ɵ=ɉ.Count>0?ɉ.Pop():new MyTuple<string,string,string,string,MySprite?,Color?>();Ɵ.Item1=s.ĥ;Ɵ.Item2=Ɲ.
ToString();Ɵ.Item3=ƨ.ToString();Ɵ.Item4=null;Ɵ.Item5=Ȯ(s);Ɵ.Item6=ƪ;if(s.Ĳ>s.Ğ)Ɇ.Add(Ɵ);else Ʌ.Add(Ɵ);if(++Ė>15){Ė=0;yield
return true;}}if(_showResources){foreach(var N in Ǉ){if(N.Value.ħ==ItemType.ORE||N.Key.SubtypeId.StartsWith("Scrap"))continue;
var F=N.Key.SubtypeId.Replace("OreToIngot","");var Ɵ=ɉ.Count>0?ɉ.Pop():new MyTuple<string,string,string,string,MySprite?,
Color?>();Ɵ.Item5=null;Ɵ.Item6=null;MyItemType ƫ=MyItemType.MakeIngot(F);Ĥ Ë;if(ǈ.TryGetValue(ƫ,out Ë)){var Ƣ=ǆ["Stone"];var
ƛ=Ǆ[ƫ];float ơ=0f;if(ƫ!=Ƣ){ơ=ƛ==Resource.CUSTOM?0f:Ë.Ĩ[0].į;if(ƛ==Resource.IRON||ƛ==Resource.NICKEL||ƛ==Resource.SILICON)
{foreach(var Ó in ǈ[Ƣ].Ĩ){if(Ó.ļ==ƛ){ơ+=Ó.į;break;}}}}else{foreach(var Ó in ǈ[Ƣ].Ĩ){if(Ó.ļ==Resource.STONE){ơ=Ó.į;break;}
}}var Ɯ=Math.Max(0,N.Value.Ħ);var Ɲ=Ë.Ħ+ơ;var ƞ=Math.Max(0,Ɯ-Ɲ);if(ƞ==0)continue;Ǒ=true;Ɵ.Item1=Ë.ĥ;Ɵ.Item2=Ɯ.ToString();
Ɵ.Item3=Ɲ.ToString();Ɵ.Item4=ƞ.ToString();}else if(ǈ.TryGetValue(N.Key,out Ë)){var Ɯ=Math.Max(0,N.Value.Ħ);var Ɲ=Ë.Ħ;var
ƞ=Math.Max(0,Ɯ-Ɲ);if(ƞ==0)continue;Ǒ=true;Ɵ.Item1=Ë.ĥ;Ɵ.Item2=Ɯ.ToString();Ɵ.Item3=Ɲ.ToString();Ɵ.Item4=ƞ.ToString();}
else{Ǒ=true;var Ɯ=Math.Max(0,N.Value.Ħ);Ɵ.Item1=N.Value.ĥ;Ɵ.Item2=Ɯ.ToString();Ɵ.Item3="0";Ɵ.Item4=Ɵ.Item2;}ɇ.Add(Ɵ);if(++Ė>
15){Ė=0;yield return true;}}}foreach(var Ɵ in Ʌ)Ɉ.Add(Ɵ);foreach(var Ɵ in Ɇ)Ɉ.Add(Ɵ);if(_showLegend){Ɉ.Add(Ɍ);Ɉ.Add(Ɏ);}if
(_showResources&&Ǒ){Ɉ.Add(ɋ);foreach(var Ɵ in ɇ)Ɉ.Add(Ɵ);}++ȳ;for(int Y=ƶ.Count-1;Y>=0;Y--){var Ơ=ƶ[Y];if(!Ơ.Ŝ){ƶ.
RemoveAtFast(Y);continue;}if(Ơ.ŗ&&Ǵ){ƀ(Ơ,Ƃ,Ƴ);continue;}var Ƅ=Ơ.Ś;var ƅ=Ƅ.ŋ;var Ɔ=Ƅ.ō;var Ƈ=Ƅ.Ō;var ŏ=Ƅ.Ń;var ƈ=ŏ.DrawFrame();var Ɖ=
Math.Min(Math.Max(ƅ.X,ƅ.Y),512f)/512f;var Ɗ=ƅ-1;var Ȧ=ȯ;Vector2 Ż,ƌ;float ȧ;if(_autoResize){ȴ.Clear().Append("M");Ż=ŏ.
MeasureStringInPixels(ȴ,ȵ,Ɖ);var ģ=Ơ.ņ?ƅ.Y*2:ƅ.Y;ȧ=Ɉ.Count*Ż.Y+20;if(_showLegend)ȧ+=Ż.Y*0.5f+4;if(_showResources&&Ǒ)ȧ+=Ż.Y*0.5f+4;var Ȩ=ģ/ȧ;
if(Ơ.ř)Ɖ=MathHelper.Clamp(Ȩ,0.5f,1f);else Ɖ=MathHelper.Clamp(Ȩ,0.3f,1.1f);}else Ɖ*=(Ơ.Ř||Ơ.ř)?0.5f:0.7f;if(ȳ%2==0)ɍ(ƈ);Ʉ(ŏ
);ȸ(ȶ,ref Ɔ,ref Ɗ,ref Ⱦ,ref ƈ);ȴ.Clear().Append("M");Ż=ŏ.MeasureStringInPixels(ȴ,ȵ,Ɖ);var Ƌ=Ƈ.Y;bool ȩ=false;ȧ=Ɉ.Count*Ż.
Y+4;if(_showLegend)ȧ+=Ż.Y*0.5f+4;if(_showResources&&Ǒ)ȧ+=Ż.Y*0.5f+4;var Ȫ=Ơ.ņ?ƅ.Y*2:ƅ.Y;if(ȧ<=Ȫ){Ơ.Ŏ=0;if(Ơ.ņ&&ȧ<=ƅ.Y)Ơ.ś
?.Ń.DrawFrame().Dispose();}bool ȫ=false;for(int K=0;K<Ɉ.Count;K++){if(Ƌ+Ż.Y>Ƈ.Y+ƅ.Y){if(Ơ.ņ&&!ȫ){ȫ=true;ƈ.Dispose();Ƌ=Ƈ.Y
;Ƅ=Ơ.ś;ŏ=Ƅ.Ń;ƈ=ŏ.DrawFrame();Ʉ(ŏ);}else{if(K<Ɉ.Count-1)Ơ.Ŏ+=_scrollLineSkip;if(Ơ.Ŏ>Ɉ.Count)Ơ.Ŏ=0;break;}}if(ȩ){ȩ=false;Ɗ=
new Vector2(ƅ.X-10,2);ƌ=new Vector2(Ɔ.X,Ƌ+2);ȸ(ȶ,ref ƌ,ref Ɗ,ref ȯ,ref ƈ);Ƌ+=Ɗ.Y+2;}var ž=(Ơ.Ŏ+K)%Ɉ.Count;var Ɵ=Ɉ[ž];var Ȣ=
Ɵ.Item1;bool Ț=false,Ȭ=false,ȭ=false;if(Ȣ=="Legend (%)")Ț=true;else if(Ȣ=="Component Name")Ȭ=true;else if(Ȣ==
"Resource Name")ȭ=true;if(Ț||Ȭ||ȭ){ȩ=true;if(K>0)Ƌ+=Ż.Y*0.5f;}ș(Ɵ,Ț,Ȭ,ȭ,Ƅ,ref ƈ,ref Ɖ,ref Ƌ,ref Ȧ);}ƈ.Dispose();yield return true;}
foreach(var N in Ǉ.Values){N.Ħ=0;foreach(var Ó in N.Ĩ)Ó.į=0;}foreach(var N in ǈ.Values)N.Ħ=0f;yield return true;foreach(var s
in Ǌ.Values)s.Ğ=0;ǌ=true;yield return false;}}MySprite?Ȯ(Č s){if(!s.ĵ&&!s.Ķ)return null;var ǿ=new MySprite(SpriteType.
TEXTURE,ȷ);if(s.Ķ){ǿ.RotationOrScale=MathHelper.Pi;ǿ.Color=ȼ;}else ǿ.Color=ȱ;return ǿ;}void ș(MyTuple<string,string,string,
string,MySprite?,Color?>Ɵ,bool Ț,bool ț,bool Ȝ,Ł µ,ref MySpriteDrawFrame ƈ,ref float Ɖ,ref float ȝ,ref Color Ȟ){bool ȟ=false;
bool Ƞ=false;bool ȡ=false;var Ȣ=Ɵ.Item1;if(!Ț&&!ț&&!Ȝ){if(Ɵ.Item4==null&&Ɵ.Item6!=null)Ƞ=true;else if(Ȣ.StartsWith("0-33"))ȟ
=true;else ȡ=true;}ȴ.Clear().Append("M");var ȣ=µ.Ń.MeasureStringInPixels(ȴ,ȵ,Ɖ);var Ƈ=µ.Ō.X;var ƅ=µ.ŋ;var Ȥ=ȝ;var ƌ=new
Vector2(Ƈ+5,Ȥ);Vector2 Ɗ;if(Ț){Ⱥ(Ȣ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT);}else if(ț){Ɗ=new Vector2(Math.Min(ȣ.X,ȣ.Y))*
0.7f;ƌ+=Ɗ*0.5f;ȸ(ȷ,ref ƌ,ref Ɗ,ref ȱ,ref ƈ);ƌ+=new Vector2(0,Ɗ.Y+1);ȸ(ȷ,ref ƌ,ref Ɗ,ref ȼ,ref ƈ,MathHelper.Pi);ƌ=new Vector2
(Ƈ+5+ȣ.X,Ȥ);Ⱥ(Ȣ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT);ƌ=new Vector2(Ƈ+ƅ.X*ɂ,Ȥ);Ⱥ(Ɵ.Item2,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref
Ȟ,TextAlignment.RIGHT);ƌ=new Vector2(Ƈ+ƅ.X*Ƀ,Ȥ);Ⱥ("%",ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT);ƌ=new Vector2(ƅ.X-5,Ȥ
);Ⱥ(Ɵ.Item3,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);}else if(Ȝ){Ⱥ(Ȣ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT);
ƌ=new Vector2(Ƈ+ƅ.X*Ɂ,Ȥ);Ⱥ(Ɵ.Item2,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);ƌ=new Vector2(Ƈ+ƅ.X*ɀ,Ȥ);Ⱥ(Ɵ.Item3,ȵ,
ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);ƌ=new Vector2(ƅ.X-5,Ȥ);Ⱥ(Ɵ.Item4,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT
);}else if(ȟ){ȴ.Clear().Append(Ȣ);var ȥ=µ.Ń.MeasureStringInPixels(ȴ,ȵ,Ɖ);Ⱥ(Ȣ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT
);Ɗ=new Vector2(Math.Min(ȣ.X,ȣ.Y));ƌ+=new Vector2(ȥ.X+5,0)+ȣ*0.5f;ȸ(ȶ,ref ƌ,ref Ɗ,ref ȼ,ref ƈ);ƌ=new Vector2(Ƈ+ƅ.X*0.34f,
Ȥ);Ⱥ(Ɵ.Item2,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);ƌ+=new Vector2(5,0)+ȣ*0.5f;ȸ(ȶ,ref ƌ,ref Ɗ,ref Ȱ,ref ƈ);ƌ=new
Vector2(Ƈ+ƅ.X*0.67f,Ȥ);Ⱥ(Ɵ.Item3,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);ƌ+=new Vector2(5,0)+ȣ*0.5f;ȸ(ȶ,ref ƌ,ref Ɗ,ref
ȱ,ref ƈ);ƌ=new Vector2(ƅ.X-ȣ.X-10,Ȥ);Ⱥ(Ɵ.Item4,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);ƌ=new Vector2(ƅ.X-5,Ȥ)+new
Vector2(ȣ.X*-0.5f,ȣ.Y*0.5f);ȸ(ȶ,ref ƌ,ref Ɗ,ref Ȳ,ref ƈ);}else if(Ƞ){if(Ɵ.Item5.HasValue){var ǿ=Ɵ.Item5.Value;Ɗ=new Vector2(
Math.Min(ȣ.X,ȣ.Y))*0.7f;ƌ+=new Vector2(Ɗ.X*0.5f,ȣ.Y*0.5f);ǿ.Size=Ɗ;ǿ.Position=ƌ;ƈ.Add(ǿ);}ƌ=new Vector2(Ƈ+ȣ.X+5,Ȥ);Ⱥ(Ȣ,ȵ,ref
Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT);var º=float.Parse(Ɵ.Item2);var ȿ=Ȕ(ref º);ƌ=new Vector2(Ƈ+ƅ.X*ɂ-ȣ.X-2.5f,Ȥ);Ⱥ(º.
ToString(),ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);if(ȿ!=null){ƌ+=new Vector2(ȣ.X+2.5f,0);Ⱥ(ȿ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,
TextAlignment.RIGHT);}ƌ=new Vector2(Ƈ+ƅ.X*Ƀ,Ȥ+0.5f)+ȣ*0.5f;Ɗ=new Vector2(Math.Min(ȣ.X,ȣ.Y));var ƪ=Ɵ.Item6.Value;ȸ(ȶ,ref ƌ,ref Ɗ,ref ƪ
,ref ƈ);º=string.IsNullOrWhiteSpace(Ɵ.Item3)?0:float.Parse(Ɵ.Item3);ȿ=Ȕ(ref º);ƌ=new Vector2(ƅ.X-ȣ.X-7.5f,Ȥ);Ⱥ(º.ToString
(),ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);if(ȿ!=null){ƌ+=new Vector2(ȣ.X+2.5f,0);Ⱥ(ȿ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,
TextAlignment.RIGHT);}}else if(ȡ){Ⱥ(Ȣ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.LEFT);var º=float.Parse(Ɵ.Item2);var ȿ=Ȕ(ref º);ƌ=new
Vector2(Ƈ+ƅ.X*Ɂ-ȣ.X-2.5f,Ȥ);Ⱥ(º.ToString(),ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);if(ȿ!=null){ƌ+=new Vector2(ȣ.X+2.5f,0
);Ⱥ(ȿ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);}º=string.IsNullOrWhiteSpace(Ɵ.Item3)?0:float.Parse(Ɵ.Item3);ȿ=Ȕ(ref
º);ƌ=new Vector2(Ƈ+ƅ.X*ɀ-ȣ.X-2.5f,Ȥ);Ⱥ(º.ToString(),ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);if(ȿ!=null){ƌ+=new
Vector2(ȣ.X+2.5f,0);Ⱥ(ȿ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);}º=string.IsNullOrWhiteSpace(Ɵ.Item4)?0:float.Parse(Ɵ.
Item4);ȿ=Ȕ(ref º);ƌ=new Vector2(ƅ.X-ȣ.X-7.5f,Ȥ);Ⱥ(º.ToString(),ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);if(ȿ!=null){ƌ+=
new Vector2(ȣ.X+2.5f,0);Ⱥ(ȿ,ȵ,ref Ɖ,ref ƌ,ref ƈ,ref Ȟ,TextAlignment.RIGHT);}}ȝ+=ȣ.Y;}float ɀ=0.75f;float Ɂ=0.5f;float ɂ=
0.75f;float Ƀ=0.765f;void Ʉ(IMyTextSurface ŏ){ŏ.ScriptBackgroundColor=Ⱦ;ŏ.ContentType=ContentType.SCRIPT;ŏ.Script=null;}void
ɍ(MySpriteDrawFrame ƈ)=>ƈ.Add(new MySprite());List<MyTuple<string,string,string,string,MySprite?,Color?>>Ʌ=new List<
MyTuple<string,string,string,string,MySprite?,Color?>>();List<MyTuple<string,string,string,string,MySprite?,Color?>>Ɇ=new List<
MyTuple<string,string,string,string,MySprite?,Color?>>();List<MyTuple<string,string,string,string,MySprite?,Color?>>ɇ=new List<
MyTuple<string,string,string,string,MySprite?,Color?>>();List<MyTuple<string,string,string,string,MySprite?,Color?>>Ɉ=new List<
MyTuple<string,string,string,string,MySprite?,Color?>>();Stack<MyTuple<string,string,string,string,MySprite?,Color?>>ɉ=new
Stack<MyTuple<string,string,string,string,MySprite?,Color?>>();MyTuple<string,string,string,string,MySprite?,Color?>Ɋ=new
MyTuple<string,string,string,string,MySprite?,Color?>("Component Name","On Hand","Quota",null,null,null);MyTuple<string,string,
string,string,MySprite?,Color?>ɋ=new MyTuple<string,string,string,string,MySprite?,Color?>("Resource Name","Needed","On Hand",
"Missing",null,null);MyTuple<string,string,string,string,MySprite?,Color?>Ɍ=new MyTuple<string,string,string,string,MySprite?,
Color?>("Legend (%)",null,null,null,null,null);MyTuple<string,string,string,string,MySprite?,Color?>Ɏ=new MyTuple<string,
string,string,string,MySprite?,Color?>("0-33:","34-66:","67-99:","100+:",null,null);Color Ⱦ,ȯ,ȼ,Ȱ,ȱ,Ȳ;int ȳ;StringBuilder ȴ=
new StringBuilder(64);const string ȵ="Debug";const string ȶ="SquareSimple";const string ȷ="Triangle";void ȸ(string L,ref
Vector2 ƌ,ref Vector2 Ɗ,ref Color ƪ,ref MySpriteDrawFrame ƈ,float ȹ=0f){var ǿ=new MySprite(SpriteType.TEXTURE,L,ƌ,Ɗ,ƪ,rotation:
ȹ);ƈ.Add(ǿ);}void Ⱥ(string ȗ,string Ȼ,ref float Ƚ,ref Vector2 ƌ,ref MySpriteDrawFrame ƈ,ref Color ƪ,TextAlignment Ǿ=
TextAlignment.CENTER){var ǿ=new MySprite(SpriteType.TEXT,ȗ,ƌ,null,ƪ,Ȼ,Ǿ,Ƚ);ƈ.Add(ǿ);}IEnumerator<bool>Ȁ(){while(true){ǌ=false;
StringSegment Ƃ=new StringSegment();if(Ǵ&&Ǯ!=null){Ƃ=new StringSegment(Ǯ.ş??"");Ƃ.GetLines(Ƴ);Ǣ=!Ǣ;}for(int Y=ƶ.Count-1;Y>=0;Y--){var
Ơ=ƶ[Y];if(!Ơ.Ŝ){ƶ.RemoveAtFast(Y);continue;}Dictionary<string,ğ>ȁ;ǉ.TryGetValue(Ơ.ŕ.CubeGrid.GridSizeEnum,out ȁ);if(ȁ==
null)throw new Exception($"There was no dictionary for 'GridSizeEnum.{Ơ.ŕ.CubeGrid.GridSizeEnum}'!");var D=Ơ.ŕ.
BlockDefinition.SubtypeName;ğ í;if(!ȁ.TryGetValue(D,out í))continue;float Ɨ=Ơ.ŕ.FontSize;if(!Ơ.ŗ&&í.Ġ<=ǜ)Ɨ=MathHelper.Clamp(Ɨ,0.3f,
0.55f);else Ɨ=MathHelper.Clamp(Ɨ,0.3f,Ơ.Ř?2.5f:1.1f);float Ɖ=1.0f/Ɨ;float Ȃ=í.ġ/37f;int ƙ=(int)(Ȃ*Ɖ);int Ƙ=(int)(í.Ġ*0.04f*Ɖ)
-2;if(Ơ.ŗ){if(Ǵ&&!Ƃ.IsEmpty){ƕ(Ơ,Ɨ,ƙ,Ƙ,Ƃ,Ƴ);yield return true;}continue;}if(Ơ.ņ)ƙ*=2;string ȃ=(-Ƙ+20).ToString();string Ȅ
=(-Ƙ+32).ToString();int ȅ=(int)Math.Round((Ƙ-30)*0.333333343f);yield return true;string Ȇ=$" {{0,{ȃ}}}{{1,-15}}{{2}}";
string ȇ=$"{{0,{ȃ}}}{{1,9}}{{2,11}}\n";string ȉ=$" {{0,{Ȅ}}}{{1,-12}}{{2,-13}}{{3}}";string Ȉ=
$" {{0,{Ȅ}}}{{1,6}}{{2,13}}{{3,13}}\n";string ǽ=$" {{0,{-7-ȅ}}}{{1,{-8-ȅ}}}{{2,{-8-ȅ}}}{{3}}\n";Ǩ.Clear();ǩ.Clear().AppendFormat(Ȇ,"Component Name",
"On Hand %","Quota\n ").Append('-',Ƙ).Append('\n');Ǫ.Clear();if(_showLegend){Ǧ.Clear().Append(" Legend (%)\n ").Append('-',Ƙ).
Append('\n').AppendFormat(ǽ,$"0-33: {ǟ}",$"34-66: {Ǭ}",$"67-99: {Ƨ}",$"100+: {ǡ}");}yield return true;int Ė=0;foreach(var N in
Ǉ){if(N.Value.ħ==ItemType.ORE||N.Key.SubtypeId.StartsWith("Scrap"))continue;var F=N.Key.SubtypeId.Replace("OreToIngot",""
);MyItemType ƫ=MyItemType.MakeIngot(F);Ĥ Ë;if(ǈ.TryGetValue(ƫ,out Ë)){var ĭ=Ë.ĥ;var Ƣ=ǆ["Stone"];var ƛ=Ǆ[ƫ];float ơ=0f;if
(ƫ!=Ƣ){ơ=ƛ==Resource.CUSTOM?0f:Ë.Ĩ[0].į;if(ƛ==Resource.IRON||ƛ==Resource.NICKEL||ƛ==Resource.SILICON){foreach(var Ó in ǈ[
Ƣ].Ĩ){if(Ó.ļ==ƛ){ơ+=Ó.į;break;}}}}else{foreach(var Ó in ǈ[Ƣ].Ĩ){if(Ó.ļ==Resource.STONE){ơ=Ó.į;break;}}}var Ɯ=Math.Max(0,N
.Value.Ħ);var Ɲ=Ë.Ħ+ơ;var ƞ=Math.Max(0,Ɯ-Ɲ);if(ƞ==0)continue;Ǒ=true;var ǹ=ȕ(Ɯ);var Ǻ=ȕ(Ɲ);var ǻ=ȕ(ƞ);Ǫ.AppendFormat(Ȉ,ĭ,ǹ
,Ǻ,ǻ);}else{Ǒ=true;var Ɯ=Math.Max(0,N.Value.Ħ);var ǹ=ȕ(Ɯ);Ǫ.AppendFormat($" {ȇ}",N.Value.ĥ,null,ǹ);}if(++Ė>15){Ė=0;yield
return true;}}foreach(var s in Ǌ.Values){if(s.Ĳ==0||(!_showTools&&s.ħ==ItemType.TOOL))continue;s.ĵ=false;foreach(var Í in ư){
if(Í.BlueprintId==s.ĸ){s.ĵ=true;break;}}s.Ķ=!s.ĵ&&Ʊ.Contains(s.ĸ);char Ǽ=s.ĵ?Ƭ:s.Ķ?ǟ:' ';float Ɲ=s.Ğ;float ƨ=s.Ĳ;float Ʃ=Ɲ
/ƨ;char ƪ=Ȍ(Ʃ);var Ǻ=$"{ȕ(Ɲ)} {ƪ.ToString()}";var Ȑ=ȕ(ƨ);if(s.Ĳ>s.Ğ)ǩ.AppendFormat($"{Ǽ.ToString()}{ȇ}",s.ĥ,Ǻ,Ȑ);else Ǩ.
AppendFormat($"{Ǽ.ToString()}{ȇ}",s.ĥ,Ǻ,Ȑ);if(++Ė>15){Ė=0;yield return true;}}yield return true;ǩ.Append(Ǩ).Append('\n');if(
_showLegend){ǩ.Append(Ǧ);if(_showResources)ǩ.Append('\n');yield return true;}if(_showResources&&Ǒ){ǩ.AppendFormat(ȉ,"Resource",
"Needed","On Hand","Missing\n ").Append('-',Ƙ).Append('\n').Append(Ǫ.ToString());}yield return true;StringSegment ȑ=new
StringSegment(ǩ.ToString());ȑ.GetLines(Ʋ);if(_autoResize){float Ȓ=Ȃ/(Ơ.ņ?Ʋ.Count+1:Ʋ.Count);int ȓ=1;if(Ơ.ņ){Ȓ*=2;ȓ=2;}if(í.Ġ<=ǜ)Ɨ=
MathHelper.Clamp(Ȓ,0.3f,0.55f);else Ɨ=MathHelper.Clamp(Ȓ,0.3f,1.1f);ƙ=(int)(Ȃ/Ɨ)*ȓ;}yield return true;ȍ(ƙ,Ʋ,Ơ,ǧ);int ȏ=(int)(ƙ*
0.5f);if(Ơ.ņ&&Ʋ.Count>ȏ){ȑ=new StringSegment(ǧ.ToString());ȑ.GetLines(Ʋ);Ș(Ơ,Ɨ,ȏ,Ʋ);}else{Ȗ(Ơ.ŕ,Ɨ,ǧ);if(Ơ.ņ&&Ơ.Ŗ?.GetText().
Length>0)Ơ.Ŗ.WriteText("");}yield return true;}foreach(var N in Ǉ.Values){N.Ħ=0;foreach(var Ó in N.Ĩ)Ó.į=0;}foreach(var N in ǈ
.Values)N.Ħ=0f;yield return true;foreach(var s in Ǌ.Values)s.Ğ=0;ǌ=true;yield return false;}}string Ȕ(ref float º){if(º>=
100000000){º=(float)Math.Round(º*0.000001f);return"M";}if(º>=1000000){º=(float)Math.Round(º*0.000001f,1);return"M";}if(º>=100000)
{º=(float)Math.Round(º*0.001f);return"k";}if(º>=1000){º=(float)Math.Round(º*0.001f,1);return"k";}º=(float)Math.Round(º,1)
;return null;}string ȕ(float º){if(º>=100000000)return$"{(º*0.000001).ToString("0")} M";if(º>=1000000)return
$"{(º*0.000001).ToString("0.#")} M";if(º>=100000)return$"{(º*0.001).ToString("0")} k";if(º>=1000)return$"{(º*0.001).ToString("0.#")} k";return
$"{Math.Round(º).ToString()}  ";}void Ȗ(IMyTextPanel í,float Ɨ,StringBuilder ȗ){í.Font=ǚ;í.FontSize=Ɨ;í.WriteText(ȗ);í.ContentType=ContentType.
TEXT_AND_IMAGE;í.TextPadding=(í.BlockDefinition.SubtypeId==Ǜ)?4.5f:0;}void Ș(œ µ,float Ɨ,int ȏ,List<string>Ȋ){ǋ.Clear();ǣ.Clear();int
º=ȏ-1;for(int Y=0;Y<Ȋ.Count;Y++){if(Y<=º)ǋ.Append(Ȋ[Y]).Append('\n');else ǣ.Append(Ȋ[Y]).Append('\n');}Ȗ(µ.ŕ,Ɨ,ǋ);Ȗ(µ.Ŗ,Ɨ
,ǣ);}Color ȋ(float º){if(º>=1f)return Ȳ;if(º>0.66f)return ȱ;if(º>0.33f)return Ȱ;return ȼ;}char Ȍ(float º){if(º>=1f)return
ǡ;if(º>0.66f)return Ƨ;if(º>0.33f)return Ǭ;return ǟ;}void ȍ(int ƙ,List<string>ƃ,œ µ,StringBuilder z){z.Clear();if(ƙ>=ƃ.
Count){µ.Ŏ=0;z.Append(ǩ.ToString());return;}Ǥ.Clear();for(int Y=0;Y<ƙ;Y++){var ž=(µ.Ŏ+Y)%ƃ.Count;if(Ǥ.Length>0&&ž==0){Ǥ.
Append('\n');ƙ--;if(Y>=ƙ)break;}Ǥ.Append($"{ƃ[ž]}\n");}µ.Ŏ+=_scrollLineSkip;if(µ.Ŏ>ƃ.Count)µ.Ŏ=0;z.Append(Ǥ.ToString());}
IEnumerator<bool>Ȏ(){while(true){ǎ=false;ƽ.Clear();ư.Clear();for(int Y=ƹ.Count-1;Y>=0;Y--){var Ɠ=ƹ[Y];if(Ɠ==null){ƹ.RemoveAtFast(Y)
;continue;}Ɠ.GetItems(ƽ);yield return false;}int Ė=0;for(int Y=ƽ.Count-1;Y>=0;Y--){var s=ƽ[Y];if(s==null){ƽ.RemoveAtFast(
Y);continue;}var Ê=s.Type;Č Ü;Ǌ.TryGetValue(Ê,out Ü);if(Ü!=null)Ü.Ğ+=(int)s.Amount;if(ǈ.ContainsKey(Ê))ǈ[Ê].Ħ+=(float)s.
Amount;if(++Ė>5){Ė=0;yield return false;}}yield return false;Ǒ=false;foreach(var s in Ǌ.Values){var Ý=s.Ĳ-s.Ğ;if(Ý<=0){if(s.Ĵ)
{ù(s);s.Ĵ=false;}continue;}Ý=(int)Math.Ceiling(Ý/s.ĳ);foreach(var v in s.Ľ){Ĥ Ë;if(Ǉ.TryGetValue(v.Key,out Ë))Ë.Ħ+=(v.
Value*Ý);}int Þ=0;bool ß=false;if(s.Ĵ){s.Ĵ=false;for(int Y=0;Y<Ƹ.Count;Y++){var à=Ƹ[Y];var á=à.OutputInventory;var â=à.
InputInventory;if(!ǳ&&(á.IsFull||â.IsFull||(float)á.CurrentVolume/(float)á.MaxVolume>0.9f||(float)â.CurrentVolume/(float)â.MaxVolume>
0.9f))Ư.Add(à);if(à.IsQueueEmpty||à.EntityId==ǯ?.EntityId)continue;à.GetQueue(Ƽ);var ã=Ƽ[0];ư.Add(ã);if(ã.BlueprintId==s.ĸ)ß
=true;for(int K=Ƽ.Count-1;K>=0;K--){var ä=Ƽ[K];if(ä.BlueprintId==s.ĸ){s.Ĵ=true;Þ+=(int)ä.Amount;}}}if(s.İ!=Þ){s.İ=Þ;s.ı=0
;}else if(ß&&++s.ı>5){û(s);s.ı=0;ß=false;}if(s.Ĵ)continue;}s.Ĵ=true;var å=int.MaxValue;int æ=-1;for(int Y=0;Y<Ƹ.Count;Y++
){var à=Ƹ[Y];if(à.EntityId==ǯ?.EntityId||!à.CanUseBlueprint(s.ĸ))continue;if(à.IsQueueEmpty){æ=Y;break;}à.GetQueue(Ƽ);if(
Ƽ.Count<å){å=Ƽ.Count;æ=Y;}}if(æ>=0&&Ƹ.Count>æ){var à=Ƹ[æ];à.AddQueueItem(s.ĸ,(MyFixedPoint)Ý);s.ı=0;}if(++Ė>5){Ė=0;yield
return false;}}yield return false;for(int Y=0;Y<ƾ.Count;Y++){var ç=ƾ[Y];var é=ç as IMyRefinery;if(é==null)continue;float è=(ç.
BlockDefinition.SubtypeName=="Blast Furnace")?0.7f:0.8f;ç.GetUpgrades(out ǂ);float Û=ǂ.GetValueOrDefault("Effectiveness",1);é.GetQueue(
Ƽ);for(int K=0;K<Ƽ.Count;K++){var s=Ƽ[K];MyItemType Ê=s.BlueprintId;Ĥ Ë;if(Ǉ.TryGetValue(Ê,out Ë)){bool Ì=true;foreach(
var Í in ư){MyItemType Î;Č Â;if(Ʈ.TryGetValue(Í.BlueprintId,out Î)&&Ǌ.TryGetValue(Î,out Â)){for(int Ï=0;Ï<Â.Ľ.Length;Ï++){
var Ð=Â.Ľ[Ï].Key;if(Ê==Ð){if(K>0)é.MoveQueueItemRequest(s.ItemId,0);Ì=false;break;}}if(!Ì)break;}}if(Ì&&Ë.Ħ>0&&K>0)é.
MoveQueueItemRequest(s.ItemId,0);if(!Ê.SubtypeId.StartsWith("Scrap")){var u=Ê.SubtypeId.Replace("OreToIngot","");MyItemType Ñ=MyItemType.
MakeOre(u);if(ǈ.ContainsKey(Ñ)){var Ò=ǈ[Ñ];foreach(var Ó in Ò.Ĩ)Ó.į=Ò.Ħ*Ó.ĳ*è*Û;}}}if(++Ė>10){Ė=0;yield return false;}}yield
return false;}if(_autoDecon&&Ƹ.Count>0){bool Ô=false;int Õ=0;MyDefinitionId Ö=new MyDefinitionId();foreach(var s in Ǌ){Õ=s.
Value.Ğ-s.Value.Ĳ;if(Õ>0){Ô=true;Ö=s.Value.ĸ;break;}}if(Ô){bool Ø=false;if(ǯ==null||ǯ.Ĭ(this)){int Ù=int.MaxValue;for(int Y=Ƹ
.Count-1;Y>=0;Y--){var Ú=Ƹ[Y];if(Ú.Ĭ(this)){Ƹ.RemoveAtFast(Y);continue;}if(Ú.IsQueueEmpty){ǯ=Ú;break;}Ú.GetQueue(Ƽ);if(Ƽ.
Count<Ù){ǯ=Ú;Ù=Ƽ.Count;}}Ø=true;}else{ǯ.GetQueue(Ƽ);if(Ƽ.Count>0){var ê=Ƽ[0];MyItemType ø;Č Ü;if(Ʈ.TryGetValue(ê.BlueprintId,
out ø)&&Ǌ.TryGetValue(ø,out Ü)){if(Ü.Ğ<=Ü.Ĳ||ǯ.OutputInventory.FindItem(ø)==null)Ø=true;}}else Ø=true;}if(Ø){Ʊ.Clear();Ʊ.
Add(Ö);ǯ.Mode=MyAssemblerMode.Disassembly;ǯ.CooperativeMode=false;ǯ.ClearQueue();ǯ.AddQueueItem(Ö,(MyFixedPoint)Õ);}}else
if(ǯ!=null){Ʊ.Clear();ǯ.Mode=MyAssemblerMode.Assembly;ǯ=null;Ƹ.ForEach(ë=>ë.ClearQueue());}yield return false;}else if(ǯ!=
null){Ʊ.Clear();ǯ.Mode=MyAssemblerMode.Assembly;ǯ=null;Ƹ.ForEach(ë=>ë.ClearQueue());}if(ǐ){for(int Y=Ƹ.Count-1;Y>=0;Y--){var
Ú=Ƹ[Y];if(Ú.Ĭ(this)){Ƹ.RemoveAtFast(Y);continue;}if(Ú.EntityId==ǯ?.EntityId)continue;Ú.Mode=MyAssemblerMode.Assembly;Ú.
CooperativeMode=Ú.IsQueueEmpty;}}ǎ=true;yield return true;}}void ù(Č s){for(int Y=Ƹ.Count-1;Y>=0;Y--){var ú=Ƹ[Y];if(ú.Ĭ(this)){Ƹ.
RemoveAtFast(Y);continue;}if(ú.IsQueueEmpty||ú.EntityId==ǯ?.EntityId)continue;ú.GetQueue(Ƽ);for(int K=Ƽ.Count-1;K>=0;K--){var ã=Ƽ[K]
;if(ã.BlueprintId==s.ĸ)ú.RemoveQueueItem(K,ã.Amount);}}}void û(Č s){for(int Y=Ƹ.Count-1;Y>=0;Y--){var ú=Ƹ[Y];if(ú.Ĭ(this)
){Ƹ.RemoveAtFast(Y);continue;}if(ú.IsQueueEmpty||ú.EntityId==ǯ?.EntityId)continue;ú.GetQueue(Ƽ);var ã=Ƽ[0];if(ã.
BlueprintId==s.ĸ)ú.MoveQueueItemRequest(ã.ItemId,Ƽ.Count+1);}}IEnumerator<bool>ü(){Runtime.UpdateFrequency=UpdateFrequency.Update1|
UpdateFrequency.Update10|UpdateFrequency.Update100;Ǐ=false;ǅ=new Dictionary<Resource,MyItemType>(){{Resource.IRON,MyItemType.Parse(
$"{Ǘ}IronOreToIngot")},{Resource.NICKEL,MyItemType.Parse($"{Ǘ}NickelOreToIngot")},{Resource.SILICON,MyItemType.Parse($"{Ǘ}SiliconOreToIngot"
)},{Resource.URANIUM,MyItemType.Parse($"{Ǘ}UraniumOreToIngot")},{Resource.PLATINUM,MyItemType.Parse(
$"{Ǘ}PlatinumOreToIngot")},{Resource.MAGNESIUM,MyItemType.Parse($"{Ǘ}MagnesiumOreToIngot")},{Resource.SILVER,MyItemType.Parse(
$"{Ǘ}SilverOreToIngot")},{Resource.COBALT,MyItemType.Parse($"{Ǘ}CobaltOreToIngot")},{Resource.GOLD,MyItemType.Parse($"{Ǘ}GoldOreToIngot")},{
Resource.STONE,MyItemType.Parse($"{Ǘ}StoneOreToIngot")},{Resource.SCRAP,MyItemType.Parse($"{Ǘ}ScrapToIronIngot")},{Resource.
SCRAPINGOT,MyItemType.Parse($"{Ǘ}ScrapIngotToIronIngot")},};foreach(var v in ǅ)ǀ[v.Value]=v.Key;ǆ=new Dictionary<string,MyItemType
>(){{"Iron",MyItemType.MakeIngot("Iron")},{"Nickel",MyItemType.MakeIngot("Nickel")},{"Silicon",MyItemType.MakeIngot(
"Silicon")},{"Uranium",MyItemType.MakeIngot("Uranium")},{"Platinum",MyItemType.MakeIngot("Platinum")},{"Magnesium",MyItemType.
MakeIngot("Magnesium")},{"Silver",MyItemType.MakeIngot("Silver")},{"Cobalt",MyItemType.MakeIngot("Cobalt")},{"Gold",MyItemType.
MakeIngot("Gold")},{"Stone",MyItemType.MakeIngot("Stone")},{"Scrap",MyItemType.MakeIngot("Scrap")},{"ScrapIngot",MyItemType.
MakeIngot("ScrapIngot")},{"IronOre",MyItemType.MakeOre("Iron")},{"NickelOre",MyItemType.MakeOre("Nickel")},{"SiliconOre",
MyItemType.MakeOre("Silicon")},{"UraniumOre",MyItemType.MakeOre("Uranium")},{"PlatinumOre",MyItemType.MakeOre("Platinum")},{
"MagnesiumOre",MyItemType.MakeOre("Magnesium")},{"SilverOre",MyItemType.MakeOre("Silver")},{"CobaltOre",MyItemType.MakeOre("Cobalt")},
{"GoldOre",MyItemType.MakeOre("Gold")},{"StoneOre",MyItemType.MakeOre("Stone")},{"ScrapOre",MyItemType.MakeOre("Scrap")},
{"ScrapIngotOre",MyItemType.MakeOre("ScrapIngot")},{"Ice",MyItemType.MakeOre("Ice")},};Ǆ=new Dictionary<MyItemType,
Resource>(){{ǆ["Iron"],Resource.IRON},{ǆ["Nickel"],Resource.NICKEL},{ǆ["Silicon"],Resource.SILICON},{ǆ["Uranium"],Resource.
URANIUM},{ǆ["Platinum"],Resource.PLATINUM},{ǆ["Magnesium"],Resource.MAGNESIUM},{ǆ["Silver"],Resource.SILVER},{ǆ["Cobalt"],
Resource.COBALT},{ǆ["Gold"],Resource.GOLD},{ǆ["Stone"],Resource.STONE},{ǆ["Scrap"],Resource.SCRAP},{ǆ["ScrapIngot"],Resource.
SCRAPINGOT},};Ǉ=new Dictionary<MyItemType,Ĥ>(){{ǅ[Resource.IRON],new Ĥ("Iron",ItemType.INGOT,new Ļ(Resource.IRON,0.7f))},{ǅ[
Resource.NICKEL],new Ĥ("Nickel",ItemType.INGOT,new Ļ(Resource.NICKEL,0.4f))},{ǅ[Resource.SILICON],new Ĥ("Silicon",ItemType.INGOT
,new Ļ(Resource.SILICON,0.7f))},{ǅ[Resource.URANIUM],new Ĥ("Uranium",ItemType.INGOT,new Ļ(Resource.URANIUM,0.01f))},{ǅ[
Resource.PLATINUM],new Ĥ("Platinum",ItemType.INGOT,new Ļ(Resource.PLATINUM,0.005f))},{ǅ[Resource.MAGNESIUM],new Ĥ("Magnesium",
ItemType.INGOT,new Ļ(Resource.MAGNESIUM,0.007f))},{ǅ[Resource.SILVER],new Ĥ("Silver",ItemType.INGOT,new Ļ(Resource.SILVER,0.1f))
},{ǅ[Resource.COBALT],new Ĥ("Cobalt",ItemType.INGOT,new Ļ(Resource.COBALT,0.3f))},{ǅ[Resource.GOLD],new Ĥ("Gold",ItemType
.INGOT,new Ļ(Resource.GOLD,0.01f))},{ǅ[Resource.STONE],new Ĥ("Gravel",ItemType.INGOT,new Ļ(Resource.STONE,0.027f),new Ļ(
Resource.IRON,0.054f),new Ļ(Resource.NICKEL,0.0046f),new Ļ(Resource.SILICON,0.007f))},{ǅ[Resource.SCRAP],new Ĥ("Scrap",ItemType.
INGOT,new Ļ(Resource.SCRAP,0.8f))},{ǅ[Resource.SCRAPINGOT],new Ĥ("Scrap",ItemType.INGOT,new Ļ(Resource.SCRAPINGOT,0.8f))},{ǆ[
"IronOre"],new Ĥ("Iron Ore",ItemType.ORE,new Ļ(Resource.IRON,0.7f))},{ǆ["NickelOre"],new Ĥ("Nickel Ore",ItemType.ORE,new Ļ(
Resource.NICKEL,0.4f))},{ǆ["SiliconOre"],new Ĥ("Silicon Ore",ItemType.ORE,new Ļ(Resource.SILICON,0.7f))},{ǆ["UraniumOre"],new Ĥ(
"Uranium Ore",ItemType.ORE,new Ļ(Resource.URANIUM,0.01f))},{ǆ["PlatinumOre"],new Ĥ("Platinum Ore",ItemType.ORE,new Ļ(Resource.
PLATINUM,0.005f))},{ǆ["MagnesiumOre"],new Ĥ("Magnesium Ore",ItemType.ORE,new Ļ(Resource.MAGNESIUM,0.007f))},{ǆ["SilverOre"],new
Ĥ("Silver Ore",ItemType.ORE,new Ļ(Resource.SILVER,0.1f))},{ǆ["CobaltOre"],new Ĥ("Cobalt Ore",ItemType.ORE,new Ļ(Resource.
COBALT,0.3f))},{ǆ["GoldOre"],new Ĥ("Gold Ore",ItemType.ORE,new Ļ(Resource.GOLD,0.01f))},{ǆ["StoneOre"],new Ĥ("Stone",ItemType.
ORE,new Ļ(Resource.STONE,0.027f),new Ļ(Resource.IRON,0.054f),new Ļ(Resource.NICKEL,0.0046f),new Ļ(Resource.SILICON,0.007f))
},{ǆ["ScrapOre"],new Ĥ("Scrap",ItemType.ORE,new Ļ(Resource.SCRAP,0.8f))},{ǆ["ScrapIngotOre"],new Ĥ("Scrap",ItemType.ORE,
new Ļ(Resource.SCRAPINGOT,0.8f))},};ǈ=new Dictionary<MyItemType,Ĥ>(){{ǆ["Iron"],new Ĥ("Iron",ItemType.INGOT,new Ļ(Resource.
IRON,0.7f))},{ǆ["Nickel"],new Ĥ("Nickel",ItemType.INGOT,new Ļ(Resource.NICKEL,0.4f))},{ǆ["Silicon"],new Ĥ("Silicon",ItemType
.INGOT,new Ļ(Resource.SILICON,0.7f))},{ǆ["Uranium"],new Ĥ("Uranium",ItemType.INGOT,new Ļ(Resource.URANIUM,0.01f))},{ǆ[
"Platinum"],new Ĥ("Platinum",ItemType.INGOT,new Ļ(Resource.PLATINUM,0.005f))},{ǆ["Magnesium"],new Ĥ("Magnesium",ItemType.INGOT,new
Ļ(Resource.MAGNESIUM,0.007f))},{ǆ["Silver"],new Ĥ("Silver",ItemType.INGOT,new Ļ(Resource.SILVER,0.1f))},{ǆ["Cobalt"],new
Ĥ("Cobalt",ItemType.INGOT,new Ļ(Resource.COBALT,0.3f))},{ǆ["Gold"],new Ĥ("Gold",ItemType.INGOT,new Ļ(Resource.GOLD,0.01f)
)},{ǆ["Stone"],new Ĥ("Gravel",ItemType.INGOT,new Ļ(Resource.STONE,0.027f),new Ļ(Resource.IRON,0.054f),new Ļ(Resource.
NICKEL,0.0046f),new Ļ(Resource.SILICON,0.007f))},{ǆ["Scrap"],new Ĥ("Scrap",ItemType.INGOT,new Ļ(Resource.SCRAP,0.8f))},{ǆ[
"ScrapIngot"],new Ĥ("Scrap",ItemType.INGOT,new Ļ(Resource.SCRAPINGOT,0.8f))},{ǆ["IronOre"],new Ĥ("Iron Ore",ItemType.ORE,new Ļ(
Resource.IRON,0.7f))},{ǆ["NickelOre"],new Ĥ("Nickel Ore",ItemType.ORE,new Ļ(Resource.NICKEL,0.4f))},{ǆ["SiliconOre"],new Ĥ(
"Silicon Ore",ItemType.ORE,new Ļ(Resource.SILICON,0.7f))},{ǆ["UraniumOre"],new Ĥ("Uranium Ore",ItemType.ORE,new Ļ(Resource.URANIUM,
0.01f))},{ǆ["PlatinumOre"],new Ĥ("Platinum Ore",ItemType.ORE,new Ļ(Resource.PLATINUM,0.005f))},{ǆ["MagnesiumOre"],new Ĥ(
"Magnesium Ore",ItemType.ORE,new Ļ(Resource.MAGNESIUM,0.007f))},{ǆ["SilverOre"],new Ĥ("Silver Ore",ItemType.ORE,new Ļ(Resource.SILVER,
0.1f))},{ǆ["CobaltOre"],new Ĥ("Cobalt Ore",ItemType.ORE,new Ļ(Resource.COBALT,0.3f))},{ǆ["GoldOre"],new Ĥ("Gold Ore",
ItemType.ORE,new Ļ(Resource.GOLD,0.01f))},{ǆ["StoneOre"],new Ĥ("Stone",ItemType.ORE,new Ļ(Resource.STONE,0.027f),new Ļ(Resource.
IRON,0.054f),new Ļ(Resource.NICKEL,0.0046f),new Ļ(Resource.SILICON,0.007f))},{ǆ["ScrapOre"],new Ĥ("Scrap",ItemType.ORE,new Ļ
(Resource.SCRAP,0.8f))},{ǆ["ScrapIngotOre"],new Ĥ("Scrap",ItemType.ORE,new Ļ(Resource.SCRAPINGOT,0.8f))},};ǃ=new
Dictionary<string,MyItemType>(){{"Missile200mm",MyItemType.MakeAmmo("Missile200mm")},{"NATO_5p56x45mm",MyItemType.MakeAmmo(
"NATO_5p56x45mm")},{"NATO_25x184mm",MyItemType.MakeAmmo("NATO_25x184mm")},{"AutomaticRifleGun_Mag_20rd",MyItemType.MakeAmmo(
"AutomaticRifleGun_Mag_20rd")},{"UltimateAutomaticRifleGun_Mag_30rd",MyItemType.MakeAmmo("UltimateAutomaticRifleGun_Mag_30rd")},{
"RapidFireAutomaticRifleGun_Mag_50rd",MyItemType.MakeAmmo("RapidFireAutomaticRifleGun_Mag_50rd")},{"PreciseAutomaticRifleGun_Mag_5rd",MyItemType.MakeAmmo(
"PreciseAutomaticRifleGun_Mag_5rd")},{"SemiAutoPistolMagazine",MyItemType.MakeAmmo("SemiAutoPistolMagazine")},{"ElitePistolMagazine",MyItemType.MakeAmmo(
"ElitePistolMagazine")},{"FullAutoPistolMagazine",MyItemType.MakeAmmo("FullAutoPistolMagazine")},{"LargeCalibreAmmo",MyItemType.MakeAmmo(
"LargeCalibreAmmo")},{"MediumCalibreAmmo",MyItemType.MakeAmmo("MediumCalibreAmmo")},{"AutocannonClip",MyItemType.MakeAmmo("AutocannonClip"
)},{"SmallRailgunAmmo",MyItemType.MakeAmmo("SmallRailgunAmmo")},{"LargeRailgunAmmo",MyItemType.MakeAmmo(
"LargeRailgunAmmo")},{"Datapad",MyItemType.Parse("MyObjectBuilder_Datapad/Datapad")},{"BulletproofGlass",MyItemType.MakeComponent(
"BulletproofGlass")},{"Canvas",MyItemType.MakeComponent("Canvas")},{"Computer",MyItemType.MakeComponent("Computer")},{"Construction",
MyItemType.MakeComponent("Construction")},{"Detector",MyItemType.MakeComponent("Detector")},{"Display",MyItemType.MakeComponent(
"Display")},{"Explosives",MyItemType.MakeComponent("Explosives")},{"Girder",MyItemType.MakeComponent("Girder")},{
"GravityGenerator",MyItemType.MakeComponent("GravityGenerator")},{"InteriorPlate",MyItemType.MakeComponent("InteriorPlate")},{"LargeTube",
MyItemType.MakeComponent("LargeTube")},{"Medical",MyItemType.MakeComponent("Medical")},{"MetalGrid",MyItemType.MakeComponent(
"MetalGrid")},{"Motor",MyItemType.MakeComponent("Motor")},{"PowerCell",MyItemType.MakeComponent("PowerCell")},{"RadioCommunication"
,MyItemType.MakeComponent("RadioCommunication")},{"Reactor",MyItemType.MakeComponent("Reactor")},{"SmallTube",MyItemType.
MakeComponent("SmallTube")},{"SolarCell",MyItemType.MakeComponent("SolarCell")},{"SteelPlate",MyItemType.MakeComponent("SteelPlate")}
,{"Superconductor",MyItemType.MakeComponent("Superconductor")},{"Thrust",MyItemType.MakeComponent("Thrust")},{
"HydrogenBottle",MyItemType.Parse($"{ǘ}HydrogenBottle")},{"OxygenBottle",MyItemType.Parse($"{Ǚ}OxygenBottle")},{"AngleGrinderItem",
MyItemType.MakeTool("AngleGrinderItem")},{"AngleGrinder2Item",MyItemType.MakeTool("AngleGrinder2Item")},{"AngleGrinder3Item",
MyItemType.MakeTool("AngleGrinder3Item")},{"AngleGrinder4Item",MyItemType.MakeTool("AngleGrinder4Item")},{"HandDrillItem",
MyItemType.MakeTool("HandDrillItem")},{"HandDrill2Item",MyItemType.MakeTool("HandDrill2Item")},{"HandDrill3Item",MyItemType.
MakeTool("HandDrill3Item")},{"HandDrill4Item",MyItemType.MakeTool("HandDrill4Item")},{"AutomaticRifleItem",MyItemType.MakeTool(
"AutomaticRifleItem")},{"PreciseAutomaticRifleItem",MyItemType.MakeTool("PreciseAutomaticRifleItem")},{"RapidFireAutomaticRifleItem",
MyItemType.MakeTool("RapidFireAutomaticRifleItem")},{"UltimateAutomaticRifleItem",MyItemType.MakeTool("UltimateAutomaticRifleItem"
)},{"WelderItem",MyItemType.MakeTool("WelderItem")},{"Welder2Item",MyItemType.MakeTool("Welder2Item")},{"Welder3Item",
MyItemType.MakeTool("Welder3Item")},{"Welder4Item",MyItemType.MakeTool("Welder4Item")},{"BasicHandHeldLauncherItem",MyItemType.
MakeTool("BasicHandHeldLauncherItem")},{"AdvancedHandHeldLauncherItem",MyItemType.MakeTool("AdvancedHandHeldLauncherItem")},{
"SemiAutoPistolItem",MyItemType.MakeTool("SemiAutoPistolItem")},{"FullAutoPistolItem",MyItemType.MakeTool("FullAutoPistolItem")},{
"ElitePistolItem",MyItemType.MakeTool("ElitePistolItem")},};Ǌ=new Dictionary<MyItemType,Č>(){{ǃ["OxygenBottle"],new Č(MyItemType.Parse(
$"{Ǘ}Position0010_OxygenBottle"),ItemType.OXYGENGAS,"Oxygen Bottle",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float
>(ǅ[Resource.IRON],80f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],10f/_efficiencySetting)
,new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],30f/_efficiencySetting),}}},{ǃ["HydrogenBottle"],new Č(MyItemType.
Parse($"{Ǘ}Position0020_HydrogenBottle"),ItemType.HYDROGENGAS,"Hydrogen Bottle",0,false,1){Ľ=new KeyValuePair<MyItemType,
float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],80f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.SILICON],10f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],30f/_efficiencySetting),}}},{ǃ[
"Construction"],new Č(MyItemType.Parse($"{Ǘ}ConstructionComponent"),ItemType.COMPONENT,"Construction Comp.",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],8f/_efficiencySetting),}}},{ǃ["Girder"],new Č(
MyItemType.Parse($"{Ǘ}GirderComponent"),ItemType.COMPONENT,"Girder",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new
KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],6f/_efficiencySetting),}}},{ǃ["MetalGrid"],new Č(MyItemType.Parse($"{Ǘ}MetalGrid"),
ItemType.COMPONENT,"Metal Grid",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.
IRON],12f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],5f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.COBALT],3f/_efficiencySetting),}}},{ǃ["InteriorPlate"],new Č(MyItemType.Parse($"{Ǘ}InteriorPlate"),
ItemType.COMPONENT,"Interior Plate",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],3f/_efficiencySetting),}}},{ǃ["SteelPlate"],new Č(MyItemType.Parse($"{Ǘ}SteelPlate"),ItemType.COMPONENT,
"Steel Plate",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],21f/
_efficiencySetting),}}},{ǃ["SmallTube"],new Č(MyItemType.Parse($"{Ǘ}SmallTube"),ItemType.COMPONENT,"Small Steel Tube",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],5f/_efficiencySetting),}}},{ǃ["LargeTube"],new
Č(MyItemType.Parse($"{Ǘ}LargeTube"),ItemType.COMPONENT,"Large Steel Tube",0,false,1){Ľ=new KeyValuePair<MyItemType,float>
[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],30f/_efficiencySetting),}}},{ǃ["Motor"],new Č(MyItemType.Parse(
$"{Ǘ}MotorComponent"),ItemType.COMPONENT,"Motor",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],20f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],5f/_efficiencySetting),}}},{ǃ[
"Display"],new Č(MyItemType.Parse($"{Ǘ}Display"),ItemType.COMPONENT,"Display",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{
new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.
SILICON],5f/_efficiencySetting),}}},{ǃ["BulletproofGlass"],new Č(MyItemType.Parse($"{Ǘ}BulletproofGlass"),ItemType.COMPONENT,
"Bulletproof Glass",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],15f/
_efficiencySetting),}}},{ǃ["Computer"],new Č(MyItemType.Parse($"{Ǘ}ComputerComponent"),ItemType.COMPONENT,"Computer",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],0.5f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.SILICON],0.2f/_efficiencySetting),}}},{ǃ["Reactor"],new Č(MyItemType.Parse($"{Ǘ}ReactorComponent"),
ItemType.COMPONENT,"Reactor Comp.",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],15f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.STONE],20f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.SILVER],5f/_efficiencySetting),}}},{ǃ["Thrust"],new Č(MyItemType.Parse(
$"{Ǘ}ThrustComponent"),ItemType.COMPONENT,"Thruster Comp.",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,
float>(ǅ[Resource.IRON],30f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],10f/_efficiencySetting)
,new KeyValuePair<MyItemType,float>(ǅ[Resource.GOLD],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource
.PLATINUM],0.4f/_efficiencySetting),}}},{ǃ["GravityGenerator"],new Č(MyItemType.Parse($"{Ǘ}GravityGeneratorComponent"),
ItemType.COMPONENT,"Gravity Comp.",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.SILVER],5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.GOLD],10f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],220f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],600f/
_efficiencySetting),}}},{ǃ["Medical"],new Č(MyItemType.Parse($"{Ǘ}MedicalComponent"),ItemType.COMPONENT,"Medical Comp.",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],60f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],70f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILVER],20f/
_efficiencySetting),}}},{ǃ["RadioCommunication"],new Č(MyItemType.Parse($"{Ǘ}RadioCommunicationComponent"),ItemType.COMPONENT,
"Radio-comm Comp.",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],8f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],1f/_efficiencySetting),}}},{ǃ["Detector"],new Č(MyItemType.
Parse($"{Ǘ}DetectorComponent"),ItemType.COMPONENT,"Detector Comp.",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new
KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],15f/
_efficiencySetting),}}},{ǃ["Canvas"],new Č(MyItemType.Parse($"{Ǘ}Position0030_Canvas"),ItemType.COMPONENT,"Canvas",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],35f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.IRON],2f/_efficiencySetting),}}},{ǃ["Explosives"],new Č(MyItemType.Parse($"{Ǘ}ExplosivesComponent"),
ItemType.COMPONENT,"Explosives",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.
SILICON],0.5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],2.0f/_efficiencySetting),}}},{ǃ[
"SolarCell"],new Č(MyItemType.Parse($"{Ǘ}SolarCell"),ItemType.COMPONENT,"Solar Cell",0,false,1){Ľ=new KeyValuePair<MyItemType,float
>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.SILICON],6f/_efficiencySetting),}}},{ǃ["PowerCell"],new Č(MyItemType.Parse($"{Ǘ}PowerCell"),ItemType.COMPONENT,
"Power Cell",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],10f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.NICKEL],2f/_efficiencySetting),}}},{ǃ["BasicHandHeldLauncherItem"],new Č(MyItemType.Parse(
$"{Ǘ}Position0080_BasicHandHeldLauncher"),ItemType.TOOL,"RO-1",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.
IRON],30f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],10f/_efficiencySetting),new KeyValuePair
<MyItemType,float>(ǅ[Resource.COBALT],5f/_efficiencySetting),}}},{ǃ["AdvancedHandHeldLauncherItem"],new Č(MyItemType.
Parse($"{Ǘ}Position0090_AdvancedHandHeldLauncher"),ItemType.TOOL,"PRO-1",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{
new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],30f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.
NICKEL],10f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],5f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.PLATINUM],5f/_efficiencySetting),}}},{ǃ["SemiAutoPistolItem"],new Č(MyItemType.Parse(
$"{Ǘ}Position0010_SemiAutoPistol"),ItemType.TOOL,"S-10",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.
IRON],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],0.3f/_efficiencySetting),}}},{ǃ[
"FullAutoPistolItem"],new Č(MyItemType.Parse($"{Ǘ}Position0020_FullAutoPistol"),ItemType.TOOL,"S-20A",0,false,1){Ľ=new KeyValuePair<
MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],1.5f/_efficiencySetting),new KeyValuePair<MyItemType,float
>(ǅ[Resource.NICKEL],0.5f/_efficiencySetting),}}},{ǃ["ElitePistolItem"],new Č(MyItemType.Parse(
$"{Ǘ}Position0030_EliteAutoPistol"),ItemType.TOOL,"S-10E",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.
IRON],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],0.4f/_efficiencySetting),new KeyValuePair
<MyItemType,float>(ǅ[Resource.PLATINUM],0.5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILVER],1f
/_efficiencySetting),}}},{ǃ["AutomaticRifleItem"],new Č(MyItemType.Parse($"{Ǘ}Position0040_AutomaticRifle"),ItemType.TOOL
,"MR-20",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],3f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),}}},{ǃ["RapidFireAutomaticRifleItem"],new
Č(MyItemType.Parse($"{Ǘ}Position0050_RapidFireAutomaticRifle"),ItemType.TOOL,"MR-50A",0,false,1){Ľ=new KeyValuePair<
MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(
ǅ[Resource.NICKEL],8f/_efficiencySetting),}}},{ǃ["PreciseAutomaticRifleItem"],new Č(MyItemType.Parse(
$"{Ǘ}Position0060_PreciseAutomaticRifle"),ItemType.TOOL,"MR-8P",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.
IRON],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.COBALT],5f/_efficiencySetting),}}},{ǃ["UltimateAutomaticRifleItem"],new Č(MyItemType.Parse(
$"{Ǘ}Position0070_UltimateAutomaticRifle"),ItemType.TOOL,"MR-30E",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource
.IRON],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.PLATINUM],4f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILVER],6f/
_efficiencySetting),}}},{ǃ["WelderItem"],new Č(MyItemType.Parse($"{Ǘ}Position0090_Welder"),ItemType.TOOL,"Welder",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],5f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.STONE],3f/
_efficiencySetting),}}},{ǃ["Welder2Item"],new Č(MyItemType.Parse($"{Ǘ}Position0100_Welder2"),ItemType.TOOL,"Enhanced Welder",0,false,1){Ľ=
new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],5f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],0.2f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],2f/_efficiencySetting),}}},{ǃ["Welder3Item"],new Č(MyItemType.
Parse($"{Ǘ}Position0110_Welder3"),ItemType.TOOL,"Proficient Welder",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new
KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],0.2f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.SILVER],2f/_efficiencySetting),}}},{ǃ["Welder4Item"],new Č(MyItemType.Parse($"{Ǘ}Position0120_Welder4"),ItemType.TOOL,
"Elite Welder",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],5f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.COBALT],0.2f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.PLATINUM],2f/_efficiencySetting),}}},{ǃ[
"AngleGrinderItem"],new Č(MyItemType.Parse($"{Ǘ}Position0010_AngleGrinder"),ItemType.TOOL,"Grinder",0,false,1){Ľ=new KeyValuePair<
MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(
ǅ[Resource.NICKEL],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.STONE],5f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],1f/_efficiencySetting),}}},{ǃ["AngleGrinder2Item"],new Č(MyItemType.Parse(
$"{Ǘ}Position0020_AngleGrinder2"),ItemType.TOOL,"Enhanced Grinder",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(
ǅ[Resource.IRON],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],2f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],6f/
_efficiencySetting),}}},{ǃ["AngleGrinder3Item"],new Č(MyItemType.Parse($"{Ǘ}Position0030_AngleGrinder3"),ItemType.TOOL,
"Proficient Grinder",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],3f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.COBALT],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],2f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.SILVER],2f/_efficiencySetting),}}},{ǃ["AngleGrinder4Item"],new Č(MyItemType.Parse(
$"{Ǘ}Position0040_AngleGrinder4"),ItemType.TOOL,"Elite Grinder",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],1f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.COBALT],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],2f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.PLATINUM],2f/_efficiencySetting),}}},{ǃ["HandDrillItem"],new Č(
MyItemType.Parse($"{Ǘ}Position0050_HandDrill"),ItemType.TOOL,"Hand Drill",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new
KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],20f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],3f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],3f/_efficiencySetting),}}},{ǃ["HandDrill2Item"],new Č(
MyItemType.Parse($"{Ǘ}Position0060_HandDrill2"),ItemType.TOOL,"Enhanced Hand Drill",0,false,1){Ľ=new KeyValuePair<MyItemType,float
>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],20f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.NICKEL],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],5f/_efficiencySetting),}}},{ǃ[
"HandDrill3Item"],new Č(MyItemType.Parse($"{Ǘ}Position0070_HandDrill3"),ItemType.TOOL,"Proficient Hand Drill",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],20f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],3f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILVER],2f/_efficiencySetting),}}},{ǃ["HandDrill4Item"],new Č(MyItemType
.Parse($"{Ǘ}Position0080_HandDrill4"),ItemType.TOOL,"Elite Hand Drill",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{
new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],20f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.
NICKEL],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],3f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.PLATINUM],2f/_efficiencySetting),}}},{ǃ["SemiAutoPistolMagazine"],new Č(MyItemType.Parse(
$"{Ǘ}Position0010_SemiAutoPistolMagazine"),ItemType.AMMO,"S-10 Magazine",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],0.25f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],0.05f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],0.05f/_efficiencySetting),}}},{ǃ["FullAutoPistolMagazine"],new Č(MyItemType.
Parse($"{Ǘ}Position0020_FullAutoPistolMagazine"),ItemType.AMMO,"S-20A Magazine",0,false,1){Ľ=new KeyValuePair<MyItemType,
float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],0.5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.NICKEL],0.1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],0.1f/_efficiencySetting),}}},
{ǃ["ElitePistolMagazine"],new Č(MyItemType.Parse($"{Ǘ}Position0030_ElitePistolMagazine"),ItemType.AMMO,"S-10E Magazine",0
,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],0.3f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],0.1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.MAGNESIUM],0.1f/_efficiencySetting),}}},{ǃ["AutomaticRifleGun_Mag_20rd"],new Č(MyItemType.Parse(
$"{Ǘ}Position0040_AutomaticRifleGun_Mag_20rd"),ItemType.AMMO,"MR-20 Magazine",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],0.8f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],0.2f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],0.15f/_efficiencySetting),}}},{ǃ["RapidFireAutomaticRifleGun_Mag_50rd"],new Č(
MyItemType.Parse($"{Ǘ}Position0050_RapidFireAutomaticRifleGun_Mag_50rd"),ItemType.AMMO,"MR-50A Magazine",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],2f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],0.5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],0.4f/
_efficiencySetting),}}},{ǃ["PreciseAutomaticRifleGun_Mag_5rd"],new Č(MyItemType.Parse($"{Ǘ}Position0060_PreciseAutomaticRifleGun_Mag_5rd")
,ItemType.AMMO,"MR-8P Magazine",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],0.8f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],0.2f/_efficiencySetting),new
KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],0.15f/_efficiencySetting),}}},{ǃ["UltimateAutomaticRifleGun_Mag_30rd"],new Č(
MyItemType.Parse($"{Ǘ}Position0070_UltimateAutomaticRifleGun_Mag_30rd"),ItemType.AMMO,"MR-30E Magazine",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],1.2f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],0.4f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],0.25f/
_efficiencySetting),}}},{ǃ["NATO_25x184mm"],new Č(MyItemType.Parse($"{Ǘ}Position0080_NATO_25x184mmMagazine"),ItemType.AMMO,
"Gatling Ammo Box",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],40f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.MAGNESIUM],3.0f/_efficiencySetting),}}},{ǃ["Missile200mm"],new Č(MyItemType.Parse($"{Ǘ}Position0100_Missile200mm"),
ItemType.AMMO,"Missile",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],
55f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],7f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.SILICON],0.2f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.URANIUM],0.1f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.PLATINUM],0.04f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ
[Resource.MAGNESIUM],1.2f/_efficiencySetting),}}},{ǃ["Superconductor"],new Č(MyItemType.Parse($"{Ǘ}Superconductor"),
ItemType.COMPONENT,"Superconductor",0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[
Resource.IRON],10f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.GOLD],2f/_efficiencySetting),}}},{ǃ[
"MediumCalibreAmmo"],new Č(MyItemType.Parse($"{Ǘ}Position0110_MediumCalibreAmmo"),ItemType.AMMO,"Assault Cannon Shell",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],15f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],2f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],1.2f/
_efficiencySetting),}}},{ǃ["LargeCalibreAmmo"],new Č(MyItemType.Parse($"{Ǘ}Position0120_LargeCalibreAmmo"),ItemType.AMMO,"Artillery Shell"
,0,false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],60f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],8f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.MAGNESIUM],5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.URANIUM],0.1f/_efficiencySetting),}}},{
ǃ["LargeRailgunAmmo"],new Č(MyItemType.Parse($"{Ǘ}Position0140_LargeRailgunAmmo"),ItemType.AMMO,"Large Railgun Sabot",0,
false,1){Ľ=new KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],20f/_efficiencySetting),
new KeyValuePair<MyItemType,float>(ǅ[Resource.NICKEL],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.
SILICON],30f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.URANIUM],1f/_efficiencySetting),}}},{ǃ[
"SmallRailgunAmmo"],new Č(MyItemType.Parse($"{Ǘ}Position0130_SmallRailgunAmmo"),ItemType.AMMO,"Small Railgun Sabot",0,false,1){Ľ=new
KeyValuePair<MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],4f/_efficiencySetting),new KeyValuePair<
MyItemType,float>(ǅ[Resource.NICKEL],0.5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.SILICON],5f/
_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.URANIUM],0.2f/_efficiencySetting),}}},{ǃ["AutocannonClip"],new Č(
MyItemType.Parse($"{Ǘ}Position0090_AutocannonClip"),ItemType.AMMO,"Autocannon Magazine",0,false,1){Ľ=new KeyValuePair<MyItemType,
float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],25f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[
Resource.NICKEL],3f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.MAGNESIUM],2f/_efficiencySetting),}}},{ǃ[
"Datapad"],new Č(MyItemType.Parse($"{Ǘ}Position0040_Datapad"),ItemType.COMPONENT,"Datapad",0,false,1){Ľ=new KeyValuePair<
MyItemType,float>[]{new KeyValuePair<MyItemType,float>(ǅ[Resource.IRON],1f/_efficiencySetting),new KeyValuePair<MyItemType,float>(
ǅ[Resource.SILICON],5f/_efficiencySetting),new KeyValuePair<MyItemType,float>(ǅ[Resource.STONE],1f/_efficiencySetting),}}
},};foreach(var v in Ǌ)Ʈ[v.Value.ĸ]=v.Key;ǉ=new Dictionary<MyCubeSize,Dictionary<string,ğ>>(){{MyCubeSize.Large,new
Dictionary<string,ğ>{{"LargeTextPanel",new ğ(ǜ,Ǟ*0.6f)},{"LargeLCDPanel",new ğ(ǜ,Ǟ)},{"LargeLCDPanelWide",new ğ(ǝ,Ǟ)},{
"LargeLCDPanel2x2",new ğ(ǜ,Ǟ)},{"LargeBlockCorner_LCD_1",new ğ(ǜ*4f,98.667f*4f)},{"LargeBlockCorner_LCD_2",new ğ(ǜ*4f,98.667f*4f)},{
"LargeBlockCorner_LCD_Flat_1",new ğ(ǜ*4f,111.0f*3.5f)},{"LargeBlockCorner_LCD_Flat_2",new ğ(ǜ*4f,111.0f*3.5f)},{"FlightLCD",new ğ(655f,240.5f)},{
"LargeLCDPanelSlope2Base4",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Base3",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Base2",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Base1"
,new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Tip4",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Tip3",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Tip2",new
ğ(ǜ,Ǟ)},{"LargeLCDPanelSlope2Tip1",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlopeV",new ğ(ǜ,Ǟ)},{"LargeLCDPanelSlopeH",new ğ(ǜ,Ǟ)},{
"TransparentLCDLarge",new ğ(ǜ,Ǟ)},{"LargeLCDPanel5x5",new ğ(ǜ,Ǟ)},{"LargeLCDPanel5x3",new ğ(ǜ,Ǟ*0.6f)},{"LargeLCDPanel3x3",new ğ(ǜ,Ǟ)}}},{
MyCubeSize.Small,new Dictionary<string,ğ>{{"SmallTextPanel",new ğ(ǜ,Ǟ)},{"SmallLCDPanel",new ğ(ǜ,Ǟ)},{"SmallLCDPanelWide",new ğ(ǝ,
Ǟ)},{"SmallBlockCorner_LCD_1",new ğ(ǜ*2f,444f)},{"SmallBlockCorner_LCD_2",new ğ(ǜ*2f,444f)},{
"SmallBlockCorner_LCD_Flat_1",new ğ(ǜ*2f,555f)},{"SmallBlockCorner_LCD_Flat_2",new ğ(ǜ*2f,555f)},{"SmallTextPanelSlopeBase4",new ğ(ǜ,Ǟ)},{
"SmallTextPanelSlopeBase3",new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeBase2",new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeBase1",new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeTip4",
new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeTip3",new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeTip2",new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeTip1",new ğ(
ǜ,Ǟ)},{"SmallTextPanelSlopeV",new ğ(ǜ,Ǟ)},{"SmallTextPanelSlopeH",new ğ(ǜ,Ǟ)},{"TransparentLCDSmall",new ğ(ǜ*0.95f,Ǟ*0.9f
)}}}};È(ů);HashSet<MyItemType>ý=new HashSet<MyItemType>();List<MyItemType>þ=new List<MyItemType>();while(true){Z();var ö=
GridTerminalSystem.GetBlockGroupWithName(_inventoryGroupName);if(ö!=null)ö.GetBlocks(ƻ);else if(_useThisGrid)GridTerminalSystem.
GetBlocksOfType(ƻ,ë=>ë.IsSameConstructAs(Me));else GridTerminalSystem.GetBlocks(ƻ);for(int Y=ƻ.Count-1;Y>=0;Y--){var ç=ƻ[Y];if(ç.Ĭ(this
)){ƻ.RemoveAtFast(Y);continue;}þ.Clear();if(ç.HasInventory){ç.GetInventory().GetAcceptedItems(þ);ý.UnionWith(þ);}if(ç.
CustomName.Contains("[Exclude]")||ç.CustomData.Contains("[Exclude]"))continue;for(int K=0;K<ç.InventoryCount;K++)ƹ.Add(ç.
GetInventory(K));var ì=ç as IMyCargoContainer;if(ì!=null){ƺ.Add(ì);continue;}var í=ç as IMyTextPanel;if(í!=null){if(í.CustomName.
Contains(_invLCDTag)||í.CustomData.Contains(_invLCDTag)){ƶ.Add(new œ(í,null,false,true));continue;}if(ö!=null||í.CustomData.
Contains(_lcdTag)||í.CustomName.Contains(_lcdTag))Ƶ.Add(í);continue;}var î=ç as IMyAssembler;if(î!=null){Ƹ.Add(î);continue;}var
ï=ç as IMyRefinery;if(ï!=null&&!ï.BlockDefinition.SubtypeName.Contains("ShieldGenerator")){Ʒ.Add(ï);ƾ.Add(ç as
IMyUpgradableBlock);}}if(ƹ.Count==0){Echo("Failed to find any inventories!");}else if(Ƹ.Count==0){Echo("Failed to find any assemblers!");}
else break;yield return false;}foreach(var s in ý){var A=s.GetItemInfo();var F=s.SubtypeId;if(F.StartsWith("GoodAI")||F.
StartsWith("CubePlacer"))continue;if(A.IsOre||A.IsIngot){if(!ǆ.ContainsKey(F)){ǆ[F]=s;Ǆ[s]=Resource.CUSTOM;}}else if(!ǃ.
ContainsKey(F))ǃ[F]=s;}ý.Clear();þ.Clear();ǰ=$"{_lcdTag}{_multiDisplayDelimeter.ToString()}";char[]ð=new char[2]{
_multiDisplayDelimeter,' '};for(int Y=Ƶ.Count-1;Y>=0;Y--){var µ=Ƶ[Y];bool ñ=µ.CustomData.Contains(ǰ);bool ò=µ.CustomName.Contains(ǰ);if(ñ||ò){
string L,ó;if(ñ)L=µ.CustomData;else L=µ.CustomName;int æ=L.IndexOf(ǰ)+ǰ.Length;ó=L.Substring(æ).Trim(ð);if(!char.IsLetter(ó[0]
))throw new Exception(
$"The Set Identifier MUST be\na character! '{string.Format(@"{0}",ó[0].ToString())}' does not\nmeet the criteria.\n\n");bool ô=false;for(int K=0;K<Ƶ.Count;K++){var õ=Ƶ[K];if(õ.EntityId==µ.EntityId)continue;if(õ.CustomData.Contains(ǰ))L=õ.
CustomData;else if(õ.CustomName.Contains(ǰ))L=õ.CustomName;else continue;æ=L.IndexOf(ǰ)+ǰ.Length;string É=L.Substring(æ).Trim(ð);
var X=char.ToUpper(É[0]);var V=char.ToUpper(ó[0]);if(X==V){if(É[1]=='1')ƶ.Add(new œ(õ,µ,true,false));else ƶ.Add(new œ(µ,õ,
true,false));ô=true;Ƶ.RemoveAtFast(K);Y--;break;}}if(!ô)ƶ.Add(new œ(µ,null,false,false));}else ƶ.Add(new œ(µ,null,false,
false));Ƶ.RemoveAtFast(Y);}for(int Y=0;Y<Ƹ.Count;Y++)Ƹ[Y].ClearQueue();Ǳ=n(ů,false);Me.CustomData=Ǳ;if(Ǵ)Ǯ=new ũ(this);ǫ=ƒ();
ǭ=Ȏ();Ƿ=_useSpriteMode?ƥ():Ȁ();}void Z(){ƺ?.Clear();ƹ?.Clear();Ƶ?.Clear();Ƹ?.Clear();Ʒ?.Clear();ƻ?.Clear();}void c(){ǥ.
Clear().Append("--- IQueue Production Manager ---\n").Append("                   by JTurp\n\n").Append(
$"Average Runtime: {g.ToString("0.0000")} ms\n").Append($"Assembler Count: {Ƹ.Count.ToString()}\n").Append($"Refinery Count: {Ʒ.Count.ToString()}\n\n").Append(
$"----- Current Settings -----\n").Append($"Scroll Line-Skip: {_scrollLineSkip.ToString()}\n").Append($"Auto Resize? {_autoResize.ToString()}\n").Append(
$"Show Tools? {_showTools.ToString()}\n").Append($"Show Legend? {_showLegend.ToString()}\n").Append($"Show Resources? {_showResources.ToString()}\n").Append(
$"Enable Co-op? {ǐ.ToString()}\n").Append($"Auto Deconstruct? {_autoDecon.ToString()}\n").Append($"Manage Inventory? {Ǵ.ToString()}\n\n").Append(d);Echo(
ǥ.ToString());}const string d="-------- Commands --------\nThese may also be configured in\nthe Custom Data of this block.\n\nAuto Resize - toggles auto LCD\n  font resizing\nShow Tools - show / hide tools\nShow Legend - show / hide legend\nShow Resources - show / hide\n  resources\nLine Skip - set the number of lines\n  the display skips when scrolling\n  USAGE: Line Skip:3\nEnable Coop - toggle co-op mode"
;const double e=0.005;double f=0;double g=0;int h=0;void l(){if(++h<21)return;g=(1-e)*f+e*Runtime.LastRunTimeMs;f=g;}
string n(MyIni o,bool p){if(p){o.Clear();o.TryParse(Me.CustomData);}o.Set("Settings","Assembler Efficiency Multiplier",o.Get(
"Settings","Assembler Efficiency Multiplier").ToInt32(_efficiencySetting));o.Set("Settings","Inventory Group Name",o.Get(
"Settings","Inventory Group Name").ToString(_inventoryGroupName));o.Set("Settings","Use This Grid",o.Get("Settings",
"Use This Grid").ToBoolean(_useThisGrid));o.Set("Settings","LCD Tag",o.Get("Settings","LCD Tag").ToString(_lcdTag));o.Set("Settings",
"Multi Display Delimeter",o.Get("Settings","Multi Display Delimeter").ToString(_multiDisplayDelimeter.ToString()));o.Set("Settings",
"Use Sprite Mode",o.Get("Settings","Use Sprite Mode").ToBoolean(_useSpriteMode));o.Set("Settings","Auto Resize LCDs",p?_autoResize:o.Get(
"Settings","Auto Resize LCDs").ToBoolean(_autoResize));o.Set("Settings","Show Tools",p?_showTools:o.Get("Settings","Show Tools").
ToBoolean(_showTools));o.Set("Settings","Show Legend",p?_showLegend:o.Get("Settings","Show Legend").ToBoolean(_showLegend));o.Set
("Settings","Show Resources",p?_showResources:o.Get("Settings","Show Resources").ToBoolean(_showResources));o.Set(
"Settings","Scroll Line Skip",p?_scrollLineSkip:o.Get("Settings","Scroll Line Skip").ToInt32(_scrollLineSkip));o.Set("Settings",
"Enable Co-op Mode",p?ǐ:o.Get("Settings","Enable Co-op Mode").ToBoolean(ǐ));o.Set("Settings","Auto Disassemble",p?_autoDecon:o.Get(
"Settings","Auto Disassemble").ToBoolean(_autoDecon));o.Set("Settings","Manage Inventory",p?Ǵ:o.Get("Settings","Manage Inventory")
.ToBoolean(Ǵ));o.Set("Settings","Inventory LCD Tag",o.Get("Settings","Inventory LCD Tag").ToString(_invLCDTag));o.
SetSectionComment("Settings"," Welcome to the IQueue Production and Resource Manager by JTurp!\n Scroll passed the Settings section and you'll find the item lists.\n "
);o.SetComment("Settings","Assembler Efficiency Multiplier"," \n REQUIRED! Set your world's 'Assembler Efficiency' as found in\n the Advanced World Settings.\n NOTES: The game defaults to 3. 'Realistic' = 1\n "
);o.SetComment("Settings","Inventory Group Name"," \n There are three ways IQueue can grab blocks, pick -ONE-\n \n OPTION 1: Place -ALL- blocks (inventories, refineries,\n assemblers, and LCDs) you want this script to use in this group.\n "
);o.SetComment("Settings","Use This Grid"," \n OPTION 2: Set 'Use This Grid' to TRUE, and the script will\n use all appropriate blocks on the same grid as the PB.\n NOTE: This includes blocks on grids connected by rotor/piston.\n \n OPTION 3: Set 'Use This Grid' to FALSE, and the script will\n use all appropriate blocks from everywhere.\n "
);o.SetComment("Settings","LCD Tag",
" \n If using option 2 or 3, you MUST specify a tag so the script\n knows which LCDs to grab.\n ");o.SetComment("Settings","Multi Display Delimeter"," \n If you want the display to stretch across 2 LCDs, set this delimeter.\n The script will append the delimeter to the LCD Tag, above, creating\n a Multi-Display Tag that must be added to the NAME or CUSTOM\n DATA of the lcds to be used.\n Format:\n   <LCD Tag><Delimeter><Set Identifier><Position Identifier>\n Example:\n   Upper LCD: [IQueue]:A1\n   Lower LCD: [IQueue]:A2\n \n NOTE: Set Identifier must be a single character, and Position\n Identifier must be a '1' or '2'\n "
);o.SetComment("Settings","Auto Resize LCDs"," \n Set this to TRUE and the script will automatically resize the\n LCD font to try and fit everything on the screen.\n NOTE: If the fontsize drops below 0.3, the script will revert\n to scrolling text mode.\n "
);o.SetComment("Settings","Use Sprite Mode",
" \n Set whether or not to display info using Sprites instead of Monospace\n text.\n ");o.SetComment("Settings","Show Tools"," \n Set whether or not to show character tools on the inventory output.\n ");o.
SetComment("Settings","Show Legend"," \n Set whether or not to show the percentage icon legend on the display.\n ");o.SetComment(
"Settings","Show Resources"," \n Set whether or not to show resource counts on the display.\n ");o.SetComment("Settings",
"Scroll Line Skip"," \n Set the number of lines the display skips when scrolling.\n ");o.SetComment("Settings","Enable Co-op Mode",
" \n Set whether the script should place assemblers with an empty queue\n into co-op mode.\n ");o.SetComment("Settings","Manage Inventory"," \n Set whether the script should sort inventory items into designated\n cargo containers.\n NOTE: See Inventory Management section for details on how this works.\n "
);o.SetComment("Settings","Auto Disassemble",
" \n Set whether the script should automatically disassemble items that\n go over their set quota.\n ");o.SetComment("Settings","Inventory LCD Tag"," \n If the script will be managing inventory, any issues that are\n detected will be displayed on an LCD with this tag in its Name\n or Custom Data.\n NOTE: This tag is required regardless of which block fetching option\n you choose\n "
);foreach(var r in ǆ.Keys){if(r.StartsWith("Scrap"))continue;o.Set("Inventory Management",r,o.Get("Inventory Management",
r).ToString(r));}foreach(var r in ǃ.Keys)o.Set("Inventory Management",r,o.Get("Inventory Management",r).ToString(r));o.
SetSectionComment("Inventory Management"," \n The script can automatically sort items into designated cargo boxes.\n If you wish it, set Manage Inventory to TRUE and edit the values for\n the items below to be the TAG for the corresponding inventory item\n NOTE: Tags must be placed somewhere in the Custom Name or Custom\n Data of a cargo box - the script doesn't do this for you!\n "
);o.Set("Vanilla Items","Place Holder","Place Holder");if(o.ContainsSection("Ingot Display Names")){o.GetKeys(
"Ingot Display Names",ƴ);for(int Y=0;Y<ƴ.Count;Y++){var r=ƴ[Y];if(r.IsEmpty)continue;var s=o.Get(r).ToString("");if(string.IsNullOrWhiteSpace
(s))continue;var u=MyItemType.MakeIngot(r.Name);if(ǈ.ContainsKey(u))ǈ[u].ĥ=s;}}else{foreach(var v in ǈ){if(v.Value.ħ==
ItemType.ORE)continue;o.Set("Ingot Display Names",v.Key.SubtypeId,v.Value.ĥ);}}o.SetSectionComment("Ingot Display Names",
" \n You may adjust the way resources are displayed by changing\n the values below.\n ");if(o.ContainsSection("Modded Items")){o.GetKeys("Modded Items",ƴ);for(int Y=0;Y<ƴ.Count;Y++){var r=ƴ[Y];var W=o.Get(r)
.ToString("");if(!W.Contains(":"))continue;string[]U=W.Trim().Split(ǔ);string[]B=U[0].Split(ǖ);if(B.Length<4)continue;
string C=B[1].Trim().ToUpperInvariant();ItemType D;if(!Enum.TryParse(C,out D))continue;MyItemType E;string F=r.Name.Trim();if(
D==ItemType.AMMO)E=MyItemType.MakeAmmo(F);else if(D==ItemType.TOOL)E=MyItemType.MakeTool(F);else if(D==ItemType.COMPONENT
)E=MyItemType.MakeComponent(F);else if(D==ItemType.HYDROGENGAS)E=MyItemType.Parse($"{ǘ}{F}");else if(D==ItemType.
OXYGENGAS)E=MyItemType.Parse($"{Ǚ}{F}");else continue;int G;MyItemType H=MyItemType.Parse($"{Ǘ}{B[0].Trim()}");if(E.TypeId.
StartsWith("null")||E.SubtypeId.StartsWith("null")||H.TypeId.StartsWith("null")||H.SubtypeId.StartsWith("null")||!int.TryParse(B[3
].Trim(),out G))continue;string[]I=U[1].Split(ǒ);int J=I.Length;ƿ.Clear();for(int K=0;K<I.Length;K++){string[]L=I[K].
Split(Ǔ);string M=L[0].Trim();string N=M.ToUpperInvariant();Resource O;MyItemType P=new MyItemType();if(!Enum.TryParse(N,out
O)){bool Q=true;if(N.StartsWith("custom",StringComparison.OrdinalIgnoreCase)){var R=M.Split(Ǖ,StringSplitOptions.
RemoveEmptyEntries);if(R.Length==2){var S=R[1];if(ǃ.TryGetValue(S,out P)){Q=false;if(!Ǉ.ContainsKey(P)||!ǈ.ContainsKey(P)){ItemType T;var
A=P.GetItemInfo();if(A.IsAmmo)T=ItemType.AMMO;else if(A.IsComponent)T=ItemType.COMPONENT;else if(A.IsIngot)T=ItemType.
INGOT;else if(A.IsOre)T=ItemType.ORE;else if(A.IsTool)T=ItemType.TOOL;else if(P.SubtypeId.IndexOf("hydrogen",StringComparison
.OrdinalIgnoreCase)>=0)T=ItemType.HYDROGENGAS;else if(P.SubtypeId.IndexOf("oxygen",StringComparison.OrdinalIgnoreCase)>=0
)T=ItemType.OXYGENGAS;else T=ItemType.CUSTOM;Ǉ[P]=new Ĥ(P.SubtypeId,T);ǈ[P]=new Ĥ(P.SubtypeId,T);}}else if(ǆ.TryGetValue(
S,out P)){Q=false;if(!Ǉ.ContainsKey(P)||!ǈ.ContainsKey(P)){ItemType T;var A=P.GetItemInfo();if(A.IsAmmo)T=ItemType.AMMO;
else if(A.IsComponent)T=ItemType.COMPONENT;else if(A.IsIngot)T=ItemType.INGOT;else if(A.IsOre)T=ItemType.ORE;else if(A.
IsTool)T=ItemType.TOOL;else if(P.SubtypeId.IndexOf("hydrogen",StringComparison.OrdinalIgnoreCase)>=0)T=ItemType.HYDROGENGAS;
else if(P.SubtypeId.IndexOf("oxygen",StringComparison.OrdinalIgnoreCase)>=0)T=ItemType.OXYGENGAS;else T=ItemType.CUSTOM;Ǉ[P]
=new Ĥ(P.SubtypeId,T);ǈ[P]=new Ĥ(P.SubtypeId,T);}Ǆ[P]=Resource.CUSTOM;}}}if(Q)throw new Exception(
$"Error! Unable to parse the TYPE OF RESOURCE for the modded item '{B[0]}'!\n\n");}else P=ǅ[O];float º;if(!float.TryParse(L[1].Trim(),out º))continue;ƿ.Add(new KeyValuePair<MyItemType,float>(P,º/
_efficiencySetting));}float À;if(B.Length<5||!float.TryParse(B[4],out À))À=1;À=Math.Max(1,À);var Á=new Č(H,D,B[2].Trim(),G,true,À){Ľ=new
KeyValuePair<MyItemType,float>[J],};for(int K=0;K<ƿ.Count;K++)Á.Ľ[K]=ƿ[K];Č Â;if(Ǌ.TryGetValue(E,out Â)){Â.Ĳ=Á.Ĳ;Â.Ľ=Á.Ľ;Â.ĥ=Á.ĥ;Â.ħ
=Á.ħ;Â.ķ=true;}else Ǌ[E]=Á;ǃ[E.SubtypeId]=E;var Ã=new MyIniKey("Inventory Management",E.SubtypeId);if(!o.ContainsKey(Ã))o
.Set(Ã,E.SubtypeId);}}else o.Set("Modded Items","Place Holder","Add your items here.");foreach(var v in Ǌ){if(v.Value.ķ){
string Ä=v.Value.ĸ.SubtypeName;ItemType D=v.Value.ħ;if(p){string W=o.Get("Modded Items",v.Key.SubtypeId).ToString("");string[]
Å=W.Split(ǔ)[0].Split(ǒ);if(Å.Length>2){v.Value.ĥ=Å[2].Trim();int G;if(Å.Length>3&&int.TryParse(Å[3].Trim(),out G))v.
Value.Ĳ=G;}}ǁ.Clear().Append($"{Ä}, {D}, {v.Value.ĥ}, {v.Value.Ĳ.ToString()}, {v.Value.ĳ}:");for(int Y=0;Y<v.Value.Ľ.Length;Y
++){var Æ=v.Value.Ľ[Y];string Ç;Resource N;if(ǀ.TryGetValue(Æ.Key,out N))Ç=N.ToString();else Ç=$"Custom.{Æ.Key.SubtypeId}"
;ǁ.Append($" {Ç} = {(Æ.Value*_efficiencySetting).ToString()}");if(Y!=v.Value.Ľ.Length-1)ǁ.Append(',');}o.Set(
"Modded Items",v.Key.SubtypeId,ǁ.ToString());}else{string W=o.Get("Vanilla Items",v.Key.SubtypeId).ToString("");string[]Å=W.Split(ǒ);
if(Å.Length>2){v.Value.ĥ=Å[2].Trim();int G;if(Å.Length>3&&int.TryParse(Å[3].Trim(),out G))v.Value.Ĳ=G;}o.Set(
"Vanilla Items",v.Key.SubtypeId,$"{v.Value.ĸ.SubtypeName}, {v.Value.ħ}, {v.Value.ĥ}, {v.Value.Ĳ.ToString()}");}}o.SetSectionComment(
"Vanilla Items"," Vanilla items are stored in the following format:\n   <DefinitionId> = <BlueprintId>, <ItemType>, <DisplayName>, <Quota>\n You may update the <DisplayName> and <Quota> to your liking.\n \n WARNING: Changing the <DefinitionId>, <BlueprintId>, or <ItemType>\n WILL BREAK THE SCRIPT!\n "
);o.SetSectionComment("Modded Items"," You may add values to the dictionary, just ensure you have the\n proper information! Modded items also require the name and number\n of resources (iron, nickel, etc). The format is as follows:\n   <DefinitionId> = <BlueprintId>, <ItemType>, <DisplayName>, <Quota>, <Yield>: <Resource1> = <Num1>, <Resource2> = <Num2>\n Example:\n   DenseSteelPlate = DenseSteelPlate, Component, Dense Steel Plate, 2000: iron = 400\n \n NOTE: Definition Id, Blueprint Id, Item Type, Item Yield, and Resource\n information can be found in the .sbc files. The Yield value is optional and\n defaults to 1 if missing\n "
);o.Delete("Vanilla Items","Place Holder");return o.ToString();}void È(MyIni o){if(!o.TryParse(Me.CustomData))return;
_efficiencySetting=o.Get("Settings","Assembler Efficiency Multiplier").ToInt32(_efficiencySetting);_inventoryGroupName=o.Get("Settings",
"Inventory Group Name").ToString(_inventoryGroupName);_lcdTag=o.Get("Settings","LCD Tag").ToString(_lcdTag);_multiDisplayDelimeter=o.Get(
"Settings","Multi Display Delimeter").ToChar(_multiDisplayDelimeter);_useThisGrid=o.Get("Settings","Use This Grid").ToBoolean(
_useThisGrid);_useSpriteMode=o.Get("Settings","Use Sprite Mode").ToBoolean(_useSpriteMode);_autoResize=o.Get("Settings",
"Auto Resize LCDs").ToBoolean(_autoResize);_showTools=o.Get("Settings","Show Tools").ToBoolean(_showTools);_showLegend=o.Get("Settings",
"Show Legend").ToBoolean(_showLegend);_showResources=o.Get("Settings","Show Resources").ToBoolean(_showResources);_scrollLineSkip=o.
Get("Settings","Scroll Line Skip").ToInt32(_scrollLineSkip);ǐ=o.Get("Settings","Enable Co-op mode").ToBoolean(ǐ);Ǵ=o.Get(
"Settings","Manage Inventory").ToBoolean(Ǵ);_autoDecon=o.Get("Settings","Auto Disassemble").ToBoolean(_autoDecon);_invLCDTag=o.Get
("Settings","Inventory LCD Tag").ToString(_invLCDTag);if(o.ContainsSection("Ingot Display Names")){o.GetKeys(
"Ingot Display Names",ƴ);for(int Y=0;Y<ƴ.Count;Y++){var r=ƴ[Y];if(r.IsEmpty)continue;var s=o.Get(r).ToString("");if(string.IsNullOrWhiteSpace
(s))continue;var u=MyItemType.MakeIngot(r.Name);if(ǈ.ContainsKey(u))ǈ[u].ĥ=s;}}}Program(){Runtime.UpdateFrequency=
UpdateFrequency.Once;Ŭ=ü();Ⱦ=Color.Black;ȯ=new Color(220,220,220);ȼ=Color.Red;Ȱ=Color.Yellow;ȱ=Color.DodgerBlue;Ȳ=Color.Green;}void
Main(string w,UpdateType y){if((!Ǐ&&Runtime.TimeSinceLastRun.TotalSeconds==0)||y==UpdateType.Mod)return;try{ƍ(w,y);}catch(
Exception e){var z=new StringBuilder();z.Append("Exception Message:\n").Append($"{e.Message}\n\n").Append("Stack trace:\n").
Append($"{e.StackTrace}\n");var ª=z.ToString();var µ=GridTerminalSystem.GetBlockWithName("Debug LCD")as IMyTextPanel;Echo(ª);
if(µ!=null){µ.ContentType=ContentType.TEXT_AND_IMAGE;µ.TextPadding=0;µ.WriteText(ª,append:false);}throw;}}
    enum Resource { IRON, NICKEL, SILICON, URANIUM, PLATINUM, MAGNESIUM, SILVER, COBALT, GOLD, STONE, SCRAP, SCRAPINGOT, CUSTOM }
    enum ItemType { TOOL, COMPONENT, AMMO, HYDROGENGAS, OXYGENGAS, ORE, INGOT, CUSTOM }
class œ{public HashSet<MyDefinitionId>Ŕ=new HashSet<MyDefinitionId>(MyDefinitionId.Comparer){{MyDefinitionId.Parse(
"MyObjectBuilder_TextPanel/LargeBlockCorner_LCD_1")},{MyDefinitionId.Parse("MyObjectBuilder_TextPanel/LargeBlockCorner_LCD_2")},{MyDefinitionId.Parse(
"MyObjectBuilder_TextPanel/LargeBlockCorner_LCD_Flat_1")},{MyDefinitionId.Parse("MyObjectBuilder_TextPanel/LargeBlockCorner_LCD_Flat_2")},{MyDefinitionId.Parse(
"MyObjectBuilder_TextPanel/SmallBlockCorner_LCD_1")},{MyDefinitionId.Parse("MyObjectBuilder_TextPanel/SmallBlockCorner_LCD_2")},{MyDefinitionId.Parse(
"MyObjectBuilder_TextPanel/SmallBlockCorner_LCD_Flat_1")},{MyDefinitionId.Parse("MyObjectBuilder_TextPanel/SmallBlockCorner_LCD_Flat_2")}};public IMyTextPanel ŕ,Ŗ;public bool
ņ,ŗ,Ř,ř;public Ł Ś,ś;public bool Ŝ=>ŕ!=null;public int Ŏ=0;public œ(IMyTextPanel Œ,IMyTextPanel Ő,bool Ŀ,bool ŀ){ŕ=Œ;Ŗ=Ő;
ņ=Ŀ;ŗ=ŀ;Ř=Ŕ.Contains(Œ.BlockDefinition);ř=Œ.BlockDefinition.SubtypeName.EndsWith("5x3")||Œ.BlockDefinition.SubtypeName.
EndsWith("5x5");if(ŕ!=null)Ś=new Ł(Œ,Œ,Ŕ,Ŧ:ņ);if(Ŀ&&Ŗ!=null)ś=new Ł(Ő,Ő,Ŕ,Ŧ:ņ);}}class Ł{public IMyTerminalBlock ł;public
IMyTextSurface Ń;public bool ń,Ņ,ņ,Ň,ň;public Vector2 ŉ,Ŋ;public Vector2 ŋ,Ō;public Vector2 ō;public int Ŏ=0;public Ł(IMyTextSurface ŏ
,IMyTerminalBlock Ē,HashSet<MyDefinitionId>ľ,bool ő=false,bool ŝ=false,bool Ŧ=false,bool ŧ=false){ł=Ē;Ń=ŏ;ņ=Ŧ;ń=ő;Ņ=ŝ;Ň=ŧ
;var Ũ=Math.Min(ŏ.SurfaceSize.X,ŏ.SurfaceSize.Y);ō=ŏ.TextureSize*0.5f;ŉ=new Vector2(Ũ,Ũ);ŋ=ŏ.SurfaceSize;var D=Ē.
BlockDefinition;ň=ľ?.Contains(D)??false;if(ŧ||D.SubtypeName=="TransparentLCDSmall"){ŉ*=0.9f;ŋ*=0.9f;}Ŋ=ō-(ŉ*0.5f);Ō=ō-(ŋ*0.5f);}}class
ũ{IMyProgrammableBlock Ū=>Ů.Me;IEnumerator<string>ū;IEnumerator<bool>Ŭ;bool ŭ=false;Program Ů;MyIni ů=new MyIni();
StringBuilder Ź=new StringBuilder();StringBuilder Ű=new StringBuilder();List<MyIniKey>ű=new List<MyIniKey>();List<IMyTerminalBlock>Ų=
new List<IMyTerminalBlock>();List<IMyTerminalBlock>ų=new List<IMyTerminalBlock>();Dictionary<MyItemType,List<
IMyTerminalBlock>>Ŵ=new Dictionary<MyItemType,List<IMyTerminalBlock>>();Dictionary<long,HashSet<MyItemType>>ŵ=new Dictionary<long,
HashSet<MyItemType>>();Dictionary<long,IMyTerminalBlock>Ŷ=new Dictionary<long,IMyTerminalBlock>();Dictionary<string,HashSet<
MyItemType>>ŷ=new Dictionary<string,HashSet<MyItemType>>();Dictionary<MyItemType,MyItemInfo>Ÿ=new Dictionary<MyItemType,MyItemInfo
>();HashSet<MyItemType>ź=new HashSet<MyItemType>();HashSet<MyItemType>ť=new HashSet<MyItemType>();HashSet<MyItemType>Ş=
new HashSet<MyItemType>();HashSet<MyItemType>Ť=new HashSet<MyItemType>();public string ş=>Ű?.ToString()??"";public int Š=0;
public ũ(Program Į){Ů=Į;ū=ĕ();Ŭ=š();}IEnumerator<bool>š(){while(true){Ź.Clear();while(true){Ź.Clear();while(true){Ź.Clear();
while(true){Ź.Clear();var Ţ=Ů.Ǳ;if(string.IsNullOrWhiteSpace(Ţ)||!ů.TryParse(Ţ))Ź.Append("Config is null or empty\n");else if
(!ů.ContainsSection("Inventory Management"))Ź.Append("Config missing section 'Inventory Management'\n");else ů.GetKeys(
"Inventory Management",ű);if(ű.Count>0)break;Ź.Append("Config found 0 entries in section 'Inventory Management'\n");Ű.Clear().Append(
$"Init Err:\n{Ź.ToString()}");yield return false;}ŷ.Clear();ŵ.Clear();for(int Y=ű.Count-1;Y>=0;Y--){var r=ű[Y];var u=r.Name.Trim();var ţ=ů.Get(r).
ToString().Trim();if(string.IsNullOrWhiteSpace(u)||string.IsNullOrWhiteSpace(ţ))continue;HashSet<MyItemType>Ć;if(!ŷ.TryGetValue(
ţ,out Ć)){Ć=new HashSet<MyItemType>();ŷ[ţ]=Ć;}if(Ů.ǆ.ContainsKey(u))Ć.Add(Ů.ǆ[u]);else if(Ů.ǃ.ContainsKey(u))Ć.Add(Ů.ǃ[u]
);}if(ŷ.Count>0)break;Ź.Append("Type Map Dict contains 0 entries\n");Ű.Clear().Append($"Init Err:\n{Ź.ToString()}");yield
return false;}yield return false;ų.Clear();Ŷ.Clear();Ų.Clear();var ö=Ů.GridTerminalSystem.GetBlockGroupWithName(Ů.
_inventoryGroupName);if(ö!=null){ö.GetBlocksOfType<IMyTerminalBlock>(null,ë=>đ(ë));}else if(Ů._useThisGrid){Ů.GridTerminalSystem.
GetBlocksOfType<IMyTerminalBlock>(null,ë=>ë.IsSameConstructAs(Ū)&&đ(ë));}else{Ů.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(
null,ë=>đ(ë));}if(ų.Count>0&&Ų.Count>0)break;if(ų.Count==0)Ź.Append("No inventory blocks found\n");if(Ų.Count==0)Ź.Append(
"No storage blocks found\n");Ű.Clear().Append($"Init Err:\n{Ź.ToString()}");yield return false;}yield return false;ź.Clear();foreach(var N in Ů.ǆ.
Values){if(N.SubtypeId.StartsWith("Scrap"))continue;ź.Add(N);}foreach(var s in Ů.ǃ.Values)ź.Add(s);int Ė=0;Ŵ.Clear();yield
return false;foreach(var v in ŷ){for(int Y=Ų.Count-1;Y>=0;Y--){var ì=Ų[Y];if(ì.Ĭ(Ů)){Ų.RemoveAtFast(Y);continue;}if(ì.
CustomName.Contains(v.Key)||ì.CustomData.Contains(v.Key)){Ŷ.Remove(ì.EntityId);HashSet<MyItemType>Ć;if(!ŵ.TryGetValue(ì.EntityId,
out Ć)){Ć=new HashSet<MyItemType>();ŵ[ì.EntityId]=Ć;}foreach(var s in v.Value){Ć.Add(s);ź.Remove(s);List<IMyTerminalBlock>Ď
;if(!Ŵ.TryGetValue(s,out Ď)){Ď=new List<IMyTerminalBlock>(){ì};Ŵ[s]=Ď;}else{bool ď=false;for(int K=Ď.Count-1;K>=0;K--){
var Đ=Ď[K];if(Đ.Ĭ(Ů)){Ď.RemoveAtFast(K);continue;}if(Đ.EntityId==ì.EntityId){ď=true;break;}}if(!ď)Ď.Add(ì);}}}}if(++Ė>5){Ė=
0;yield return false;}}if(Ŵ.Count==0){Ź.Append("Inventory Map Dict contains 0 entries\n");Ű.Clear().Append(
$"Init Err:\n{Ź.ToString()}");}yield return Ŵ.Count>0;}}bool đ(IMyTerminalBlock Ē){if(!Ē.HasInventory||Ē is IMyLargeTurretBase||Ē is IMyShipWelder||
Ē is IMyReactor||Ē is IMyGasGenerator)return false;ų.Add(Ē);if(Ē is IMyCargoContainer){Ŷ[Ē.EntityId]=Ē;Ų.Add(Ē);}return
false;}public void ē(){if(!ŭ){if(Ŭ==null)Ŭ=š();if(!Ŭ.MoveNext()){Ŭ.Dispose();Ŭ=š();}ŭ=Ŭ.Current;return;}if(ū==null)ū=ĕ();Š+=
10;if(Š<60||Ů.Ư.Count>0||Ů.g>0.05)return;if(!ū.MoveNext()){ū.Dispose();ū=ĕ();}}int Ĕ;IEnumerator<string>ĕ(){while(true){Ź.
Clear();foreach(var s in ź)Ź.Append($"{s.SubtypeId} has no designated storage\n");int Ė=0;float ė=0;bool Ę=Ŷ.Count>0;bool ę=
true;ť.Clear();Ť.Clear();Ş.Clear();if(!Ę)Ź.Append("System requires an unused cargo box\n");else{foreach(var Ċ in Ŷ.Values){
if(Ċ.Ĭ(Ů))continue;var ċ=Ċ.GetInventory();if(!ċ.IsFull&&(float)ċ.CurrentVolume/(float)ċ.MaxVolume<0.95f){ę=false;break;}}
if(ę)Ź.Append("Unused cargo boxes are all full\n");yield return null;}bool Ě=false;for(int Y=ų.Count-1;Y>=0;Y--){var ě=ų[Y
];if(ě.Ĭ(Ů)){ų.RemoveAtFast(Y);continue;}var ĝ=ě.GetInventory(ě.InventoryCount-1);if(ĝ.ItemCount==0)continue;if(ě is
IMyGasTank||ě is IMyGasGenerator){if(!Ě){Ĕ++;Ě=true;}if(Ĕ<3)continue;}var Ĝ=ě is IMyAssembler;if(Ĝ&&Ů.ǯ?.EntityId==ě.EntityId)
continue;bool č=false;for(int K=ĝ.ItemCount-1;K>=0;K--){if(ě.Ĭ(Ů)){ų.RemoveAtFast(Y);break;}var ÿ=ĝ.GetItemAt(K);if(!ÿ.HasValue)
continue;List<IMyTerminalBlock>Ā;if(!Ŵ.TryGetValue(ÿ.Value.Type,out Ā)){ė=(float)Ů.Runtime.CurrentInstructionCount/Ů.Runtime.
MaxInstructionCount;if(++Ė>50||ė>0.1f){č=false;Ė=0;yield return null;}continue;}MyItemInfo ā;if(!Ÿ.TryGetValue(ÿ.Value.Type,out ā)){ā=ÿ.
Value.Type.GetItemInfo();Ÿ[ÿ.Value.Type]=ā;}bool Ă=false;for(int Ï=Ā.Count-1;Ï>=0;Ï--){var ì=Ā[Ï];if(ì.Ĭ(Ů)){Ā.RemoveAtFast(Ï
);continue;}if(ì?.EntityId==ě.EntityId){Ă=true;break;}}if(Ă||Ā.Count==0){ė=(float)Ů.Runtime.CurrentInstructionCount/Ů.
Runtime.MaxInstructionCount;if(++Ė>50||ė>0.1f){č=false;Ė=0;yield return null;}continue;}bool ă=false;bool Ą=false;for(int Ï=Ā.
Count-1;Ï>=0;Ï--){var ì=Ā[Ï];if(ì.Ĭ(Ů)){Ā.RemoveAtFast(Ï);continue;}var ą=ì?.GetInventory(0);if(ą==null){Ā.RemoveAtFast(Y);
continue;}if(Ę&&!ę&&ą.ItemCount>0){HashSet<MyItemType>Ć;ŵ.TryGetValue(ì.EntityId,out Ć);for(int ć=ą.ItemCount-1;ć>=0;ć--){var Ĉ=
ą.GetItemAt(ć);if(!Ĉ.HasValue||Ć?.Contains(Ĉ.Value.Type)==true)continue;MyItemInfo ĉ;if(!Ÿ.TryGetValue(Ĉ.Value.Type,out ĉ
)){ĉ=Ĉ.Value.Type.GetItemInfo();Ÿ[Ĉ.Value.Type]=ĉ;}ę=true;foreach(var Ċ in Ŷ.Values){if(Ċ.Ĭ(Ů))continue;var ċ=Ċ?.
GetInventory();if(ċ==null||ċ.IsFull||ċ.CurrentVolume+(MyFixedPoint)ĉ.Volume>ą.MaxVolume)continue;ę=false;č=true;Š=0;if(ċ.
TransferItemFrom(ą,ć,null,true,Ĉ.Value.Amount))break;else{Ş.Add(Ĉ.Value.Type);Ė=0;yield return null;}}ė=(float)Ů.Runtime.
CurrentInstructionCount/Ů.Runtime.MaxInstructionCount;if(č||++Ė>20||ė>0.1f){č=false;Ė=0;yield return null;}}}if(ą.IsFull||ą.CurrentVolume+(
MyFixedPoint)ā.Volume>ą.MaxVolume)continue;ă=true;č=true;Š=0;if(ą.TransferItemFrom(ĝ,K,null,true,ÿ.Value.Amount)){Ą=true;break;}else
{Ė=0;yield return null;}}if(!ă)ť.Add(ÿ.Value.Type);else if(!Ą)Ť.Add(ÿ.Value.Type);ė=(float)Ů.Runtime.
CurrentInstructionCount/Ů.Runtime.MaxInstructionCount;if(č||++Ė>20||ė>0.1f){č=false;Ė=0;yield return null;}}Ė=0;yield return null;}if(Ĕ>2)Ĕ=0;
foreach(var s in ť)Ź.Append($"{s.SubtypeId} requires more storage space\n");foreach(var s in Ť)Ź.Append(
$"{s.SubtypeId} cannot reach its destination\n");foreach(var s in Ş)Ź.Append($"Unable to move {s.SubtypeId} to extra cargo\n");ŭ=false;Ű.Clear().Append(Ź.ToString());
yield return null;}}}class Č{public int Ğ;public int İ;public int ı;public int Ĳ;public float ĳ;public bool Ĵ;public bool ĵ;
public bool Ķ;public bool ķ;public string ĥ;public ItemType ħ;public MyDefinitionId ĸ;public KeyValuePair<MyItemType,float>[]Ľ
;public Č(MyItemType Ĺ,ItemType D,string ĭ,int G,bool ĺ,float À=1){ĸ=Ĺ;ĥ=ĭ;Ĳ=G;Ĵ=false;ĵ=false;Ğ=0;ı=0;İ=0;ħ=D;ķ=ĺ;ĳ=À;}}
class Ļ{public Resource ļ;public float ĳ;public float į;public Ļ(Resource ĭ,float À){ļ=ĭ;ĳ=À;į=0f;}}struct ğ{public float Ġ,ġ
;public ğ(float Ģ,float ģ){Ġ=Ģ;ġ=ģ;}}class Ĥ{public string ĥ;public float Ħ;public ItemType ħ;public List<Ļ>Ĩ=new List<Ļ>
();public Ĥ(string ĩ,ItemType D,params Ļ[]Ī){Ħ=0f;ħ=D;ĥ=ĩ;foreach(var Ü in Ī)Ĩ.Add(Ü);}}
}static class ī{public static bool Ĭ(this IMyTerminalBlock Ē,Program Į){if(Ē==null||Ē.WorldMatrix.Translation==Vector3D.
Zero)return true;return Į.GridTerminalSystem.GetBlockWithId(Ē.EntityId)!=Ē;}