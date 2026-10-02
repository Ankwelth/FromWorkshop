/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */
MyIni ʀ;List<string>ɿ=new List<string>();List<IMyTextPanel>ɾ=new List<IMyTextPanel>();List<IMyTextPanel>ɽ=new List<
IMyTextPanel>();List<IMyTextPanel>ɼ=new List<IMyTextPanel>();List<IMyTextPanel>ɻ=new List<IMyTextPanel>();List<IMyTextPanel>ɺ=new
List<IMyTextPanel>();List<IMyTextPanel>ɹ=new List<IMyTextPanel>();List<IMyTextPanel>ɸ=new List<IMyTextPanel>();List<
IMyTextPanel>ɷ=new List<IMyTextPanel>();List<IMyTextPanel>ɶ=new List<IMyTextPanel>();List<IMyTextPanel>ɵ=new List<IMyTextPanel>();
List<IMyCockpit>ɴ=new List<IMyCockpit>();List<IMyTextPanel>ɳ=new List<IMyTextPanel>();List<IMyTextPanel>ɲ=new List<
IMyTextPanel>();List<IMyTextPanel>ʁ=new List<IMyTextPanel>();List<IMyTextPanel>ɱ=new List<IMyTextPanel>();List<IMyTextPanel>ʂ=new
List<IMyTextPanel>();List<IMyTextPanel>ʕ=new List<IMyTextPanel>();List<IMyTextPanel>ʓ=new List<IMyTextPanel>();List<
IMyTextPanel>ʒ=new List<IMyTextPanel>();List<IMyTextPanel>ʑ=new List<IMyTextPanel>();List<IMyTextPanel>ʐ=new List<IMyTextPanel>();
List<IMyTextPanel>ʏ=new List<IMyTextPanel>();List<IMyTextPanel>ʎ=new List<IMyTextPanel>();List<IMyTextPanel>ʍ=new List<
IMyTextPanel>();Dictionary<string,string>ʔ=new Dictionary<string,string>();List<IMyPowerProducer>ʌ=new List<IMyPowerProducer>();List
<IMyAssembler>ʊ=new List<IMyAssembler>();List<IMyRefinery>ʉ=new List<IMyRefinery>();List<IMyShipConnector>ʈ=new List<
IMyShipConnector>();List<IMyCockpit>ʇ=new List<IMyCockpit>();List<IMyCryoChamber>ʆ=new List<IMyCryoChamber>();List<IMyConveyorSorter>ʅ=
new List<IMyConveyorSorter>();List<IMyGasGenerator>ʄ=new List<IMyGasGenerator>();List<IMyCargoContainer>ʃ=new List<
IMyCargoContainer>();List<IMyGasTank>ʋ=new List<IMyGasTank>();List<IMyGasTank>ɰ=new List<IMyGasTank>();List<IMyRadioAntenna>ɥ=new List<
IMyRadioAntenna>();List<IMyTerminalBlock>ɚ=new List<IMyTerminalBlock>();List<string>ə;List<string>ɘ=new List<string>();List<string>ɗ=
new List<string>();List<ȳ>ɖ;float ɕ=0;DateTime ɔ=DateTime.Now;DateTime ɓ=DateTime.Now;int ɒ=0,ɑ=0,ɐ=0,ɏ=0,Ɏ=1,ɍ=0,Ɍ=1,ɛ=1,ɋ
=1,ɝ=1,ɯ=1,ɭ=1,ɬ=1,ɫ=0,ɪ=0,ɩ=0,ɨ=1,ɧ=1,ɮ=1,ɦ=1,ɤ=20;const int ɣ=4,ɢ=7,ɡ=ɣ*ɢ,ɠ=3,ɟ=6,ɞ=ɠ*ɟ,ɜ=20,ʖ=10;const string ʨ=
"Information",ˑ="Function_On_Off(Y/N)",ˏ="Translate_List",ˎ="AutoProduction_List",ˍ="Length";const string ˌ="BroadCastConnectorGPS",ˋ
="ShowOverall",ˊ="ShowItems",ˉ="ShowFacilities",ˈ="AutoProduction",ˇ="AssemblerClear",ˆ="ConnectorClear",ˁ=
"CyroChamberClear",ˀ="SorterClear",ʿ="HydrogenBottleRefill",ʾ="OxygenBottleRefill",ʽ="CombiningLikeTerms",ː="CombinedRefining",ʼ=
"ShowCombinedRefining",ˠ="ShowCargoContainerRatio",Έ="DirectionalSign",ͽ="WriteItemText";bool ͼ=true,ͻ=true,ͺ=false,ͷ=false,Ͷ=false,ʹ=true,Ά=
false,ͳ=false,ͱ=true,Ͱ=true;const string ˮ="CustomName";const string ˬ="VolumeThreshold",ˤ="Facility_Display_Amount(2~20)",ˣ=
"Refresh_Rate(F_FF_FFF)";const string ˢ="Ore",ˡ="Combined_Mode",Ͳ="Default_Amount",ʻ="Refinery_ID",ʰ="Page",ʦ="Item_ID";const string ʥ=
"Panel_Information",ʤ="Amount";const string ʣ="Item_Type",ʢ="Category",ʡ="Ore",ʠ="Ingot",ʟ="Component",ʞ="AmmoMagazine",ʝ="Item_Amount_1",ʜ
="Item_Amount_2",ʛ="Time",ʚ="Time1",ʙ="Time2",ʘ="ProductionAmount";const string ʧ="Combined_Refining";const string ʗ=
"Settings",ʩ="Left_Right_Offset",ʺ="Up_Down_Offset",ʸ="Up",ʷ="Down",ʶ="Left",ʵ="Right",ʴ="Line_1",ʳ="Line_2",ʲ="Direction(-1_0_1)"
;string ʹ="";string[]ʱ={ˌ,ˋ,ˊ,ˉ,ˈ,ˇ,ˆ,ˁ,ˀ,ʿ,ʾ,ʽ,ˠ,ː,Έ,ͽ,};struct ʯ{public string Ê;public string ʮ;public double ș;public
double ʭ;public double ʬ;public DateTime ʫ;public DateTime ʪ;}ʯ[]Ɋ;ʯ[]Ƞ;ʯ[]ȸ;ʯ[]Ȟ;ʯ[]ȝ;struct Ȝ{public string ț;public string
Ț;public double ș;}Ȝ[]Ș;struct ȗ{public bool Ȗ;public string Ê;public bool ȕ;public bool Ȕ;public bool ȓ;public string Ȓ;
public double ȑ;public string ȟ;}ȗ[]Ȑ;ȗ[]ȡ;class ȳ{public string ȱ;public int Ȱ;public Dictionary<string,double>ȯ=new
Dictionary<string,double>();}Dictionary<string,double>Ȯ=new Dictionary<string,double>();Dictionary<string,double>ȭ;Dictionary<
string,double>Ȭ;Dictionary<string,double>ȫ;Dictionary<string,double>Ȳ=new Dictionary<string,double>();Dictionary<string,double
>Ȫ;Dictionary<string,MyIni>ȩ;Dictionary<string,string>Ȩ=new Dictionary<string,string>();Dictionary<long,float>ȧ=new
Dictionary<long,float>();Color Ȧ=new Color(10,20,40);Color ȥ=new Color(10,5,2);Color Ȥ=new Color(5,2,15);Color ȣ=new Color(230,255
,255);Color Ȣ=new Color(30,50,90);Color ȏ=new Color(50,40,20);Color Ȏ=new Color(35,25,45);Color ȋ=new Color(11,25,25);
Color ȍ=new Color(0,60,60);Program(){Echo("Program");Ȍ();ɇ();ħ();Ǵ();Ƀ();Ⱦ();ȼ();Ʌ(Ȯ);Ɉ();ϯ();æ();å();Ɇ();}void Save(){}void
Ȍ(){Echo("Build_Block_List");GridTerminalSystem.GetBlocksOfType(ɾ,Ƒ=>Ƒ.IsSameConstructAs(Me));GridTerminalSystem.
GetBlocksOfType(ɶ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Overall_Display"));GridTerminalSystem.GetBlocksOfType(ɽ,Ƒ=>Ƒ.
IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɼ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&
Ƒ.CustomName.Contains("LCD_Ore_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɻ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("LCD_Ingot_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɺ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName
.Contains("LCD_Component_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɹ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("LCD_AmmoMagazine_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɸ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("LCD_Refinery_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɷ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("LCD_Assembler_Inventory_Display:"));GridTerminalSystem.GetBlocksOfType(ɵ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("LCD_Combined_Refining_Display"));GridTerminalSystem.GetBlocksOfType(ɴ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("Cockpit_Combined_Refining"));GridTerminalSystem.GetBlocksOfType(ɳ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.
Contains("LCD_Directional_Sign"));GridTerminalSystem.GetBlocksOfType(ɲ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains(
"LCD_Hydrogen_Sign"));GridTerminalSystem.GetBlocksOfType(ʁ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Oxygen_Sign"));
GridTerminalSystem.GetBlocksOfType(ɱ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Reactor_Sign"));GridTerminalSystem.
GetBlocksOfType(ʂ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Power_Sign"));GridTerminalSystem.GetBlocksOfType(ʕ,Ƒ=>Ƒ.
IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Refinery_Sign"));GridTerminalSystem.GetBlocksOfType(ʓ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.
CustomName.Contains("LCD_Assembler_Sign"));GridTerminalSystem.GetBlocksOfType(ʒ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains(
"LCD_Bedroom_Sign"));GridTerminalSystem.GetBlocksOfType(ʑ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Washroom_Sign"));
GridTerminalSystem.GetBlocksOfType(ʐ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Canteen_Sign"));GridTerminalSystem.
GetBlocksOfType(ʏ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_GasGenerator_Sign"));GridTerminalSystem.GetBlocksOfType(ʍ,Ƒ=>Ƒ
.IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_Server_Sign"));GridTerminalSystem.GetBlocksOfType(ʎ,Ƒ=>Ƒ.
IsSameConstructAs(Me)&&Ƒ.CustomName.Contains("LCD_CargoContainer_Sign"));GridTerminalSystem.GetBlocksOfType(ʊ,Ƒ=>Ƒ.IsSameConstructAs(Me))
;GridTerminalSystem.GetBlocksOfType(ʉ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&!Ƒ.BlockDefinition.ToString().Contains("Shield"));
GridTerminalSystem.GetBlocksOfType(ʈ,Ƒ=>Ƒ.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ʇ,Ƒ=>Ƒ.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ʆ,Ƒ=>Ƒ.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ʅ,Ƒ=>Ƒ.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ʌ,Ƒ=>Ƒ.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType(ʄ,Ƒ=>Ƒ.IsSameConstructAs(Me));
GridTerminalSystem.GetBlocksOfType(ʃ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.BlockDefinition.SubtypeName.ToString().Contains("Container"));
GridTerminalSystem.GetBlocksOfType(ʋ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.BlockDefinition.SubtypeName.ToString().Contains("Hydrogen"));
GridTerminalSystem.GetBlocksOfType(ɰ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.BlockDefinition.ToString().Contains("Oxygen")&&!Ƒ.BlockDefinition.
SubtypeName.ToString().Contains("Hydrogen"));GridTerminalSystem.GetBlocksOfType(ɥ,Ƒ=>Ƒ.IsSameConstructAs(Me));GridTerminalSystem.
GetBlocksOfType(ɚ,Ƒ=>Ƒ.IsSameConstructAs(Me)&&Ƒ.InventoryCount>0&&!Ƒ.BlockDefinition.ToString().Contains("Bathroom")&&!Ƒ.
BlockDefinition.ToString().Contains("Desk")&&!Ƒ.BlockDefinition.ToString().Contains("Console")&&!Ƒ.BlockDefinition.ToString().Contains(
"Couch")&&!Ƒ.BlockDefinition.ToString().Contains("Bridge")&&!Ƒ.BlockDefinition.ToString().Contains("Bridge")&&!Ƒ.
BlockDefinition.ToString().Contains("Standing")&&!Ƒ.BlockDefinition.ToString().Contains("Toilet")&&!Ƒ.BlockDefinition.ToString().
Contains("Bench")&&!Ƒ.BlockDefinition.ToString().Contains("Seat")&&!Ƒ.BlockDefinition.ToString().Contains("Passenger")&&!Ƒ.
BlockDefinition.ToString().Contains("Bed")&&!Ƒ.BlockDefinition.ToString().Contains("Bookshelf")&&!Ƒ.BlockDefinition.ToString().Contains
("Locker")&&!Ƒ.BlockDefinition.ToString().Contains("Rack"));}void ɇ(){Echo("Set_Default_Configuration");ȶ(ʨ,
"LCD_Overall_Display","LCD_Overall_Display | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Inventory_Display",
"LCD_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Ore_Inventory_Display","LCD_Ore_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(ʨ,
"LCD_Ingot_Inventory_Display","LCD_Ingot_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Component_Inventory_Display",
"LCD_Component_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(ʨ,"LCD_AmmoMagazine_Inventory_Display",
"LCD_AmmoMagazine_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Refinery_Inventory_Display","LCD_Refinery_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(
ʨ,"LCD_Assembler_Inventory_Display","LCD_Assembler_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");ȶ(ʨ,
"LCD_Combined_Refining_Display","LCD_Combined_Refining_Display | Fill In CustomName of Panel");ȶ(ʨ,"Cockpit_Combined_Refining",
"Cockpit_Combined_Refining | Fill In CustomName of Cockpit");ȶ(ʨ,"LCD_Directional_Sign","LCD_Directional_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Hydrogen_Sign",
"LCD_Hydrogen_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Oxygen_Sign","LCD_Oxygen_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Reactor_Sign",
"LCD_Reactor_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Power_Sign","LCD_Power_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Refinery_Sign",
"LCD_Refinery_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Assembler_Sign","LCD_Assembler_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Bedroom_Sign",
"LCD_Bedroom_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Washroom_Sign","LCD_Washroom_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Canteen_Sign",
"LCD_Canteen_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_GasGenerator_Sign","LCD_GasGenerator_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_CargoContainer_Sign",
"LCD_CargoContainer_Sign | Fill In CustomName of Panel");ȶ(ʨ,"LCD_Server_Sign","LCD_Server_Sign | Fill In CustomName of Panel");ȶ(ʨ,"Assemblers_CooperativeMode",
"CO_ON or CO_OFF | Fill In Argument of PB And Press Run");ȶ(ʨ,"Clear_Assembler_Queue","CLS | Fill In Argument of PB And Press Run");ȶ(ʨ,"LCD_Refresh",
"LCD_REF | Fill In Argument of PB And Press Run");ȶ(ʨ,"Reset_CustomData","DEFAULT | Fill In Argument of PB And Press Run");ȶ(ʨ,"IGCTAG","CHANNEL1");ȶ(ʨ,ˬ,"6000");ȶ(ʨ,ˤ,
"20");ȶ(ʨ,ˣ,"FF");ȴ(true);ȶ(ˢ,ˡ,"1");ȶ(ˢ,Ͳ,"10000");for(int ú=1;ú<=ʖ;ú++)ȶ(ˢ,ú.ToString(),"");ȶ(ˢ,ʻ,"1");ȶ(ˢ,ʰ,"1");ȶ(ˢ,ʦ,
"1");ȶ(ˏ,ˍ,"1");ȶ(ˏ,"1","AH_BoreSight:More");ȶ(ˎ,ˍ,"3");ȶ(ˎ,"1",
"MyObjectBuilder_Component/SteelPlate:MyObjectBuilder_BlueprintDefinition/SteelPlate:5000");ȶ(ˎ,"2","MyObjectBuilder_Component/Construction:MyObjectBuilder_BlueprintDefinition/ConstructionComponent:5000");ȶ(ˎ,
"3","MyObjectBuilder_Component/InteriorPlate:MyObjectBuilder_BlueprintDefinition/InteriorPlate:5000");foreach(var s in ɽ)s.
CustomData="";foreach(var s in ɼ)s.CustomData="";foreach(var s in ɻ)s.CustomData="";foreach(var s in ɺ)s.CustomData="";foreach(var
s in ɹ)s.CustomData="";foreach(var Ǔ in ɴ){ȶ(Ǔ,ʥ,ʧ,"0");}foreach(var s in ɵ){ȶ(s,ʥ,ʧ,"0");}}void Ɇ(){Echo(
"Initialize_Directional_Sign_Panels");foreach(var s in ɳ){s.ContentType=ContentType.SCRIPT;ȶ(s,ʗ,ʩ,"0");ȶ(s,ʗ,ʺ,"0");ȶ(s,ʸ,ʴ,"");ȶ(s,ʸ,ʳ,"");ȶ(s,ʸ,ʲ,"0");ȶ(
s,ʶ,ʴ,"");ȶ(s,ʶ,ʳ,"");ȶ(s,ʶ,ʲ,"0");ȶ(s,ʵ,ʴ,"");ȶ(s,ʵ,ʳ,"");ȶ(s,ʵ,ʲ,"0");ȶ(s,ʷ,ʴ,"");ȶ(s,ʷ,ʳ,"");ȶ(s,ʷ,ʲ,"0");}}void Ʌ(
Dictionary<string,double>Ó){Echo("Build_Method_Dic");for(int ú=1;ú<=ʖ;ú++){string Ă;Ă=Ϫ(ˢ,ú.ToString());ɉ(Ă,Ó);}foreach(var Ø in Ó
.Keys)Echo($"{Ø}={Ó[Ø]/1000000}");}void Ʌ(Dictionary<string,double>Ó,IMyRefinery Õ){for(int ú=1;ú<=ʖ;ú++){string Ă;Ă=Ϫ(Õ,
ˢ,ú.ToString());ɉ(Ă,Ó);}foreach(var Ø in Ó.Keys)Echo($"{Ø}={Ó[Ø]/1000000}");}void ɉ(string Ă,Dictionary<string,double>Ó){
string[]ǲ=Ă.Split(':');if(ǲ.Length==2&&!Ó.ContainsKey(ǲ[0])){Ó.Add(ǲ[0],Convert.ToDouble(ǲ[1])*1000000);}else if(ǲ.Length==2&&
Ó.ContainsKey(ǲ[0])){Ó[ǲ[0]]+=Convert.ToDouble(ǲ[1])*1000000;}}void Ɉ(){Echo("Set_Refineries_Defalut_CustomData");foreach
(var ã in ʉ){ȶ(ã,ˢ,ˡ,"1");for(int ú=1;ú<=ʖ;ú++)ȶ(ã,ˢ,ú.ToString(),"");}}void ȶ(string ȵ,string Ø,string Ħ){string ȷ=Ϫ(ȵ,Ø
);if(ȷ==""){Ⱥ(ȵ,Ø,Ħ);}}void ȶ(IMyRefinery ë,string ȵ,string Ø,string Ħ){string ȷ=Ϫ(ë,ȵ,Ø);if(ȷ=="")Ⱥ(ë,ȵ,Ø,Ħ);}void ȶ(
IMyCockpit ë,string ȵ,string Ø,string Ħ){string ȷ=Ϫ(ë,ȵ,Ø);if(ȷ=="")Ⱥ(ë,ȵ,Ø,Ħ);}void ȶ(IMyTextPanel ë,string ȵ,string Ø,string Ħ){
string ȷ=Ϫ(ë,ȵ,Ø);if(ȷ=="")Ⱥ(ë,ȵ,Ø,Ħ);}void ȴ(bool ȹ){if(ȹ){foreach(var Ʉ in ʱ){ȶ(ˑ,Ʉ,"Y");}}else{foreach(var Ʉ in ʱ){ȶ(ˑ,Ʉ,
"N");}}}void Ƀ(){Echo("Build_Sprite_List");if(ɾ.Count<1){if(Me.SurfaceCount>0){Me.GetSurface(0).GetSprites(ɿ);}}else{ɾ[0].
GetSprites(ɿ);}foreach(var ɂ in ɿ){if(ɂ.IndexOf('/')!=-1){string[]ȿ=ɂ.Split('/');string Ɂ=ȿ[ȿ.Length-1];if(!Ȩ.ContainsKey(Ɂ)){Ȩ.
Add(Ɂ,ɂ);}}}foreach(var ɀ in Ș){string[]ȿ=ɀ.Ț.Split('/');string Ɂ=ȿ[ȿ.Length-1];if(!Ȩ.ContainsKey(Ɂ)){Ȩ.Add(Ɂ,ɀ.ț);}}}void
Ⱦ(){Echo("Get_Function_Order");string Ă;Ă=Ϫ(ˑ,ː);if(Ă=="Y"){ɗ.Add(ː);ɘ.Add(ʼ);}Ă=Ϫ(ˑ,ˋ);if(Ă=="Y")ɘ.Add(ˋ);Ă=Ϫ(ˑ,ˊ);if(Ă
=="Y"){ɘ.Add(ˊ);ͺ=true;}Ă=Ϫ(ˑ,ˉ);if(Ă=="Y"){ɘ.Add(ˉ);ͷ=true;}Ă=Ϫ(ˑ,ˈ);if(Ă=="Y"){ɘ.Add(ˈ);ͳ=true;}int Ƚ=0;Ă=Ϫ(ˑ,ˇ);if(Ă==
"Y"){ɘ.Add(ˇ);Ƚ++;}Ă=Ϫ(ˑ,ˆ);if(Ă=="Y"){ɘ.Add(ˆ);Ƚ++;}Ă=Ϫ(ˑ,ˁ);if(Ă=="Y"){ɘ.Add(ˁ);Ƚ++;}Ă=Ϫ(ˑ,ˀ);if(Ă=="Y"){ɘ.Add(ˀ);Ƚ++;}Ă=
Ϫ(ˑ,ʽ);if(Ă=="Y"){ɘ.Add(ʽ);Ƚ++;}if(Ƚ==5)Ͷ=true;Ă=Ϫ(ˑ,ʿ);if(Ă=="Y")ɘ.Add(ʿ);Ă=Ϫ(ˑ,ʾ);if(Ă=="Y")ɘ.Add(ʾ);Ă=Ϫ(ˑ,Έ);if(Ă=="Y"
)ɘ.Add(Έ);Ă=Ϫ(ˑ,ˠ);if(Ă=="Y"){ɘ.Add(ˠ);Ά=true;}Ă=Ϫ(ˑ,ͽ);if(Ă!="Y")Ͱ=false;Ă=Ϫ(ˑ,ˌ);if(Ă!="Y")ʹ=false;}void ȼ(){Echo(
"Set_Refresh_Rate");string Ȼ=Ϫ(ʨ,ˣ);switch(Ȼ){case"0":Runtime.UpdateFrequency=UpdateFrequency.Once;break;case"F":Runtime.UpdateFrequency=
UpdateFrequency.Once|UpdateFrequency.Update100;break;case"FF":Runtime.UpdateFrequency=UpdateFrequency.Once|UpdateFrequency.Update10;
break;case"FFF":Runtime.UpdateFrequency=UpdateFrequency.Once|UpdateFrequency.Update1;break;}}void Ⱥ(string ȵ,string Ø,string
Ħ){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.TryParse(Me.CustomData,out Ĥ))throw new Exception(Ĥ.ToString());ʀ.Set(ȵ,Ø,Ħ);Me
.CustomData=ʀ.ToString();}void Ⱥ(IMyCargoContainer ë,string ȵ,string Ø,string Ħ){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.
TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ.ToString());ʀ.Set(ȵ,Ø,Ħ);ë.CustomData=ʀ.ToString();}void Ⱥ(IMyTextPanel ë,
string ȵ,string Ø,string Ħ){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ.ToString
());ʀ.Set(ȵ,Ø,Ħ);ë.CustomData=ʀ.ToString();}void Ⱥ(IMyRefinery ë,string ȵ,string Ø,string Ħ){ʀ=new MyIni();
MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ.ToString());ʀ.Set(ȵ,Ø,Ħ);ë.CustomData=ʀ.ToString();}void Ⱥ(
IMyCockpit ë,string ȵ,string Ø,string Ħ){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ
.ToString());ʀ.Set(ȵ,Ø,Ħ);ë.CustomData=ʀ.ToString();}string Ϫ(string ȵ,string Ø){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.
TryParse(Me.CustomData,out Ĥ))throw new Exception(Ĥ.ToString());string ϫ="";return ʀ.Get(ȵ,Ø).ToString(ϫ);}string Ϫ(
IMyShipConnector ë,string ȵ,string Ø){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ.ToString
());string ϫ="";return ʀ.Get(ȵ,Ø).ToString(ϫ);}string Ϫ(IMyCargoContainer ë,string ȵ,string Ø){ʀ=new MyIni();
MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ.ToString());string ϫ="";return ʀ.Get(ȵ,Ø).ToString(ϫ);}
string Ϫ(IMyTextPanel ë,string ȵ,string Ø){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new
Exception(Ĥ.ToString());string ϫ="";return ʀ.Get(ȵ,Ø).ToString(ϫ);}string Ϫ(IMyRefinery ë,string ȵ,string Ø){ʀ=new MyIni();
MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new Exception(Ĥ.ToString());string ϫ="";return ʀ.Get(ȵ,Ø).ToString(ϫ);}
string Ϫ(IMyCockpit ë,string ȵ,string Ø){ʀ=new MyIni();MyIniParseResult Ĥ;if(!ʀ.TryParse(ë.CustomData,out Ĥ))throw new
Exception(Ĥ.ToString());string ϫ="";return ʀ.Get(ȵ,Ø).ToString(ϫ);}void ϳ(){ɕ++;if(ɕ>=360f)ɕ=0f;IMyTextSurface s=Me.GetSurface(0)
;if(s==null)return;s.ContentType=ContentType.SCRIPT;RectangleF z=new RectangleF((s.TextureSize-s.SurfaceSize)/2f+new
Vector2(0,ɕ/360f),s.SurfaceSize),ϱ=new RectangleF(z.Position,new Vector2(z.Width,z.Height*0.6f)),ϰ=new RectangleF(z.Position+
new Vector2(0,ϱ.Height),new Vector2(z.Width,z.Height*0.4f));ϱ=В(ϱ,0.8f,2);MySpriteDrawFrame f=s.DrawFrame();ϩ(ref f,ϱ);Ϋ(
ref f,"Inventory_Management\nWith_Graphic_Interface_V1.3\nby Hi.James",ϰ,1f,ȣ,TextAlignment.CENTER);f.Dispose();}void ϯ(){
Echo("Set_Facility_Display_Amount");string Ϯ=Ϫ(ʨ,ˤ);int ϲ;if(int.TryParse(Ϯ,out ϲ)){if(ϲ<2)ɤ=2;else if(ϲ>20)ɤ=20;else ɤ=ϲ;}
else{ɤ=20;}}void ϭ(){foreach(var s in ɶ){if(!s.Enabled)continue;if(ȧ.ContainsKey(s.EntityId)){if(ȧ[s.EntityId]==0f)ȧ[s.
EntityId]=0.001f;else ȧ[s.EntityId]=0f;}else ȧ.Add(s.EntityId,0f);if(s.ContentType!=ContentType.SCRIPT)s.ContentType=ContentType
.SCRIPT;s.ScriptBackgroundColor=Ȧ;MySpriteDrawFrame f=s.DrawFrame();Ϭ(s,ref f);f.Dispose();}ͻ=true;}void Ϭ(IMyTextPanel s
,ref MySpriteDrawFrame f){RectangleF z=new RectangleF((s.TextureSize-s.SurfaceSize)/2f+new Vector2(0,ȧ[s.EntityId]),s.
SurfaceSize);float ĺ;if(z.Width<=z.Height)ĺ=z.Width;else ĺ=z.Height;RectangleF e=new RectangleF(new Vector2(z.Center.X-ĺ/2,z.Center
.Y-ĺ/2),new Vector2(ĺ,ĺ));έ(ref f,e,ȣ);float Â=ĺ/512f,І=0.6f,Ч=1.2f,Х=e.Height/7f,Ф=Х*0.02f,У=0.85f,Т=0.7f;Vector2 С=new
Vector2(Х-2f*Ф,Х-2f*Ф),Р=new Vector2(e.Width-2f*Х-2f*Ф,Х-2f*Ф),П=new Vector2(e.Width-Х-2f*Ф,Х-2f*Ф),О=new Vector2(e.Width-Х-2f*
Ф,(Х-2f*Ф)/2f);RectangleF Н=new RectangleF(new Vector2(e.X+Ф,e.Y+Ф),С),М=В(Н,У),Л=new RectangleF(new Vector2(e.Right-Н.
Width-Ф,Н.Y),С),К=В(Л,У),Й=new RectangleF(new Vector2(Н.X+Х,e.Y+Ф),Р);έ(ref f,Н,Ȧ);έ(ref f,Й,Ȧ);έ(ref f,Л,Ȧ);ϩ(ref f,М);ϩ(ref
f,К);Ϋ(ref f,s.GetOwnerFactionTag(),Й,2.3f*Â,ȣ,TextAlignment.CENTER);RectangleF И=new RectangleF(new Vector2(Н.X,Н.Y+Х),С
),З=В(И,У),Ц=new RectangleF(new Vector2(И.X+Х,И.Y),П),Ж=new RectangleF(new Vector2(Ц.X,Ц.Y),О),Ш=new RectangleF(new
Vector2(Ц.X,Ц.Center.Y),О);έ(ref f,И,Ȧ);Λ(ref f,"Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds",З,ȣ);Ϋ(ref f,ʃ.Count.
ToString(),З,І*Â,ȣ,TextAlignment.RIGHT);if(ͺ==false){Λ(ref f,"Danger",З,ȣ);}έ(ref f,Ц,Ȧ);string Τ,Σ;Ψ(out Τ,out Σ);Ѝ(ref f,Ц,Τ);
Ϋ(ref f,Τ,Ж,Ч*Â,ȣ,TextAlignment.CENTER);Ϋ(ref f,Σ,Ш,Ч*Â,ȣ,TextAlignment.CENTER);RectangleF и=new RectangleF(new Vector2(И
.X,И.Y+Х),С),з=В(и,У),ж=new RectangleF(new Vector2(и.X+Х,и.Y),П),е=new RectangleF(new Vector2(ж.X,ж.Y),О),д=new
RectangleF(new Vector2(ж.X,ж.Center.Y),О);έ(ref f,и,Ȧ);έ(ref f,ж,Ȧ);Λ(ref f,"IconHydrogen",з,ȣ);Ϋ(ref f,ʋ.Count.ToString(),з,І*Â,ȣ
,TextAlignment.RIGHT);Φ(ʋ,out Τ,out Σ);Ѝ(ref f,ж,Τ);Ϋ(ref f,Τ,е,Ч*Â,ȣ,TextAlignment.CENTER);Ϋ(ref f,Σ,д,Ч*Â,ȣ,
TextAlignment.CENTER);RectangleF г=new RectangleF(new Vector2(и.X,и.Y+Х),С),й=В(г,У),в=new RectangleF(new Vector2(г.X+Х,г.Y),П),а=new
RectangleF(new Vector2(в.X,г.Y),О),Я=new RectangleF(new Vector2(в.X,г.Center.Y),О);έ(ref f,г,Ȧ);έ(ref f,в,Ȧ);Λ(ref f,"IconOxygen",
й,ȣ);Ϋ(ref f,ɰ.Count.ToString(),й,І*Â,ȣ,TextAlignment.RIGHT);Φ(ɰ,out Τ,out Σ);Ѝ(ref f,в,Τ);Ϋ(ref f,Τ,а,Ч*Â,ȣ,
TextAlignment.CENTER);Ϋ(ref f,Σ,Я,Ч*Â,ȣ,TextAlignment.CENTER);RectangleF Ю=new RectangleF(new Vector2(г.X,г.Y+Х),С),Э=В(Ю,У),Ь=new
RectangleF(new Vector2(Ю.X+Х,Ю.Y),П),Ы=new RectangleF(new Vector2(Ь.X,Ь.Y),О),Ъ=new RectangleF(new Vector2(Ь.X,Ь.Center.Y),О);έ(
ref f,Ю,Ȧ);έ(ref f,Ь,Ȧ);Λ(ref f,"IconEnergy",Э,ȣ);Ϋ(ref f,ʌ.Count.ToString(),Э,І*Â,ȣ,TextAlignment.RIGHT);δ(out Τ,out Σ);Ѝ(
ref f,Ь,Τ);Ϋ(ref f,Τ,Ы,Ч*Â,ȣ,TextAlignment.CENTER);Ϋ(ref f,Σ,Ъ,Ч*Â,ȣ,TextAlignment.CENTER);RectangleF Щ=new RectangleF(new
Vector2(Ю.X,Ю.Y+Х),С),б=В(Щ,У),Е=new RectangleF(new Vector2(Щ.X+Х,Щ.Y),П),Ў=new RectangleF(new Vector2(Е.X,Е.Y),О),Є=new
RectangleF(new Vector2(Е.X,Е.Center.Y),О);έ(ref f,Щ,Ȧ);έ(ref f,Е,Ȧ);Α(ref f,б,ȣ);if(ʹ==false){Λ(ref f,"Danger",б,ȣ);}string Ѓ=Ϫ(ʨ,
"IGCTAG");Ϋ(ref f," "+Ѓ,Ў,Ч*Â,ȣ,TextAlignment.LEFT);Ϋ(ref f," "+Ε(),Є,Ч*Â,ȣ,TextAlignment.LEFT);RectangleF Ђ=new RectangleF(new
Vector2(Щ.X,Щ.Y+Х),С),Ё=В(Ђ,У);έ(ref f,Ђ,Ȧ);ε(ref f,Ё,ȣ);if(ͷ==false){Λ(ref f,"Danger",Ё,ȣ);}RectangleF Ѐ=new RectangleF(new
Vector2(Ђ.X+Х,Ђ.Y),С),Ͽ=В(Ѐ,У),Ͼ=В(Ѐ,Т);έ(ref f,Ѐ,Ȧ);ϐ(ref f,Ͽ,ȣ,Ȧ);if(Ͷ==false){Λ(ref f,"Danger",Ͼ,ȣ);}RectangleF Ͻ=new
RectangleF(new Vector2(Ѐ.X+Х,Ѐ.Y),С),ϼ=В(Ͻ,У),ϻ=В(Ͻ,Т);έ(ref f,Ͻ,Ȧ);Λ(ref f,"Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds",ϼ
,ȣ);if(Ά==false){Λ(ref f,"Danger",ϻ,ȣ);}Ϋ(ref f,"%",ϼ,І*Â,ȣ,TextAlignment.RIGHT);RectangleF Ϻ=new RectangleF(new Vector2(
Ͻ.X+Х,Ͻ.Y),С),Ϲ=В(Ϻ,У);έ(ref f,Ϻ,Ȧ);Ϣ(ref f,Ϲ,ȣ,Ȧ);RectangleF ϸ=new RectangleF(new Vector2(Ϻ.X+Х,Ϻ.Y),С),Ϸ=В(ϸ,У);έ(ref f
,ϸ,Ȧ);λ(ref f,Ϸ,ȣ,Ȧ);string ϵ=Ϫ(ˢ,ˡ);Ϋ(ref f,ϵ,Ϸ,І*Â,ȣ,TextAlignment.RIGHT);RectangleF Ѕ=new RectangleF(new Vector2(ϸ.X+Х
,ϸ.Y),С),ϴ=В(Ѕ,У),Ї=В(Ѕ,Т);έ(ref f,Ѕ,Ȧ);ϋ(ref f,ϴ,ȣ);if(ͳ==false){Λ(ref f,"Danger",Ї,ȣ);}RectangleF Д=new RectangleF(new
Vector2(Ѕ.X+Х,Ѕ.Y),С),Г=В(Д,У);έ(ref f,Д,Ȧ);}RectangleF В(RectangleF Б,float А,int Џ=1){float w=Б.Width,v=Б.Height;if(Џ==1){w=w
*А;v=v*А;}else{if(w<v){w=w*А;v-=2f*w*(1f-А);}else{v=v*А;w-=2f*v*(1f-А);}}float Γ=Б.Center.X-w/2f,Β=Б.Center.Y-v/2f;return
new RectangleF(new Vector2(Γ,Β),new Vector2(w,v));}void Ѝ(ref MySpriteDrawFrame f,RectangleF e,string Ќ){string[]Ћ=Ќ.Split(
'%');float Њ=Convert.ToSingle(Ћ[0]);float Љ=e.Width*Њ/100;float Ј=e.Center.X-e.Width/2+Љ/2;if(Њ==0)return;Λ(ref f,
"SquareSimple",Ј,e.Center.Y,Љ,e.Height,Ȣ);}void ϩ(ref MySpriteDrawFrame f,RectangleF e){float Γ=e.Center.X,Β=e.Center.Y,w=e.Width,v=e.
Height;if(w>v)w=v;MySprite ɂ=new MySprite(){Type=SpriteType.TEXTURE,Data="Screen_LoadingBar",Position=new Vector2(Γ,Β),Size=
new Vector2(w,w),RotationOrScale=Convert.ToSingle(ɕ/360*2*Math.PI),Alignment=TextAlignment.CENTER,Color=ȣ,};f.Add(ɂ);ɂ=new
MySprite(){Type=SpriteType.TEXTURE,Data="Screen_LoadingBar",Position=new Vector2(Γ,Β),Size=new Vector2(w*0.6f,w*0.6f),
RotationOrScale=Convert.ToSingle(2*Math.PI-ɕ/360*2*Math.PI),Alignment=TextAlignment.CENTER,Color=ȣ,};f.Add(ɂ);ɂ=new MySprite(){Type=
SpriteType.TEXTURE,Data="Screen_LoadingBar",Position=new Vector2(Γ,Β),Size=new Vector2(w*0.33f,w*0.33f),RotationOrScale=Convert.
ToSingle(Math.PI+ɕ/360*2*Math.PI),Alignment=TextAlignment.CENTER,Color=ȣ,};f.Add(ɂ);}void Ψ(out string Τ,out string Σ){double Χ=
0,Ρ=0;foreach(var Ð in ʃ){Χ+=((double)Ð.GetInventory().CurrentVolume);Ρ+=((double)Ð.GetInventory().MaxVolume);}Τ=Math.
Round(Χ/Ρ*100,1).ToString()+"%";Σ=ί(Χ*1000,false)+" L / "+ί(Ρ*1000,false)+" L";}void Φ(List<IMyGasTank>Υ,out string Τ,out
string Σ){double Χ=0,Ρ=0;foreach(var Ω in Υ){Χ+=Ω.Capacity*Ω.FilledRatio;Ρ+=Ω.Capacity;}Τ=Math.Round(Χ/Ρ*100,1).ToString()+"%"
;Σ=ί(Χ,false)+" L / "+ί(Ρ,false)+" L";}void δ(out string Τ,out string Σ){float β=0,α=0;foreach(var ΰ in ʌ){if(ΰ.IsWorking
){β+=ΰ.CurrentOutput;α+=ΰ.MaxOutput;}}Τ=Math.Round(β/α*100,1).ToString()+"%";Σ=ί(β*1000000,true)+" W / "+ί(α*1000000,true
)+" W";}string ί(double ή,bool γ){double S=0;string Ĥ="";if(ή>=1000000000000000){S=Math.Round(ή/1000000000000000,1);Ĥ=S.
ToString()+"KT";}else if(ή>=1000000000000){S=Math.Round(ή/1000000000000,1);Ĥ=S.ToString()+"T";}else if(ή>=1000000000){S=Math.
Round(ή/1000000000,1);if(γ)Ĥ=S.ToString()+"G";else Ĥ=S.ToString()+"B";}else if(ή>=1000000){S=Math.Round(ή/1000000,1);Ĥ=S.
ToString()+"M";}else if(ή>=1000){S=Math.Round(ή/1000,1);Ĥ=S.ToString()+"K";}else{S=Math.Round(ή,1);Ĥ=S.ToString();}return Ĥ;}
void έ(ref MySpriteDrawFrame f,RectangleF ά,Color n){Λ(ref f,"SquareSimple",ά,n);}void Ϋ(ref MySpriteDrawFrame f,string Ϊ,
RectangleF Π,float Ζ,Color Ή,TextAlignment Δ=TextAlignment.LEFT){float Γ,Β=Π.Y;if(Δ==TextAlignment.LEFT)Γ=Π.X;else if(Δ==
TextAlignment.CENTER)Γ=Π.Center.X;else Γ=Π.Right;using(f.Clip((int)(Π.X-Π.Width*0.05f),(int)Π.Y,(int)(Π.Width*1.1f),(int)Π.Height)){
MySprite ɂ=new MySprite(){Type=SpriteType.TEXT,Data=Ϊ,Position=new Vector2(Γ,Β),RotationOrScale=Ζ,Color=Ή,Alignment=Δ,FontId=
"LoadingScreen"};f.Add(ɂ);}}void Α(ref MySpriteDrawFrame f,RectangleF e,Color d){float w=e.Width/8f,v=e.Height/3f;RectangleF ΐ=new
RectangleF(new Vector2(e.Right-w,e.Y),new Vector2(w,v*3f)),Ώ=new RectangleF(new Vector2(e.Right-w*3f,e.Y+v),new Vector2(w,v*2f)),Ύ
=new RectangleF(new Vector2(e.Right-w*5f,e.Y+v*2f),new Vector2(w,v)),Ό=new RectangleF(new Vector2(e.Right-w*7f,e.Y+v*0.5f
),new Vector2(w,v*3f-v*0.5f)),Ί=new RectangleF(new Vector2(e.X,e.Y),new Vector2(w*3f,v*1.5f));έ(ref f,ΐ,d);έ(ref f,Ώ,d);έ
(ref f,Ύ,d);έ(ref f,Ό,d);Λ(ref f,"Triangle",Ί,d,180f);}string Ε(){float Η=0;int ğ=1;foreach(var Ο in ɥ){if(Ο.Enabled&&Ο.
EnableBroadcasting){if(ğ==1){Η=Ο.Radius;ğ++;}else if(Η<Ο.Radius)Η=Ο.Radius;}}return ί(Η,false)+"m";}void Λ(ref MySpriteDrawFrame f,string
Κ,float ĉ,float Ĉ,float Ξ,float Ν,Color Θ,float Μ=0){var ɂ=new MySprite{Type=SpriteType.TEXTURE,Data=Κ,Position=new
Vector2(ĉ,Ĉ),RotationOrScale=Convert.ToSingle(Μ/360f*2f*Math.PI),Size=new Vector2(Ξ,Ν),Color=Θ,Alignment=TextAlignment.CENTER};
f.Add(ɂ);}void Λ(ref MySpriteDrawFrame f,string Κ,RectangleF Ι,Color Θ,float Μ=0){var ɂ=new MySprite{Type=SpriteType.
TEXTURE,Data=Κ,Position=new Vector2(Ι.Center.X,Ι.Center.Y),RotationOrScale=Convert.ToSingle(Μ/360f*2f*Math.PI),Size=new Vector2
(Ι.Width,Ι.Height),Color=Θ,Alignment=TextAlignment.CENTER};f.Add(ɂ);}void ε(ref MySpriteDrawFrame f,RectangleF e,Color d)
{float Ϛ=e.Height/3f,ϙ=e.Width-Ϛ,Ĺ=0.7f;RectangleF Ϙ=new RectangleF(new Vector2(e.X,e.Y),new Vector2(Ϛ,Ϛ)),ϗ=new
RectangleF(new Vector2(Ϙ.X,Ϙ.Y+Ϛ),new Vector2(Ϛ,Ϛ)),ϖ=new RectangleF(new Vector2(ϗ.X,ϗ.Y+Ϛ),new Vector2(Ϛ,Ϛ)),ϕ=new RectangleF(new
Vector2(e.X+Ϛ,e.Y),new Vector2(ϙ,Ϛ)),ϔ=new RectangleF(new Vector2(ϕ.X,ϕ.Y+Ϛ),new Vector2(ϙ,Ϛ)),ϓ=new RectangleF(new Vector2(ϔ.X
,ϔ.Y+Ϛ),new Vector2(ϙ,Ϛ));List<RectangleF>ϒ=new List<RectangleF>(){Ϙ,ϗ,ϖ,ϕ,ϔ,ϓ};for(int ê=0;ê<=2;ê++){RectangleF ϑ=ϒ[ê];ϑ
=В(ϑ,Ĺ);έ(ref f,ϑ,d);}for(int ê=3;ê<=5;ê++){RectangleF ϑ=ϒ[ê];ϑ=В(ϑ,Ĺ,2);έ(ref f,ϑ,d);}}void ϐ(ref MySpriteDrawFrame f,
RectangleF e,Color Ϩ,Color ϧ){RectangleF Ϧ=new RectangleF(new Vector2(e.X,e.Center.Y),new Vector2(e.Width,e.Height/2f)),ϥ=В(e,0.7f
),Ϥ=new RectangleF(new Vector2(e.Center.X-e.Width*0.125f,e.Y),new Vector2(e.Width*0.25f,e.Height*0.25f)),ϣ=В(e,0.6f);έ(
ref f,Ϧ,Ϩ);έ(ref f,ϥ,ϧ);έ(ref f,Ϥ,Ϩ);Λ(ref f,"Triangle",ϣ,Ϩ,180f);}void Ϣ(ref MySpriteDrawFrame f,RectangleF e,Color Ϡ,
Color n){float Ĺ=0.85f,ϟ=e.Width/3f,Ϟ=ϟ*Ĺ,ϝ=e.Height,Ϝ=e.Height*Ĺ;Vector2 ϛ=new Vector2(ϟ,ϝ),ϡ=new Vector2(Ϟ,Ϝ);RectangleF Ϗ=
new RectangleF(new Vector2(e.X+ϟ*(1-Ĺ)*2f,e.Y),ϛ),ϊ=new RectangleF(new Vector2(Ϗ.X+ϟ-ϟ*(1-Ĺ),e.Y),ϛ),ο=new RectangleF(new
Vector2(ϊ.X+ϟ-ϟ*(1-Ĺ),e.Y),ϛ),ξ=В(Ϗ,Ĺ),ν=В(ϊ,Ĺ),μ=В(ο,Ĺ);Λ(ref f,"Triangle",Ϗ.Center.X,Ϗ.Center.Y,Ϗ.Height*Ĺ,Ϗ.Width,Ϡ,90f);Λ(
ref f,"Triangle",ϊ.Center.X,ϊ.Center.Y,ϊ.Height*Ĺ,ϊ.Width,Ϡ,90f);Λ(ref f,"Triangle",ο.Center.X,ο.Center.Y,ο.Height*Ĺ,ο.
Width,Ϡ,90f);string Ȼ=Ϫ(ʨ,ˣ);if(Ȼ=="FFF"){return;}else if(Ȼ=="FF"){Λ(ref f,"Triangle",μ.Center.X,μ.Center.Y,μ.Height*Ĺ,μ.
Width,n,90f);}else if(Ȼ=="F"){Λ(ref f,"Triangle",μ.Center.X,μ.Center.Y,μ.Height*Ĺ,μ.Width,n,90f);Λ(ref f,"Triangle",ν.Center.
X,ν.Center.Y,ν.Height*Ĺ,ν.Width,n,90f);}else{Λ(ref f,"Triangle",μ.Center.X,μ.Center.Y,μ.Height*Ĺ,μ.Width,n,90f);Λ(ref f,
"Triangle",ν.Center.X,ν.Center.Y,ν.Height*Ĺ,ν.Width,n,90f);Λ(ref f,"Triangle",ξ.Center.X,ξ.Center.Y,ξ.Height*Ĺ,ξ.Width,n,90f);}}
void λ(ref MySpriteDrawFrame f,RectangleF e,Color d,Color n){float κ=e.Width/4f,Ĺ=0.8f;RectangleF ι=new RectangleF(new
Vector2(e.X+e.Width*0.1f,e.Y),new Vector2(κ,κ)),θ=В(ι,Ĺ),η=new RectangleF(new Vector2(ι.X,ι.Y+κ),new Vector2(κ,κ)),π=В(η,Ĺ),ζ=
new RectangleF(new Vector2(η.X,η.Y+κ),new Vector2(κ,κ)),ρ=В(ζ,Ĺ),ώ=new RectangleF(new Vector2(ζ.X,ζ.Y+κ),new Vector2(κ,κ)),
ύ=В(ώ,Ĺ),ό=new RectangleF(new Vector2(ι.X+κ*1.1f,θ.Y),new Vector2(κ*2f,ύ.Bottom-θ.Y)),Ί=new RectangleF(new Vector2(e.
Right-e.Width*0.4f,e.Y),new Vector2(e.Width*0.4f,e.Height*0.4f));έ(ref f,θ,ȣ);έ(ref f,π,ȣ);έ(ref f,ρ,ȣ);έ(ref f,ύ,ȣ);έ(ref f,
ό,ȣ);Λ(ref f,"RightTriangle",Ί,n,180f);}void ϋ(ref MySpriteDrawFrame f,RectangleF e,Color d){float κ=e.Width/3f,Ĺ=0.8f;
RectangleF ω=new RectangleF(new Vector2(e.Center.X-κ/2f,e.Center.Y-κ/2f),new Vector2(κ,κ)),ψ=В(ω,Ĺ),χ=new RectangleF(new Vector2(ω
.X,ω.Y-κ),new Vector2(κ,κ)),φ=В(χ,Ĺ),υ=new RectangleF(new Vector2(ω.X,ω.Y+κ),new Vector2(κ,κ)),τ=В(υ,Ĺ),σ=new RectangleF(
new Vector2(ω.X-κ,ω.Y),new Vector2(κ,κ)),ς=В(σ,Ĺ),Ȋ=new RectangleF(new Vector2(ω.X+κ,ω.Y),new Vector2(κ,κ)),ĩ=В(Ȋ,Ĺ);έ(ref
f,ψ,d);έ(ref f,φ,d);έ(ref f,τ,d);έ(ref f,ς,d);έ(ref f,ĩ,d);}void Ǉ(){const int Ĩ=7;if(ɚ.Count<1){ͻ=true;ɏ=1;return;}Echo(
$"{ɏ}/{Ĩ}");switch(ɏ){case 1:Echo("GetItems");ģ();break;case 2:Echo("AllItems");ĭ(Ɋ,ɽ);break;case 3:Echo("Ore");ĭ(Ƞ,ɼ);break;case
4:Echo("Ingot");ĭ(ȸ,ɻ);break;case 5:Echo("Component");ĭ(Ȟ,ɺ);break;case 6:Echo("AmmoMagazine");ĭ(ȝ,ɹ);break;case 7:Echo(
"DrawItemPanels");Ĭ();break;}if(ɏ>=Ĩ){ͻ=true;ɏ=1;return;}ɏ++;}void ħ(){Echo("Build_Translate_Dic");string Ħ=Ϫ(ˏ,ˍ);int ĥ=Convert.ToInt16
(Ħ);for(int ê=1;ê<=ĥ;ê++){Ħ=Ϫ(ˏ,ê.ToString());string[]Ĥ=Ħ.Split(':');ʔ.Add(Ĥ[0],Ĥ[1]);}}void ģ(){ɔ=ɓ;if(ɓ==null)ɔ=
DateTime.Now;ɓ=DateTime.Now;Dictionary<string,double>Ģ=new Dictionary<string,double>();ȩ=new Dictionary<string,MyIni>();foreach(
var ë in ɚ){var Û=new List<MyInventoryItem>();var ġ=new List<MyInventoryItem>();ë.GetInventory().GetItems(Û);if(ë.
InventoryCount>=2){ë.GetInventory(1).GetItems(ġ);Û.AddRange(ġ);}foreach(var Ú in Û){string Ô=Ú.Type.ToString();double Ġ=Convert.
ToDouble(Ú.Amount.RawValue);if(Ģ.ContainsKey(Ô))Ģ[Ô]+=Ġ;else Ģ.Add(Ô,Ġ);}}Ɋ=new ʯ[Ģ.Count];int ğ=0;foreach(var Ø in Ģ.Keys){Ɋ[ğ]
.Ê=Ø;if(Ø.Contains("MyObjectBuilder_Ore"))Ɋ[ğ].ʮ=ʡ;else if(Ø.Contains("MyObjectBuilder_Ingot"))Ɋ[ğ].ʮ=ʠ;else if(Ø.
Contains("MyObjectBuilder_AmmoMagazine"))Ɋ[ğ].ʮ=ʞ;else Ɋ[ğ].ʮ=ʟ;Ɋ[ğ].ʬ=Ģ[Ø];Ɋ[ğ].ʫ=ɔ;Ɋ[ğ].ʪ=ɓ;if(Ȳ.ContainsKey(Ø))Ɋ[ğ].ʭ=Ȳ[Ø];
else Ɋ[ğ].ʭ=0;foreach(var Ú in Ș){if(Ú.ț==Ø)Ɋ[ğ].ș=Ú.ș*1000000;}ğ++;}Ȳ.Clear();Ȳ=Ģ;Ƞ=new ʯ[ı("MyObjectBuilder_Ore")];ȸ=new ʯ
[ı("MyObjectBuilder_Ingot")];ȝ=new ʯ[ı("MyObjectBuilder_AmmoMagazine")];į(Ƞ,"MyObjectBuilder_Ore");į(ȸ,
"MyObjectBuilder_Ingot");į(ȝ,"MyObjectBuilder_AmmoMagazine");Ȟ=new ʯ[Ɋ.Length-Ƞ.Length-ȸ.Length-ȝ.Length];ğ=0;foreach(var Ú in Ɋ){if(Ú.Ê.
IndexOf("MyObjectBuilder_Ore")==-1&&Ú.Ê.IndexOf("MyObjectBuilder_Ingot")==-1&&Ú.Ê.IndexOf("MyObjectBuilder_AmmoMagazine")==-1){
Ȟ[ğ]=Ú;ğ++;}}}int ı(string Į){Dictionary<string,double>İ=new Dictionary<string,double>();foreach(var Ú in Ɋ){if(Ú.Ê.
IndexOf(Į)!=-1){İ.Add(Ú.Ê,Ú.ʭ);}}return İ.Count;}void į(ʯ[]Č,string Į){int ğ=0;foreach(var Ú in Ɋ){if(Ú.Ê.IndexOf(Į)!=-1){Č[ğ]=
Ú;ğ++;}}}void ĭ(ʯ[]Č,List<IMyTextPanel>ī){if(ī.Count==0)return;int[]Ī=new int[ī.Count];int ğ=0;foreach(var s in ī){string
[]u=s.CustomName.Split(':');Ī[ğ]=Convert.ToInt16(u[1]);ğ++;}if(Č.Length>ą(Ī)*ɡ){foreach(var s in ī){if(ȩ.ContainsKey(s.
CustomName))continue;string[]u=s.CustomName.Split(':');if(Convert.ToInt16(u[1])<ą(Ī)){č(s,u[1],true,Č);}else{č(s,u[1],false,Č);}}}
else{foreach(var s in ī){if(ȩ.ContainsKey(s.CustomName))continue;string[]u=s.CustomName.Split(':');č(s,u[1],true,Č);}}}int ą
(int[]u){int Ď=0;for(int ê=0;ê<u.Length;ê++){if(ê==0)Ď=u[ê];else if(u[ê]>Ď)Ď=u[ê];}return Ď;}void č(IMyTextPanel s,string
r,bool q,ʯ[]Č){MyIni ċ=new MyIni();for(int ê=0;ê<ɡ;ê++){int Ċ=(Convert.ToInt16(r)-1)*ɡ+ê;int ĉ=(ê+1)%7;if(ĉ==0)ĉ=7;int Ĉ=
Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(ê+1)/7)));if(Ċ>Č.Length-1){ċ.Set(ʥ,ʤ,ê.ToString());break;}else{
if(ĉ==7&&Ĉ==4){if(q){Ć(ref ċ,ê+1,Č[Ċ]);}else{int ć=Č.Length-ɡ*Convert.ToInt16(r)+1;đ(ref ċ,ê+1,ć*1000000);}}else{Ć(ref ċ,ê
+1,Č[Ċ]);}ċ.Set(ʥ,ʤ,(ê+1).ToString());}}ȩ.Add(s.CustomName,ċ);}void Ć(ref MyIni ď,int ú,ʯ ĝ){string Ğ=ĝ.Ê;double Ĝ=ĝ.ʭ;
double ě=ĝ.ʬ;DateTime Ě=ĝ.ʫ;DateTime ę=ĝ.ʪ;double Ę=ĝ.ș;double ė=Ĝ-ě;TimeSpan Ė=ę-Ě;long ĕ=0;if(Ė.Ticks>0&&ė>0){double Ĕ=ė/Ė.
Ticks;if(Int64.MaxValue>ě/Ĕ){ĕ=Convert.ToInt64(ě/Ĕ);}}TimeSpan ē=new TimeSpan(ĕ);string Ē="";if(ē.Days==0&&ĕ!=0){Ē=ē.Hours.
ToString()+":"+ē.Minutes.ToString()+":"+ē.Seconds.ToString();}else if(ē.Days>0&&ĕ!=0){Ē=ē.Days.ToString()+"d"+ē.Hours.ToString()
+":"+ē.Minutes.ToString();}ď.Set(ú.ToString(),ʣ,Ğ);ď.Set(ú.ToString(),ʢ,ĝ.ʮ);ď.Set(ú.ToString(),ʝ,Ĝ.ToString());ď.Set(ú.
ToString(),ʜ,ě.ToString());ď.Set(ú.ToString(),ʛ,Ē);ď.Set(ú.ToString(),ʚ,Ě.ToString());ď.Set(ú.ToString(),ʙ,ę.ToString());ď.Set(ú
.ToString(),ʘ,Ę.ToString());}void đ(ref MyIni ď,int ú,int Đ){ď.Set(ú.ToString(),ʣ,"AH_BoreSight");ď.Set(ú.ToString(),ʜ,Đ.
ToString());ď.Set(ú.ToString(),ʛ,"");ď.Set(ú.ToString(),ʘ,"0");}void Ĭ(){foreach(var s in ɽ)ń(s,Ȧ);foreach(var s in ɼ)ń(s,ȥ);
foreach(var s in ɻ)ń(s,Ȥ);foreach(var s in ɺ)ń(s,Ȧ);foreach(var s in ɹ)ń(s,ȋ);}void ń(IMyTextPanel s,Color Ń){if(!s.Enabled)
return;s.ContentType=ContentType.SCRIPT;s.ScriptBackgroundColor=Ȧ;s.WriteText("",false);MySpriteDrawFrame f=s.DrawFrame();int
ł=Convert.ToInt16(ȩ[s.CustomName].Get(ʥ,ʤ).ToString());if(ȧ.ContainsKey(s.EntityId)){if(ȧ[s.EntityId]==0f)ȧ[s.EntityId]=
1f;else ȧ[s.EntityId]=0f;}else ȧ.Add(s.EntityId,0f);float ª=ȧ[s.EntityId];RectangleF z=new RectangleF((s.TextureSize-s.
SurfaceSize)/2f+new Vector2(0,ª),s.SurfaceSize);float ĺ;if(z.Width<=z.Height)ĺ=z.Width;else ĺ=z.Height;float Ĺ=ĺ/512f;RectangleF e=
new RectangleF(new Vector2(z.Center.X-ĺ/2,z.Center.Y-ĺ/2),new Vector2(ĺ,ĺ));έ(ref f,e,Ń);for(int Ċ=0;Ċ<ɡ;Ċ++){int ĉ=(Ċ+1)%ɢ
;if(ĉ==0)ĉ=ɢ;int Ĉ=Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(Ċ+1)/ɢ)));RectangleF Ņ=new RectangleF(
e.Position+new Vector2(e.Width/ɢ*Convert.ToSingle(ĉ-1),e.Height/ɣ*Convert.ToSingle(Ĉ-1)),new Vector2(e.Width/ɢ,e.Height/ɣ
));if(Ċ>ł-1)break;ŏ(s,ref f,Ċ+1,Ņ,Ĺ);if(Ͱ){MyIni Ł=ȩ[s.CustomName];string Ô=Ł.Get((Ċ+1).ToString(),ʣ).ToString().Replace(
"{","").Replace("}","").Replace(":","_");s.WriteText(Ô,true);s.WriteText("\n",true);}}f.Dispose();}void ŏ(IMyTextPanel s,
ref MySpriteDrawFrame f,int ú,RectangleF e,float Ĺ){MyIni Ł=ȩ[s.CustomName];string Ô=Ł.Get(ú.ToString(),ʣ).ToString();
double Ŏ=Convert.ToDouble(Ł.Get(ú.ToString(),ʜ).ToString());string Ē=Ł.Get(ú.ToString(),ʛ).ToString();double Ō=Convert.
ToDouble(Ł.Get(ú.ToString(),ʘ).ToString());Color µ=new Color();RectangleF ŋ=В(e,0.96f,2);switch(Ł.Get(ú.ToString(),ʢ).ToString()
){case ʡ:µ=ȏ;break;case ʠ:µ=Ȏ;break;case ʟ:µ=Ȣ;break;case ʞ:µ=ȍ;break;}έ(ref f,ŋ,µ);RectangleF Ŋ=В(ŋ,0.95f,2);Ϋ(ref f,ķ(Ô
.Replace("{","").Replace("}","").Replace(":","_")),Ŋ,0.53f*Ĺ,ȣ);RectangleF ŉ=new RectangleF(ŋ.Position+new Vector2(0,ŋ.
Height*0.12f),new Vector2(ŋ.Width,ŋ.Width));Λ(ref f,Ô,ŉ,ȣ);RectangleF ň=new RectangleF(ŉ.Position+new Vector2(0,ŉ.Height*0.96f
),new Vector2(ŋ.Width,ŋ.Height*0.2f)),Ň=В(ň,0.93f,2);Ϋ(ref f,ί(Ŏ/1000000,false),Ň,0.8f*Ĺ,ȣ,TextAlignment.RIGHT);
RectangleF ņ=new RectangleF(new Vector2(ŉ.X,ŉ.Bottom-ŋ.Height*0.1f),new Vector2(ŋ.Width,ŋ.Height*0.12f)),ō=В(ņ,0.96f,2);if(Ō!=0){Ϋ
(ref f,ί(Ō/1000000,false),ō,0.5f*Ĺ,ȣ,TextAlignment.RIGHT);}RectangleF ŀ=new RectangleF(new Vector2(ŋ.X,ŋ.Bottom-ŋ.Height*
0.13f),new Vector2(ŋ.Width,ŋ.Height*0.14f));Ϋ(ref f,Ē,ŀ,0.57f*Ĺ,ȣ,TextAlignment.RIGHT);}string Ļ(string C){string[]S=C.Split(
'/');if(S.Length==2){return S[1];}else{return C;}}string ķ(string C){if(ʔ.ContainsKey(C)){return ʔ[C];}else{return Ļ(C);}}
void Ķ(){if((ʉ.Count<1&&ʊ.Count<1)||(ɸ.Count<1&&ɷ.Count<1)){ͻ=true;return;}Echo($"{Ɏ}/{ɸ.Count+ɷ.Count+2}");if(Ɏ==1){Echo(
"GetFacilities");ĵ();ɩ=ĸ(Ȑ,ɸ);ɪ=ĸ(ȡ,ɷ);Ɏ++;return;}else if(Ɏ==2){Echo("MaxPanelNumber");ɩ=ĸ(Ȑ,ɸ);ɪ=ĸ(ȡ,ɷ);Ɏ++;return;}else if(Ɏ>ɸ.Count
+2){Echo("Ass");ɍ=Ɏ-ɸ.Count-3;if(ʊ.Count>0)Ľ(ȡ,ɷ,ɪ,Ȏ,Ȥ);}else if(Ɏ>2){Echo("Ref");ɍ=Ɏ-3;if(ʉ.Count>0)Ľ(Ȑ,ɸ,ɩ,ȏ,ȥ);}if(Ɏ>=
ɸ.Count+ɷ.Count+2){ͻ=true;Ɏ=1;return;}Ɏ++;}void ĵ(){Ȑ=new ȗ[ʉ.Count];int ğ=0;foreach(var ã in ʉ){Ȑ[ğ].Ê=ã.CustomName;Ȑ[ğ]
.Ȗ=ã.Enabled;Ȑ[ğ].ȕ=ã.IsProducing;if(Ϫ(ã,ˢ,ˡ)=="1")Ȑ[ğ].Ȕ=true;else Ȑ[ğ].Ȕ=false;Ȑ[ğ].ȓ=false;List<MyInventoryItem>Û=new
List<MyInventoryItem>();ã.InputInventory.GetItems(Û);if(Û.Count==0){Ȑ[ğ].Ȓ="Empty";Ȑ[ğ].ȑ=0;}else{Ȑ[ğ].Ȓ=Û[0].Type.ToString(
);Ȑ[ğ].ȑ=(double)Û[0].Amount;}char[]ĳ={':','：'};string[]Ĵ=ã.DetailedInfo.Split('%');string[]Ĳ=Ĵ[0].Split(ĳ);Ȑ[ğ].ȟ=Ĳ[Ĳ.
Length-1];ğ++;}ȡ=new ȗ[ʊ.Count];ğ=0;foreach(var W in ʊ){ȡ[ğ].Ê=W.CustomName;ȡ[ğ].Ȗ=W.Enabled;ȡ[ğ].ȕ=W.IsProducing;ȡ[ğ].Ȕ=W.
CooperativeMode;ȡ[ğ].ȓ=W.Repeating;List<MyProductionItem>Û=new List<MyProductionItem>();W.GetQueue(Û);if(Û.Count==0){ȡ[ğ].Ȓ="Empty";ȡ[ğ
].ȑ=0;}else{ȡ[ğ].Ȓ=Û[0].BlueprintId.ToString();ȡ[ğ].ȑ=(double)Û[0].Amount;}char[]ĳ={':','：'};string[]Ĵ=W.DetailedInfo.
Split('%');string[]Ĳ=Ĵ[0].Split(ĳ);ȡ[ğ].ȟ=Ĳ[Ĳ.Length-1];ğ++;}}int ĸ(ȗ[]o,List<IMyTextPanel>Ŀ){if(o.Length==0||Ŀ.Count==0)
return 0;int[]ľ=new int[Ŀ.Count];int ğ=0;foreach(var s in Ŀ){string[]u=s.CustomName.Split(':');ľ[ğ]=Convert.ToInt16(u[1]);ğ++;
}return ą(ľ);}void Ľ(ȗ[]o,List<IMyTextPanel>Ŀ,int ļ,Color µ,Color n){if(o.Length==0||Ŀ.Count==0)return;Echo(
$"{ɍ+1}/{Ŀ.Count}");if(o.Length>ļ*ɤ){var s=Ŀ[ɍ];Echo(s.CustomName);if(ȧ.ContainsKey(s.EntityId)){if(ȧ[s.EntityId]==0f)ȧ[s.EntityId]=1f;
else ȧ[s.EntityId]=0f;}else ȧ.Add(s.EntityId,0f);float ª=ȧ[s.EntityId];if(s.ContentType!=ContentType.SCRIPT)s.ContentType=
ContentType.SCRIPT;s.ScriptBackgroundColor=Ȧ;RectangleF z=new RectangleF((s.TextureSize-s.SurfaceSize)/2f+new Vector2(0,ª),s.
SurfaceSize);float ĺ;if(z.Width<=z.Height)ĺ=z.Width;else ĺ=z.Height;float Ĺ=ĺ/512f;RectangleF e=new RectangleF(new Vector2(z.Center
.X-ĺ/2,z.Center.Y-ĺ/2),new Vector2(ĺ,ĺ));MySpriteDrawFrame f=s.DrawFrame();έ(ref f,e,n);string[]u=s.CustomName.Split(':')
;if(Convert.ToInt16(u[1])<ļ){t(s,ref f,e,u[1],true,o,n,µ);}else{t(s,ref f,e,u[1],false,o,n,µ);}f.Dispose();}else{var s=Ŀ[
ɍ];Echo(s.CustomName);if(s.CustomData!="0")s.CustomData="0";else s.CustomData="1";float ª=Convert.ToSingle(s.CustomData);
if(s.ContentType!=ContentType.SCRIPT)s.ContentType=ContentType.SCRIPT;s.ScriptBackgroundColor=Ȧ;RectangleF z=new
RectangleF((s.TextureSize-s.SurfaceSize)/2f+new Vector2(0,ª),s.SurfaceSize);float w=z.Width,v=z.Height;if(z.Width<=z.Height)v=z.
Width;RectangleF e=new RectangleF(new Vector2(z.Center.X-w/2,z.Center.Y-v/2),new Vector2(w,v));MySpriteDrawFrame f=s.
DrawFrame();έ(ref f,e,n);string[]u=s.CustomName.Split(':');t(s,ref f,e,u[1],true,o,n,µ);f.Dispose();}}void t(IMyTextPanel s,ref
MySpriteDrawFrame f,RectangleF e,string r,bool q,ȗ[]o,Color n,Color µ){Echo("DrawFullFacilityScreen");if(!s.Enabled)return;s.WriteText(""
,false);float m=e.Height/ɜ;for(int À=0;À<ɤ;À++){int Ï=(Convert.ToInt16(r)-1)*ɤ+À;if(Ï>o.Length-1)return;RectangleF Í=new
RectangleF(e.Position+new Vector2(0,m*À),new Vector2(e.Width,m));if(À==ɤ-1){if(q){Ë(s,ref f,Í,(Ï+1).ToString()+". "+o[Ï].Ê+" ×"+o[
Ï].ȟ+"%",o[Ï].ȕ,ί(o[Ï].ȑ,false),o[Ï].Ȓ,o[Ï].ȓ,o[Ï].Ȕ,o[Ï].Ȗ,À,µ);}else{double Ì=o.Length-ɤ*Convert.ToInt16(r)+1;Ë(s,ref f
,Í,"+ "+Ì.ToString()+" Facilities",false,"0","Empty",false,false,false,À,µ);}}else{Ë(s,ref f,Í,(Ï+1).ToString()+". "+o[Ï]
.Ê+" ×"+o[Ï].ȟ+"%",o[Ï].ȕ,ί(o[Ï].ȑ,false),o[Ï].Ȓ,o[Ï].ȓ,o[Ï].Ȕ,o[Ï].Ȗ,À,µ);}if(Ͱ){s.WriteText(
$"{(Ï+1).ToString()+".\n"}{o[Ï].Ê}",true);s.WriteText($"\n{o[Ï].Ȓ}",true);s.WriteText("\n\n",true);}}}void Ë(IMyTextPanel s,ref MySpriteDrawFrame f,
RectangleF e,string Ê,bool Î,string É,string Ç,bool Æ,bool Å,bool Ä,int Ã,Color µ){float Â=e.Height*ɜ/512f,Á=0.92f;float l=e.
Height;float R=0.75f;RectangleF A=new RectangleF(e.Position,new Vector2(l*4f,l)),P=В(A,Á,2),O=В(P,0.93f,2),N=new RectangleF(P.
Position,new Vector2(l*0.75f,l)),M=В(N,Á);έ(ref f,P,µ);if(Æ)g(ref f,M,ȣ);Ϋ(ref f,É,O,R*Â,ȣ,TextAlignment.RIGHT);RectangleF L=new
RectangleF(A.Position+new Vector2(A.Width,0),new Vector2(l,l)),K=В(L,Á);έ(ref f,K,µ);if(Ç!="Empty")Λ(ref f,D(Ç),K,ȣ);RectangleF J=
new RectangleF(L.Position+new Vector2(L.Width,0),new Vector2(l,l)),I=В(J,Á),H=В(I,0.7f);if(Ä){if(Î)έ(ref f,I,new Color(0,
140,0));else έ(ref f,I,new Color(130,100,0));}else{έ(ref f,I,new Color(178,9,9));}if(Å)j(ref f,H,Color.White);RectangleF G=
new RectangleF(J.Position+new Vector2(J.Width,0),new Vector2(e.Width-J.Width-L.Width-A.Width,l)),F=В(G,Á,2),E=В(F,0.93f,2);
έ(ref f,F,µ);Ϋ(ref f,Ê,E,R*Â,ȣ);}string D(string C){string[]B=C.Split('/');string Q=B[B.Length-1];string S=
"Textures\\FactionLogo\\Empty.dds";if(Ȩ.ContainsKey(Q)){S=Ȩ[Q];}return S;}void j(ref MySpriteDrawFrame f,RectangleF e,Color d){Vector2 h=new Vector2(e.
Width*0.5f,e.Height);Λ(ref f,"Triangle",e.Center.X-h.X*0.6f,e.Center.Y,h.Y,h.X,d,270f);Λ(ref f,"Triangle",e.Center.X-h.X*0.1f
,e.Center.Y,h.Y,h.X,d,270f);Λ(ref f,"Triangle",e.Center.X+h.X*0.4f,e.Center.Y,h.Y,h.X,d,270f);}void g(ref
MySpriteDrawFrame f,RectangleF e,Color d){RectangleF c=new RectangleF(new Vector2(e.X,e.Y+e.Height*0.25f),new Vector2(e.Width,e.Height*
0.5f)),a=new RectangleF(new Vector2(e.Right-e.Width*0.5f,e.Y),new Vector2(e.Width*0.5f,e.Height*0.5f)),Z=new RectangleF(new
Vector2(e.X,e.Bottom-e.Height*0.5f),new Vector2(e.Width*0.5f,e.Height*0.5f));Λ(ref f,"SquareHollow",c,d);Λ(ref f,"AH_BoreSight"
,a,d);Λ(ref f,"AH_BoreSight",Z,d,180f);}void Y(){if(ʊ.Count<1||ʃ.Count<1){ͻ=true;return;}Echo($"{Ɍ}/{ʊ.Count}");X(ʊ[Ɍ-1])
;if(Ɍ>=ʊ.Count){ͻ=true;Ɍ=1;return;}Ɍ++;}void X(IMyAssembler W){if(W.Mode==MyAssemblerMode.Assembly){V(W);}}void V(
IMyProductionBlock U){if(!U.IsProducing){È(U.InputInventory);}}void È(IMyInventory T){foreach(var Ð in ʃ){List<MyInventoryItem>Û=new List<
MyInventoryItem>();T.GetItems(Û);if(Û.Count<1)return;foreach(var Ú in Û){bool é=T.TransferItemTo(Ð.GetInventory(),Ú);}}}void õ(){if(ʈ.
Count<1||ʃ.Count<1){ͻ=true;return;}Echo($"{ɋ}/{ʈ.Count}");È(ʈ[ɋ-1].GetInventory());if(ɋ>=ʈ.Count){ͻ=true;ɋ=1;return;}ɋ++;}
void ô(){if(ʆ.Count<1||ʃ.Count<1){ͻ=true;return;}Echo($"{ɝ}/{ʆ.Count}");È(ʆ[ɝ-1].GetInventory());if(ɝ>=ʆ.Count){ͻ=true;ɝ=1;
return;}ɝ++;}void ó(){if(ʅ.Count<1||ʃ.Count<1){ͻ=true;return;}Echo($"{ɯ}/{ʅ.Count}");È(ʅ[ɯ-1].GetInventory());if(ɯ>=ʅ.Count){ͻ
=true;ɯ=1;return;}ɯ++;}void ò(List<IMyGasTank>ñ,ref int ð,string ï){if(ñ.Count<1||ʄ.Count<1){ͻ=true;ð=1;return;}Echo(
$"{ð}/{ñ.Count}");î(ï,ñ[ð-1]);if(ð>=ñ.Count){ͻ=true;ð=1;return;}ð++;}void î(string í,IMyGasTank ì){if(!ì.AutoRefillBottles)ì.
AutoRefillBottles=true;foreach(var ë in ʄ){List<MyInventoryItem>Û=new List<MyInventoryItem>();ë.GetInventory().GetItems(Û,ê=>ê.Type.
ToString().Contains(í));foreach(var Ú in Û){bool é=ë.GetInventory().TransferItemTo(ì.GetInventory(),Ú);}}}void ö(){if(!ʹ)return;
List<IMyShipConnector>Ą=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(Ą,ë=>ë.IsConnected==false&&Ϫ(ë,
"Connector_Tag","ForAutoParking")!="No"&&ë.IsSameConstructAs(Me));if(Ą.Count==0)return;StringBuilder ă=new StringBuilder();ă.Clear();ă.
Append(Ą.Count.ToString());ă.Append("=");string Ă=Ϫ(ʨ,"IGCTAG");foreach(var ā in Ą){ă.Append("~");ă.Append(ā.CustomName.
ToString());ă.Append(":");ă.Append(ā.GetPosition().X.ToString());ă.Append(":");ă.Append(ā.GetPosition().Y.ToString());ă.Append(
":");ă.Append(ā.GetPosition().Z.ToString());ă.Append(":");ă.Append(ā.WorldMatrix.Forward.X.ToString());ă.Append(":");ă.
Append(ā.WorldMatrix.Forward.Y.ToString());ă.Append(":");ă.Append(ā.WorldMatrix.Forward.Z.ToString());}Ă=Ϫ(ʨ,"IGCTAG");IGC.
SendBroadcastMessage(Ă,ă.ToString());}void Ā(){if(ʃ.Count<1){ͻ=true;return;}Echo($"{ɫ}/{ʃ.Count}");string ÿ=Ϫ(ʨ,ˬ);double þ=Convert.ToDouble
(ÿ)/1000;for(int ê=0;ê<=9;ê++){ɫ++;if(ɫ>ʃ.Count){ͻ=true;ɫ=0;return;}var Ð=ʃ[ɫ-1];string ý;double ü;if((double)Ð.
GetInventory().MaxVolume<þ){continue;}else{ý=Ϫ(Ð,ʨ,ˮ);if(ý==""){ý=Ð.CustomName;Ⱥ(Ð,ʨ,ˮ,ý);}ü=(double)Ð.GetInventory().CurrentVolume/
(double)Ð.GetInventory().MaxVolume;}ü=Math.Round(ü*100,1);Ð.CustomName=ý+"__"+ü.ToString()+"%";}}void û(){ͼ=true;if(ʉ.
Count<1)return;for(int ú=1;ú<=2;ú++){IMyRefinery Õ=ʉ[ɛ-1];Echo($"{ɛ}/{ʉ.Count}");Echo(Õ.CustomName);if(ɛ++>=ʉ.Count)ɛ=1;if(!Õ
.Enabled)continue;switch(Ϫ(ˢ,ˡ)){case"0":ù(Õ);break;case"1":ø(Õ);break;case"2":è(Õ);break;}}}void ù(IMyRefinery Õ){Echo(
"IndependentMode");Ⱥ(Õ,ˢ,ˡ,"0");ȭ=new Dictionary<string,double>();Ʌ(ȭ,Õ);if(ȭ.Count<1)return;Ü(Õ);Ù(Õ,ȭ);}void ø(IMyRefinery Õ){Echo(
"UnifiedMode");Ⱥ(Õ,ˢ,ˡ,"1");if(Ȯ.Count<1)return;Ü(Õ);Ù(Õ,Ȯ);}void è(IMyRefinery Õ){Echo("MixedMode");ȭ=new Dictionary<string,double>(
);Ʌ(ȭ,Õ);if(ȭ.Count>0){Ⱥ(Õ,ˢ,ˡ,"0");Ü(Õ);Ù(Õ,ȭ);}else{Ⱥ(Õ,ˢ,ˡ,"1");if(Ȯ.Count<1)return;Ü(Õ);Ù(Õ,Ȯ);}}void Ü(IMyRefinery Õ
){var Û=new List<MyInventoryItem>();ȫ=new Dictionary<string,double>();Õ.InputInventory.GetItems(Û);foreach(var Ú in Û){if
(ȫ.ContainsKey(Ú.Type.ToString()))ȫ[Ú.Type.ToString()]+=(double)Ú.Amount.RawValue;else ȫ.Add(Ú.Type.ToString(),(double)Ú.
Amount.RawValue);}}void Ù(IMyRefinery Õ,Dictionary<string,double>Ó){foreach(var Ø in ȫ.Keys)if(!Ó.ContainsKey(Ø))ç(Õ,Ø);
foreach(var Ø in Ó.Keys)Ö(Õ,Ø,Ó);}void Ö(IMyRefinery Õ,string Ô,Dictionary<string,double>Ó){foreach(var Ð in ʃ){MyFixedPoint Ò;
var Û=new List<MyInventoryItem>();double Ñ;Ü(Õ);if(ȫ.ContainsKey(Ô))Ñ=ȫ[Ô]-Ó[Ô];else Ñ=-Ó[Ô];Ò.RawValue=Convert.ToInt64(
Math.Abs(Ñ));if(Ñ>0){Õ.InputInventory.GetItems(Û);foreach(var Ú in Û){if(Ú.Type.ToString()==Ô){Õ.InputInventory.
TransferItemTo(Ð.GetInventory(),Ú,Ò);}}}else if(Ñ<0){Ð.GetInventory().GetItems(Û);foreach(var Ú in Û){if(Ú.Type.ToString()==Ô){Ð.
GetInventory().TransferItemTo(Õ.InputInventory,Ú,Ò);}}}else return;}}void ç(IMyRefinery Õ,string Ô){foreach(var Ð in ʃ){if(!ȫ.
ContainsKey(Ô))return;var Û=new List<MyInventoryItem>();Õ.InputInventory.GetItems(Û);foreach(var Ú in Û){if(Ú.Type.ToString()==Ô){Õ
.InputInventory.TransferItemTo(Ð.GetInventory(),Ú);}}}}void æ(){Echo("Build_Ore_List");ə=new List<string>();ə.Add(ˡ);
foreach(var C in ɿ){if(C.IndexOf("MyObjectBuilder_Ore")!=-1){ə.Add(C);}}}void å(){Echo("Build_Combined_Refining_UI_List");ɖ=new
List<ȳ>();ȳ ä=new ȳ();ä.ȱ=Me.CustomName;ä.Ȱ=Convert.ToInt16(Ϫ(ˢ,ˡ));Ʌ(ä.ȯ);ɖ.Add(ä);foreach(var ã in ʉ){ä=new ȳ();ä.ȱ=ã.
CustomName;ä.Ȱ=Convert.ToInt16(Ϫ(ã,ˢ,ˡ));Ʌ(ä.ȯ,ã);ɖ.Add(ä);}}void â(){int á=Convert.ToInt16(Ϫ(ˢ,ʻ)),à=Convert.ToInt16(Ϫ(ˢ,ˡ)),ß=
Convert.ToInt16(Ϫ(ˢ,ʦ)),Þ=0,º=0,Ý=0;string Ô=ə[ß-1];double ǚ=Convert.ToDouble(Ϫ(ˢ,Ͳ));if(ǚ<0)ǚ=0;foreach(var Ǔ in ɴ){if(Ǔ.
MoveIndicator.Y>0)Ý++;else if(Ǔ.MoveIndicator.Y<0)Ý--;else if(Ǔ.MoveIndicator.Z<0)Þ--;else if(Ǔ.MoveIndicator.Z>0)Þ++;else if(Ǔ.
MoveIndicator.X>0)º++;else if(Ǔ.MoveIndicator.X<0)º--;}if(Ý!=0){int Ǚ;if(ß==1){à+=Ý;if(à>2)à=0;else if(à<0)à=2;Ⱥ(ˢ,ˡ,à.ToString());}
else{if(á==1){Dictionary<string,double>ǘ=new Dictionary<string,double>();Ʌ(ǘ);if(ǘ.ContainsKey(Ô)){ǘ[Ô]+=Ý*ǚ*1000000;if(ǘ[Ô]
<=0)ǘ.Remove(Ô);}else if(ǘ.Count<10&&Ý>0){ǘ.Add(Ô,ǚ*1000000);}for(int ú=1;ú<=10;ú++)Ⱥ(ˢ,ú.ToString(),"");Ǚ=1;foreach(var Ø
in ǘ.Keys){Ⱥ(ˢ,Ǚ.ToString(),Ø+":"+(ǘ[Ø]/1000000).ToString());Ǚ++;}}else{Dictionary<string,double>Ǘ=new Dictionary<string,
double>();Ʌ(Ǘ,ʉ[á-2]);if(Ǘ.ContainsKey(Ô)){Ǘ[Ô]+=Ý*ǚ*1000000;if(Ǘ[Ô]<=0)Ǘ.Remove(Ô);}else if(Ǘ.Count<10&&Ý>0){Ǘ.Add(Ô,ǚ*
1000000);}for(int ú=1;ú<=10;ú++)Ⱥ(ʉ[á-2],ˢ,ú.ToString(),"");Ǚ=1;foreach(var Ø in Ǘ.Keys){Ⱥ(ʉ[á-2],ˢ,Ǚ.ToString(),Ø+":"+(Ǘ[Ø]/
1000000).ToString());Ǚ++;}}}}if(Þ!=0){á+=Þ;if(á>ʉ.Count+1)á=1;else if(á<1)á=ʉ.Count+1;Ⱥ(ˢ,ʻ,á.ToString());}if(º!=0){ß+=º;if(ß>ə
.Count)ß=1;else if(ß<1)ß=ə.Count;Ⱥ(ˢ,ʦ,ß.ToString());decimal ǖ=Math.Ceiling(Convert.ToDecimal(ß)/ɞ);Ⱥ(ˢ,ʰ,ǖ.ToString());}
if(Þ!=0||º!=0||Ý!=0){å();ǔ();ǝ();Ʌ(Ȯ);}}void Ǖ(){if(ʉ.Count<1){ͻ=true;return;}å();ǔ();ǝ();ͻ=true;}void ǔ(){foreach(var Ǔ
in ɴ){int ǒ=Ǔ.SurfaceCount;if(ǒ<1)continue;Ǔ.ControlThrusters=false;Ǔ.ControlWheels=false;Ǔ.SetValue("ControlGyros",false)
;int Ǒ=Convert.ToInt16(Ϫ(Ǔ,ʥ,ʧ));if(Ǒ>=ǒ)Ǒ=ǒ;else if(Ǒ<0)Ǒ=0;IMyTextSurface ǐ=Ǔ.GetSurface(Ǒ);if(ǐ.BackgroundColor!=Ȧ)ǐ.
BackgroundColor=Ȧ;if(ǐ.ContentType!=ContentType.SCRIPT)ǐ.ContentType=ContentType.SCRIPT;MySpriteDrawFrame f=ǐ.DrawFrame();if(ȧ.
ContainsKey(Ǔ.EntityId)){if(ȧ[Ǔ.EntityId]==0f)ȧ[Ǔ.EntityId]=1f;else ȧ[Ǔ.EntityId]=0f;}else ȧ.Add(Ǔ.EntityId,0f);float ª=ȧ[Ǔ.
EntityId];RectangleF z=new RectangleF((ǐ.TextureSize-ǐ.SurfaceSize)/2f+new Vector2(0,ª),ǐ.SurfaceSize);float ĺ=z.Height;if(z.
Width<=z.Height)ĺ=z.Width;RectangleF e=new RectangleF(new Vector2(z.Center.X-ĺ/2,z.Center.Y-ĺ/2),new Vector2(ĺ,ĺ));έ(ref f,z,
Ȧ);έ(ref f,e,ȥ);ǜ(ref f,e);f.Dispose();}}void ǝ(){foreach(var s in ɵ){if(!s.Enabled)continue;if(s.BackgroundColor!=Ȧ)s.
BackgroundColor=Ȧ;if(s.ContentType!=ContentType.SCRIPT)s.ContentType=ContentType.SCRIPT;MySpriteDrawFrame f=s.DrawFrame();if(ȧ.
ContainsKey(s.EntityId)){if(ȧ[s.EntityId]==0f)ȧ[s.EntityId]=1f;else ȧ[s.EntityId]=0f;}else ȧ.Add(s.EntityId,0f);float ª=ȧ[s.
EntityId];RectangleF z=new RectangleF((s.TextureSize-s.SurfaceSize)/2f+new Vector2(0,ª),s.SurfaceSize);float ĺ=z.Height;if(z.
Width<=z.Height)ĺ=z.Width;RectangleF e=new RectangleF(new Vector2(z.Center.X-ĺ/2,z.Center.Y-ĺ/2),new Vector2(ĺ,ĺ));έ(ref f,z,
Ȧ);έ(ref f,e,ȥ);ǜ(ref f,e);f.Dispose();}}void ǜ(ref MySpriteDrawFrame f,RectangleF e){float Â=e.Height/512f,Á=0.92f;float
l=e.Height;float R=0.75f;int á=Convert.ToInt16(Ϫ(ˢ,ʻ)),ß=Convert.ToInt16(Ϫ(ˢ,ʦ)),Ǜ=Convert.ToInt16(Ϫ(ˢ,ʰ)),Ǐ=Convert.
ToInt16(Math.Ceiling(Convert.ToDecimal(ə.Count)/ɞ));string Ǌ=á.ToString()+". "+ɖ[á-1].ȱ+" ("+á.ToString()+"/"+(ʉ.Count+1).
ToString()+")",Ǆ=ɖ[á-1].ȯ.Count.ToString()+"/10",ǃ="P "+Ǜ.ToString()+"/"+Ǐ.ToString();RectangleF ǂ=new RectangleF(e.Position,new
Vector2(e.Width,e.Height/20f)),ǁ=В(ǂ,Á,2),ǀ=В(ǁ,Á,2);έ(ref f,ǁ,ȏ);Ϋ(ref f,Ǌ,ǀ,R*Â,ȣ);RectangleF ƿ=new RectangleF(e.Position+new
Vector2(ǂ.Width-ǂ.Height,0),new Vector2(ǂ.Height,ǂ.Height)),ƾ=В(ƿ,Á,2),ƽ=В(ƾ,Á,2);if(ɖ[á-1].Ȱ==1&&ɖ[á-1].ȱ!=Me.CustomName)j(ref
f,ƽ,ȣ);for(int Ƽ=0;Ƽ<ɞ;Ƽ++){int ƻ=Ƽ+(Ǜ-1)*ɞ;if(ƻ<=ə.Count-1){int ĉ=(Ƽ+1)%ɟ;if(ĉ==0)ĉ=ɟ;int Ĉ=Convert.ToInt16(Math.Ceiling
(Convert.ToDecimal(Convert.ToDouble(Ƽ+1)/ɟ)));RectangleF Ņ=new RectangleF(e.Position+new Vector2(0,ǂ.Height)+new Vector2(
e.Width/ɟ*Convert.ToSingle(ĉ-1),(e.Height-2f*ǂ.Height)/ɠ*Convert.ToSingle(Ĉ-1)),new Vector2(e.Width/ɟ,(e.Height-2f*ǂ.
Height)/ɠ));ǋ(ref f,Ņ,ƻ,á,Â);}}RectangleF ƺ=new RectangleF(new Vector2(e.X,e.Bottom-ǂ.Height),new Vector2(e.Width,ǂ.Height)),ƹ
=В(ƺ,Á,2),ǅ=В(ƹ,Á,2);έ(ref f,ƹ,ȏ);Ϋ(ref f,ǃ,ǅ,R*Â,ȣ,TextAlignment.RIGHT);RectangleF Ƹ=new RectangleF(new Vector2(e.X,e.
Bottom-ǂ.Height),new Vector2(ǂ.Height,ǂ.Height)),ǆ=В(Ƹ,Á,2);Λ(ref f,"AH_BoreSight",ǆ,ȣ,270f);RectangleF ǎ=new RectangleF(new
Vector2(e.X+ǂ.Height,e.Bottom-ǂ.Height),new Vector2(e.Width*0.5f-ǂ.Height,ǂ.Height)),Ǎ=В(ǎ,Á,2),ǌ=В(Ǎ,Á,2);Ϋ(ref f,Ǆ,ǌ,R*Â,ȣ);}
void ǋ(ref MySpriteDrawFrame f,RectangleF e,int Ċ,int á,float Ĺ){string Ô=ə[Ċ],ǉ=Ϫ(ˢ,ˡ),ǈ=Ϫ(ˢ,ʦ),ƚ="("+ǈ+"/"+ə.Count.
ToString()+")";double Ŏ=0;int ß=Convert.ToInt16(ǈ);RectangleF ŋ=В(e,0.96f,2);έ(ref f,ŋ,ȏ);RectangleF Ŋ=В(ŋ,0.95f,2);if(Ô!=ˡ)Ϋ(
ref f,Ļ(Ô),Ŋ,0.53f*Ĺ,ȣ);RectangleF ŉ=new RectangleF(ŋ.Position+new Vector2(0,ŋ.Height*0.12f),new Vector2(ŋ.Width,ŋ.Width));
if(Ô==ˡ)λ(ref f,ŉ,ȣ,ȏ);else Λ(ref f,Ô,ŉ,ȣ);RectangleF ň=new RectangleF(ŉ.Position+new Vector2(0,ŉ.Height*0.96f),new
Vector2(ŋ.Width,ŋ.Height*0.2f)),Ň=В(ň,0.93f,2),Ǿ=new RectangleF(new Vector2(ŋ.Right,ŋ.Bottom)-new Vector2(ŋ.Height*0.12f,ŋ.
Height*0.12f),new Vector2(ŋ.Height*0.12f,ŋ.Height*0.12f)),Ǽ=В(Ǿ,0.93f,2);if(Ô!=ˡ){if(ɖ[á-1].ȯ.ContainsKey(Ô))Ŏ=ɖ[á-1].ȯ[Ô];if(
Ŏ!=0){Ϋ(ref f,ί(Ŏ/1000000,false),Ň,0.8f*Ĺ,ȣ,TextAlignment.RIGHT);Λ(ref f,"AH_BoreSight",Ǽ,ȣ,270f);}}else{Ϋ(ref f,ǉ,Ň,0.8f
*Ĺ,ȣ,TextAlignment.RIGHT);}if(ß==Ċ+1){RectangleF ǻ=new RectangleF(new Vector2(ň.X,ň.Y+ň.Height),new Vector2(ŋ.Height*
0.13f,ŋ.Height*0.13f)),Ǻ=В(ǻ,0.96f);Λ(ref f,"Arrow",Ǻ,ȣ);RectangleF ǹ=new RectangleF(new Vector2(ň.X+ŋ.Height*0.13f,ň.Y+ň.
Height),new Vector2(ň.Width-2f*ŋ.Height*0.13f,ŋ.Height*0.13f)),Ǹ=В(ǹ,0.96f);Ϋ(ref f,ƚ,ǹ,0.65f*Ĺ,ȣ,TextAlignment.CENTER);}}
string Ƿ(int ú){string Ƕ;if(ú==1)Ƕ=Me.CustomName;else Ƕ=ʉ[ú-2].CustomName;return ú.ToString()+". "+Ƕ+" ("+ú.ToString()+"/"+(ʉ.
Count+1).ToString()+")";}string ǵ(int ú){if(ú==1)return Ϫ(ˢ,ˡ);else return Ϫ(ʉ[ú-2],ˢ,ˡ);}void Ǵ(){Echo(
"Build_Production_List");int ǳ=Convert.ToInt16(Ϫ(ˎ,ˍ));Ș=new Ȝ[ǳ];for(int ê=1;ê<=ǳ;ê++){string Ă=Ϫ(ˎ,ê.ToString());string[]ǲ=Ă.Split(':');if(ǲ.
Length==3){Ș[ê-1].ț=ǲ[0];Ș[ê-1].Ț=ǲ[1];Ș[ê-1].ș=Convert.ToDouble(ǲ[2]);}}}void ǽ(){const int Ĩ=2;if(ʊ.Count<1){ͻ=true;ɨ=1;
return;}Echo($"{ɨ}/{Ĩ}");switch(ɨ){case 1:Echo("PrepareData");ǿ();Ȉ();break;case 2:Echo("SendProductionOrder");ȇ();break;}if(ɨ
>=Ĩ){ͻ=true;ɨ=1;return;}ɨ++;}void ǿ(){Ȭ=new Dictionary<string,double>();foreach(var W in ʊ){List<MyProductionItem>Û=new
List<MyProductionItem>();W.GetQueue(Û);foreach(var Ú in Û){if(Ȭ.ContainsKey(Ú.BlueprintId.ToString()))Ȭ[Ú.BlueprintId.
ToString()]+=(double)Ú.Amount.RawValue;else Ȭ.Add(Ú.BlueprintId.ToString(),(double)Ú.Amount.RawValue);}}}void Ȉ(){Ȫ=new
Dictionary<string,double>();Ȫ=Ȳ;foreach(var Ú in Ș){if(Ȭ.ContainsKey(Ú.Ț)){if(Ȳ.ContainsKey(Ú.ț)){Ȫ[Ú.ț]+=Ȭ[Ú.Ț];}else{Ȫ.Add(Ú.ț,Ȭ
[Ú.Ț]);}}}}void ȇ(){foreach(var Ú in Ș){if(Ȫ.ContainsKey(Ú.ț)){double Ȇ=Ú.ș*1000000-Ȫ[Ú.ț];if(Ȇ>0)ȉ(Ú.Ț,Ȇ/1000000);}else{
ȉ(Ú.Ț,Ú.ș);}}}void ȉ(string Ô,double Ġ){foreach(var W in ʊ){if(!W.CooperativeMode&&(W.BlockDefinition.SubtypeName==
"LargeAssembler"||W.BlockDefinition.SubtypeName=="LargeAssemblerIndustrial")){MyDefinitionId Ȅ=MyDefinitionId.Parse(Ô);W.AddQueueItem(Ȅ,
Ġ);return;}}}void ȃ(){if(ʃ.Count<2){ͻ=true;return;}Echo($"{ɧ}/2");switch(ɧ){case 1:Echo("TransferItemsToFrontBoxes");Ȃ();
break;case 2:Echo("CargoContainerCycle");Ǥ();break;}}void Ȃ(){ɧ=2;for(int ȁ=ʃ.Count;ȁ>=2;ȁ--){IMyCargoContainer Ǣ=ʃ[ȁ-1];if(Ǣ
.GetInventory().CurrentVolume==0)continue;for(int Ȁ=1;Ȁ<ȁ;Ȁ++){if(Ǣ.GetInventory().CurrentVolume==0)break;
IMyCargoContainer ȅ=ʃ[Ȁ-1];if(ȅ.GetInventory().CurrentVolume==ȅ.GetInventory().MaxVolume)continue;List<MyInventoryItem>Û=new List<
MyInventoryItem>();Ǣ.GetInventory().GetItems(Û);foreach(var Ú in Û){Ǣ.GetInventory().TransferItemTo(ȅ.GetInventory(),Ú);}}}}void Ǥ(){
for(int ê=1;ê<=2;ê++){int ǣ=ɮ+ê-1;if(ǣ>ʃ.Count){ɮ=1;ǣ=ɮ+ê-1;}IMyCargoContainer Ǣ=ʃ[ǣ-1];List<MyInventoryItem>Û=new List<
MyInventoryItem>();Ǣ.GetInventory().GetItems(Û);foreach(var Ú in Û){Ǣ.GetInventory().TransferItemTo(Ǣ.GetInventory(),Ú);}}ͻ=true;ɧ=1;}
void ǡ(){if(ɳ.Count>=1){int Ǡ=Convert.ToInt16(Math.Ceiling(Convert.ToDouble(ɳ.Count)/10));Echo($"{ɦ}/{Ǡ}");for(int ǟ=0;ǟ<10;
ǟ++){int Ǟ=ǟ+(ɦ-1)*10;if(Ǟ>=ɳ.Count)break;ǰ(ɳ[Ǟ]);}ɦ++;if(ɦ>Ǡ)ɦ=1;}foreach(var s in ɲ){Ɓ(s,"IconHydrogen");}foreach(var s
in ʁ){Ɓ(s,"IconOxygen");}foreach(var s in ɱ){Ɓ(s,"Textures\\FactionLogo\\Others\\OtherIcon_19.dds");}foreach(var s in ʂ){Ɓ
(s,"IconEnergy");}foreach(var s in ʕ){Ɓ(s,"Refinery");}foreach(var s in ʓ){Ɓ(s,"Assembler");}foreach(var s in ʒ){Ɓ(s,
"Bedroom");}foreach(var s in ʑ){Ɓ(s,"Washroom");}foreach(var s in ʐ){Ɓ(s,"Canteen");}foreach(var s in ʏ){Ɓ(s,"GasGenerator");}
foreach(var s in ʎ){Ɓ(s,"CargoContainer");}foreach(var s in ʍ){Ɓ(s,"Server");}ͻ=true;}void ǰ(IMyTextPanel s){if(!s.Enabled)
return;string ǯ=Ϫ(s,ʸ,ʴ),Ǯ=Ϫ(s,ʸ,ʳ),Ǳ=Ϫ(s,ʶ,ʴ),ǭ=Ϫ(s,ʶ,ʳ),Ǭ=Ϫ(s,ʵ,ʴ),ǫ=Ϫ(s,ʵ,ʳ),Ǫ=Ϫ(s,ʷ,ʴ),ǩ=Ϫ(s,ʷ,ʳ);int Ǩ=Convert.ToInt16(Ϫ(
s,ʸ,ʲ)),ǧ=Convert.ToInt16(Ϫ(s,ʶ,ʲ)),Ǧ=Convert.ToInt16(Ϫ(s,ʵ,ʲ)),ǥ=Convert.ToInt16(Ϫ(s,ʷ,ʲ));float Ʒ=Convert.ToSingle(Ϫ(s,
ʗ,ʩ)),Ź=Convert.ToSingle(Ϫ(s,ʗ,ʺ));float Ǝ=42.5f,ŷ=Ǝ,Ŷ=1,ŵ=1.3f;if(ȧ.ContainsKey(s.EntityId)){if(ȧ[s.EntityId]==0f)ȧ[s.
EntityId]=1f;else ȧ[s.EntityId]=0f;}else ȧ.Add(s.EntityId,0f);float Ŵ=ȧ[s.EntityId];RectangleF z=new RectangleF((s.TextureSize-s
.SurfaceSize)/2f+new Vector2(0,Ŵ),s.SurfaceSize),ų=new RectangleF(z.Position+new Vector2(Ʒ,Ź),z.Size-new Vector2(Ʒ*2f,Ź*
2f));ŷ=ų.Height/12f;Ŷ=ŷ/Ǝ;ŵ=ŵ*Ŷ;RectangleF Ų=new RectangleF(new Vector2(ų.Center.X-ŷ/2f,ų.Y),new Vector2(ŷ,ŷ)),ű=new
RectangleF(Ų.Position+new Vector2(Ų.Width,0),Ų.Size),Ű=new RectangleF(new Vector2(ų.X,Ų.Y+ŷ),new Vector2(ų.Width,ŷ)),ů=new
RectangleF(new Vector2(ų.X,Ű.Y+ŷ),Ű.Size),Ů=new RectangleF(new Vector2(z.X,ų.Y),new Vector2(z.Width,ŷ*3)),ŭ=new RectangleF(new
Vector2(ų.X,ů.Y+ŷ),new Vector2(ŷ,ŷ)),Ŭ=new RectangleF(ŭ.Position+new Vector2(ŭ.Width,0),ŭ.Size),ū=new RectangleF(new Vector2(ų.
X,ŭ.Y+ŷ),Ű.Size),Ū=new RectangleF(new Vector2(ų.X,ū.Y+ŷ),Ű.Size),Ÿ=new RectangleF(new Vector2(ų.Right-ŷ,Ū.Y+ŷ),new
Vector2(ŷ,ŷ)),ũ=new RectangleF(Ÿ.Position-new Vector2(Ÿ.Width,0),Ÿ.Size),ź=new RectangleF(new Vector2(ų.X,Ÿ.Y+ŷ),Ű.Size),Ƈ=new
RectangleF(new Vector2(ų.X,ź.Y+ŷ),Ű.Size),Ɔ=new RectangleF(Ů.Position+new Vector2(0,Ů.Height*2f),Ů.Size),ƅ=new RectangleF(new
Vector2(ų.Center.X-ŷ/2f,Ƈ.Y+ŷ),new Vector2(ŷ,ŷ)),Ƅ=new RectangleF(ƅ.Position+new Vector2(ƅ.Width,0),ƅ.Size),ƃ=new RectangleF(
new Vector2(ų.X,ƅ.Y+ŷ),Ű.Size),Ƃ=new RectangleF(new Vector2(ų.X,ƃ.Y+ŷ),Ű.Size);MySpriteDrawFrame f=s.DrawFrame();Λ(ref f,
"SquareSimple",z,Ȧ);if(Ǯ!=""||ǯ!=""){έ(ref f,Ů,Ȣ);Λ(ref f,"Arrow",Ų,Color.White);Ϋ(ref f,ǯ,Ű,ŵ,ȣ,TextAlignment.CENTER);Ϋ(ref f,Ǯ,ů,ŵ,ȣ
,TextAlignment.CENTER);if(Ǩ!=0)Ƥ(ref f,ű,ȣ,Ǩ);}if(ǭ!=""||Ǳ!=""){Λ(ref f,"Arrow",ŭ,Color.White,270f);Ϋ(ref f,Ǳ,ū,ŵ,ȣ);Ϋ(
ref f,ǭ,Ū,ŵ,ȣ);if(ǧ!=0)Ƥ(ref f,Ŭ,ȣ,ǧ);}if(ǫ!=""||Ǭ!=""){έ(ref f,Ɔ,Ȣ);Λ(ref f,"Arrow",Ÿ,Color.White,90f);Ϋ(ref f,Ǭ,ź,ŵ,ȣ,
TextAlignment.RIGHT);Ϋ(ref f,ǫ,Ƈ,ŵ,ȣ,TextAlignment.RIGHT);if(Ǧ!=0)Ƥ(ref f,ũ,ȣ,Ǧ);}if(ǩ!=""||Ǫ!=""){Λ(ref f,"Arrow",ƅ,Color.White,180f
);Ϋ(ref f,Ǫ,ƃ,ŵ,ȣ,TextAlignment.CENTER);Ϋ(ref f,ǩ,Ƃ,ŵ,ȣ,TextAlignment.CENTER);if(ǥ!=0)Ƥ(ref f,Ƅ,ȣ,ǥ);}f.Dispose();}void Ɓ
(IMyTextPanel s,string ƀ){if(!s.Enabled)return;if(ȧ.ContainsKey(s.EntityId)){if(ȧ[s.EntityId]==0f)ȧ[s.EntityId]=1f;else ȧ
[s.EntityId]=0f;}else ȧ.Add(s.EntityId,0f);float Ŵ=ȧ[s.EntityId];float ž=s.SurfaceSize.X,Ž=s.SurfaceSize.Y;if(ž<Ž)Ž=ž;
else ž=Ž;RectangleF z=new RectangleF((s.TextureSize-s.SurfaceSize)/2f+new Vector2(0,Ŵ),s.SurfaceSize),ż=new RectangleF(new
Vector2(z.Center.X-ž/2f,z.Center.Y-Ž/2f),new Vector2(ž,Ž));ż=В(ż,0.95f);MySpriteDrawFrame f=s.DrawFrame();Λ(ref f,
"SquareSimple",z,Ȧ);switch(ƀ){case"Refinery":λ(ref f,ż,ȣ,Ȧ);break;case"Assembler":ϋ(ref f,ż,ȣ);break;case"Bedroom":Ż(ref f,ż,ȣ);break;
case"Washroom":ŗ(ref f,ż,ȣ);break;case"Canteen":Ŧ(ref f,ż,ȣ);break;case"GasGenerator":Ţ(ref f,ż,ȣ);break;case
"CargoContainer":ϐ(ref f,ż,ȣ,Ȧ);break;case"Server":Ɯ(ref f,ż,ȣ,Ȧ);break;default:Λ(ref f,ƀ,ż,ȣ);break;}f.Dispose();}void Ż(ref
MySpriteDrawFrame f,RectangleF e,Color d){RectangleF Ő=new RectangleF(new Vector2(e.X,e.X+2f/3f*e.Height),new Vector2(e.Width,e.Height/6f
)),ś=new RectangleF(new Vector2(e.X,e.Center.Y),new Vector2(e.Width*0.05f,e.Height*0.5f)),Ś=new RectangleF(new Vector2(e.
Right-ś.Width,Ő.Y),new Vector2(ś.Width,Ő.Height*2f)),ř=new RectangleF(new Vector2(e.X+ś.Width*2f,Ő.Y-ś.Width-e.Height*0.2f),
new Vector2(e.Height*0.2f,e.Height*0.2f)),Ř=new RectangleF(new Vector2(ř.X+ř.Width+ś.Width,ř.Y),new Vector2(e.Width-3f*ś.
Width-ř.Width,e.Height*0.2f));έ(ref f,Ő,d);έ(ref f,ś,d);έ(ref f,Ś,d);Λ(ref f,"LCD_Emote_Sleepy",ř,d,270f);Λ(ref f,
"RightTriangle",Ř,d);}void ŗ(ref MySpriteDrawFrame f,RectangleF e,Color d){float Ĺ=0.95f;RectangleF Ŗ=new RectangleF(e.Position,new
Vector2(e.Width/3f,e.Height/3f)),ŕ=В(Ŗ,Ĺ),Ŕ=new RectangleF(e.Position+new Vector2(0,Ŗ.Height),new Vector2(Ŗ.Width,Ŗ.Height*2f))
,œ=В(Ŕ,Ĺ),Œ=new RectangleF(new Vector2(e.Center.X,e.Y)-new Vector2(e.Width*0.05f*0.5f,0),new Vector2(e.Width*0.05f,e.
Height)),ő=new RectangleF(e.Position+new Vector2(Ŗ.Width*2f,0),Ŗ.Size),Ŝ=В(ő,Ĺ),ŝ=new RectangleF(ő.Position+new Vector2(0,ő.
Height),Ŕ.Size),ŧ=В(ŝ,Ĺ),Ũ=new RectangleF(new Vector2(ŧ.Center.X-ŧ.Width*0.1f*0.5f,ŧ.Y),new Vector2(ŧ.Width*0.1f,ŧ.Height));Λ(
ref f,"LCD_Emote_Suspicious_Right",ŕ,d);Λ(ref f,"Triangle",œ,d,180f);έ(ref f,Œ,d);Λ(ref f,"LCD_Emote_Suspicious_Left",ő,d);
Λ(ref f,"Triangle",ŧ,d);έ(ref f,Ũ,d);}void Ŧ(ref MySpriteDrawFrame f,RectangleF e,Color d){RectangleF ť=new RectangleF(
new Vector2(e.Center.X-e.Width*0.6f*0.5f,e.Y+e.Height*0.6f*0.2f),new Vector2(e.Width*0.6f,e.Height*0.6f)),Ť=new RectangleF(
new Vector2(ť.Right,ť.Y)-new Vector2(e.Width*0.4f*0.5f,0),new Vector2(e.Width*0.4f,e.Height*0.4f)),ţ=new RectangleF(new
Vector2(e.X,e.Bottom)-new Vector2(0,e.Height*0.2f),new Vector2(e.Width,e.Height*0.1f));Λ(ref f,"CircleHollow",Ť,d);έ(ref f,ť,d)
;έ(ref f,ţ,d);}void Ţ(ref MySpriteDrawFrame f,RectangleF e,Color d){float Ĺ=0.95f;RectangleF š=new RectangleF(e.Position,
new Vector2(e.Width*0.5f,e.Height*0.5f)),Š=В(š,Ĺ),ş=new RectangleF(e.Position+new Vector2(š.Width,0),š.Size),ſ=В(ş,Ĺ),Ş=new
RectangleF(new Vector2(e.Center.X,e.Y)+new Vector2(-š.Width*0.5f,š.Height),š.Size),ƈ=В(Ş,Ĺ),ƨ=new RectangleF(new Vector2(e.X,Ş.Y),
new Vector2(Ş.Width*0.5f,Ş.Height)),Ƨ=В(ƨ,Ĺ),Ʀ=new RectangleF(new Vector2(Ş.X+Ş.Width,Ş.Y),ƨ.Size),ƥ=В(Ʀ,Ĺ);Λ(ref f,
"IconHydrogen",Š,d);Λ(ref f,"IconOxygen",ſ,d);Λ(ref f,"MyObjectBuilder_Ore/Ice",ƈ,d);Λ(ref f,"AH_PullUp",Ƨ,d);Λ(ref f,"AH_PullUp",ƥ,d)
;}void Ƥ(ref MySpriteDrawFrame f,RectangleF e,Color d,int ƣ){RectangleF Ƣ=new RectangleF(e.Position,e.Size),ơ=new
RectangleF(e.Position,e.Size*0.25f),Ơ=new RectangleF(e.Position+ơ.Size,ơ.Size),Ɵ=new RectangleF(Ơ.Position+ơ.Size,ơ.Size),ƞ=new
RectangleF(Ɵ.Position+ơ.Size,ơ.Size),Ɲ=new RectangleF(new Vector2(e.Center.X,e.Y),e.Size*0.5f);Λ(ref f,"RightTriangle",Ƣ,d);έ(ref
f,ơ,d);έ(ref f,Ơ,d);έ(ref f,Ɵ,d);έ(ref f,ƞ,d);if(ƣ==-1)Λ(ref f,"Arrow",Ɲ,d,135f);else if(ƣ==1)Λ(ref f,"Arrow",Ɲ,d,315f);}
void Ɯ(ref MySpriteDrawFrame f,RectangleF e,Color d,Color n){for(int ú=1;ú<=4;ú++){RectangleF Ƶ=new RectangleF(e.Position+
new Vector2(0,e.Height*0.25f*Convert.ToSingle(ú-1)),new Vector2(e.Width,e.Height*0.25f)),ƴ=В(Ƶ,0.9f,2);if(ú<4){έ(ref f,ƴ,d)
;RectangleF Ƴ=new RectangleF(Ƶ.Position,new Vector2(ƴ.Height,ƴ.Height)),Ʋ=В(Ƴ,0.5f);Λ(ref f,"Circle",Ʋ,n);for(int Ʊ=1;Ʊ<=
5;Ʊ++){RectangleF ư=new RectangleF(new Vector2(ƴ.Center.X,ƴ.Y)+new Vector2(0,ƴ.Height*0.2f)+new Vector2(ƴ.Height*0.2f*
Convert.ToSingle(Ʊ-1)*2f,0),new Vector2(ƴ.Height*0.2f,ƴ.Height*0.6f));έ(ref f,ư,n);}}else{RectangleF ƶ=new RectangleF(new
Vector2(ƴ.Center.X,ƴ.Y)+new Vector2(-ƴ.Height*0.25f*0.5f,0),new Vector2(ƴ.Height*0.25f,ƴ.Height)),Ư=new RectangleF(new Vector2(
ƴ.X,ƴ.Bottom)+new Vector2(0,-ƴ.Height*0.25f),new Vector2(ƴ.Width,ƴ.Height*0.25f));έ(ref f,ƶ,d);έ(ref f,Ư,d);}}}void Ʈ(
string ƭ){if(ƭ==""||ƭ==null)return;if(ƭ=="CLS"){ƫ();}else if(ƭ=="CO_ON"){ƪ();}else if(ƭ=="CO_OFF"){Ʃ();}else if(ƭ=="LCD_REF"){
ƛ();}else if(ƭ=="DEFAULT"){Ƌ();}else if(ƭ=="OK"){Ɠ();}else{string[]Ƭ=ƭ.Split('=');if(Ƭ.Length==2){switch(Ƭ[0]){case
"RE_REF":Ɗ(ʉ,Ƭ[1]);break;case"RE_ASS":Ɗ(ʊ,Ƭ[1]);break;case"RE_BOX":Ɗ(ʃ,Ƭ[1]);break;}}}}void ƫ(){foreach(var W in ʊ)W.ClearQueue(
);}void ƪ(){foreach(var W in ʊ)if(W.CooperativeMode!=true)W.CooperativeMode=true;}void Ʃ(){foreach(var W in ʊ)if(W.
CooperativeMode!=false)W.CooperativeMode=false;}void ƛ(){List<IMyTextPanel>ƕ=new List<IMyTextPanel>();ƕ.AddRange(ɽ);ƕ.AddRange(ɼ);ƕ.
AddRange(ɻ);ƕ.AddRange(ɺ);ƕ.AddRange(ɹ);ƕ.AddRange(ɸ);ƕ.AddRange(ɷ);ƕ.AddRange(ɶ);foreach(var ƍ in ƕ){float ƌ=Convert.ToSingle(ɕ
);if(ƍ.ContentType!=ContentType.SCRIPT)ƍ.ContentType=ContentType.SCRIPT;MySpriteDrawFrame f=ƍ.DrawFrame();RectangleF e=
new RectangleF((ƍ.TextureSize-ƍ.SurfaceSize)/2f,ƍ.SurfaceSize+new Vector2(0,ƌ));Λ(ref f,"UVChecker",e.Center.X,e.Center.Y,e
.Width,e.Height,Color.White);f.Dispose();}ɑ=0;ɐ=0;ͼ=false;ͻ=false;}void Ƌ(){Me.CustomData="";ɇ();}void Ɗ(List<IMyRefinery
>Ɖ,string ý){int ê=1;foreach(var ë in Ɖ){ë.CustomName=ƙ(Ɖ.Count,ý,ê);ê++;}}void Ɗ(List<IMyAssembler>Ɖ,string ý){int ê=1;
foreach(var ë in Ɖ){ë.CustomName=ƙ(Ɖ.Count,ý,ê);ê++;}}void Ɗ(List<IMyCargoContainer>Ɖ,string ý){string ÿ=Ϫ(ʨ,ˬ);double þ=
Convert.ToDouble(ÿ)/1000;int ê=1;foreach(var ë in Ɖ){if((double)ë.GetInventory().MaxVolume<þ)continue;ë.CustomName=ƙ(Ɖ.Count,ý,
ê);Ⱥ(ë,ʨ,ˮ,ƙ(Ɖ.Count,ý,ê));ê++;}}string ƙ(int Ƙ,string ý,int Ɨ){string ƚ=Ɨ.ToString();Ƙ=Convert.ToString(Ƙ).Length;int Ɩ=
Ƙ-ƚ.Length;for(int Ɣ=1;Ɣ<=Ɩ;Ɣ++){ƚ="0"+ƚ;}ý=ý+"_"+ƚ;return ý;}void Ɠ(){List<IMyFunctionalBlock>ƒ=new List<
IMyFunctionalBlock>();GridTerminalSystem.GetBlocksOfType(ƒ,Ƒ=>Ƒ.IsSameConstructAs(Me));foreach(var ë in ƒ)if(!ë.Enabled)ë.Enabled=true;}
void Ɛ(){ö();if(ɗ.Count<1&&ɘ.Count<1)return;else if(ɗ.Count>=1&&ɘ.Count<1){if(ͼ)ɑ++;ͼ=false;if(ɑ>=ɘ.Count)ɑ=0;ʹ=ɗ[ɑ];}else
if(ɗ.Count<1&&ɘ.Count>=1){if(ͻ)ɐ++;ͻ=false;if(ɐ>=ɘ.Count)ɐ=0;ʹ=ɘ[ɐ];}else{switch(ɒ){case 0:ɒ=1;if(ͼ)ɑ++;ͼ=false;if(ɑ>=ɗ.
Count)ɑ=0;ʹ=ɗ[ɑ];break;case 1:ɒ=0;if(ͻ)ɐ++;ͻ=false;if(ɐ>=ɘ.Count)ɐ=0;ʹ=ɘ[ɐ];break;}}Echo($"{ʹ}");switch(ʹ){case ː:û();break;
case ˊ:Ǉ();break;case ˉ:Ķ();break;case ˈ:ǽ();break;case ˇ:Y();break;case ˆ:õ();break;case ˁ:ô();break;case ˀ:ó();break;case
ʿ:ò(ʋ,ref ɭ,"HydrogenBottle");break;case ʾ:ò(ɰ,ref ɬ,"OxygenBottle");break;case ʽ:ȃ();break;case ˠ:Ā();break;case ʼ:Ǖ();
break;case Έ:ǡ();break;case ˋ:ϭ();break;}}void Main(string ƭ,UpdateType Ə){if(ͱ){ͱ=false;return;}Echo("Main");ϳ();Ɛ();Ʈ(ƭ);â(
);}