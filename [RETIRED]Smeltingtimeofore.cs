//__________________Start__________________//
        
		string RefName = "Ref";
		string LCD_refiningTime = "LCDTime";

		int Pro = 0; //ProductivityModule 100%
		int ServerRefinerySpeed = 1; // Server refinery speed

		//__________________END__________________//



		IMyTextPanel LCD;
		public Program()
        {
			
			LCD = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(LCD_refiningTime);
			LCD.ContentType = ContentType.TEXT_AND_IMAGE;
			LCD.FontSize = 12f / 20;

			LCD.TextPadding = 0;
			LCD.Font = "Monospace";
			LCD.FontColor = new Color(255, 70, 0);
			LCD.BackgroundColor = Color.Black;

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
			//Переменные
			
			const int COLUMN_WIDTH = 12;
			const int COLUMN = 1;
			string info = "";

			float BaseSpeed = 0;
			int RefineryCount = 0;

			// Ore Refining Speed from
			int ORSCobalt = 1170;
			int ORSGold = 11700;
			int ORSIron = 93600;
			int ORSMagnesium = 4680;
			int ORSNickel = 2340;
			int ORSSilicon = 7800;
			int ORSSilver = 4680;
			int ORSStone = 468000;
			int ORSPlatinum = 1170;
			int ORSUranium = 1170;

			// Ore   
			float OreCobalt = 0;
			float OreGold = 0;
			float OreIron = 0;
			float OreMagnesium = 0;
			float OreNickel = 0;
			float OreSilicon = 0;
			float OreSilver = 0;
			float OreStone = 0;
			float OrePlatinum = 0;
			float OreUranium = 0;

			// Ore Time
			float CobaltTime = 0;
			float GoldTime = 0;
			float IronTime = 0;
			float MagnesiumTime = 0;
			float NickelTime = 0;
			float SiliconTime = 0;
			float SilverTime = 0;
			float StoneTime = 0;
			float PlatinumTime = 0;
			float UraniumTime = 0;
			float AllOreTime = 0;
			//Search Refinery
			List<IMyRefinery> gg = new List<IMyRefinery>();
			GridTerminalSystem.GetBlocksOfType(gg, b => b.CustomName.Contains(RefName));
			if (gg.Count == 0)
            {
				Echo("No refinery found with prefix - Ref");
            }
            else 
			{ 
				for (int i = 0; i < gg.Count; i++) 
				{
					RefineryCount = gg.Count;
					break;
				}
			}
			
			//Search All Inventories
			var allBlocks = new List<IMyTerminalBlock>();
			GridTerminalSystem.GetBlocks(allBlocks);
			List<IMyInventoryOwner> inventoryBlocks = new List<IMyInventoryOwner>(); 
			List<IMyInventory> inventories = new List<IMyInventory>(); 
			for (int x = 0; x < allBlocks.Count; x++){var InventoryOwner = allBlocks[x] as IMyInventoryOwner;if (InventoryOwner != null){inventoryBlocks.Add(InventoryOwner);}}
			for (int i = 0; i < inventoryBlocks.Count; i++)
			{inventories.Add(inventoryBlocks[i].GetInventory(0));if (inventoryBlocks[i].InventoryCount > 1){IMyInventory inventory = inventoryBlocks[i].GetInventory(1);inventories.Add(inventory);}}
			for (int j = 0; j < inventories.Count; j++)
			{ List<MyInventoryItem> items = new List<MyInventoryItem>();inventories[j].GetItems(items);
				for (int i = 0; i < items.Count; i++)
				{
					//Search Ore
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Cobalt") OreCobalt = OreCobalt + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Gold") OreGold = OreGold + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Iron") OreIron = OreIron + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Magnesium") OreMagnesium = OreMagnesium + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Nickel") OreNickel = OreNickel + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Silicon") OreSilicon = OreSilicon + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Silver") OreSilver = OreSilver + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Stone") OreStone = OreStone + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Platinum") OrePlatinum = OrePlatinum + float.Parse(items[i].Amount.ToString());
					if (items[i].ToString().Contains("Ore") && items[i].Type.SubtypeId == "Uranium") OreUranium = OreUranium + float.Parse(items[i].Amount.ToString());
				}
			}


			//Calculation of ore melting time
			
			BaseSpeed = ServerRefinerySpeed * ORSCobalt;
			CobaltTime = OreCobalt / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSGold;
			GoldTime = OreGold / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSIron;
			IronTime = OreIron / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSMagnesium;
			MagnesiumTime = OreMagnesium / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSNickel;
			NickelTime = OreNickel / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSSilicon;
			SiliconTime = OreSilicon / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSSilver;
			SilverTime = OreSilver / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSStone;
			StoneTime = OreStone / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSPlatinum;
			PlatinumTime = OrePlatinum / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);
			BaseSpeed = ServerRefinerySpeed * ORSUranium;
			UraniumTime = OreUranium / (((BaseSpeed) + (Pro * BaseSpeed)) * RefineryCount);

			AllOreTime = CobaltTime + GoldTime + IronTime + MagnesiumTime + NickelTime + SiliconTime + SilverTime + StoneTime + PlatinumTime + UraniumTime;

			//Out LCD information
			info += ($"Smelting time of ore v1.0 by Trooper\n{"".PadRight(36, '≡')}\nRefinery:{gg.Count,3} synchronized"+ "\n") + PadRight("Ore", COLUMN_WIDTH) + PadLeft("amount" + "       " + "time" + "\n", COLUMN);
			if (OreCobalt > 0) info += "\n" + PadRight("Cobalt:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreCobalt) + "       " + GetTimeString(CobaltTime) + "\n", COLUMN);
			if (OreGold > 0) info += "\n" + PadRight("Gold:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreGold) + "       " + GetTimeString(GoldTime) + "\n", COLUMN);
			if (OreIron > 0) info += "\n" + PadRight("Iron:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreIron) + "       " + GetTimeString(IronTime) + "\n", COLUMN);
			if (OreMagnesium > 0) info += "\n" + PadRight("Magnesium:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreMagnesium) + "       " + GetTimeString(MagnesiumTime) + "\n", COLUMN);
			if (OreNickel > 0) info += "\n" + PadRight("Nickel:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreNickel) + "       " + GetTimeString(NickelTime) + "\n", COLUMN);
			if (OreSilicon > 0) info += "\n" + PadRight("Silicon:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreSilicon) + "       " + GetTimeString(SiliconTime) + "\n", COLUMN);
			if (OreSilver > 0) info += "\n" + PadRight("Silver:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreSilver) + "       " + GetTimeString(SilverTime) + "\n", COLUMN);
			if (OreStone > 0) info += "\n" + PadRight("Stone:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreStone) + "       " + GetTimeString(StoneTime) + "\n", COLUMN);
			if (OrePlatinum > 0) info += "\n" + PadRight("Platinum:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OrePlatinum) + "       " + GetTimeString(PlatinumTime) + "\n", COLUMN);
			if (OreUranium > 0) info += "\n" + PadRight("Uranium:  ", COLUMN_WIDTH) + PadLeft("" + Math.Round(OreUranium) + "       " + GetTimeString(UraniumTime) + "\n", COLUMN);





			info += "\n\r";
			info = (info + "All Time together  " + GetTimeString(AllOreTime));
			LCD.WriteText(info);
			var і = Runtime; Echo($"{System.DateTime.UtcNow}\nCode runtime:{і.LastRunTimeMs:F2} ms");
		}


		//__________________CUSTOM__________________//
		public static string GetTimeString(float Hours)
		{DateTime dTime = new DateTime().AddHours(Hours);return dTime.ToString("HH") + "h : " + dTime.ToString("mm") + "m : " + dTime.ToString("ss") + "s";}
		string PadRight(string input, int num)
		{if (input.Length < num){for (int i = input.Length; i < num; i++){input += " ";}}return input;}
		string PadLeft(string input, int num){if (input.Length < num){for (int i = input.Length; i < num; i++){input = " " + input;}}return input;}
		//__________________END__________________//