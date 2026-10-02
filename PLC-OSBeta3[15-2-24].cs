string title="P.L.C.-O.S. Version beta 3 date:  15-2-23 ",dev="By Twotwinbrothers.";

	//write "commands" in customdata of this programeble block to show you a list of commands
	//write "settings" in customdata of this programeble block to show all the setting of this script

	 // instruction count cut nr, try to avoid script exception "Script To Complex"
	 // 50000 is game max.
		int cut=35000;

	 // controller tags, if no tag is specified it select one by it self.
		//string cockpit="";
		//string remote="",

	//Translation strings

		string Sl=" Sleep mode ",

		B1="boot",
		B2="Booting",
		B3="Collecting Blocks",
		B4="almost Done",
	
		E1="Line",
		E2="Missing Value in Command",
		E3="Unknown Command",
		E4="cur. instruction",
		E5=" count to hieght",

	 //IGC 
		C1="Channel list",
		C2="Searching",
		C3="Sending",
		C4="Data",
		C5="Action",
		C6="To",
		C7="message",
		C8="From",

		C10="Received IGC public Message",
		C11=",Public message delivered: ",
		C12="Received IGC request, sending website to: ",
		C13="Website received ",
		C14="Action Request received: ",
		C15="Received IGC private Message",
		C16=",Private message delivered: ",
		//C11 & C16 must contain C17
		C17="message delivered",
		C18="Tag delivered",

		C20="<send Q>",
		C21="<send E>",

	 //Status
		S1="No value found",
		S2="Assembling",
		S3="DisAssembling",
		S4="Tag not found",
		S5="Itemname not found",
		S6="Group not found",
		
		S10="On",
		S11="Off",
		S12="Shooting",
		S13="Active",
		S14="Charging",
		S15="Not Charging",
		S16="Attached",
		S17="Leaking",
		//S17="Autolock On",

	 //arguments can be change here.

		A1="add",
		A2="remove",
		A3=" all",
		A4=" group",

		A5="itemname",
		A6="pittag",
		A7="remtag",

		A8="mouse",
		A9="mouse mode",
		A10="park switch",

		A11="setput",
		A12="sleep",
		A13="update",

		A14="sendfrom",
		A15="sendto",
		A16="next",
		A17="prev",
		A18="ping",
		A19="cl",
		A20="www",
		A21="list",

		A25="lcdtag",
		A26="res",
		A27="link",
		A28="input",
		A29="output",
		A30="object",

		A31="brake",
		A32="set",
		A33="cansel",

		A34="stop",
		A35="play",
		A36="+",
		A37="-",
		A38="del",

		A40="mode",
		A41="rep",
		A42="coop",

		A50=" W ",
		A51=" S ",
		A52=" D ",
		A53=" A ",
		A54=" C ",
		A55=" F ",
		A56="WSAD",

		A57=" cl ",
		A58="<auto>",

	 //type name can be change here.

		чa="action",
		чb="prop",
		чc="bool",
		чd="float",
		чe="color",
		чf="term",
		чg="mem",
		чh="bat",
		чi="pow",
		чj="tank",
		чk="cargo",
		чl="inv",
		чm="cockpit",
		чn="cont",
		чo="conn",
		чp="land",
		чq="gun",
		чr="rotor",
		чs="piston",
		чt="wheel",
		чu="door",
		чv="vent",
		чw="trust",
		чx="sens",
		чy="proj",
		чz="sound",
		ч1="prod",
		ч2="ass",
		ч3="sort",
		ч4="jump",
		ч5="com",
		ч6="speed",
		ч7="alt",
		ч8="hor",
		ч9="time",
		ч10="xtim",
		ч11="text",
		ч12="custom",
		ч13="data",
		ч14="pb",
		ч15="cruise",

	 //sup type(<>)

		h4="<#>",
		h5="<%>",
		h6="<R>",
		h7="<G>",
		h8="<B>",
		h9="<A>",
		h0="<N>",
		hA="<level>",

		b1="<cur>",
		b2="<max>",
		b3="<list>",

		b4="<input>",
		b5="<cur i>",
		b6="<max i>",
		b7="<low>",

		c1="<iscon>",
		c2="<R>",
		c3="<P>",

		r1="<angle>",
		r2="<torq>",
		r3="<break>",
		r4="<dis>",
		r5="<vel>",
		r6="<low>",
		r7="<up>",
		r8="<pos>",
		
		w1="<angle>",
		w2="<power>",
		w3="<strength>",
		w4="<height>",
		w5="<friction>",
		w6="<speed>",
		w7="<prop>",
		w8="<steer>",

		s1="<name>",
		s2="<type>",
		s3="<rel>",
		s4="<X>",
		s5="<Y>",
		s6="<Z>",
		s7="<L>",
		s8="<W>",
		s9="<H>",
		s0="<time>",

		p1="<total>",
		p2="<rem>",
		p3="<build>",
		p4="<offset>",
		p5="<rotation>",

		m1="<vol>",
		m2="<range>",
		m3="<loop>",
		m4="<music>",

		a1="<coop>",
		a2="<rep>",
		a3="<mode>",

		j1="<input>",
		j2="<time>",

		i1="<from>",
		i2="<to>",

		i3="<item>",
		i4="<Iitem>",
		i5="<Oitem>",
		i6="<Qitem>";

	//Colors.

		Color Ƈ_Arrow=new Color(150,150,150),
		Ƈ_Back=new Color(100,70,40),
		Ƈ_ObjBack=new Color(100,70,40),

		Ƈ_Arrowon=new Color(255,255,255),
		Ƈ_Arrowoff=new Color(150,150,150),
		Ƈ_Link=new Color(100,70,40),

		Ƈ_on=new Color(75,150,75),
		Ƈ_off=new Color(75,75,75),

		Ƈ_gauges=new Color(255,255,255),
		Ƈ_gaugeback=new Color(0,0,0),
		Ƈ_ƥointer=new Color(255,75,75),
		
		Ƈ_white=new Color(150,150,150),
		Ƈ_grey=new Color(75,75,75),
		Ƈ_green=new Color(75,150,75),
		Ƈ_red=new Color(150,75,75),
		Ƈ_yellow=new Color(150,150,5),
		Ƈ_orange=new Color(255,150,0),
		Ƈ_black=new Color(0,0,0),
		Ƈ_dblue=new Color(0,0,150),
		Ƈ_lblue=new Color(0,150,150);

//don't change anything below

	 //lists
		IMyShipController ȣ,ȣi,ȣC,ȣR;
		List<IMyTerminalBlock>Ƀ=new List<IMyTerminalBlock>();

		List<IMyTerminalBlock>Т=new List<IMyTerminalBlock>();
		List<IMyTerminalBlock>Т1=new List<IMyTerminalBlock>();
		List<IMyBatteryBlock>в=new List<IMyBatteryBlock>();
		List<IMyPowerProducer>Ƿ=new List<IMyPowerProducer>();
		List<IMyGasTank>Ф=new List<IMyGasTank>();
		List<IMyCockpit>ҏ=new List<IMyCockpit>();
		List<IMyShipController>Ɔ=new List<IMyShipController>();
		List<IMyShipConnector>ĉ=new List<IMyShipConnector>();
		List<IMyLandingGear>ǵ=new List<IMyLandingGear>();
		List<IMyUserControllableGun>ϙ=new List<IMyUserControllableGun>();
		List<IMyMotorStator>Ŕ=new List<IMyMotorStator>();
		List<IMyPistonBase>ϥ=new List<IMyPistonBase>();
		List<IMyMotorSuspension>ώ=new List<IMyMotorSuspension>();
		List<IMyDoor>Ð=new List<IMyDoor>();
		List<IMyAirVent>α=new List<IMyAirVent>();
		List<IMyThrust>τ=new List<IMyThrust>();
		List<IMySensorBlock>Ș=new List<IMySensorBlock>();
		List<IMyConveyorSorter>һ=new List<IMyConveyorSorter>();
		List<IMyJumpDrive>Ď=new List<IMyJumpDrive>();
		List<IMyProjector>φ=new List<IMyProjector>();
		List<IMyLightingBlock>Ŀ=new List<IMyLightingBlock>();
		List<IMySoundBlock>ʘ=new List<IMySoundBlock>();
		List<IMyProductionBlock>þ=new List<IMyProductionBlock>();
		List<IMyAssembler>Ă=new List<IMyAssembler>();
		List<IMyTimerBlock>Ț=new List<IMyTimerBlock>();
		List<IMyProgrammableBlock>ш=new List<IMyProgrammableBlock>();
	
		List<string>music=new List<string>();
		List<MyInventoryItemFilter>Ȉs=new List<MyInventoryItemFilter>();
	 //

		string ϐ="BlueprintDefinition",CD,SD,MD,ЩD,ψD,Ƥ="boot++ 0\n ",Щlcd=" ",cockpit="",remote="",
		Ə,Э,ǂ,name,ϡ=" ",nϡ,ƧT="none set",Ϟ,Ø4,Ø5="",Ø=" ",ζ,isPRun,lcdl,dir =" ",
		ғ="",ҭ="",Ѩ,λѠ,Ѡ="",Œ="Everyone",ˠ="Everyone",ś="Everyone",Ѽ,Ѡχ,ѠӋ,ѠЋ,ѠЬ,ǈ,PL="\n"/*,cтт="none set"*/;
		int Ĉ=0,time=10,slĈ=0,ȅ=0,Ԕ=0,sltime=0;double ЩSM=0.5;
		bool slmode=false,ЩЩ=true,QEǷ=true,ʗQ=false,ʗE=false,Ϛ=false,HsЩ=false,K=false;
		float deg,Aχ=128,AӋ=100,SӋ=0/*,ѠŞ=0*/;

		IMyBroadcastListener β1,β2;
		IEnumerator<bool> state;
		Color Ƈ_m=new Color(0,0,0),Ƈ_l=new Color(0,0,0);

	public IEnumerator<bool> RunStuffOverTime(){

		if(Me.CustomData.Contains("yield")){

			var ЩCC=Me.CustomData.Split(new string[] {"yield"},StringSplitOptions.None);
			ArrayList list=new ArrayList(ЩCC);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(ЩCC[i]);

			if(i==0)
			SD = ψ;

			if(!ψ.Contains(Щlcd)&&i!=0)
			MD = ψ;

			if(ψ.Contains(Щlcd))
			ЩD = ψ;

			CD = SD+ЩD+MD;

			Ø4="Me. part "+i;
			ψD=Me.CustomData;
			Display();
			search(" ");
			Me.CustomData=ψD;
			yield return true;
			}}
		else{
			if(Me.CustomData.Contains(Щlcd)){
			ЩD = Me.CustomData;
			CD = SD+ЩD;
			}
			else CD = SD+ЩD+Me.CustomData;
			Ø4="Me.";
			ψD=Me.CustomData;
			Display();
			search(" ");
			Me.CustomData=ψD;
			yield return true;
			}
		if(!Ƥ.Contains(B1)){
		foreach(var β in Т){

			if(C(β.CustomName,ƧT+"[D]")){

			if(β.CustomData.Contains("yield")){
			var ЩCC=β.CustomData.Split(new string[] {"yield"},StringSplitOptions.None);
			ArrayList list=new ArrayList(ЩCC);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(ЩCC[i]);

			if(!ψ.Contains(Щlcd))
			MD = ψ;

			if(ψ.Contains(Щlcd))
			ЩD = ψ;

			CD = SD+ЩD+MD;

			Ø4=β.CustomName+". part "+i;
			ψD=β.CustomData;
			Display();
			search(" ");
			β.CustomData=ψD;
			yield return true;
			}}
		else{

			if(β.CustomData.Contains(Щlcd)){
			ЩD = β.CustomData;
			CD = SD+ЩD;
			}
			else CD = SD+ЩD+β.CustomData;

			Ø4=β.CustomName+".";
			//R(β.CustomData)
			ψD=β.CustomData;
			Display();
			search(" ");
			β.CustomData=ψD;
			//β.CustomData=R;
			yield return true;
			}

			}
			}
		}
		Ø4="Done";

		}
	/*string R(string d){
			ψD=d;
			Display();
			search(" ");
			d=ψD;

		}*/

	void Display(){
		GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null,Dβ);
		}

	bool Dβ(IMyTerminalBlock lβ){
		var lcd=lβ as IMyTextSurfaceProvider;
		if(lcd==null || lcd.SurfaceCount==0){
			return false;
			}
		if(lβ.CustomName.Contains(ϡ)){
			if(lβ.CustomName.Contains(ƧT+"[-") ){
			for (var index=0;index < lcd.SurfaceCount;index++){
			var β=lcd.GetSurface(index);
			β.ContentType=ContentType.NONE;
			}}
			if(lβ.CustomName.Contains(ƧT+"[*]")){
			for (var index=0;index < lcd.SurfaceCount;index++){
			var β=lcd.GetSurface(index);
			β.ContentType=ContentType.TEXT_AND_IMAGE;
			β.ClearImagesFromSelection();
			β.FontSize=3.0f;
			β.Alignment=TextAlignment.CENTER;
			β.WriteText($"{index}");
			}}
		}
		int nsur=0;
		float XX=0;
		float XY=0;
		float Nχ=0;
		float NӋ=0;

		var ЩCC=CD.Split('\n');
		ArrayList list=new ArrayList(ЩCC);
		for(int i=0;i < list.Count;i++){
		string ψ=Convert.ToString(ЩCC[i]);

	 if(!ψ.Contains("//")){
		if(ψ.Contains(",,")){
		if(!ǂ.Contains(ψ))
		ǂ+="\n"+E1+" "+(i+1).ToString()+"="+E2+"\n="+ψ+"\n";
		}
	 else if(!ψ.Contains(",,")){
	 if(!ψ.Contains(A25)){
		if(ψ.Contains(ƧT)){

		string stag=Թ(ref ψ),
		tag=Թ(ref ψ),
		sur=Թ(ref ψ),
		Ƨ=Թ(ref ψ),
		nXX=Թ(ref ψ),
		nXY=Թ(ref ψ),
		nNχ=Թ(ref ψ),
		nNӋ=Թ(ref ψ);
		
		//tag,sur,Ƨ,XX,XY,Nχ,NӋ
		if(!sur.Contains("x"))
		nsur=int.Parse(sur);
		if(!nXX.Contains("XX"))
		XX=float.Parse(nXX);
		if(!nXY.Contains("XY"))
		XY=float.Parse(nXY);
		if(!nNχ.Contains("Nχ"))
		Nχ=float.Parse(nNχ);
		if(!nNӋ.Contains("NӋ"))
		NӋ=float.Parse(nNӋ);
		
		if(lβ.CustomName.Contains(ϡ)){
		if(lβ.CustomName.Contains(ƧT+tag))
		LCD((lcd.GetSurface(nsur)),Ƨ,XX,XY,Nχ,NӋ);
		if (Щlcd==" ")Щlcd=Ƨ;
		}}}}}
		//Echo("Customdata line "+(i).ToString()+" \n");

		}

		return false;
	 }
