const string SHIP_CONNECTOR_NAME = "Ship Connector";
const string BASE_CONNECTOR_NAME = "Base Connector";

const string BASE_ASSEMBLER_NAME = "Base Assembler";
const string ASSEMBLER_NAME = "Assembler";

const string SHIP_CONTAINER_NAME = "Ship Components";
const string BASE_CONTAINER_NAME = "Base Components";

const string IGC_TAG = "RESUPPLY_STATUS";

const int STEEL_PRODUCTION_BATCH = 75000;
const int INTERIOR_PRODUCTION_BATCH = 75000;
const int OTHER_PRODUCTION_BATCH = 25000;

const double STEEL_TARGET = 50000;
const double INTERIOR_TARGET = 50000;
const double OTHER_TARGET = 15000;

const double EPSILON = 0.001;

const int COMPONENT_COUNT = 9;
const int MAX_ERROR_LOGS = 30;

IMyShipConnector shipConnector;
IMyShipConnector baseConnector;
IMyAssembler baseAssembler;

List<IMyAssembler> normalAssemblers =
    new List<IMyAssembler>();

List<IMyCargoContainer> shipContainers =
    new List<IMyCargoContainer>();

List<IMyCargoContainer> baseContainers =
    new List<IMyCargoContainer>();

List<IMyInventory> baseSourceInventories =
    new List<IMyInventory>();

bool[] productionRequested =
    new bool[COMPONENT_COUNT];

bool broadcastSent = false;

List<string> errorLog =
    new List<string>();

ComponentData[] components =
{
    // Name, Inventory Subtype, Blueprint Subtype, Target, Batch

    new ComponentData(
        "Steel Plate",
        "SteelPlate",
        "SteelPlate",
        STEEL_TARGET,
        STEEL_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Interior Plate",
        "InteriorPlate",
        "InteriorPlate",
        INTERIOR_TARGET,
        INTERIOR_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Construction Component",
        "Construction",
        "ConstructionComponent",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Computer",
        "Computer",
        "ComputerComponent",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Girder",
        "Girder",
        "GirderComponent",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Large Steel Tube",
        "LargeTube",
        "LargeTube",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Metal Grid",
        "MetalGrid",
        "MetalGrid",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Motor",
        "Motor",
        "MotorComponent",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    ),

    new ComponentData(
        "Small Steel Tube",
        "SmallTube",
        "SmallTube",
        OTHER_TARGET,
        OTHER_PRODUCTION_BATCH
    )
};


// =====================================================
// PROGRAM
// =====================================================

public Program()
{
    Runtime.UpdateFrequency =
        UpdateFrequency.Update10;

    LoadErrorLog();
    RefreshBlocks();
}


// =====================================================
// MAIN
// =====================================================

