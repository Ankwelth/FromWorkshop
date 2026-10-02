public const int STEEL_PLATE_QUOTA = 750;
public const int CON_COMP_QUOTA = 500;
public const int COMPUTER_QUOTA = 500;
public const int INTERIOR_PLACE_QUOTA = 500;
public const int SMALL_TUBE_QUOTA = 500;
public const int MOTOR_QUOTA = 500;
public const int GIRDER_QUOTA = 100;
public const int LARGE_TUBE_QUOTA = 100;
public const int METAL_GRID_QUOTA = 100;
public const int DISPLAY_QUOTA = 50;
public const int BULLETPROOF_QUOTA = 100;

public const String TARGET_ASSEMBLER_NAME = "Auto-Assembler";

public Program()
{
  Runtime.UpdateFrequency = UpdateFrequency.Update100;
}   

List<IMyCargoContainer> cargos = new List<IMyCargoContainer>();
List<IMyProductionBlock> assemblers = new List<IMyProductionBlock>();


/// <summary>
/// Inserts a number of components into the target Assembler
/// </summary>
/// <param name="component">The desired component type</param>
/// <param name="number">The number of components</param>
void QueueComponents(MyItemType component, decimal number)
{
  Echo("Queuing " + number + " " + component.SubtypeId);
  IMyProductionBlock assembler = GridTerminalSystem.GetBlockWithName(TARGET_ASSEMBLER_NAME) as IMyProductionBlock;

  if (assembler == null)
  {
    Echo("Failed to find assembler");
  }
  else
  {
    MyDefinitionId blueprint= MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/" + ComponentToBlueprint(component));
    if (!assembler.CanUseBlueprint(blueprint))
    {
      Echo("Can't produce this?!?");
    }
    else
    {
      assembler.AddQueueItem(blueprint, number);
    }
  }
}

/// <summary>
/// Converts the component Subtype to the blueprint Subtype.  This is required to insert components into the assembler queue
/// because the blueprint subtype string is different from the "MyItemType" subtype string, in some cases.
/// </summary>
/// <param name="componet">The component to convert from</param>
/// <returns>Blueprint Subtype</returns>
string ComponentToBlueprint(MyItemType component)
{
  if (component.SubtypeId == "Computer")
  {
    return "ComputerComponent";
  }
  else if (component.SubtypeId == "Girder")
  {
    return "GirderComponent";
  }
  else if (component.SubtypeId == "Construction")
  {
    return "ConstructionComponent";
  }
  else if (component.SubtypeId == "Motor")
  {
    return "MotorComponent";
  }
  else
  {
    return component.SubtypeId;
  }
}

/// <summary>
/// Search all cargo containers and assemblers for the current count of the given component.
/// Will also search currently queued components.
/// </summary>
/// <param name="componet">Component to search for</param>
// <returns>Number of components available and queued</returns>
int GetNumberComponents(MyItemType componet)
{
  GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargos);
  GridTerminalSystem.GetBlocksOfType<IMyProductionBlock>(assemblers);

  int component_count = 0;

  List<MyProductionItem> production_queue = new List<MyProductionItem>();
  foreach (IMyProductionBlock assembler in assemblers)
  {
    component_count += assembler.OutputInventory.GetItemAmount(componet).ToIntSafe();
    assembler.GetQueue(production_queue);
    foreach (MyProductionItem queued_item in production_queue)
    {          
      if (queued_item.BlueprintId.ToString().Contains(componet.SubtypeId))
      {
        component_count += queued_item.Amount.ToIntSafe();
      }
    }
  }

  foreach (IMyCargoContainer cargo in cargos)
  {
    component_count += cargo.GetInventory().GetItemAmount(componet).ToIntSafe();
  }

  return component_count;
}

/// <summary>
/// Check the number of components available verses the provided quota, and then add the difference
/// to the assembler queue.
/// </summary>
/// <param name="component">Target component</param>
/// <param name="quota">Target quota</param>
void CheckComponentQuota(String component, int quota)
{
  MyItemType component_type = new MyItemType("MyObjectBuilder_Component", component);
  int num_components = GetNumberComponents(component_type);
  if (num_components < quota)
  {
    QueueComponents(component_type, quota - num_components);
  }
  else
  {
    Echo("Number of " + component + "s: " + num_components);
  }
}

public void Main()
{
  CheckComponentQuota("SteelPlate", STEEL_PLATE_QUOTA);
  CheckComponentQuota("Construction", CON_COMP_QUOTA);
  CheckComponentQuota("Computer", COMPUTER_QUOTA);
  CheckComponentQuota("InteriorPlate", INTERIOR_PLACE_QUOTA);
  CheckComponentQuota("SmallTube", SMALL_TUBE_QUOTA);
  CheckComponentQuota("Motor", MOTOR_QUOTA);
  CheckComponentQuota("Girder", GIRDER_QUOTA);
  CheckComponentQuota("LargeTube", LARGE_TUBE_QUOTA);
  CheckComponentQuota("MetalGrid", METAL_GRID_QUOTA);
  CheckComponentQuota("Display", DISPLAY_QUOTA);
  CheckComponentQuota("BulletproofGlass", BULLETPROOF_QUOTA);
}