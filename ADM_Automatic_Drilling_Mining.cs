	string ERR_TXT = "";
	string Mod = "";
	string txt = "";
	float TotalDepth = 0f;
	float MaxDepth = 0f;
//IMyProgrammableBlock ProgBlock;
IMyMotorStator Rotor_A;
IMyMotorStator Rotor_Z;
IMyMotorStator Rotor_base;
List<IMyTerminalBlock> MyGroup = new List<IMyTerminalBlock>();//Все брлки с интерфейсом записать в MyGroup
List<IMyShipDrill> Drill_List = new List<IMyShipDrill>();         //список для буров создать Drill_List
List<IMyMotorStator> Rotor_List = new List<IMyMotorStator>();     //список для роторов создать Rotor_List
List<IMyTerminalBlock> ProgBlocks = new List<IMyTerminalBlock>(); //список для прог.блоков создать ProgBlocks
List<IMyPistonBase> Piston_List = new List<IMyPistonBase>();      //список для поршней создать Piston_List
  List<IMyLandingGear> LandingGear_List = new List<IMyLandingGear>();
  Vector3D Up_ProgBlock ;
  IMyLandingGear LandingGear;
List<IMyButtonPanel> Button_List = new List<IMyButtonPanel>();
float MeScale = 1f;
public Program(){ 

if(Me.CubeGrid.GridSizeEnum != MyCubeSize.Large ){ MeScale=0.65f; }//else{ }
if(Me.CustomName.IndexOf("[ADM]")<0)Me.CustomName="[ADM] "+Me.CustomName;
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	GetSystem();
}
public void GetSystem()
{	
	
	
	//Mod="stop";
	//scale=*MeScale;
	
	
    // Конструктор, вызванный единожды в каждой сессии и
	// Получите первую текстовую поверхность на программируемом блоке
    MeSurf = Me.GetSurface(0);
	// Это центр текстовой поверхности
    using (var frame = MeSurf.DrawFrame())
    {
        SetupDrawSurface(MeSurf);
		DrawSprites(frame, MeSurf.TextureSize * 0.5f,"Logo", 0.95f);
		DrawSprites(frame, MeSurf.TextureSize * 0.5f,"ADM", 0.7f);
		DrawSprites(frame, MeSurf.TextureSize * 0.5f,"Fon", 1f);
    }
    //DrawRotationSprites(MeSurf.TextureSize * 0.5f);
	 //-------------------------------------------------------------------- 
 GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(MyGroup);
  if(GridTerminalSystem.GetBlockGroupWithName("ADM") != null) {
	
    GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyShipDrill>(Drill_List);
	GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyMotorStator>(Rotor_List);
	//GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyProgrammableBlock>(ProgBlocks);
	GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyPistonBase>(Piston_List);
	
	//GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyLandingGear>(LandingGear_List);
	//IMyLandingGear
	GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyLandingGear>(LandingGear_List);
	LandingGear = (LandingGear_List.DefaultIfEmpty(null)).First();
	List<IMyMotorStator> Rotorbase_List = new List<IMyMotorStator>();
	GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyMotorStator>(Rotorbase_List, x => x.CustomName.Contains("база"));
	Rotor_base = (Rotorbase_List.DefaultIfEmpty(null)).First();
	
	if(Rotor_base != null) Rotor_base.Torque = 1300000;
	/*if(ProgBlocks.Count == 0) {
		ERR_TXT += "Прог Блок не найдены\n";
		Mod="eror";
    }else {
        ProgBlock = (IMyProgrammableBlock)ProgBlocks[0];
        //break;
    }*/
	if(Drill_List.Count == 0) {
		ERR_TXT += "Буры не найдены\n";
		Mod="eror";
    }else{
		foreach (IMyShipDrill Drill in  Drill_List){
			if(Drill.CustomName.IndexOf("[ADM]")<0)Drill.CustomName="[ADM] "+Drill.CustomName;
		}
	}
		if(Rotor_List.Count == 0) {
		ERR_TXT += "Роторы не найдены\n";
		Mod="eror";
		}else{
			//________________функционольный разбор__________________________________________________________________________
			foreach (IMyMotorStator Rotor in  Rotor_List){
			if(Rotor.CustomName.IndexOf("[ADM]")<0)Rotor.CustomName="[ADM] "+Rotor.CustomName;
				Vector3D Up_Rotor = Rotor.WorldMatrix.Up;
				 Up_ProgBlock = Me.WorldMatrix.Up;
				if(Vector3D.Dot(Up_Rotor,-Up_ProgBlock) > 0.8 || Vector3D.Dot(Up_Rotor,Up_ProgBlock) > 0.8){
					Rotor_A = Rotor;
					Rotor_A.Torque = 1300000;
					//Rotor_A.UpperLimitDeg = 360f ;//UpperLimitDeg (float)361f; Infinity
					Echo("Rotor_A- "+Rotor.CustomName);
				
				}else{
					if(Vector3D.Dot(Up_Rotor,Up_ProgBlock) < 0.03){
						Rotor_Z = Rotor;
						Rotor_Z.Torque = 10000000;
						Echo("Rotor_Z- "+Rotor.CustomName);
						Loong = Drill_List[0].GetPosition()-Rotor_Z.GetPosition();
						Dlinna = (float)(Loong.Length());
					}
				}
			}
			//---------------------------------------------------------------------------------------------------------------
			Dlinna = (float)(Loong.Length()+(Drill_List[0].CubeGrid.GridSize * (((IMyCubeBlock)Drill_List[0]).Max.Y - ((IMyCubeBlock)Drill_List[0]).Min.Y + 1))/2);
			if(GetValue("AutomaticStop")!="")AutomaticStop=Convert.ToBoolean(GetValue("AutomaticStop"));
			if(GetValue("Mod")==""){Mod="stop";}else{Mod=GetValue("Mod");}
			if(GetValue("ReturnMod")==""){ReturnMod="stop";}else{ReturnMod=GetValue("ReturnMod");}
			if(GetValue("LimitDeg")==""){LimitDeg=15f;}else{LimitDeg=Single.Parse(GetValue("LimitDeg"));}
			if(GetValue("Z")==""){Z=0f;}else{Z=Single.Parse(GetValue("Z"));}
			if(GetValue("trend")==""){trend=1f;}else{Z=Single.Parse(GetValue("trend"));}
			//Mod=GetValue("Mod");
			//LimitDeg=Single.Parse(GetValue("LimitDeg"));
			//Z=Single.Parse(GetValue("Z"));
			//Mod="ADM";
			
		}
	
	
    foreach (IMyPistonBase Piston in  Piston_List){ //цикл по поршням
	if(Piston.CustomName.IndexOf("[ADM]")<0)Piston.CustomName="[ADM] "+Piston.CustomName;
	if(Piston.CubeGrid.GridSizeEnum == MyCubeSize.Large ){ /*Echo("Большой грид")*/ MaxDepth+=10f; }else{ /*Echo("Малый грид")*/ MaxDepth+=2;}
	
				if(Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7){
					Piston.MaxLimit = Piston.CurrentPosition;	//макс длинна текущая 
					//Piston.ApplyAction("Extend");					//поршень выдвенуть
				}else{
					Piston.MinLimit = Piston.CurrentPosition;	//макс длинна текущая 
					//Piston.ApplyAction("Retract");				//поршень задвинуть
				}
				//Piston.MaxLimit = Piston.CurrentPosition;	//макс длинна текущая 
				//Piston.ApplyAction("Extend");					//поршень выдвенуть
				//Piston.ApplyAction("Retract");
			}
	if(Piston_List.Count == 0) {
		//ERR_TXT += "Поршни не найдены\n";
		//Mod="eror";
    }
	
  }
  else {
    ERR_TXT += "Нет группы ADM \n";
	Mod="eror";
	
  }
  
  //-----------------------------------------------------------------------
}

