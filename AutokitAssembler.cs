/**************
 Autokit Assembler Script ver.1.2 - Maintaining stocks of components on a ship or base

 Author: Survival Ready - steamcommunity.com/profiles/76561199069720721/myworkshopfiles/
  
 Programm Block (PB) paramaters (case insenitive):

 NoARG - start producing

 STOP  - stop producing (clear assembler queue)
 
 ITEM  - show component name at first kitAssembler cell
 
 ANY   - checking the possibility of crafting a component with name ANY
 
 Note: executing a script with parameters always stops production, to restart it you need 
       to execute a script without parameters. NATO_5p56x45mm is depricated and no crafting.
*/

string head   = "= AutoKit Assembler =\n";
string prefix = "Autokit"; // default prefix for blocks
bool   sshort = false;     // true = LCD out short status

// Required items dictionary {component name, amount}
Dictionary<string, int> reqItems = new Dictionary<string, int> {
{ "Large Railgun Sabot;LargeRailgunAmmo;Position0140_LargeRailgunAmmo", 0 },
{ "Small Railgun Sabot;SmallRailgunAmmo;Position0130_SmallRailgunAmmo", 0 },
{ "Artillery Shell;LargeCalibreAmmo;Position0120_LargeCalibreAmmo", 0 },
{ "Assault Cannon Shel;MediumCalibreAmmo;Position0110_MediumCalibreAmmo", 0 },
{ "Autocannon Magazine;AutocannonClip;Position0090_AutocannonClip", 0 },
{ "MR-30E Rifle Magazine;UltimateAutomaticRifleGun_Mag_30rd;Position0070_UltimateAutomaticRifleGun_Mag_30rd", 0 },
{ "MR-50A Rifle Magazine;RapidFireAutomaticRifleGun_Mag_50rd;Position0050_RapidFireAutomaticRifleGun_Mag_50rd", 0 },
{ "MR-8P Rifle Magazine;PreciseAutomaticRifleGun_Mag_5rd;Position0060_PreciseAutomaticRifleGun_Mag_5rd", 0 },
{ "MR-20 Rifle Magazine;AutomaticRifleGun_Mag_20rd;Position0040_AutomaticRifleGun_Mag_20rd", 0 },
{ "S-10E Pistol;ElitePistolMagazine;Position0030_ElitePistolMagazine", 0 },
{ "S-20A Pistol Magazine;FullAutoPistolMagazine;Position0020_FullAutoPistolMagazine", 0 },
{ "S-10 Pistol Magazine;SemiAutoPistolMagazine;Position0010_SemiAutoPistolMagazine", 0 },
{ "Rocket;Missile200mm;Position0100_Missile200mm", 0 },
{ "Gatling Ammo Box;NATO_25x184mm;Position0080_NATO_25x184mmMagazine", 0 },
{ "Explosives;Explosives;ExplosivesComponent", 0 },
{ "Medical Comp;Medical;MedicalComponent", 0 },
{ "Reactor;Reactor;ReactorComponent", 0 },
{ "Detector Comp;Detector;DetectorComponent", 0 },
{ "Gravity Comp;GravityGenerator;GravityGeneratorComponent", 0 },
{ "Thruster Comp;Thrust;ThrustComponent", 0 },
{ "Canvas;Canvas;Position0030_Canvas", 0 },
{ "Superconductor;Superconductor;Superconductor", 0 },
{ "Power Cell;PowerCell;PowerCell", 0 },
{ "Radio Comm Comp;RadioCommunication;RadioCommunicationComponent", 0 },
{ "Display;Display;Display", 0 },
{ "Bulletproof Glass;BulletproofGlass;BulletproofGlass", 0 },
{ "Girder;Girder;GirderComponent", 0 },
{ "Motor;Motor;MotorComponent", 0 },
{ "Large Tube;LargeTube;LargeTube", 0 },
{ "Small Tube;SmallTube;SmallTube", 0 },
{ "Computer;Computer;ComputerComponent", 0 },
{ "Construction Comp;Construction;ConstructionComponent", 0 },
{ "Interior Plate;InteriorPlate;InteriorPlate", 0 },
{ "Metal Grid;MetalGrid;MetalGrid", 0 },
{ "Steel Plate;SteelPlate;SteelPlate", 0 },
};

// --- Dont't change the code below
IMyTextPanel kitPanel = null;

IMyAssembler kitAssembler = null;

Dictionary<string, int> stockItems = new Dictionary<string, int>();

