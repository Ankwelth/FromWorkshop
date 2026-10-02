/*
 * S E T U P
 * ---------
 * 
 * 1. Add the [XMT] tag to the custom name of any LCD.
 * 2. Add the [RCV] tag to the custom name of any LCD.
 * 3. All PBs running this code must be set to Share with Faction.
 * 
 * U S A G E
 * ---------
 * 
 * *** You can only communicate with faction members. ***
 * 
 * SENDING MESSAGES
 * 1. Click on the [XMT] tagged LCD and type your message.
 * 2. Wait a few seconds.
 * 3. When the message is sent, it will be cleared from the display and appear on the [RCV] tagged LCDs.
 * 
 * RECEIVING MESSAGES
 * 1. When messages arrive, they are automatically displayed on the [RCV] tagged LCDs.
 * 
 * CHANGING "CHANNELS"
 * 1. To send a message, you must set the broadcast channel.
 * 2. In the [XMT] tagged LCD, type "Channel: L337"
 * 3. Channel name can include any character, e.g., L337, ABC-123, !@#$
 * 4. Anyone* within antenna range, that is also set to the same channel, will receive any message you type.
 *     * Anyone in your faction, with this code running on a PB set to Share with Faction.
 * 
 * 5. There is a timeout for channels. The default is 10 minutes. Set to 0 to disable the timeout.
 * 6. If you do not send a message within this window, the channel will be disabled, preventing
 *     you from sending another message on the channel again. In order to resume transmission on 
 *     the same channel, you must set the channel again.
 * 
 * C U S T O M I Z A T I O N
 * -------------------------
 * 
 * 1. The CustomData field of the programmable block provides customization options.
 * 2. This includes the send and receive LCD tags, as well as the channel timeout.
 */
