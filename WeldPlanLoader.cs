// Space Engineers Programmable Block script
// Weld Plan Loader
//
// The PB itself is the job list carrier: paste the BoM into THIS PB's CustomData.
//
// Expected CustomData format:
//   WELD PLANNER BoM
//   Construction: 6
//   SmallTube: 1
//   Motor: 2
//   Computer: 5
//   SteelPlate: 4
//   Construction: 12
//   ...
//
// Rules:
//   * First line must be exactly "WELD PLANNER BoM" (ignoring surrounding whitespace).
//   * Every following non-empty line is one sequential component task.
//   * Tasks are NOT merged. Duplicate component names on different lines stay separate.
//   * Only the first task is processed. Later tasks are untouched until the first is complete.
//   * Target = cargo containers on the PB's current construct (same mechanical construct).
//     A ship/base connected only by a connector is NOT a target.
//   * Sources = inventories reachable through this GridTerminalSystem and conveyor-connected
//     to at least one target. Character inventory is never included by GridTerminalSystem.
//   * Existing material in target containers does NOT satisfy a task. Only newly transferred
//     material reduces the task amount.
//   * After every successful transfer batch, CustomData is rewritten with the remaining job.
//   * When all tasks are complete, CustomData is cleared and PB reports that no tasks remain.
//   * If no target container can accept the current component, PB reports that space is full.
//
// Performance:
//   * PB wakes every Update10 but performs the inventory scan/work about every 0.5 seconds.

const string HEADER = "WELD PLANNER BoM";
const int UPDATE_TICKS = 30; // 30 simulation ticks ~= 0.5 s at 60 ticks/s

int m_tickCounter = UPDATE_TICKS;
string m_console = "";
string m_lastWaitMessage = null;
bool m_hadTasks = false;
bool m_noTasksShown = false;

class JobLine
{
    public string Name;
    public int Amount;
}

class MoveResult
{
    public int Moved;
    public bool TargetHasSpace;
    public bool SourceHasItem;
    public bool ConnectedSourceExists;
}

List<IMyTerminalBlock> m_allBlocks = new List<IMyTerminalBlock>();
List<IMyCargoContainer> m_targetContainers = new List<IMyCargoContainer>();
List<IMyInventory> m_sourceInventories = new List<IMyInventory>();
List<MyInventoryItem> m_items = new List<MyInventoryItem>();
List<JobLine> m_jobs = new List<JobLine>();
List<JobLine> m_remainingJobs = new List<JobLine>();
List<long> m_targetBlockIds = new List<long>();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    bool runNow = (updateSource & UpdateType.Once) != 0;

    if (!runNow && (updateSource & UpdateType.Update10) != 0)
    {
        m_tickCounter -= 10;
        if (m_tickCounter > 0)
            return;

        m_tickCounter = UPDATE_TICKS;
    }

    string data = Me.CustomData ?? "";

    if (string.IsNullOrWhiteSpace(data))
    {
        ShowNoTasks();
        return;
    }

    string parseError;
    if (!TryParseCustomData(data, m_jobs, out parseError))
    {
        BeginTaskScreenIfNeeded();
        AppendConsoleOnce("Weld Planner parse error: " + parseError, "PARSE:" + parseError);
        return;
    }

    if (m_jobs.Count == 0)
    {
        Me.CustomData = "";
        ShowNoTasks();
        return;
    }

    BeginTaskScreenIfNeeded();

    CollectTargetContainers();
    if (m_targetContainers.Count == 0)
    {
        AppendConsoleOnce("No target cargo containers; will retry.", "NO_TARGET_CONTAINERS");
        return;
    }

    CollectSourceInventories();

    JobLine current = m_jobs[0];
    MoveResult result = MoveOneTask(current);

    if (result.Moved > 0)
    {
        current.Amount -= result.Moved;
        if (current.Amount < 0)
            current.Amount = 0;

        BuildRemainingJobs(m_jobs);

        Me.CustomData = BuildCustomData(m_remainingJobs);

        AppendConsole(
            "Loading " + current.Name + " " + result.Moved + ": DONE");

        if (m_remainingJobs.Count == 0)
        {
            ShowNoTasks();
            return;
        }

        // More work remains. Keep the existing console and continue with the next task.
        return;
    }

    // No items moved this iteration. Keep the current task intact.
    if (!result.ConnectedSourceExists)
    {
        AppendWaitMessage(
            "Loading " + current.Name + " " + current.Amount + ": no connected source; will retry.",
            "NO_SOURCE|" + current.Name + "|" + current.Amount);
        return;
    }

    if (!result.SourceHasItem)
    {
        AppendWaitMessage(
            "Loading " + current.Name + " " + current.Amount + ": component unavailable; will retry.",
            "NO_ITEM|" + current.Name + "|" + current.Amount);
        return;
    }

    if (!result.TargetHasSpace)
    {
        AppendWaitMessage(
            "Loading " + current.Name + " " + current.Amount + ": no space; will retry.",
            "NO_SPACE|" + current.Name + "|" + current.Amount);
        return;
    }

    AppendWaitMessage(
        "Loading " + current.Name + " " + current.Amount + ": transfer failed; will retry.",
        "TRANSFER_FAILED|" + current.Name + "|" + current.Amount);
}