public void Main(
    string argument,
    UpdateType updateSource)
{
    try
    {
        string command =
            argument == null
            ? ""
            : argument.Trim().ToUpper();

        if (command == "CLEAR_ERRORS")
        {
            errorLog.Clear();
            SaveErrorLog();

            Echo("ERROR LOG CLEARED");
            return;
        }

        if (command == "RESET")
        {
            ResetSystem();

            Echo("SYSTEM RESET");
            return;
        }

        RefreshIfNeeded();

        Echo("================================");
        Echo("      SHIP RESUPPLY SYSTEM");
        Echo("================================");
        Echo("");

        if (shipConnector == null)
        {
            Echo("ERROR: Ship Connector not found");

            LogSimpleError(
                "BLOCK CHECK",
                "Ship Connector not found"
            );

            return;
        }

        if (baseConnector == null)
        {
            Echo("ERROR: Base Connector not found");

            LogSimpleError(
                "BLOCK CHECK",
                "Base Connector not found"
            );

            return;
        }

        if (baseAssembler == null)
        {
            Echo("ERROR: Base Assembler not found");

            LogSimpleError(
                "BLOCK CHECK",
                "Base Assembler not found"
            );

            return;
        }

        bool docked =
            shipConnector.Status ==
            MyShipConnectorStatus.Connected;

        Echo(
            docked
            ? "SHIP: DOCKED"
            : "SHIP: NOT DOCKED"
        );

        Echo(
            "Ship Containers: " +
            shipContainers.Count
        );

        Echo(
            "Base Containers: " +
            baseContainers.Count
        );

        Echo(
            "Normal Assemblers: " +
            normalAssemblers.Count
        );

        Echo("");

        // Production is checked continuously.
        CheckProduction();

        // =================================================
        // COMPONENT STATUS
        // =================================================

        for (int i = 0;
             i < components.Length;
             i++)
        {
            ShowComponentStatus(
                components[i]
            );
        }

        // =================================================
        // ONLY TRANSFER WHEN DOCKED
        // =================================================

        if (!docked)
        {
            Echo("");
            Echo("WAITING FOR SHIP TO DOCK");

            broadcastSent = false;

            return;
        }

        // =================================================
        // RESUPPLY
        // =================================================

        for (int i = 0;
             i < components.Length;
             i++)
        {
            try
            {
                ResupplyComponent(
                    components[i]
                );
            }
            catch (Exception ex)
            {
                LogError(
                    "RESUPPLY - " +
                    components[i].Name,
                    ex
                );
            }
        }

        // =================================================
        // FINAL TARGET CHECK
        // =================================================

        bool allAtTarget =
            AllComponentsAtTarget();

        Echo("");
        Echo("================================");

        if (allAtTarget)
        {
            Echo("ALL COMPONENTS AT TARGET");

            if (!broadcastSent)
            {
                IGC.SendBroadcastMessage(
                    IGC_TAG,
                    "SHIP REFILLED - ALL COMPONENTS AT TARGET"
                );

                broadcastSent = true;
            }
        }
        else
        {
            Echo("SHIP STILL NEEDS COMPONENTS");

            broadcastSent = false;
        }
    }
    catch (Exception ex)
    {
        LogError(
            "MAIN",
            ex
        );

        Echo("");
        Echo("================================");
        Echo("SCRIPT ERROR");
        Echo(ex.GetType().Name);
        Echo(ex.Message);
        Echo("ERROR SAVED TO CUSTOM DATA");
        Echo("================================");
    }
}


// =====================================================
// REFRESH
// =====================================================

void RefreshIfNeeded()
{
    if (shipConnector == null ||
        baseConnector == null ||
        baseAssembler == null)
    {
        RefreshBlocks();
        return;
    }

    if (shipContainers.Count == 0 ||
        baseContainers.Count == 0)
    {
        RefreshBlocks();
    }
}


