/*###############################################################################################################  
# 	Script Name: H2/O2 Status on LCD																  
# 	Author: AcId   
# 	 
#																												  
# 	Project start:	18.11.17 				Project end: 25.11.17														  
################################################################################################################																							 
# 	Description: 																								  
	This script shows fill level state of Hydrogen and / or Oxygen Tank's on standart LCDs or wide LCDs, as digital Symbols 
	Just edit your options, and it you will able to overwatch up to 100 Tank's very easy.*/ 
 
	//-----   Generals    ----- 
 
	// Choose what to search for, Hydrogen or Oxygen. 
	bool SearchHydro 	=     	true;     	// true = Show Hydrogentank's, false = Show Oxygentank's = false; 
 
	// Change display status between Hydrogen and Oxygen Status on LCD's 
	int AutoChangeInterval =     	0;     	// 0 = Off, 1 = every time script is acticated, 2 = every 2nd time etc. 
 
	// Show both status, for Hydrogen default/ Text Panel's, and for Oxygen seperate LCD / Text Panel's 
	bool ShowOnSeperateLCD =     	true;     	// true = show on seperate lcd, false = show only default LCD's 
	 
	//	This Script needs to identifying the Hydrogentanks, to distinguish Oxygentank's or Hydrogentanks, there for all of your 
	// Hydrogentank's needs to have the following NameTag in their name, you can also edit the word. 
	string Hydro_ident_String =     	"Hydro";     	// all your hydrogentank's must have these part of name inside, ex: my Hydrotank 12 
	 
	// LCD / Text Panel to show status 
	// All LCD with a specific NameTag included in their name, will be show the information. You can edit the NameTag here. 
	string LCD_NameTag =     	"[Hydrogen-Status LCD]";     	//example: my LCD 31 [Hydrogen-Status LCD] 	 
	string SeperateLCD_NameTag =     	"[Seperate-Status LCD]";     	//if ShowOnSeperateLCD = true , you need 2nd nametag, example: my LCD 31 [Oxygen-Status LCD] 	 
 
	// NameTags Specific Blocks 
	// on default this script search for all Hydro-/ Oxygentanks attached, and show there Status, but somethimes you want be able to 
	// show only specific Hydro-/Oxygentanks's, then set OnlyTanksWithNameTag to true. 
	bool OnlyTanksWithNameTag =     	false;     	//false	= show all found Tank's attached, true = show only Tank's with Specific NameTag in their name 
 
	//	that means that only tank's with a specific Name or a Word, that you add to your Hydro-/ Oxygen Blockname, and then only 
	//	the status of that specific tank's will be shown, you can change the Nametag here. 
	string SpecificNameTag =     	"[Hydro-Status]";     	//NameTag to show only specific Hydrogen Tank's, 	example: my Hydrotank 12 [Hydro-Status] 
	string SeperateSpecificNameTag =     	"[SepSpecfic-Status]";     	//NameTag to show only specific Tank's, for seperate LCD, 	example: my Hydrotank 12 [Oxygen-Status] 
 
	//-----   Display Settings    -----	 
 
	// Wide LCD or LCD 
	// on default it shows a total amount of max. 50 tanks. There's a option to show a total amount of 100 Tank's, but this only works for wide LCDs,  
	// on 1x1 LCD will be then shown just the half information! if you use Wide LCDs, then set WideLCD to true. 
	bool WideLCD =     	true;     	// Show		Status on Wide LCD, up to 100 Tank's,		This Option activated on Standart LCD, you just se the half information 
 
	// this script change size of the Symbols depending of the amount.  
	// it shows Large Symbols for an total amount of 1-10 Tank's's on 1x1 LCDs, and a total amount of 1-20 Tank's on wide LCDs, all amount above will be shown 
	// in small Symbols, to show aways small Symbols set it to true 
	bool OnlySmallSigns_Enabled =     	false;     	// true = always small symbols, false = depending on amount 
 
	// Self updating System 
	// thanks to SpaceEngeneers Update 1.185.1, we are able to use a new system, no need for timer,  
	// false = you need an activation 
	bool SelfUpdatingSys_Enabled =     	true;     	// false = Off, true Selfupdating on 
	 
	//if Self updating System enabled you can choose how many times per second the script will be activated 
	int SelfUpSys_perSecond	=     	2;     	// 1 = 1 sec, 2 = sec etc. 
	 
	//-----   Other Settings    -----	 
	 
	// Shows Title on top of the LCD / Text Panel 
	// "HYDROGEN STATUS" for Hydrogen, and "OXYGEN STATUS for Oxygen,if disabled it leaves it space black 
	bool StatusTitle_Enabled =     	true;     	// true = show title, false = no title 
 
	//there are three lines as border, you can deaktivate each of them of you want, true = on, false = off 
	bool Underline_1_Enabled     	=     	true; 		// Show Underline below Title 
	bool BatSpaceline_Enabled     	=     	true; 		// Show Middle line between Tank Amount and Stored Volume 
	bool Underline_2_Enabled     	=     	true; 		// Show Underline Below Tank Amount and Stored Volume 
 
	// Show Amount of Hydro-/Oxygentank's displayed 
	bool TankAmountEnabled =     	true;     	// true = Show Amount, false = off 
 
	// Show total amount of all Stored Volume in HL or m3 (cubicmeter), Units depending on amount of volume, above 999'999 HL it displays in m3				 
	bool TankAllStoredVolumeEnabled =     	true;     	// true = Show stored volume, false = off 
 
	// Show Stored Volume always in Cubicmeter 
	bool VolumeOnlyInM2 =     	false;     	// true = Show volume always in cubicmeter, false = switch between hL and M² 
	 
	// LCD Brightness 
	// 0-255, 0 = dark, 255 = Bright 
	int LCDbright =     	255;   
	 
/* 
# 	General Thanks to the Community, to all that share their knowledge, that helped me a lot to make this  
# 	scripts working. Thanks for that.  
*/  
 
 
//Dont touch the script below, to prevent errors 
//############################################################################################################## 
 
int SelfUpSysCounttimes = 5; 
int SelfUpSysCounter = 0; 
bool firsttime	= true; 
int IntervalCounter = 0; 
public Program()	{ 
	if	(SelfUpdatingSys_Enabled)	{ 
		Runtime.UpdateFrequency = UpdateFrequency.Update10;	 
	} 
} 
 