public void Save()
{
    // Вызывается, когда программе требуется сохранить своё состояние.
   
	
}
string ReturnMod;
IMyTextSurface MeSurf ;
Vector3D Loong;
int Tik=0;
float Z=0;
float A=0;
float TikA=0;
float LimitDeg = 10;
float Dlinna ;
float trend = 1f;
bool RotoReady = true;
bool AutomaticStop = false;
//bool DrillingEnd = true;
//bool AutoStop = false;
public void Main(string argument, UpdateType updateSource)
{
	
	
	if(Mod=="eror"){
		Runtime.UpdateFrequency = UpdateFrequency.Update100;
		Echo("status\n"+ERR_TXT);
		ERR_TXT="";
		GetSystem();
		return;
	}
	TotalDepth=0f;
	foreach (IMyPistonBase Piston in  Piston_List){
					//if(Piston.MaxLimit >= 10f){Mod = "stop"; break;}
		if(Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7){
			TotalDepth+=Piston.CurrentPosition;
		}else{
			if(Piston.CubeGrid.GridSizeEnum == MyCubeSize.Large ){ /*Echo("Большой грид")*/ TotalDepth+=10-Piston.CurrentPosition; }else{ /*Echo("Малый грид")*/ TotalDepth+=2-Piston.CurrentPosition;;}
						
			TotalDepth+=10-Piston.CurrentPosition;
		}
	}


	var і=Runtime; Echo($"{System.DateTime.Now}\nCode runtime:{і.LastRunTimeMs:F2} ms {і.UpdateFrequency}\nКоличество Текущих Инструкций {і.CurrentInstructionCount}\n {і.CurrentCallChainDepth}");
	//void SaveValue(string Teg, string Value)
	SaveValue("Mod", Mod);
	SaveValue("ReturnMod", ReturnMod);
	
	Echo(this.Storage);
	Echo("*******");
	Echo("Mod "+ Mod);
	//Convert.ToBoolean(number)
	//if(GetValue("AutomaticStop")!="")Echo("AutomaticStop "+Convert.ToBoolean(GetValue("AutomaticStop")));
	//Echo("*******");
	//this.Storage=Mod;
	float MaxVolue=0;
	float CurVolue=0;
    foreach (IMyShipDrill MyDrill in  Drill_List){  //цикл по бурам
		MaxVolue += (float)MyDrill.GetInventory(0).MaxVolume; //максимальная загрузк дрелей
		CurVolue += (float)MyDrill.GetInventory(0).CurrentVolume;  //текущая загрузк дрелей
	}
   if(argument != ""){
			Runtime.UpdateFrequency = UpdateFrequency.Update10;
			
			if(argument=="On/Off"){
				
				if(Mod=="stop"){Mod="start";}else{Mod="stop"; }
			}else{
				if(Mod!="pause") Mod=argument;
			} 
			
		}
		//LimitDeg=(float)GetValue("LimitDeg");
			//Z=(float)GetValue("Z");
			//Echo( $"длинна :{ Dlinna } метров" );
			
		//Echo(this.Storage+"\n=======");
	//++++++++++++++++++++++++**********************+++++++++++++++++++++++*/
	if(Mod!=""){
		foreach (IMyPistonBase Piston in  Piston_List){TerminalPropertyExtensions.SetValue(Piston, "ShareInertiaTensor", false);}
		TerminalPropertyExtensions.SetValue(Rotor_A, "ShareInertiaTensor", false);
		TerminalPropertyExtensions.SetValue(Rotor_A, "RotorLock", false);
		TerminalPropertyExtensions.SetValue(Rotor_Z, "ShareInertiaTensor", false);
		TerminalPropertyExtensions.SetValue(Rotor_Z, "RotorLock", false);
	}
	switch (Mod){
		default:
			Mod=ReturnMod;
			
		break;
		//ошибка--------------------------------------------- 
		case "eror":
		    //Me.GetSurface(0).WriteText("status "+ERR_TXT);
			Runtime.UpdateFrequency = UpdateFrequency.Update100;
			//Echo("status "+Mod);
			Echo("status\n"+ERR_TXT);
			ERR_TXT="";
			GetSystem();
		break;
		//---------------------------------------------------
		//старт----------------------------------------------
		case "start":
		Runtime.UpdateFrequency = UpdateFrequency.Update10;
		//LandingGear_List[0].IsLocked
		
		
		if(LandingGear != null){
			LandingGear?.Unlock();
			LandingGear.AutoLock = false;
		}
		if(Rotor_base != null){
			Rotor_base.TargetVelocityRad = 0.4f;
			Rotor_base.UpperLimitDeg = 0f;
		}
		//H2Rotors(0,0); .TargetVelocityRad = 0.7f;
		if(Math.Abs(Rotor_A.UpperLimitDeg)<361f && Math.Abs(Rotor_A.LowerLimitDeg)<361f){
			LimitDeg=Rotor_Z.UpperLimitDeg;
			Z=0;
			if(Vector3D.Dot(Rotor_A.WorldMatrix.Up,Up_ProgBlock) > 0.8){
			 if(Math.Abs(Rotor_A.UpperLimitDeg)<361f)A=Rotor_A.UpperLimitDeg;
			 
			}else{
			 if(Math.Abs(Rotor_A.LowerLimitDeg)<361f)A=Rotor_A.LowerLimitDeg;
			 
			}
			
			H2Rotors(Z,A);
			if(RotoReady){
				Mod = "Drilling";
				break;
			}
		}else{
			Z=0;
			A=0;
			H2Rotors(Z,A);
		}
		 
		 
		 //Rotation_Z(Z);
		 //Rotation_A(A);
		foreach (IMyShipDrill MyDrill in  Drill_List){  //цикл по бурам
			MyDrill.ApplyAction("OnOff_On")	;			//бур включить
			MaxVolue += (float)MyDrill.GetInventory(0).MaxVolume; //максимальная загрузк дрелей
			CurVolue += (float)MyDrill.GetInventory(0).CurrentVolume;  //текущая загрузк дрелей
		}
		//RotoReady=true;
		if(RotoReady){
			
		if(CurVolue == 0f){
			foreach (IMyPistonBase Piston in  Piston_List){ //цикл по поршням
				Piston.MaxLimit = 10f;							//макс длинна 10
				if(Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7){
					//Piston.MaxLimit = Piston.CurrentPosition;
					Piston.MaxLimit = Piston.CurrentPosition+0.5f;
					Piston.ApplyAction("Extend");					//поршень выдвенуть
				}else{
					//Piston.MinLimit = Piston.CurrentPosition;
					Piston.MinLimit = Piston.CurrentPosition-0.5f;
					Piston.ApplyAction("Retract");				//поршень задвинуть
				}
				//Piston.MaxLimit = 10f;							//макс длинна 10
				//Piston.ApplyAction("Extend");					//поршень выдвенуть
				//Piston.ApplyAction("Retract");				//поршень задвинуть
			}
		}else{
			foreach (IMyPistonBase Piston in  Piston_List){ //цикл по поршням
				
				if(Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7){
					Piston.MaxLimit = Piston.CurrentPosition;	//макс длинна текущая 
					Piston.ApplyAction("Extend");					//поршень выдвенуть
				}else{
					Piston.MinLimit = Piston.CurrentPosition;	//макс длинна текущая 
					Piston.ApplyAction("Retract");				//поршень задвинуть
				}
				//Piston.MaxLimit = Piston.CurrentPosition;	//макс длинна текущая 
				//Piston.ApplyAction("Extend");					//поршень выдвенуть
				//Piston.ApplyAction("Retract");
			}
			Mod = "Drilling";
		}
		}
		txt ="--<start>--";
		break;
		//------------------------------------------------------
		//стоп===================================================
		case "stop":
		//Piston.ApplyAction("Retract");
		//Piston.MaxLimit = 0;//Rotor_A.UpperLimitDeg = 360f ; Rotor_Z.TargetVelocityRad = 0.7f;
		this.Storage="";
		 Z=0;//Rotor_Z.UpperLimitDeg;
		 A=0;
		txt ="\n----<stop>----\n";
		//Angle=0;
		
		//float div =0;
		txt += "Возврат в стартовое положение\n";
		//Rotation_A(0);
		//if(!RotoReady)Rotation_Z(0);
		//if(!RotoReady)
			//Z=Rotor_Z.LowerLimitDeg;
			//A=Rotor_A.UpperLimitDeg;
			if(Vector3D.Dot(Rotor_A.WorldMatrix.Up,Up_ProgBlock) > 0.8){
			 if(Math.Abs(Rotor_A.UpperLimitDeg)<361f)A=Rotor_A.UpperLimitDeg;
			}else{
			 if(Math.Abs(Rotor_A.LowerLimitDeg)<361f)A=Rotor_A.LowerLimitDeg;
			}
			if(Math.Abs(Rotor_Z.LowerLimitDeg)<90f)Z=Rotor_A.LowerLimitDeg;
			
			Echo("Z="+Rotor_Z.LowerLimitDeg);
			H2Rotors(Z,A);
		
		foreach (IMyShipDrill MyDrill in  Drill_List){  //цикл по бурам
			MyDrill.ApplyAction("OnOff_Off");			//бур выключить
		}
		foreach (IMyPistonBase Piston in  Piston_List){//цикл по поршням
			Piston.MaxLimit = 0f;
			if(Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7){
				Piston.MinLimit=0;
				Piston.MaxLimit=0;
				Piston.ApplyAction("Retract");					//поршень задвинуть
			}else{
				Piston.MinLimit=10;
				Piston.MaxLimit=10;
				Piston.ApplyAction("Extend");				//поршень выдвенуть
			}
			//Piston.ApplyAction("Retract");
		}
		
		if(RotoReady == true){
			//Runtime.UpdateFrequency = UpdateFrequency.None;
			//if(Rotor_base != null)Rotor_base.TargetVelocityRad = -0.4f;
			//Rotor_Z.TargetVelocityRad = 0.15f;
			
			//if(Math.Abs((Rotor_Z.Angle*180)/Math.PI - Rotor_Z.UpperLimitDeg) < 0.02){
				LandingGear?.Lock();
				//LandingGear.AutoLock = false;
				//RotoReady = true;
				txt += "----<!Stop!>----\n";
				foreach (IMyPistonBase Piston in  Piston_List){TerminalPropertyExtensions.SetValue(Piston, "ShareInertiaTensor", true);}
				TerminalPropertyExtensions.SetValue(Rotor_A, "ShareInertiaTensor", true);
				TerminalPropertyExtensions.SetValue(Rotor_A, "RotorLock", true);
				TerminalPropertyExtensions.SetValue(Rotor_Z, "ShareInertiaTensor", true);
				TerminalPropertyExtensions.SetValue(Rotor_Z, "RotorLock", true);
				Runtime.UpdateFrequency = UpdateFrequency.None;
			//}//else{RotoReady = false;}
			
			
			
			//Rotor_base.UpperLimitDeg = 0f; .Angle
			
			//txt += "----<!Stop!>----\n";
			Tik=0;
			LimitDeg = 15;
			Dlinna = 1;
			RotoReady = true;
			//DrillingEnd = true;
		}else{Runtime.UpdateFrequency = UpdateFrequency.Update10;}
		
		//Me.GetSurface(0).WriteText(txt);
		//Echo(txt);
		
		break;
		//---------------------------------------------------
		//бурение---------------------------------------
		case "Drilling":
			//void GetProperties(List<ITerminalProperty> resultList, Func<ITerminalProperty, bool> collect = null)
			List<ITerminalProperty> SPisik = new List<ITerminalProperty>();
			
			foreach (IMyShipDrill MyDrill in  Drill_List){  //цикл по бурам
			MyDrill.ApplyAction("OnOff_On")	;			//бур включить
			MaxVolue += (float)MyDrill.GetInventory(0).MaxVolume; //максимальная загрузк дрелей
			CurVolue += (float)MyDrill.GetInventory(0).CurrentVolume;  //текущая загрузк дрелей
			}
			if((CurVolue * 100f)/MaxVolue > 95f){
				ReturnMod="Drilling";
				AutomaticStop = true;
				SaveValue("AutomaticStop", AutomaticStop.ToString());
				Mod = "pause";
				break;
			}
			Loong = Drill_List[0].GetPosition()-Rotor_Z.GetPosition();
			Dlinna = (float)(Loong.Length()+(Drill_List[0].CubeGrid.GridSize * (((IMyCubeBlock)Drill_List[0]).Max.Y - ((IMyCubeBlock)Drill_List[0]).Min.Y + 1))/2);
			
			if(Math.Abs(Z)<0.3f){
				Mod = "angle";
				break;
			} //Z=(float)(2f/((Dlinna*3.14f)/(180f)));
			Rotation_Z(Z);
			
			
			//Echo( "Ширина "+ (((IMyCubeBlock)Drill_List[0]).Max.X - ((IMyCubeBlock)Drill_List[0]).Min.X + 1) +" метров" );
			//Echo( $"Ширина :{ Drill_List[0].CubeGrid.GridSize * (((IMyCubeBlock)Drill_List[0]).Max.X - ((IMyCubeBlock)Drill_List[0]).Min.X + 1) } метров" );
			//Echo( $"длинна :{ Drill_List[0].CubeGrid.GridSize * (((IMyCubeBlock)Drill_List[0]).Max.Y - ((IMyCubeBlock)Drill_List[0]).Min.Y + 1) } метров" );
			//Echo( $"Ширина :{ Drill_List[0].CubeGrid.GridSize * (((IMyCubeBlock)Drill_List[0]).Max.Z - ((IMyCubeBlock)Drill_List[0]).Min.Z + 1) } метров" );
			
			Echo("A_Velocity="+Rotor_A.TargetVelocityRad+(Rotor_A.TargetVelocityRad == 0f));
			//if(Rotor_A.TargetVelocityRad == 0f){
				float Dlinna2 = (float)Math.Sqrt(Dlinna*Dlinna-(Vector3D.Dot(Loong,-Up_ProgBlock))*(Vector3D.Dot(Loong,-Up_ProgBlock)));
			//float Dlinna2 = (float)Math.Sqrt(Dlinna*Dlinna-(Vector3D.Dot(Loong,-Up_ProgBlock))*(Vector3D.Dot(Loong,-Up_ProgBlock)));
			//if(Rotor_A.TargetVelocityRad==0f ) Rotor_A.TargetVelocityRad = (float)(1f/(((Dlinna2*Math.PI)/(180f)))*Math.PI/180);
			Echo("Rotor_A "+trend);
			if(Math.Abs(Rotor_A.UpperLimitDeg)<361f && Math.Abs(Rotor_A.LowerLimitDeg)<361f){
				if(Vector3D.Dot(Rotor_Z.WorldMatrix.Forward ,Up_ProgBlock) > 0.8 || Vector3D.Dot(Rotor_Z.WorldMatrix.Right ,Up_ProgBlock) > 0.8 ){LimitDeg=Rotor_Z.LowerLimitDeg;}else{LimitDeg=Rotor_Z.UpperLimitDeg;}
				Rotor_A.TargetVelocityRad = trend*(float)((0.5f/(((Dlinna*Math.Cos(Z*Math.PI/180))*Math.PI)/(180f)))*Math.PI/180);
				if(Math.Abs(Rotor_A.TargetVelocityRad)>0.5f)Rotor_A.TargetVelocityRad=trend*0.5f;
			}else{

			//}
			Rotor_A.TargetVelocityRad = trend*(float)((0.5f/(((Dlinna*Math.Sin(Z*Math.PI/180))*Math.PI)/(180f)))*Math.PI/180);
			//HourAngle +=(2f/((DriligLineZ.Length()*Math.PI)/(180f)))*AngleTenden;
			//Echo("Угол "+Rotor_List[masA[0]].Angle);
			}
			
				//Rotor_A.TargetVelocityRad=0.5f;
				Echo("Tik="+Tik);
			if(Math.Abs(Rotor_A.Angle-TikA) < 0.001f && Rotor_A.TargetVelocityRad!=0f){
				if(Tik<60){
					Tik+=1;
				}else{
					Tik=0; 
					//DepthStep(-1); 
					//if(Z>0f) AngleStep(-1);
				}
			}else{Tik = 0;}
			TikA=Rotor_A.Angle;//+="\nTorque="+Rotor_A.Torque ; //(float)Math.Atan2(gL2,gF2); Math.Cos(Rotor_A.Angle)
			
			//Rotor_A.UpperLimitDeg = 361f ;//UpperLimitDeg (float)361f; Infinity
			Rotor_A.Torque = 1000000;
			//Echo("Z "+Z);
			//Echo("LimitDeg "+LimitDeg);trend
			Echo("Angle "+Math.Round(Rotor_A.Angle,2)+(Math.Abs(Rotor_A.Angle)>=6.28f));
			Echo("UpperLimit "+Math.Round(Rotor_A.UpperLimitDeg,2));
			Echo("LowerLimit "+Math.Round(Rotor_A.LowerLimitDeg,2) );
			float A_Angle;
			
			if(Math.Abs(Rotor_A.Angle)<6.28f){A_Angle=Math.Abs(Rotor_A.Angle);}else{A_Angle=6.28f;}
			
			if(Math.Abs(Rotor_A.UpperLimitDeg-((Rotor_A.Angle*180)/Math.PI))<0.1f && trend>0){
				Mod = "angle";
				trend=-1;
				SaveValue("trend", Convert.ToString(trend));
				
			}else{
				if(Math.Abs(Rotor_A.LowerLimitDeg-((Rotor_A.Angle*180)/Math.PI))<0.1f && trend<0){
				trend=1;
				SaveValue("trend", Convert.ToString(trend));
				Mod = "angle";
				}
			}
			
			if(Math.Abs(Rotor_A.UpperLimitDeg)>360f || Math.Abs(Rotor_A.LowerLimitDeg)>360f){
				if(6.28f-Math.Abs(Rotor_A.Angle)<0.01f ){
					Tik = 0;
					Mod = "angle";
				}
				
			}
			
			
			if( Math.Abs(Z)>=Math.Abs(LimitDeg)){
				//Echo("!!!!"+Z);
				//Runtime.UpdateFrequency = UpdateFrequency.None;
				Mod = "depth";
			}
			
		break;
		//---------------------------------------------
		//поршни---------------------------------
		case "depth":
			//H2Rotors(0,0);
			Z=0;
			Rotation_Z(Z);
			Rotor_A.TargetVelocityRad=0f;
			if(RotoReady == true){
				
				float StepLimit = 2f/Piston_List.Count;
				TotalDepth=0f;
				foreach (IMyPistonBase Piston in  Piston_List){
					//if(Piston.MaxLimit >= 10f){Mod = "stop"; break;}
					
					
					if(Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7){
						TotalDepth+=Piston.CurrentPosition;
						Piston.MaxLimit += StepLimit;
						Piston.ApplyAction("Extend");					//поршень выдвенуть
					}else{
						if(Piston.CubeGrid.GridSizeEnum == MyCubeSize.Large ){ /*Echo("Большой грид")*/ TotalDepth+=10-Piston.CurrentPosition; }else{ /*Echo("Малый грид")*/ TotalDepth+=2-Piston.CurrentPosition;;}
						//TotalDepth+=10-Piston.CurrentPosition;
						Piston.MinLimit -= StepLimit;
						Piston.ApplyAction("Retract");				//поршень задвинуть
					}
				}
				Echo("if "+(TotalDepth/MaxDepth >= 0.99f));
				if(TotalDepth/MaxDepth >= 0.99f || Piston_List.Count == 0){
					Mod = "stop"; break;
				}else{
					Mod = "Drilling";
					LimitDeg+=(float)(3.5f/((Dlinna*3.14f)/(180f)));
					if(Math.Abs(LimitDeg)>=Math.Abs(Rotor_Z.UpperLimitDeg)){
						LimitDeg=Rotor_Z.UpperLimitDeg;
					}
					Z=0;//(float)(2f/((Dlinna*3.14f)/(180f)));
					SaveValue("Z", Convert.ToString(Z));
					//if(LimitDeg<Rotor_Z.UpperLimitDeg)
						SaveValue("LimitDeg", Convert.ToString(LimitDeg));
				}
				//if(Mod != "stop")Mod = "ADM";
			}
			RotoReady = false;
			//DrillingEnd = false;
		break;
		//---------------------------------------------
		
		case "angle":
			//Runtime.UpdateFrequency = UpdateFrequency.Update10;
			Rotor_A.TargetVelocityRad=0f;
			
			//if(RotoReady){
				if(Vector3D.Dot(Rotor_Z.WorldMatrix.Forward ,Up_ProgBlock) > 0.8 || Vector3D.Dot(Rotor_Z.WorldMatrix.Right ,Up_ProgBlock) > 0.8 ){AngleStep(-1);}else{AngleStep();}
				//Rotation_Z(Z);
			//}
			//Rotation_Z(Z);
			//if(RotoReady) 
				Mod = "Drilling";
			
		break;
		case "pause":
		switch (argument){
			
			case "depth":
				DepthStep(1);
			break;
			
			
			case "angle":
			
				AngleStep(1);
				
			break;
			
			default:
			 //string GetValue(string TxtTeg)
			 //void SaveValue(string Teg, string Value)
			if(GetValue("Mod")==Mod && argument=="pause") {Mod=ReturnMod;}else{if(GetValue("Mod")!=Mod) ReturnMod=GetValue("Mod"); /*PauseZ=Rotor_Z.Angle;*/}
			RotoReady=true;
			foreach (IMyShipDrill MyDrill in  Drill_List){  //цикл по бурам
				MyDrill.ApplyAction("OnOff_Off");			//бур выключить
				MaxVolue += (float)MyDrill.GetInventory(0).MaxVolume; //максимальная загрузк дрелей
				CurVolue += (float)MyDrill.GetInventory(0).CurrentVolume;  //текущая загрузк дрелей
			}
			//Rotor_A.UpperLimitDeg = 0f ;
			foreach (IMyMotorStator Rotor in  Rotor_List){
				Rotor.TargetVelocityRad = 0f;
			}
			Rotation_Z(Z);
			
			if(CurVolue == 0 && AutomaticStop ) {AutomaticStop=false; Mod = "Drilling"; }
			//if(Mod == argument){
				//Mod = "pause";
				//break;
			//}else 
				//if(CurVolue == 0 && AutoStop ){
				//Mod = "ADM";
				//break;
			//}
				break;
			}
			
		break;
	}
	//++++++++++++++++++++++++*************************+++++++++++++++++++++++
	
	if(Runtime.UpdateFrequency == UpdateFrequency.None||Mod=="eror"){
		MeSurf = Me.GetSurface(0);
		using (var frame = MeSurf.DrawFrame())
		{
			DrawSprites(frame, MeSurf.TextureSize * 0.5f,"Logo", 0.95f);
			DrawSprites(frame, MeSurf.TextureSize * 0.5f,"ADM", 0.7f);
			DrawSprites(frame, MeSurf.TextureSize * 0.5f,"Fon", 1f);
			//frame.AddRange(SpriteArray);
		}
	}else{
		MeSurf = Me.GetSurface(0);
		
		using (var frame = MeSurf.DrawFrame())
		{
			DrawSprites(frame, MeSurf.TextureSize * 0.5f+new Vector2(-50f,-65f)*MeScale,"Logo", 0.4f);
			//DrawSprites(frame, MeSurf.TextureSize * 0.5f,"ADM", 0.7f);
			
			ButtonSprites(frame, MeSurf.TextureSize * 0.5f+new Vector2(5f,-50f)*MeScale, Mod, true, 0.14f);
			frame.Add(new MySprite(SpriteType.TEXT, "Mod "+Mod, MeSurf.TextureSize * 0.5f+new Vector2(45f,-67f)*MeScale, null, new Color(255,255,255,255), "DEBUG", TextAlignment.LEFT, 1f*MeScale)); // text5
			
			DrawSprites(frame, MeSurf.TextureSize * 0.5f+new Vector2(-85f,32f)*MeScale,"Fon", -1f);
			frame.Add(new MySprite(SpriteType.TEXT, "Automatic Drilling Mining", MeSurf.TextureSize * 0.5f+new Vector2(-130f,-155f)*MeScale, null, new Color(255,255,255,255), "DEBUG", TextAlignment.LEFT, 1.1f*MeScale)); // text5
			
			DrawScale(frame, MeSurf.TextureSize * 0.5f+new Vector2(-40f,-10f)*MeScale, "Cargo Drills               "+Math.Round((CurVolue * 100f)/MaxVolue)+"%", CurVolue/MaxVolue, 1.3f);
			DrawScale(frame, MeSurf.TextureSize * 0.5f+new Vector2(-40f,25f)*MeScale, "Angle                      "+Math.Round(Z)+"/"+Math.Round(Rotor_Z.UpperLimitDeg), Z/Rotor_Z.UpperLimitDeg, 1.3f);
			DrawScale(frame, MeSurf.TextureSize * 0.5f+new Vector2(-40f,60f)*MeScale, "Depth                      "+Math.Round(TotalDepth)+"/"+Math.Round(MaxDepth),  TotalDepth/MaxDepth, 1.3f);
			if(AutomaticStop)frame.Add(new MySprite(SpriteType.TEXT, "waiting (Cargo Drills = empty)", MeSurf.TextureSize * 0.5f+new Vector2(0f,90f)*MeScale, null, new Color(155,25,25,255), "DEBUG", TextAlignment.CENTER, 1.1f*MeScale)); // text5
			
			//DrawScale(MySpriteDrawFrame frame, Vector2 centerPos, string caption="Текст         100%", float Value = 1f, float scale = 0.8f)
		}
	}
	
    // Главная точка входа в скрипт вызывается каждый раз,
	if(GridTerminalSystem.GetBlockGroupWithName("ADM") != null) {
		//GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyTerminalBlock>(_List);
		GridTerminalSystem.GetBlockGroupWithName("ADM").GetBlocksOfType<IMyButtonPanel>(Button_List);
	}
	
	foreach(IMyButtonPanel Blok in Button_List){
		//if(Blok.CustomName.IndexOf("[ADM]")<0)Blok.CustomName="[ADM] "+Blok.CustomName;
		if (Blok.CustomData=="") Blok.CustomData = "[Button1]On/Off\n[Button2]pause\n[Button3]angle\n[Button4]depth";
		for(int i = 0; i < 4 ; i++){
			if(Blok.IsButtonAssigned(i)){
				if(Blok.BlockDefinition.SubtypeId.Contains("LargeSciFiButtonPanel")){
					int S = Blok.CustomData.IndexOf("[Button"+(i+1)+"]")+("[Button"+(i+1)+"]").Length;
					int F = Blok.CustomData.IndexOf("\n",S);
					if(F<0)F=Blok.CustomData.Length;
					string ButtonName = Blok.CustomData.Substring(S, F-S);
					Blok.SetCustomButtonName(i, ButtonName);
					// Получите первую текстовую поверхность 
					IMyTextSurface Surf = ((IMyTextSurfaceProvider)Blok).GetSurface(i);
					// Это центр текстовой поверхности
					Vector2 centerPos = Surf.TextureSize * 0.5f;
					// Масштаб, равный 1, означает, что спрайты будут иметь свой первоначальный размер
					using (var frame = Surf.DrawFrame()){
						SetupDrawSurface(Surf);
						ButtonSprites(frame, centerPos, ButtonName, Mod==ButtonName);//public void ButtonSprites(MySpriteDrawFrame frame,Vector2 centerPos, string ButtonName="On/Off", bool Active = false , float scale = 0.26f)
					}
				}
			}
		}
	}	
	
    
}
public string GetValue(string TxtTeg="null"){
	if(this.Storage.IndexOf(TxtTeg)<0)return "";
	int S = this.Storage.IndexOf(TxtTeg+"(")+(TxtTeg+"(").Length;
	int F = this.Storage.IndexOf(")",S);
	if(F<0)F=this.Storage.Length;
	//string Message = Blok.CustomData.Substring(S, F-S);
	return this.Storage.Substring(S, F-S);
	//return "";
}

