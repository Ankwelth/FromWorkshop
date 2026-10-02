int SelfUpSys_perSecond	= 10;     	// 1 = 1 sec, 2 = 2 sec etc. 
int SelfUpSysCounter = 0;

// Initial blocks found in system
int InitialTotalBlocks = 0;
int InitialSlimBlocks = 0;

// Runtime variables
float BatteryLevel = 0f;
int BlockCount = 0;
int CurrentDamage = 0;
int BuildIntegrity = 0;
int TotalBlocks = 0;
int SlimBlocks = 0; 
bool ProjectorEnabled = true;
String DamageList = "";

public Program()
{
   Runtime.UpdateFrequency = UpdateFrequency.Update100;
   InitialTotalBlocks = 0;
   InitialSlimBlocks = 0; 
   GetHullIntegrity(ref InitialTotalBlocks, ref InitialSlimBlocks);
   TotalBlocks = InitialTotalBlocks;
   SlimBlocks = InitialSlimBlocks;
   Echo("Initial Save of State:" +
            "\nInitial Structure:" + InitialTotalBlocks.ToString() +
            "\nInitial Armor:" + InitialSlimBlocks.ToString());
   Me.GetSurface(0).Font = "Monospace";
   Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
   Me.GetSurface(0).FontSize = 2.8f;
   Me.GetSurface(0).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
   // Setup for faster first run
   BatteryLevel = GetBatteryLevel();
   GetTerminalBlocks(ref BlockCount, ref CurrentDamage, ref BuildIntegrity);
   SelfUpSysCounter = 1;
}

public void Main(string argument, UpdateType updateSource) 
{
  // slow down update rate
  if (SelfUpSysCounter == 0)	{ 
     SelfUpSysCounter = SelfUpSys_perSecond/2;
  } else {
     SelfUpSysCounter -= 1; 
  }

  // Spread processing over time
  if (SelfUpSysCounter == 5) {
          BatteryLevel = GetBatteryLevel();
  } else if (SelfUpSysCounter == 4) {
          BlockCount = 0;
          CurrentDamage = 0;
          BuildIntegrity = 0;
          GetTerminalBlocks(ref BlockCount, ref CurrentDamage, ref BuildIntegrity);
  } else if (SelfUpSysCounter == 3) {
          TotalBlocks = 0;
          SlimBlocks = 0; 
          GetHullIntegrity(ref TotalBlocks, ref SlimBlocks);
  } else if (SelfUpSysCounter == 2) {
          if (TotalBlocks < InitialTotalBlocks) {
               if (!ProjectorEnabled) {
                    SetProjector(true);
                    ProjectorEnabled = true;
               }
          } else {
               if (ProjectorEnabled) {
                    SetProjector(false);
                    ProjectorEnabled = false;
               }
          }
  } else if (SelfUpSysCounter == 1) {
          Echo("Status Report\n" +
          "\nStructure: " + TotalBlocks + "/" + InitialTotalBlocks +
          "\nArmor: " + SlimBlocks + "/" + InitialSlimBlocks +
          "\nParts: "+ BlockCount +
          "\nDamaged Parts: " + CurrentDamage +
          "\nIncomplete Parts: " + BuildIntegrity +
          "\nBattery Level: " + (BatteryLevel*100).ToString() + "%");
          Echo(DamageList);
  } else if (SelfUpSysCounter == 0) {
          // Display Info
          IMyTextSurface mesurface0 = Me.GetSurface(0);

          if (CurrentDamage != 0 || BuildIntegrity != 0) {
                 mesurface0.FontColor = new Color(255, 20, 0);
          } else if (TotalBlocks == InitialTotalBlocks && SlimBlocks == InitialSlimBlocks) {
                 mesurface0.FontColor = Color.LightBlue;
          } else {
                 mesurface0.FontColor = Color.Gold;    
          }
          mesurface0.WriteText("Parts:" + BlockCount +
                                    "\n"+ TotalBlocks + "/" + InitialTotalBlocks +
                                    "\n" + SlimBlocks + "/" + InitialSlimBlocks +
                                    "\nDamage:" + (CurrentDamage+BuildIntegrity).ToString() +
                                    "\n" + DrawBatteryLevel(BatteryLevel));
     }
} 

