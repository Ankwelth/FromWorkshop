// --- KONFIGURATION ---
const string TAG_COMPONENTS = "components";
const string TAG_ORE = "ore";
const string TAG_INGOTS = "ingots";
const string TAG_AMMO = "ammo";
const string TAG_TOOLS = "tools";
const string LCD_NAME_TAG = "LCD"; 
const int ZEILEN_PRO_SEITE = 24; 

// --- GLOBALE LISTEN (GC-SCHUTZ) ---
readonly List<IMyTerminalBlock> sourceBlocks = new List<IMyTerminalBlock>();
readonly List<IMyCargoContainer> targetContainers = new List<IMyCargoContainer>();
readonly List<IMyTextPanel> lcdPanels = new List<IMyTextPanel>();
readonly List<MyInventoryItem> tempItems = new List<MyInventoryItem>();
readonly List<string> formattedInventoryLines = new List<string>();

// --- DOUBLE-BUFFERING STRUKTUREN (LÖST DAS PROBLEM LEERER SEITEN) ---
readonly Dictionary<string, double> inventoryLiveCalculation = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
readonly Dictionary<string, double> inventoryTotalsSnapshot = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
readonly System.Text.StringBuilder statusBuilder = new System.Text.StringBuilder();

// --- STATE MACHINE & PAGING VARIABLES ---
int blockScanTick = 0;
int currentBlockIndex = 0;
int itemsMovedThisRun = 0;
int totalItemsMovedLastCycle = 0;

int globalPageTicker = 0;
int virtualPageTrigger = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    InitializeBlocks();
}