public void SaveValue(string Teg, string Value){
	
	if(this.Storage.IndexOf(Teg)<0){
		this.Storage += "\n"+Teg+"("+Value+")";
	}else{
		int St = this.Storage.IndexOf(Teg);//+(Teg).Length;
		int Fi = this.Storage.IndexOf(")",St);
		if(Fi<0)Fi=this.Storage.Length;
		string ForwText = this.Storage.Substring(St, Fi-St);
		this.Storage = this.Storage.Replace(ForwText, Teg+"("+Value);
	}
	//this.Storage ="";
}
public void DepthStep(float tendency=1f){
	float StepLimit = (2f/Piston_List.Count);
	foreach (IMyPistonBase Piston in  Piston_List){
					//if(Piston.MaxLimit >= 10f){Mod = "stop"; break;}
					//if ( Piston.CubeGrid.GridSizeEnum == MyCubeSize.Large ){ MaxDepth+=10f; }else{  MaxDepth+=2;}
					bool Reverse =Vector3D.Dot(Piston.WorldMatrix.Up,-Up_ProgBlock) >= -0.7;
					if(tendency < 0f) Reverse=!Reverse;
					if(Reverse){
						//TotalDepth+=Piston.CurrentPosition;
						Piston.MaxLimit=Piston.MinLimit;
						Piston.MaxLimit += StepLimit;
						Piston.MinLimit=Piston.MaxLimit;
						Piston.ApplyAction("Extend");					//поршень выдвенуть
					}else{
						//TotalDepth+=Piston.CurrentPosition-Piston.MinLimit;
						Piston.MinLimit=Piston.MaxLimit;
						Piston.MinLimit -= StepLimit;
						Piston.MaxLimit=Piston.MinLimit;
						Piston.ApplyAction("Retract");				//поршень задвинуть
					}
					//Piston.MaxLimit += StepLimit;
					//Piston.ApplyAction("Extend");
				}
				//TotalDepth = TotalDepth/MaxDepth*Piston_List.Count;
}
public void AngleStep(float tendency=1f){
	//Echo("Z "+Z);
	//Runtime.UpdateFrequency = UpdateFrequency.None;
			RotoReady=false;
			Rotor_A.TargetVelocityRad =0f;
			Dlinna = (float)(Loong.Length()+(Drill_List[0].CubeGrid.GridSize * (((IMyCubeBlock)Drill_List[0]).Max.Y - ((IMyCubeBlock)Drill_List[0]).Min.Y + 1))/2);
			//Echo("Dlinna "+Dlinna);
			Z+=(float)(2f/((Dlinna*3.14f)/(180f)))*tendency;
			
			if(Math.Abs(Z)>Math.Abs(Rotor_Z.UpperLimitDeg)){
				//LimitDeg=Rotor_Z.UpperLimitDeg*tendency;
				Z=(Rotor_Z.UpperLimitDeg+1)*tendency;
			}
			SaveValue("Z", Convert.ToString(Z));
			//Echo("Z "+Z);
			//Echo("Верхний придел "+Rotor_Z.UpperLimitDeg);
			
}
public void SetupDrawSurface(IMyTextSurface surface){
    // Draw background color
    surface.ScriptBackgroundColor = new Color(0, 0, 0, 0);
    // Set content type
    surface.ContentType = ContentType.SCRIPT;
	colorPiston = surface.ScriptForegroundColor;
    // Set script to none
    surface.Script = "";
}
Color colorPiston;
public void DrawSprites(MySpriteDrawFrame frame, Vector2 centerPos, string Content="Fon", float scale = 1f){
	scale=scale*MeScale;
	switch (Content){
		
	case "ADM":
	centerPos += new Vector2(74,-86)*MeScale ;
	MySprite[] SpriteArray = new MySprite [14];
	SpriteArray[0] = new MySprite(SpriteType.TEXT, "Automatic Drilling Mining", new Vector2(-155f,70f)*scale+centerPos, null, new Color(255,255,255,255), "DEBUG", TextAlignment.LEFT, 0.8f*MeScale); // text5
	SpriteArray[1] = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(-103f,-17f)*scale+centerPos, new Vector2(100f,132f)*scale, new Color(128,255,128,255), null, TextAlignment.CENTER, 0f); // sprite A
    SpriteArray[2] = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(-103f,-62f)*scale+centerPos, new Vector2(19f,26f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f); // sprite ACopy
    SpriteArray[3] = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(-103f,11f)*scale+centerPos, new Vector2(58f,76f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f); // sprite A
    SpriteArray[4] = new MySprite(SpriteType.TEXTURE, "SemiCircle", new Vector2(0f,0f)*scale+centerPos, new Vector2(100f,100f)*scale, new Color(128,255,128,255), null, TextAlignment.CENTER, 1.5708f); // sprite11CopyCopy
    SpriteArray[5] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-19f,0f)*scale+centerPos, new Vector2(100f,40f)*scale, new Color(128,255,128,255), null, TextAlignment.CENTER, 1.5708f); // sprite11CopyCopyCopyCopy
    SpriteArray[6] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-27f,-16f)*scale+centerPos, new Vector2(28f,25f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.5708f); // sprite11CopyCopyCopyCopyCopyCopyCopy
    SpriteArray[7] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-5f,0f)*scale+centerPos, new Vector2(60f,19f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.5708f); // sprite11CopyCopyCopyCopyCopy
    SpriteArray[8] = new MySprite(SpriteType.TEXTURE, "SemiCircle", new Vector2(0f,0f)*scale+centerPos, new Vector2(60f,60f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.5708f); // sprite11CopyCopyCopy
    SpriteArray[9] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(122f,0f)*scale+centerPos, new Vector2(100f,100f)*scale, new Color(128,255,128,255), null, TextAlignment.CENTER, 0f); // sprite M 5
    SpriteArray[10] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(86f,-3f)*scale+centerPos, new Vector2(100f,20f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.0472f); // sprite M 5CopyCopyCopy
    SpriteArray[11] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(120f,38f)*scale+centerPos, new Vector2(51f,27f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f); // sprite M 5CopyCopyCopy
    SpriteArray[12] = new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(156f,-3f)*scale+centerPos, new Vector2(100f,20f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.0472f); // sprite M 5CopyCopy
    SpriteArray[13] = new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(120f,-27f)*scale+centerPos, new Vector2(50f,50f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 3.1416f); // sprite M 5Copy
	frame.AddRange(SpriteArray);
	 break;
	case "Fon":
	centerPos += new Vector2(20, -25)*MeScale ;
	frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-233f,100f)*scale+centerPos, new Vector2(350f,10f)*scale, new Color(79,167,255,255), null, TextAlignment.CENTER, 0f)); // sprite11CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(-54f,100f)*scale+centerPos, new Vector2(10f,10f)*scale, new Color(79,167,255,255), null, TextAlignment.CENTER, 1.5708f)); // sprite11CopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-20f,62f)*scale+centerPos, new Vector2(64f,6f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, -0.7854f)); // sprite11CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(76f,40f)*scale+centerPos, new Vector2(150f,5f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 0f)); // sprite11CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-216f,84f)*scale+centerPos, new Vector2(350f,6f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 0f)); // sprite11CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(171f,78f)*scale+centerPos, new Vector2(350f,10f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 0f)); // sprite11Copy
    frame.Add(new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(-24f,93f)*scale+centerPos, new Vector2(40f,40f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, -1.5708f)); // sprite11CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(19f,108f)*scale+centerPos, new Vector2(50f,50f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 1.5708f)); // sprite11Copy
    //frame.Add(new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(-151f,138f)*scale+centerPos, new Vector2(10f,10f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 1.5708f)); // sprite11CopyCopy
   // frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-230f,138f)*scale+centerPos, new Vector2(150f,10f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 0f)); // sprite11Copy
    frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(155f,40f)*scale+centerPos, new Vector2(11f,11f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 0f)); // sprite11Copy
    frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(155f,40f)*scale+centerPos, new Vector2(5f,5f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite11CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-179f,123f)*scale+centerPos, new Vector2(348f,20f)*scale, new Color(0,102,204,255), null, TextAlignment.CENTER, 0f)); // sprite11
	//frame.Add(new MySprite(SpriteType.TEXT, "Automatic ADM Mining", new Vector2(-55f,-10f)*scale+centerPos, null, new Color(255,255,255,255), "DEBUG", TextAlignment.LEFT, 0.8f)); // text5

	 break;
	case "Logo":
	centerPos += new Vector2(-101,-54)*MeScale ;
     frame.Add(new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(-11f,-8f)*scale+centerPos, new Vector2(200f,200f)*scale, new Color(255,255,255,255), null, TextAlignment.CENTER, 0f)); // sprite11
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-102f,87f)*scale+centerPos, new Vector2(20f,10f)*scale, new Color(0,0,0,0), null, TextAlignment.CENTER, 1.0472f)); // sprite4CopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(79f,85f)*scale+centerPos, new Vector2(20f,10f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.0472f)); // sprite4CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(70f,80f)*scale+centerPos, new Vector2(10f,77f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.0472f)); // sprite4CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(23f,93f)*scale+centerPos, new Vector2(10f,20f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.0472f)); // sprite4CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-44f,93f)*scale+centerPos, new Vector2(10f,20f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.0472f)); // sprite4CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-92f,81f)*scale+centerPos, new Vector2(10f,75f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.0472f)); // sprite4Copy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-7f,-12f)*scale+centerPos, new Vector2(15f,5f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -2.3562f)); // sprite4CopyCopyCopyCopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-21f,43f)*scale+centerPos, new Vector2(18f,6f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -2.3562f)); // sprite4CopyCopyCopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(13f,38f)*scale+centerPos, new Vector2(61f,6f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -2.3562f)); // sprite4CopyCopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-53f,53f)*scale+centerPos, new Vector2(15f,5f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.309f)); // sprite4CopyCopyCopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(2f,37f)*scale+centerPos, new Vector2(18f,6f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.309f)); // sprite4CopyCopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-19f,10f)*scale+centerPos, new Vector2(61f,6f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.309f)); // sprite4CopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(24f,60f)*scale+centerPos, new Vector2(15f,4f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -0.2618f)); // sprite4CopyCopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-15f,21f)*scale+centerPos, new Vector2(18f,6f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -0.2618f)); // sprite4CopyCopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-28f,53f)*scale+centerPos, new Vector2(61f,6f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -0.2618f)); // sprite4CopyCopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-11f,89f)*scale+centerPos, new Vector2(55f,10f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite4CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-12f,63f)*scale+centerPos, new Vector2(101f,7f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite4CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-42f,19f)*scale+centerPos, new Vector2(41f,10f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.0472f)); // sprite4CopyCopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-36f,18f)*scale+centerPos, new Vector2(101f,7f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.0472f)); // sprite4CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(19f,18f)*scale+centerPos, new Vector2(41f,10f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.0472f)); // sprite4CopyCopy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(13f,17f)*scale+centerPos, new Vector2(101f,7f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.0472f)); // sprite4Copy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-11f,-70f)*scale+centerPos, new Vector2(20f,10f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite4Copy
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-11f,-58f)*scale+centerPos, new Vector2(10f,75f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite4, null, TextAlignment.CENTER, 0f)); // sprite4
	
	break;
	case "Button":
	
	break;
	}
}


