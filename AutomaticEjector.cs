const string ª="[AE]";IMyTextSurface z;List<IMyTerminalBlock>y;List<IMyShipConnector>x;List<IMyTextSurface>w;
IMyConveyorSorter v;bool u=true;DateTime µ=DateTime.MinValue;DateTime r=DateTime.MinValue;int q=0;string p="";MyIni o=new MyIni();MyIni n
=new MyIni();Program(){Runtime.UpdateFrequency=UpdateFrequency.Update100;z=((IMyTextSurfaceProvider)Me).GetSurface(0);z.
ContentType=ContentType.TEXT_AND_IMAGE;if(!Me.CustomName.Contains(ª))Me.CustomName+=" "+ª;if(Me.CustomData.Replace(" ","")==""){o.
Set("Ores selection","Stone",true);o.Set("Ores selection","Ice",false);o.Set("Connectors","Connector Group (Optional)",(ª.
Replace("[","").Replace("]","")+" Connectors"));Me.CustomData=o.ToString();}else{o.TryParse(Me.CustomData);}n.TryParse(Storage)
;u=n.Get("Config","ejEnabled").ToBoolean();Ò(true);}void Save(){Storage=n.ToString();}void m(){Storage.Remove(0,Storage.
Length);}void Main(string l,UpdateType º){if(l!=""){switch(l){case"toggle":u=!u;break;case"on":u=true;break;case"off":u=false;
break;default:break;}n.Set("Config","ejEnabled",u);Save();G(false);}if(º==UpdateType.Update1||º==UpdateType.Update10||º==
UpdateType.Update100){G(false);c();Q();}Ò();}void Ò(bool Ð=false){if((DateTime.Now-µ).TotalSeconds>3||Ð){o.TryParse(Me.CustomData)
;y=h<IMyTerminalBlock>();x=Ï();w=P();p="Refreshed Grid Blocks\n";µ=DateTime.Now;v=Ì();if(v!=null)p+=
"Found designated sorter\n";}}List<IMyShipConnector>Ï(){List<IMyShipConnector>Î=new List<IMyShipConnector>();bool Í=true;try{GridTerminalSystem.
GetBlockGroupWithName(o.Get("Connectors","Connector Group (Optional)").ToString()).GetBlocksOfType<IMyShipConnector>(Î);}catch(Exception){Í=
false;}if(!Í||Î.Count==0)Î=h<IMyShipConnector>();return Î;}IMyConveyorSorter Ì(){foreach(IMyConveyorSorter Ë in h<
IMyConveyorSorter>()){if(Ë.CustomName.Contains("[AE_DES]"))return Ë;}return null;}void Ê(string É,bool È=false){string Ç=
"Automatic Ejector => "+(u?"ON":"OFF")+"\n——————————————————";foreach(IMyTextSurface M in w){StringBuilder Æ=new StringBuilder(Ç+"\n"+É);
IMyTextSurface Å=Me.GetSurface(0);float Ä=M.MeasureStringInPixels(Æ,M.Font,M.FontSize).Y;bool Ã=Ä>M.SurfaceSize.Y;M.WriteText(Ç);M.
WriteText((È||!Ã)?("\n"+É):Â(É+"\n"),true);M.ContentType=ContentType.TEXT_AND_IMAGE;}Echo(Ç+"\n"+É);}string Â(string Á){string[]À
=Á.Split('\n');string Ñ="";string k="";if(q>=À.Length)q=0;for(int j=0;j<À.Length;j++){if(j<q)Ñ+=À[j]+"\n";else k+=À[j]+
"\n";}return"\n"+k+Ñ;}void Q(){Ê(p);}List<IMyTextSurface>P(){List<IMyTextSurface>O=new List<IMyTextSurface>();string N=ª.
Substring(0,ª.Length-1).ToLower();foreach(IMyTextSurfaceProvider M in h<IMyTextSurfaceProvider>()){string L=(M as
IMyTerminalBlock).CustomName.ToLower();if(L.Contains(N.ToLower())){IMyTextSurface K=null;if(M.SurfaceCount==1)K=M.GetSurface(0);else{
foreach(string R in L.Substring(L.IndexOf(N)).Split(' ')){if(R.Contains(N.ToLower())&&R.Contains(":")){string J=R.Split(':')[1]
;int H=Convert.ToInt32(J.Substring(0,J.Length-1));if(H<0)H=0;else if(H>M.SurfaceCount)H=M.SurfaceCount;K=M.GetSurface(H);
}}}if(K!=null){O.Add(K);}}}O.Add(z);return O;}void G(bool F){foreach(IMyShipConnector E in x){if(F){List<MyInventoryItem>
D=new List<MyInventoryItem>();IMyInventory C=E.GetInventory();C.GetItems(D);bool B=true;foreach(MyInventoryItem I in D){
string A=I.Type.SubtypeId;if(!o.Get("Ores selection",S(A)).ToBoolean()){B=false;foreach(IMyTerminalBlock T in y){if(!x.
Contains(T)&&T.HasInventory){C.TransferItemTo(T.GetInventory(),I);if(!D.Contains(I))B=true;}}}p="\n    "+S(A)+": "+o.Get(
"Ores selection",S(A)).ToBoolean().ToString()+"\n\n"+p;}if(!B){p=E.CustomName+"\n    Ejection prevented\n\n"+p;}else{E.ThrowOut=(F&&(v==
null||v.Enabled));}}else E.ThrowOut=false;}if(F&&v!=null&&!v.Enabled)p=
"Designated sorter turned off.\n    Ejection paused\n\n"+p;}void G(IMyShipConnector E,bool F){E.ThrowOut=F;}List<g>h<g>()where g:class{List<g>f=new List<g>();List<g>e=new List<
g>();GridTerminalSystem.GetBlocksOfType<g>(f);foreach(g d in f){if(Me.CubeGrid==(d as IMyTerminalBlock).CubeGrid&&!(d as
IMyTerminalBlock).CustomName.Contains("[AE_IGN]"))e.Add(d);}return e;}void c(){int a=0;bool Z=false;if(u&&(DateTime.Now-r).TotalSeconds>
0.3){r=DateTime.Now;foreach(IMyTerminalBlock Y in y){if(Y.HasInventory){List<MyInventoryItem>X=new List<MyInventoryItem>();
IMyInventory C=Y.GetInventory();C.GetItems(X);string W="";bool V=false;p="";foreach(MyInventoryItem I in X){string A=I.Type.
SubtypeId;if(o.Get("Ores selection",S(A)).ToBoolean()){if(!x.Contains(Y)){W+="\n    "+A;for(a=0;a<x.Count;a++){C.TransferItemTo(x
[a].GetInventory(),I);}V=true;}Z=true;}}W+="\n";if(V)p+=(Y.CustomName+W);}}G(Z&&u);}}static string S(string U){if(String.
IsNullOrEmpty(U))throw new ArgumentException("ARGH!");return U.First().ToString().ToUpper()+String.Join("",U.Skip(1));}