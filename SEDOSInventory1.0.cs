/*
 * README   SE-DOS INVENTORY 1.0
 * =========
 * Hello Space Engineer, this script was created to be able to visualize inventories 
 * and filter them according to the item type and also which container we want to display, 
 * including assemblers and refineries as well.
 * 
 * To use this script, you must pass a comma-separated text. 
 * In this text, you should include
 * the screen name, the type of item to display, and the container 
 * and this text is separated by an underscore (_).
 * 
 * The example format is SCREEN_ITEM_CONTAINER
 * 
 * LCD1_ALL_container1
 * In this example, we will display on the screen named LCD1 
 * all the items from the container called container1.
 * 
 * LCD2_COMPONENT_container2
 * In this example, we will display on the screen named LCD2 
 * component-type items from the container called container2.
 * 
 * LCD1_ALL_ALL
 * In this example, we will display on the screen named LCD1 
 * all the items from the all containers.
 * 
 * For refineries and assemblers, we should put the name of the block in the parameter as:
 * LCD1_ALL_Assembler1
 * 
 * ITEMS
 * ========
 * For items, we have the following parameters.
 * 
 * INGOT
 * TOOL
 * ORE
 * AMMO
 * COMPONENT
 * ALL   (ALL ITEMS)
 * 
 * Examples:
 * 
 * LCD2_COMPONENT_container2
 * In this example, we will display on the screen named LCD2 
 * component-type items from the container called container2.
 * 
 * LCD2_ORE_container2
 * In this example, we will display on the screen named LCD2 
 * ore-type items from the container called container2.
 */

//===========INSERT THE PARAMETERS SEPARATED BY COMMAS INSIDE THE CURLY BRACES HERE IN ""============//

string[]q={"LCD1_ALL_container1","LCD2_ALL_container2"};

//=============================================================================================//

//If you want to deactivate the credits on the programmable block screen, set it to 0; otherwise, leave it as 1.

int p=1;

//=============================================================================================//

/*
 * Enjoy the script! I'm open to discussions, suggestions, and ideas for future projects. 
 * This script is part of a larger project, which is an Operating System for Space Engineers - SE-DOS.
 * 
 * Credits:
 * Atte: DrHousexx
 * Steam Profile: https://steamcommunity.com/id/drhousexx/
 * GitHub: https://github.com/DrHousexx
 * WebSite Developer: https://CaritaKawai.com/
 * 
 * 
 * 
 * Developed whit: Malware's Development Kit for SE
 * Thanks to Malware-Dev for malware-dev/MDK-SE: Malware's Development Kit for SE.
 * Thanks to Whiplash141 for SESpriteBuilder.
*/