public void DrawScale(MySpriteDrawFrame frame, Vector2 centerPos, string caption="Текст         100%", float Value = 0.15f, float scale = 1f)
{
	scale=scale*MeScale;
if(Math.Abs(Value)>1){Value=1;}else{Value=Math.Abs(Value);}
	//MySprite[] SpritsScale = new MySprite [7];
	float Sme=45f;
    
		
		MySprite[] SpritsScale = new MySprite [7];
		SpritsScale[0] = (new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-15f+Sme,0f)*scale+centerPos, new Vector2(136f*Value,18f)*scale, new Color(0.5f*Value,0.5f*(1f-Value),0f,1f), null, TextAlignment.LEFT, 0f)); // sprite0 Шкала
		SpritsScale[1] = (new MySprite(SpriteType.TEXTURE, "SquareHollow", new Vector2(50f+Sme,0f)*scale+centerPos, new Vector2(140f,20f)*scale, new Color(0,128,255,255), null, TextAlignment.CENTER, 0f)); // sprite1 Рамка
		SpritsScale[2] = (new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(109f+Sme,1f)*scale+centerPos, new Vector2(3f,24f)*scale, new Color(0,128,255,255), null, TextAlignment.CENTER, 0.7854f)); // sprite3Copy Рамка
		SpritsScale[3] = (new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-11f+Sme,-1f)*scale+centerPos, new Vector2(3f,25f)*scale, new Color(0,128,255,255), null, TextAlignment.CENTER, 0.7854f)); // sprite3  Рамка
		SpritsScale[4] = (new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(111f+Sme,1f)*scale+centerPos, new Vector2(20f,20f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, -1.5708f)); // sprite2Copy Маска
		SpritsScale[5] = (new MySprite(SpriteType.TEXTURE, "RightTriangle", new Vector2(-12f+Sme,-1f)*scale+centerPos, new Vector2(20f,20f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 1.5708f)); // sprite2 Маска
		SpritsScale[6] = (new MySprite(SpriteType.TEXT, caption, new Vector2(-115f,-13f)*scale+centerPos, null, new Color(255,255,255,255), "DEBUG", TextAlignment.LEFT, 0.8f*scale)); // text1
		
		
	
	frame.AddRange(SpritsScale);
}
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++