static class Ì{private const string Ë="Intergrid Messaging";private const int Ê=1;private const int É=0;private const
int È=1;private const int Ç=42;private const string Æ="Version {0}.{1}.{2}.{3}";public static string Å=string.Format(Æ,Ê,É,
È,Ç);private const string Ä="{0}\n{1}";public static string Ã=string.Format(Ä,Ë,Å);}private enum Â{Á,À,º,µ}const string ª
="Send Lcd tag:";const string z="Receive Lcd tag:";const string y="[XMT]";const string x="[RCV]";const string w=
"Channel:";const string v="Channel Timeout:";const int u=10;int t;DateTime Í;string Î;string â;List<IMyTextSurface>ã;List<
IMyTextSurface>á;IMyIntergridCommunicationSystem à;Â Z;List<MyTuple<string,string>>ß;IMyBroadcastListener Þ;string A;string Ý=
"-<|>->|<-";int Ü;const string Û="// {0} //";const string Ú="{0:HH:mm:ss} [{1}] {2} {3}\n{4}";const string Ù="->";const string Ø=
"<-";Program(){Runtime.UpdateFrequency=UpdateFrequency.Update100;Z=Â.Á;ß=new List<MyTuple<string,string>>();Î=Ò(ª);if(string
.IsNullOrWhiteSpace(Î)){Me.CustomData+=ª+" "+y+"\n";Î=y;}â=Ò(z);if(string.IsNullOrWhiteSpace(â)){Me.CustomData+=z+" "+x+
"\n";â=x;}ã=new List<IMyTextSurface>();foreach(var g in r<IMyTextPanel>(Î)){ã.Add(g);}á=new List<IMyTextSurface>();foreach(
var g in r<IMyTextPanel>(â)){á.Add(g);}IMyTextSurface Ö=Me.GetSurface(0);á.Add(Ö);Ö.ContentType=ContentType.TEXT_AND_IMAGE;
à=IGC;t=u;string Õ=Ò(v);if(string.IsNullOrWhiteSpace(Õ)){Me.CustomData+=v+" "+u;}else{int Ô;int.TryParse(Õ,out Ô);if(Ô>0)
{t=Ô;}}Í=DateTime.Now.AddMinutes(-t);Ü=0;List<IMyRadioAntenna>Ó=r<IMyRadioAntenna>(string.Empty);if(Ó.Count==0){l(á,
string.Format(Û,"No antenna on grid"));}l(á,Ì.Ã);}string Ò(string Ñ){string[]Ð=Me.CustomData.Split(new[]{"\r\n","\n"},
StringSplitOptions.None);foreach(string Ï in Ð){if(Ï.IndexOf(Ñ)!=0){continue;}string Q=Ï.Substring(Ñ.Length).Trim();if(string.
IsNullOrWhiteSpace(Q)){continue;}return Q;}return string.Empty;}List<X>r<X>(string p)where X:class,IMyTerminalBlock{List<X>V=new List<X>()
;GridTerminalSystem.GetBlocksOfType(V);for(int U=V.Count;U>0;U--){int S=U-1;X R=V[S];if(R.CustomName.IndexOf(p)==-1){V.
RemoveAt(S);}}return V;}void Main(string Q){Echo(Ì.Ã);Echo(DateTime.Now.ToLongTimeString()+" "+Ý[Ü++%Ý.Length]);Echo(s(Z));if(Z
==Â.Á&&Þ!=null&&Þ.HasPendingMessage){Z=Â.º;}switch(Z){case Â.Á:P();break;case Â.À:q();break;case Â.º:o();break;case Â.µ:G(
);break;}}void P(){foreach(var F in ã){string O=F.GetText();if(string.IsNullOrEmpty(O)){continue;}Runtime.UpdateFrequency
=UpdateFrequency.Update10;if(O.StartsWith(w,StringComparison.OrdinalIgnoreCase)){Z=Â.µ;return;}string N;if(K){string W=Me
.CubeGrid.CustomName;MyTuple<string,string>M=new MyTuple<string,string>(W,O);ß.Add(M);N=string.Format(Ú,DateTime.Now,A,W,
Ù,O);}else{N=string.Format(Û,"No channel set");Runtime.UpdateFrequency=UpdateFrequency.Update100;}l(á,N);F.WriteText(
string.Empty);Z=Â.À;}}private bool K{get{bool J=!string.IsNullOrWhiteSpace(A);bool I=t==0;bool H=DateTime.Now.Subtract(Í).
TotalMinutes<t;return J&&(I||H);}}void G(){foreach(var F in ã){string E=F.GetText();if(!E.StartsWith(w,StringComparison.
OrdinalIgnoreCase)){continue;}string[]D=E.Split('\n');string C=D[0];string B=C.Substring(w.Length).Trim();L(B);Í=DateTime.Now;if(D.Length
==1){Runtime.UpdateFrequency=UpdateFrequency.Update100;E=string.Empty;}else{E=E.Substring(C.Length+1).Trim();}F.WriteText(
E,append:false);}Z=Â.Á;}void L(string A){if(string.IsNullOrWhiteSpace(A)){return;}if(Þ!=null){IGC.
DisableBroadcastListener(Þ);}this.A=A;Þ=IGC.RegisterBroadcastListener(A);string Y=$"{DateTime.Now:HH:mm:ss} Channel set [{A}]\n";l(á,Y);}void q(
){Z=Â.Á;while(ß.Count()>0){var O=ß[0];à.SendBroadcastMessage(A,O,transmissionDistance:TransmissionDistance.AntennaRelay);
ß.RemoveAt(0);}ß.Clear();Í=DateTime.Now;Runtime.UpdateFrequency=UpdateFrequency.Update100;}void o(){Z=Â.Á;MyIGCMessage O=
Þ.AcceptMessage();MyTuple<string,string>M=(MyTuple<string,string>)O.Data;string n=M.Item1;string m=M.Item2;string N=
string.Format(Ú,DateTime.Now,A,Ø,n,m);l(á,N);}void l(List<IMyTextSurface>j,string O){foreach(IMyTextSurface h in j){string f=h
.GetText();h.WriteText(O+"\n"+f,false);}}void k(List<IMyTextSurface>j,string O){foreach(IMyTextSurface h in j){
IMyTextPanel g=h as IMyTextPanel;if(g==null){continue;}string f=g.CustomData;g.CustomData=O+"\n"+f;}}const string e="Idle";const
string d="Sending";const string c="Receiving";const string b="Tuning";const string a="unknown";string s(Â Z){switch(Z){case Â.
Á:return e;case Â.À:return d;case Â.º:return c;case Â.µ:return b;default:return a;}}