/*
 * Version 1.2.0
 * 2023-08-30
 * 
 * Configuration
 *           
 * - Add a wind turbine to your grid and give it a unique name.
 *   - The turbine needs to be on a static grid in order to function.
 *   - The amount of power the turbine is actually providing to your grid doesn't matter.
 * - You can change the names of the blocks to use in this programmable block's custom data.
 * - Enter the name of a wind turbine on your grid.
 *   - If you leave the wind turbine name empty, the script won't do anything.
 *   - If the wind turbine is missing or damaged, the script will notify you on its screen.
 * - Enter the names of any timer blocks you want to trigger.
 *   - If you leave any of the timer block names empty, those events will be ignored.   
 * - Multiple turbines with the same name can be used as fallbacks (Not recommended).
 *   - The script will choose one (probably the oldest one on the grid).
 *   - This is just a side effect of how the script works. I don't recommend doing this unless all of your turbines are in optimal
 *     positions, otherwise the baseline max power outputs will be different and the script won't work correctly.
 * 
 * Terminal Arguments (Don't include the quotation marks)
 * 
 * - "reset" - Resets all stored values and takes a new baseline reading if the turbine defined in the custom data is accessible and functional.
 *          
 * Tips
 * 
 * - Make sure the weather is clear when you first compile the script so that it can get an accurate baseline reading.
 *   If you run the script for the first time during a weather event, it will use the current values as the baseline,
 *   causing inaccurate event triggers.
 * - If the script doesn't seem to be detecting wind speed correctly or if you compiled it for the first time during a weather event,
 *   try running it with "reset" as the argument during clear weather in order to reset any stored values and take a new baseline.
 * - Modded blocks will work fine, but be careful using different types of turbines with the same name.
 *   - If a turbine is being monitored and is damaged, and the script falls back to a turbine with a different max power,
 *     a new baseline will not be measured and the events won't tigger properly.
 *   - In this case, run the program with "reset" as the argument in clear weather to take a new baseline.
 *   - Or just don't use different types of turbines with the script.
 */