float ColorScale = 0f;
float RotationOnOff = 0f;
float RotationPause = 0f;
float RotationAngle	= 0f;
float RotationDepth = 0f;
//float[] rotation = new bool[4]{false,false,false,true};
public void ButtonSprites(MySpriteDrawFrame frame,Vector2 centerPos, string ButtonName="On/Off", bool Active = false , float scale = 0.26f)
{
	scale=scale*MeScale;
	if(Mod=="stop"){if(ColorScale>0f) ColorScale-=0.1f;}else{if(ColorScale<0.6f) ColorScale+=0.1f;}
	//if(Active)RotationAngle=rotation;
	 frame.Add(new MySprite(SpriteType.TEXTURE, "CircleHollow", new Vector2(0f,0f)*scale+centerPos, new Vector2(250f,250f)*scale, new Color(1f-ColorScale,0f+ColorScale,0f ,1f), null, TextAlignment.CENTER, 0f)); // CircleHollow
	switch (ButtonName){	
		//--------------------------------------------- 
		//case "eror":
		default:
		
		//case "On/Off":
			if(Active) {
			RotationOnOff+= 17f*(float)Math.PI/180; 
			}
			DrawRotationSprites(frame,centerPos,scale,RotationOnOff);
			//float ColorStep=0f;
			
			
			frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(0f,0f)*scale+centerPos, new Vector2(150f,150f)*scale, new Color(0.5019608f-ColorScale,0f+ColorScale,0f ,1f), null, TextAlignment.CENTER, 0f)); // sprite3
			frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(0f,0f)*scale+centerPos, new Vector2(100f,100f)*scale, new Color(0f ,0f ,0f ,1f), null, TextAlignment.CENTER, 0f)); // sprite3Copy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(0f,-57f)*scale+centerPos, new Vector2(62f,72f)*scale, new Color(0f ,0f ,0f ,1f), null, TextAlignment.CENTER, 0f)); // sprite1Copy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(0f,-57f)*scale+centerPos, new Vector2(27f,72f)*scale, new Color(0.5019608f-ColorScale,0f+ColorScale,0f ,1f), null, TextAlignment.CENTER, 0f)); // sprite1
	
		break;
		case "pause":
			if(Active) {RotationPause+= 20f*(float)Math.PI/180;}
			float ColorPause = 0f;
			if(Mod=="pause"){ ColorPause=ColorScale;}else{ ColorPause=0f;}
			DrawRotationSprites(frame,centerPos,scale,RotationPause);
			    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(25f,0f)*scale+centerPos, new Vector2(25f,120f)*scale, new Color(0.3f+ColorPause,0.4f+ColorPause,0.5f-ColorPause,1f), null, TextAlignment.CENTER, 0f)); // sprite10Copy
				frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-25f,0f)*scale+centerPos, new Vector2(25f,120f)*scale, new Color(0.3f+ColorPause,0.4f+ColorPause,0.5f-ColorPause,1f), null, TextAlignment.CENTER, 0f)); // sprite10
			//frame.Add(new MySprite(SpriteType.TEXT, "P", new Vector2(-83f,-149f)*scale+centerPos, null, new Color(255,255,255,255), "Monospace", TextAlignment.LEFT, 10f*scale)); // text7
		break;
		case "angle":
			float ColorAngle = 0f;
			if(Mod=="angle"){ ColorAngle=ColorScale;}else{ ColorAngle=0f;}
			if(Active) {RotationAngle+= 20f*(float)Math.PI/180;}
			DrawRotationSprites(frame,centerPos,scale,RotationAngle);
			 frame.Add(new MySprite(SpriteType.TEXTURE, "CircleHollow", new Vector2(0f,-13f)*scale+centerPos, new Vector2(151f,141f)*scale, new Color(0.2f,0.4f+ColorAngle,0.5f,1f), null, TextAlignment.CENTER, 3.1416f)); // стрелка10CopyCopy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(0f,-21f)*scale+centerPos, new Vector2(154f,128f)*scale, new Color(0,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite10CopyCopyCopy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-35f,0f)*scale+centerPos, new Vector2(143f,15f)*scale, new Color(0.2f,0.4f+ColorAngle,0.5f,1f), null, TextAlignment.CENTER, -1.0472f)); // sprite10Copy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(35f,0f)*scale+centerPos, new Vector2(143f,15f)*scale, new Color(0.2f,0.4f+ColorAngle,0.5f,1f), null, TextAlignment.CENTER, 1.0472f)); // sprite10
			frame.Add(new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(-40f,44f)*scale+centerPos, new Vector2(21f,20f)*scale, new Color(0.2f,0.4f+ColorAngle,0.5f,1f), null, TextAlignment.CENTER, -1.0472f)); // стрелка1Copy
			frame.Add(new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(41f,44f)*scale+centerPos, new Vector2(20f,20f)*scale, new Color(0.2f,0.4f+ColorAngle,0.5f,1f), null, TextAlignment.CENTER, 1.0472f)); // стрелка

		break;
		case "depth":
			float ColorDepth = 0f;
			if(Mod=="depth"){ ColorDepth=ColorScale;}else{ ColorDepth=0f;}
			if(Active) {RotationDepth+= 20f*(float)Math.PI/180;}
			DrawRotationSprites(frame,centerPos,scale,RotationDepth);
			 frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-23f,57f)*scale+centerPos, new Vector2(15f,80f)*scale, new Color(0.2f,0.4f+ColorDepth,0.5f,1f), null, TextAlignment.CENTER, -0.7854f)); // sprite10CopyCopyCopy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(23f,57f)*scale+centerPos, new Vector2(15f,80f)*scale, new Color(0.2f,0.4f+ColorDepth,0.5f,1f), null, TextAlignment.CENTER, 0.7854f)); // sprite10CopyCopy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(25f,-5f)*scale+centerPos, new Vector2(20f,120f)*scale, new Color(0.2f,0.4f+ColorDepth,0.5f,1f), null, TextAlignment.CENTER, 0f)); // sprite10Copy
			frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(-25f,-5f)*scale+centerPos, new Vector2(20f,120f)*scale, new Color(0.2f,0.4f+ColorDepth,0.5f,1f), null, TextAlignment.CENTER, 0f)); // sprite10

		break;
		
	}//*/
    // frame.Add(new MySprite(SpriteType.TEXTURE, "Circle", new Vector2(0f,0f)*scale+centerPos, new Vector2(150f,150f)*scale, new Color(128,0,0,255), null, TextAlignment.CENTER, 0f)); // sprite3
    frame.Add(new MySprite(SpriteType.TEXTURE, "DecorativeBracketRight", new Vector2(195f,0f)*scale+centerPos, new Vector2(100f,300f)*scale, new Color(0f ,0.4f ,0.8f ,1f), null, TextAlignment.CENTER, 0f)); // DecorativeBracketRight
    frame.Add(new MySprite(SpriteType.TEXTURE, "DecorativeBracketLeft", new Vector2(-195f,0f)*scale+centerPos, new Vector2(100f,300f)*scale, new Color(0f ,0.4f ,0.8f ,1f), null, TextAlignment.CENTER, 0f)); // DecorativeBracketLeft
}