//
	void LCD(IMyTextSurface β,string Ƨ,float XX,float XY,float Nχ,float NӋ ){

		β.ContentType=ContentType.SCRIPT;
		β.Script=title;
		β.BackgroundAlpha=0;
		Ƈ_Back=β.ScriptBackgroundColor;
		Ƈ_ObjBack=β.ScriptBackgroundColor;

	// auto calculate 

		if( XX==0)
		XX=β.SurfaceSize.X;
		if( XY==0)
		XY=β.SurfaceSize.Y;

		if( Nχ==0)
		Nχ=(β.TextureSize.X-XX)/2;
		if( NӋ==0)
		NӋ=(β.TextureSize.Y-XY)/2;

		SӋ=XY;
		β.WriteText(CD);

	using (var ƪ=β.DrawFrame()){

			float χ=0,Ӌ=0,Ћ=0,Ь=0,π=0,AX=0,AY=0;
			int CR=0,CG=0,CB=0;
			
			var ЩCC=CD.Split('\n');
			ArrayList list=new ArrayList(ЩCC);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(ЩCC[i]);

		if(Ĉ==1)
		Object(ƪ,XX/2+Nχ,XY/2+NӋ," ",XY,XX,0," ","0.6",Ƈ_white," ");
		
		
		if(Runtime.CurrentInstructionCount<=cut){
		if(!ψ.Contains("//")){
		if(ψ.Contains(",,")){
			if(!ǂ.Contains(ψ))
			ǂ+="\n"+E1+" "+(i+1).ToString()+"="+E2+"\n="+ψ+"\n";
			}
		else if(!ψ.Contains(",,")){
		if(ψ.Contains("Ʊ"))
			ψD=ψD.Replace("\n"+ψ,"");

		else if(ψ.Contains("res,"+Ƨ)){

			lcdl=("//Calculated or set resolutie of "+Ƨ+" =X "+XX+",Y "+XY+",-X "+Nχ+",-Y "+NӋ);
			ψD=ψD.Replace("res,"+Ƨ,lcdl);
			}
		else if(ψ.Contains("detect")){
			string time=":"+DateTime.Now.ToString("mm");

			if(!ψ.Contains(time))
			ψD=ψD.Replace("\n"+ψ,"");
			}

		else if(ψ.Contains(ƧT));
			else if(ψ.Contains("commands"));
			else if(ψ.Contains(A5+","));
			else if(ψ.Contains(A6+","));
			else if(ψ.Contains(A7+","));
			else if(ψ.Contains(A25+","));
			else if(ψ.Contains(A8));
			else if(ψ.Contains(A9+","));
			else if(ψ.Contains(A10+","));
			else if(ψ.Contains(A11+","));
			else if(ψ.Contains(A12+","));
			else if(ψ.Contains(A13+","));
			else if(ψ.Contains(A1));
			else if(ψ.Contains(A2));
			else if(ψ.Contains(A14+","));
			else if(ψ.Contains(A15+","));
			else if(ψ.Contains(A56+","));
	else if(ψ.Contains(C20)|ψ.Contains(C21)|ψ.Contains(A29)|ψ.Contains(A28)|ψ.Contains(A27)|ψ.Contains(A30)){
		λѠ=ψ;
		string Դ=Թ(ref ψ),
		Ƨ1=Թ(ref ψ),
		Ƥ1=Թ(ref ψ),
		posx=Թ(ref ψ),
		posy=Թ(ref ψ),
		ה=Թ(ref ψ),
		nhi=Թ(ref ψ),
		nbr=Թ(ref ψ),
		npi=Թ(ref ψ);
		if(Ƨ1.Contains("web")){
		Ѡχ=posx;
		ѠӋ=posy;
		ѠЋ=nhi;
		ѠЬ=nbr;
		}
		//adjust text size to screen size
		/*if(ψ.Contains(A20)){
		if(XX<125)
		ѠŞ=2;
		else if(XX<256)
		ѠŞ=1,5;

		}*/

		if(posx.Contains("|")){
			posx=posx.Replace("|",",");
			string n1=Թ(ref posx);
			string n2=Թ(ref posx);
			float nn1=float.Parse(n1);
			float nn2=float.Parse(n2);
			χ=(XX/nn1*nn2);
			}
			else if(!posx.Contains("x"))
			χ=float.Parse(posx);

		if(posy.Contains("|")){
			posy=posy.Replace("|",",");
			string n1=Թ(ref posy);
			string n2=Թ(ref posy);
			float nn1=float.Parse(n1);
			float nn2=float.Parse(n2);
			Ӌ=(XY/nn1*nn2);
			}
			else if(!posy.Contains("x"))
			Ӌ=float.Parse(posy);

		if(nhi.Contains("|")){
			nhi=nhi.Replace("|",",");
			string n1=Թ(ref nhi);
			string n2=Թ(ref nhi);
			float nn1=float.Parse(n1);
			float nn2=float.Parse(n2);
			Ћ=(XY/nn1*nn2);
			}
			else if(!nhi.Contains("x"))
			Ћ=float.Parse(nhi);

		if(nbr.Contains("|")){
			nbr=nbr.Replace("|",",");
			string n1=Թ(ref nbr);
			string n2=Թ(ref nbr);
			float nn1=float.Parse(n1);
			float nn2=float.Parse(n2);
			Ь=(XX/nn1*nn2);
			}
			else if(!nbr.Contains("x"))
			Ь=float.Parse(nbr);

		if(!npi.Contains("x"))
		π=float.Parse(npi);

		string ҕ1=Թ(ref ψ),
		ҕ2=Թ(ref ψ),
		ҕ3=Թ(ref ψ),
		ҕ4=Թ(ref ψ),
		ҕ5=Թ(ref ψ),
		ҕ6=Թ(ref ψ),
		ҕ7=Թ(ref ψ),
		ҕ8=Թ(ref ψ);

	if(λѠ.Contains(C20)|λѠ.Contains(C21)){

		string т=ҕ1,
		Ş=ҕ2,
		ή=ҕ3,
		чQ=ҕ4,
		λQ=ҕ5,
		чE=ҕ6,
		λE=ҕ7;

		if(Щlcd==Ƨ1){
		if(AӋ>(NӋ+Ӌ-Ћ/2) && AӋ<(NӋ+Ӌ+Ћ/2) && Aχ>(Nχ+χ-Ь/2) && Aχ<(Nχ+χ+Ь/2)){
			Ƈ_m=Ƈ_Arrowon;

			if(ʗQ==true){
			λѠ=λѠ.Replace(C20,"CQ");
			λѠ=λѠ.Replace(C21," ");
			com("ӆ,"+λѠ);
			ʗQ=false;
			}
			if(ʗE==true){
			λѠ=λѠ.Replace(C21,"CE");
			λѠ=λѠ.Replace(C20," ");
			com("ӆ,"+λѠ);
			ʗE=false;
			}
		}
		else
		Ƈ_m=Ƈ_Arrowoff;
		}
		else
		Ƈ_m=Ƈ_Arrowoff;

		if(Ƨ==Ƨ1&&(Logic(Ƥ1)==true))
		Object(ƪ,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,Ƈ_m,"Ϟ");
		}
	else if(Դ.Contains(A29)){
		
		string т=ҕ1,
		Ş=ҕ2,
		ή=ҕ3,
		чQ=ҕ4,
		λQ=ҕ5,
		чE=ҕ6,
		λE=ҕ7;

		if(C(чQ,"CQ")|C(чE,"CE"))
		output(ƪ,Ƨ1,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,ή,чQ,λQ,чE,λE," ");
		else if(Ƨ==Ƨ1&&(Logic(Ƥ1)==true))
		output(ƪ,Ƨ1,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,ή,чQ,λQ,чE,λE,"1");
		else if(Ƨ==Ƨ1&&(Logic(Ƥ1)==false))
		output(ƪ,Ƨ1,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,ή,чQ,λQ,чE,λE,"0");
		}

	else if(Դ.Contains(A28)){

		string т=ҕ1,
		Ş=ҕ2,
		ή=ҕ3,
		ч=ҕ4;
		
		if(Ƨ==Ƨ1&&(Logic(Ƥ1)==true))
		input(ƪ,Ƨ1,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,ή,ч);
		if(Ƨ1.Contains("conv")&&(Logic(Ƥ1)==true))
		input(ƪ,Ƨ1,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,ή,ч);
		if(Ƨ1.Contains("conv")&&(Logic(Ƥ1)==false))
		input(ƪ,Ƨ1+"Ʊ",χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,ή,ч);
		}
	else if(Դ.Contains(A27)){

		string ms=ҕ1,
		mx=ҕ2,
		my=ҕ3;

		if(!mx.Contains("x")){
		AX=float.Parse(mx);
		AX=AX+Nχ;
		}
		if(!my.Contains("x")){
		AY=float.Parse(my);
		AY=AY+NӋ;
		}

		if(Ƨ==Ƨ1&&Ƥ.Contains(Ƥ1))
		Link(ƪ,Ƨ1,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,ms,AX,AY);
		}
	else if(Դ.Contains(A30)){

		string т=ҕ1,
		Ş=ҕ2,
		colR=ҕ3,
		colG=ҕ4,
		colB=ҕ5,
		Status=ҕ6;

		if(!colR.Contains("x")){
		CR=int.Parse(colR);
		CG=int.Parse(colG);
		CB=int.Parse(colB);
		Ƈ_l=new Color(CR,CG,CB);
		}
		else 
		Ƈ_l=Ƈ_white;

		if(Status.Contains("bli")){
			if(Ĉ !=1);
			else
			Ƈ_l=Ƈ_grey;
			}

		if(Ƨ==Ƨ1&&(Logic(Ƥ1)==true))
		Object(ƪ,χ+Nχ,Ӌ+NӋ,ה,Ћ,Ь,π,т,Ş,Ƈ_l,Status);
		}
		}

	else{

		if(!ǂ.Contains(ψ))
			ǂ+="\n"+E1+" "+(i+1).ToString()+" = "+E3+" \n = "+ψ+"\n";
		}
		}}
		Ø5="";
		}
		else if(!Ø5.Contains(E5))
			Ø5="\n"+E1+" "+(i+1).ToString()+" = "+E4+E5+" = "+Runtime.CurrentInstructionCount;
		}

	//DRAW LOG
		if(C(Ƨ,"log")){
		Object(ƪ,XX/2+Nχ,XY/2+NӋ,"",XY,XX,0,Ø,"0.6",Ƈ_white,"");
		β.WriteText(Ø);
		}

	//DRAW SLEEPMODE 
		else if(slmode==true){
		Object(ƪ,XX/2+Nχ,XY/2+NӋ,"SquareFilled",XY,XX,0,"","0.7",Ƈ_white,"");
		if(C(Ƨ,Щlcd))
		Object(ƪ,XX/2+Nχ,XY/2+NӋ,"SquareFilled",20,120,0,Sl,"0.7",Ƈ_white,"");
		}
	//DRAW BOOT
		else if(C(Ƥ,B1)){
		Object(ƪ,XX/2+Nχ,XY/2+NӋ,"SquareFilled",XY,XX,0,"","0.7",Ƈ_white,"");
		Object(ƪ,XX/2+Nχ,XY/2+NӋ,"bar|0|3",120,20,90,"#####"+name,"0.5",Ƈ_white,B1+"++ ");
		if(C(Ƥ,B1+"++ 1"))
		Object(ƪ,XX/2+Nχ,XY/2+NӋ," ",120,20,90,"#"+B2,"0.7",Ƈ_white," ");
		if(C(Ƥ,B1+"++ 2"))
		Object(ƪ,XX/2+Nχ,XY/2+NӋ," ",120,20,90,"#"+B3,"0.7",Ƈ_white," ");
		if(C(Ƥ,B1+"++ 3"))
		Object(ƪ,XX/2+Nχ,XY/2+NӋ," ",120,20,90,"#"+B4,"0.7",Ƈ_white," ");
		}
	//DRAW SCREEN NAME
		else if(C(Ƨ,A26)){
		Object(ƪ,XX/2+Nχ,XY/2+NӋ,"SquareFilled",XY,XX,0,"#"+Ƨ+"#Resolutie = X:"+XX+" x Y:"+XY+"#Offset = -X:"+Nχ+" x -Y:"+NӋ,"0.6",Ƈ_white,"");
		}
	//DRAW MOUSE 
		if(Щlcd==Ƨ){
		if(ЩЩ){
			Draw_Mouse_Arrow(ƪ,Aχ,AӋ,Ƈ_Arrow);
		}}
	

	}	//END DRAW
	//ʗQ
		if(ЩЩ==true){
		if(ȣ.RollIndicator<-0.5){
		if(QEǷ){
		ʗQ=true;
		QEǷ=false;
		}
		}
	//ʗE
		else if(ȣ.RollIndicator>0.5){
		if(QEǷ){
		ʗE=true;
		QEǷ=false;
		}
		}
		else{
		QEǷ=true;
		}
		}
	// WSADFC
		if(K==true){
		if(ȣ.MoveIndicator.Z<-0.5&&!Ƥ.Contains(A50))
		Ƥ+=" W \n";

		if(ȣ.MoveIndicator.Z>0.5&&!Ƥ.Contains(A51))
		Ƥ+=" S \n";

		if(ȣ.MoveIndicator.X>0.5&&!Ƥ.Contains(A52))
		Ƥ+=" D \n";

		if(ȣ.MoveIndicator.X<-0.5&&!Ƥ.Contains(A53))
		Ƥ+=" A \n";

		if(ȣ.MoveIndicator.Y>0.5&&!Ƥ.Contains(A54))
		Ƥ+=" F \n";

		if(ȣ.MoveIndicator.Y<-0.5&&!Ƥ.Contains(A55))
		Ƥ+=" C \n";
		}
	//Switch Mode
		if(HsЩ==true ){
		if(ȣ.GetValueBool("Park")==true){
		if(ЩЩ==false){
		ЩЩ=true;
		}
		}

		if(ȣ.GetValueBool("Park")==false){
		if(ЩЩ==true){
		ЩЩ=false;
		}
		}
		}
	//Switch Gyro
		if(ЩЩ==true)
		ȣ.SetValueBool("ControlGyros",false);
		if(ЩЩ==false)
		ȣ.SetValueBool("ControlGyros",true);


	//Arrow Position
		if(Щlcd==Ƨ){
		if(ЩЩ){
		Ƈ_Arrow=new Color(255,255,255);
		Aχ+=(float)(0.6*APS);
		AӋ+=(float)(-0.6*AES);

		if(Aχ<Nχ){Aχ=Nχ;}
		if(Aχ>XX+Nχ){Aχ=XX+Nχ;}
		if(AӋ<NӋ){AӋ=NӋ;}
		if(AӋ>XY+NӋ){AӋ=XY+NӋ;}
		}}
	}// END LCD