// Returns the number of blocks, with first value the total-blocks, and second value non-TerminalBlocks
public void GetHullIntegrity(ref int TotalBlocks, ref int SlimBlocks) {
    TotalBlocks = 0;
    SlimBlocks = 0; 
    for (int x = Me.CubeGrid.Min.X - 1; x <= Me.CubeGrid.Max.X + 1; x++) {
        for (int y = Me.CubeGrid.Min.Y - 1; y <= Me.CubeGrid.Max.Y + 1; y++) {
            for (int z = Me.CubeGrid.Min.Z - 1; z <= Me.CubeGrid.Max.Z + 1; z++) {
                var Location = new Vector3I(x, y, z);
                if (Me.CubeGrid.CubeExists(Location)) {
                     TotalBlocks++;
                     if (Me.CubeGrid.GetCubeBlock(Location) == null) {
                           SlimBlocks++;
                     }
                }
            }
        }
    }
}

// Returns the Terminal System status of blocks being damaged or under construction
public void GetTerminalBlocks(ref int BlockCount, ref int CurrentDamage, ref int BuildIntegrity) {
    // Check through the terminal system for damaged blocks
    CurrentDamage = 0;
    BuildIntegrity = 0;
    DamageList = "";
    List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType(Blocks, b => b.CubeGrid == Me.CubeGrid); 
    for (int i = 0; i < Blocks.Count; i++) { 
        IMyTerminalBlock block = Blocks[i]; 
	        if (block.CubeGrid != Me.CubeGrid) {
           break;
        }
        IMySlimBlock slim = block.CubeGrid.GetCubeBlock(block.Position); 
        if (slim.CurrentDamage > 0) {
             DamageList += "\nDamaged: " + block.CustomName;
             CurrentDamage++;
        }
        if (slim.BuildIntegrity < slim.MaxIntegrity) {
             DamageList += "\nIncomplete: " + block.CustomName;
             BuildIntegrity++;
        }
    }
    BlockCount = Blocks.Count;
}

// Battery Check
public float GetBatteryLevel() {
    var batterylist = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batterylist, b => b.CubeGrid == Me.CubeGrid); //Put all Batterys in this list 
    float BatteryMaxStoredTotal = 0f;
    float BatteryCurrStoredTotal  = 0f;
    for (int i=0; i<batterylist.Count; i++) {
        IMyBatteryBlock BatteryBlock = (IMyBatteryBlock)batterylist[i];
        BatteryMaxStoredTotal += BatteryBlock.MaxStoredPower;
        BatteryCurrStoredTotal += BatteryBlock.CurrentStoredPower;
    }
    return BatteryCurrStoredTotal/BatteryMaxStoredTotal;
}

// draw 5 bars of battery, with 7 levels per bar
public String DrawBatteryLevel(float level) {
    String BatteryIcon = "\u2590";
    int batteryInt = Convert.ToInt32(level*5*7);
    for (int i=0; i<5; i++) {
        if (batteryInt >= 7) {
            BatteryIcon += "\ue138";
            batteryInt -= 7;
        } else if (batteryInt > 0) {
            BatteryIcon += (char)(0xe100 + (batteryInt << 3));
            batteryInt = 0;
        } else {
            BatteryIcon += (char)(0xe149);
        }
    }
    BatteryIcon += "\u25A0";
    return BatteryIcon;
}

// Turn the projector on or off
public void SetProjector(bool enable) {
    var projectorlist = new List<IMyProjector>();
    GridTerminalSystem.GetBlocksOfType<IMyProjector>(projectorlist, b => b.CubeGrid == Me.CubeGrid);
    for (int i=0; i<projectorlist.Count; i++) {
        projectorlist[i].Enabled = enable;
    }
}

// Drawing display function
// public static char ColorToChar(byte r, byte g, byte b) {
//    //return (char)(0xe100 + ((int)Math.Round(r / (255.0/7.0)) << 6) + ((int)Math.Round(g / (255.0/7.0)) << 3) + (int)Math.Round(b / (255.0/7.0)));
//    return (char)(0xe100 + (r << 6) + (g << 3) + b); }