public void DrawRotationSprites(MySpriteDrawFrame frame, Vector2 centerPos, float scale = 1f, float rotation = 0f)
{
	MySprite[] ButtonAnimation = new MySprite [3];
    float sin = (float)Math.Sin(rotation);
    float cos = (float)Math.Cos(rotation);
   //				new MySprite(SpriteType.TEXTURE, Sprites[i], 												   Offset, 					 SpriteSize, 			   Color.White,   "", TextAlignment.LEFT, 0);
	ButtonAnimation[0]= new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(cos*0f-sin*0f,sin*0f+cos*0f)*scale+centerPos, new Vector2(360f,226f)*scale, new Color(0f ,0f ,0f ,1f), null, TextAlignment.CENTER, -4.6775f+rotation); // rotation
	ButtonAnimation[1]= new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(cos*0f-sin*0f,sin*0f+cos*0f)*scale+centerPos, new Vector2(360f,226f)*scale, new Color(0f ,0f ,0f ,1f), null, TextAlignment.CENTER, 3.8048f+rotation); // rotation
	ButtonAnimation[2]= new MySprite(SpriteType.TEXTURE, "Triangle", new Vector2(cos*0f-sin*0f,sin*0f+cos*0f)*scale+centerPos, new Vector2(360f,226f)*scale, new Color(0f ,0f ,0f ,1f), null, TextAlignment.CENTER, -0.5585f+rotation); // rotation
	frame.AddRange(ButtonAnimation);
	
}
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/