// 
	void output(MySpriteDrawFrame ƪ,string Ƨ,float χ,float Ӌ,string ה,float Ћ,float Ь,float π,string т,string Ş,string ή,string чQ,string λQ,string чE,string λE,string LO ){
			string ч="",λ="";

		if(C(чQ,"CQ")){
			ψD=ψD.Replace("CQ","Ʊ");
			ч=чQ;
			λ=λQ;
			}
		else if(C(чE,"CE")){
			ψD=ψD.Replace("CE","Ʊ");
			ч=чE;
			λ=λE;
			}

		else if(C(чQ,A58)&&LO=="1"){
			ч=чQ;
			λ=λQ;
			}
		else if(C(чQ,A58)&&LO=="0"){
			ч=чE;
			λ=λE;
			}

		if(Щlcd==Ƨ&&LO=="1"){
			if(AӋ>(Ӌ-Ћ/2) && AӋ<(Ӌ+Ћ/2) && Aχ>(χ-Ь/2) && Aχ<(χ+Ь/2)){
				Ƈ_m=Ƈ_Arrowon;

			if(ʗQ==true){
				ч=чQ;
				λ=λQ;
				ʗQ=false;
				}
			else if(ʗE==true){
				ч=чE;
				λ=λE;
				ʗE=false;
				}
			else if(чE==чQ){
				ч=чQ;
				λ=λQ;
				}
			}
			else
			Ƈ_m=Ƈ_Arrowoff;
			}
			else
			Ƈ_m=Ƈ_Arrowoff;
		
		if(C(ч,чd)){
			if(C(λ,"|")){
			λ=λ.Replace("|",",");
			string ҕ1=Թ(ref λ),ҕ2=Թ(ref λ),ҕ3=Թ(ref λ);

			float ҕ4=float.Parse(ҕ3),s=0,m=0,ҕ6=0;
			if (ҕ2!=" "&&!ҕ2.Contains(чg)&&ҕ2!="+"&&ҕ2!="-")
			ҕ6=float.Parse(ҕ2);

			bool c=ҕ2.Any(char.IsDigit);
			ҕ2=ҕ2.Replace(чg,"");

			if(C(Ƥ,ҕ2)&&c==false&&ҕ2.Length>=2){

				var Page=Ƥ.Split('\n');
				ArrayList list=new ArrayList(Page);
				for(int i=0;i < list.Count;i++){
				string ψ=Convert.ToString(Page[i]);

				if(C(ψ,ҕ2)){
				ψ=ψ.Replace(ҕ2,"");
				ҕ6=float.Parse(ψ);
				}
				}
			}

			foreach(var β in Т){

			if(C(β.CustomName,ή)){
			float ҕ5=β.GetValue<float>(ҕ1);

			if (ҕ2.Contains("+"))
			β.SetValue<float>(ҕ1,(ҕ5+ҕ4));
			else if (ҕ2.Contains("-"))
			β.SetValue<float>(ҕ1,(ҕ5-ҕ4));
			else if (ҕ2==" ")
			β.SetValue<float>(ҕ1,ҕ4);
			else if(C(Ƥ,ҕ2)&&c==false&&ҕ2.Length>=2)
			β.SetValue<float>(ҕ1,ҕ6);
			else{
			if(C(ч,s8)){
				m=(Aχ-(χ-Ь/2));
				s=m/(Ь/ҕ6);
				}
			if(C(ч,s9)){
				m=((SӋ-AӋ)-(SӋ-(Ӌ+Ћ/2)));
				s=m/(Ћ/ҕ6);
				}
			β.SetValue<float>(ҕ1,(ҕ4+s/100));
			if(C(т,h4))
			т=т.Replace(h4,s.ToString("F0"));
			}
			}}}
			}
		else if(C(ч,чe)){
			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string ҕ1=Թ(ref λ),ҕ2=Թ(ref λ),ҕ3=Թ(ref λ),ҕ4=Թ(ref λ);

			int R=int.Parse(ҕ2),G=int.Parse(ҕ3),B=int.Parse(ҕ4);
	

			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			β.SetValue<Color>(ҕ1,new Color (R,G,B));
			}}}
			}
		else if(C(ч,чf)){
			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			//if (λ=="Lock"|λ=="UnLock"|λ=="SwitchLock"|λ=="Attach"|λ=="Detach"){
			if (λ=="Attach"|λ=="Detach"){
			β.CustomName=β.CustomName+"["+λ+"]";
			λ=" ";
			}
			if(!λ.Contains(" "))
			β.ApplyAction(λ);
			}}
			}
		else if(C(ч,ч12)){
			foreach(var β in Т){
			string βC=β.CustomName;

			if(βC.Contains(ή)){

			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string ҕ1=Թ(ref λ),ҕ2=Թ(ref λ);

			if(!βC.Contains(ҕ1))
			βC+=ҕ1+" "+ҕ2;

			else if(βC.Contains(ҕ1)){

			if(βC.Contains(ҕ1)&&!βC.Contains(ҕ2))
			βC=βC.Replace(ҕ1,ҕ1+ҕ2);
			else if(βC.Contains(ҕ1)&&βC.Contains(ҕ2))
			βC=βC.Replace(ҕ2,"");

			}}
			else if(βC.Contains(λ)&&C(ч,"-"))
			βC=βC.Replace(λ,"");
			else if(!βC.Contains(λ)&&!ч.Contains("-"))
			βC+=λ;

			β.CustomName=βC;
			}}
			}
		else if(C(ч,ч13)){
			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string ҕ1=Թ(ref λ),ҕ2=Թ(ref λ);

			if(ҕ1.Contains("/#"))
			ҕ1=ҕ1.Replace("/#","//");

			if(ҕ2.Contains("/#"))
			ҕ2=ҕ2.Replace("/#","//");

			var Customdata=β.CustomData.Split('\n');
			β.CustomData="";
			ArrayList list=new ArrayList(Customdata);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(Customdata[i]);

			if(!ψ.Contains(ч14)){
			if(ψ.Contains(ҕ2));
			else if(ψ.Contains(ҕ1))
			ψ=ψ.Replace(ҕ1,ҕ2);
			
			}
			β.CustomData+=ψ+"\n";
			}}
			}}
			}
		else if(C(ч,чg)){

			if(!λ.Contains("|")&&ч.Contains("-"))
			Ƥ=Ƥ.Replace(λ+"\n","");
			else if(!Ƥ.Contains(λ)&&!ч.Contains("-")&&!λ.Contains("|"))
			Ƥ+=λ+"\n";

			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string ҕ1=Թ(ref λ),ҕ2=Թ(ref λ);

			if(!Ƥ.Contains(ҕ1))
			Ƥ+=ҕ1+" "+ҕ2+"\n";

			var Page=Ƥ.Split('\n');
			ArrayList list=new ArrayList(Page);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(Page[i]);

			if(C(ҕ2,"inv")&&C(ψ,ҕ1)){
			string ҕ7=ψ.Replace(ҕ1+" ","");
			if (ҕ7.Contains("-"))
			ҕ7=ҕ7.Replace("-","");
			else
			ҕ7="-"+ҕ7;
			Ƥ=Ƥ.Replace(ψ,ҕ1+" "+ҕ7);
			}

			else if(ҕ2.Contains("+")|ҕ2.Contains("-")&&ψ.Contains(ҕ1)&&!ψ.Contains("++")|!ψ.Contains("--")){
			if(ҕ2.Contains("+")){
			ҕ2=ҕ2.Replace("+","");
			string ҕ7=ψ.Replace(ҕ1,"");
			float ҕ4=float.Parse(ҕ7);
			float ҕ5=float.Parse(ҕ2);
			string ҕ6=(ҕ4+ҕ5).ToString();
			Ƥ=Ƥ.Replace(ψ,ҕ1+" "+ҕ6);
			}
			if(ҕ2.Contains("-")){
			ҕ2=ҕ2.Replace("-","");
			string ҕ7=ψ.Replace(ҕ1,"");
			float ҕ4=float.Parse(ҕ7);
			float ҕ5=float.Parse(ҕ2);
			string ҕ6=(ҕ4-ҕ5).ToString();
			Ƥ=Ƥ.Replace(ψ,ҕ1+" "+ҕ6);
			}
			}
			else if(!Ƥ.Contains(ҕ1)&&ҕ2.Contains("--")|ҕ2.Contains("++"));
			
			else if(Ƥ.Contains(ҕ1)){
			if(ψ.Contains(ҕ1)&&!ψ.Contains(ҕ2))
			Ƥ=Ƥ.Replace(ψ,ҕ1+" "+ҕ2);
			else if(ψ.Contains(ҕ1)&&ψ.Contains(ҕ2))
			Ƥ=Ƥ.Replace(ψ,ҕ1+" ");
			}}}
			}
		else if(C(ч,чh)){
			foreach(var β in в){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.ChargeMode==ChargeMode.Recharge)
			β.ChargeMode=ChargeMode.Discharge;
			else if(β.ChargeMode==ChargeMode.Discharge)
			β.ChargeMode=ChargeMode.Auto;
			else
			β.ChargeMode=ChargeMode.Recharge;
			}}}
			}
		else if(C(ч,чo)){
			foreach(var β in ĉ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			
			if(β.Status==MyShipConnectorStatus.Unconnected&&β.CollectAll==false){
			β.CollectAll=true;
			β.SetValueBool("Trading",true);
			}
			else if(β.Status==MyShipConnectorStatus.Unconnected&&β.CollectAll==true){
			β.CollectAll=false;
			β.SetValueBool("Trading",false);
			}

			else if(β.Status==MyShipConnectorStatus.Connected&&β.CollectAll==true)
			β.Disconnect();

			else if(β.Status==MyShipConnectorStatus.Connectable&&β.CollectAll==true)
			β.ToggleConnect();

			}}}
			}
		else if(C(ч,чp)){
			foreach(var β in ǵ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.AutoLock==true)
			β.AutoLock=false;
			else if(β.LockMode==LandingGearMode.Unlocked)
			β.AutoLock=true;
			else if(β.LockMode==LandingGearMode.Locked){
			β.Unlock();
			β.AutoLock=false;
			}
			else if(β.LockMode==LandingGearMode.ReadyToLock)
			β.Lock();

			}}}
			}
		else if(C(ч,ч14)){
			foreach(var β in ш){
			
			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			
			β.TryRun(λ);

			}}}
			}
		else if(C(ч,чr)){

			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string ntor=Թ(ref λ),nbtor=Թ(ref λ),nvel=Թ(ref λ),nlow=Թ(ref λ),nup=Թ(ref λ),ndis=Թ(ref λ);

			foreach(var β in Ŕ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float tor=β.Torque,btor=β.BrakingTorque,vel=β.TargetVelocityRPM,low=β.LowerLimitDeg,U=β.UpperLimitDeg,dis=β.Displacement;

			if(!ntor.Contains("x"))
			tor=float.Parse(ntor);
			if(!nbtor.Contains("x"))
			btor=float.Parse(nbtor);
			if(!nvel.Contains("x"))
			vel=float.Parse(nvel);
			if(!nlow.Contains("x"))
			low=float.Parse(nlow);
			if(!nup.Contains("x"))
			U=float.Parse(nup);
			if(!ndis.Contains("x"))
			dis=float.Parse(ndis);

			β.Torque=tor;
			β.BrakingTorque=btor;
			β.LowerLimitDeg=low;
			β.UpperLimitDeg=U;
			β.Displacement=dis*100+5;

			if(C(ч,"+"))
			β.TargetVelocityRPM=(β.TargetVelocityRPM+vel);
			else if(C(ч,"-"))
			β.TargetVelocityRPM=(β.TargetVelocityRPM-vel);
			else β.TargetVelocityRPM=vel;

			}}}}
			}
		else if(C(ч,чs)){
			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string nvel=Թ(ref λ),nlow=Թ(ref λ),nup=Թ(ref λ);

			foreach(var β in ϥ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float vel=β.Velocity,low=β.MinLimit,U=β.MaxLimit;
			
			if(!nvel.Contains("x"))
			vel=float.Parse(nvel);
			if(!nlow.Contains("x"))
			low=float.Parse(nlow);
			if(!nup.Contains("x"))
			U=float.Parse(nup);

			β.MinLimit=low;
			β.MaxLimit=U;

			if(C(ч,"<+>"))
			β.Velocity=(β.Velocity+vel);
			else if(C(ч,"<->"))
			β.Velocity=(β.Velocity-vel);
			else{β.Velocity=vel;}
			
			}}}}
			}
		else if(C(ч,чt)){
			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string nms=Թ(ref λ),npp=Թ(ref λ),nst=Թ(ref λ),nho=Թ(ref λ),nfr=Թ(ref λ),nsl=Թ(ref λ),npo=Թ(ref λ),nso=Թ(ref λ);

			foreach(var β in ώ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float ms=β.MaxSteerAngle,
			pp=β.Power,
			st=β.Strength,
			ho=β.Height,
			fr=β.Friction,
			sl=β.GetValue<float>("Speed Limit"),
			po=β.GetValue<float>("Propulsion override"),
			so=β.GetValue<float>("Steer override");
			
			if(!nms.Contains("x"))
			ms=float.Parse(nms);
			if(!npp.Contains("x"))
			pp=float.Parse(npp);
			if(!nst.Contains("x"))
			st=float.Parse(nst);
			if(!nho.Contains("x"))
			ho=float.Parse(nho);
			if(!nfr.Contains("x"))
			fr=float.Parse(nfr);
			if(!nsl.Contains("x"))
			sl=float.Parse(nsl);
			if(!npo.Contains("x"))
			po=float.Parse(npo);
			if(!nso.Contains("x"))
			so=float.Parse(nso);

			β.MaxSteerAngle=ms;
			β.Power=pp;
			β.Strength=st;
			β.Height=ho;
			β.Friction=fr;
			β.SetValue<float>("Speed Limit",sl);
			β.SetValue<float>("Propulsion override",po);
			β.SetValue<float>("Steer override",so);
			
			}}}}
			}

		else if(C(ч,ч15)){

			foreach(var β in ώ){
			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float po=β.GetValue<float>("Propulsion override"),
			speed=(float)ȣ.GetShipSpeed(),
			sl=β.GetValue<float>("Speed Limit");

			if(C(λ,A31)){
			if(dir==" "){
			if(po>=0)
			dir="+";
			if(po<=0)
			dir="-";
			}

			if(speed>=1&&dir=="-")
			β.SetValue<float>("Propulsion override",1);
			else if(speed>=1&&dir=="+")
			β.SetValue<float>("Propulsion override",-1);
			if(speed<=1){
			β.SetValue<float>("Propulsion override",0);
			dir=" ";
			}}

			else if(C(λ,A32)){
			sl=speed;
			po=100;
			}
			else if(C(λ,A33)){
			sl=200;
			po=0;
			}

			}}}
			}
		else if(C(ч,чw)){
			foreach(var β in τ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float cur=β.CurrentThrust,over=β.ThrustOverridePercentage;
			string ove=λ;

			if(ove.Contains(A32))
			over=cur;
			else if(ove.Contains(A33))
			over=0;
			else if(!ove.Contains("x"))
			over=float.Parse(ove);

			β.ThrustOverridePercentage=over;

			}}}
			}
		else if(C(ч,чz)){

			foreach(var β in ʘ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float vol=β.Volume,ran=β.Range,loop=β.LoopPeriod;
			string sou=β.SelectedSound;

			β.GetSounds(music);
			for(int i=0;i < music.Count;i++){
			string song=music[i];


			if(λ.Contains(A16)){
			if(song==sou){
			if(i+1 < music.Count)
			β.SelectedSound=music[i+1];
			}
			}

			else if(λ.Contains(A17)){
			if(song==sou){
			if(i - 1 > -1)
			β.SelectedSound=music[i-1];
			}
			}

			else if(λ.Contains(A34))
			β.Stop();
			else if(λ.Contains(A35))
			β.Play();
			
			else if(λ.Contains(A36))
			β.Volume=vol+0.01f;
			else if(λ.Contains(A37))
			β.Volume=vol-0.01f;

			}
			//presets
			var Page=PL.Split('\n');
			ArrayList list=new ArrayList(Page);
			for(int l=0;l < list.Count;l++){
			string ψ=Convert.ToString(Page[l]);

			if(λ.Contains(A38+т)&&ψ.Contains(т))
			PL=PL.Replace(ψ+"\n","");

			else if(λ.Contains(A32+т)){
			if (!PL.Contains(т))
			PL=PL.Insert(1,т+","+sou+"\n");
			else if(ψ.Contains(т)){
			ψ=ψ.Replace(т+",","");
			β.SelectedSound=ψ;
			if(β.IsSoundSelected)
			β.Play();
			}}

			}


			}}}
			}
		else if(C(ч,ч1)){

			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string Դ=Թ(ref λ),nQL=Թ(ref λ),nȈ=Թ(ref λ),nvol=Թ(ref λ);
			nȈ="MyObjectBuilder_"+ϐ+"/"+nȈ;

			foreach(var β in þ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			int QL=int.Parse(nQL);
			MyDefinitionId Ȉ=MyDefinitionId.Parse( nȈ);
			MyFixedPoint vol=int.Parse(nvol);

			if(Դ.Contains(A32))
			β.InsertQueueItem(QL,Ȉ,vol);

			if(Դ.Contains(A38))
			β.RemoveQueueItem(QL,vol);

			}}}}
			}
		else if(C(ч,ч2)){
			foreach(var β in Ă){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(C(λ,A40)){
			if(β.Mode==MyAssemblerMode.Assembly)
			β.Mode=MyAssemblerMode.Disassembly;
			else
			β.Mode=MyAssemblerMode.Assembly;
			}
			if(C(λ,A41)){
			if(β.Repeating)
			β.Repeating=false;
			else
			β.Repeating=true;
			}
			if(C(λ,A42)){
			if(β.CooperativeMode)
			β.CooperativeMode=false;
			else
			β.CooperativeMode=true;
			}

			}}}
			}
		else if(C(ч,ч3)){

			if(C(λ,"|")){
			λ=λ.Replace("|",",");

			string Դ=Թ(ref λ),nȈ=Թ(ref λ);
			nȈ="MyObjectBuilder_"+nȈ;

			foreach(var β in һ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			MyDefinitionId Ȉ=MyDefinitionId.Parse(nȈ);

			if(Դ.Contains(A32)){}
			β.AddItem(Ȉ);

			if(Դ.Contains(A38))
			β.RemoveItem(Ȉ);
			}}}}

			/*if(C(λ,"switch")){
			foreach(var β in һ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.Mode==MyConveyorSorterMode.Whitelist)
			β.SetFilter(MyConveyorSorterMode.Blacklist,Ȉs);
			else
			β.SetFilter(MyConveyorSorterMode.Whitelist,Ȉs);
			}}}}*/

			}
		else if(C(ч,ч11)){

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string λт=Թ(ref ч),nnr=Թ(ref ч);

			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			int λI=int.Parse(nnr);
			var lcd=β as IMyTextSurfaceProvider;

			if(lcd==null | lcd.SurfaceCount<λI){

			}
			else{
			IMyTextSurface SS=lcd.GetSurface(λI);
			if(λт.Contains(h4)){
			SS.WriteText(λ);
			}
			else
			SS.WriteText(т);
			}
			}}}
			}
		else if(C(ч,ч5)){
			com(λ);
			}
		if(LO=="1")
		Object(ƪ,χ,Ӌ,ה,Ћ/100*105,Ь/100*105,π,т,Ş,Ƈ_m,"Ϟ");
		}
	void input(MySpriteDrawFrame ƪ,string Ƨ,float χ,float Ӌ,string ה,float Ћ,float Ь,float π,string т,string Ş,string ή,string ч){

		string ήI=" ";Ϟ="0";

		if(ή.Contains("|")){
			ή=ή.Replace("|",",");
			string ή1=Թ(ref ή);
			string ή2=Թ(ref ή);
			ή=ή2;
			ήI=ή1;
		}
		Ƈ_l=Ƈ_orange;

		if(C(ч,чa)){
			foreach(var β in Т){
			Ƈ_l=Ƈ_white;
			if(C(β.CustomName,ή)){

			List<ITerminalAction> βA = new List<ITerminalAction>();
			β.GetActions(βA);
			foreach (var A in βA){
			string тт=($"{A.Id}: {A.Name}");
			т+=тт+"#";

			if(!ψD.Contains(тт))
			ψD+="//"+тт+"\n";
			}
			}}}

		else if(C(ч,чb)){
			foreach(var β in Т){
			Ƈ_l=Ƈ_white;
			if(C(β.CustomName,ή)){

			List<ITerminalProperty> βP = new List<ITerminalProperty>();
			β.GetProperties(βP);
			foreach (var A in βP){
			string тт=($"{A.Id}: {A.TypeName}");
			т+=тт+"#";

			if(!ψD.Contains(тт))
			ψD+="//"+тт+"\n";
			}
			}}}

		else if(C(ч,чc)){
			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string ҕ1=Թ(ref ч),
			ҕ2=Թ(ref ч),
			тт=β.GetValueBool(ҕ2).ToString();

			if(тт=="True"){
			Ƈ_l=Ƈ_on;
			Ϟ=S10;
			}
			if(тт=="False"){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}
			
			if(C(т,h4)){
			т=т.Replace(h4,Ϟ);
			}

			}
			}}
			}
		else if(C(ч,чd)){
			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string ҕ1=Թ(ref ч),ҕ2=Թ(ref ч),ҕ3=Թ(ref ч),
			тт=β.GetValue<float>(ҕ2).ToString(ҕ3);

			Ϟ=тт;

			if(C(т,h4)){
			т=т.Replace(h4,Ϟ);
			}

			}
			}}
			}
		else if(C(ч,чe)){
			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string ҕ1=Թ(ref ч),
			ҕ2=Թ(ref ч),
			тт=β.GetValue<Color>(ҕ2).ToString();
			тт=тт.Replace("{","");
			тт=тт.Replace("}","");
			тт=тт.Replace("R","");
			тт=тт.Replace("G","");
			тт=тт.Replace("B","");
			тт=тт.Replace("A","");
			тт=тт.Replace(" :",",");
			
			string R=Թ(ref тт),
			G=Թ(ref тт),
			B=Թ(ref тт),
			A=Թ(ref тт);

			тт="";
			тт+=" R"+R;
			тт+=" G:"+G;
			тт+=" B:"+B;
			тт+=" A:"+B;
			
			Ϟ=тт;
			Ƈ_l=β.GetValue<Color>(ҕ2);

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);
			if(C(т,h6))
			т=т.Replace(h6,"R"+R);
			if(C(т,h7))
			т=т.Replace(h7,"G:"+G);
			if(C(т,h8))
			т=т.Replace(h8,"B:"+B);
			if(C(т,h9))
			т=т.Replace(h9,"A:"+A);

			}
			}}
			}
		else if(C(ч,чf)){
			foreach(var β in Т){

			if(C(β.CustomName,ή)){
			Vector3D dis=β.GetPosition();

			if(!β.IsFunctional){
			Ƈ_l=Ƈ_orange;
			}
			else if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			Ϟ=S10;
			}
			else{
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}

			if(C(т,h4)){
			т=т.Replace(h4,Ϟ);
			}

			if(C(т,h5)){
			Ƈ_l=Ƈ_white;
			string тт=(dis).ToString("F0");
			тт=тт.Replace("{","");
			тт=тт.Replace("}","");
			т=т.Replace(h5,тт);
			}

			}}
			}
		else if(C(ч,чg)){
			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string ҕ1=Թ(ref ч),ҕ2=Թ(ref ч);

			if(Ƥ.Contains(ҕ2))
			Ƈ_l=Ƈ_on;
			else
			Ƈ_l=Ƈ_off;
			
			if(C(т,h4)){

			var Page=Ƥ.Split('\n');
			ArrayList list=new ArrayList(Page);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(Page[i]);

			if(C(ψ,ҕ2)){
			ψ=ψ.Replace(ҕ2,"");
			т=т.Replace(h4,ψ);
			}
			if(C(ψ,"++")|C(ψ,"--")&&C(ψ,ҕ2)){
			if(Ĉ==1 )
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_lblue;
			}
			}}}
			}

		else if(C(ч,чh)){
			float maxP=0,curP=0,maxI=0,curI=0;string c="",CH="";
			foreach(var β in в){
			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			

			if(!β.IsWorking)
			c+="r";
			else if(β.ChargeMode==ChargeMode.Recharge)
			c+="y";
			else if(β.ChargeMode==ChargeMode.Discharge)
			c+="b";
			else
			c+="g";

			maxP=maxP+β.MaxStoredPower;
			curP=curP+β.CurrentStoredPower;

			maxI=maxI+β.MaxInput;
			curI=curI+β.CurrentInput;

			if (β.IsCharging)
			CH=S14;
			else
			CH=S15;

			if(C(т,a3))
			т=т.Replace(a3,(β.ChargeMode.ToString()));
			if(C(ч,a3))
			Ϟ=(β.ChargeMode.ToString());

			if(C(т,h4))
			т=т.Replace(h4,CH);

			if(C(ч,h4))
			Ϟ=CH;

			}}}
			var fill=curP/(maxP/100);
			bool f = Double.IsNaN(fill);
			if(f== true)fill=0;
			if(C(ч,h5))
			Ϟ=fill.ToString("F0");

			var fillI=curI/(maxI/100);
			bool f1 = Double.IsNaN(fillI);
			if(f1== true)fill=0;
			if(C(ч,b4))
			Ϟ=fillI.ToString("F0");


			if (C(c,"y")&&!c.Contains("b")&&!c.Contains("r")&&!c.Contains("g"))
			Ƈ_l=Ƈ_yellow;
			else if (C(c,"b")&&!c.Contains("y")&&!c.Contains("r")&&!c.Contains("g"))
			Ƈ_l=Ƈ_lblue;
			else if (C(c,"g")&&!c.Contains("y")&&!c.Contains("r")&&!c.Contains("b"))
			Ƈ_l=Ƈ_green;
			else if (C(c,"r")&&!c.Contains("y")&&!c.Contains("b")&&!c.Contains("g"))
			Ƈ_l=Ƈ_off;
			else
			Ƈ_l=Ƈ_on;


			if( fill<10){
			Ƈ_l=Ƈ_red;
			ч=ч+"bli";
			}


			if(C(т,b1))
			т=т.Replace(b1,(curP.ToString("F3")));
			if(C(т,b2))
			т=т.Replace(b2,(maxP.ToString("F3")));
			if(C(т,b5))
			т=т.Replace(b5,(curI.ToString("F3")));
			if(C(т,b6))
			т=т.Replace(b6,(maxI.ToString("F3")));

			if(C(т,h5))
			т=т.Replace(h5,(fill*1).ToString("F0"));

			if(C(т,b4))
			т=т.Replace(b4,(fillI*1).ToString("F0"));
			}
		else if(C(ч,чi)){
			float max=0,cur=0;
			foreach(var β in Ƿ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			
			if(!β.IsWorking)
			Ƈ_l=Ƈ_off;
			else if(β.IsWorking){
			Ƈ_l=Ƈ_on;

			max=max+β.MaxOutput;
			cur=cur+β.CurrentOutput;

			}
			}}}
			var fill=cur/(max/100);
			bool f = Double.IsNaN(fill);
			if(f== true)fill=0;

			Ϟ=(fill).ToString("F0");

			if(fill>95){
			Ƈ_l=Ƈ_red;
			ч=ч+"bli";
			}

			if(C(т,b1))
			т=т.Replace(b1,(cur.ToString("F3")));
			if(C(т,b2))
			т=т.Replace(b2,(max.ToString("F3")));

			if(C(т,h5))
			т=т.Replace(h5,(fill).ToString("F0"));

			Ϟ=(fill*1).ToString("F0");

			}
		else if(C(ч,чj)){
			double fuel=0;int tanks=0;float max=0,cur=0;string c="";
			foreach(var β in Ф){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(!β.IsWorking)
			c+="r";
			else if(β.Stockpile==true)
			c+="b";
			else
			c+="g";

			fuel+=β.FilledRatio;
			++tanks;

			max=max+β.Capacity;
			cur=cur+(β.Capacity*(float)β.FilledRatio);
			}}}

			var fill=(float)(fuel/tanks*100);

			if (C(c,"b")&&!c.Contains("r")&&!c.Contains("g"))
			Ƈ_l=Ƈ_lblue;
			else if (C(c,"r")&&!c.Contains("b")&&!c.Contains("g"))
			Ƈ_l=Ƈ_off;
			else 
			Ƈ_l=Ƈ_on;

			if(C(т,b1))
			т=т.Replace(b1,(cur.ToString("F0")));
			if(C(т,b2))
			т=т.Replace(b2,(max.ToString("F0")));

			if(fill<10){
			Ƈ_l=Ƈ_red;
			ч=ч+"bli";
			}

			if(C(т,h5))
			т=т.Replace(h5,(fill*1).ToString("F0"));
			Ϟ=(fill*1).ToString("F0");
			}

		else if(C(ч,чk)){
			float CM=0,CC=0;
			string тт="";

			foreach(var β in Т){
			if(C(β.CustomName,ή)){

			Ƈ_l=Ƈ_on;

			List<MyInventoryItem>GQ=new List<MyInventoryItem>();
			β.GetInventory(0).GetItems(GQ);
			CM+=(float)β.GetInventory(0).MaxVolume*1000;
			CC+=(float)β.GetInventory(0).CurrentVolume*1000;

			if(β.InventoryCount>1){
			β.GetInventory(1).GetItems(GQ);
			CM+=(float)β.GetInventory(1).MaxVolume*1000;
			CC+=(float)β.GetInventory(1).CurrentVolume*1000;
			}
			foreach(var QI in GQ){
				string Ҋ=(QI.Type).ToString();
				string am=(QI.Amount).ToString();
				float an=float.Parse(am);
			
				Ҋ=Ҋ.Replace("MyObjectBuilder_","");
				Ҋ=Ҋ.Replace("OxygenContainerObject/","");
				Ҋ=Ҋ.Replace("GasContainerObject/","");
				Ҋ=Ҋ.Replace("PhysicalGunObject/","");
				Ҋ=Ҋ.Replace("PhysicalGunObject/","");
				Ҋ=Ҋ.Replace("Component/","");
				Ҋ=Ҋ.Replace("AmmoMagazine/","");
				Ҋ=Ҋ.Replace("Item","");
				if(Ҋ.Contains("Ingot"))
				Ҋ+=" ingot";
				Ҋ=Ҋ.Replace("Ingot/"," ");
				if(Ҋ.Contains("Ore"))
				Ҋ+=" ore";
				Ҋ=Ҋ.Replace("Ore/"," ");

			if(C(тт,Ҋ)){
				var Page=тт.Split('#');
				ArrayList list=new ArrayList(Page);
				for(int i=0;i < list.Count;i++){
				string ψ=Convert.ToString(Page[i]);

				if(C(ψ,Ҋ)){
				string ҕ1=ψ.Replace(Ҋ,"");
				float ҕ2=float.Parse(ҕ1);
				string ҕ6;
				if(C(Ҋ,"ore")|C(Ҋ,"ingot")){
				ҕ6=((ҕ2+an)).ToString("F2");
				}
				else
				ҕ6=(ҕ2+an).ToString("F0");
				тт=тт.Replace(ψ,Ҋ+" "+ҕ6);
				}}}
			else{
			if(C(Ҋ,"ore")|C(Ҋ,"ingot"))
			тт+="#"+Ҋ+" "+(an).ToString("F2");
			else
			тт+="#"+Ҋ+" "+(an).ToString("F0");
			}
			}

			}}
			тт+="#";
			var fill=CC/(CM/100);
			bool f = Double.IsNaN(fill);
			if(f== true)fill=0;

			if(C(ч,h5))
			Ϟ=(fill).ToString("F0");

			if(C(т,h5))
			т=т.Replace(h5,(fill).ToString("F0"));
			if(C(т,b1))
			т=т.Replace(b1,(CC).ToString("F0"));
			if(C(т,b2))
			т=т.Replace(b2,(CM).ToString("F0"));

			if(ч.Contains(b7)){
			if(fill<10){
			Ƈ_l=Ƈ_red;
			ч=ч+"bli";
			}}

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string ҕ1=Թ(ref ч),ҕ2=Թ(ref ч);

			if(C(тт,ҕ2)){
				var Page=тт.Split('#');
				ArrayList list=new ArrayList(Page);
				for(int i=0;i < list.Count;i++){
				string ψ=Convert.ToString(Page[i]);

			if(C(ψ,ҕ2)){
			ψ=ψ.Replace(ҕ2,"");
			т=т.Replace(i3,ψ);
			Ϟ=ψ;
			}}}
			else
			т=т.Replace(i3," 0");

			}

			if(C(т,b3))
			т=т.Replace(b3,тт);
			}
		else if(C(ч,чl)){

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string Դ=Թ(ref ч),nnr=Թ(ref ч);
			Ƈ_l=Ƈ_white;

			foreach(var β in Т){

			if(C(β.CustomName,ή)){

			int nr=int.Parse(nnr);

			if(Դ.Contains(i3)){
			List<MyInventoryItem>QA=new List<MyInventoryItem>();
			β.GetInventory(0).GetItems(QA);

			if(nr<QA.Count){
			var QI=QA[nr];
			string Ҋ=(QI.Type).ToString(),am=(QI.Amount).ToString();
			ה=Ҋ;
			if(C(т,h4)){
			if(Ҋ.Contains("Ore")|Ҋ.Contains("Ingot")){
			float an=float.Parse(am);
			т=т.Replace(h4,(an).ToString("F0")+" kg");
			Ϟ=(an).ToString("F2");
			}
			else{
			т=т.Replace(h4,am);
			Ϟ=am;
			}
			}}
			
			}

			}}}

			}

		else if(C(ч,чm)){
			foreach(var β in ҏ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			Ƈ_l=Ƈ_on;

			double fuel=0;int tanks=0;
			fuel+=β.OxygenFilledRatio;
			++tanks;
			var fill=(float)(fuel/tanks*100);
			if(tanks==0)fill=0;

			if(C(ч,h5))
			Ϟ=(fill).ToString("F0");

			if(C(т,h5))
			т=т.Replace(h5,(fill.ToString("F0")));

			}}}}
		else if(C(ч,чn)){
			foreach(var β in Ɔ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(C(ч,c1)){
			if(β.IsUnderControl==true){
			Ƈ_l=Ƈ_lblue;
			Ϟ=S10;
			}}

			if(C(ч,c2+h0)){
			Vector2D R=Rot("N");
			т+=R.X.ToString("F0");
			Ϟ=R.X.ToString("F0");
			Ƈ_l=Ƈ_white;
			}

			if(C(ч,c3+h0)){
			Vector2D R=Rot("N");
			т+=R.Y.ToString("F0");
			Ϟ=R.Y.ToString("F0");;
			Ƈ_l=Ƈ_white;
			}

			if(C(ч,c2+h9)){
			Vector2D R=Rot("A");
			т+=R.X.ToString("F0");
			Ϟ=R.X.ToString("F0");
			Ƈ_l=Ƈ_white;
			}

			if(C(ч,c3+h9)){
			Vector2D R=Rot("A");
			т+=R.Y.ToString("F0");
			Ϟ=R.Y.ToString("F0");;
			Ƈ_l=Ƈ_white;
			}

			}}}}
		else if(C(ч,чo)){
			foreach(var β in ĉ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.Status==MyShipConnectorStatus.Unconnected)
			Ƈ_l=Ƈ_white;
			if(β.CollectAll==true)
			Ƈ_l=Ƈ_lblue;
			if(β.ThrowOut==true)
			Ƈ_l=Ƈ_red;
			if(β.Status==MyShipConnectorStatus.Connectable)
			Ƈ_l=Ƈ_yellow;
			if(β.Status==MyShipConnectorStatus.Connected)
			Ƈ_l=Ƈ_green;
			if(β.CollectAll==true){
			if(Ĉ==1 )
			Ƈ_l=Ƈ_lblue;
			}
			if(β.ThrowOut==true){
			if(Ĉ==5 )
			Ƈ_l=Ƈ_red;
			}
			Ϟ=β.Status.ToString();

			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);

			}}}}

		else if(C(ч,чp)){
			foreach(var β in ǵ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.LockMode==LandingGearMode.Unlocked)
			Ƈ_l=Ƈ_white;
			if(β.AutoLock==true)
			Ƈ_l=Ƈ_lblue;

			if(β.LockMode==LandingGearMode.Locked)
			Ƈ_l=Ƈ_green;
			if(β.LockMode==LandingGearMode.ReadyToLock)
			Ƈ_l=Ƈ_yellow;
			if(β.AutoLock==true){
			if(Ĉ==1 )
			Ƈ_l=Ƈ_lblue;
			}
			//if(β.AutoLock==true)Ϟ=S17;
			Ϟ=β.LockMode.ToString();

			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);

			}}}}
		else if(C(ч,чq)){
			foreach(var β in ϙ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			
			if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			Ϟ=S10;
			}
			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}
			if(β.IsShooting){
			Ƈ_l=Ƈ_lblue;
			Ϟ=S12;
			}

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);
			}}}
			}
		else if(C(ч,чr)){
			foreach(var β in Ŕ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			Ƈ_l=Ƈ_Arrowon;

			double ang=β.Angle*57.2957795,
			tor=β.Torque,
			btor=β.BrakingTorque,
			vel=β.TargetVelocityRPM,
			low=β.LowerLimitDeg,
			U=β.UpperLimitDeg,
			dis=β.Displacement*100+5;

			if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			Ϟ=S10;
			}
			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}
			if(β.PendingAttachment){
			Ƈ_l=Ƈ_red;
			}
			if(β.IsAttached){
			Ƈ_l=Ƈ_lblue;
			Ϟ=S16;
			}

			if(ang >360)
			ang=0;

			if(C(ч,h4));
			else{
			Ϟ=S1;
			if(C(ч,r1))
			Ϟ=(ang).ToString("F0");
			else if(C(ч,h5))
			Ϟ=(ang/100).ToString("F0");
			else if(C(ч,r2))
			Ϟ=(tor).ToString("F0");
			else if(C(ч,r3))
			Ϟ=(btor).ToString("F0");
			else if(C(ч,r4))
			Ϟ=(dis).ToString("F0");
			else if(C(ч,r5))
			Ϟ=(vel).ToString("F0");
			else if(C(ч,r7))
			Ϟ=(U).ToString("F0");
			else if(C(ч,r6))
			Ϟ=(low).ToString("F0");
			}

			if(C(т,r1))
			т=т.Replace(r1,(ang).ToString("F0")+"°");
			if(C(т,r2))
			т=т.Replace(r2,(tor).ToString("F0")+"Nm");
			if(C(т,r3))
			т=т.Replace(r3,(btor).ToString("F0")+"Nm");
			if(C(т,r5))
			т=т.Replace(r5,(vel).ToString("F0")+"rpm");
			if(C(т,r6))
			т=т.Replace(r6,(low).ToString("F0")+"°");
			if(C(т,r7))
			т=т.Replace(r7,(U).ToString("F0")+"°");
			if(C(т,r4))
			т=т.Replace(r4,(dis).ToString("F0")+"cm");

			}}}}
		else if(C(ч,чs)){
			foreach(var β in ϥ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			Ƈ_l=Ƈ_Arrowon;

			float vel=β.Velocity,
			low=β.MinLimit,
			U=β.MaxLimit,
			pos=β.CurrentPosition,
			upm=β.HighestPosition;

			Ϟ=S1;
			if(C(ч,h5))
			Ϟ=(pos/upm*100).ToString("F0");
			else if(C(ч,r8))
			Ϟ=(pos).ToString("F0");
			else if(C(ч,r5))
			Ϟ=(50+vel*10).ToString("F0");
			else if(C(ч,r7))
			Ϟ=(U/upm*100).ToString("F0");
			else if(C(ч,r6))
			Ϟ=(low/upm*100).ToString("F0");

			if(C(т,r5))
			т=т.Replace(r5,(vel).ToString("F0")+" rpm");
			if(C(т,r6))
			т=т.Replace(r6,(low).ToString("F2")+" m");
			if(C(т,r7))
			т=т.Replace(r7,(U).ToString("F2")+" m");
			if(C(т,r8))
			т=т.Replace(r8,(pos).ToString("F2")+" m");

			}}}}
		else if(C(ч,чt)){
			foreach(var β in ώ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			Ƈ_l=Ƈ_Arrowon;

			float ms=β.MaxSteerAngle*58,
			pp=β.Power,
			st=β.Strength,
			ho=β.Height*100,
			fr=β.Friction,
			sl=β.GetValue<float>("Speed Limit"),
			po=β.GetValue<float>("Propulsion override"),
			so=β.GetValue<float>("Steer override");

			Ϟ=S1;

			if(C(ч,w1))
			Ϟ=(ms).ToString("F0");
			else if(C(ч,w2))
			Ϟ=(pp).ToString("F0");
			else if(C(ч,w3))
			Ϟ=(st).ToString("F0");
			else if(C(ч,w4))
			Ϟ=(ho+50).ToString("F0");
			else if(C(ч,w5))
			Ϟ=(fr).ToString("F0");
			else if(C(ч,w6))
			Ϟ=(sl).ToString("F0");
			else if(C(ч,w7))
			Ϟ=(po*100).ToString("F0");
			else if(C(ч,w8))
			Ϟ=(so*100).ToString("F0");

			if(C(т,w1))
			т=т.Replace(w1,(ms).ToString("F0")+" °");
			if(C(т,w2))
			т=т.Replace(w2,(pp).ToString("F2")+" %");
			if(C(т,w3))
			т=т.Replace(w3,(st).ToString("F2")+" %");
			if(C(т,w4))
			т=т.Replace(w4,(ho).ToString("F2")+" cm");
			if(C(т,w5))
			т=т.Replace(w5,(fr).ToString("F0")+" %");
			if(C(т,w6))
			т=т.Replace(w6,(sl).ToString("F2")+" km/h");
			if(C(т,w7))
			т=т.Replace(w7,(po*100).ToString("F2")+" %");
			if(C(т,w8))
			т=т.Replace(w8,(so*100).ToString("F2")+" %");

			}}}}
		else if(C(ч,чu)){
			foreach(var β in Ð){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			float open=β.OpenRatio*100;
			
			if(β.Status==DoorStatus.Open)
			Ƈ_l=Ƈ_yellow;
			
			if(β.Status==DoorStatus.Opening)
			if(Ĉ==1 )
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_yellow;
			
			if(β.Status==DoorStatus.Closing)
			if(Ĉ==1 )
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_lblue;
			
			if(β.Status==DoorStatus.Closed)
			Ƈ_l=Ƈ_lblue;

			if(!β.IsWorking)
			Ƈ_l=Ƈ_off;

			if(C(ч,h4))
			Ϟ=β.Status.ToString();

			if(C(ч,h5))
			Ϟ=(open).ToString("F0");

			if(C(т,h4))
			т=т.Replace(h4,(β.Status.ToString()));

			if(C(т,h5))
			т=т.Replace(h5,(open.ToString("F0")));

			}}}}
		else if(C(ч,чv)){
			double fuel=0;int tanks=0;string c="";
			foreach(var β in α){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			fuel+=β.GetOxygenLevel();
			++tanks;

			if(β.CanPressurize){
			if(β.Status==VentStatus.Depressurized)
			Ƈ_l=Ƈ_lblue;
			
			if(β.Status==VentStatus.Depressurizing)
			if(Ĉ==1)
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_lblue;
			
			if(β.Status==VentStatus.Pressurizing)
			if(Ĉ==1)
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_green;

			if(β.Status==VentStatus.Pressurized)
			Ƈ_l=Ƈ_green;

			c=β.Status.ToString();
			}
			else{
			Ƈ_l=Ƈ_yellow;
			c=S17;
			}
			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			c=S11;
			}
			}}}

			var fill=(float)(fuel/tanks*100);

			if(C(ч,h4))
			Ϟ=c;
		
			if(C(ч,h5))
			Ϟ=(fill).ToString("F0");

			if(C(т,h4))
			т=т.Replace(h4,c);

			if(C(т,h5))
			т=т.Replace(h5,(fill.ToString("F0")));

			}
		else if(C(ч,чw)){
			foreach(var β in τ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			
			Vector3I dir=β.GridThrustDirection;
			float cur=β.CurrentThrust,
			max=β.MaxThrust,
			eff=β.MaxEffectiveThrust;

			if(β.IsWorking)
			Ƈ_l=Ƈ_on;

			if(!β.IsWorking)
			Ƈ_l=Ƈ_off;

			if(C(т,h4))
			т=т.Replace(h4,(dir.ToString()));

			if(C(т,h5))
			т=т.Replace(h5,((cur/max*100).ToString("F0")));

			if(C(ч,h5))
			Ϟ=(cur/max*100).ToString("F0");

			}}}}
		else if(C(ч,чx)){
			foreach(var β in Ș){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			Ϟ=S10;
			}
			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}
			if(β.IsActive&&β.IsWorking){
			Ƈ_l=Ƈ_lblue;
			Ϟ=S13;

			List<MyDetectedEntityInfo>entities=new List<MyDetectedEntityInfo>();
			β.DetectedEntities(entities);
			foreach(var info in entities){

			long id=info.EntityId;
			MyDetectedEntityType idtype=info.Type;
			double posx=info.Position.X,
			posy=info.Position.Y,
			posz=info.Position.Z,
			boxx=info.BoundingBox.Size.X,
			boxy=info.BoundingBox.Size.Y,
			boxz=info.BoundingBox.Size.Z;
			MyRelationsBetweenPlayerAndBlock rel=info.Relationship;
			string Ҋ=info.Name,
			time=DateTime.Now.ToString("HH:mm"),
			checkid=(id).ToString();

			if(!ψD.Contains(checkid)){
			ψD+="\ndetected="+(id).ToString();
			ψD+=","+(ή).ToString();
			ψD+=", "+(Ҋ).ToString();
			ψD+=", "+(idtype).ToString();
			ψD+=", "+(rel).ToString();
			ψD+=", "+(posx).ToString("F0");
			ψD+=", "+(posy).ToString("F0");
			ψD+=", "+(posz).ToString("F0");
			ψD+=", "+(boxx).ToString("F0");
			ψD+=", "+(boxy).ToString("F0");
			ψD+=", "+(boxz).ToString("F0");
			ψD+=", "+(time).ToString();

			}}}

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);

			if(C(ч,b3)){
			var ЩCC=ψD.Split('\n');
			ArrayList list=new ArrayList(ЩCC);
			for(int t=0;t < list.Count;t++){
			string ψ=Convert.ToString(ЩCC[t]);

			if(ψ.Contains("detected")){
			ψ=ψ.Replace("detected","");

			if(ψ.Contains(",")){
			string id=Թ(ref ψ),
			Ď=Թ(ref ψ),
			Ҋ=Թ(ref ψ),
			dtype=Թ(ref ψ),
			rel=Թ(ref ψ),
			X=Թ(ref ψ),
			Y=Թ(ref ψ),
			Z=Թ(ref ψ),
			L=Թ(ref ψ),
			W=Թ(ref ψ),
			H=Թ(ref ψ),
			dtime=Թ(ref ψ);

			if(ή==Ď){
			т+="#";
			if(C(ч,s1))т+=Ҋ;
			if(C(ч,s2))т+=dtype;
			if(C(ч,s3))т+=rel;
			if(C(ч,s4))т+=X;
			if(C(ч,s5))т+=Y;
			if(C(ч,s6))т+=Z;
			if(C(ч,s7))т+=L;
			if(C(ч,s8))т+=W;
			if(C(ч,s9))т+=H;
			if(C(ч,s0))т+=dtime;
			}

			}
			}}}
			}}}}
		else if(C(ч,чy)){
			foreach(var β in φ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			Ϟ=S10;
			}
			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}
			if(β.IsProjecting&&β.IsWorking){
			Ƈ_l=Ƈ_lblue;
			Ϟ=S13;
			}

			string BT="Armor"+β.RemainingArmorBlocks.ToString()+"\n";
			foreach(var kv in β.RemainingBlocksPerType)
			{
			string BR=kv.Key.ToString();
			var ЩCC=BR.Split('/');
			ArrayList list=new ArrayList(ЩCC);
			BT+=Convert.ToString(ЩCC[0]);

			BT+=": "+ kv.Value.ToString()+"\n";
			}
			BT=BT.Replace("MyObjectBuilder_","");

			string TB=β.TotalBlocks.ToString(),
			RB=β.RemainingBlocks.ToString(),
			BC=β.BuildableBlocksCount.ToString(),
			PO=β.ProjectionOffset.ToString(),
			PR=β.ProjectionRotation.ToString(),
			PT=((β.TotalBlocks-β.RemainingBlocks)/(β.TotalBlocks/100)).ToString();//percent calculation
			PO=PO.Replace("[","");
			PO=PO.Replace("]","");
			PR=PR.Replace("[","");
			PR=PR.Replace("]","");


			if(C(ч,p1))
			Ϟ=(TB);
			if(C(ч,p2))
			Ϟ=(RB);
			if(C(ч,p3))
			Ϟ=(BC);
			if(C(ч,h5))
			Ϟ=(PT);

			if(C(т,p1))
			т=т.Replace(p1,TB);
			if(C(т,p2))
			т=т.Replace(p2,RB);
			if(C(т,b3))
			т=т.Replace(b3,BT);
			if(C(т,p3))
			т=т.Replace(p3,BC);
			if(C(т,p4))
			т=т.Replace(p4,PO);
			if(C(т,p5))
			т=т.Replace(p5,PR);
			if(C(т,h5))
			т=т.Replace(h5,PT);
			if(C(т,h4))
			т=т.Replace(h4,Ϟ);
			}}}

			}
		else if(C(ч,чz)){
			foreach(var β in ʘ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			
			if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			
			float vol=β.Volume,
			ran=β.Range,
			loop=β.LoopPeriod;
			string sou=β.SelectedSound;

			if(C(т,m1))
			т=т.Replace(m1,(vol*100).ToString("F0"));
			Ϟ=(vol*100).ToString("F0");
			if(C(т,m2))
			т=т.Replace(m2,(ran).ToString("F0"));
			if(C(т,m3))
			т=т.Replace(m3,(loop).ToString("F0"));
			if(C(т,m4))
			т=т.Replace(m4,(sou).ToString());
			if(C(т,b3))
			т=т.Replace(b3,PL);
			}
			else if(!β.IsWorking){
			т="";
			Ƈ_l=Ƈ_off;
			}
			}}}

			}

		else if(C(ч,ч1)){
			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string Դ=Թ(ref ч),
			nnr=Թ(ref ч);
			Ƈ_l=Ƈ_white;

			foreach(var β in þ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			int nr=int.Parse(nnr);
			
			if(Դ.Contains(i4)|Դ.Contains(i5)){
				List<MyInventoryItem>QA=new List<MyInventoryItem>();

				if(Դ.Contains(i4))
				β.InputInventory.GetItems(QA);
				if(Դ.Contains(i5))
				β.OutputInventory.GetItems(QA);

				if(nr<QA.Count){
				var QI=QA[nr];
			
				string Ҋ=(QI.Type).ToString(),
				am=(QI.Amount).ToString();

				ה=Ҋ;

				if(Ҋ.Contains("Ore")|Ҋ.Contains("Ingot")){
				float an=float.Parse(am);
				т=т.Replace(h4,(an).ToString("F0")+"kg");
				Ϟ=(an).ToString("F2");
				}
				else{
				т=т.Replace(h4,am);
				Ϟ=am;
				}
				}
				else{
				т=т.Replace(h4,"");
				
				}
			}

			if(Դ.Contains(i6)){
				List<MyProductionItem>QA=new List<MyProductionItem>();
				β.GetQueue(QA);
				if(nr<QA.Count){
				var QI=QA[nr];

				string Ҋ=(QI.BlueprintId).ToString(),
				am=(QI.Amount).ToString();

				if(Ҋ.Contains("Component")){
				Ҋ=Ҋ.Replace("Component","");
				Ҋ=Ҋ.Replace(ϐ,"Component");
				}
				else if(Ҋ.Contains("mm")){
				Ҋ=Ҋ.Replace("Magazine","");
				Ҋ=Ҋ.Replace(ϐ,"AmmoMagazine");
				}
				else if(Ҋ.Contains("Oxy"))
				Ҋ=Ҋ.Replace(ϐ,"OxygenContainerObject");
			
				else if(Ҋ.Contains("Hyd"))
				Ҋ=Ҋ.Replace(ϐ,"GasContainerObject");

				else if(Ҋ.Contains("Data"))
				Ҋ=Ҋ.Replace(ϐ,"Datapad");
			
				else if(Ҋ.Contains("Wel")|Ҋ.Contains("Gri")|Ҋ.Contains("Dri")){
				Ҋ=Ҋ.Replace(ϐ,"PhysicalGunObject");
				Ҋ+="Item";
				}
			
				else if(Ҋ.Contains("Rifle")){
				Ҋ=Ҋ.Replace(ϐ,"PhysicalGunObject");
				Ҋ+="Item";
				}
				else if(Ҋ.Contains("OreTo")){
				Ҋ=Ҋ.Replace("OreToIngot","");
				Ҋ=Ҋ.Replace(ϐ,"Ingot");
				}
				else
				Ҋ=Ҋ.Replace(ϐ,"Component");

				if(Ҋ.Contains("Basic"))
				Ҋ=Ҋ.Replace("Basic","");

				ה=Ҋ;

				Ҋ=Ҋ.Replace("MyObjectBuilder_","");
				if(Ҋ.Contains("Ore")|Ҋ.Contains("Ingot")){
				float an=float.Parse(am);
				т=т.Replace(h4,(an).ToString("F2")+"kg");
				Ϟ=Ҋ+" "+(an).ToString("F2");
				}
				else{
				т=т.Replace(h4,Ҋ+" "+am);
				Ϟ=am;
				}
				}
				else{
				т=т.Replace(h4,"");
				
				}
			}

			}}}}
			}
		else if(C(ч,ч2)){
			foreach(var β in Ă){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){
			float CPr=β.CurrentProgress*100;
			Ƈ_l=Ƈ_grey;
			if(C(ч,a3)){
			if(β.Mode==MyAssemblerMode.Assembly)
			т+=" "+S2;
			else if(β.Mode==MyAssemblerMode.Disassembly	)
			т+=" "+S3;
			}

			if(C(ч,a1)){
			if(β.CooperativeMode)
			Ƈ_l=Ƈ_green;
			}
			
			if(C(ч,a2)){
			if(β.Repeating)
			if(Ĉ==1 )
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_green;
			}

			if(C(ч,h5))
			т=т.Replace(h5,(CPr.ToString("F0")));

			}}}
			}
		else if(C(ч,ч3)){
			string тт ="";

			foreach(var β in һ){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S10;
			}
			if(β.IsWorking){
			Ƈ_l=Ƈ_on;
			Ϟ=S11;
			}

			β.GetFilterList(Ȉs);
			foreach(var info in Ȉs){

			string id= info.ItemId.ToString();
			string idt="";
			if (id.Contains("null")){
			id=id.Replace("MyObjectBuilder_","");
			var ЩCC=id.Split('/');
			ArrayList list=new ArrayList(ЩCC);
			idt+=Convert.ToString(ЩCC[0]);
			тт +="*"+idt+"* \n";
			}
			else{
			var ЩCC=id.Split('/');
			ArrayList list=new ArrayList(ЩCC);
			idt+=Convert.ToString(ЩCC[1]);
			тт +=idt +" \n";
			}
			}
			if(C(т,b3))
			т=т.Replace(b3,тт);

			if(C(т,a3))
			т=т.Replace(a3,β.Mode.ToString());

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);
			
			}}}
			}
		else if(C(ч,ч4)){
			double fuel=0;int tanks=0;
			string IP="",CT="";

			foreach(var β in Ď){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			float max=β.MaxStoredPower,cur=β.CurrentStoredPower;

			string IF=β.DetailedInfo;
			var ЩCC=IF.Split('\n');
			ArrayList list=new ArrayList(ЩCC);
			for(int i=0;i < list.Count;i++){
			IP=Convert.ToString(ЩCC[3]);
			IP=IP.Replace("Current Input:","");
			IP=IP.Replace("M","");
			IP=IP.Replace("W","");
			CT=Convert.ToString(ЩCC[5]);
			CT=CT.Replace("Fully recharged in:","");
			}

			if(β.IsWorking){
			if(β.Status==MyJumpDriveStatus.Charging&&(!IP.Contains("0"))){
			Ƈ_l=Ƈ_yellow;
			Ϟ=β.Status.ToString();
			}
			else if(β.Status==MyJumpDriveStatus.Ready){
			Ƈ_l=Ƈ_green;
			Ϟ=β.Status.ToString();
			}
			else if(β.Status==MyJumpDriveStatus.Jumping){
			Ƈ_l=Ƈ_lblue;
			Ϟ=β.Status.ToString();
			}
			else{
			Ƈ_l=Ƈ_red;
			Ϟ=S14;
			}
			}
			if(!β.IsWorking){
			Ƈ_l=Ƈ_off;
			Ϟ=S11;
			}

			fuel+=cur/(max/100);
			++tanks;
			}}}
			var fill=(float)(fuel/tanks*1);

			if(C(т,h5))
			т=т.Replace(h5,(fill*1).ToString("F0"));

			if(C(т,h4))
			т=т.Replace(h4,Ϟ);

			if(C(т,j2))
			т=т.Replace(j2,CT);

			if(C(т,j1))
			т=т.Replace(j1,IP);

			if(C(ч,h5))
			Ϟ=(fill*1).ToString("F0");

			}
		else if(C(ч,ч5)){

			Ƈ_l=Ƈ_white;

			if(C(т,i1))
			т=т.Replace(i1,ғ);

			if(C(т,i2))
			т=т.Replace(i2,ś);

			if(C(т,b3)){
			if(Ѽ=="ѼT")
			т=т.Replace(b3,C1+"#"+ˠ);
			else
			т=т.Replace(b3,Ѩ);
			}
			}
		else if(C(ч,"bli")){
			if(Ĉ==1);
			else
			Ƈ_l=Ƈ_grey;
			}
		else if(C(ч,ч6)){
			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string Դ=Թ(ref ч),speed=Թ(ref ч),value=Թ(ref ч);


			if(C(т,h4))
			т=т.Replace(h4,SpeedText(speed,value));
			Ϟ=SpeedText(speed,value);

			Ƈ_l=Ƈ_white;
			}
			}
		else if(C(ч,ч7)){
			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string Դ=Թ(ref ч),alt=Թ(ref ч),ncor=Թ(ref ч);
			float cor = 0;

			if(!ncor.Contains("x"))
			cor=float.Parse(ncor);

			if(C(т,h4))
			т=т.Replace(h4,AltText(alt,cor));
			Ϟ=AltText(alt,cor);

			Ƈ_l=Ƈ_white;
			}
			}
		else if(C(ч,ч8)){
			Ƈ_l=Ƈ_white;

			horizon(ƪ,χ,Ӌ,ה,Ћ,Ь,π,т,Ş,Ϟ);
			т="";
			}
		else if(C(ч,ч9)){
			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string Դ=Թ(ref ч),time=Թ(ref ч);

			т+=DateTime.Now.ToString(time);

			Ϟ=DateTime.Now.ToString(time);
			Ƈ_l=Ƈ_white;
			}
			}
		
		else if(C(ч,ч10)){
			foreach(var β in Ț){

			if(C(β.CustomName,ή)){
			if(C(β.CustomName,ϡ)){

			var timeSpan=TimeSpan.FromSeconds(β.TriggerDelay);
			string hh=timeSpan.Hours.ToString("00"),
			mm=timeSpan.Minutes.ToString("00"),
			ss=timeSpan.Seconds.ToString("00"),
			delay=(hh+":"+mm+":"+ss),details,IF=β.DetailedInfo;
			int SS=IF.IndexOf(":");
			string Gdetails=IF.Substring(SS);
			details=Gdetails.Replace(": ","");

			if(β.IsWorking)
			Ƈ_l=Ƈ_on;
			if(!β.IsWorking)
			Ƈ_l=Ƈ_off;
			if(β.IsCountingDown){
			if(Ĉ==1 )
			Ƈ_l=Ƈ_grey;
			else
			Ƈ_l=Ƈ_lblue;

			if(C(т,h5))
			т=т.Replace(h5,(details.ToString()));
			
			if(C(т,h4))
			т=т.Replace(h4,(delay.ToString()));

			}
			}}}}

		else if(C(ч,ч11)){

			if(C(ч,"|")){
			ч=ч.Replace("|",",");

			string Դ=Թ(ref ч),nnr=Թ(ref ч);

			foreach(var β in Т){
			int nr=int.Parse(nnr);

			Ƈ_l=Ƈ_white;
			if(C(β.CustomName,ή)){

			var lcd=β as IMyTextSurfaceProvider;
			var SS = (lcd is IMyTextPanel)? (IMyTextSurface)lcd : lcd.GetSurface(nr);

			string тT=SS.GetText();

			т+=тT;
			}}}
			}

		if(C(Ƨ,"conv")){

			if(ψD.Contains(ή+",web")){

			var ЩCC=CD.Split('\n');
			ArrayList list=new ArrayList(ЩCC);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(ЩCC[i]);

			if(ψ.Contains(A30+" "+ή+",web")&&(C(Ƨ,"convƱ"))){
			ψD=ψD.Replace("\n"+ψ,"");
			}
			else if(ψ.Contains(A30+" "+ή+",web")){
			ψD=ψD.Replace("\n"+ψ,("\n"+A30+" "+ή+",web, ,"+Ѡχ+","+ѠӋ+","+ה+","+ѠЋ+","+ѠЬ+","+π+","+т+","+Ş+","+Ƈ_l.R+","+Ƈ_l.G+","+Ƈ_l.B+","+Ϟ).ToString());
			}
			}}
			else if (C(Ƨ,"convƱ"));
			else {
			ψD+=("\n"+A30+" "+ή+",web, ,"+Ѡχ+","+ѠӋ+","+ה+","+ѠЋ+","+ѠЬ+","+π+","+т+","+Ş+","+Ƈ_l.R+","+Ƈ_l.G+","+Ƈ_l.B+","+Ϟ).ToString();
			}
			}
		
		else{
		Object(ƪ,χ,Ӌ,ה,Ћ,Ь,π,т,Ş,Ƈ_l,Ϟ);
		}
		
		if(ήI!= " "){
			
			if(!Ƥ.Contains(ήI))
			Ƥ+=ήI+Ϟ+"\n";
			
			var Page=Ƥ.Split('\n');
			ArrayList listƤ=new ArrayList(Page);
			for(int i=0;i < listƤ.Count;i++){
			string ψ=Convert.ToString(Page[i]);

		 //refrech Memory
			if(ψ.Contains(ήI))
			Ƥ=Ƥ.Replace(ψ,ήI+Ϟ);
			}
		 	}
		}

	void Link(MySpriteDrawFrame ƪ,string Ƨ,float χ,float Ӌ,string ה,float Ћ,float Ь,float π,string ms,float AX,float AY){
		
		if(AӋ>(Ӌ-Ћ/2)&&AӋ<(Ӌ+Ћ/2)&&Aχ>(χ-Ь/2)&&Aχ<(χ+Ь/2)&&Щlcd==Ƨ|Ƨ==ms|ms.Contains(A58)&&Щlcd==Ƨ){
		ms=ms.Replace("|","");
		ms=ms.Replace(" "+A58,"");
		Щlcd=ms;
		Aχ=AX;
		AӋ=AY;
		}
		Object(ƪ,χ,Ӌ,ה,Ћ,Ь,π,"","1",Ƈ_Link,"Ϟ");
		}
	
	void Object(MySpriteDrawFrame ƪ,float χ,float Ӌ,string ה,float Ћ,float Ь,float π,string т,string Ş,Color Ƈ,string Ϟ){
		Ћ=Ћ/100*95;
		Ь=Ь/100*95;
		//compare memory
			bool c=Ϟ.Any(char.IsDigit);
			if(C(Ƥ,Ϟ)&&c==false&&Ϟ.Length>=2){

			var Page=Ƥ.Split('\n');
			ArrayList list=new ArrayList(Page);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(Page[i]);

			if(C(ψ,Ϟ)){
			bool a=ψ.Any(char.IsDigit);
			if(a==true){
			ψ=ψ.Replace(Ϟ,"");
			Ϟ=ψ;
			}}
			}}

		if(ה.Contains("SquareFilled"))
			Ξ(ƪ,χ,Ӌ,"SquareSimple",Ћ,Ь,π,Ƈ);

		if(ה.Contains("TriangleFilled"))
			Ξ(ƪ,χ,Ӌ,"Triangle",Ћ,Ь,π,Ƈ);
			
		if(ה.Contains("CircleFilled"))
			Ξ(ƪ,χ,Ӌ,"Circle",Ћ,Ь,π,Ƈ);

		if(ה.Contains("bar")){
			MySprite S=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ),new Vector2(Ь,Ћ));

			deg=float.Parse(Ϟ);

			if(C(ה,"|")){
			ה=ה.Replace("|",",");
			string ҕ1=Թ(ref ה),ҕ2=Թ(ref ה),ҕ3=Թ(ref ה);
			float B=float.Parse(ҕ2),R=float.Parse(ҕ3);

			S.RotationOrScale=(float)(Math.PI/180*(π));
			S.Color=Ƈ_white;
			ƪ.Add(S);

			S.RotationOrScale=(float)(Math.PI/180*(π));
			S.Size=new Vector2(Ь-2,Ћ-2);
			S.Color=Ƈ_grey;
			ƪ.Add(S);

			S.RotationOrScale=(float)(Math.PI/180*(π));
			S.Size=new Vector2(Ь-2,(Ћ/R)*deg-2);

			if(π==90|π==270)
			S.Position=new Vector2(χ-(Ћ/2-1+((Ћ/R)*B))+(((Ћ/2)/R)*deg),Ӌ);
			else
			S.Position=new Vector2(χ,Ӌ+(Ћ/2-1+((Ћ/R)*B))-(((Ћ/2)/R)*deg));


			S.Color=Ƈ;
			ƪ.Add(S);
			}
			}
		if(ה.Contains("gauges")){

			deg=float.Parse(Ϟ);

			MySprite Ɓ1=MySprite.CreateSprite("CircleHollow",new Vector2(χ,Ӌ),new Vector2(Ь,Ћ));
			MySprite Ɓ15=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ћ-4,Ь-4));
			MySprite Ɓ3=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ћ-4,Ь-4));

			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ),new Vector2(Ћ,2));
			MySprite Ɓ5=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(4,Ь));

			MySprite Ɓ12=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ћ/20*15,Ь/20*15));

			MySprite ƥ1=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь/20*1,Ћ));
			MySprite Ƈ1=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь-4,Ћ-4));
			MySprite Ƈ2=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь-4,Ћ-4));

			if(ה.Contains("1+")){
			Ɓ3.RotationOrScale=(float)(Math.PI/180*(π+202));
			ƥ1.RotationOrScale=(float)(Math.PI/180*(π+111-(deg*2.2)));
			Ƈ1.RotationOrScale=(float)(Math.PI/180*(π+201-(deg*2.2)));
			if (deg>80){
			Ƈ1.RotationOrScale=(float)(Math.PI/180*(π-337));
			Ƈ2.RotationOrScale=(float)(Math.PI/180*(π+201-(deg*2.2)));
			}
			}
			else if(ה.Contains("1-")){
			Ɓ3.RotationOrScale=(float)(Math.PI/180*(π+158));
			ƥ1.RotationOrScale=(float)(Math.PI/180*(π-111+(deg*2.2)));
			Ƈ1.RotationOrScale=(float)(Math.PI/180*(π-201+(deg*2.2)));
			if (deg>80){
			Ƈ1.RotationOrScale=(float)(Math.PI/180*(π+337));
			Ƈ2.RotationOrScale=(float)(Math.PI/180*(π-201+(deg*2.2)));
			}
			}
			else{
			ƥ1.RotationOrScale=(float)(Math.PI/180*(π-0+(deg*1)));
			
			}
			Ɓ1.Color=Ƈ_gauges;
			Ɓ2.Color=Ƈ_gauges;
			Ɓ5.Color=Ƈ_gauges;
			Ɓ3.Color=Ƈ_gaugeback;
			Ɓ12.Color=Ƈ_gaugeback;

			ƥ1.Color=Ƈ_ƥointer;
			Ƈ1.Color=Ƈ;
			Ƈ2.Color=Ƈ;
			Ɓ15.Color=Ƈ_gaugeback;

			ƪ.Add(Ɓ1);
			ƪ.Add(Ɓ15);

			if(!ה.Contains("1")){
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π+90));
			ƪ.Add(Ɓ2);
			}
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π+180));
			ƪ.Add(Ɓ2);
			if(ה.Contains("1")){
			if(!ה.Contains("No C")){
			ƪ.Add(Ƈ1);
			ƪ.Add(Ɓ3);
			if (deg>80)
			ƪ.Add(Ƈ2);
			}

			Ɓ5.RotationOrScale=(float)(Math.PI/180*(π+45));
			ƪ.Add(Ɓ5);
			Ɓ5.RotationOrScale=(float)(Math.PI/180*(π-45));
			ƪ.Add(Ɓ5);
			Ɓ5.RotationOrScale=(float)(Math.PI/180*(π-22));
			ƪ.Add(Ɓ5);
			Ɓ5.RotationOrScale=(float)(Math.PI/180*(π+22));
			ƪ.Add(Ɓ5);
			Ɓ5.RotationOrScale=(float)(Math.PI/180*(π));
			ƪ.Add(Ɓ5);

			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π+180));
			ƪ.Add(Ɓ2);
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π-22));
			ƪ.Add(Ɓ2);
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π+22));
			ƪ.Add(Ɓ2);
			}
			ƪ.Add(Ɓ12);

			if(!ה.Contains("No P"))
			ƪ.Add(ƥ1);
			}
		if(ה.Contains("pointer")){
			if(Ϟ.Contains(S1))
			deg=0;
			else
			deg=float.Parse(Ϟ);

			if(ה.Contains("|"))
			ה=ה.Replace("|",",");
			string link=Թ(ref ה),colR=Թ(ref ה),colG=Թ(ref ה),colB=Թ(ref ה);
			int CR,CG,CB;
			

			if(!colR.Contains("x")){
			CR=int.Parse(colR);
			CG=int.Parse(colG);
			CB=int.Parse(colB);
			Ƈ=new Color(CR,CG,CB);
			}
			
			MySprite ƥ1=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь/20*1,Ћ));

			if(ה.Contains("1+"))
			ƥ1.RotationOrScale=(float)(Math.PI/180*(π+111-(deg*2.2)));
			else if(ה.Contains("1-"))
			ƥ1.RotationOrScale=(float)(Math.PI/180*(π-111+(deg*2.2)));
			else
			ƥ1.RotationOrScale=(float)(Math.PI/180*(π-0+(deg*1)));
			ƥ1.Color=Ƈ;
			ƪ.Add(ƥ1);
			}
		if(ה=="spot"){

			MySprite Ɓ1=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь,Ћ));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ-(Ь/2*1)+(Ь/8*2),Ӌ-(Ћ/2*1)+(Ћ/8*2)),new Vector2(Ь/15*5,Ћ/10*1));

			Ɓ1.RotationOrScale=(float)(Math.PI/180*(90));
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(-25));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ;

			ƪ.Add(Ɓ1);
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ-(Ь/2*1)+(Ь/8*2),Ӌ-(Ћ/2*1)+(Ћ/8*4));
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ-(Ь/2*1)+(Ь/8*2),Ӌ-(Ћ/2*1)+(Ћ/8*6));
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ-(Ь/2*1)+(Ь/8*2),Ӌ-(Ћ/2*1)+(Ћ/8*8));
			ƪ.Add(Ɓ2);
			}

		if(ה=="off"){

			MySprite Ɓ1=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ь/10*9,Ћ/10*9));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ/10*9 ),new Vector2(Ь/10*2,Ћ/10*8));
			Ɓ1.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π));

			Ɓ1.Color=Ƈ;
			ƪ.Add(Ɓ1);
			Ɓ1.Size=new Vector2(Ь/10*8,Ћ/10*8);
			Ɓ1.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ1);

			Ɓ2.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ2);
			Ɓ2.Size=new Vector2(Ь/10*1,Ћ/10*6);
			Ɓ2.Color=Ƈ;
			ƪ.Add(Ɓ2);
			}

		if(ה=="ant"){

			MySprite Ɓ1=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь/10*9,Ћ/10*9));
			MySprite Ɓ2=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ь/10*6,Ћ/10*6));
			MySprite Ɓ3;
			if(π==180)
			Ɓ3=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ/10*10 ),new Vector2(Ь/10*2,Ћ/10*2));
			else
			Ɓ3=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ/10*11 ),new Vector2(Ь/10*2,Ћ/10*2));

			Ɓ1.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ3.RotationOrScale=(float)(Math.PI/180*(π));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ_ObjBack;
			Ɓ3.Color=Ƈ;

			ƪ.Add(Ɓ1);
			Ɓ1.Color=Ƈ_ObjBack;
			Ɓ1.Size=new Vector2(Ь/10*8,Ћ/10*8);
			ƪ.Add(Ɓ1);
			Ɓ1.Color=Ƈ;
			Ɓ1.Size=new Vector2(Ь/10*7,Ћ/10*7);
			ƪ.Add(Ɓ1);

			ƪ.Add(Ɓ2);
			ƪ.Add(Ɓ3);
			}

		if(ה=="proj"){

			MySprite Ɓ1=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ь/10*7,Ћ/10*7));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ),new Vector2(Ь,Ћ/10*2));
			MySprite Ɓ3=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ь/10*4,Ћ/10*4));

			Ɓ1.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ3.RotationOrScale=(float)(Math.PI/180*(π));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ;
			Ɓ3.Color=Ƈ_ObjBack;

			ƪ.Add(Ɓ1);
			ƪ.Add(Ɓ2);
			Ɓ2.Size=new Vector2(Ь/10*2,Ћ);
			ƪ.Add(Ɓ2);
			ƪ.Add(Ɓ3);
			Ɓ3.Size=new Vector2(Ь/10*2,Ћ/10*2);
			Ɓ3.Color=Ƈ;
			ƪ.Add(Ɓ3);
			}

		if(ה=="conn"){

			MySprite Ɓ1=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ),new Vector2(Ь/10*9,Ћ/10*9));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ),new Vector2(Ь/10*2,Ћ/10*8));
		
			Ɓ1.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ_ObjBack;

			ƪ.Add(Ɓ1);
			
			ƪ.Add(Ɓ2);
			Ɓ2.Size=new Vector2(Ь/10*1,Ћ/10*6);
			Ɓ2.Color=Ƈ;
			ƪ.Add(Ɓ2);
			}

		if(ה=="land"){

			MySprite Ɓ1=MySprite.CreateSprite("SemiCircle",new Vector2(χ,Ӌ),new Vector2(Ь/10*5,Ћ/10*5));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/10*7) ),new Vector2(Ь/10*6,Ћ/10*1));
		
			Ɓ1.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ2.RotationOrScale=(float)(Math.PI/180*(π));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ;

			ƪ.Add(Ɓ1);
			ƪ.Add(Ɓ2);
			}

		if(ה=="gun"){
			MySprite Ɓ1=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/10*3)),new Vector2(Ь/20*17,Ћ/30*3));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ-(Ћ/2*1)+(Ћ/20*8),Ӌ),new Vector2(Ь/20*1,Ћ/10*7));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ;

			ƪ.Add(Ɓ1);
			Ɓ1.Size=new Vector2(Ь/20*12,Ћ/30*2);
			Ɓ1.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ1);

			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ-(Ћ/2*1)+(Ћ/20*12),Ӌ);
			ƪ.Add(Ɓ2);
			Ɓ2.Size=new Vector2(Ь/20*1,Ћ/10*8);

			Ɓ2.Position=new Vector2(χ-(Ћ/2*1)+(Ћ/20*9),Ӌ);
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ-(Ћ/2*1)+(Ћ/20*11),Ӌ);
			ƪ.Add(Ɓ2);

			Ɓ1.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/20*15));
			Ɓ1.Size=new Vector2(Ь/20*17,Ћ/30*3);
			Ɓ1.Color=Ƈ;
			ƪ.Add(Ɓ1);
			Ɓ1.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/20*16));
			ƪ.Add(Ɓ1);
			Ɓ1.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/20*17));
			ƪ.Add(Ɓ1);
			Ɓ1.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/20*18));
			ƪ.Add(Ɓ1);
			}

		if(ה=="rock"){

			MySprite Ɓ1=MySprite.CreateSprite("SquareSimple",new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/10*5) ),new Vector2(Ь/20*7,Ћ/10*5));
			MySprite Ɓ2=MySprite.CreateSprite("Circle",new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/10*8) ),new Vector2(Ь/20*7,Ћ/30*2));
			MySprite Ɓ3=MySprite.CreateSprite("Triangle",new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/7*1) ),new Vector2(Ь/20*7,Ћ/10*2));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ;
			Ɓ3.Color=Ƈ;

			ƪ.Add(Ɓ1);
			ƪ.Add(Ɓ3);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*20));
			Ɓ2.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*21));
			Ɓ2.Color=Ƈ;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*22));
			Ɓ2.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*23));
			Ɓ2.Color=Ƈ;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*37));
			Ɓ2.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*38));
			Ɓ2.Color=Ƈ;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*39));
			Ɓ2.Color=Ƈ_ObjBack;
			ƪ.Add(Ɓ2);
			Ɓ2.Position=new Vector2(χ,Ӌ-(Ћ/2*1)+(Ћ/50*40));
			Ɓ2.Color=Ƈ;
			ƪ.Add(Ɓ2);
			}
		if(ה=="engine"){

			MySprite Ɓ1=MySprite.CreateSprite("SquareHollow",new Vector2(χ-(Ь/2*1)+(Ь/10*6),Ӌ),new Vector2(Ь/20*10,Ћ/20*7));
			MySprite Ɓ2=MySprite.CreateSprite("SquareSimple",new Vector2(χ-(Ь/2*1)+(Ь/10*2),Ӌ),new Vector2(Ь/20*1,Ћ/10*3));
			MySprite Ɓ3=MySprite.CreateSprite("SquareSimple",new Vector2(χ-(Ь/2*1)+(Ь/10*3),Ӌ),new Vector2(Ь/10*1,Ћ/20*1));

			Ɓ1.Color=Ƈ;
			Ɓ2.Color=Ƈ;
			Ɓ3.Color=Ƈ;

			ƪ.Add(Ɓ1);
			ƪ.Add(Ɓ2);
			ƪ.Add(Ɓ3);
			
			}
		else{
			MySprite Ɓ=MySprite.CreateSprite(ה,new Vector2(χ,Ӌ),new Vector2(Ь,Ћ));

			Ɓ.RotationOrScale=(float)(Math.PI/180*(π));
			Ɓ.Color=Ƈ;
			ƪ.Add(Ɓ);
		}
		//Text size, color,offset
			var тA=TextAlignment.CENTER;
			float тŞ;

			if(C(т,h4))
			т=т.Replace(h4,"0");
			if(C(т,"#"))
			т=т.Replace("#","\n");

			if(Ş.Contains("|")){
			Ş=Ş.Replace("|",",");

			int CR=0,CG=0,CB=0;
			string ттŞ=Թ(ref Ş),ƇR=Թ(ref Ş),ƇG=Թ(ref Ş),ƇB=Թ(ref Ş),ттA=Թ(ref Ş);

			if(ттA.Contains("CENTER")|ттA.Contains(""))
			тA=TextAlignment.CENTER;
			if(ттA.Contains("LEFT"))
			тA=TextAlignment.LEFT;
			if(ттA.Contains("RIGHT"))
			тA=TextAlignment.RIGHT;

			CR=int.Parse(ƇR);
			CG=int.Parse(ƇG);
			CB=int.Parse(ƇB);
			Ƈ=new Color(CR,CG,CB);
			тŞ=float.Parse(ттŞ);
			}
			else {
			тA=TextAlignment.CENTER;
			тŞ=float.Parse(Ş);
			}

		// add text
			MySprite Text1=MySprite.CreateText(т,"Debug",Ƈ,тŞ,тA);

			if(тA==TextAlignment.CENTER)
			Text1.Position=new Vector2(χ,Ӌ-(Ћ/2));
			if(тA==TextAlignment.LEFT)
			Text1.Position=new Vector2(χ-(Ь/2),Ӌ-(Ћ/2));
			if(тA==TextAlignment.RIGHT)
			Text1.Position=new Vector2(χ+(Ь/2),Ӌ-(Ћ/2));

			ƪ.Add(Text1);
			

		}

	void horizon(MySpriteDrawFrame ƪ,float χ,float Ӌ,string ה,float Ћ,float Ь,float π,string т,string Ş,string ч){

		Vector2 ЬЋ=new Vector2(Ь,Ћ);
		Vector2 χӋ=new Vector2(χ,Ӌ)-ЬЋ/2;

		MySprite B;
		MySprite N;
		MySprite A;

		B=MySprite.CreateSprite("Circle",χӋ+ЬЋ / 2,ЬЋ);
		B.Color=Ƈ_white;
		ƪ.Add(B);
		B.Color=Ƈ_gaugeback;
		B.Size=ЬЋ * 0.9f;
		ƪ.Add(B);
		
		if (C(ה,hA)){//level
			N=MySprite.CreateSprite("CircleHollow",χӋ+ЬЋ / 2,new Vector2(ЬЋ.X * 0.5f,ЬЋ.Y * 0.5f));
			A=MySprite.CreateSprite("Circlehollow",χӋ+ЬЋ / 2,new Vector2(ЬЋ.X * 0.5f,ЬЋ.Y * 0.5f));

			if(ȣ.GetNaturalGravity().LengthSquared() !=0){
			Vector2D R=Rot("N");
			Vector2 W=new Vector2(-(float)(Math.Cos(R.Y * Math.PI / 180d) * R.X *2),-(float)(Math.Cos(R.X * Math.PI / 180d) * R.Y *2));
			N.Position=(χӋ+ЬЋ / 2+W);
			}

			if(ȣ.GetArtificialGravity().LengthSquared() !=0){
			Vector2D R=Rot("A");
			Vector2 W=new Vector2(-(float)(Math.Cos(R.Y * Math.PI / 180d) * R.X *2),-(float)(Math.Cos(R.X * Math.PI / 180d) * R.Y *2));
			A.Position=(χӋ+ЬЋ / 2+W);
			}
		}
		else{//horizon
			N=MySprite.CreateSprite("Triangle",χӋ+ЬЋ / 2,new Vector2(ЬЋ.X * 1.1f,ЬЋ.Y *0.1f));
			A=MySprite.CreateSprite("Triangle",χӋ+ЬЋ / 2,new Vector2(ЬЋ.X * 1.1f,ЬЋ.Y *0.1f ));

			if(ȣ.GetNaturalGravity().LengthSquared() !=0){
			Vector2D R=Rot("N");
			Vector2 W=new Vector2(0,-(float)(Math.Cos(R.X * Math.PI / 180d) * R.Y / 180f * 35f ));
			N.Position=(χӋ+ЬЋ / 2+W);
			N.RotationOrScale=(float)R.X / 180f * (float)Math.PI;
			}

			if(ȣ.GetArtificialGravity().LengthSquared() !=0){
			Vector2D R=Rot("A");
			Vector2 W=new Vector2(0,-(float)(Math.Cos(R.X * Math.PI / 180d) * R.Y / 180f * 35f ));
			A.Position=(χӋ+ЬЋ / 2+W);
			A.RotationOrScale=(float)R.X / 180f * (float)Math.PI;
			}
		}

		N.Color=Ƈ_red;
		A.Color=Ƈ_yellow;

		if(ȣ.GetNaturalGravity().LengthSquared() !=0){
			Vector2D R=Rot("N");
			if(R.Y < 140 & -140 < R.Y)
			ƪ.Add(N);
			}

		if(ȣ.GetArtificialGravity().LengthSquared() !=0){
			Vector2D R=Rot("A");
			if(R.Y < 140 & -140 < R.Y)
			ƪ.Add(A);
			}

		double nat=ȣ.GetNaturalGravity().LengthSquared()/100;
		double art=ȣ.GetArtificialGravity().LengthSquared()/100;

		if(C(т,h0))
		т=т.Replace(h0,nat.ToString("F1"));
		if(C(т,h9))
		т=т.Replace(h9,art.ToString("F1"));

		Object(ƪ,χ,Ӌ,ה,Ћ,Ь,π,т,Ş,Ƈ_white,Ϟ);
		}

