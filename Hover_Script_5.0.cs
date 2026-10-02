	//Объявляем нужные блоки как глоб. переменные
	IMyShipController RemCon;
    List<IMyShipController> RemConList = new List<IMyShipController>();
    List<IMyGyro> gyroList = new List<IMyGyro>();
	List<IMyThrust> Thrusters;
	List<IMyTextPanel> LCD_List = new List<IMyTextPanel>();
	List<IMyTerminalBlock> MyList = new List<IMyTerminalBlock>();
	//List<IMyTerminalBlock> Progblocks = new List<IMyTerminalBlock>();
	//IMyTextSurfaceProvider ProgBlock;
	string GROUP_NAME_RU = "Ховер";
	string GROUP_NAME_EN = "Hover";
	
	string language = "RU";

	//Находим блоки, устанавливаем частоту обновления
	public Program()
    {
		Runtime.UpdateFrequency = UpdateFrequency.Update1;
		
		language= this.Storage;
		
		List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();  //переменная для групп   
		GridTerminalSystem.GetBlockGroups(blockGroups);     //получаем список групп
		
	//Ищем группу 
		GridTerminalSystem.GetBlocksOfType<IMyShipController>(RemConList, g => g.IsSameConstructAs(Me));
		/*RemCon = (gts<IMyShipController>(x => x.CustomName.Contains("hover")).DefaultIfEmpty(RemCon)).First();  
		if(RemCon  == null){
			RemCon = (gts<IMyShipController>().DefaultIfEmpty(RemCon)).First();
			if(RemCon  == null) ERR_TXT += "\nКокпит не найден";
		} */
		GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyroList, g => g.IsSameConstructAs(Me));
		GridTerminalSystem.GetBlocksOfType<IMyThrust>(Thrusters, g => g.IsSameConstructAs(Me));
		//GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(Progblocks, g => g.IsSameConstructAs(Me));
		GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(LCD_List, g => g.IsSameConstructAs(Me));
		Echo("the Hover group contains:");
	for(var i = 0; i < blockGroups.Count; i++){//сканим по всем группам
		if(blockGroups[i].Name == GROUP_NAME_RU || blockGroups[i].Name == GROUP_NAME_EN){//условие на совподение названия группы
			//Получаем группу и раскидываем блоки по типам.
			
			if(blockGroups[i].Name == GROUP_NAME_EN){ language="EN"; this.Storage ="EN";}
			
			List<IMyShipController> CL = new List<IMyShipController>();
			blockGroups[i].GetBlocksOfType<IMyShipController>(CL, g => g.IsSameConstructAs(Me)); Echo("Cockpit "+CL.Count);
			if(CL.Count>0)RemConList = CL;
			
			List<IMyGyro> GL = new List<IMyGyro>();
			blockGroups[i].GetBlocksOfType<IMyGyro>(GL, g => g.IsSameConstructAs(Me)); Echo("Gyro "+GL.Count);
			if(GL.Count>0)gyroList = GL;
			
			List<IMyThrust> TL = new List<IMyThrust>();
			blockGroups[i].GetBlocksOfType<IMyThrust>(TL, g => g.IsSameConstructAs(Me)); Echo("Thrusters "+TL.Count);
			if(TL.Count>0)Thrusters = TL;
			
			List<IMyTextPanel> TPL = new List<IMyTextPanel>();
			blockGroups[i].GetBlocksOfType<IMyTextPanel>(TPL, g => g.IsSameConstructAs(Me)); Echo("Text Panel "+TPL.Count);
			if(TPL.Count>0)LCD_List = TPL;
			break;   
		}   
	}
		//foreach (IMyShipDrill Thruster in  Thrusters){  //цикл 
		//	Thruster.ApplyAction("OnOff_Off")	;			// выключить
		//}
		//Me.CustomData = "Cockpit "+RemConList.Count+"\ngyro "+gyroList.Count+"\nThrusters "+Thrusters.Count+"\nText Panel "+LCD_List.Count;
		RemCon = RemConList?[0] as IMyShipController;
		//RemCon.TryGetPlanetElevation(MyPlanetElevation.Surface, out DesiredElevation);
		
		//ProgBlock = Progblocks[0] as IMyProgrammableBlock;
		Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
		Me.GetSurface(0).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
		Me.GetSurface(0).FontColor = new Color(21, 72, 41);
		//if(Me.CubeGrid)
		Me.GetSurface(0).FontSize = (float)1.3;
		
	}
	//в главной функции запускаем скрипт в рабочем режиме или останавливаем в зависимости от аргумента
	public void Main(string argument, UpdateType uType)
    {
		switch (argument)
        {
            case "start":
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            break;
            case "stop":
            foreach (IMyThrust Thruster in Thrusters)
			{
				Thruster.ThrustOverridePercentage = 0f;
			}
			foreach (IMyGyro gyro in gyroList)
			{
				gyro.GyroOverride = false;
			}
			RemCon.DampenersOverride = true;
			Runtime.UpdateFrequency = UpdateFrequency.None;
			Echo("\n ---<скрипт остановлен>---\n Запустите с аргументом\n start");
            break;
			case "en":
			language="EN"; 
			this.Storage ="EN";
			break;
			case "ru":
			language="RU"; 
			this.Storage ="RU";
			break;
            default:
			Vector3D GravityVector = RemCon.GetNaturalGravity();
			Vector3D GravNorm = Vector3D.Normalize(GravityVector);
			Vector3D TargetVector=HoverVector(GravityVector);
			double MoveLeft = RemCon.MoveIndicator.X;
			
			Vector3D TargetVector2=RemCon.WorldMatrix.Forward+RemCon.WorldMatrix.Left*(RemCon.MoveIndicator.X*TargetVector.Dot(GravNorm)*TargetVector.Dot(GravNorm)/2);
			
			TurnTarget(TargetVector, TargetVector2);
			HoverThrust();
			HoverDispley();
			break;
        }
		
	}
	
	//Рабочие процедуры Функций
	public void TurnTarget(string arg){
		if (arg == "stop")
        {
            foreach (IMyThrust Thruster in Thrusters)
            {
                Thruster.ThrustOverridePercentage = 0f;
            }
			foreach (IMyGyro gyro in gyroList)
			{
				gyro.GyroOverride = false;
			}
            RemCon.DampenersOverride = true;
            Runtime.UpdateFrequency = UpdateFrequency.None;
			Echo("\n ---<скрипт остановлен>---\n Запустите с аргументом start");
			
        }else{
			if(arg =="start")Runtime.UpdateFrequency = UpdateFrequency.Update1;
		}
	}
	
	//---------------------Поворот на цель------------------------------
	public void TurnTarget(Vector3D TargetVector, Vector3D TargetVector2)
    {
		Vector3D Forward_RC = RemCon.WorldMatrix.Forward;
		Vector3D Left_RC = RemCon.WorldMatrix.Left;
		Vector3D Up_RC = RemCon.WorldMatrix.Up;
		
		//Получаем проекции вектора прицеливания на все три оси блока ДУ. 
		double gF = TargetVector.Dot(Forward_RC);
		double gL = TargetVector.Dot(Left_RC);
		double gU = TargetVector.Dot(Up_RC);
		
		double gF2 = TargetVector2.Dot( RemCon.WorldMatrix.Forward);
        double gL2 = TargetVector2.Dot(RemCon.WorldMatrix.Left);
		float YawInput2 = (float)Math.Atan2(gL2,gF2);
		
		float PitchInput0=0;
		float YawInput0=0;
		float RollInput0=0;
		//Получаем сигналы по тангажу и крены операцией atan2
		PitchInput0 = (float)Math.Atan2(gF, -gU);
		YawInput0 = (float)Math.Atan2(gL,gF);
		RollInput0 = (float)Math.Atan2(gL, -gU);
		
		Vector3D TargetRotation = new Vector3D(-PitchInput0, YawInput2, RollInput0);//вектор который надо преобразовать от RemCon к Гироскопу
        Vector3D vec2 = new Vector3D();
        Matrix M1 = new Matrix();
        RemCon.Orientation.GetMatrix(out M1);//матрица ориентации RemcCon относительно грид
        Matrix M2 = new Matrix();
		
		foreach (IMyGyro gyro in gyroList)
        {
			gyro.Orientation.GetMatrix(out M2);//Матрица ориентации Гироскопа относительно грид
            M2 = Matrix.Transpose(M2);//транспонируем для конвертации от старшей системы отсчета кмладшей
            M2 = M1* M2;// получаем матрицу поворота вектора от RemCon к Гироскопу
			vec2 = Vector3D.TransformNormal(TargetRotation,M2);//вектор преобразованный к гироскопу
			gyro.GyroOverride = true;
			gyro.Pitch = (float)vec2.X;
            gyro.Yaw = (float)vec2.Y;
            gyro.Roll = (float)vec2.Z;
		}
		
	}
	//----------------------------------------------------------------------
	
	//-----------------задаёт вектор тяги ховеркрафта----------------
	double DesiredForwardVelocity = 0;
	Vector3D HoverVector (Vector3D GravityVector)
	{
		double ForwardVelocity = 0;
		double BackwardVelocity = 0;
		float ShipMass = RemCon.CalculateShipMass().PhysicalMass;
		//Vector3D GravityVector = RemCon.GetNaturalGravity();
		Vector3D VesVector = GravityVector*ShipMass;
		double ves = VesVector.Length();
		Vector3D GravNorm = Vector3D.Normalize(GravityVector);
		Vector3D MyVelocityVector = RemCon.GetShipVelocities().LinearVelocity;
		Vector3D TargetVector=GravNorm;
		
		//-------найти ускорители вниз---------          
		GridTerminalSystem.GetBlocksOfType<IMyThrust>(MyList);     
		double max = 0;  
		float enginesThrust = 0.0f;//Текущая тяга   
		foreach (IMyTerminalBlock thr in MyList){      
			if(thr.WorldMatrix.Backward.Dot(RemCon.WorldMatrix.Up) > 0.7071){//Это условие на совподение напровлений примерно на 70%
				IMyThrust curr_thr = (IMyThrust)thr;  
				max += curr_thr.MaxThrust;                 
				enginesThrust += curr_thr.CurrentThrust; 
			}else{
				if(thr.WorldMatrix.Backward.Dot(RemCon.WorldMatrix.Forward) > 0.7071) {
					ForwardVelocity = MyVelocityVector.Dot(-RemCon.WorldMatrix.Forward);
					thr.ApplyAction("OnOff_On");
				}else{
					if(thr.WorldMatrix.Backward.Dot(RemCon.WorldMatrix.Backward) > 0.7071){
						BackwardVelocity = MyVelocityVector.Dot(RemCon.WorldMatrix.Forward);
						thr.ApplyAction("OnOff_On");
					}else{
						thr.ApplyAction("OnOff_Off");
					}
				}
			}        
		}      
		//--------------------------------------- */
		
		
		
		//---Вводные сигналы---
		//
		if(ForwardVelocity!=0 && RemCon.MoveIndicator.Z != 0){
			if(RemCon.MoveIndicator.Z < 0){
				DesiredForwardVelocity=ForwardVelocity;
			}else{
				DesiredForwardVelocity=BackwardVelocity;
			}
		}else{
			if(!RemCon.DampenersOverride || DesiredElevation > 10){
			
				DesiredForwardVelocity += RemCon.MoveIndicator.Z/5;
				//if(RemCon.MoveIndicator.Z == 0)DesiredForwardVelocity +=1/5d;
			}else{
				if(ForwardVelocity>0){
					DesiredForwardVelocity=ForwardVelocity;
				}else{
					if(BackwardVelocity!=0){
						DesiredForwardVelocity=-BackwardVelocity;
					}
						DesiredForwardVelocity = RemCon.MoveIndicator.Z*2;
					
					
				}
				
			}
			if(DesiredForwardVelocity > 0 && BackwardVelocity==0)DesiredForwardVelocity=4;
			if((DesiredForwardVelocity > 0 )  & RemCon.MoveIndicator.Z == 0) {DesiredForwardVelocity=0;}
		}
		
		Vector3D ForwardVelocityVector = RemCon.WorldMatrix.Forward*DesiredForwardVelocity+RemCon.WorldMatrix.Left*RemCon.RollIndicator*3;
		
		MyVelocityVector =MyVelocityVector+ForwardVelocityVector;
		
		//----------------------*/
		float VerticalVelocity = -(float)MyVelocityVector.Dot(GravNorm);
		
		MyVelocityVector=Vector3D.Reject(MyVelocityVector, GravNorm);
		
		
		Vector3D VectorVerticalThrust = RemCon.WorldMatrix.Up*max;
		float VerticalThrust=-(float)VectorVerticalThrust.Dot(GravNorm);
		Vector3D MaxDeviation = Vector3D.Normalize(MyVelocityVector)*Math.Sqrt(max*max-ves*ves);
		Vector3D Deviation = MyVelocityVector*ShipMass;
		if(MaxDeviation.Length() < Deviation.Length()){
			MyVelocityVector= MaxDeviation;
		}else{MyVelocityVector= Deviation;}
		
		TargetVector= 2*VesVector + MyVelocityVector;
		TargetVector=Vector3D.Normalize(TargetVector);
		return TargetVector;
	}
	//----------------------------------------------------------------------*/
	
	//--------------------Простой Компенсатор для ховера--------------------
	double DesiredElevation = 10;
	double CurrentElevation = 0;
	bool Dive = false;
	public void HoverThrust()
	{
		Vector3D GravityVector = RemCon.GetNaturalGravity();
		Vector3D GravNorm = Vector3D.Normalize(GravityVector);
		float ShipMass = RemCon.CalculateShipMass().PhysicalMass;
		Vector3D VesVector = GravityVector*ShipMass;
		float ves = (float)VesVector.Length();
		Vector3D MyVelocityVector = RemCon.GetShipVelocities().LinearVelocity;
		Thrusters = new List<IMyThrust>();
		GridTerminalSystem.GetBlocksOfType<IMyThrust>(Thrusters);
		
		
		
        RemCon.TryGetPlanetElevation(MyPlanetElevation.Surface, out CurrentElevation);
		
		float VerticalVelocity = -(float)MyVelocityVector.Dot(GravNorm);
		
		if(Dive == false){
			DesiredElevation = CurrentElevation;
			Dive=true;
		}
		/*if( !RemCon.DampenersOverride){
			if(CurrentElevation>0 && DesiredElevation<0) DesiredElevation=0;
			DesiredElevation += RemCon.MoveIndicator.Y*0.4;
			
			
		}else{//*/
			
				
			//if(RemCon.MoveIndicator.Y==0)
			if(DesiredElevation<0)CurrentElevation=-CurrentElevation;				
			if(RemCon.MoveIndicator.Y != 0){
				DesiredElevation = CurrentElevation;
				DesiredElevation += RemCon.MoveIndicator.Y*2;
			} 
			VerticalVelocity = VerticalVelocity - RemCon.MoveIndicator.Y*3;
			
			//if(DesiredElevation>0)Dive = false;
		//}
		//------------------------------------------------
		
       // float DeltaElevation = (float)(Math.Abs(DesiredElevation) - CurrentElevation);
		float DeltaElevation = (float)(DesiredElevation - CurrentElevation);
		if( !RemCon.DampenersOverride && DeltaElevation>0)DeltaElevation=DeltaElevation*100;
		//коэфф-ты скорости и ускорения для управления тягой
		float kV = 1;
		float kA = 1;
		float TiltCos = (float)RemCon.WorldMatrix.Down.Dot(GravNorm);	
		//----------------------------------------------------------------
		
		float Thrust = ((1 + (DeltaElevation * kV - VerticalVelocity*Math.Abs(VerticalVelocity)) * kA)*(ves)/ TiltCos);
		if (Thrust <= 0){Thrust = 1;}
		float CountThrusters=0;
		foreach (IMyThrust thr in Thrusters)
		{
			if(thr.WorldMatrix.Backward.Dot(RemCon.WorldMatrix.Up) > 0.7071){CountThrusters++;}//else{thr.ApplyAction("OnOff_Off");}
		}
		foreach (IMyThrust thr in Thrusters)
		{	
			if(thr.WorldMatrix.Backward.Dot(RemCon.WorldMatrix.Up) > 0.7071){
				thr.ThrustOverride = Thrust / CountThrusters;
				
			}
		}
	}
	//----------------------------------------------------------------------
	
	//------------------------Вывод на дисплеи-----------------------------
	
	public void HoverDispley()
    {
		
		//double CurrentElevation = 0;
		
        //RemCon.TryGetPlanetElevation(MyPlanetElevation.Surface, out CurrentElevation);
		//-------найти ускорители вниз---------          
		GridTerminalSystem.GetBlocksOfType<IMyThrust>(MyList);     
		double max = 0;  
		float enginesThrust = 0.0f;//Текущая тяга   
		foreach (IMyTerminalBlock thr in MyList){      
			if(thr.WorldMatrix.Backward.Dot(RemCon.WorldMatrix.Up) > 0.7071){//Это условие на совподение напровлений примерно на 70%
				IMyThrust curr_thr = (IMyThrust)thr;  
				max += curr_thr.MaxThrust;                 
				enginesThrust += curr_thr.CurrentThrust; 
			}        
		}      
		//--------------------------------------- */
		
		double total_mass = RemCon.CalculateShipMass().TotalMass;
		double PhysicalShipMass = RemCon.CalculateShipMass().PhysicalMass;
		double base_mass = RemCon.CalculateShipMass().BaseMass;
		
		
		double CargoMass = (total_mass-base_mass);
		
		double PhysicalCargoMass = (PhysicalShipMass-base_mass);
		
		double InventMult = PhysicalCargoMass/CargoMass;
		
		double G_ms = RemCon.GetNaturalGravity().Length();
		
		double shipWeight = PhysicalShipMass * G_ms; //вес в ньютонах корабля
		//---------------------------------------   cargo
     
		double capacity = (max/G_ms-base_mass)*10; 
		capacity = capacity*InventMult; // Math.Round(capacity) округление до целого. 
		//--сформировать строку индикатора веса---- Weight 
		string TulBar = "\n Вес корабля[";  
		for (int x = 0; x <= 38; x++) {        
			if (shipWeight >= max/38*x) {        
				TulBar += "|";        
			} else {        
				TulBar += "'";        
			}        
		}  
		TulBar += "]";  
		//---------------------------------------  
		string[] DisplayText= new string[3];
		 DisplayText[1]=" 2 "; 
		 DisplayText[2]=" 3 ";
		double MyVelocity = RemCon.GetShipVelocities().LinearVelocity.Length();
		
		//--сформировать строки индикаторов---- 
		//---------------------горизонтальные тул бары--------------------------
		string TulBarVelocity = " скорость";
		string TulBarElevation = " Высота";
		string TulBarWeight = " Вес";
		if(language == "EN"){
			TulBarVelocity = "    speed";
			TulBarElevation = "Elevat.";
			TulBarWeight = "Weight";
		}
		
		TulBarVelocity += String.Format("[{0, 4}<",Math.Round(MyVelocity))+String.Format(">{0, -4}] м.\n [",Math.Round(-DesiredForwardVelocity));
		TulBarElevation += String.Format("[{0, 6}<",Math.Round(CurrentElevation))+String.Format(">{0, -6}] м.\n [",Math.Round(DesiredElevation));
		TulBarWeight += String.Format("[{0, 7}<",Math.Round(shipWeight/1000))+String.Format(">{0, -7}] T.\n [",Math.Round(max/1000)); 
		for (int x = 0; x <= 38; x++) {
			//---скорость---------------
			if (MyVelocity >= 100/38*x){
				if(-DesiredForwardVelocity >= 100/38*x && -DesiredForwardVelocity <= 100/38*(x+1))
				{
					TulBarVelocity += "V";
					}else{
						TulBarVelocity += "|";
					}
			} else {        
				if(-DesiredForwardVelocity >= 100/38*x && -DesiredForwardVelocity <= 100/38*(x+1))
				{
					TulBarVelocity += "V";
				}else{
					TulBarVelocity += "'";
				}
			}
			//---высота-----------------
			if (CurrentElevation >= 250/38*x){
				if(DesiredElevation >= 250/38*x && DesiredElevation < 250/38*(x+1))
				{
					TulBarElevation += "V";
					}else{
						TulBarElevation += "|";
					}
			} else {        
				if(DesiredElevation >= 250/38*x && DesiredElevation < 250/38*(x+1))
				{
					TulBarElevation += "V";
				}else{
					TulBarElevation += "'";
				}
			}
			//-------------------------------
			//------вес--------------------------------
			if (shipWeight >= max/38*x){
				TulBarWeight += "|";
			} else {        
				TulBarWeight += "'";
			}
			//-------------------------------
			
		}  
		TulBarVelocity += "]";
		TulBarElevation += "]";
		TulBarWeight += "]";
		//-----------------------Вертикальные тул бары--------------------------------
		string TulBar0 = 			  "скорость     Вес       Высота\n";
		if(language == "EN")TulBar0 = "speed      Weight   Elevation\n";
		TulBar0 += String.Format("[{0, 6}<",Math.Round(MyVelocity))+String.Format(">{0, -6}]",Math.Round(-DesiredForwardVelocity));
		TulBar0 += "           "+String.Format("[{0, 6}<",Math.Round(CurrentElevation))+String.Format(">{0, -6}]",Math.Round(DesiredElevation));
		for (int x = 10; x >= 0; x--) {      
			 
			
			if (MyVelocity >= 100/10*x){
				if(-DesiredForwardVelocity >= 100/10*x && -DesiredForwardVelocity <= 100/10*(x+1))
				{
					TulBar0 += "\n|<------ >|";
					}else{
						TulBar0 += "\n|-|||||||||||||-|";
					}
			} else {        
				if(-DesiredForwardVelocity >= 100/10*x && -DesiredForwardVelocity <= 100/10*(x+1))
				{
					TulBar0 += "\n|<------ >|";
				}else{
					TulBar0 += "\n|_______|";
				}
			}
			//-------------------------вес-----------------------------------!!!
			if (shipWeight >= max/10*x){
				TulBar0 += "       |-|||-|";
			} else {
				TulBar0 += "       |'__'|";
			}
			//---высота-----------------
			if (CurrentElevation >= 500/10*x){
				if(DesiredElevation >= 500/10*x && DesiredElevation < 500/10*(x+1))
				{
					TulBar0 += "      |< ------>|";
				}else{
					TulBar0 += "      |-|||||||||||||-|";
				}
			} else {        
				if(DesiredElevation >= 500/10*x && DesiredElevation < 500/10*(x+1))
				{
					TulBar0 += "      |< ------>|";
				}else{
					TulBar0 += "      |_______|";
				}
			} 
		}
		DisplayText[0] = TulBar0;
		DisplayText[1] = TulBarElevation;
		DisplayText[2] = TulBarVelocity;
		
		//Echo(TulBar0); 
		Me.GetSurface(0).WriteText(TulBar0);
		//Echo("Block ["+LCD_List[0].CubeGrid+"]\nBlock ["+Me.CubeGrid);
		
		//GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(LCD_List); 	
		//Echo(""+LCD_List.Count);
		int j = 1;
		for(int i = 0; i < 3 && i < LCD_List.Count; i++) {                            
			                            
			if(LCD_List[i] != null){
				IMyTextPanel LCD = (IMyTextPanel) LCD_List[i];
				//Echo(""+LCD_List[i].BlockDefinition.SubtypeId);
				//Echo("\nLCD ["+LCD.BlockDefinition.SubtypeId+"]");
				//Echo(""+LCD.BlockDefinition.SubtypeId.Contains("SmallBlockCorner_LCD_1"));
				
				LCD.FontSize = (float)1.3;
				if(LCD.BlockDefinition.SubtypeId.Contains("Corner"))
				{
					LCD.WriteText(DisplayText[j]); 
					LCD.FontSize = (float)4f; 
					//Echo("FontSize=1.9");
					j++;
					if(j>2)j=1;
				}else{LCD.WriteText(DisplayText[0]);}
				LCD.FontColor = new Color(21, 72, 41);
				LCD.SetValue("Content", Convert.ToInt64(1));
				LCD.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
			}     
		}    
	}
	