public void Save() 
{} 
 
 
// Main Script Start 
public void Main(string argument, UpdateType updateSource) 
{ 
	bool Run_ThisScript	=	false; 
	if	(SelfUpdatingSys_Enabled)	{ 
		if	(SelfUpSysCounter	== 0)	{ 
			SelfUpSysCounter = SelfUpSysCounttimes	*	SelfUpSys_perSecond; 
			Run_ThisScript	=	true; 
		} 
		if	(!Run_ThisScript)	{ 
			SelfUpSysCounter	-=	1; 
		} 
	}	else	{ 
		Run_ThisScript	=	true; 
	} 
 
	if	(Run_ThisScript)	{ 
		string E_Search	=	""; 
		if	(SearchHydro)	{ 
			E_Search = "Hydrogen Status"; 
		}	else	{ 
			E_Search = "Oxygen Status"; 
		} 
				 
		if	(ShowOnSeperateLCD)	{ 
			if	(SearchHydro)	{ 
				E_Search = "Seperate - show Hydro"; 
			}	else	{ 
				E_Search = "Seperate - show Oxygen"; 
			} 
		}	else	{ 
			if	(AutoChangeInterval	> 0)	{ 
				if	(SearchHydro)	{ 
					E_Search = "Switch every "	+	AutoChangeInterval	+ " x"	+	" - to Hydro"; 
				}	else	{ 
					E_Search = "Switch every "	+	AutoChangeInterval	+ " x"	+	" - to Oxygen"; 
				} 
			} 
		} 
 
		string ELine_1	=	""; 
		string ELine_2	=	""; 
		string ELine_3	=	""; 
		string ELine_4	=	""; 
		string ELine_5	=	""; 
		 
		ELine_1	=	"Hydrogen & Oxygen Status Script:\nScript by Lightwolf\n\nOptions:\n"; 
		ELine_2	=	"Search Mode: "	+	E_Search	+	"\n"; 
		ELine_3	=	"Hydrotank's identifying NameTag: "	+	Hydro_ident_String	+	"\n"; 
		ELine_4	=	"Default LCD: "	+	LCD_NameTag	+	"\n"; 
		ELine_5	=	"Seperate LCD: "	+	SeperateLCD_NameTag	+	"\n"; 
		if	(ShowOnSeperateLCD)	{ 
			ELine_4	= ELine_4 + ELine_5; 
		} 
		string ELine_6	=	""; 
		if	(OnlyTanksWithNameTag)	{ 
			if	(ShowOnSeperateLCD)	{ 
				if	(SearchHydro)	{ 
					ELine_6	=	"only Hydrotank's with: "	+	SpecificNameTag	+	"\n"; 
				}	else	{ 
					ELine_6	=	"only Oxygentank's with: "	+	SeperateSpecificNameTag	+	"\n"; 
				} 
				ELine_4 = ELine_4 + ELine_6; 
			} 
		} 
		string ELine_7	=	""; 
		if	(WideLCD)	{ 
			ELine_7	=	"\nShow only on Wide LCD'S / Text Panel's\n"; 
			ELine_4 = ELine_4 + ELine_7; 
		} 
		string ELine_8	=	""; 
		if	(OnlySmallSigns_Enabled)	{ 
			ELine_8	=	"\nShow always small symbols\n"; 
			ELine_4 = ELine_4 + ELine_8; 
		} 
		string ELine_9	=	""; 
		string Eline_10	=	""; 
		if	(SelfUpdatingSys_Enabled)	{ 
			ELine_9	=	"\nSelf Updating enabled\n"; 
			Eline_10	=	"Updating every "	+	SelfUpSys_perSecond	+	" Second\n"; 
			ELine_4 = ELine_4 + ELine_9; 
			ELine_4 = ELine_4 + Eline_10; 
		} 
 
		string echotext	=	ELine_1	+	ELine_2	+	ELine_3	+	ELine_4; 
 
		Echo(echotext); 
 
		if (firsttime)	{	 
			firsttime	=	false; 
			if	(ShowOnSeperateLCD)	{	AutoChangeInterval	= 1;	} 
			IntervalCounter = IntervalCounter	+	AutoChangeInterval; 
		}  
 
		string LCD_NameTag_now	=	LCD_NameTag; 
		string SpecificNameTag_now	=	SpecificNameTag; 
		 
		if	(ShowOnSeperateLCD)	{ 
			if	(IntervalCounter	==	0)	{ 
				if	(SearchHydro)	{	SearchHydro 	=     	false; 
										LCD_NameTag_now	= SeperateLCD_NameTag; 
										SpecificNameTag_now = SeperateSpecificNameTag; 
									}	else {	 
										SearchHydro 	=     	true;	 
									} 
				IntervalCounter = IntervalCounter	+	AutoChangeInterval; 
			} 
			IntervalCounter -= 1; 
		} else	{ 
			if	(AutoChangeInterval	> 0)	{ 
				ShowOnSeperateLCD =     	false; 
				 
				 
				if	(IntervalCounter	==	0)	{ 
					if	(SearchHydro)	{	SearchHydro 	=     	false;		}	 
					else {	SearchHydro 	=     	true;	} 
					IntervalCounter = IntervalCounter	+	AutoChangeInterval; 
				} 
				 
				if	(!SearchHydro)	{	SpecificNameTag_now = SeperateSpecificNameTag;	} 
 
				IntervalCounter -= 1; 
			} 
		} 
 
		string TankStoredUnit	=	""; 
		bool OnlyNameTag		=	false; 
		int AmountOfTank		=	0; 
		float Fill_All_L_F		=	0;	 
 
		//Pixel Tempelates 
		string P1 = ""; 
		string P2 = ""; 
		string P3 = ""; 
		string P6	=	""; 
		string P7	=	""; 
		string P8	=	""; 
		string P9	=	""; 
		string P10	=	""; 
		string P11	=	""; 
		string P17	=	P10	+	P7; 
		string P18	=	P10	+	P8; 
		string P28	=	P10	+	P10	+	P7; 
		string P38	=	P10	+	P10	+	P10	+	P8; 
		string PxFull	=	""; 
		string Breakli	=	"\n"; 
		string underli_A	=	""; 
		string underli_B	=	""; 
		//Space		######################################## 
		string li035	=	""; 
		string li036	=	""; 
		string li037	=	"";string li038	=	"";string li039	=	""; 
		string li040	=	"";string li041	=	"";string li042	=	"";string li043	=	"";string li044	=	"";string li045	=	"";string li046	=	"";string li047	=	"";string li048	=	"";string li049	=	""; 
		string li050	=	"";string li051	=	"";string li052	=	"";string li053	=	"";string li054	=	"";string li055	=	"";string li056	=	"";string li057	=	"";string li058	=	"";string li059	=	""; 
		string li060	=	"";string li061	=	"";string li062	=	"";string li063	=	"";string li064	=	"";string li065	=	"";string li066	=	"";string li067	=	"";string li068	=	"";string li069	=	""; 
		string li070	=	"";string li071	=	"";string li072	=	"";string li073	=	"";string li074	=	"";string li075	=	"";string li076	=	"";string li077	=	"";string li078	=	"";string li079	=	""; 
		string li080	=	"";string li081	=	"";string li082	=	"";string li083	=	"";string li084	=	"";string li085	=	"";string li086	=	"";string li087	=	"";string li088	=	""; 
		string li089	=	"";string li090	=	"";string li091	=	"";string li092	=	"";string li093	=	""; 
		string li094	=	"";string li095	=	"";string li096	=	"";string li097	=	"";string li098	=	"";string li099	=	"";string li100	=	""; 
		string li101	=	"";string li102	=	"";string li103	=	"";string li104	=	"";string li105	=	"";string li106	=	"";string li107	=	"";string li108	=	"";string li109	=	""; 
		string li110	=	"";string li111	=	"";string li112	=	"";string li113	=	"";string li114	=	"";string li115	=	"";string li116	=	"";string li117	=	"";string li118	=	"";string li119	=	""; 
		string li120	=	"";string li121	=	"";string li122	=	"";string li123	=	"";string li124	=	"";string li125	=	"";string li126	=	"";string li127	=	"";string li128	=	"";string li129	=	""; 
		string li130	=	"";string li131	=	"";string li132	=	"";string li133	=	"";string li134	=	"";string li135	=	"";string li136	=	"";string li137	=	"";string li138	=	"";string li139	=	""; 
		string li140	=	"";string li141	=	"";string li142	=	"";string li143	=	"";string li144	=	"";string li145	=	"";string li146	=	"";string li147	=	"";string li148	=	"";string li149	=	""; 
		string li150	=	"";string li151	=	"";string li152	=	"";string li153	=	"";string li154	=	"";string li155	=	"";string li156	=	"";string li157	=	"";string li158	=	"";string li159	=	""; 
		string li160	=	"";string li161	=	"";string li162	=	"";string li163	=	"";string li164	=	"";string li165	=	"";string li166	=	"";string li167	=	"";string li168	=	"";string li169	=	""; 
		string li170	=	"";string li171	=	"";string li172	=	"";string li173	=	"";string li174	=	"";string li175	=	"";string li176	=	"";string li177	=	"";string li178	=	""; 
		 
		// Create a new List for all found Hydro & Oxygen tanks 
		var allTanks_list	=	new List<IMyTerminalBlock>();	//create new empty list 
		GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allTanks_list);	//put all Hydro & Oxygen tanks in this list 
 
		//	create Variables to hold data 
		int AmountOfOxygenTank	=	0; 
		int AmountOfOxygenTank_Working	=	0; 
		int AmountOfHydroTank	=	0; 
		int AmountOfHydroTank_Working	=	0;	 
		int HydTankCount	=	0; 
		int OxyTankCount	=	0; 
		var allTanks_CustomName	=	""; 
		var Filled_Value_Current	=	""; 
		var Filledmax_Value_Max	=	""; 
		var stockpile_bool	=	false; 
		float Filled_Value_Current_L_F	=	0.00f; 
		float Filledmax_Value_Max_L_F	=	0.00f; 
		float hydro_Fill_All_L_F	=	0.0000f;	 
		float oxy_Fill_All_L_F	=	0.0000f; 
		bool OnlySmallSigns			=	false; 
		int TankCountX	=	0;	 
		 
		for (int i=0;	i<allTanks_list.Count;	i++) 
		{	if (allTanks_list[i].CustomName.Contains(Hydro_ident_String))	{	HydTankCount	+=	1	;	} 
			else {	OxyTankCount	+=	1;	} 
		} 
		 
		// amount of changing size of symbols 
		int WideMaxValue	=	10; 
		int WideMinValue	=	11; 
		if	(WideLCD)	{ 
		WideMaxValue	=	20; 
		WideMinValue	=	21; 
		} 
 
		if	(SearchHydro)	{	TankCountX	=	HydTankCount;	} 
		else {	TankCountX	=	OxyTankCount;	} 
		 
		if		(OnlySmallSigns_Enabled)	{	OnlySmallSigns			=	true;	} 
		else if	(TankCountX	>	WideMaxValue)	{	OnlySmallSigns			=	true;	} 
		 
		string pz	=	P2; 
		if (OnlySmallSigns)	{ 
			li035	=	pz;li036	=	pz;li037	=	pz;li038	=	pz;li039	=	pz;li040	=	pz;li041	=	pz;li042	=	pz;li043	=	pz;li044	=	pz;li045	=	pz;li046	=	pz;li047	=	pz;li048	=	pz; 
			li049	=	pz;li050	=	pz;li051	=	pz;li052	=	pz;li053	=	pz;li054	=	pz;li055	=	pz;li056	=	pz;li057	=	pz;li058	=	pz;li059	=	pz;li060	=	pz;li061	=	pz; 
			li064	=	pz;li065	=	pz;li066	=	pz;li067	=	pz;li068	=	pz;li069	=	pz;li070	=	pz;li071	=	pz;li072	=	pz;li073	=	pz;li074	=	pz;li075	=	pz;li076	=	pz;li077	=	pz; 
			li078	=	pz;li079	=	pz;li080	=	pz;li081	=	pz;li082	=	pz;li083	=	pz;li084	=	pz;li085	=	pz;li086	=	pz;li087	=	pz;li088	=	pz;li089	=	pz;li090	=	pz; 
			li093	=	pz;li094	=	pz;li095	=	pz;li096	=	pz;li097	=	pz;li098	=	pz;li099	=	pz;li100	=	pz;li101	=	pz;li102	=	pz;li103	=	pz;li104	=	pz;li105	=	pz;li106	=	pz; 
			li107	=	pz;li108	=	pz;li109	=	pz;li110	=	pz;li111	=	pz;li112	=	pz;li113	=	pz;li114	=	pz;li115	=	pz;li116	=	pz;li117	=	pz;li118	=	pz;li119	=	pz; 
			li122	=	pz;li123	=	pz;li124	=	pz;li125	=	pz;li126	=	pz;li127	=	pz;li128	=	pz;li129	=	pz;li130	=	pz;li131	=	pz;li132	=	pz;li133	=	pz;li134	=	pz;li135	=	pz; 
			li136	=	pz;li137	=	pz;li138	=	pz;li139	=	pz;li140	=	pz;li141	=	pz;li142	=	pz;li143	=	pz;li144	=	pz;li145	=	pz;li146	=	pz;li147	=	pz;li148	=	pz; 
			li151	=	pz;li152	=	pz;li153	=	pz;li154	=	pz;li155	=	pz;li156	=	pz;li157	=	pz;li158	=	pz;li159	=	pz;li160	=	pz;li161	=	pz;li162	=	pz;li163	=	pz;li164	=	pz; 
			li165	=	pz;li166	=	pz;li167	=	pz;li168	=	pz;li169	=	pz;li170	=	pz;li171	=	pz;li172	=	pz;li173	=	pz;li174	=	pz;li175	=	pz;li176	=	pz;li177	=	pz; 
		} 
		 
		//	This Loop find all Hydrotanks and Oxygentanks, and find all needed Information to show it on LCDs 
		//######	For Loop	######################################################################################## 
		for (int i=0;	i<allTanks_list.Count;	i++) 
		{	 
			var allTanksList_TankX_CustomName = allTanks_list[i].CustomName;							// Custom name of Tank Block 
			IMyGasTank allTanksList_TankX = GridTerminalSystem.GetBlockWithName(allTanksList_TankX_CustomName) as IMyGasTank; 
		 
			OnlyNameTag	=	false; 
			if	(OnlyTanksWithNameTag)	{ 
				if (allTanks_list[i].CustomName.Contains(SpecificNameTag_now))	{	OnlyNameTag	=	true;		} 
			} 
			// Get Information about each found Tank, to Show all needed Data 
			var tank_X_Detailed = allTanks_list[i].DetailedInfo;						// Get detailed info string of the Specific tank  
			allTanks_CustomName = allTanks_list[i].CustomName;							// get Custom name of this Tank 
			var dataCache = tank_X_Detailed.Split('\n');								// Split the string line, to get the needed information 
			var tank_X_Typ = dataCache[0].Split(':')[1].Trim();							// Get Tank Type Oxygen or Hydrogen 
			var Filled_Value_line = dataCache[2].Split(':')[1].Trim();					// Filled Value, cut at ":" "Filled: 100.0% (80000L/80000L)" 
			var Filled_Val_line = Filled_Value_line.Split('(')[1].Trim();				// Filled Value, cut at "(" "100.0% (80000L/80000L)" 
			Filled_Value_Current = Filled_Val_line.Split('L')[0].Trim();				// Value Currently filled, cut at "L" "80000L/80000L)" 
			var Filledmax_Val_line = Filled_Val_line.Split('/')[1].Trim();				// Filled Value, cut at "/" "80000L/80000L)" 
			Filledmax_Value_Max = Filledmax_Val_line.Split('L')[0].Trim();				// Max Filled Value, cut at "L" "80000L)" 
			stockpile_bool = allTanks_list[i].GetValueBool("Stockpile");				//Var / Bool 
 
			Filled_Value_Current_L_F = float.Parse(Filled_Value_Current);				// Count Value 
			Filledmax_Value_Max_L_F = float.Parse(Filledmax_Value_Max);					// Count Value 
			 
			//Get tank state converted to level bars 
			float TankLevel		=	Filledmax_Value_Max_L_F/6;			//Tank Level 1 
			float TankLevel_1	=	TankLevel;			// 1 Bar 
			float TankLevel_2	=	TankLevel	*	2;	// 2 Bar		 
			float TankLevel_3	=	TankLevel	*	3;	// 3 Bar		 
			float TankLevel_4	=	TankLevel	*	4;	// 4 Bar		 
			float TankLevel_5	=	TankLevel	*	5;	// 5 Bar		 
			float TankLevel_6	=	TankLevel	*	6;	// 6 Bar		 
 
			string Py	=	P17; 
			string BB_FX	=	""; 
			string BB_Bl	=	"";string BB_Gr	=	"";string BB_Ye	=	"";string BB_Or	=	"";string BB_Re		=	""; 
			if	(OnlySmallSigns)	{Py	=	P11;BB_Bl	=	"";BB_Gr	=	"";BB_Ye	=	"";BB_Or	=	"";BB_Re	=	"";} 
																		  
			if	(SearchHydro)	{	BB_FX	=	BB_Ye;	} 
			else	{	BB_FX	=	BB_Bl;	} 
 
			string BBST01	=	Py;string BBST02	=	Py;string BBST03	=	Py;string BBST04	=	Py;string BBST05	=	Py;string BBST06	=	Py;string BBST07	=	Py;string BBST08	=	Py;string BBST09	=	Py;			 
			string BBST10	=	Py;string BBST11	=	Py;string BBST12	=	Py;string BBST13	=	Py;string BBST14	=	Py;string BBST15	=	Py;string BBST16	=	Py;string BBST17	=	Py;string BBST18	=	Py;string BBST19	=	Py; 
			string BBST20	=	Py;string BBST21	=	Py;string BBST22	=	Py;string BBST23	=	Py;string BBST24	=	Py;string BBST25	=	Py;string BBST26	=	Py;string BBST27	=	Py;string BBST28	=	Py;string BBST29	=	Py; 
			string BBST30	=	Py;string BBST31	=	Py;string BBST32	=	Py;string BBST33	=	Py;string BBST34	=	Py;string BBST35	=	Py;string BBST36	=	Py;string BBST37	=	Py;string BBST38	=	Py;string BBST39	=	Py; 
			 
			bool Tank_IsFunctional	=	false; 
			bool Tank_IsCharging	=	false; 
			bool Tank_Stockpile		=	false; 
			bool Tank_IsOff			=	false; 
			 
			if (allTanksList_TankX.Enabled) 	{ 
				Tank_IsOff			=	true; 
				if (allTanks_list[i].IsFunctional)	{ 
					if (allTanks_list[i].IsWorking)	{ 
						Tank_IsFunctional	=	true; 
						if (stockpile_bool)	{	Tank_Stockpile	=	true;	if 	(Filled_Value_Current_L_F	<	Filledmax_Value_Max_L_F)	{	Tank_IsCharging		=	true;	}} 
			}}}	else if (allTanks_list[i].IsFunctional)			{	Tank_IsFunctional	=	true;	} 
			 
			bool Level_on	= false; 
			//if Tank Charging 
			if			(Tank_IsCharging)	{	Level_on	= true;		}	 
			//if Tank OnlyRecharge 
			else if	(Tank_Stockpile)	{ 
				if	(OnlySmallSigns)	{ 
					BBST01	=	P11;BBST02	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11; 
					BBST07	=	"";BBST08	=	"";BBST09	=	"";BBST10	=	"";BBST11	=	"";BBST12	=	"";BBST13	=	""; 
					BBST14	=	"";BBST15	=	"";BBST16	=	"";BBST17	=	""; 
					BBST18	=	P11;BBST19	=	P11;BBST20	=	P11;BBST21	=	P11;BBST22	=	P11;BBST23	=	P11;BBST24	=	P11; 
				}	else	{ 
					BBST10	=	"";BBST11	=	"";BBST12	=	"";BBST13	=	"";BBST14	=	"";BBST15	=	""; 
					BBST16	=	"";BBST17	=	"";BBST18	=	"";BBST19	=	"";BBST20	=	"";BBST21	=	""; 
					BBST22	=	"";BBST23	=	"";BBST24	=	"";BBST25	=	"";BBST26	=	"";BBST27	=	"";BBST28	=	""; 
			}} 
			//if Tank Offline 
			else if	(!Tank_IsOff)	{	Level_on	= true;		}	 
			//if Tank Not Functional 
			else if	(!Tank_IsFunctional)	{ 
				if	(OnlySmallSigns)	{ 
					BBST01	=	P11;BBST02	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11; 
					BBST07	=	"";BBST08	=	"";BBST09	=	"";BBST10	=	"";BBST11	=	"";BBST12	=	""; 
					BBST13	=	"";BBST14	=	"";BBST15	=	"";BBST16	=	"";BBST17	=	""; 
					BBST18	=	P11;BBST19	=	P11;BBST20	=	P11;BBST21	=	P11;BBST22	=	P11;BBST23	=	P11;BBST24	=	P11; 
				}	else	{ 
					BBST10	=	"";BBST11	=	"";BBST12	=	"";BBST13	=	"";BBST14	=	"";BBST15	=	"";BBST16	=	""; 
					BBST17	=	"";BBST18	=	"";BBST19	=	"";BBST20	=	"";BBST21	=	"";BBST22	=	"";BBST23	=	""; 
					BBST24	=	"";BBST25	=	"";BBST26	=	"";BBST27	=	"";BBST28	=	""; 
					BBST29	=	P17;BBST30	=	P17;BBST31	=	P17;BBST32	=	P17;BBST33	=	P17;BBST34	=	P17;BBST35	=	P17;BBST36	=	P17;BBST37	=	P17;BBST38	=	P17;BBST39	=	P17; 
				} 
			}	else {	Level_on	= true;		} 
 
			if	(Tank_IsFunctional)	{ 
				if	(Level_on)	{ 
					if	(Filled_Value_Current_L_F	>=	TankLevel_6)	{ 
						if	(OnlySmallSigns)	{ 
							BBST01	=	P11;BBST02	=	BB_Gr;BBST03	=	BB_Gr;BBST04	=	P11;BBST05	=	P11;BBST06	=	BB_FX;BBST07	=	BB_FX;BBST08	=	P11;BBST09	=	P11;BBST10	=	BB_FX;BBST11	=	BB_FX;BBST12	=	P11; 
							BBST13	=	P11;BBST14	=	BB_FX;BBST15	=	BB_FX;BBST16	=	P11;BBST17	=	P11;BBST18	=	BB_FX;BBST19	=	BB_FX;BBST20	=	P11;BBST21	=	P11;BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	P11; 
						}	else	{ 
							BBST01	=	BB_Gr;BBST02	=	BB_Gr;BBST03	=	BB_Gr;BBST04	=	BB_Gr; 
							BBST08	=	BB_FX;BBST09	=	BB_FX;BBST10	=	BB_FX;BBST11	=	BB_FX; 
							BBST15	=	BB_FX;BBST16	=	BB_FX;BBST17	=	BB_FX;BBST18	=	BB_FX; 
							BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	BB_FX;BBST25	=	BB_FX; 
							BBST29	=	BB_FX;BBST30	=	BB_FX;BBST31	=	BB_FX;BBST32	=	BB_FX; 
							BBST36	=	BB_FX;BBST37	=	BB_FX;BBST38	=	BB_FX;BBST39	=	BB_FX; 
						} 
					}	else if	(Filled_Value_Current_L_F	>=	TankLevel_5)	{ 
						if	(OnlySmallSigns)	{ 
							BBST01	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	BB_FX;BBST07	=	BB_FX;BBST08	=	P11;BBST09	=	P11;BBST10	=	BB_FX;BBST11	=	BB_FX;BBST12	=	P11; 
							BBST13	=	P11;BBST14	=	BB_FX;BBST15	=	BB_FX;BBST16	=	P11;BBST17	=	P11;BBST18	=	BB_FX;BBST19	=	BB_FX;BBST20	=	P11;BBST21	=	P11;BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	P11; 
						}	else	{ 
							BBST08	=	BB_FX;BBST09	=	BB_FX;BBST10	=	BB_FX;BBST11	=	BB_FX; 
							BBST15	=	BB_FX;BBST16	=	BB_FX;BBST17	=	BB_FX;BBST18	=	BB_FX; 
							BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	BB_FX;BBST25	=	BB_FX; 
							BBST29	=	BB_FX;BBST30	=	BB_FX;BBST31	=	BB_FX;BBST32	=	BB_FX; 
							BBST36	=	BB_FX;BBST37	=	BB_FX;BBST38	=	BB_FX;BBST39	=	BB_FX; 
						} 
					}	else if	(Filled_Value_Current_L_F	>=	TankLevel_4)	{ 
						if	(OnlySmallSigns)	{ 
							BBST01	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11;BBST07	=	P11;BBST08	=	P11;BBST09	=	P11;BBST10	=	BB_FX;BBST11	=	BB_FX;BBST12	=	P11;BBST13	=	P11; 
							BBST14	=	BB_FX;BBST15	=	BB_FX;BBST16	=	P11;BBST17	=	P11;BBST18	=	BB_FX;BBST19	=	BB_FX;BBST20	=	P11;BBST21	=	P11;BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	P11; 
						}	else	{ 
							BBST15	=	BB_FX;BBST16	=	BB_FX;BBST17	=	BB_FX;BBST18	=	BB_FX; 
							BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	BB_FX;BBST25	=	BB_FX; 
							BBST29	=	BB_FX;BBST30	=	BB_FX;BBST31	=	BB_FX;BBST32	=	BB_FX; 
							BBST36	=	BB_FX;BBST37	=	BB_FX;BBST38	=	BB_FX;BBST39	=	BB_FX; 
						} 
					}	else if	(Filled_Value_Current_L_F	>=	TankLevel_3)	{ 
						if	(OnlySmallSigns)	{ 
							BBST01	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11;BBST07	=	P11;BBST08	=	P11;BBST09	=	P11;BBST10	=	P11;BBST11	=	P11;BBST12	=	P11;BBST13	=	P11; 
							BBST14	=	BB_Ye;BBST15	=	BB_Ye;BBST16	=	P11;BBST17	=	P11;BBST18	=	BB_FX;BBST19	=	BB_FX;BBST20	=	P11;BBST21	=	P11;BBST22	=	BB_FX;BBST23	=	BB_FX;BBST24	=	P11; 
						}	else	{ 
							BBST22	=	BB_Ye;BBST23	=	BB_Ye;BBST24	=	BB_Ye;BBST25	=	BB_Ye; 
							BBST29	=	BB_FX;BBST30	=	BB_FX;BBST31	=	BB_FX;BBST32	=	BB_FX; 
							BBST36	=	BB_FX;BBST37	=	BB_FX;BBST38	=	BB_FX;BBST39	=	BB_FX; 
						} 
					}	else if	(Filled_Value_Current_L_F	>=	TankLevel_2)	{ 
						if	(OnlySmallSigns)	{ 
							BBST01	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11;BBST07	=	P11;BBST08	=	P11;BBST09	=	P11;BBST10	=	P11;BBST11	=	P11;BBST12	=	P11;					BBST13	=	P11; 
							BBST14	=	P11;BBST15	=	P11;BBST16	=	P11;BBST17	=	P11;BBST18	=	BB_Or;BBST19	=	BB_Or;BBST20	=	P11;BBST21	=	P11;BBST22	=	BB_FX;BBST23	=	BB_FX;					BBST24	=	P11; 
						}	else	{ 
							BBST29	=	BB_Or;BBST30	=	BB_Or;BBST31	=	BB_Or;BBST32	=	BB_Or; 
							BBST36	=	BB_FX;BBST37	=	BB_FX;BBST38	=	BB_FX;BBST39	=	BB_FX; 
						} 
					}	else if	(Filled_Value_Current_L_F	>=	TankLevel_1)	{ 
						if	(OnlySmallSigns)	{ 
							BBST01	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11;BBST07	=	P11;BBST08	=	P11;BBST09	=	P11;BBST10	=	P11;BBST11	=	P11;BBST12	=	P11; 
							BBST13	=	P11;BBST14	=	P11;BBST15	=	P11;BBST16	=	P11;BBST17	=	P11;BBST18	=	P11;BBST19	=	P11;BBST20	=	P11;BBST21	=	P11;BBST22	=	BB_Re;BBST23	=	BB_Re;BBST24	=	P11; 
						}	else	{ 
							BBST36	=	BB_Re;BBST37	=	BB_Re;BBST38	=	BB_Re;BBST39	=	BB_Re; 
						} 
					} 
				} 
			}	else	{ 
					if	(OnlySmallSigns)	{ 
					BBST01	=	P11;BBST02	=	P11;BBST02	=	P11;BBST03	=	P11;BBST04	=	P11;BBST05	=	P11;BBST06	=	P11; 
					BBST07	=	"";BBST08	=	"";BBST09	=	"";BBST10	=	"";BBST11	=	"";BBST12	=	""; 
					BBST13	=	"";BBST14	=	"";BBST15	=	"";BBST16	=	"";BBST17	=	""; 
					BBST18	=	P11;BBST19	=	P11;BBST20	=	P11;BBST21	=	P11;BBST22	=	P11;BBST23	=	P11;BBST24	=	P11; 
				}	else	{ 
					BBST10	=	"";BBST11	=	"";BBST12	=	"";BBST13	=	"";BBST14	=	"";BBST15	=	"";BBST16	=	""; 
					BBST17	=	"";BBST18	=	"";BBST19	=	"";BBST20	=	"";BBST21	=	"";BBST22	=	"";BBST23	=	""; 
					BBST24	=	"";BBST25	=	"";BBST26	=	"";BBST27	=	"";BBST28	=	""; 
					BBST29	=	P17;BBST30	=	P17;BBST31	=	P17;BBST32	=	P17;BBST33	=	P17;BBST34	=	P17;BBST35	=	P17;BBST36	=	P17;BBST37	=	P17;BBST38	=	P17;BBST39	=	P17; 
				} 
			} 
 
			// Fuse Tank Sign together	############################################ 
			string BC_037	=	"";				string BR_037	=	"";				string Bo_037	=	""; 
			string BC_038	=	"";				string BR_038	=	"";				string Bo_038	=	""; 
			string BC_039	=	"";				string BR_039	=	"";				string Bo_039	=	""; 
			string BC_040	=	"";				string BR_040	=	"";				string Bo_040	=	""; 
			string BC_041	=	"";				string BR_041	=	"";				string Bo_041	=	""; 
			string BC_042	=	"";				string BR_042	=	"";				string Bo_042	=	""; 
			string BC_043	=	"";				string BR_043	=	"";				string Bo_043	=	""; 
 
			string BC_044	=	""	+	BBST01	+	"";string BR_044	=	""	+	BBST01	+	"";string Bo_044	=	""	+	BBST01	+	""; 
			string BC_045	=	""	+	BBST02	+	"";string BR_045	=	""	+	BBST02	+	"";string Bo_045	=	""	+	BBST02	+	""; 
			string BC_046	=	""	+	BBST03	+	"";string BR_046	=	""	+	BBST03	+	"";string Bo_046	=	""	+	BBST03	+	""; 
			string BC_047	=	""	+	BBST04	+	"";string BR_047	=	""	+	BBST04	+	"";string Bo_047	=	""	+	BBST04	+	""; 
			string BC_048	=	""	+	BBST05	+	"";string BR_048	=	""	+	BBST05	+	"";string Bo_048	=	""	+	BBST05	+	""; 
			string BC_049	=	""	+	BBST06	+	"";string BR_049	=	""	+	BBST06	+	"";string Bo_049	=	""	+	BBST06	+	""; 
			string BC_050	=	""	+	BBST07	+	"";string BR_050	=	""	+	BBST07	+	"";string Bo_050	=	""	+	BBST07	+	""; 
			string BC_051	=	""	+	BBST08	+	"";string BR_051	=	""	+	BBST08	+	"";string Bo_051	=	""	+	BBST08	+	""; 
			string BC_052	=	""	+	BBST09	+	"";string BR_052	=	""	+	BBST09	+	"";string Bo_052	=	""	+	BBST09	+	""; 
			string BC_053	=	""	+	BBST10	+	"";string BR_053	=	""	+	BBST10	+	"";string Bo_053	=	""	+	BBST10	+	""; 
			string BC_054	=	""	+	BBST11	+	"";string BR_054	=	""	+	BBST11	+	"";string Bo_054	=	""	+	BBST11	+	""; 
			string BC_055	=	""	+	BBST12	+	"";string BR_055	=	""	+	BBST12	+	"";string Bo_055	=	""	+	BBST12	+	""; 
			string BC_056	=	""	+	BBST13	+	"";string BR_056	=	""	+	BBST13	+	"";string Bo_056	=	""	+	BBST13	+	""; 
			string BC_057	=	""	+	BBST14	+	"";string BR_057	=	""	+	BBST14	+	"";string Bo_057	=	""	+	BBST14	+	""; 
			string BC_058	=	""	+	BBST15	+	"";string BR_058	=	""	+	BBST15	+	"";string Bo_058	=	""	+	BBST15	+	""; 
			string BC_059	=	""	+	BBST16	+	"";string BR_059	=	""	+	BBST16	+	"";string Bo_059	=	""	+	BBST16	+	""; 
			string BC_060	=	""	+	BBST17	+	"";string BR_060	=	""	+	BBST17	+	"";string Bo_060	=	""	+	BBST17	+	""; 
			string BC_061	=	""	+	BBST18	+	"";string BR_061	=	""	+	BBST18	+	"";string Bo_061	=	""	+	BBST18	+	""; 
			string BC_062	=	""	+	BBST19	+	"";string BR_062	=	""	+	BBST19	+	"";string Bo_062	=	""	+	BBST19	+	""; 
			string BC_063	=	""	+	BBST20	+	"";string BR_063	=	""	+	BBST20	+	"";string Bo_063	=	""	+	BBST20	+	""; 
			string BC_064	=	""	+	BBST21	+	"";string BR_064	=	""	+	BBST21	+	"";string Bo_064	=	""	+	BBST21	+	""; 
			string BC_065	=	""	+	BBST22	+	"";string BR_065	=	""	+	BBST22	+	"";string Bo_065	=	""	+	BBST22	+	""; 
			string BC_066	=	""	+	BBST23	+	"";string BR_066	=	""	+	BBST23	+	"";string Bo_066	=	""	+	BBST23	+	""; 
			string BC_067	=	""	+	BBST24	+	"";string BR_067	=	""	+	BBST24	+	"";string Bo_067	=	""	+	BBST24	+	""; 
			string BC_068	=	""	+	BBST25	+	"";string BR_068	=	""	+	BBST25	+	"";string Bo_068	=	""	+	BBST25	+	""; 
			string BC_069	=	""	+	BBST26	+	"";string BR_069	=	""	+	BBST26	+	"";string Bo_069	=	""	+	BBST26	+	""; 
			string BC_070	=	""	+	BBST27	+	"";string BR_070	=	""	+	BBST27	+	"";string Bo_070	=	""	+	BBST27	+	""; 
			string BC_071	=	""	+	BBST28	+	"";string BR_071	=	""	+	BBST28	+	"";string Bo_071	=	""	+	BBST28	+	""; 
			string BC_072	=	""	+	BBST29	+	"";string BR_072	=	""	+	BBST29	+	"";string Bo_072	=	""	+	BBST29	+	""; 
			string BC_073	=	""	+	BBST30	+	"";string BR_073	=	""	+	BBST30	+	"";string Bo_073	=	""	+	BBST30	+	""; 
			string BC_074	=	""	+	BBST31	+	"";string BR_074	=	""	+	BBST31	+	"";string Bo_074	=	""	+	BBST31	+	""; 
			string BC_075	=	""	+	BBST32	+	"";string BR_075	=	""	+	BBST32	+	"";string Bo_075	=	""	+	BBST32	+	""; 
			string BC_076	=	""	+	BBST33	+	"";string BR_076	=	""	+	BBST33	+	"";string Bo_076	=	""	+	BBST33	+	""; 
			string BC_077	=	""	+	BBST34	+	"";string BR_077	=	""	+	BBST34	+	"";string Bo_077	=	""	+	BBST34	+	""; 
			string BC_078	=	""	+	BBST35	+	"";string BR_078	=	""	+	BBST35	+	"";string Bo_078	=	""	+	BBST35	+	""; 
			string BC_079	=	""	+	BBST36	+	"";string BR_079	=	""	+	BBST36	+	"";string Bo_079	=	""	+	BBST36	+	""; 
			string BC_080	=	""	+	BBST37	+	"";string BR_080	=	""	+	BBST37	+	"";string Bo_080	=	""	+	BBST37	+	""; 
			string BC_081	=	""	+	BBST38	+	"";string BR_081	=	""	+	BBST38	+	"";string Bo_081	=	""	+	BBST38	+	""; 
			string BC_082	=	""	+	BBST39	+	"";string BR_082	=	""	+	BBST39	+	"";string Bo_082	=	""	+	BBST39	+	""; 
			string BC_083	=	"";				string BR_083	=	"";				string Bo_083	=	""; 
			string BC_084	=	"";				string BR_084	=	"";				string Bo_084	=	""; 
			string BC_085	=	"";				string BR_085	=	"";				string Bo_085	=	""; 
			string BC_086	=	"";				string BR_086	=	"";				string Bo_086	=	""; 
			string BC_087	=	"";				string BR_087	=	"";				string Bo_087	=	""; 
			string BC_088	=	"";				string BR_088	=	"";				string Bo_088	=	""; 
			if	(!SearchHydro)	{ 
			BC_037	=	"";		BR_037	=	"";	Bo_037	=	""; 
			BC_038	=	"";		BR_038	=	"";	Bo_038	=	""; 
			BC_039	=	"";		BR_039	=	"";	Bo_039	=	""; 
			BC_040	=	"";		BR_040	=	"";	Bo_040	=	""; 
 
			BC_041	=	"";		BR_041	=	"";	Bo_041	=	""; 
			BC_042	=	"";		BR_042	=	"";	Bo_042	=	""; 
			BC_043	=	"";		BR_043	=	"";	Bo_043	=	""; 
 
			BC_044	=	""	+	BBST01	+	"";	BR_044	=	""	+	BBST01	+	"";	Bo_044	=	""	+	BBST01	+	""; 
			BC_045	=	""	+	BBST02	+	"";	BR_045	=	""	+	BBST02	+	"";	Bo_045	=	""	+	BBST02	+	""; 
			BC_046	=	""	+	BBST03	+	"";	BR_046	=	""	+	BBST03	+	"";	Bo_046	=	""	+	BBST03	+	""; 
			BC_047	=	""	+	BBST04	+	"";	BR_047	=	""	+	BBST04	+	"";	Bo_047	=	""	+	BBST04	+	""; 
			BC_048	=	""	+	BBST05	+	"";	BR_048	=	""	+	BBST05	+	"";	Bo_048	=	""	+	BBST05	+	""; 
			BC_049	=	""	+	BBST06	+	"";	BR_049	=	""	+	BBST06	+	"";	Bo_049	=	""	+	BBST06	+	""; 
 
			BC_050	=	""	+	BBST07	+	"";	BR_050	=	""	+	BBST07	+	"";	Bo_050	=	""	+	BBST07	+	""; 
			BC_051	=	""	+	BBST08	+	"";	BR_051	=	""	+	BBST08	+	"";	Bo_051	=	""	+	BBST08	+	""; 
			BC_052	=	""	+	BBST09	+	"";	BR_052	=	""	+	BBST09	+	"";	Bo_052	=	""	+	BBST09	+	""; 
			BC_053	=	""	+	BBST10	+	"";	BR_053	=	""	+	BBST10	+	"";	Bo_053	=	""	+	BBST10	+	""; 
			BC_054	=	""	+	BBST11	+	"";	BR_054	=	""	+	BBST11	+	"";	Bo_054	=	""	+	BBST11	+	""; 
			BC_055	=	""	+	BBST12	+	"";	BR_055	=	""	+	BBST12	+	"";	Bo_055	=	""	+	BBST12	+	""; 
			BC_056	=	""	+	BBST13	+	"";	BR_056	=	""	+	BBST13	+	"";	Bo_056	=	""	+	BBST13	+	""; 
			BC_057	=	""	+	BBST14	+	"";	BR_057	=	""	+	BBST14	+	"";	Bo_057	=	""	+	BBST14	+	""; 
			BC_058	=	""	+	BBST15	+	"";	BR_058	=	""	+	BBST15	+	"";	Bo_058	=	""	+	BBST15	+	""; 
			BC_059	=	""	+	BBST16	+	"";	BR_059	=	""	+	BBST16	+	"";	Bo_059	=	""	+	BBST16	+	""; 
 
			BC_060	=	""	+	BBST17	+	"";	BR_060	=	""	+	BBST17	+	"";	Bo_060	=	""	+	BBST17	+	""; 
			BC_061	=	""	+	BBST18	+	"";	BR_061	=	""	+	BBST18	+	"";	Bo_061	=	""	+	BBST18	+	""; 
 
			BC_062	=	""	+	BBST19	+	"";	BR_062	=	""	+	BBST19	+	"";	Bo_062	=	""	+	BBST19	+	""; 
			BC_063	=	""	+	BBST20	+	"";	BR_063	=	""	+	BBST20	+	"";	Bo_063	=	""	+	BBST20	+	""; 
 
			BC_064	=	""	+	BBST21	+	"";	BR_064	=	""	+	BBST21	+	"";	Bo_064	=	""	+	BBST21	+	""; 
			BC_065	=	""	+	BBST22	+	"";	BR_065	=	""	+	BBST22	+	"";	Bo_065	=	""	+	BBST22	+	""; 
 
			BC_066	=	""	+	BBST23	+	"";	BR_066	=	""	+	BBST23	+	"";	Bo_066	=	""	+	BBST23	+	""; 
			BC_067	=	""	+	BBST24	+	"";	BR_067	=	""	+	BBST24	+	"";	Bo_067	=	""	+	BBST24	+	""; 
			BC_068	=	""	+	BBST25	+	"";	BR_068	=	""	+	BBST25	+	"";	Bo_068	=	""	+	BBST25	+	""; 
			BC_069	=	""	+	BBST26	+	"";	BR_069	=	""	+	BBST26	+	"";	Bo_069	=	""	+	BBST26	+	""; 
			BC_070	=	""	+	BBST27	+	"";	BR_070	=	""	+	BBST27	+	"";	Bo_070	=	""	+	BBST27	+	""; 
			BC_071	=	""	+	BBST28	+	"";	BR_071	=	""	+	BBST28	+	"";	Bo_071	=	""	+	BBST28	+	""; 
			BC_072	=	""	+	BBST29	+	"";	BR_072	=	""	+	BBST29	+	"";	Bo_072	=	""	+	BBST29	+	""; 
			BC_073	=	""	+	BBST30	+	"";	BR_073	=	""	+	BBST30	+	"";	Bo_073	=	""	+	BBST30	+	""; 
			BC_074	=	""	+	BBST31	+	"";	BR_074	=	""	+	BBST31	+	"";	Bo_074	=	""	+	BBST31	+	""; 
			BC_075	=	""	+	BBST32	+	"";	BR_075	=	""	+	BBST32	+	"";	Bo_075	=	""	+	BBST32	+	""; 
 
			BC_076	=	""	+	BBST33	+	"";	BR_076	=	""	+	BBST33	+	"";	Bo_076	=	""	+	BBST33	+	""; 
			BC_077	=	""	+	BBST34	+	"";	BR_077	=	""	+	BBST34	+	"";	Bo_077	=	""	+	BBST34	+	""; 
			BC_078	=	""	+	BBST35	+	"";	BR_078	=	""	+	BBST35	+	"";	Bo_078	=	""	+	BBST35	+	""; 
			BC_079	=	""	+	BBST36	+	"";	BR_079	=	""	+	BBST36	+	"";	Bo_079	=	""	+	BBST36	+	""; 
			BC_080	=	""	+	BBST37	+	"";	BR_080	=	""	+	BBST37	+	"";	Bo_080	=	""	+	BBST37	+	""; 
			BC_081	=	""	+	BBST38	+	"";	BR_081	=	""	+	BBST38	+	"";	Bo_081	=	""	+	BBST38	+	""; 
			BC_082	=	""	+	BBST39	+	"";	BR_082	=	""	+	BBST39	+	"";	Bo_082	=	""	+	BBST39	+	""; 
			BC_083	=	"";		BR_083	=	"";		Bo_083	=	""; 
			BC_084	=	"";		BR_084	=	"";		Bo_084	=	""; 
			BC_085	=	"";		BR_085	=	"";		Bo_085	=	""; 
			BC_086	=	"";		BR_086	=	"";		Bo_086	=	""; 
			BC_087	=	"";		BR_087	=	"";		Bo_087	=	""; 
			BC_088	=	"";		BR_088	=	"";		Bo_088	=	""; 
			} 
 
			string pC	=	"";		string pR	=	"";		string pO	=	""; 
			if	(OnlySmallSigns)	{ 
				if	(SearchHydro)	{ 
					BC_037	=	"";				BR_037	=	"";				Bo_037	=	""; 
					BC_038	=	"";				BR_038	=	"";				Bo_038	=	""; 
					BC_039	=	""	+	BBST01	+	"";	BR_039	=	""	+	BBST01	+	"";	Bo_039	=	""	+	BBST01	+	""; 
					BC_040	=	pC	+	BBST02	+	pC;			BR_040	=	pR	+	BBST02	+	pR;			Bo_040	=	pO	+	BBST02	+	pO; 
					BC_041	=	pC	+	BBST03	+	pC;			BR_041	=	pR	+	BBST03	+	pR;			Bo_041	=	pO	+	BBST03	+	pO; 
					BC_042	=	pC	+	BBST04	+	pC;			BR_042	=	pR	+	BBST04	+	pR;			Bo_042	=	pO	+	BBST04	+	pO; 
					BC_043	=	pC	+	BBST05	+	pC;			BR_043	=	pR	+	BBST05	+	pR;			Bo_043	=	pO	+	BBST05	+	pO; 
					BC_044	=	pC	+	BBST06	+	pC;			BR_044	=	pR	+	BBST06	+	pR;			Bo_044	=	pO	+	BBST06	+	pO; 
					BC_045	=	pC	+	BBST07	+	pC;			BR_045	=	pR	+	BBST07	+	pR;			Bo_045	=	pO	+	BBST07	+	pO; 
					BC_046	=	""	+	BBST08	+	"";	BR_046	=	""	+	BBST08	+	"";	Bo_046	=	""	+	BBST08	+	""; 
					BC_047	=	pC	+	BBST09	+	pC;			BR_047	=	pR	+	BBST09	+	pR;			Bo_047	=	pO	+	BBST09	+	pO; 
					BC_048	=	""	+	BBST10	+	"";	BR_048	=	""	+	BBST10	+	"";	Bo_048	=	""	+	BBST10	+	""; 
					BC_049	=	pC	+	BBST11	+	pC;			BR_049	=	pR	+	BBST11	+	pR;			Bo_049	=	pO	+	BBST11	+	pO; 
					BC_050	=	pC	+	BBST12	+	pC;			BR_050	=	pR	+	BBST12	+	pR;			Bo_050	=	pO	+	BBST12	+	pO; 
					BC_051	=	""	+	BBST13	+	"";	BR_051	=	""	+	BBST13	+	"";	Bo_051	=	""	+	BBST13	+	""; 
					BC_052	=	pC	+	BBST14	+	pC;			BR_052	=	pR	+	BBST14	+	pR;			Bo_052	=	pO	+	BBST14	+	pO; 
					BC_053	=	""	+	BBST15	+	"";	BR_053	=	""	+	BBST15	+	"";	Bo_053	=	""	+	BBST15	+	""; 
					BC_054	=	pC	+	BBST16	+	pC;			BR_054	=	pR	+	BBST16	+	pR;			Bo_054	=	pO	+	BBST16	+	pO; 
					BC_055	=	pC	+	BBST17	+	pC;			BR_055	=	pR	+	BBST17	+	pR;			Bo_055	=	pO	+	BBST17	+	pO; 
					BC_056	=	pC	+	BBST18	+	pC;			BR_056	=	pR	+	BBST18	+	pR;			Bo_056	=	pO	+	BBST18	+	pO; 
					BC_057	=	pC	+	BBST19	+	pC;			BR_057	=	pR	+	BBST19	+	pR;			Bo_057	=	pO	+	BBST19	+	pO; 
					BC_058	=	pC	+	BBST20	+	pC;			BR_058	=	pR	+	BBST20	+	pR;			Bo_058	=	pO	+	BBST20	+	pO; 
					BC_059	=	pC	+	BBST21	+	pC;			BR_059	=	pR	+	BBST21	+	pR;			Bo_059	=	pO	+	BBST21	+	pO; 
					BC_060	=	pC	+	BBST22	+	pC;			BR_060	=	pR	+	BBST22	+	pR;			Bo_060	=	pO	+	BBST22	+	pO; 
					BC_061	=	pC	+	BBST23	+	pC;			BR_061	=	pR	+	BBST23	+	pR;			Bo_061	=	pO	+	BBST23	+	pO; 
					BC_062	=	pC	+	BBST24	+	pC;			BR_062	=	pR	+	BBST24	+	pR;			Bo_062	=	pO	+	BBST24	+	pO; 
					BC_063	=	"";				BR_063	=	"";				Bo_063	=	""; 
				}	else	{ 
					pC	=	"";		pR	=	"";		pO	=	""; 
				 
					BC_037	=	"";				BR_037	=	"";				Bo_037	=	""; 
					BC_038	=	"";				BR_038	=	"";				Bo_038	=	""; 
					BC_039	=	""	+	BBST01	+	"";	BR_039	=	""	+	BBST01	+	"";	Bo_039	=	""	+	BBST01	+	""; 
					BC_040	=	""	+	BBST02	+	"";	BR_040	=	""	+	BBST02	+	"";	Bo_040	=	""	+	BBST02	+	""; 
					BC_041	=	""	+	BBST03	+	"";	BR_041	=	""	+	BBST03	+	"";	Bo_041	=	""	+	BBST03	+	""; 
					BC_042	=	""	+	BBST04	+	"";	BR_042	=	""	+	BBST04	+	"";	Bo_042	=	""	+	BBST04	+	""; 
					BC_043	=	""	+	BBST05	+	"";	BR_043	=	""	+	BBST05	+	"";	Bo_043	=	""	+	BBST05	+	""; 
					BC_044	=	""	+	BBST06	+	"";	BR_044	=	""	+	BBST06	+	"";	Bo_044	=	""	+	BBST06	+	""; 
					BC_045	=	""	+	BBST07	+	"";	BR_045	=	""	+	BBST07	+	"";	Bo_045	=	""	+	BBST07	+	""; 
					BC_046	=	""	+	BBST08	+	"";	BR_046	=	""	+	BBST08	+	"";	Bo_046	=	""	+	BBST08	+	""; 
					BC_047	=	""	+	BBST09	+	"";	BR_047	=	""	+	BBST09	+	"";	Bo_047	=	""	+	BBST09	+	""; 
					BC_048	=	""	+	BBST10	+	"";	BR_048	=	""	+	BBST10	+	"";	Bo_048	=	""	+	BBST10	+	""; 
					BC_049	=	""	+	BBST11	+	"";	BR_049	=	""	+	BBST11	+	"";	Bo_049	=	""	+	BBST11	+	""; 
					BC_050	=	""	+	BBST12	+	"";	BR_050	=	""	+	BBST12	+	"";	Bo_050	=	""	+	BBST12	+	""; 
					BC_051	=	""	+	BBST13	+	"";	BR_051	=	""	+	BBST13	+	"";	Bo_051	=	""	+	BBST13	+	""; 
					BC_052	=	""	+	BBST14	+	"";	BR_052	=	""	+	BBST14	+	"";	Bo_052	=	""	+	BBST14	+	""; 
					BC_053	=	""	+	BBST15	+	"";	BR_053	=	""	+	BBST15	+	"";	Bo_053	=	""	+	BBST15	+	""; 
					BC_054	=	""	+	BBST16	+	"";	BR_054	=	""	+	BBST16	+	"";	Bo_054	=	""	+	BBST16	+	""; 
					BC_055	=	""	+	BBST17	+	"";	BR_055	=	""	+	BBST17	+	"";	Bo_055	=	""	+	BBST17	+	""; 
					BC_056	=	""	+	BBST18	+	"";	BR_056	=	""	+	BBST18	+	"";	Bo_056	=	""	+	BBST18	+	""; 
					BC_057	=	""	+	BBST19	+	"";	BR_057	=	""	+	BBST19	+	"";	Bo_057	=	""	+	BBST19	+	""; 
					BC_058	=	""	+	BBST20	+	"";	BR_058	=	""	+	BBST20	+	"";	Bo_058	=	""	+	BBST20	+	""; 
					BC_059	=	""	+	BBST21	+	"";	BR_059	=	""	+	BBST21	+	"";	Bo_059	=	""	+	BBST21	+	""; 
					BC_060	=	""	+	BBST22	+	"";	BR_060	=	""	+	BBST22	+	"";	Bo_060	=	""	+	BBST22	+	""; 
					BC_061	=	""	+	BBST23	+	"";	BR_061	=	""	+	BBST23	+	"";	Bo_061	=	""	+	BBST23	+	""; 
					BC_062	=	""	+	BBST24	+	"";	BR_062	=	""	+	BBST24	+	"";	Bo_062	=	""	+	BBST24	+	""; 
					BC_063	=	"";				BR_063	=	"";				Bo_063	=	""; 
				} 
			} 
			string liX037	=	"";string liX038	=	"";string liX039	=	""; 
			string liX040	=	"";string liX041	=	"";string liX042	=	"";string liX043	=	"";string liX044	=	"";string liX045	=	"";string liX046	=	"";string liX047	=	"";string liX048	=	"";string liX049	=	""; 
			string liX050	=	"";string liX051	=	"";string liX052	=	"";string liX053	=	"";string liX054	=	"";string liX055	=	"";string liX056	=	"";string liX057	=	"";string liX058	=	"";string liX059	=	""; 
			string liX060	=	"";string liX061	=	"";string liX062	=	"";string liX063	=	"";string liX064	=	"";string liX065	=	"";string liX066	=	"";string liX067	=	"";string liX068	=	"";string liX069	=	""; 
			string liX070	=	"";string liX071	=	"";string liX072	=	"";string liX073	=	"";string liX074	=	"";string liX075	=	"";string liX076	=	"";string liX077	=	"";string liX078	=	"";string liX079	=	""; 
			string liX080	=	"";string liX081	=	"";string liX082	=	"";string liX083	=	"";string liX084	=	"";string liX085	=	"";string liX086	=	"";string liX087	=	"";string liX088	=	""; 
			 
			string Px	=	""; 
			bool Cyan_on	= false; 
			bool Orange_on	= false; 
			bool CheckToShow	= false; 
			bool CheckToShowHydro	= false; 
			 
			//------	Check if is Hydrotank or Oxygentank 
			if (tank_X_Typ.Contains(Hydro_ident_String)) 
			{ 
				if	(SearchHydro)	{	CheckToShowHydro	=	true;	} 
			} else {	 
			//------	Oxygentanks 
				if	(!SearchHydro)	{	CheckToShowHydro	=	true;	} 
			}// End of Check Hydrotank or Oxygentank	---------------------------------------------------------------------------------- 
 
			if	(OnlyTanksWithNameTag)	{ 
				if (OnlyNameTag)	{ 
					if	(CheckToShowHydro)	{	CheckToShow	= true;	} 
				} 
			}	else	{ 
					if	(CheckToShowHydro)	{	CheckToShow	= true;	} 
			} 
			 
			if	(CheckToShow)	{ 
				//if Tank Charging 
				if			(Tank_IsCharging)	{ 
					Orange_on	= true; 
				}	else if	(Tank_Stockpile)	{ 
				//if Tank OnlyRecharge 
					Orange_on	= true; 
				}	else if	(!Tank_IsFunctional)	{ 
				//if Tank Not Functional 
					if		(OnlySmallSigns)	{	Px	=	P2;	} 
					else 								{	Px	=	P2;	} 
						liX037	+=	Px	+	BR_037;liX038	+=	Px	+	BR_038;liX039	+=	Px	+	BR_039;liX040	+=	Px	+	BR_040;liX041	+=	Px	+	BR_041;liX042	+=	Px	+	BR_042;liX043	+=	Px	+	BR_043; 
						liX044	+=	Px	+	BR_044;liX045	+=	Px	+	BR_045;liX046	+=	Px	+	BR_046;liX047	+=	Px	+	BR_047;liX048	+=	Px	+	BR_048;liX049	+=	Px	+	BR_049;liX050	+=	Px	+	BR_050; 
						liX051	+=	Px	+	BR_051;liX052	+=	Px	+	BR_052;liX053	+=	Px	+	BR_053;liX054	+=	Px	+	BR_054;liX055	+=	Px	+	BR_055;liX056	+=	Px	+	BR_056;liX057	+=	Px	+	BR_057; 
						liX058	+=	Px	+	BR_058;liX059	+=	Px	+	BR_059;liX060	+=	Px	+	BR_060;liX061	+=	Px	+	BR_061;liX062	+=	Px	+	BR_062;liX063	+=	Px	+	BR_063; 
					if	(TankCountX	< WideMinValue)	 { 
						liX064	+=	Px	+	BR_064;liX065	+=	Px	+	BR_065;liX066	+=	Px	+	BR_066;liX067	+=	Px	+	BR_067;liX068	+=	Px	+	BR_068;liX069	+=	Px	+	BR_069; 
						liX070	+=	Px	+	BR_070;liX071	+=	Px	+	BR_071;liX072	+=	Px	+	BR_072;liX073	+=	Px	+	BR_073;liX074	+=	Px	+	BR_074;liX075	+=	Px	+	BR_075;liX076	+=	Px	+	BR_076;liX077	+=	Px	+	BR_077;liX078	+=	Px	+	BR_078;liX079	+=	Px	+	BR_079; 
						liX080	+=	Px	+	BR_080;liX081	+=	Px	+	BR_081;liX082	+=	Px	+	BR_082;liX083	+=	Px	+	BR_083;liX084	+=	Px	+	BR_084;liX085	+=	Px	+	BR_085;liX086	+=	Px	+	BR_086;liX087	+=	Px	+	BR_087;liX088	+=	Px	+	BR_088; 
					} 
				}	else if	(!Tank_IsOff)	{ 
				//if Tank OffliX 
					if		(OnlySmallSigns)	{	Px	=	P2;	} 
					else 								{	Px	=	P2;	} 
						liX037	+=	Px	+	BR_037;liX038	+=	Px	+	BR_038;liX039	+=	Px	+	BR_039;liX040	+=	Px	+	BR_040;liX041	+=	Px	+	BR_041;liX042	+=	Px	+	BR_042;liX043	+=	Px	+	BR_043; 
						liX044	+=	Px	+	BR_044;liX045	+=	Px	+	BR_045;liX046	+=	Px	+	BR_046;liX047	+=	Px	+	BR_047;liX048	+=	Px	+	BR_048;liX049	+=	Px	+	BR_049;liX050	+=	Px	+	BR_050; 
						liX051	+=	Px	+	BR_051;liX052	+=	Px	+	BR_052;liX053	+=	Px	+	BR_053;liX054	+=	Px	+	BR_054;liX055	+=	Px	+	BR_055;liX056	+=	Px	+	BR_056;liX057	+=	Px	+	BR_057; 
						liX058	+=	Px	+	BR_058;liX059	+=	Px	+	BR_059;liX060	+=	Px	+	BR_060;liX061	+=	Px	+	BR_061;liX062	+=	Px	+	BR_062;liX063	+=	Px	+	BR_063; 
					if	(TankCountX	< WideMinValue)	 { 
						liX064	+=	Px	+	BR_064;liX065	+=	Px	+	BR_065;liX066	+=	Px	+	BR_066;liX067	+=	Px	+	BR_067;liX068	+=	Px	+	BR_068;liX069	+=	Px	+	BR_069; 
						liX070	+=	Px	+	BR_070;liX071	+=	Px	+	BR_071;liX072	+=	Px	+	BR_072;liX073	+=	Px	+	BR_073;liX074	+=	Px	+	BR_074;liX075	+=	Px	+	BR_075;liX076	+=	Px	+	BR_076;liX077	+=	Px	+	BR_077;liX078	+=	Px	+	BR_078;liX079	+=	Px	+	BR_079; 
						liX080	+=	Px	+	BR_080;liX081	+=	Px	+	BR_081;liX082	+=	Px	+	BR_082;liX083	+=	Px	+	BR_083;liX084	+=	Px	+	BR_084;liX085	+=	Px	+	BR_085;liX086	+=	Px	+	BR_086;liX087	+=	Px	+	BR_087;liX088	+=	Px	+	BR_088; 
					} 
				}	else	{ 
					Cyan_on	= true; 
				} 
				 
				if	(Cyan_on)	{ 
					if		(OnlySmallSigns)	{	Px	=	P2;	} 
					else 								{	Px	=	P2;	} 
 
						liX037	+=	Px	+	BC_037;liX038	+=	Px	+	BC_038;liX039	+=	Px	+	BC_039;liX040	+=	Px	+	BC_040;liX041	+=	Px	+	BC_041;liX042	+=	Px	+	BC_042;liX043	+=	Px	+	BC_043; 
						liX044	+=	Px	+	BC_044;liX045	+=	Px	+	BC_045;liX046	+=	Px	+	BC_046;liX047	+=	Px	+	BC_047;liX048	+=	Px	+	BC_048;liX049	+=	Px	+	BC_049;liX050	+=	Px	+	BC_050; 
						liX051	+=	Px	+	BC_051;liX052	+=	Px	+	BC_052;liX053	+=	Px	+	BC_053;liX054	+=	Px	+	BC_054;liX055	+=	Px	+	BC_055;liX056	+=	Px	+	BC_056;liX057	+=	Px	+	BC_057; 
						liX058	+=	Px	+	BC_058;liX059	+=	Px	+	BC_059;liX060	+=	Px	+	BC_060;liX061	+=	Px	+	BC_061;liX062	+=	Px	+	BC_062;liX063	+=	Px	+	BC_063; 
					if	(TankCountX	< WideMinValue)	 { 
						liX064	+=	Px	+	BC_064;liX065	+=	Px	+	BC_065;liX066	+=	Px	+	BC_066;liX067	+=	Px	+	BC_067;liX068	+=	Px	+	BC_068;liX069	+=	Px	+	BC_069; 
						liX070	+=	Px	+	BC_070;liX071	+=	Px	+	BC_071;liX072	+=	Px	+	BC_072;liX073	+=	Px	+	BC_073;liX074	+=	Px	+	BC_074;liX075	+=	Px	+	BC_075;liX076	+=	Px	+	BC_076;liX077	+=	Px	+	BC_077;liX078	+=	Px	+	BC_078;liX079	+=	Px	+	BC_079; 
						liX080	+=	Px	+	BC_080;liX081	+=	Px	+	BC_081;liX082	+=	Px	+	BC_082;liX083	+=	Px	+	BC_083;liX084	+=	Px	+	BC_084;liX085	+=	Px	+	BC_085;liX086	+=	Px	+	BC_086;liX087	+=	Px	+	BC_087;liX088	+=	Px	+	BC_088; 
					} 
				} 
				if	(Orange_on)	{ 
					if		(OnlySmallSigns)	{	Px	=	P2;	} 
					else 								{	Px	=	P2;	} 
						liX037	+=	Px	+	Bo_037;liX038	+=	Px	+	Bo_038;liX039	+=	Px	+	Bo_039;liX040	+=	Px	+	Bo_040;liX041	+=	Px	+	Bo_041;liX042	+=	Px	+	Bo_042;liX043	+=	Px	+	Bo_043; 
						liX044	+=	Px	+	Bo_044;liX045	+=	Px	+	Bo_045;liX046	+=	Px	+	Bo_046;liX047	+=	Px	+	Bo_047;liX048	+=	Px	+	Bo_048;liX049	+=	Px	+	Bo_049;liX050	+=	Px	+	Bo_050; 
						liX051	+=	Px	+	Bo_051;liX052	+=	Px	+	Bo_052;liX053	+=	Px	+	Bo_053;liX054	+=	Px	+	Bo_054;liX055	+=	Px	+	Bo_055;liX056	+=	Px	+	Bo_056;liX057	+=	Px	+	Bo_057; 
						liX058	+=	Px	+	Bo_058;liX059	+=	Px	+	Bo_059;liX060	+=	Px	+	Bo_060;liX061	+=	Px	+	Bo_061;liX062	+=	Px	+	Bo_062;liX063	+=	Px	+	Bo_063; 
					if	(TankCountX	< WideMinValue)	 { 
						liX064	+=	Px	+	Bo_064;liX065	+=	Px	+	Bo_065;liX066	+=	Px	+	Bo_066;liX067	+=	Px	+	Bo_067;liX068	+=	Px	+	Bo_068;liX069	+=	Px	+	Bo_069; 
						liX070	+=	Px	+	Bo_070;liX071	+=	Px	+	Bo_071;liX072	+=	Px	+	Bo_072;liX073	+=	Px	+	Bo_073;liX074	+=	Px	+	Bo_074;liX075	+=	Px	+	Bo_075;liX076	+=	Px	+	Bo_076;liX077	+=	Px	+	Bo_077;liX078	+=	Px	+	Bo_078;liX079	+=	Px	+	Bo_079; 
						liX080	+=	Px	+	Bo_080;liX081	+=	Px	+	Bo_081;liX082	+=	Px	+	Bo_082;liX083	+=	Px	+	Bo_083;liX084	+=	Px	+	Bo_084;liX085	+=	Px	+	Bo_085;liX086	+=	Px	+	Bo_086;liX087	+=	Px	+	Bo_087;liX088	+=	Px	+	Bo_088; 
					} 
				} 
				 
				//------	Check if is Hydrotank or Oxygentank 
				if (tank_X_Typ.Contains(Hydro_ident_String)) 
				{//------	Hydrotanks 
					AmountOfHydroTank += 1;		// Count amout of all Hydrotanks 
					// Count all stored fuel 
					if	(OnlyTanksWithNameTag)	{	if (OnlyNameTag)	{	hydro_Fill_All_L_F	+=	Filled_Value_Current_L_F;	}} 
					else	{	hydro_Fill_All_L_F	+=	Filled_Value_Current_L_F;	}	 
				 
					// If is Working/Online 
					if (allTanks_list[i].IsWorking)	 
					{	// if is Functional 
						if (allTanks_list[i].IsFunctional)  
						{	AmountOfHydroTank_Working += 1; 
 
						} else { 
						// if is NOT Functional 
 
						} 
					} else { 
						// If is Offline & Functional 
						if (allTanks_list[i].IsFunctional)  
						{ 
 
						} else { 
						// if is Offline & NOT Functional 
 
						} 
					} 
				} else {	 
				//------	Oxygentanks 
					AmountOfOxygenTank += 1;	// Count amout of all Oxgentanks 
					// Count all stored fuel 
					if	(OnlyTanksWithNameTag)	{ 
						if (OnlyNameTag)	{	oxy_Fill_All_L_F	+=	Filled_Value_Current_L_F;	} 
					}	else	{	oxy_Fill_All_L_F	+=	Filled_Value_Current_L_F;	}	 
					// If is Working / Online 
					if (allTanks_list[i].IsWorking)  
					{	// if is Functional 
						if (allTanks_list[i].IsFunctional)	{	AmountOfOxygenTank_Working += 1;	} 
					} 
				}// End of Check Hydrotank or Oxygentank	---------------------------------------------------------------------------------- 
			} 
 
			if	(SearchHydro)	{	AmountOfTank	=	AmountOfHydroTank;	Fill_All_L_F	=	hydro_Fill_All_L_F;	} 
			else	{	AmountOfTank	=	AmountOfOxygenTank;	Fill_All_L_F	=	oxy_Fill_All_L_F;	} 
			 
			int Am50_10To20		=	0; 
			int Am50_20To40		=	0; 
			int Am50_30To60		=	0; 
			int Am50_40To80		=	0; 
			int Am50_50To100	=	0; 
			int Am10_5To10		=	0; 
 
			if	(WideLCD)	{ 
			Am50_10To20		=	21; 
			Am50_20To40		=	41; 
			Am50_30To60		=	61; 
			Am50_40To80		=	81; 
			Am50_50To100	=	80;	 
 
			Am10_5To10		=	11; 
			}	else	{ 
			Am50_10To20		=	11; 
			Am50_20To40		=	21; 
			Am50_30To60		=	31; 
			Am50_40To80		=	41; 
			Am50_50To100	=	40; 
 
			Am10_5To10		=	6; 
			} 
 
			if		(OnlySmallSigns)	{ 
			// if batt Amount 19-50 
				if			(AmountOfTank	< Am50_10To20)	{ 
					//0-10 
					li035	+=	liX037;li036	+=	liX038;li037	+=	liX039;li038	+=	liX040;li039	+=	liX041;li040	+=	liX042;li041	+=	liX043;li042	+=	liX044;li043	+=	liX045;li044	+=	liX046; 
					li045	+=	liX047;li046	+=	liX048;li047	+=	liX049;li048	+=	liX050;li049	+=	liX051;li050	+=	liX052;li051	+=	liX053;li052	+=	liX054;li053	+=	liX055;li054	+=	liX056; 
					li055	+=	liX057;li056	+=	liX058;li057	+=	liX059;li058	+=	liX060;li059	+=	liX061;li060	+=	liX062;li061	+=	liX063; 
					li062	=	PxFull; 
					li063	=	PxFull; 
				}	else if	(AmountOfTank	< Am50_20To40)	{ 
					//11-20 
					li064	+=	liX037;li065	+=	liX038;li066	+=	liX039;li067	+=	liX040;li068	+=	liX041;li069	+=	liX042;li070	+=	liX043;li071	+=	liX044;li072	+=	liX045;li073	+=	liX046; 
					li074	+=	liX047;li075	+=	liX048;li076	+=	liX049;li077	+=	liX050;li078	+=	liX051;li079	+=	liX052;li080	+=	liX053;li081	+=	liX054;li082	+=	liX055;li083	+=	liX056; 
					li084	+=	liX057;li085	+=	liX058;li086	+=	liX059;li087	+=	liX060;li088	+=	liX061;li089	+=	liX062;li090	+=	liX063; 
					li091	=	PxFull; 
					li092	=	PxFull; 
				}	else if	(AmountOfTank	< Am50_30To60)	{ 
					//21-30 
					li093	+=	liX037;li094	+=	liX038;li095	+=	liX039;li096	+=	liX040;li097	+=	liX041;li098	+=	liX042;li099	+=	liX043;li100	+=	liX044;li101	+=	liX045;li102	+=	liX046; 
					li103	+=	liX047;li104	+=	liX048;li105	+=	liX049;li106	+=	liX050;li107	+=	liX051;li108	+=	liX052;li109	+=	liX053;li110	+=	liX054;li111	+=	liX055;li112	+=	liX056; 
					li113	+=	liX057;li114	+=	liX058;li115	+=	liX059;li116	+=	liX060;li117	+=	liX061;li118	+=	liX062;li119	+=	liX063; 
					li120	=	PxFull; 
					li121	=	PxFull; 
				}	else if	(AmountOfTank	< Am50_40To80)	{ 
					//31-40 
					li122	+=	liX037;li123	+=	liX038;li124	+=	liX039;li125	+=	liX040;li126	+=	liX041;li127	+=	liX042;li128	+=	liX043;li129	+=	liX044;li130	+=	liX045;li131	+=	liX046; 
					li132	+=	liX047;li133	+=	liX048;li134	+=	liX049;li135	+=	liX050;li136	+=	liX051;li137	+=	liX052;li138	+=	liX053;li139	+=	liX054;li140	+=	liX055;li141	+=	liX056; 
					li142	+=	liX057;li143	+=	liX058;li144	+=	liX059;li145	+=	liX060;li146	+=	liX061;li147	+=	liX062;li148	+=	liX063; 
					li149	=	PxFull; 
					li150	=	PxFull; 
				}	else if	(AmountOfTank	> Am50_50To100)	{ 
					//41-50 
					li151	+=	liX037;li152	+=	liX038;li153	+=	liX039;li154	+=	liX040;li155	+=	liX041;li156	+=	liX042;li157	+=	liX043;li158	+=	liX044;li159	+=	liX045;li160	+=	liX046; 
					li161	+=	liX047;li162	+=	liX048;li163	+=	liX049;li164	+=	liX050;li165	+=	liX051;li166	+=	liX052;li167	+=	liX053;li168	+=	liX054;li169	+=	liX055;li170	+=	liX056; 
					li171	+=	liX057;li172	+=	liX058;li173	+=	liX059;li174	+=	liX060;li175	+=	liX061;li176	+=	liX062;li177	+=	liX063; 
				}			 
			}	else { 
			// if batt Amount 1-10 
				if			(AmountOfTank	< Am10_5To10)	{ 
					li035	=	PxFull; 
					li036	=	PxFull; 
				 
					li037	+=	liX037;li038	+=	liX038;li039	+=	liX039; 
					li040	+=	liX040;li041	+=	liX041;li042	+=	liX042;li043	+=	liX043;li044	+=	liX044;li045	+=	liX045;li046	+=	liX046;li047	+=	liX047;li048	+=	liX048;li049	+=	liX049; 
					li050	+=	liX050;li051	+=	liX051;li052	+=	liX052;li053	+=	liX053;li054	+=	liX054;li055	+=	liX055;li056	+=	liX056;li057	+=	liX057;li058	+=	liX058;li059	+=	liX059; 
					li060	+=	liX060;li061	+=	liX061;li062	+=	liX062;li063	+=	liX063;li064	+=	liX064;li065	+=	liX065;li066	+=	liX066;li067	+=	liX067;li068	+=	liX068;li069	+=	liX069; 
					li070	+=	liX070;li071	+=	liX071;li072	+=	liX072;li073	+=	liX073;li074	+=	liX074;li075	+=	liX075;li076	+=	liX076;li077	+=	liX077;li078	+=	liX078;li079	+=	liX079; 
					li080	+=	liX080;li081	+=	liX081;li082	+=	liX082;li083	+=	liX083;li084	+=	liX084;li085	+=	liX085;li086	+=	liX086;li087	+=	liX087;li088	+=	liX088; 
					 
					li089	=	PxFull; 
					li090	=	PxFull; 
					li091	=	PxFull; 
					li092	=	PxFull; 
					li093	=	PxFull; 
				}	else	{ 
					//6-10 
					li094	+=	liX037;li095	+=	liX038;li096	+=	liX039; 
					li097	+=	liX040;li098	+=	liX041;li099	+=	liX042;li100	+=	liX043;li101	+=	liX044;li102	+=	liX045;li103	+=	liX046;li104	+=	liX047;li105	+=	liX048;li106	+=	liX049; 
					li107	+=	liX050;li108	+=	liX051;li109	+=	liX052;li110	+=	liX053;li111	+=	liX054;li112	+=	liX055;li113	+=	liX056;li114	+=	liX057;li115	+=	liX058;li116	+=	liX059; 
					li117	+=	liX060;li118	+=	liX061;li119	+=	liX062;li120	+=	liX063;li121	+=	liX064;li122	+=	liX065;li123	+=	liX066;li124	+=	liX067;li125	+=	liX068;li126	+=	liX069; 
					li127	+=	liX070;li128	+=	liX071;li129	+=	liX072;li130	+=	liX073;li131	+=	liX074;li132	+=	liX075;li133	+=	liX076;li134	+=	liX077;li135	+=	liX078;li136	+=	liX079; 
					li137	+=	liX080;li138	+=	liX081;li139 	+=	liX082;li140	+=	liX083;li141	+=	liX084;li142	+=	liX085;li143	+=	liX086;li144	+=	liX087;li145	+=	liX088; 
				}		 
			} 
		} // End For Loop 
 
		string SymAmX018	=	P11; 
		string SymAmX019	=	P11; 
		string SymAmX020	=	P11; 
		string SymAmX021	=	P11; 
		string SymAmX022	=	P11; 
		string SymAmX023	=	P11; 
		string SymAmX024	=	P11; 
		string SymAmX025	=	P11; 
		string SymAmX026	=	P11; 
		string SymAmX027	=	P11; 
		string SymAmX028	=	P11; 
		string SymAmX029	=	P11; 
		string SymAmX030	=	P11; 
 
		string Am_NX_018	=	P11;string Am_NXX_018	=	P11;string Am_NXXX_018	=	P11;string BatSpLi018	=	P2; 
		string Am_NX_019	=	P11;string Am_NXX_019	=	P11;string Am_NXXX_019	=	P11;string BatSpLi019	=	P2; 
		string Am_NX_020	=	P11;string Am_NXX_020	=	P11;string Am_NXXX_020	=	P11;string BatSpLi020	=	P2; 
		string Am_NX_021	=	P11;string Am_NXX_021	=	P11;string Am_NXXX_021	=	P11;string BatSpLi021	=	P2; 
		string Am_NX_022	=	P11;string Am_NXX_022	=	P11;string Am_NXXX_022	=	P11;string BatSpLi022	=	P2; 
		string Am_NX_023	=	P11;string Am_NXX_023	=	P11;string Am_NXXX_023	=	P11;string BatSpLi023	=	P2; 
		string Am_NX_024	=	P11;string Am_NXX_024	=	P11;string Am_NXXX_024	=	P11;string BatSpLi024	=	P2; 
		string Am_NX_025	=	P11;string Am_NXX_025	=	P11;string Am_NXXX_025	=	P11;string BatSpLi025	=	P2; 
		string Am_NX_026	=	P11;string Am_NXX_026	=	P11;string Am_NXXX_026	=	P11;string BatSpLi026	=	P2; 
		string Am_NX_027	=	P11;string Am_NXX_027	=	P11;string Am_NXXX_027	=	P11;string BatSpLi027	=	P2; 
		string Am_NX_028	=	P11;string Am_NXX_028	=	P11;string Am_NXXX_028	=	P11;string BatSpLi028	=	P2; 
		string Am_NX_029	=	P11;string Am_NXX_029	=	P11;string Am_NXXX_029	=	P11;string BatSpLi029	=	P2; 
		string Am_NX_030	=	P11;string Am_NXX_030	=	P11;string Am_NXXX_030	=	P11;string BatSpLi030	=	P2; 
		 
		string MW_Nx6_018	=	P11;string MW_Nx5_018	=	P11;string MW_Nx4_018	=	P11;string MW_Nx3_018	=	P11;string MW_NXX_018	=	P11;string MW_NX_018	=	P11; 
		string MW_Nx6_019	=	P11;string MW_Nx5_019	=	P11;string MW_Nx4_019	=	P11;string MW_Nx3_019	=	P11;string MW_NXX_019	=	P11;string MW_NX_019	=	P11; 
		string MW_Nx6_020	=	P11;string MW_Nx5_020	=	P11;string MW_Nx4_020	=	P11;string MW_Nx3_020	=	P11;string MW_NXX_020	=	P11;string MW_NX_020	=	P11; 
		string MW_Nx6_021	=	P11;string MW_Nx5_021	=	P11;string MW_Nx4_021	=	P11;string MW_Nx3_021	=	P11;string MW_NXX_021	=	P11;string MW_NX_021	=	P11; 
		string MW_Nx6_022	=	P11;string MW_Nx5_022	=	P11;string MW_Nx4_022	=	P11;string MW_Nx3_022	=	P11;string MW_NXX_022	=	P11;string MW_NX_022	=	P11; 
		string MW_Nx6_023	=	P11;string MW_Nx5_023	=	P11;string MW_Nx4_023	=	P11;string MW_Nx3_023	=	P11;string MW_NXX_023	=	P11;string MW_NX_023	=	P11; 
		string MW_Nx6_024	=	P11;string MW_Nx5_024	=	P11;string MW_Nx4_024	=	P11;string MW_Nx3_024	=	P11;string MW_NXX_024	=	P11;string MW_NX_024	=	P11; 
		string MW_Nx6_025	=	P11;string MW_Nx5_025	=	P11;string MW_Nx4_025	=	P11;string MW_Nx3_025	=	P11;string MW_NXX_025	=	P11;string MW_NX_025	=	P11; 
		string MW_Nx6_026	=	P11;string MW_Nx5_026	=	P11;string MW_Nx4_026	=	P11;string MW_Nx3_026	=	P11;string MW_NXX_026	=	P11;string MW_NX_026	=	P11; 
		string MW_Nx6_027	=	P11;string MW_Nx5_027	=	P11;string MW_Nx4_027	=	P11;string MW_Nx3_027	=	P11;string MW_NXX_027	=	P11;string MW_NX_027	=	P11; 
		string MW_Nx6_028	=	P11;string MW_Nx5_028	=	P11;string MW_Nx4_028	=	P11;string MW_Nx3_028	=	P11;string MW_NXX_028	=	P11;string MW_NX_028	=	P11; 
		string MW_Nx6_029	=	P11;string MW_Nx5_029	=	P11;string MW_Nx4_029	=	P11;string MW_Nx3_029	=	P11;string MW_NXX_029	=	P11;string MW_NX_029	=	P11; 
		string MW_Nx6_030	=	P11;string MW_Nx5_030	=	P11;string MW_Nx4_030	=	P11;string MW_Nx3_030	=	P11;string MW_NXX_030	=	P11;string MW_NX_030	=	P11; 
		 
		int input		=	0; 
		int Am_NX		=	0; 
		int Am_NXX	=	0; 
		int Am_NXXX	=	0; 
		int MW_NX		=	0; 
		int MW_NXX		=	0; 
		int MW_NXXX		=	0; 
		int MW_NX_XXX	=	0; 
		int MW_NXX_XXX	=	0; 
		int MW_NXXX_XXX	=	0;	 
		if	(TankAmountEnabled) 
		{ 
				SymAmX018	=	""; 
				SymAmX019	=	""; 
				SymAmX020	=	""; 
				SymAmX021	=	""; 
				SymAmX022	=	""; 
				SymAmX023	=	""; 
				SymAmX024	=	""; 
				SymAmX025	=	""; 
				SymAmX026	=	""; 
				SymAmX027	=	""; 
				SymAmX028	=	""; 
				SymAmX029	=	""; 
				SymAmX030	=	""; 
		} 
		//Convert Val-Number seperate Num, for Amount + Stored volume 
		int CounterNumConvert	=	2; 
		int CheckerNumConvert	=	CounterNumConvert; 
		if		(VolumeOnlyInM2)	{ 
			Fill_All_L_F	=	Fill_All_L_F	/	1000;		TankStoredUnit	=	"M3"; 
		}	else { 
			if		(Fill_All_L_F	<	99999999)		{	Fill_All_L_F	=	Fill_All_L_F	/	100;} 
			else 									{	Fill_All_L_F	=	Fill_All_L_F	/	1000;		TankStoredUnit	=	"M3";	} 
		} 
 
 
		int numVal = Convert.ToInt32(Fill_All_L_F); 
 
		for (int i=0; i<CounterNumConvert; i++) 
		{	// Here set your Number to split 
			if		(CheckerNumConvert	==	2)	{input = AmountOfTank;} 
			else if	(CheckerNumConvert	==	1)	{input = numVal;} 
 
			// Create Variables to hold Data 
			int InNoX6 = 0; 
			int InNoX5 = 0; 
			int InNoX4 = 0; 
			int InNoX3 = 0; 
			int InNoX2 = 0; 
			int InNoX1 = 0; 
 
			int InNoX5C = 0; 
			int InNoX4C = 0; 
			int InNoX3C = 0; 
			 
			int InNoX2C = 0; 
			int InNoX1C = 0; 
 
 
			if (input > 99999)  
			{	// Input > then 99 999 (100000 bis 999999) 
				if 		(input > 899999) {	InNoX6 = 9;	InNoX5C = input - 900000;}	// 900000-999999 
				else if (input > 799999) {	InNoX6 = 8;	InNoX5C = input - 800000;}	// 800000-899999 
				else if (input > 699999) {	InNoX6 = 7;	InNoX5C = input - 700000;}	// 700000-799999 
				else if (input > 599999) {	InNoX6 = 6;	InNoX5C = input - 600000;}	// 600000-699999 
				else if (input > 499999) {	InNoX6 = 5;	InNoX5C = input - 500000;}	// 500000-599999 
				else if (input > 399999) {	InNoX6 = 4;	InNoX5C = input - 400000;}	// 400000-499999 
				else if (input > 299999) {	InNoX6 = 3;	InNoX5C = input - 300000;}	// 300000-399999 
				else if (input > 199999) {	InNoX6 = 2;	InNoX5C = input - 200000;}	// 200000-299999 
				else if (input > 99999) {	InNoX6 = 1;	InNoX5C = input - 100000;}	// 100000-199999 
				else if (input < 100000) {	InNoX6 = 0;	InNoX5C = input;}		// 0-99999 
			} 
			 
			if (input < 100000) {	InNoX5C	=	input;	} 
			// Input > then 9 999 (10000 bis 99999) 
			if 		(InNoX5C > 89999) {	InNoX5 = 9;	InNoX4C = InNoX5C - 90000;}	// 90000-99999 
			else if (InNoX5C > 79999) {	InNoX5 = 8;	InNoX4C = InNoX5C - 80000;}	// 80000-89999 
			else if (InNoX5C > 69999) {	InNoX5 = 7;	InNoX4C = InNoX5C - 70000;}	// 70000-79999 
			else if (InNoX5C > 59999) {	InNoX5 = 6;	InNoX4C = InNoX5C - 60000;}	// 60000-69999 
			else if (InNoX5C > 49999) {	InNoX5 = 5;	InNoX4C = InNoX5C - 50000;}	// 50000-59999 
			else if (InNoX5C > 39999) {	InNoX5 = 4;	InNoX4C = InNoX5C - 40000;}	// 40000-49999 
			else if (InNoX5C > 29999) {	InNoX5 = 3;	InNoX4C = InNoX5C - 30000;}	// 30000-39999 
			else if (InNoX5C > 19999) {	InNoX5 = 2;	InNoX4C = InNoX5C - 20000;}	// 20000-29999 
			else if (InNoX5C > 9999) {	InNoX5 = 1;	InNoX4C = InNoX5C - 10000;}	// 10000-19999 
			else if (InNoX5C < 10000) {	InNoX5 = 0;	InNoX4C = InNoX5C;}			// 0-9999 
 
			if (input < 10000) {	InNoX4C	=	input;	} 
			// Input > then 999 (1000 bis 9999) 
			if 		(InNoX4C > 8999) {	InNoX4 = 9;	InNoX3C = InNoX4C - 9000;}	// 9000-9999 
			else if (InNoX4C > 7999) {	InNoX4 = 8;	InNoX3C = InNoX4C - 8000;}	// 8000-8999 
			else if (InNoX4C > 6999) {	InNoX4 = 7;	InNoX3C = InNoX4C - 7000;}	// 7000-7999 
			else if (InNoX4C > 5999) {	InNoX4 = 6;	InNoX3C = InNoX4C - 6000;}	// 6000-6999 
			else if (InNoX4C > 4999) {	InNoX4 = 5;	InNoX3C = InNoX4C - 5000;}	// 5000-5999 
			else if (InNoX4C > 3999) {	InNoX4 = 4;	InNoX3C = InNoX4C - 4000;}	// 4000-4999 
			else if (InNoX4C > 2999) {	InNoX4 = 3;	InNoX3C = InNoX4C - 3000;}	// 3000-3999 
			else if (InNoX4C > 1999) {	InNoX4 = 2;	InNoX3C = InNoX4C - 2000;}	// 2000-2999 
			else if (InNoX4C > 999) {		InNoX4 = 1;	InNoX3C = InNoX4C - 1000;}	// 1000-1999 
			else if (InNoX4C < 1000) {	InNoX4 = 0;	InNoX3C = InNoX4C;}			// 0-999 
 
			if (input < 1000) {	InNoX3C	=	input;	} 
			// Input > then 99 (100 bis 999) 
			if 		(InNoX3C > 899) {	InNoX3 = 9;	InNoX2C = InNoX3C - 900;}	// 900-999 
			else if (InNoX3C > 799) {	InNoX3 = 8;	InNoX2C = InNoX3C - 800;}	// 800-899 
			else if (InNoX3C > 699) {	InNoX3 = 7;	InNoX2C = InNoX3C - 700;}	// 700-799 
			else if (InNoX3C > 599) {	InNoX3 = 6;	InNoX2C = InNoX3C - 600;}	// 600-699 
			else if (InNoX3C > 499) {	InNoX3 = 5;	InNoX2C = InNoX3C - 500;}	// 500-599 
			else if (InNoX3C > 399) {	InNoX3 = 4;	InNoX2C = InNoX3C - 400;}	// 400-499 
			else if (InNoX3C > 299) {	InNoX3 = 3;	InNoX2C = InNoX3C - 300;}	// 300-399 
			else if (InNoX3C > 199) {	InNoX3 = 2;	InNoX2C = InNoX3C - 200;}	// 200-299 
			else if (InNoX3C > 99) {	InNoX3 = 1;	InNoX2C = InNoX3C - 100;}	// 100-199 
			else if (InNoX3C < 100) {	InNoX3 = 0;	InNoX2C = InNoX3C;}		// 0-99 
 
			if (input < 100) {	InNoX2C	=	input;	} 
			// Input > then 9 (10 bis 90) 
			if 		(InNoX2C > 89) {	InNoX2 = 9;	InNoX1C = InNoX2C - 90;}	// 90-99 
			else if (InNoX2C > 79) {	InNoX2 = 8;	InNoX1C = InNoX2C - 80;}	// 80-89 
			else if (InNoX2C > 69) {	InNoX2 = 7;	InNoX1C = InNoX2C - 70;}	// 70-79 
			else if (InNoX2C > 59) {	InNoX2 = 6;	InNoX1C = InNoX2C - 60;}	// 60-69 
			else if (InNoX2C > 49) {	InNoX2 = 5;	InNoX1C = InNoX2C - 50;}	// 50-59 
			else if (InNoX2C > 39) {	InNoX2 = 4;	InNoX1C = InNoX2C - 40;}	// 40-49 
			else if (InNoX2C > 29) {	InNoX2 = 3;	InNoX1C = InNoX2C - 30;}	// 30-39 
			else if (InNoX2C > 19) {	InNoX2 = 2;	InNoX1C = InNoX2C - 20;}	// 20-29 
			else if (InNoX2C > 9) {		InNoX2 = 1;	InNoX1C = InNoX2C - 10;}	// 10-19 
			else if (InNoX2C < 10) {	InNoX2 = 0;	InNoX1C = InNoX2C;}		// 0-9 
 
			if (input < 10) {	InNoX1C	=	input;	} 
			// Input > then 0 (1 bis 9) 
			if 		(InNoX1C > 8) {	InNoX1 = 9;	}	// 9 
			else if (InNoX1C > 7) {	InNoX1 = 8;	}	// 8 
			else if (InNoX1C > 6) {	InNoX1 = 7;	}	// 7 
			else if (InNoX1C > 5) {	InNoX1 = 6;	}	// 6 
			else if (InNoX1C > 4) {	InNoX1 = 5;	}	// 5 
			else if (InNoX1C > 3) {	InNoX1 = 4;	}	// 4 
			else if (InNoX1C > 2) {	InNoX1 = 3;	}	// 3 
			else if (InNoX1C > 1) {	InNoX1 = 2;	}	// 2 
			else if (InNoX1C > 0) {	InNoX1 = 1;	}	// 1 
			else if (InNoX1C < 1) {	InNoX1 = 0;	}	// 0 
 
			if		(CheckerNumConvert	==	2)	{Am_NX		=	InNoX1;Am_NXX	=	InNoX2;Am_NXXX	=	InNoX3;} 
			else if	(CheckerNumConvert	==	1)	{MW_NXXX_XXX	=	InNoX6;MW_NXX_XXX	=	InNoX5;MW_NX_XXX	=	InNoX4;MW_NXXX	=	InNoX3;MW_NXX	=	InNoX2;MW_NX	=	InNoX1;	} 
			CheckerNumConvert	=	CheckerNumConvert -	1; 
		} 
 
		string Chk_Num_018	=	P11;string Chk_Num_019	=	P11;string Chk_Num_020	=	P11;string Chk_Num_021	=	P11;string Chk_Num_022	=	P11;string Chk_Num_023	=	P11;string Chk_Num_024	=	P11; 
		string Chk_Num_025	=	P11;string Chk_Num_026	=	P11;string Chk_Num_027	=	P11;string Chk_Num_028	=	P11;string Chk_Num_029	=	P11;string Chk_Num_030	=	P11; 
		 
		int Chk_Num_X			=	0; 
		int Chk_StateCounter	=	9; 
		int Chk_State			=	Chk_StateCounter; 
 
		for (int i=0; i<Chk_StateCounter; i++) 
		{ 
			if		(Chk_State	==	9)	{Chk_Num_X	=	Am_NXXX;} 
			else if	(Chk_State	==	8)	{Chk_Num_X	=	Am_NXX;} 
			else if	(Chk_State	==	7)	{Chk_Num_X	=	Am_NX;} 
			else if	(Chk_State	==	6)	{Chk_Num_X	=	MW_NXXX_XXX;} 
			else if	(Chk_State	==	5)	{Chk_Num_X	=	MW_NXX_XXX;} 
			else if	(Chk_State	==	4)	{Chk_Num_X	=	MW_NX_XXX;} 
			else if	(Chk_State	==	3)	{Chk_Num_X	=	MW_NXXX;} 
			else if	(Chk_State	==	2)	{Chk_Num_X	=	MW_NXX;} 
			else if	(Chk_State	==	1)	{Chk_Num_X	=	MW_NX;} 
 
			if			(Chk_Num_X	==	9)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	8)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	7)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	6)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	5)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	4)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	3)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	2)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	1)	{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}	else if	(Chk_Num_X	==	0)	{ 
				bool Chk_Num_ShwPx	=	false; 
				if		(Chk_State	==	9)	{	Chk_Num_ShwPx	=	true;	}	 
				else if	(Chk_State	==	8)	{	if	(AmountOfTank	<	10)	{	Chk_Num_ShwPx	=	true;	}}	 
				else if	(Chk_State	==	6)	{	Chk_Num_ShwPx	=	true;	}	 
				else if	(Chk_State	==	5)	{	if	(Fill_All_L_F	<	10000)	{	Chk_Num_ShwPx	=	true;	}}	 
				else if	(Chk_State	==	4)	{	if	(Fill_All_L_F	<	1000)	{	Chk_Num_ShwPx	=	true;	}}	 
				else if	(Chk_State	==	3)	{	if	(Fill_All_L_F	<	100)	{	Chk_Num_ShwPx	=	true;	}}	 
				else if	(Chk_State	==	2)	{	if	(Fill_All_L_F	<	10)		{	Chk_Num_ShwPx	=	true;	}}	 
 
				if	(Chk_Num_ShwPx)	{ 
				Chk_Num_018	=	P11;Chk_Num_019	=	P11;Chk_Num_020	=	P11;Chk_Num_021	=	P11;Chk_Num_022	=	P11;Chk_Num_023	=	P11;Chk_Num_024	=	P11; 
				Chk_Num_025	=	P11;Chk_Num_026	=	P11;Chk_Num_027	=	P11;Chk_Num_028	=	P11;Chk_Num_029	=	P11;Chk_Num_030	=	P11; 
				}	else			{ 
				Chk_Num_018	=	"";Chk_Num_019	=	"";Chk_Num_020	=	"";Chk_Num_021	=	"";Chk_Num_022	=	"";Chk_Num_023	=	""; 
				Chk_Num_024	=	"";Chk_Num_025	=	"";Chk_Num_026	=	"";Chk_Num_027	=	"";Chk_Num_028	=	"";Chk_Num_029	=	"";Chk_Num_030	=	""; 
			}} 
 
			if		(Chk_State	==	9)	{		 
				if	(TankAmountEnabled)	{ 
					Am_NXXX_018	=	Chk_Num_018;Am_NXXX_019	=	Chk_Num_019;Am_NXXX_020	=	Chk_Num_020;Am_NXXX_021	=	Chk_Num_021;Am_NXXX_022	=	Chk_Num_022;Am_NXXX_023	=	Chk_Num_023;Am_NXXX_024	=	Chk_Num_024; 
					Am_NXXX_025	=	Chk_Num_025;Am_NXXX_026	=	Chk_Num_026;Am_NXXX_027	=	Chk_Num_027;Am_NXXX_028	=	Chk_Num_028;Am_NXXX_029	=	Chk_Num_029;Am_NXXX_030	=	Chk_Num_030;	}} 
			else if	(Chk_State	==	8)	{		 
				if	(TankAmountEnabled)	{ 
					Am_NXX_018	=	Chk_Num_018;Am_NXX_019	=	Chk_Num_019;Am_NXX_020	=	Chk_Num_020;Am_NXX_021	=	Chk_Num_021;Am_NXX_022	=	Chk_Num_022;Am_NXX_023	=	Chk_Num_023;Am_NXX_024	=	Chk_Num_024; 
					Am_NXX_025	=	Chk_Num_025;Am_NXX_026	=	Chk_Num_026;Am_NXX_027	=	Chk_Num_027;Am_NXX_028	=	Chk_Num_028;Am_NXX_029	=	Chk_Num_029;Am_NXX_030	=	Chk_Num_030;	}} 
			else if	(Chk_State	==	7)	{		 
				if	(TankAmountEnabled)	{ 
					Am_NX_018	=	Chk_Num_018;Am_NX_019	=	Chk_Num_019;Am_NX_020	=	Chk_Num_020;Am_NX_021	=	Chk_Num_021;Am_NX_022	=	Chk_Num_022;Am_NX_023	=	Chk_Num_023;Am_NX_024	=	Chk_Num_024; 
					Am_NX_025	=	Chk_Num_025;Am_NX_026	=	Chk_Num_026;Am_NX_027	=	Chk_Num_027;Am_NX_028	=	Chk_Num_028;Am_NX_029	=	Chk_Num_029;Am_NX_030	=	Chk_Num_030;}} 
			else if	(Chk_State	==	6)	{		 
				if	(TankAllStoredVolumeEnabled)	{ 
					MW_Nx6_018	=	Chk_Num_018;MW_Nx6_019	=	Chk_Num_019;MW_Nx6_020	=	Chk_Num_020;MW_Nx6_021	=	Chk_Num_021;MW_Nx6_022	=	Chk_Num_022;MW_Nx6_023	=	Chk_Num_023;MW_Nx6_024	=	Chk_Num_024; 
					MW_Nx6_025	=	Chk_Num_025;MW_Nx6_026	=	Chk_Num_026;MW_Nx6_027	=	Chk_Num_027;MW_Nx6_028	=	Chk_Num_028;MW_Nx6_029	=	Chk_Num_029;MW_Nx6_030	=	Chk_Num_030;}} 
			else if	(Chk_State	==	5)	{		 
				if	(TankAllStoredVolumeEnabled)	{ 
					MW_Nx5_018	=	Chk_Num_018;MW_Nx5_019	=	Chk_Num_019;MW_Nx5_020	=	Chk_Num_020;MW_Nx5_021	=	Chk_Num_021;MW_Nx5_022	=	Chk_Num_022;MW_Nx5_023	=	Chk_Num_023;MW_Nx5_024	=	Chk_Num_024; 
					MW_Nx5_025	=	Chk_Num_025;MW_Nx5_026	=	Chk_Num_026;MW_Nx5_027	=	Chk_Num_027;MW_Nx5_028	=	Chk_Num_028;MW_Nx5_029	=	Chk_Num_029;MW_Nx5_030	=	Chk_Num_030;}} 
			else if	(Chk_State	==	4)	{		 
				if	(TankAllStoredVolumeEnabled)	{ 
					MW_Nx4_018	=	Chk_Num_018;MW_Nx4_019	=	Chk_Num_019;MW_Nx4_020	=	Chk_Num_020;MW_Nx4_021	=	Chk_Num_021;MW_Nx4_022	=	Chk_Num_022;MW_Nx4_023	=	Chk_Num_023;MW_Nx4_024	=	Chk_Num_024; 
					MW_Nx4_025	=	Chk_Num_025;MW_Nx4_026	=	Chk_Num_026;MW_Nx4_027	=	Chk_Num_027;MW_Nx4_028	=	Chk_Num_028;MW_Nx4_029	=	Chk_Num_029;MW_Nx4_030	=	Chk_Num_030;}} 
			else if	(Chk_State	==	3)	{		 
				if	(TankAllStoredVolumeEnabled)	{ 
					MW_Nx3_018	=	Chk_Num_018;MW_Nx3_019	=	Chk_Num_019;MW_Nx3_020	=	Chk_Num_020;MW_Nx3_021	=	Chk_Num_021;MW_Nx3_022	=	Chk_Num_022;MW_Nx3_023	=	Chk_Num_023;MW_Nx3_024	=	Chk_Num_024; 
					MW_Nx3_025	=	Chk_Num_025;MW_Nx3_026	=	Chk_Num_026;MW_Nx3_027	=	Chk_Num_027;MW_Nx3_028	=	Chk_Num_028;MW_Nx3_029	=	Chk_Num_029;MW_Nx3_030	=	Chk_Num_030;}} 
			else if	(Chk_State	==	2)	{		 
				if	(TankAllStoredVolumeEnabled)	{ 
					MW_NXX_018	=	Chk_Num_018;MW_NXX_019	=	Chk_Num_019;MW_NXX_020	=	Chk_Num_020;MW_NXX_021	=	Chk_Num_021;MW_NXX_022	=	Chk_Num_022;MW_NXX_023	=	Chk_Num_023;MW_NXX_024	=	Chk_Num_024; 
					MW_NXX_025	=	Chk_Num_025;MW_NXX_026	=	Chk_Num_026;MW_NXX_027	=	Chk_Num_027;MW_NXX_028	=	Chk_Num_028;MW_NXX_029	=	Chk_Num_029;MW_NXX_030	=	Chk_Num_030;}} 
			else if	(Chk_State	==	1)	{		 
				if	(TankAllStoredVolumeEnabled)	{ 
					MW_NX_018	=	Chk_Num_018;MW_NX_019	=	Chk_Num_019;MW_NX_020	=	Chk_Num_020;MW_NX_021	=	Chk_Num_021;MW_NX_022	=	Chk_Num_022;MW_NX_023	=	Chk_Num_023;MW_NX_024	=	Chk_Num_024; 
					MW_NX_025	=	Chk_Num_025;MW_NX_026	=	Chk_Num_026;MW_NX_027	=	Chk_Num_027;MW_NX_028	=	Chk_Num_028;MW_NX_029	=	Chk_Num_029;MW_NX_030	=	Chk_Num_030;}} 
			Chk_State -=	1; 
		} 
		string Unit_ST01	=	P38; 
		string Unit_ST02	=	P38; 
		string Unit_ST03	=	P38; 
		string Unit_ST04	=	P38; 
		string Unit_ST05	=	P38; 
		string Unit_ST06	=	P38; 
		string Unit_ST07	=	P38; 
		string Unit_ST08	=	P38; 
		string Unit_ST09	=	P38; 
		string Unit_ST10	=	P38; 
		string Unit_ST11	=	P38; 
		string Unit_ST12	=	P38; 
		string Unit_ST13	=	P38; 
 
		bool TankUnit_L		=	false; 
		bool TankUnit_M3	=	false; 
		 
		if		(TankStoredUnit	==	"M3")	{	TankUnit_M3	=	true;	} 
		else								{	TankUnit_L	=	true;	} 
 
		if	(TankAllStoredVolumeEnabled) 
		{ 
			if	(TankUnit_M3) 
			{	Unit_ST01	=	""; 
				Unit_ST02	=	""; 
				Unit_ST03	=	""; 
				Unit_ST04	=	""; 
				Unit_ST05	=	""; 
				Unit_ST06	=	""; 
				Unit_ST07	=	""; 
				Unit_ST08	=	""; 
				Unit_ST09	=	""; 
				Unit_ST10	=	""; 
				Unit_ST11	=	""; 
				Unit_ST12	=	""; 
				Unit_ST13	=	""; 
			}	else if	(TankUnit_L) 
			{	Unit_ST01	=	""; 
				Unit_ST02	=	""; 
				Unit_ST03	=	""; 
				Unit_ST04	=	""; 
				Unit_ST05	=	""; 
				Unit_ST06	=	""; 
				Unit_ST07	=	""; 
				Unit_ST08	=	""; 
				Unit_ST09	=	""; 
				Unit_ST10	=	""; 
				Unit_ST11	=	""; 
				Unit_ST12	=	""; 
				Unit_ST13	=	""; 
			} 
		}	 
		//Start lis 1-3	################################ 
		string li001	=	PxFull; 
		string li002	=	PxFull; 
		string li003	=	PxFull; 
		//Tank Title 4-13	############################ 
		string li004	=	""; 
		string li005	=	""; 
		string li006	=	""; 
		string li007	=	""; 
		string li008	=	""; 
		string li009	=	""; 
		string li010	=	""; 
		string li011	=	""; 
		string li012	=	""; 
		string li013	=	""; 
 
		if	(!SearchHydro)	{ 
		li004	=	""; 
		li005	=	""; 
		li006	=	""; 
		li007	=	""; 
		li008	=	""; 
		li009	=	""; 
		li010	=	""; 
		li011	=	""; 
		li012	=	""; 
		li013	=	""; 
		} 
		 
		if	(!StatusTitle_Enabled) 
		{	li004	=	PxFull;li005	=	PxFull;li006	=	PxFull;li007	=	PxFull;li008	=	PxFull;li009	=	PxFull;li010	=	PxFull;li011	=	PxFull;li012	=	PxFull;li013	=	PxFull;	} 
		//Space		######################################## 
		string li014	=	PxFull; 
		//Underli 1	#################################### 
		string li015	=	underli_A; 
		string li016	=	underli_B; 
		if	(!Underline_1_Enabled)	{	li015	=	PxFull;li016	=	PxFull;	} 
		if	(BatSpaceline_Enabled)	 
		{	BatSpLi018	=	"";BatSpLi019	=	"";BatSpLi020	=	"";BatSpLi021	=	"";BatSpLi022	=	"";BatSpLi023	=	"";BatSpLi024	=	""; 
			BatSpLi025	=	"";BatSpLi026	=	"";BatSpLi027	=	"";BatSpLi028	=	"";BatSpLi029	=	"";BatSpLi030	=	""; 
		} 
 
		//Space		######################################## 
		string li017	=	PxFull; 
		//lis Battery Amount + MW		#################### 
		string li018	=	P6	+	SymAmX018	+	Am_NXXX_018	+	Am_NXX_018	+	P1	+	Am_NX_018	+	P9	+	BatSpLi018	+	P10	+	MW_Nx6_018	+	P1	+	MW_Nx5_018	+	P1	+	MW_Nx4_018	+	P1	+	MW_Nx3_018	+	P1	+	MW_NXX_018	+	P1	+	MW_NX_018	+	P3	+	Unit_ST01; 
		string li019	=	P6	+	SymAmX019	+	Am_NXXX_019	+	Am_NXX_019	+	P1	+	Am_NX_019	+	P9	+	BatSpLi019	+	P10	+	MW_Nx6_019	+	P1	+	MW_Nx5_019	+	P1	+	MW_Nx4_019	+	P1	+	MW_Nx3_019	+	P1	+	MW_NXX_019	+	P1	+	MW_NX_019	+	P3	+	Unit_ST02; 
		string li020	=	P6	+	SymAmX020	+	Am_NXXX_020	+	Am_NXX_020	+	P1	+	Am_NX_020	+	P9	+	BatSpLi020	+	P10	+	MW_Nx6_020	+	P1	+	MW_Nx5_020	+	P1	+	MW_Nx4_020	+	P1	+	MW_Nx3_020	+	P1	+	MW_NXX_020	+	P1	+	MW_NX_020	+	P3	+	Unit_ST03; 
		string li021	=	P6	+	SymAmX021	+	Am_NXXX_021	+	Am_NXX_021	+	P1	+	Am_NX_021	+	P9	+	BatSpLi021	+	P10	+	MW_Nx6_021	+	P1	+	MW_Nx5_021	+	P1	+	MW_Nx4_021	+	P1	+	MW_Nx3_021	+	P1	+	MW_NXX_021	+	P1	+	MW_NX_021	+	P3	+	Unit_ST04; 
		string li022	=	P6	+	SymAmX022	+	Am_NXXX_022	+	Am_NXX_022	+	P1	+	Am_NX_022	+	P9	+	BatSpLi022	+	P10	+	MW_Nx6_022	+	P1	+	MW_Nx5_022	+	P1	+	MW_Nx4_022	+	P1	+	MW_Nx3_022	+	P1	+	MW_NXX_022	+	P1	+	MW_NX_022	+	P3	+	Unit_ST05; 
		string li023	=	P6	+	SymAmX023	+	Am_NXXX_023	+	Am_NXX_023	+	P1	+	Am_NX_023	+	P9	+	BatSpLi023	+	P10	+	MW_Nx6_023	+	P1	+	MW_Nx5_023	+	P1	+	MW_Nx4_023	+	P1	+	MW_Nx3_023	+	P1	+	MW_NXX_023	+	P1	+	MW_NX_023	+	P3	+	Unit_ST06; 
		string li024	=	P6	+	SymAmX024	+	Am_NXXX_024	+	Am_NXX_024	+	P1	+	Am_NX_024	+	P9	+	BatSpLi024	+	P10	+	MW_Nx6_024	+	P1	+	MW_Nx5_024	+	P1	+	MW_Nx4_024	+	P1	+	MW_Nx3_024	+	P1	+	MW_NXX_024	+	P1	+	MW_NX_024	+	P3	+	Unit_ST07; 
		string li025	=	P6	+	SymAmX025	+	Am_NXXX_025	+	Am_NXX_025	+	P1	+	Am_NX_025	+	P9	+	BatSpLi025	+	P10	+	MW_Nx6_025	+	P1	+	MW_Nx5_025	+	P1	+	MW_Nx4_025	+	P1	+	MW_Nx3_025	+	P1	+	MW_NXX_025	+	P1	+	MW_NX_025	+	P3	+	Unit_ST08; 
		string li026	=	P6	+	SymAmX026	+	Am_NXXX_026	+	Am_NXX_026	+	P1	+	Am_NX_026	+	P9	+	BatSpLi026	+	P10	+	MW_Nx6_026	+	P1	+	MW_Nx5_026	+	P1	+	MW_Nx4_026	+	P1	+	MW_Nx3_026	+	P1	+	MW_NXX_026	+	P1	+	MW_NX_026	+	P3	+	Unit_ST09; 
		string li027	=	P6	+	SymAmX027	+	Am_NXXX_027	+	Am_NXX_027	+	P1	+	Am_NX_027	+	P9	+	BatSpLi027	+	P10	+	MW_Nx6_027	+	P1	+	MW_Nx5_027	+	P1	+	MW_Nx4_027	+	P1	+	MW_Nx3_027	+	P1	+	MW_NXX_027	+	P1	+	MW_NX_027	+	P3	+	Unit_ST10; 
		string li028	=	P6	+	SymAmX028	+	Am_NXXX_028	+	Am_NXX_028	+	P1	+	Am_NX_028	+	P9	+	BatSpLi028	+	P10	+	MW_Nx6_028	+	P1	+	MW_Nx5_028	+	P1	+	MW_Nx4_028	+	P1	+	MW_Nx3_028	+	P1	+	MW_NXX_028	+	P1	+	MW_NX_028	+	P3	+	Unit_ST11; 
		string li029	=	P6	+	SymAmX029	+	Am_NXXX_029	+	Am_NXX_029	+	P1	+	Am_NX_029	+	P9	+	BatSpLi029	+	P10	+	MW_Nx6_029	+	P1	+	MW_Nx5_029	+	P1	+	MW_Nx4_029	+	P1	+	MW_Nx3_029	+	P1	+	MW_NXX_029	+	P1	+	MW_NX_029	+	P3	+	Unit_ST12; 
		string li030	=	P6	+	SymAmX030	+	Am_NXXX_030	+	Am_NXX_030	+	P1	+	Am_NX_030	+	P9	+	BatSpLi030	+	P10	+	MW_Nx6_030	+	P1	+	MW_Nx5_030	+	P1	+	MW_Nx4_030	+	P1	+	MW_Nx3_030	+	P1	+	MW_NXX_030	+	P1	+	MW_NX_030	+	P3	+	Unit_ST13; 
		//Space		######################################## 
		string li031	=	PxFull; 
		string li032	=	PxFull; 
		//Underli 2	#################################### 
		string li033	=	underli_A; 
		string li034	=	underli_B; 
		if	(!Underline_2_Enabled)	{	li033	=	PxFull;	li034	=	PxFull;	} 
		//Bound all lis together 
		string str_Boundli_001_To_010	=	li001	+	Breakli	+	li002	+	Breakli	+	li003	+	Breakli	+	li004	+	Breakli	+	li005	+	Breakli	+	li006	+	Breakli	+	li007	+	Breakli	+	li008	+	Breakli	+	li009	+	Breakli	+	li010	+	Breakli; 
		string str_Boundli_011_To_020	=	li011	+	Breakli	+	li012	+	Breakli	+	li013	+	Breakli	+	li014	+	Breakli	+	li015	+	Breakli	+	li016	+	Breakli	+	li017	+	Breakli	+	li018	+	Breakli	+	li019	+	Breakli	+	li020	+	Breakli;	 
		string str_Boundli_021_To_030	=	li021	+	Breakli	+	li022	+	Breakli	+	li023	+	Breakli	+	li024	+	Breakli	+	li025	+	Breakli	+	li026	+	Breakli	+	li027	+	Breakli	+	li028	+	Breakli	+	li029	+	Breakli	+	li030	+	Breakli;	 
		string str_Boundli_031_To_040	=	li031	+	Breakli	+	li032	+	Breakli	+	li033	+	Breakli	+	li034	+	Breakli	+	li035	+	Breakli	+	li036	+	Breakli	+	li037	+	Breakli	+	li038	+	Breakli	+	li039	+	Breakli	+	li040	+	Breakli;	 
		string str_Boundli_041_To_050	=	li041	+	Breakli	+	li042	+	Breakli	+	li043	+	Breakli	+	li044	+	Breakli	+	li045	+	Breakli	+	li046	+	Breakli	+	li047	+	Breakli	+	li048	+	Breakli	+	li049	+	Breakli	+	li050	+	Breakli;	 
		string str_Boundli_051_To_060	=	li051	+	Breakli	+	li052	+	Breakli	+	li053	+	Breakli	+	li054	+	Breakli	+	li055	+	Breakli	+	li056	+	Breakli	+	li057	+	Breakli	+	li058	+	Breakli	+	li059	+	Breakli	+	li060	+	Breakli;	 
		string str_Boundli_061_To_070	=	li061	+	Breakli	+	li062	+	Breakli	+	li063	+	Breakli	+	li064	+	Breakli	+	li065	+	Breakli	+	li066	+	Breakli	+	li067	+	Breakli	+	li068	+	Breakli	+	li069	+	Breakli	+	li070	+	Breakli;	 
		string str_Boundli_071_To_080	=	li071	+	Breakli	+	li072	+	Breakli	+	li073	+	Breakli	+	li074	+	Breakli	+	li075	+	Breakli	+	li076	+	Breakli	+	li077	+	Breakli	+	li078	+	Breakli	+	li079	+	Breakli	+	li080	+	Breakli;	 
		string str_Boundli_081_To_090	=	li081	+	Breakli	+	li082	+	Breakli	+	li083	+	Breakli	+	li084	+	Breakli	+	li085	+	Breakli	+	li086	+	Breakli	+	li087	+	Breakli	+	li088	+	Breakli	+	li089	+	Breakli	+	li090	+	Breakli;	 
		string str_Boundli_091_To_100	=	li091	+	Breakli	+	li092	+	Breakli	+	li093	+	Breakli	+	li094	+	Breakli	+	li095	+	Breakli	+	li096	+	Breakli	+	li097	+	Breakli	+	li098	+	Breakli	+	li099	+	Breakli	+	li100	+	Breakli;	 
		string str_Boundli_101_To_110	=	li101	+	Breakli	+	li102	+	Breakli	+	li103	+	Breakli	+	li104	+	Breakli	+	li105	+	Breakli	+	li106	+	Breakli	+	li107	+	Breakli	+	li108	+	Breakli	+	li109	+	Breakli	+	li110	+	Breakli; 
		string str_Boundli_111_To_120	=	li111	+	Breakli	+	li112	+	Breakli	+	li113	+	Breakli	+	li114	+	Breakli	+	li115	+	Breakli	+	li116	+	Breakli	+	li117	+	Breakli	+	li118	+	Breakli	+	li119	+	Breakli	+	li120	+	Breakli;	 
		string str_Boundli_121_To_130	=	li121	+	Breakli	+	li122	+	Breakli	+	li123	+	Breakli	+	li124	+	Breakli	+	li125	+	Breakli	+	li126	+	Breakli	+	li127	+	Breakli	+	li128	+	Breakli	+	li129	+	Breakli	+	li130	+	Breakli;	 
		string str_Boundli_131_To_140	=	li131	+	Breakli	+	li132	+	Breakli	+	li133	+	Breakli	+	li134	+	Breakli	+	li135	+	Breakli	+	li136	+	Breakli	+	li137	+	Breakli	+	li138	+	Breakli	+	li139	+	Breakli	+	li140	+	Breakli;	 
		string str_Boundli_141_To_150	=	li141	+	Breakli	+	li142	+	Breakli	+	li143	+	Breakli	+	li144	+	Breakli	+	li145	+	Breakli	+	li146	+	Breakli	+	li147	+	Breakli	+	li148	+	Breakli	+	li149	+	Breakli	+	li150	+	Breakli;	 
		string str_Boundli_151_To_160	=	li151	+	Breakli	+	li152	+	Breakli	+	li153	+	Breakli	+	li154	+	Breakli	+	li155	+	Breakli	+	li156	+	Breakli	+	li157	+	Breakli	+	li158	+	Breakli	+	li159	+	Breakli	+	li160	+	Breakli;	 
		string str_Boundli_161_To_170	=	li161	+	Breakli	+	li162	+	Breakli	+	li163	+	Breakli	+	li164	+	Breakli	+	li165	+	Breakli	+	li166	+	Breakli	+	li167	+	Breakli	+	li168	+	Breakli	+	li169	+	Breakli	+	li170	+	Breakli;	 
		string str_Boundli_171_To_178	=	li171	+	Breakli	+	li172	+	Breakli	+	li173	+	Breakli	+	li174	+	Breakli	+	li175	+	Breakli	+	li176	+	Breakli	+	li177	+	Breakli	+	li178;	 
		string str_AllBoundlis_001_To_178	=	str_Boundli_001_To_010	+		str_Boundli_011_To_020	+	str_Boundli_021_To_030	+	str_Boundli_031_To_040	+	str_Boundli_041_To_050	+	str_Boundli_051_To_060	+	str_Boundli_061_To_070	+	str_Boundli_071_To_080	+	str_Boundli_081_To_090	+	str_Boundli_091_To_100	+	str_Boundli_101_To_110	+	str_Boundli_111_To_120	+	str_Boundli_121_To_130	+	str_Boundli_131_To_140	+	str_Boundli_141_To_150	+	str_Boundli_151_To_160	+	str_Boundli_161_To_170	+	str_Boundli_171_To_178; 
 
		// find all LCD with NameTag 
		var TankStatus_Lcd_list = new List<IMyTerminalBlock>(); 
		GridTerminalSystem.SearchBlocksOfName(LCD_NameTag_now,TankStatus_Lcd_list); 
		// this loop send Message to show to all found Lcds	 
		for (int i=0; i<TankStatus_Lcd_list.Count; i++) 
		{ 
			var TankStatus_Lcd_X_Name = TankStatus_Lcd_list[i].CustomName; 
			IMyTextPanel TankStatus_Lcd_X = GridTerminalSystem.GetBlockWithName(TankStatus_Lcd_X_Name) as IMyTextPanel; 
			 
			TankStatus_Lcd_X.SetValue("FontColor", new Color(LCDbright,LCDbright,LCDbright));	// White 
			TankStatus_Lcd_X.SetValue("Font", (long)1147350002); 
			 
			TankStatus_Lcd_X.SetValue("FontSize", 0.10f);	// set Font size of your LCD 
			TankStatus_Lcd_X.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; 
			//TankStatus_Lcd_X.WriteText("", false); 
			TankStatus_Lcd_X.WriteText("" + str_AllBoundlis_001_To_178, false); 
		} 
	} 
} // End Void Main