//Main
	Program(){
		Runtime.UpdateFrequency=UpdateFrequency.Update1;
		state=RunStuffOverTime();
		Ƥ=Storage;
		}

	void Save()
		{
		Storage=Ƥ+B1+"++ 0\n";
		}

	void Main(string ҕ){

		if (Т.Count<=0){
		ȣCs();
		Gβ();
		}

		if(!ȣC.IsUnderControl && ȣR.IsUnderControl){
		ȣ=ȣR;
		}
		else {
		ȣ=ȣC;
		}

		ǂ="";
		ǈ="";
		RunStateMachine();
		SleepM(ҕ);
		com(ҕ);
		search(ҕ);
		conn(ϡ);
		rot(ϡ);
		MouseC(ȣ,false);
		
	// MOUSE
		if(C(ҕ,A8)){
		if(ЩЩ==true){
		ЩЩ=false;
		}
		else if(ЩЩ==false){
		ЩЩ=true;
		}}

	// LOG
		timercheck(out isPRun);
			Ø=" ";
		if(!Me.CustomName.Contains("-COM"))
			Ø+="\n----- COMBINED DATA-----\n"+Ø4+Ø5;

		if(!Me.CustomName.Contains("-COU")){
			Ø+="\n\n ----- COUNT ----- ";
			Ø+="\nTotal itemname block = "+Т.Count;
			Ø+="\nTotal controller block = "+Ƀ.Count;
			Ø+="\nmax. Instruction = "+Runtime.MaxInstructionCount;
			Ø+="\ncur. Instruction = "+Runtime.CurrentInstructionCount;
			}
		if(!Me.CustomName.Contains("-SET")){
			Ø+="\n\n----- SETTINGS -----\n";
			Ø+="\n----- TAGS -----";
			Ø+="\n"+A5+" = "+ϡ;
			Ø+="\n"+A25+" = "+ƧT;
			Ø+="\n"+A6+" = "+cockpit;
			Ø+="\n"+A7+" = "+remote;
			Ø+="\n\n----- CURRENT CONTROLER -----";
			Ø+="\nController = "+ȣ.CustomName;
			Ø+="\nCockpit = "+ȣC.CustomName;
			Ø+="\nRemote = "+ȣR.CustomName;
			Ø+="\n\n----- MOUSE -----";
			Ø+="\nmouse lcd = "+Щlcd;
			Ø+="\n"+A9+" = "+ЩЩ;
			Ø+="\n"+A10+" = "+HsЩ;
			Ø+="\n\n----- ADDITIONAL -----";
			Ø+="\n"+A12+" = "+slĈ;
			Ø+="\nupdate freq.= "+Runtime.UpdateFrequency;
			Ø+="\n"+A56+" = "+K;
			Ø+="\n\n----- IGC -----";
			Ø+="\n"+A14+" = "+ғ;
			Ø+="\n"+A15+" = "+ś;
			}
		if(!Me.CustomName.Contains("-IGC"))
			Ø+="\n\n-----IGC CHANNELS-----\n"+ˠ+"\n"+Ѩ;
		if(!Me.CustomName.Contains("-MEM"))
			Ø+="\n\n----- MEMORY -----\n"+Ƥ;
		if(!Me.CustomName.Contains("-LOG"))
			Ø+="\n\n----- LOGIC -----\n"+ǈ;
		if(!Me.CustomName.Contains("-ERR"))
			Ø+="\n\n----- ERRORS -----\n"+ǂ;

		name=title+"\n\n"+dev;

		if(slmode==true){
		Echo(Sl+"\n"+name);
		}
		else{
		Echo(RunSym()+"\n"+name);

		if(Me.CustomName.Contains("log")){
		Ø=Ø.Replace("[","[[");
		Ø=Ø.Replace("]","]]");
		Echo(Ø);
		}
		Echo(isPRun);
		}
	}