public Program()
{
  if(Me.CustomData.IndexOf("[stock]") == -1) {
    string s = "[stock]\n";
    foreach(KeyValuePair<string, int> req in reqItems) {
      s += defItem(req.Key,0) + " = " + req.Value + "\n";
    }
    Me.CustomData = s;
  } else {  
    Dictionary<string, int> temp = new Dictionary<string, int>(reqItems);
    string[] stock = Me.CustomData.Split('\n'); reqItems.Clear();
    
    foreach(string s in stock) {
      int i = s.IndexOf("="); 
      if(i > 0) { try { reqItems[getItem(s.Substring(0,i-1).Trim(),temp)] = int.Parse(s.Substring(i+1).Trim()); } catch { } }
    }
  }  
  
  Runtime.UpdateFrequency = UpdateFrequency.Once;
}

public void Main(string argument)
{

  string aut = "";

  List<IMyTerminalBlock> list = new List<IMyTerminalBlock>();
  
  // get panel
    if( kitPanel == null ) {
      GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(list, block => block.CubeGrid == Me.CubeGrid && block.CustomName.Contains(prefix));
      if(list.Count > 0) kitPanel = list[0] as IMyTextPanel;
    }
    
  // get assemblers  
    List<IMyAssembler> assemblers = new List<IMyAssembler>();
    
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(list, block => block.CubeGrid == Me.CubeGrid);
    
    for(var i=0; i<list.Count; i++) {
      IMyAssembler ass = list[i] as IMyAssembler;
      
      if(ass.IsWorking && ass.Mode.ToString() == "Assembly" && !ass.CustomName.Contains("!"+prefix)) {
        assemblers.Add(ass);
        
        if(ass.CustomName.Contains(prefix)) {
          kitAssembler = ass;
        }  
      }  
    }

    if(assemblers.Count == 0) { outLCD(aut + "No assemblers found","Error"); return; }  
    
    if(kitAssembler == null) kitAssembler = assemblers[0];

  // get containers
    List<IMyCargoContainer> kitContainer = new List<IMyCargoContainer>();
    
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(list, block => block.CubeGrid == Me.CubeGrid);

    for(var i=0; i<list.Count; i++) {
      IMyCargoContainer con = list[i] as IMyCargoContainer;
      
      if(con.IsWorking) {
        if(con.CustomName.Contains(prefix)) {
          kitContainer.Add(con);
        }
      }
    }
    
    if(kitContainer.Count == 0) { outLCD(aut + "No Autokit containers","Error"); return; }

    aut += "Main: " + kitAssembler.CustomName.Replace(prefix,"")+"\n"+
           "Assemblers: " + assemblers.Count+"\n"+
           "Containers  : " + kitContainer.Count+"\n";

    if(argument.Length > 0) {
      Runtime.UpdateFrequency = UpdateFrequency.None; 
      
      switch(argument.ToLower()) 
      {
        case "stop":
          foreach (var assembler in assemblers) assembler.ClearQueue(); break;
          
        case "item":
          try {
            string[] it = kitAssembler.OutputInventory.GetItemAt(0).ToString().Split(new char[]{'/'}); aut += "Item: " + it[1]; 
          } catch { 
            aut += "No items in " + kitAssembler.CustomName; 
          }
          break;
          
        default:
          aut += argument + ": Producing "+ (kitAssembler.CanUseBlueprint(CreateBlueprint(argument)) ? "": "im") + "possible"; break;
      } 
      
      outLCD(aut,"Stop"); return;
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update100; stockItems.Clear();

  // get items from containers
    Dictionary<string, int> missItems = new Dictionary<string, int>(reqItems);
    
    foreach(var container in kitContainer)
    {
      List<MyInventoryItem> containerItems = new List<MyInventoryItem>();
      
      container.GetInventory(0).GetItems(containerItems);

      foreach(KeyValuePair<string, int> req in reqItems) {
        string key = defItem(req.Key,1);

        foreach(var containerItem in containerItems) {
          if(containerItem.Type.SubtypeId.ToString() == key && req.Value > 0) {
             missItems[req.Key] -= inStock(req.Key,(int)containerItem.Amount);
             if(missItems[req.Key] < 0) { missItems[req.Key] = 0; }
             break;
          }
        }
      }
    }

  // inspect autokit containers
    Dictionary<string, int> missContainerItems = new Dictionary<string, int>(missItems);
    
    int v=0; foreach (KeyValuePair<string, int> miss in missContainerItems) v += miss.Value;
    
    if(v == 0) { outLCD(aut,"Stuff"); return; }
    
  // get items from assemblers inventory & queue
    bool producing = true, queempty = true;
  
    foreach(var assembler in assemblers)
    {
      List<MyInventoryItem> assemblerItems = new List<MyInventoryItem>();
      assembler.GetInventory(1).GetItems(assemblerItems); 
      List<MyProductionItem> queItems = new List<MyProductionItem>();
      assembler.GetQueue(queItems);

      if(queItems.Count > 0) {
         producing = producing && assembler.IsProducing;
         queempty = queempty && assembler.IsQueueEmpty;
      }
      
      foreach(KeyValuePair<string, int> missItem in new Dictionary<string, int>(missItems)) {
        foreach(var que in queItems) {
          if(que.BlueprintId.SubtypeName.ToString() == defItem(missItem.Key,2))
             missItems[missItem.Key] -= (int)que.Amount;
        }
      }

      foreach(var assemblerItem in assemblerItems) {
        foreach(KeyValuePair<string, int> missItem in new Dictionary<string, int>(missItems)) {
          if(assemblerItem.Type.SubtypeId.ToString() == defItem(missItem.Key,1) && reqItems[missItem.Key] > 0)
             missItems[missItem.Key] -= inStock(missItem.Key,(int)assemblerItem.Amount);
        }
      }
    }

  // remove components with 0 value from missItems
    Dictionary<string, int> missClear = new Dictionary<string, int>(missItems);

    missItems = new Dictionary<string, int>();

    foreach (KeyValuePair<string, int> missItem in missClear) {
      if(missItem.Value > 0) missItems[missItem.Key] = missItem.Value;
    }
    
  // add miss items to queue
    foreach (KeyValuePair<string, int> missItem in missItems) {
        kitAssembler.InsertQueueItem(0, CreateBlueprint(defItem(missItem.Key,2)),(double)missItem.Value);
    }    
    
    string assembling = "";
      
    foreach(KeyValuePair<string, int> stockItem in stockItems) {
       int i = reqItems[stockItem.Key] - stockItems[stockItem.Key];
       if(i > 0) { assembling += defItem(stockItem.Key,0)+" (" + i + ")\n"; }
    }
    
  // show status
    string s = "Work";
    if(!producing) s = (!queempty ? "No ingot" : "Idle"); outLCD(aut,s,assembling);
           
  // move items from assemblers to containers
    foreach(var assembler in assemblers)
    {
      InventoryRefresh:
  
      List<MyInventoryItem> assemblerItems = new List<MyInventoryItem>();

      assembler.GetInventory(1).GetItems(assemblerItems);

      for(int i = 0; i < assemblerItems.Count; i++)
      {
        foreach(KeyValuePair<string, int> component in missContainerItems)
        {
          if(assemblerItems[i].Type.SubtypeId.ToString() == defItem(component.Key,1))
          {
            for(int j = 0; j < kitContainer.Count; j++)
            {
              if( assembler.GetInventory(1).TransferItemTo(kitContainer[j].GetInventory(0), i, null, true, assemblerItems[i].Amount)) {
                 missContainerItems[component.Key] -= (int)assemblerItems[i].Amount; break;
              }   
            }
            
            goto InventoryRefresh;
          }
        }
      }
    }
      
}

void outLCD(string t = "", string s = "", string w = "") 
{
  IMyTextSurface surface;
  
  if(kitPanel != null) { 
    surface = kitPanel as IMyTextSurface;
  } else {
    surface = (IMyTextSurface)Me.GetSurface(0);
  }  
  
  surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
  t = $"\n{t}\nStatus: {s}\n"; if(w != "") t += "\nAssembling:\n"+w; 
  
  surface.WriteText(head+(sshort ? s : t)); Echo(head+t);
}  

public int inStock(string k, int i) {
  try {
    stockItems[k] += i;
  } catch {   
    stockItems[k] = i;
  }
  return i;
}  

public string defItem(string n, int i) {
  string[] it = n.Split(new char[]{';'}); if (i > it.Length-1) return it[it.Length-1]; return it[i];
}

public string getItem(string name, Dictionary<string, int> temp) {
  foreach(KeyValuePair<string, int> req in temp) {
    if(req.Key.StartsWith(name+";")) return req.Key;
  }
  return "";
}

public static MyDefinitionId CreateBlueprint(string name)
{
  MyDefinitionId blueprint = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/" + name); return blueprint;
}