string f="Weather Station";Dictionary<string,string>g=new Dictionary<string,string>(){{"Turbine","Turbine Name"},{
"LowWindStartTimer","Low Wind Event Start Timer Name"},{"LowWindPeakTimer","Low Wind Event Peak Timer Name"},{"LowWindEndTimer",
"Low Wind Event End Timer Name"},{"HighWindStartTimer","High Wind Event Start Timer Name"},{"HighWindPeakTimer","High Wind Event Peak Timer Name"},{
"HighWindEndTimer","High Wind Event End Timer Name"},{"TurbineLostTimer","Turbine Lost Timer Name"},{"TurbineFoundTimer",
"Turbine Found Timer Name"},{"TurbineChangedTimer","Turbine Changed Timer Name"},{"ProgramResetTimer","Program Reset Timer Name"},};MyIni h=new
MyIni();StringBuilder j=new StringBuilder();IMyWindTurbine k;IMyTimerBlock l;IMyTimerBlock m;IMyTimerBlock n;IMyTimerBlock o;
IMyTimerBlock p;IMyTimerBlock q;IMyTimerBlock r;IMyTimerBlock s;IMyTimerBlock t;IMyTimerBlock v;float Ä=-1.0f;float[]w;int x=0;int y=
0;bool z=false;int ª=0;long µ=-1L;bool º=false;bool À=false;IMyTextSurface Á;int Â=0;Program(){Á=Me.GetSurface(0);Á.
ContentType=ContentType.TEXT_AND_IMAGE;Runtime.UpdateFrequency=UpdateFrequency.Update100;u();C();}void Save(){Storage=string.Join(
";",Convert.ToString(Ä));}void Main(string Ã,UpdateType Å){j.Clear();Z();P();if((Å&(UpdateType.Trigger|UpdateType.Terminal)
)!=0){if(Ã.ToLower()=="reset"){j.AppendLine("RESETTING");J();C();D(h.Get(f,g["ProgramResetTimer"]).ToString(),l,ref l);X(
l);}}if((Å&UpdateType.Update100)!=0){D(h.Get(f,g["Turbine"]).ToString(),k,ref k);I();if(h.Get(f,g["Turbine"]).ToString()
!=""&&k!=null&&!k.Closed&&k.Enabled&&k.IsFunctional){if(Ä==-1.0f){O();}c();R(w,ref x,ref y,out z);ª=w[w.Length-1]>Ä?1:w[w.
Length-1]<Ä?-1:0;if(z&&y==0&&x==-1&&ª==-1){D(h.Get(f,g["LowWindStartTimer"]).ToString(),m,ref m);X(m);}if(z&&y==-1&&x>=0&&ª==-
1){D(h.Get(f,g["LowWindPeakTimer"]).ToString(),n,ref n);X(n);}if(z&&y==1&&x!=1&&ª==0){D(h.Get(f,g["LowWindEndTimer"]).
ToString(),o,ref o);X(o);}if(z&&y==0&&x==1&&ª==1){D(h.Get(f,g["HighWindStartTimer"]).ToString(),p,ref p);X(p);}if(z&&y==1&&x<=0
&&ª==1){D(h.Get(f,g["HighWindPeakTimer"]).ToString(),q,ref q);X(q);}if(z&&y==-1&&x!=-1&&ª==0){D(h.Get(f,g[
"HighWindEndTimer"]).ToString(),r,ref r);X(r);}a();}else if(h.Get(f,g["Turbine"]).ToString()==""){j.AppendLine(
"Provide a turbine name in this block's custom data");}}Echo(j.ToString());Á.WriteText(j.ToString());}void u(){P();foreach(KeyValuePair<string,string>e in g){h.Set(f,e.
Value,h.Get(f,e.Value).ToString());}Me.CustomData=h.ToString();}void P(){MyIniParseResult B;if(!h.TryParse(Me.CustomData,out
B)){throw new Exception(B.ToString());}}void C(){D(h.Get(f,g["Turbine"]).ToString(),k,ref k);I();O();}void D<E>(string F,
E G,ref E H)where E:IMyFunctionalBlock{if(G!=null&&!G.Closed&&G.Enabled&&G.IsFunctional){H=G;}else if(F!=""){H=(E)
GridTerminalSystem.GetBlockWithName(F);}}void I(){if(k!=null&&µ==-1L){µ=k.EntityId;}if(k!=null&&!k.Closed&&k.EntityId!=µ){µ=k.EntityId;º=
true;D(h.Get(f,g["TurbineChangedTimer"]).ToString(),v,ref v);X(v);}if(!À&&µ!=-1L&&(k==null||k.Closed||!k.Enabled||!k.
IsFunctional)){À=true;D(h.Get(f,g["TurbineLostTimer"]).ToString(),s,ref s);X(s);}else if(À&&k!=null&&!k.Closed&&k.Enabled&&k.
IsFunctional){À=false;D(h.Get(f,g["TurbineFoundTimer"]).ToString(),t,ref t);X(t);}if(º){j.AppendLine(
"* Monitored turbine has changed!");j.AppendLine("* Run \"reset\" during clear");j.AppendLine("* weather to measure a new baseline");j.AppendLine(
"* and clear this message");}if(À){j.AppendLine("* Monitored turbine");j.AppendLine("* is not accessible!");}}void J(){Storage="";k=null;µ=-1L;º=
false;À=false;m=null;n=null;o=null;p=null;q=null;r=null;v=null;t=null;s=null;Ä=-1f;w=K();}float[]K(){return new float[]{-1,-1
,-1,-1,-1};}Boolean L(float[]M){for(int N=0;N<M.Length;N++){if(M[N]<0f)return false;}return true;}void O(){string[]Q=
Storage.Split(';');if(Q.Length>=1&&Q[0]!=""){float.TryParse(Q[0],out Ä);}if(Ä==-1.0f&&k!=null&&!k.Closed&&k.Enabled&&k.
IsFunctional){µ=k.EntityId;Ä=(float)Math.Round(k.MaxOutput,5);}}void c(){if(w!=null){Array.Copy(w,1,w,0,w.Length-1);}if(k!=null&&!k.
Closed&&k.Enabled&&k.IsFunctional){if(w==null){w=K();}w[w.Length-1]=(float)Math.Round(k.MaxOutput,5);}else if(w!=null){w=null;
x=0;y=0;}}void R(float[]M,ref int S,ref int T,out bool U){if(L(M)){int V=0;for(int N=1;N<M.Length;N++){int W=M[N]>M[N-1]?
1:M[N]<M[N-1]?-1:0;V+=W;}U=Math.Abs(V)==M.Length-1||V==0;T=S;if(U){S=V>0?1:V<0?-1:0;}}else{S=0;U=false;}}void X(
IMyTimerBlock Y){if(Y!=null&&!Y.Closed&&Y.Enabled&&Y.IsFunctional){Y.Trigger();}}void Z(){switch(Â){case 0:j.AppendLine(
"Weather Station -");j.AppendLine("");Â=1;return;case 1:j.AppendLine("Weather Station \\");j.AppendLine("");Â=2;return;case 2:j.AppendLine(
"Weather Station |");j.AppendLine("");Â=3;return;case 3:j.AppendLine("Weather Station /");j.AppendLine("");Â=0;return;}}void a(){j.
AppendLine("Turbine: "+h.Get(f,g["Turbine"]).ToString());j.AppendLine("Baseline Max: "+(Ä*1000f)+"kW");if(w[w.Length-1]>-1){j.
AppendLine("Current Max: "+(w[w.Length-1]*1000f)+"kW");}j.AppendLine("Current Event: "+A(ª));j.AppendLine("Trending: "+b(x));}
string b(int W){switch(W){case 1:return"Up";case-1:return"Down";case 0:default:return"Flat";}}string A(int d){switch(d){case 1
:return"High Wind";case-1:return"Low Wind";case 0:default:return"Baseline";}}