//
	bool Logic(string ƤƤ){
		string lo="",ҕ1="",ҕ2,ҕ3;
		float f1=0,f2=0;
		
		if (C(ƤƤ,"|")){
			var ЩCC=ƤƤ.Split('|');
			ArrayList list=new ArrayList(ЩCC);
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(ЩCC[i]);

		if(C(ψ,">")|C(ψ,"<")|C(ψ,"=")){
			if(C(ψ,">"))
			ҕ1=">";
			if(C(ψ,"<"))
			ҕ1="<";
			if(C(ψ,"="))
			ҕ1="=";

			ψ=ψ.Replace(">",",");
			ψ=ψ.Replace("<",",");
			ψ=ψ.Replace("=",",");

			ҕ2=Թ(ref ψ);
			ҕ3=Թ(ref ψ);

			bool a=ҕ3.Any(char.IsDigit);
			if(a==true) f1=float.Parse(ҕ3);

			var Page=Ƥ.Split('\n');
			ArrayList Ƥlist=new ArrayList(Page);
			for(int Ƥi=0;Ƥi < Ƥlist.Count;Ƥi++){
			string Ƥψ=Convert.ToString(Page[Ƥi]);

			if(Ƥψ.Contains(ҕ3)&&a==false){
			Ƥψ=Ƥψ.Replace(ҕ3,"");
			f1=float.Parse(Ƥψ);
			}

			if(Ƥψ.Contains(ҕ2)){
			Ƥψ=Ƥψ.Replace(ҕ2,"");
			f2=float.Parse(Ƥψ);
			}}

			if(ҕ1.Contains(">")&&(f2>f1))
			lo+="1";
			else if(ҕ1.Contains("<")&&(f2<f1))
			lo+="1";
			else if(ҕ1.Contains("=")&&(f2==f1))
			lo+="1";
			else
			lo+="0";
			}
		else if(i!=0&&Ƥ.Contains(ψ))
			lo+="1";
		else if(i!=0&&!Ƥ.Contains(ψ))
			lo+="0";
		
		}
		ƤƤ=ƤƤ.Replace("|","\n");
		ǈ+=ƤƤ+"\n = "+lo+"\n\n";

		if(C(ƤƤ,"XOR")&&lo.Contains("1")&&lo.Contains("0"))
			return true;
		else if(C(ƤƤ," NOR")&&lo.Contains("0"))
			return true;
		else if(C(ƤƤ,"XNOR")&&lo.Contains("0")&&!lo.Contains("1"))
			return true;
		else if(C(ƤƤ,"XNOR")&&lo.Contains("1")&&!lo.Contains("0"))
			return true;
		else if(C(ƤƤ," OR")&&lo.Contains("1"))
			return true;
		else if(C(ƤƤ,"NAND")&&lo.Contains("0")&&!lo.Contains("1"))
			return true;
		else if(C(ƤƤ," AND")&&lo.Contains("1")&&!lo.Contains("0"))
			return true;
		else 
			return false;
			}
		else if (Ƥ.Contains(ƤƤ))
			return true;
		else
			return false;
		
		}


	string SpeedText(string speedu,string value){
		float speed=(float)ȣ.GetShipSpeed();
		if(value =="km/h")
		return (speed* 3.6).ToString(speedu);
		else
		return (speed* 1).ToString(speedu);
		}

	

	string AltText(string altu,float cor){
		double alt=0;
		ȣ.TryGetPlanetElevation(MyPlanetElevation.Surface,out alt);
		return (alt * 1f-cor).ToString(altu);
		}

	void Gβ(){
		GridTerminalSystem.SearchBlocksOfName(ϡ,Т);
		GridTerminalSystem.GetBlocksOfType(Т1);
		GridTerminalSystem.GetBlocksOfType(в);
		GridTerminalSystem.GetBlocksOfType(Ƿ);
		GridTerminalSystem.GetBlocksOfType(Ф);
		GridTerminalSystem.GetBlocksOfType(ҏ);
		GridTerminalSystem.GetBlocksOfType(Ɔ);
		GridTerminalSystem.GetBlocksOfType(ĉ);
		GridTerminalSystem.GetBlocksOfType(ǵ);
		GridTerminalSystem.GetBlocksOfType(ϙ);
		GridTerminalSystem.GetBlocksOfType(Ŕ);
		GridTerminalSystem.GetBlocksOfType(ϥ);
		GridTerminalSystem.GetBlocksOfType(ώ);
		GridTerminalSystem.GetBlocksOfType(Ð);
		GridTerminalSystem.GetBlocksOfType(α);
		GridTerminalSystem.GetBlocksOfType(τ);
		GridTerminalSystem.GetBlocksOfType(Ș);
		GridTerminalSystem.GetBlocksOfType(һ);
		GridTerminalSystem.GetBlocksOfType(Ď);
		GridTerminalSystem.GetBlocksOfType(φ);
		GridTerminalSystem.GetBlocksOfType(Ŀ);
		GridTerminalSystem.GetBlocksOfType(ʘ);
		GridTerminalSystem.GetBlocksOfType(þ);
		GridTerminalSystem.GetBlocksOfType(Ă);
		GridTerminalSystem.GetBlocksOfType(Ț);
		GridTerminalSystem.GetBlocksOfType(ш);
		}
	void ȣCs(){
		GridTerminalSystem.GetBlocksOfType<IMyShipController>(Ƀ);
		for(int i=0;i< Ƀ.Count;i++){

		ȣi=Ƀ[i] as IMyShipController;

		if(ȣi.CustomName.Contains(cockpit ))
		ȣC=ȣi;
		
		if(ȣi.CustomName.Contains(remote ))
		ȣR=ȣi;
		}}
	bool C(string ч,string i){
		if(ч.Contains(i))
		return true;
		else
		return false;
		}

	void Ξ(MySpriteDrawFrame ƪ,float χ,float Ӌ,string ה,float Ћ,float Ь,float π,Color Ƈ){
		MySprite Ɓ1=MySprite.CreateSprite(ה,new Vector2(χ,Ӌ),new Vector2(Ь,Ћ));
		MySprite Ɓ2=MySprite.CreateSprite(ה,new Vector2(χ,Ӌ),new Vector2(Ь-2,Ћ-2));
		Ɓ1.RotationOrScale=(float)(Math.PI/180*(π));
		Ɓ2.RotationOrScale=(float)(Math.PI/180*(π));
		Ɓ1.Color=Ƈ;
		Ɓ2.Color=Ƈ_ObjBack;
		ƪ.Add(Ɓ1);
		ƪ.Add(Ɓ2);
		}
	string Թ(ref string ر){
		string str=ر;
		ر=" ";
		int i=str.IndexOf(',');
		if(i < 1) return str;
		ر=str.Substring(i+1);
		str=str.Substring(0,i);
		return str;
		}
	void Draw_Mouse_Arrow(MySpriteDrawFrame ƪ,float χ,float Ӌ,Color Ƈ){
		MySprite sprite=MySprite.CreateSprite("Triangle",new Vector2(χ,Ӌ),new Vector2(10,12));
		sprite.RotationOrScale=(float)(Math.PI/(-4));
		sprite.Color=Ƈ;
		ƪ.Add(sprite);
		MySprite sprite2=MySprite.CreateSprite("SquareSimple",new Vector2(χ+5,Ӌ+5),new Vector2(2,10));
		sprite2.RotationOrScale=(float)(Math.PI/(-4));
		sprite2.Color=Ƈ;
		ƪ.Add(sprite2);
		}
	void conn(string ϡ){
		foreach(var β in ĉ){
		
		if(C(β.CustomName,ϡ)){

		if(β.Status==MyShipConnectorStatus.Connectable&&β.CollectAll==false)
		β.ToggleConnect();

		}}
		}

	void rot(string ϡ){
		foreach(var β in Ŕ){

		if(C(β.CustomName,ϡ)){

		if(β.IsAttached&&C(β.CustomName,"[Detach]")){
		β.Detach();
		β.CustomName=β.CustomName.Replace("[Detach]","");
		}
		if(β.IsAttached==false&&C(β.CustomName,"[Attach]")){
		β.Attach();
		β.CustomName=β.CustomName.Replace("[Attach]","");
		}

		}}}
	public Vector2D Rot(string ر){
		Vector3D G;
		if (C(ر,"N"))
		G=ȣ.GetNaturalGravity();
		else
		G=ȣ.GetArtificialGravity();

		G.Normalize();

		Vector3D F=ȣ.WorldMatrix.Forward;
		Vector3D R=ȣ.WorldMatrix.Right;
		Vector3D U=ȣ.WorldMatrix.Up;
		F.Normalize();
		R.Normalize();
		U.Normalize();

		bool Ώ=false;
		if(Vector3D.Dot(G,U) >=0) Ώ=true;

		Vector3D GF=Vector3D.Cross(G,F);

		double P=0;
		if(Ώ) P=(-angle(G,F)+90);
		else P=(angle(G,F) - 90);
		double B=0;
		B=-angle(GF,R) * Math.Sign(Vector3D.Dot(U,GF));
		if(Ώ) if(P > 35 | -35 > P) B=0;
		else if(P > 35 | -35 > P) B=180;

		P *=Math.Sign(Vector3.Dot(G,-U));
		return new Vector2D(B,P);

		}

	double angle(Vector3D v1,Vector3D v2)
		{
		v1.Normalize();
		v2.Normalize();
		double angleRAD=Math.Acos(Vector3D.Dot(v1,v2));
		return angleRAD * 180 / Math.PI;
		}
	void com(string ҕ){
		if(ғ=="")
		ғ=Me.CubeGrid.CustomName;
		Ѡ="";

		var ЩCC=Me.CustomData.Split('\n');
		ArrayList list=new ArrayList(ЩCC);
		for(int i=0;i < list.Count;i++){
		string ψ=Convert.ToString(ЩCC[i]);
	 // AUTO REFRECH
		if(ψ.Contains(A20+".")){
			string www=Թ(ref ψ),Ƨ=Թ(ref ψ);
			if(Ĉ==1 ){
			ҕ=A20+"@"+Ƨ;
			}}
	 // CUSTOMDATA INPUT
		if(!ψ.Contains("//")){
		if(ψ.Contains(A14+",")){
			nϡ=Թ(ref ψ);
			ғ=Թ(ref ψ);
			}
		if(ψ.Contains(A15+",")){
			nϡ=Թ(ref ψ);
			ś=Թ(ref ψ);
			}
	 // ADD OBJECTS TO BE SENT
		if(ψ.Contains("web")){
			if(!Ѡ.Contains(ψ))
			Ѡ+="\n"+A20+"."+ψ+"";
			}
		}}
	 // ADD SELECTION TO LIST
		if(!ˠ.Contains(ś))
			ˠ+="\n"+ś;
	 // PERSONAL CHANNEL
		β1=IGC.RegisterBroadcastListener(ғ);
	 // EVERYONE CHANNEL
		β2=IGC.RegisterBroadcastListener(Œ);
	 // ARGUMENTS
		if(ҕ==A16|ҕ==A17)
			Ϛ=true;

			var ϥ=ˠ.Split('\n');
			ArrayList Ч=new ArrayList(ϥ);
			for(int i=0;i < Ч.Count;i++){
			string Ϸ=Ч[i].ToString();
		
		if(ҕ==A16){
			if(Ϸ==ś&& Ϛ==true){
			if(i+1 < Ч.Count)
			ś=Ч[i+1].ToString();
			Ϛ=false;
			Ѽ="ѼT";
			}
			}
		else if(ҕ==A17){
			if(Ϸ==ś&&Ϛ==true){
			if(i-1>-1)
			ś=Ч[i-1].ToString();
			Ϛ=false;
			Ѽ="ѼT";
			}
			}
		}
		if(ҕ==A16);
		else if(ҕ==A17);
		else if(ҕ==A18){
			ˠ=Œ;
			ś=Œ;
			IGC.SendBroadcastMessage(Œ,ғ+","+ҕ);
			Ѩ=C2+":\n";
			Ѽ="ѼT";
			}
		
		else if(ҕ==A21){
			if(Ѽ=="ѼT")
			Ѽ="";
			else
			Ѽ="ѼT";
			}
		else if(ҕ==A19){
			if(Me.CustomData.Contains(A20+"."))
			Me.CustomData=Me.CustomData.Replace(A20+".","Ʊ");
			else{
			if(ҕ==A19+A3){
			ҭ=ś;
			IGC.SendBroadcastMessage(ҭ,ғ+","+ҕ);
			}

			}
			Ѩ="";
			}
		else if(ҕ==A20+"|"){
			ҭ=ś;
			IGC.SendBroadcastMessage(ҭ,ғ+","+ҕ);
			Ѩ=C3+A20+":\n "+C6+": "+ҭ+"\n "+C4+": "+ҕ+"\n";
			Ѽ="";
			}
		else if(C(ҕ,A11+",")){
			nϡ=Թ(ref ҕ);
			string ҕ2=Թ(ref ҕ);
			if ((C(Ƥ,ҕ2)))
			Ƥ=Ƥ.Replace(ҕ2+"\n","");
			else 
			Ƥ+=ҕ2+"\n";
			}
		else if(C(ҕ,"ӆ")){
			ҭ=ś;
			IGC.SendBroadcastMessage(ҭ,ғ+","+ҕ);
			Ѩ=C4+" "+C5+":\n "+C6+": "+ҭ+"\n";
			Ѽ="";
			}
		else if(ҕ==A8);
		else if(ҕ !=""){
			ҭ=ś;
			IGC.SendBroadcastMessage(ҭ,ғ+","+ҕ);
			Ѩ=C3+" "+C7+":\n "+C6+": "+ҭ+"\n "+C4+": "+ҕ+"\n";
			Ѽ="";
			}
		

	 // WHAT TO DO WEN MASSAGE
		while (β2.HasPendingMessage){
			MyIGCMessage MѨ=β2.AcceptMessage();
			if(MѨ.Tag==Œ){

			if(MѨ.Data is string){
			string str=MѨ.Data.ToString(),Ѱ=Թ(ref str),ɗ=Թ(ref str);

			if(!ˠ.Contains(Ѱ))
			ˠ+="\n"+Ѱ;

			if(ɗ.Contains(A18))
			IGC.SendBroadcastMessage(Ѱ,ғ+","+C18);
			else if(ɗ.Contains(A19))
			Ѩ="";
			else{
			Ѩ=C10+"\n";
			Ѩ+=C8+": "+Ѱ+"\n";
			Ѩ+=C6+": "+MѨ.Tag+"\n";
			Ѩ+=C4+": "+ɗ+"\n";
			IGC.SendBroadcastMessage(Ѱ,ғ+C11+ɗ);
			}

		}}}

		while (β1.HasPendingMessage){
			MyIGCMessage MѨ=β1.AcceptMessage();
			if(MѨ.Tag==ғ){

			if(MѨ.Data is string){
			string str=MѨ.Data.ToString(),Ѱ=Թ(ref str),ɗ=Թ(ref str);

			if(!ˠ.Contains(Ѱ))
			ˠ+="\n"+Ѱ;
			
			if(ɗ.Contains(C17)){
			if(!Ѩ.Contains(C8+": "+Ѱ+". "+ɗ+"\n"))
			Ѩ+=C8+": "+Ѱ+". "+ɗ+"\n";
			Ѱ="";
			}

			else if(ɗ.Contains(C18)){
			if(!Ѩ.Contains(C8+": "+Ѱ+". "+ɗ+"\n"))
			Ѩ+="From: "+Ѱ+". "+ɗ+"\n";
			Ѱ="";
			}

			else if(ɗ.Contains(A19))
			Ѩ="";

			else if(ɗ.Contains(A20+"@")){
			ɗ=ɗ.Replace("@",",");
			string www=Թ(ref ɗ),Ƨ=Թ(ref ɗ);
			Ѡ=Ѡ.Replace("web",Ƨ);

			IGC.SendBroadcastMessage(Ѱ,MѨ.Tag+",web,"+Ѡ);
			Ѩ=C12+Ѱ+"\n";
			}

			else if(ɗ.Contains("web")){
			for(int i=0;i < list.Count;i++){
			string ψ=Convert.ToString(ЩCC[i]);
			if(ψ.Contains(A20+"."))
			Me.CustomData=Me.CustomData.Replace("\n"+ψ,"");
			}
			str=str.Replace("web","");
			if(!Me.CustomData.Contains(str))
			Me.CustomData+=""+str;
			Ѩ=C13/*+str*/+"\n";
			}

			else if(ɗ.Contains("ӆ")){
			str=str.Replace("ӆ","");
			if(!Me.CustomData.Contains(str))
			Me.CustomData+="\n"+str;
			Ѩ=C14/*+str*/+"\n";
			}

			else{
			Ѩ=C15+"\n";
			Ѩ+=C8+": "+Ѱ+"\n";
			Ѩ+=C6+": "+MѨ.Tag+"\n";
			Ѩ+=C4+": "+ɗ+"\n";
			IGC.SendBroadcastMessage(Ѱ,MѨ.Tag+C16+ɗ);
			}

		}}}

		}
	void addremove(IMyTerminalBlock β,string Ə,string ϡ){

		if(Ə.Contains(A1)){
		if(!C(β.CustomName,ϡ) ){
		β.CustomName=(ϡ+" "+β.CustomName);
		ǂ+="\n Added: "+β.CustomName;
		}}
		if(Ə.Contains(A2)){
		if(C(β.CustomName,ϡ) ){
		β.CustomName=(β.CustomName.Replace (ϡ+" ",""));
		ǂ+="\n removed: "+β.CustomName;
		}}
		}
		double APS,AES;

	void MouseC(IMyShipController ȣ,bool Control_On){

		var ЩI=ȣ.RotationIndicator;
		Vector3D absUpVec=ȣ.WorldMatrix.Up;
		double YM=1,PM=1,PS=ЩSM * ЩI.Y * YM,ES=ЩSM * -ЩI.X * PM;
		APS=PS;
		AES=ES;
		var CWM=ȣ.WorldMatrix;

		if(CWM.Left.Dot(absUpVec) > 0.7071){
		APS=-ES;
		AES=PS;
		}
		else if(CWM.Right.Dot(absUpVec) > 0.7071){
		APS=ES;
		AES=-PS;
		}
		else if(CWM.Down.Dot(absUpVec) > 0.7071){
		APS=-PS;
		AES=-ES;
		}
		}

	
	void search(string ҕ){
		
		var ЩCC=CD.Split('\n');
		ArrayList list=new ArrayList(ЩCC);
		for(int i=0;i < list.Count;i++){
		string ψ=Convert.ToString(ЩCC[i]);

		if(!ψ.Contains("//")){

		if(ψ.Contains("settings")){
			ζ="// SETTINGS\n\n";

			ζ+="// TAGS\n";
			ζ+="// "+A5+",+objectname = sets itemname, script only handle blocks whit itemname in it.\n";
			ζ+=""+A5+","+ϡ+"\n";
			ζ+="// "+A25+",+lcdtag = sets first part of the lcdtag.\n";
			ζ+=""+A25+","+ƧT+"\n";
			ζ+="// "+A6+",cockpit tag = name that the cockpit must contain.\n";
			ζ+=""+A6+","+cockpit+"\n";
			ζ+="// "+A7+",remote tag = name that the remote must contain.\n";
			ζ+=""+A7+","+remote+"\n\n";

			ζ+="// MOUSE(only use for setup)\n";
			ζ+="// "+A8+",+screen = sets mouse to screen, mouse can't leave set screen.\n";
			ζ+="//"+A8+","+Щlcd+"\n";
			ζ+="// "+A9+",true or false = if true mouse is shown on screen.\n";
			ζ+="//"+A9+","+ЩЩ+"\n";
			ζ+="// "+A10+",true or false = if true mouse is shown on screen when park is on.\n";
			ζ+="//"+A10+","+HsЩ+"\n\n";

			ζ+="// IGC SETTINGS.\n";
			ζ+="// "+A14+",channel name = set this PB IGC Channel Name.\n";
			ζ+=""+A14+","+ғ+"\n";
			ζ+="// "+A15+",channel name = set IGC Channel Name to send to, wen set you can't change it in script.\n";
			ζ+="//"+A15+","+ś+"\n\n";
			
			ζ+="// ADDITIONAL\n";
			ζ+="// "+A12+",+number = sets sleepmode timer, if set to 0 ,sleepmode is off.\n";
			ζ+=""+A12+","+slĈ+"\n";
			ζ+="// "+A13+",+1 or 100 = sets script updatetype, usefull for reading log.\n";
			ζ+=""+A13+","+Runtime.UpdateFrequency+"\n\n";
			ζ+="// "+A56+",+true or false = allows to put these characters directly into memory\n";
			ζ+=""+A56+","+K+" \n\n";
			

			ψD=ψD.Replace("settings",ζ);
			}
		if(ψ.Contains("commands")){
			ζ="// COMMAND LIST\n\n";

			ζ+="// BLOCK RENAME COMMANDS.\n";
			ζ+="// "+A1+A3+",+tag = add tag to all blocks , even blocks after rotor en connectors.\n";
			ζ+="// "+A2+A3+",+tag = remove tag to all blocks , even blocks after rotor en connectors.\n";
			ζ+="// "+A1+A4+",+tag = add tag to all blocks in group , even blocks after rotor en connectors.\n";
			ζ+="// "+A2+A4+",+tag = remove tag to all blocks in group , even blocks after rotor en connectors.\n\n";

			ζ+="// LCD SET COMMANDS.\n";
			ζ+="// "+A25+",tag = sets screen tag is the same for every screen.\n";
			ζ+="// "+A25+",screentag,sur,screenname,X,Y,-X,-Y = make an surface to write on and eneble to set the resolution.\n";
			ζ+="//  if X,Y,-X,-Y = 0, it calculate and set the resolution and/or offset automatically.\n";
			ζ+="// "+A26+",screenname = tels you the set or calculated resolution.\n\n";

			ζ+="// LCD OBJECT COMMANDS.\n";
			ζ+="// "+A27+",screenname,memory,X,Y,Shape,H,W,°,screen to,new mouse X,new mouse Y, , , , , \n";
			ζ+="// "+A29+",screenname,memory,X,Y,Shape,H,W,°,text,Size,Name,typeQ,actionQ,typeE,actionE, \n";
			ζ+="// "+A28+",screenname,memory,X,Y,Shape,H,W,°,text,Size,Name,type, , , , \n";
			ζ+="// "+A30+",screenname,memory,X,Y,Shape,H,W,°,text,Ş,R,G,B,status, , \n\n";

			ζ+="// LOGIC GATES\n";
			ζ+="// logic will be place in the memory start with LOGIC NAME and than the memmory items divided with '|'.\n";
			ζ+="// OR NOR XOR XNOR AND NAND are suported\n";
		
			ζ+="// ADDITIONAL\n";
			ζ+="// "+A11+",+memory name = set memory, only for check.\n";
			ψD=ψD.Replace("commands",ζ);
			}

		if(ψ.Contains(A5+",")){
			nϡ=Թ(ref ψ);
			ϡ=Թ(ref ψ);
			}

		if(ψ.Contains(A25+",")){
			nϡ=Թ(ref ψ);
			ƧT=Թ(ref ψ);
			}

		if(ψ.Contains(A8+",")){
			nϡ=Թ(ref ψ);
			Щlcd=Թ(ref ψ);
			}

		if(ψ.Contains(A9+",true"))
			ЩЩ=true;

		if(ψ.Contains(A9+",false"))
			ЩЩ=false;

		if(ψ.Contains(A10+",true"))
			HsЩ=true;

		if(ψ.Contains(A10+",false"))
			HsЩ=false;
		if(ψ.Contains(A56+",true"))
			K=true;

		if(ψ.Contains(A56+",false"))
			K=false;

		if(ψ.Contains(A6+",")){
			nϡ=Թ(ref ψ);
			cockpit=Թ(ref ψ);
			}
		if(ψ.Contains(A7+",")){
			nϡ=Թ(ref ψ);
			remote=Թ(ref ψ);
			}
		if(ψ.Contains(A11+",")){
			nϡ=Թ(ref ψ);
			string ҕ2=Թ(ref ψ);
			if(!Ƥ.Contains(ҕ2))
			Ƥ+=ҕ2;
			}
		if(ψ.Contains(A12+",")){
			nϡ=Թ(ref ψ);
			string slt=Թ(ref ψ);
			sltime=int.Parse(slt);
			}
		if(ψ.Contains(A13+",")){
			nϡ=Թ(ref ψ);
			string ҕ2=Թ(ref ψ);
			if(ҕ2=="100")
			Runtime.UpdateFrequency=UpdateFrequency.Update100;
			else if(ҕ2=="1")
			Runtime.UpdateFrequency=UpdateFrequency.Update1;
			}
	
	 // INSTAL

		if((ψ.Contains(A1))|(ψ.Contains(A2)))
		ҕ=ψ;
		}

		if((C(ҕ,","))){
		if(ҕ.Length > 0){
		Ə=Թ(ref ҕ);
		if(Ə==""){
		ǂ=S4;
		return;
		}
		ϡ=Թ(ref ҕ);
		if(ϡ==""){
		ǂ=S5;
		return;
		}
		
		if((Ə.Contains(A1+A4))|(Ə.Contains(A2+A4))){
		Э=Թ(ref ҕ);
		if(Э==""){
		ǂ=S6;
		return;
		}

		IMyBlockGroup g=GridTerminalSystem.GetBlockGroupWithName(Э);

		List<IMyTerminalBlock> β1=new List<IMyTerminalBlock>();
		g.GetBlocks(β1);
		foreach (var β in β1){
		addremove(β,Ə,ϡ);
		}
		
		}
		else if((Ə.Contains(A1+A3))|(Ə.Contains(A2+A3))){
		foreach(var β in Т1){
		addremove(β,Ə,ϡ);
		}
		}
		}}
	
		}
		}

	void SleepM(string ҕ){

		if(C(ҕ,ҕ)){
			Runtime.UpdateFrequency=UpdateFrequency.Update1;
			Me.CustomName=(Me.CustomName.Replace(Sl,""));
			slmode=false;
			}
			if(!Ƥ.Contains(B1)){
			if(ȣ.IsUnderControl==true){
			Runtime.UpdateFrequency=UpdateFrequency.Update1;
			Me.CustomName=(Me.CustomName.Replace(Sl,""));
			slmode=false;
			slĈ=sltime;
			}
			if(sltime>1&&slĈ<100){
			slmode=true;
			ЩЩ=false;
			}

			if(sltime>1&&slĈ==0){
			Runtime.UpdateFrequency=UpdateFrequency.None;
			Me.CustomName=(Me.CustomName+(Sl));
			}}

		if(C(ҕ,B1)){
			if(!Ƥ.Contains(B1))
			Ƥ+=B1+"++ 0\n";
			}
			if(C(Ƥ,B1+"++ 2")){
			ȣCs();
			Gβ();
			}
			if(C(Ƥ,A57))
			Ƥ=" ";
		
			if(C(Ƥ,B1+"++ 4")){
			Ƥ=Ƥ.Replace(B1+"++ 4\n","");
			}

		var Page=Ƥ.Split('\n');
			ArrayList listƤ=new ArrayList(Page);
			for(int i=0;i < listƤ.Count;i++){
			string ψ=Convert.ToString(Page[i]);
		 // countdown internal timer

			if(ψ.Contains("--")){
			string ψt =	ψ;
			ψ=ψ.Replace("--",",");

			string ήt=Թ(ref ψ);
			string st=Թ(ref ψ);

			int t = int.Parse(st);
			if(Ĉ==1 ){
			if(t>0)
			t--;
			}
			Ƥ=Ƥ.Replace(ψt,ήt+"-- "+(t.ToString()));
			}
		 // countup internal timer
			if(ψ.Contains("++")){
			string ψt =	ψ;
			ψ=ψ.Replace("++",",");

			string ήt=Թ(ref ψ);
			string st=Թ(ref ψ);

			int t = int.Parse(st);
			if(Ĉ==1 ){
			t++;
			}
			Ƥ=Ƥ.Replace(ψt,ήt+"++ "+(t.ToString()));
			}

			}
		}

	void timercheck(out string running){

		if(slĈ==0){
		slĈ=sltime;
		}
		if(sltime>1)
		slĈ--;

		Ĉ++;
		if(Ĉ>time)
		Ĉ=0;

		if(Ĉ==time){
		ȅ++;
		if(ȅ>2)
		ȅ=0;
		}

		string ȅs=new string('.',ȅ);
		running="."+ȅs;
		}

	string RunSym(){
		Ԕ++;
		string ԗ="";
		if(Ԕ < 1)
		ԗ="|";
		else if(Ԕ < 2)
		ԗ="/";
		else if(Ԕ < 3)
		ԗ="--";
		else if(Ԕ < 4)
		ԗ="\\";
		else{
		ԗ="|";
		Ԕ=-1;
		}
		return ԗ;
		}

	public void RunStateMachine(){
		if(state !=null){
		bool hasMoreSteps=state.MoveNext();
		if(hasMoreSteps){
		Runtime.UpdateFrequency |=UpdateFrequency.Update1;
		}
		else{
		state.Dispose();

		state=null;
		}
		}
		if(state ==null)
		state=RunStuffOverTime();
		
	}