void RefreshBlocks()
{
    shipContainers.Clear();
    baseContainers.Clear();
    normalAssemblers.Clear();
    baseSourceInventories.Clear();

    shipConnector =
        GridTerminalSystem.GetBlockWithName(
            SHIP_CONNECTOR_NAME
        ) as IMyShipConnector;

    baseConnector =
        GridTerminalSystem.GetBlockWithName(
            BASE_CONNECTOR_NAME
        ) as IMyShipConnector;

    baseAssembler =
        GridTerminalSystem.GetBlockWithName(
            BASE_ASSEMBLER_NAME
        ) as IMyAssembler;


    // =================================================
    // CARGO CONTAINERS
    // =================================================

    List<IMyCargoContainer> allCargo =
        new List<IMyCargoContainer>();

    GridTerminalSystem.GetBlocksOfType(
        allCargo
    );

    for (int i = 0;
         i < allCargo.Count;
         i++)
    {
        IMyCargoContainer container =
            allCargo[i];

        if (container == null)
            continue;

        if (container.CustomName ==
            SHIP_CONTAINER_NAME)
        {
            shipContainers.Add(
                container
            );
        }

        if (container.CustomName ==
            BASE_CONTAINER_NAME)
        {
            baseContainers.Add(
                container
            );
        }
    }


    // =================================================
    // NORMAL ASSEMBLERS
    // =================================================

    List<IMyAssembler> allAssemblers =
        new List<IMyAssembler>();

    GridTerminalSystem.GetBlocksOfType(
        allAssemblers
    );

    for (int i = 0;
         i < allAssemblers.Count;
         i++)
    {
        IMyAssembler assembler =
            allAssemblers[i];

        if (assembler == null)
            continue;

        if (assembler.CustomName ==
            ASSEMBLER_NAME)
        {
            normalAssemblers.Add(
                assembler
            );
        }
    }


    // =================================================
    // BASE CARGO INVENTORIES
    // =================================================

    for (int i = 0;
         i < baseContainers.Count;
         i++)
    {
        IMyCargoContainer container =
            baseContainers[i];

        if (container == null)
            continue;

        IMyInventory inventory =
            container.GetInventory();

        if (inventory != null)
        {
            baseSourceInventories.Add(
                inventory
            );
        }
    }


    // =================================================
    // NORMAL ASSEMBLER OUTPUT INVENTORIES
    // =================================================

    for (int i = 0;
         i < normalAssemblers.Count;
         i++)
    {
        IMyAssembler assembler =
            normalAssemblers[i];

        if (assembler == null)
            continue;

        IMyInventory output =
            assembler.GetInventory(1);

        if (output != null)
        {
            baseSourceInventories.Add(
                output
            );
        }
    }
}


// =====================================================
// PRODUCTION
// =====================================================

void CheckProduction()
{
    if (baseAssembler == null)
    {
        LogSimpleError(
            "PRODUCTION",
            "Base Assembler is NULL"
        );

        return;
    }

    for (int i = 0;
         i < components.Length;
         i++)
    {
        ComponentData component =
            components[i];

        try
        {
            // -------------------------------------------------
            // Check storage + normal assembler outputs
            // -------------------------------------------------

            double available =
                GetComponentAmountFromSources(
                    component.Subtype
                );

            // -------------------------------------------------
            // If already available, no production needed
            // -------------------------------------------------

            if (available > EPSILON)
            {
                productionRequested[i] = false;
                continue;
            }

            // -------------------------------------------------
            // Check Base Assembler queue
            // -------------------------------------------------

            double queued =
                GetQueuedAmount(
                    component.Blueprint
                );

            if (queued > EPSILON)
            {
                productionRequested[i] = false;
                continue;
            }

            // -------------------------------------------------
            // Prevent duplicate queue requests
            // -------------------------------------------------

            if (productionRequested[i])
                continue;

            // -------------------------------------------------
            // Queue production
            // -------------------------------------------------

            QueueProduction(
                component
            );

            productionRequested[i] = true;
        }
        catch (Exception ex)
        {
            LogError(
                "PRODUCTION - " +
                component.Name,
                ex
            );

            productionRequested[i] = true;
        }
    }
}


// =====================================================
// QUEUE PRODUCTION
// =====================================================

void QueueProduction(
    ComponentData component)
{
    if (component == null)
    {
        throw new Exception(
            "ComponentData is NULL"
        );
    }

    if (baseAssembler == null)
    {
        throw new Exception(
            "Base Assembler is NULL"
        );
    }

    if (!baseAssembler.IsFunctional)
    {
        throw new Exception(
            "Base Assembler is not functional"
        );
    }

    if (component.ProductionBatch <= 0)
    {
        throw new Exception(
            "Production batch is <= 0"
        );
    }

    string blueprintPath =
        "MyObjectBuilder_BlueprintDefinition/" +
        component.Blueprint;

    MyDefinitionId blueprint;

    try
    {
        blueprint =
            MyDefinitionId.Parse(
                blueprintPath
            );
    }
    catch (Exception ex)
    {
        throw new Exception(
            "Blueprint Parse failed: " +
            blueprintPath +
            " | " +
            ex.Message
        );
    }

    // -------------------------------------------------
    // Add queue item
    // -------------------------------------------------

    try
    {
        baseAssembler.AddQueueItem(
            blueprint,
            (MyFixedPoint)
                component.ProductionBatch
        );
    }
    catch (Exception ex)
    {
        throw new Exception(
            "AddQueueItem failed: " +
            blueprintPath +
            " | Batch=" +
            component.ProductionBatch +
            " | " +
            ex.Message
        );
    }
}