void Rotation_A(float Azimuth){
	Azimuth = (float)(Azimuth*Math.PI/180);
	float RAngle=0;
	RAngle = CutTurn(Azimuth-Rotor_A.Angle);
	Rotor_A.TargetVelocityRad = RAngle*0.5f;
	if(Math.Abs(RAngle) > 0.05){
		RotoReady=false;
	}else{
		RotoReady=true;
		Rotor_A.TargetVelocityRad = 0f;
	}
}
void Rotation_Z(float Zenit){
	Zenit = (float)(Zenit*Math.PI/180);
	float RAngle=0;
	RAngle = CutTurn(Zenit-Rotor_Z.Angle);
	Rotor_Z.TargetVelocityRad = RAngle*0.5f;
	if(Math.Abs(RAngle) > 0.05){
		RotoReady=false;
	}else{
		RotoReady=true;
		Rotor_Z.TargetVelocityRad = 0f;
	}
}


void H2Rotors(float Zenit, float Azimuth){
		Zenit = (float)(Zenit*Math.PI/180);
		Azimuth = (float)(Azimuth*Math.PI/180);
        

        float RAngle1=0;
		float RAngle2=0;
		float SumAngle=0;
		
			RAngle1 = CutTurn(Azimuth-Rotor_A.Angle);
			Rotor_A.TargetVelocityRad = RAngle1*0.5f;
			SumAngle+=RAngle1;
			
			RAngle2 = CutTurn(Zenit-Rotor_Z.Angle);
			Rotor_Z.TargetVelocityRad = RAngle2*0.5f;
			SumAngle+=RAngle2;
			
		
		if(Math.Abs(SumAngle) > 0.05){
			RotoReady=false;
			//Echo("H2R !Abs "+Math.Abs(SumAngle));
		}else{
			RotoReady=true;
			foreach (IMyMotorStator Rotor in  Rotor_List){
				Rotor.TargetVelocityRad = 0f;
			}
			//Echo("H2R Abs "+Math.Abs(SumAngle));
		}
		
    }
	private float CutTurn(float Turn)
    {
        if (float.IsNaN(Turn)) Turn = 0;
        if (Turn < -Math.PI) Turn += 2 * (float)Math.PI;
        else if (Turn > Math.PI) Turn -= 2 * (float)Math.PI;
        return Turn;
    }
	//----------------------------------------------------------