void InitializeBlocks()
{
    sourceBlocks.Clear();
    targetContainers.Clear();
    lcdPanels.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, b => {
        if (b == null || !b.IsFunctional) return false;
        
        if (b.HasInventory)
        {
            sourceBlocks.Add(b);
            if (b is IMyCargoContainer) targetContainers.Add(b as IMyCargoContainer);
        }
        
        if (b is IMyTextPanel && b.CustomName.IndexOf(LCD_NAME_TAG, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            var lcd = b as IMyTextPanel;
            lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
            lcdPanels.Add(lcd);
        }
        return false; 
    });

    currentBlockIndex = 0;
    inventoryLiveCalculation.Clear();
}
public void Main(string argument, UpdateType updateSource)
{
    const int SEKUNDEN_PRO_SEITE = 20; 
    int ticksFuerSeite = SEKUNDEN_PRO_SEITE * 6;

    blockScanTick++;
    globalPageTicker++;

    if (globalPageTicker >= ticksFuerSeite)
    {
        globalPageTicker = 0;
        virtualPageTrigger++; 
        RenderLCDs(); // Nutzt jetzt den sicheren Snapshot, keine leeren Seiten mehr!
    }

    if (blockScanTick >= 600 || sourceBlocks.Count == 0)
    {
        InitializeBlocks();
        blockScanTick = 0;
        return;
    }

    if (sourceBlocks.Count == 0) return;

    // ZYKLUSABSCHLUSS: Wenn alle Blöcke fertig gescannt wurden
    if (currentBlockIndex >= sourceBlocks.Count)
    {
        totalItemsMovedLastCycle = itemsMovedThisRun;
        itemsMovedThisRun = 0;
        currentBlockIndex = 0;

        // FEHLERBEHEBUNG: Wir überschreiben den alten Snapshot ERST JETZT, wo alle Daten vollständig sind!
        inventoryTotalsSnapshot.Clear();
        foreach (var kvp in inventoryLiveCalculation)
        {
            inventoryTotalsSnapshot.Add(kvp.Key, kvp.Value);
        }

        RenderLCDs();
        
        // Bereite die Live-Berechnung für den nächsten, unsichtbaren Hintergrund-Scan vor
        inventoryLiveCalculation.Clear();
        return;
    }

    IMyTerminalBlock srcBlock = sourceBlocks[currentBlockIndex];
    currentBlockIndex++;

    if (srcBlock == null || !srcBlock.IsFunctional || !srcBlock.HasPlayerAccess(Me.OwnerId)) return;
    if (srcBlock is IMyLargeTurretBase || srcBlock is IMySmallMissileLauncher || srcBlock is IMySmallGatlingGun) return;

    bool isCargo = srcBlock is IMyCargoContainer;

    for (int i = 0; i < srcBlock.InventoryCount; i++)
    {
        IMyInventory srcInv = srcBlock.GetInventory(i);
        if (srcInv == null) continue;

        // Daten wandern in das unsichtbare Hintergrund-Dictionary (LiveCalculation)
        tempItems.Clear();
        srcInv.GetItems(tempItems);
        foreach (var item in tempItems)
        {
            string fullTypeKey = item.Type.ToString();
            double amount = (double)item.Amount;
            
            double currentAmount;
            if (inventoryLiveCalculation.TryGetValue(fullTypeKey, out currentAmount)) inventoryLiveCalculation[fullTypeKey] = currentAmount + amount;
            else inventoryLiveCalculation[fullTypeKey] = amount;
        }

        if (srcInv.ItemCount == 0) continue;

        bool isRefinery = srcBlock is IMyRefinery;
        bool isAssembler = srcBlock is IMyAssembler;
        bool isReactor = srcBlock is IMyReactor;
        bool isInputInventory = (i == 0);

        tempItems.Clear();
        srcInv.GetItems(tempItems);
        int movedThisBlock = 0;

        for (int j = tempItems.Count - 1; j >= 0; j--)
        {
            if (movedThisBlock >= 10) break; 
            if (j >= srcInv.ItemCount) continue;

            MyInventoryItem item = tempItems[j];
            string typeId = item.Type.TypeId.ToString();
            IMyCargoContainer targetContainer = null;

            if (typeId.EndsWith("Ore"))
            {
                if (isRefinery && isInputInventory) continue;
                targetContainer = FindContainer(TAG_ORE);
            }
            else if (typeId.EndsWith("Ingot"))
            {
                if (isReactor) continue;
                if (isAssembler && isInputInventory) continue;
                targetContainer = FindContainer(TAG_INGOTS);
            }
            else if (typeId.EndsWith("Component")) targetContainer = FindContainer(TAG_COMPONENTS);
            else if (typeId.EndsWith("AmmoMagazine")) targetContainer = FindContainer(TAG_AMMO);
            else targetContainer = FindContainer(TAG_TOOLS);

            if (targetContainer == null) continue;
            if (isCargo && srcBlock.EntityId == targetContainer.EntityId) continue;

            IMyInventory destInv = targetContainer.GetInventory(0);
            if (destInv == null || destInv.IsFull) continue;

            if (srcInv.TransferItemTo(destInv, j, null, true, null))
            {
                movedThisBlock++;
                itemsMovedThisRun++;
            }
        }
    }
}
void RenderLCDs()
{
    foreach (var lcd in lcdPanels)
    {
        if (lcd == null || !lcd.IsFunctional) continue;

        string displayName = lcd.CustomName.ToLower();
        formattedInventoryLines.Clear();

        // Greift AUSSCHLIESSLICH auf den vollständigen Snapshot zu
        foreach (var kvp in inventoryTotalsSnapshot)
        {
            string itemTypePath = kvp.Key; 
            int slashIndex = itemTypePath.IndexOf('/');
            if (slashIndex <= 0) continue;

            string typeId = itemTypePath.Substring(0, slashIndex);
            string subtypeId = itemTypePath.Substring(slashIndex + 1);

            bool match = false;
            if (displayName.Contains("components") && typeId.EndsWith("Component")) match = true;
            else if (displayName.Contains("ore") && typeId.EndsWith("Ore")) match = true;
            else if (displayName.Contains("ingots") && typeId.EndsWith("Ingot")) match = true;
            else if (displayName.Contains("ammo") && typeId.EndsWith("AmmoMagazine")) match = true;
            else if (displayName.Contains("tools") && !typeId.EndsWith("Component") && !typeId.EndsWith("Ore") && !typeId.EndsWith("Ingot") && !typeId.EndsWith("AmmoMagazine")) match = true;
            else if (!displayName.Contains("components") && !displayName.Contains("ore") && !displayName.Contains("ingots") && !displayName.Contains("ammo") && !displayName.Contains("tools")) match = true;

            if (match)
            {
                formattedInventoryLines.Add($"- {subtypeId}: {kvp.Value:N0}");
            }
        }

        int localMaxPages = (formattedInventoryLines.Count - 1) / ZEILEN_PRO_SEITE;
        if (localMaxPages < 0) localMaxPages = 0;
        
        int localCurrentPage = 0;
        if (localMaxPages > 0)
        {
            localCurrentPage = virtualPageTrigger % (localMaxPages + 1);
        }

        statusBuilder.Clear();
        statusBuilder.AppendLine($"=== {lcd.CustomName} ===");
        statusBuilder.AppendLine($"Gescannte Blöcke: {sourceBlocks.Count}");
        statusBuilder.AppendLine($"Items verschoben: {totalItemsMovedLastCycle}");
        statusBuilder.AppendLine($"=== Seite {localCurrentPage + 1}/{localMaxPages + 1} ===");

        if (formattedInventoryLines.Count == 0)
        {
            statusBuilder.AppendLine("(Keine Gegenstände)");
        }
        else
        {
            int startZeile = localCurrentPage * ZEILEN_PRO_SEITE;
            int endZeile = Math.Min(startZeile + ZEILEN_PRO_SEITE, formattedInventoryLines.Count);

            for (int i = startZeile; i < endZeile; i++)
            {
                statusBuilder.AppendLine(formattedInventoryLines[i]);
            }
        }

        lcd.WriteText(statusBuilder.ToString(), false);
    }

    Echo($"Sortiersystem aktiv.\nLCDs gesteuert: {lcdPanels.Count}\nZuletzt verschoben: {totalItemsMovedLastCycle}");
}

IMyCargoContainer FindContainer(string tag)
{
    foreach (var container in targetContainers)
    {
        if (container == null || !container.IsFunctional || !container.HasPlayerAccess(Me.OwnerId)) continue;
        if (container.CustomName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) < 0) continue;

        IMyInventory inv = container.GetInventory(0);
        if (inv == null || inv.IsFull) continue;
        return container;
    }
    return null;
}