string[]o;string[]n;string[]m;int[]l;
IMyProgrammableBlock k;IMyTextSurface j,h;MySpriteDrawFrame g,f;Program(){o=new string[q.Length];n=new string[q.Length];m=new string[q.
Length];l=new int[q.Length];for(int H=0;H<q.Length;H++){string[]e=q[H].Split('_');l[H]=0;if(e.Length==3){o[H]=e[0];n[H]=e[1];m
[H]=e[2];}else{Echo("Each parameter should be in the following format: SCREEN_ITEM_CONTAINER.");}}k=GridTerminalSystem.
GetBlockWithName(Me.CustomName)as IMyProgrammableBlock;j=k.GetSurface(0);h=k.GetSurface(1);j.ContentType=VRage.Game.GUI.TextPanel.
ContentType.SCRIPT;j.ScriptBackgroundColor=new Color(0,0,0,255);h.ContentType=VRage.Game.GUI.TextPanel.ContentType.SCRIPT;h.
ScriptBackgroundColor=new Color(0,0,0,255);Runtime.UpdateFrequency=UpdateFrequency.Update100;}void Main(string r,UpdateType s){Echo(
"Parameters found: "+q.Length);Echo("========================");Echo("| SCREEN | ITEM | CONTAINER |");Echo("========================");for(
int H=0;H<q.Length;H++){l[H]=º(o[H],m[H],n[H],l[H]);}Echo("========================");if(p==1){Vector2 Á=j.TextureSize*0.5f
;g=j.DrawFrame();V(g,Á,1f);g.Dispose();}else{Vector2 Á=j.TextureSize*0.5f;g=j.DrawFrame();U(g,Á,k.CustomName,o.Length.
ToString(),Me.CubeGrid.CustomName,1f);g.Dispose();}Vector2 À=h.TextureSize*0.5f;f=h.DrawFrame();U(f,À,k.CustomName,o.Length.
ToString(),Me.CubeGrid.CustomName,1f);f.Dispose();}int º(string µ,string ª,string z,int y){IMyTextSurface x;MySpriteDrawFrame w;
x=GridTerminalSystem.GetBlockWithName(µ)as IMyTextSurface;Vector2 E=x.TextureSize*0.5f;x.ContentType=VRage.Game.GUI.
TextPanel.ContentType.SCRIPT;x.ScriptBackgroundColor=new Color(0,0,0,255);List<MyInventoryItem>v=new List<MyInventoryItem>();List
<IMyTerminalBlock>u=new List<IMyTerminalBlock>();if(ª=="ALL"){GridTerminalSystem.GetBlocks(u);for(int H=0;H<u.Count;H++){
if(u[H].HasInventory){if(u[H].BlockDefinition.TypeIdString.Contains("Assembler")){IMyAssembler d=u[H]as IMyAssembler;d.
InputInventory.GetItems(v);d.OutputInventory.GetItems(v);}else if(u[H].BlockDefinition.TypeIdString.Contains("Refinery")){IMyRefinery
c=u[H]as IMyRefinery;c.InputInventory.GetItems(v);c.OutputInventory.GetItems(v);}else{u[H].GetInventory().GetItems(v);}}}
ª="All containers";}else{IMyTerminalBlock t=GridTerminalSystem.GetBlockWithName(ª);if(t.BlockDefinition.TypeIdString.
Contains("Assembler")){IMyAssembler d=GridTerminalSystem.GetBlockWithName(ª)as IMyAssembler;d.InputInventory.GetItems(v);d.
OutputInventory.GetItems(v);}else if(t.BlockDefinition.TypeIdString.Contains("Refinery")){IMyRefinery c=GridTerminalSystem.
GetBlockWithName(ª)as IMyRefinery;c.InputInventory.GetItems(v);c.OutputInventory.GetItems(v);}else{GridTerminalSystem.GetBlockWithName(ª
).GetInventory().GetItems(v);}}List<string>A=new List<string>();List<double>O=new List<double>();for(int H=0;H<v.Count;H
++){if(z=="ALL"){A.Add((v[H].Type.TypeId+"/"+v[H].Type.SubtypeId).ToString());O.Add(((float)v[H].Amount));}else if(z==
"INGOT"&&v[H].Type.GetItemInfo().IsIngot){A.Add((v[H].Type.TypeId+"/"+v[H].Type.SubtypeId).ToString());O.Add(((float)v[H].
Amount));}else if(z=="TOOL"&&v[H].Type.GetItemInfo().IsTool){A.Add((v[H].Type.TypeId+"/"+v[H].Type.SubtypeId).ToString());O.
Add(((float)v[H].Amount));}else if(z=="ORE"&&v[H].Type.GetItemInfo().IsOre){A.Add((v[H].Type.TypeId+"/"+v[H].Type.SubtypeId
).ToString());O.Add(((float)v[H].Amount));}else if(z=="AMMO"&&v[H].Type.GetItemInfo().IsAmmo){A.Add((v[H].Type.TypeId+"/"
+v[H].Type.SubtypeId).ToString());O.Add(((float)v[H].Amount));}else if(z=="COMPONENT"&&v[H].Type.GetItemInfo().
IsComponent){A.Add((v[H].Type.TypeId+"/"+v[H].Type.SubtypeId).ToString());O.Add(((float)v[H].Amount));}}string[]N=A.ToArray();
double[]M=O.ToArray();if(N.Length>16){int L=(y*15)+y;int K=((y+1)*16)<N.Length?15:(N.Length-L)-1;string[]J=new string[K+1];
double[]I=new double[K+1];for(int H=0;H<=K;H++){J[H]=N[H+L];I[H]=M[H+L];}w=x.DrawFrame();G(w,E,µ+": Inventory "+z+" of "+ª,1,J
,I,1f);w.Dispose();}else{w=x.DrawFrame();G(w,E,µ+": Inventory "+z+" of "+ª,1,N,M,1f);w.Dispose();}y=(y<(N.Length/16))?y+1
:0;Echo(µ+" - "+z+" - "+ª);return y;}void G(MySpriteDrawFrame F,Vector2 E,string D,int C,string[]B,double[]P,float R=1f){
F.Add(new MySprite(SpriteType.TEXT,D,new Vector2(-240f,-245f)*R+E,null,new Color(0,128,255,255),"DEBUG",TextAlignment.
LEFT,1f*R));if(p==1){F.Add(new MySprite(SpriteType.TEXT,"SE-DOS (By:DrHousexx)",new Vector2(-255f,239f)*R+E,null,new Color(
0, 128, 255, 255),"DEBUG",TextAlignment.LEFT,0.6f*R));}else{F.Add(new MySprite(SpriteType.TEXT,"SE-DOS",new Vector2(-255f,239f)*
R+E,null,new Color(0, 128, 255, 255),"DEBUG",TextAlignment.LEFT,0.6f*R));}C=C==0?80:150;F.Add(new MySprite(SpriteType.TEXTURE,
"Circle",new Vector2(C,245f)*R+E,new Vector2(10f,10f)*R,new Color(0,255,0,255),null,TextAlignment.CENTER,0f));Dictionary<string,
int>a=new Dictionary<string,int>();string[]b=new string[16];string[]Z=new string[16];string[]Y=new string[16];for(int H=0;H
<16;H++){if(H<B.Length){b[H]=B[H];string[]X=b[H].Split('/');Z[H]=X[1];if(P[H]%1!=0){if(P[H]>=1000){P[H]=Math.Round(P[H]/
1000.0,1);string W=P[H].ToString("N1")+"K";Y[H]=W;}else{P[H]=Math.Round(P[H],2);string W=P[H].ToString("N1");Y[H]=W;}}else{Y[H
]=P[H].ToString();}}else{b[H]="";Z[H]="";Y[H]="";}}F.Add(new MySprite(SpriteType.TEXT,Z[15]+"\n"+Y[15],new Vector2(126f,
195f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[15],
new Vector2(161f,160f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new
MySprite(SpriteType.TEXT,Z[14]+"\n"+Y[14],new Vector2(9f,195f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,
0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[14],new Vector2(44f,160f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,
255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[13]+"\n"+Y[13],new Vector2(-108f,195f)*R+E,null,
new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[13],new Vector2(-73f
,160f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType
.TEXT,Z[12]+"\n"+Y[12],new Vector2(-225f,195f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.
Add(new MySprite(SpriteType.TEXTURE,b[12],new Vector2(-190f,160f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),
null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[11]+"\n"+Y[11],new Vector2(126f,85f)*R+E,null,new Color(
255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[11],new Vector2(161f,50f)*R+E,
new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[10]+
"\n"+Y[10],new Vector2(9f,85f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(
SpriteType.TEXTURE,b[10],new Vector2(44f,50f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f))
;F.Add(new MySprite(SpriteType.TEXT,Z[9]+"\n"+Y[9],new Vector2(-108f,85f)*R+E,null,new Color(255,255,255,255),"DEBUG",
TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[9],new Vector2(-73f,50f)*R+E,new Vector2(70f,70f)*R,new Color(255
,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[8]+"\n"+Y[8],new Vector2(-225f,85f)*R+E
,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[8],new
Vector2(-190f,50f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(
SpriteType.TEXT,Z[7]+"\n"+Y[7],new Vector2(126f,-25f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.
Add(new MySprite(SpriteType.TEXTURE,b[7],new Vector2(161f,-60f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,
TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[6]+"\n"+Y[6],new Vector2(9f,-25f)*R+E,null,new Color(255,255,255,255),
"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[6],new Vector2(44f,-60f)*R+E,new Vector2(70f,70f)*R
,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[5]+"\n"+Y[5],new Vector2(
-108f,-25f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE
,b[5],new Vector2(-73f,-60f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(
new MySprite(SpriteType.TEXT,Z[4]+"\n"+Y[4],new Vector2(-225f,-25f)*R+E,null,new Color(255,255,255,255),"DEBUG",
TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[4],new Vector2(-190f,-60f)*R+E,new Vector2(70f,70f)*R,new Color(
255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[3]+"\n"+Y[3],new Vector2(126f,-135f)*R
+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[3],new
Vector2(161f,-170f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(
SpriteType.TEXT,Z[2]+"\n"+Y[2],new Vector2(9f,-135f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add
(new MySprite(SpriteType.TEXTURE,b[2],new Vector2(44f,-170f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,
TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[1]+"\n"+Y[1],new Vector2(-108f,-135f)*R+E,null,new Color(255,255,255,
255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(SpriteType.TEXTURE,b[1],new Vector2(-73f,-170f)*R+E,new Vector2
(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType.TEXT,Z[0]+"\n"+Y[0],
new Vector2(-225f,-135f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.6f*R));F.Add(new MySprite(
SpriteType.TEXTURE,b[0],new Vector2(-190f,-170f)*R+E,new Vector2(70f,70f)*R,new Color(255,255,255,255),null,TextAlignment.CENTER,
0f));}void V(MySpriteDrawFrame F,Vector2 E,float R=1f){F.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",new Vector2(-
1f,10f)*R+E,new Vector2(500f,226f)*R,new Color(0,128,255,255),null,TextAlignment.CENTER,0f));F.Add(new MySprite(SpriteType
.TEXT,"For Programmable Blocks compatible with \nSpace Engineers Vanilla , Programmable Blocks.",new Vector2(-242f,-101f)
*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,0.85f*R));F.Add(new MySprite(SpriteType.TEXT,
"Developed by (DrHousexx) | STEAM ID: drhousexx",new Vector2(-239f,129f)*R+E,null,new Color(255,0,0,255),"DEBUG",TextAlignment.LEFT,0.85f*R));F.Add(new MySprite(
SpriteType.TEXT,"Space Enginners DOS (SE - DOS)",new Vector2(-239f,-150f)*R+E,null,new Color(255,128,128,255),"DEBUG",
TextAlignment.LEFT,1.25f*R));F.Add(new MySprite(SpriteType.TEXT,"SE - DOS:\\>_\nInventory",new Vector2(-222f,-60f)*R+E,null,new Color
(0,0,0,255),"DEBUG",TextAlignment.LEFT,3f*R));}void U(MySpriteDrawFrame F,Vector2 E,string T,string S,string Q,float R=1f
){F.Add(new MySprite(SpriteType.TEXT,"Name block: "+T+"\nNo. of lcds connected: "+S+"\nOn Grid: "+Q+
"\n||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||\nSE - DOS:\\> _",new Vector2(-246f,-96f)*R+E,null,new Color(255,255,255,255),"DEBUG",TextAlignment.LEFT,1.3f*R));}