void ShowNoTasks()
{
    m_hadTasks = false;

    if (m_noTasksShown)
        return;

    m_noTasksShown = true;
    m_console = "No tasks.";
    m_lastWaitMessage = null;
    Echo(m_console);
}

void BeginTaskScreenIfNeeded()
{
    if (m_hadTasks)
        return;

    m_hadTasks = true;
    m_noTasksShown = false;
    m_console = "WELD PLANNER BoM Loading:";
    m_lastWaitMessage = null;
    Echo(m_console);
}

void AppendConsole(string line)
{
    if (string.IsNullOrEmpty(line))
        return;

    if (m_console.Length == 0)
        m_console = line;
    else
        m_console += "\n" + line;

    Echo(m_console);
    m_lastWaitMessage = null;
}

void AppendConsoleOnce(string line, string key)
{
    if (string.Equals(key, m_lastWaitMessage, StringComparison.Ordinal))
        return;

    m_lastWaitMessage = key;
    AppendConsole(line);
    m_lastWaitMessage = key;
}

void AppendWaitMessage(string line, string key)
{
    if (string.Equals(key, m_lastWaitMessage, StringComparison.Ordinal))
        return;

    m_lastWaitMessage = key;
    if (m_console.Length == 0)
        m_console = "WELD PLANNER BoM Loading:";

    m_console += "\n" + line;
    Echo(m_console);
}

bool TryParseCustomData(string data, List<JobLine> output, out string error)
{
    output.Clear();
    error = "";

    string normalized = data.Replace("\r", "");
    string[] lines = normalized.Split('\n');

    if (lines.Length == 0 || !string.Equals(lines[0].Trim(), HEADER, StringComparison.Ordinal))
    {
        error = "First line must be exactly: " + HEADER;
        return false;
    }

    for (int i = 1; i < lines.Length; i++)
    {
        string line = lines[i].Trim();
        if (line.Length == 0)
            continue;

        int colon = line.LastIndexOf(':');
        if (colon <= 0 || colon >= line.Length - 1)
        {
            error = "Invalid task line " + (i + 1) + ": " + line;
            return false;
        }

        string name = line.Substring(0, colon).Trim();
        string amountText = line.Substring(colon + 1).Trim();

        int amount;
        if (name.Length == 0 || !int.TryParse(amountText, out amount) || amount < 0)
        {
            error = "Invalid task line " + (i + 1) + ": " + line;
            return false;
        }

        // A zero amount is already complete; do not keep it in the active list.
        if (amount == 0)
            continue;

        JobLine job = new JobLine();
        job.Name = name;
        job.Amount = amount;
        output.Add(job);
    }

    return true;
}

void CollectTargetContainers()
{
    m_targetContainers.Clear();
    m_targetBlockIds.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(m_targetContainers, delegate(IMyCargoContainer block)
    {
        if (block == null || !block.IsSameConstructAs(Me))
            return false;

        IMyInventory inv = block.GetInventory(0);
        if (inv == null || !inv.CanPutItems)
            return false;

        m_targetBlockIds.Add(block.EntityId);
        return true;
    });
}

void CollectSourceInventories()
{
    m_sourceInventories.Clear();
    m_allBlocks.Clear();
    GridTerminalSystem.GetBlocks(m_allBlocks);

    for (int b = 0; b < m_allBlocks.Count; b++)
    {
        IMyTerminalBlock block = m_allBlocks[b];
        if (block == null)
            continue;

        if (IsTargetBlock(block))
            continue;

        for (int i = 0; i < block.InventoryCount; i++)
        {
            IMyInventory inv = block.GetInventory(i);
            if (inv == null)
                continue;

            m_sourceInventories.Add(inv);
        }
    }
}