// =====================================================
// QUEUE CHECK
// =====================================================

double GetQueuedAmount(
    string blueprintSubtype)
{
    if (baseAssembler == null)
        return 0;

    List<MyProductionItem> queue =
        new List<MyProductionItem>();

    try
    {
        baseAssembler.GetQueue(queue);
    }
    catch (Exception ex)
    {
        throw new Exception(
            "GetQueue failed: " +
            ex.Message
        );
    }

    MyDefinitionId targetBlueprint;

    try
    {
        targetBlueprint =
            MyDefinitionId.Parse(
                "MyObjectBuilder_BlueprintDefinition/" +
                blueprintSubtype
            );
    }
    catch (Exception ex)
    {
        throw new Exception(
            "Queue blueprint parse failed: " +
            blueprintSubtype +
            " | " +
            ex.Message
        );
    }

    double total = 0;

    for (int i = 0;
         i < queue.Count;
         i++)
    {
        if (queue[i].BlueprintId ==
            targetBlueprint)
        {
            total +=
                (double)queue[i].Amount;
        }
    }

    return total;
}


// =====================================================
// RESUPPLY COMPONENT
// =====================================================

void ResupplyComponent(
    ComponentData component)
{
    double current =
        GetComponentAmount(
            shipContainers,
            component.Subtype
        );

    // -------------------------------------------------
    // SHIP HAS TOO LITTLE
    // -------------------------------------------------

    if (current <
        component.Target -
        EPSILON)
    {
        double needed =
            component.Target -
            current;

        PullFromBaseSources(
            component.Subtype,
            needed
        );
    }

    // -------------------------------------------------
    // SHIP HAS TOO MUCH
    // -------------------------------------------------

    else if (current >
             component.Target +
             EPSILON)
    {
        double excess =
            current -
            component.Target;

        ReturnExcessToBase(
            component.Subtype,
            excess
        );
    }
}


// =====================================================
// PULL FROM BASE
// =====================================================

void PullFromBaseSources(
    string subtype,
    double amount)
{
    double remaining =
        amount;

    for (int i = 0;
         i < baseSourceInventories.Count &&
         remaining > EPSILON;
         i++)
    {
        IMyInventory source =
            baseSourceInventories[i];

        if (source == null)
            continue;

        List<MyInventoryItem> items =
            new List<MyInventoryItem>();

        source.GetItems(items);

        for (int j = 0;
             j < items.Count &&
             remaining > EPSILON;
             j++)
        {
            MyInventoryItem item =
                items[j];

            if (item.Type.TypeId !=
                "MyObjectBuilder_Component")
            {
                continue;
            }

            if (item.Type.SubtypeId !=
                subtype)
            {
                continue;
            }

            double available =
                (double)item.Amount;

            double transferAmount =
                Math.Min(
                    available,
                    remaining
                );

            if (TransferToShip(
                source,
                j,
                transferAmount))
            {
                remaining -=
                    transferAmount;
            }
        }
    }
}


// =====================================================
// TRANSFER TO SHIP
// =====================================================

bool TransferToShip(
    IMyInventory source,
    int sourceIndex,
    double amount)
{
    if (source == null)
        return false;

    for (int i = 0;
         i < shipContainers.Count;
         i++)
    {
        IMyCargoContainer container =
            shipContainers[i];

        if (container == null)
            continue;

        IMyInventory destination =
            container.GetInventory();

        if (destination == null)
            continue;

        if (source.TransferItemTo(
            destination,
            sourceIndex,
            null,
            true,
            (MyFixedPoint)amount))
        {
            return true;
        }
    }

    return false;
}


// =====================================================
// RETURN EXCESS
// =====================================================

void ReturnExcessToBase(
    string subtype,
    double amount)
{
    double remaining =
        amount;

    for (int i = 0;
         i < shipContainers.Count &&
         remaining > EPSILON;
         i++)
    {
        IMyCargoContainer container =
            shipContainers[i];

        if (container == null)
            continue;

        IMyInventory source =
            container.GetInventory();

        if (source == null)
            continue;

        List<MyInventoryItem> items =
            new List<MyInventoryItem>();

        source.GetItems(items);

        for (int j = items.Count - 1;
             j >= 0 &&
             remaining > EPSILON;
             j--)
        {
            MyInventoryItem item =
                items[j];

            if (item.Type.TypeId !=
                "MyObjectBuilder_Component")
            {
                continue;
            }

            if (item.Type.SubtypeId !=
                subtype)
            {
                continue;
            }

            double available =
                (double)item.Amount;

            double transferAmount =
                Math.Min(
                    available,
                    remaining
                );

            if (TransferToBase(
                source,
                j,
                transferAmount))
            {
                remaining -=
                    transferAmount;
            }
        }
    }
}


// =====================================================
// TRANSFER TO BASE
// =====================================================

bool TransferToBase(
    IMyInventory source,
    int sourceIndex,
    double amount)
{
    if (source == null)
        return false;

    for (int i = 0;
         i < baseContainers.Count;
         i++)
    {
        IMyCargoContainer container =
            baseContainers[i];

        if (container == null)
            continue;

        IMyInventory destination =
            container.GetInventory();

        if (destination == null)
            continue;

        if (source.TransferItemTo(
            destination,
            sourceIndex,
            null,
            true,
            (MyFixedPoint)amount))
        {
            return true;
        }
    }

    return false;
}


// =====================================================
// GET COMPONENT AMOUNT
// =====================================================

double GetComponentAmount(
    List<IMyCargoContainer> containers,
    string subtype)
{
    double total = 0;

    for (int i = 0;
         i < containers.Count;
         i++)
    {
        IMyCargoContainer container =
            containers[i];

        if (container == null)
            continue;

        IMyInventory inventory =
            container.GetInventory();

        if (inventory == null)
            continue;

        List<MyInventoryItem> items =
            new List<MyInventoryItem>();

        inventory.GetItems(items);

        for (int j = 0;
             j < items.Count;
             j++)
        {
            MyInventoryItem item =
                items[j];

            if (item.Type.TypeId ==
                "MyObjectBuilder_Component" &&
                item.Type.SubtypeId ==
                subtype)
            {
                total +=
                    (double)item.Amount;
            }
        }
    }

    return total;
}


// =====================================================
// GET BASE + ASSEMBLER OUTPUT AMOUNT
// =====================================================

double GetComponentAmountFromSources(
    string subtype)
{
    double total = 0;

    for (int i = 0;
         i < baseSourceInventories.Count;
         i++)
    {
        IMyInventory inventory =
            baseSourceInventories[i];

        if (inventory == null)
            continue;

        List<MyInventoryItem> items =
            new List<MyInventoryItem>();

        inventory.GetItems(items);

        for (int j = 0;
             j < items.Count;
             j++)
        {
            MyInventoryItem item =
                items[j];

            if (item.Type.TypeId ==
                "MyObjectBuilder_Component" &&
                item.Type.SubtypeId ==
                subtype)
            {
                total +=
                    (double)item.Amount;
            }
        }
    }

    return total;
}


// =====================================================
// DISPLAY STATUS
// =====================================================