bool IsTargetBlock(IMyTerminalBlock block)
{
    for (int i = 0; i < m_targetBlockIds.Count; i++)
    {
        if (m_targetBlockIds[i] == block.EntityId)
            return true;
    }

    return false;
}

MoveResult MoveOneTask(JobLine job)
{
    MoveResult result = new MoveResult();
    result.TargetHasSpace = false;

    MyItemType itemType = new MyItemType("MyObjectBuilder_Component", job.Name);

    for (int t = 0; t < m_targetContainers.Count; t++)
    {
        if (job.Amount <= result.Moved)
            break;

        IMyCargoContainer targetBlock = m_targetContainers[t];
        IMyInventory target = targetBlock.GetInventory(0);
        if (target == null || !target.CanPutItems)
            continue;

        if (target.CanItemsBeAdded((MyFixedPoint)1, itemType))
            result.TargetHasSpace = true;

        for (int s = 0; s < m_sourceInventories.Count; s++)
        {
            if (job.Amount <= result.Moved)
                break;

            IMyInventory source = m_sourceInventories[s];
            if (source == null || source == target)
                continue;

            if (!source.CanTransferItemTo(target, itemType))
                continue;

            result.ConnectedSourceExists = true;

            MyFixedPoint sourceTotal = source.GetItemAmount(itemType);
            if (sourceTotal <= (MyFixedPoint)0)
                continue;

            result.SourceHasItem = true;

            m_items.Clear();
            source.GetItems(m_items);

            for (int i = 0; i < m_items.Count; i++)
            {
                if (job.Amount <= result.Moved)
                    break;

                MyInventoryItem item = m_items[i];
                if (!item.Type.Equals(itemType))
                    continue;

                int available = (int)item.Amount;
                if (available <= 0)
                    continue;

                int remaining = job.Amount - result.Moved;
                int wanted = available < remaining ? available : remaining;
                int fit = FindMaxFit(target, itemType, wanted);

                if (fit <= 0)
                {
                    continue;
                }

                if (!source.TransferItemTo(target, item, (MyFixedPoint)fit))
                    continue;

                result.Moved += fit;
            }
        }
    }

    // Re-evaluate space for the current component after transfers. This gives a
    // reliable "full" answer when the last successful move filled all target cargo.
    if (!HasAnyTargetSpace(itemType))
        result.TargetHasSpace = false;

    return result;
}

bool HasAnyTargetSpace(MyItemType itemType)
{
    for (int i = 0; i < m_targetContainers.Count; i++)
    {
        IMyInventory inv = m_targetContainers[i].GetInventory(0);
        if (inv == null || !inv.CanPutItems)
            continue;

        if (inv.CanItemsBeAdded((MyFixedPoint)1, itemType))
            return true;
    }

    return false;
}

int FindMaxFit(IMyInventory target, MyItemType itemType, int requested)
{
    if (requested <= 0)
        return 0;

    if (target.CanItemsBeAdded((MyFixedPoint)requested, itemType))
        return requested;

    int low = 0;
    int high = requested;

    while (low < high)
    {
        int mid = low + (high - low + 1) / 2;
        if (target.CanItemsBeAdded((MyFixedPoint)mid, itemType))
            low = mid;
        else
            high = mid - 1;
    }

    return low;
}

void BuildRemainingJobs(List<JobLine> jobs)
{
    m_remainingJobs.Clear();

    for (int i = 0; i < jobs.Count; i++)
    {
        JobLine job = jobs[i];
        if (job.Amount <= 0)
            continue;

        JobLine copy = new JobLine();
        copy.Name = job.Name;
        copy.Amount = job.Amount;
        m_remainingJobs.Add(copy);
    }
}

string BuildCustomData(List<JobLine> jobs)
{
    var sb = new StringBuilder();
    sb.AppendLine(HEADER);

    for (int i = 0; i < jobs.Count; i++)
    {
        if (jobs[i].Amount <= 0)
            continue;

        sb.Append(jobs[i].Name);
        sb.Append(": ");
        sb.AppendLine(jobs[i].Amount.ToString());
    }

    return sb.ToString().TrimEnd('\r', '\n');
} 