void ShowComponentStatus(
    ComponentData component)
{
    try
    {
        double ship =
            GetComponentAmount(
                shipContainers,
                component.Subtype
            );

        double baseAmount =
            GetComponentAmountFromSources(
                component.Subtype
            );

        double queued =
            GetQueuedAmount(
                component.Blueprint
            );

        Echo(
            component.Name +
            ": " +
            Math.Round(ship, 0) +
            "/" +
            component.Target
        );

        if (queued > EPSILON)
        {
            Echo(
                "  Queue: " +
                Math.Round(queued, 0)
            );
        }
        else if (baseAmount > EPSILON)
        {
            Echo(
                "  Base: " +
                Math.Round(baseAmount, 0)
            );
        }
        else
        {
            Echo("  Production: needed");
        }
    }
    catch (Exception ex)
    {
        LogError(
            "STATUS - " +
            component.Name,
            ex
        );

        Echo(
            component.Name +
            ": ERROR"
        );
    }
}


// =====================================================
// FINAL TARGET CHECK
// =====================================================

bool AllComponentsAtTarget()
{
    for (int i = 0;
         i < components.Length;
         i++)
    {
        double amount =
            GetComponentAmount(
                shipContainers,
                components[i].Subtype
            );

        if (Math.Abs(
            amount -
            components[i].Target
        ) > EPSILON)
        {
            return false;
        }
    }

    return true;
}


// =====================================================
// RESET
// =====================================================

void ResetSystem()
{
    broadcastSent = false;

    for (int i = 0;
         i < productionRequested.Length;
         i++)
    {
        productionRequested[i] = false;
    }

    RefreshBlocks();
}


// =====================================================
// ERROR LOGGING
// =====================================================

void LogError(
    string location,
    Exception ex)
{
    string timestamp =
        DateTime.Now.ToString(
            "yyyy-MM-dd HH:mm:ss"
        );

    string message =
        "[" +
        timestamp +
        "] " +
        location +
        " | " +
        ex.GetType().Name +
        " | " +
        ex.Message;

    errorLog.Add(message);

    while (errorLog.Count >
           MAX_ERROR_LOGS)
    {
        errorLog.RemoveAt(0);
    }

    SaveErrorLog();
}


void LogSimpleError(
    string location,
    string message)
{
    string timestamp =
        DateTime.Now.ToString(
            "yyyy-MM-dd HH:mm:ss"
        );

    errorLog.Add(
        "[" +
        timestamp +
        "] " +
        location +
        " | " +
        message
    );

    while (errorLog.Count >
           MAX_ERROR_LOGS)
    {
        errorLog.RemoveAt(0);
    }

    SaveErrorLog();
}


void SaveErrorLog()
{
    StringBuilder output =
        new StringBuilder();

    output.AppendLine(
        "================================"
    );

    output.AppendLine(
        "SHIP RESUPPLY SYSTEM ERROR LOG"
    );

    output.AppendLine(
        "================================"
    );

    output.AppendLine("");

    if (errorLog.Count == 0)
    {
        output.AppendLine(
            "No errors recorded."
        );
    }
    else
    {
        for (int i = 0;
             i < errorLog.Count;
             i++)
        {
            output.AppendLine(
                errorLog[i]
            );
        }
    }

    output.AppendLine("");

    output.AppendLine(
        "Run CLEAR_ERRORS to erase the log."
    );

    Me.CustomData =
        output.ToString();
}


void LoadErrorLog()
{
    errorLog.Clear();

    string data =
        Me.CustomData;

    if (string.IsNullOrWhiteSpace(data))
        return;

    string[] lines =
        data.Split(
            new char[] { '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

    for (int i = 0;
         i < lines.Length;
         i++)
    {
        string line =
            lines[i].Trim();

        if (line.StartsWith("["))
        {
            errorLog.Add(line);
        }
    }

    while (errorLog.Count >
           MAX_ERROR_LOGS)
    {
        errorLog.RemoveAt(0);
    }
}


// =====================================================
// COMPONENT CLASS
// =====================================================

public class ComponentData
{
    public string Name;
    public string Subtype;
    public string Blueprint;
    public double Target;
    public int ProductionBatch;

    public ComponentData(
        string name,
        string subtype,
        string blueprint,
        double target,
        int productionBatch)
    {
        Name = name;
        Subtype = subtype;
        Blueprint = blueprint;
        Target = target;
        ProductionBatch = productionBatch;
    }
}