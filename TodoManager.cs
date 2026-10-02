/*
 * R e a d m e
 * -----------
 * Use one or two LCDs to maintain a list of todo items. Inspired from MadMavn's let's play.
 * 
 * === Usage ===
 * 
 * A LCD with the name set in TODO_LIST (default "Todo List")
 * The programmable block with this script
 * 
 * Commands:
 *   up      (move the selection up)
 *   down    (move the selection down)
 *   done    (mark selection done)
 * 
 * The programmable block 
 * 
 * *** A todo comment is
 * any line that does not start with the TODO_PREFIX
 * And the selected todo item has the TODO_CURSOR as well
 * -) A todo Item
 * > -) The selected todo
 * 
 * 
 * === User-changeable constants ===
 * 
 * These variables can be changed and the block recompiled
 * const String TODO_LIST = "Todo List";
 * const String DONE_LIST = "Done List";
 * const String TODO_PREFIX = "-) ";
 * const String TODO_CURSOR = "> ";
 * const String TODO_DONE_PREFIX = "X) ";
 * 
 * 
 */

    const String TODO_LIST = "Todo List";
    const String DONE_LIST = "Done List";
    const String TODO_PREFIX = "-) ";
    const String TODO_CURSOR = "> ";
    const String TODO_DONE_PREFIX = "X) ";


    // End of user-variables
    String lastTodoText;
    String lastDone;
    String selectedTodo;
    bool hasDoneLCD;

    List<String> lines;

    int timesParsed;
    int commentCount;
    int todoCount;

    IMyTextSurface GetGridTextSurface(String blockName)
    {
      IMyTerminalBlock surface = GridTerminalSystem.GetBlockWithName(blockName);
      if(surface == null || surface.CubeGrid != Me.CubeGrid) { return null; }
      return surface as IMyTextSurface;
    }

    public Program()
    {
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
      lines = new List<String>();
      timesParsed = 0;
      commentCount = 0;
      todoCount = 0;
    }

    bool IsTodoItem(String line)
    {
      return line.Trim().StartsWith(TODO_PREFIX);
    }

    void ParseTodoList(String todoList)
    {
      List<String> completed = new List<String>();
      String nextPanelText = "";
      String todoLine;
      todoCount = 0;
      commentCount = 0;
      lines.Clear();
      foreach(var line in todoList.Split('\n'))
      {
todoLine = line;
if(todoLine.StartsWith(TODO_DONE_PREFIX)) { completed.Add(todoLine); continue; }

// Copy the text as-is for change detection
if (nextPanelText != "") { nextPanelText += "\n" + line; } else { nextPanelText = line; }

if (todoLine.StartsWith(TODO_CURSOR))
{
  todoLine = todoLine.Replace(TODO_CURSOR, "");
  selectedTodo = todoLine;
}

lines.Add(todoLine);
if(todoLine.StartsWith(TODO_PREFIX))
{
  todoCount++;
}
else
{
  commentCount++;
}
      }
      if(!hasDoneLCD)
      {
foreach(var line in completed)
{
  lines.Add(line);
  if (nextPanelText != "") { nextPanelText += "\n" + line; } else { nextPanelText = line; }
}
      }
      lastTodoText = nextPanelText;
      timesParsed++;
    }

    bool UpdateData()
    {
      IMyTextSurface todoPanel = GetGridTextSurface(TODO_LIST);
      if (todoPanel != null)
      {
String todoText = todoPanel.GetText();

if (todoText != lastTodoText)
{
  ParseTodoList(todoText);
  Echo("Times list re-parsed: " + timesParsed + "\nComments: " + commentCount + "\nTodos: " + todoCount + "\nSelected: " + selectedTodo);
  return true;
}
      }
      else
      {
Echo("No text panel '" + TODO_LIST + "' found");
      }
      return false;
    }

    void UpdatePanel()
    {
      IMyTextSurface todoPanel = GetGridTextSurface(TODO_LIST);
      if (todoPanel == null) { return; }
      String panelText = "";
      String prefix = "";
      bool selectedStillExists = false;

      foreach(var line in lines)
      {
if (IsTodoItem(line) && selectedTodo == "") { selectedTodo = line; }

if (panelText == "") { prefix = "";  } else { prefix = "\n"; }
if (line == selectedTodo) { selectedStillExists = true; prefix += TODO_CURSOR; }

panelText += prefix + line;
      }

      // Clear selection if it doesn't exist anymore
      if(!selectedStillExists) { selectedTodo = ""; }

      todoPanel.WriteText(panelText, false);
    }

    void SelectPreviousTodo()
    {
      String previousTodo = "";
      foreach(var line in lines)
      {
if (IsTodoItem(line))
{
  if (line == selectedTodo)
    break;
  previousTodo = line;
}
      }
      if(previousTodo != "") { selectedTodo = previousTodo; }
    }

    void SelectNextTodo()
    {
      String nextTodo = "";
      bool nextTodoIsNew = false;
      foreach (var line in lines)
      {
if(IsTodoItem(line))
{
  if(line == selectedTodo)
  {
    nextTodoIsNew = true;
    continue;
  }

  if(nextTodoIsNew)
  {
    nextTodo = line;
    break;
  }
}
      }
      if(nextTodo != "") { selectedTodo = nextTodo; }
    }

    bool HasDoneLCD()
    {
      IMyTextSurface donePanel = GetGridTextSurface(DONE_LIST);
      hasDoneLCD = donePanel != null;
      return hasDoneLCD;
    }

    bool MarkSelectedDone()
    {
      if(selectedTodo == "") { return false; }
      for(var i = 0; i < lines.Count(); i++)
      {
if(lines[i] == selectedTodo)
{
  lines[i] = lines[i].Replace(TODO_CURSOR, "");
  lines[i] = lines[i].Replace(TODO_PREFIX, TODO_DONE_PREFIX);
  lastDone = lines[i];
  return true;
}
      }
      return false;
    }

    void UpdateDoneList(String item)
    {
      IMyTextSurface donePanel = GetGridTextSurface(DONE_LIST);
      if (lastDone == "" || donePanel == null) { return; }
      donePanel.WriteText(lastDone + "\n" + donePanel.GetText(), false);
    }

    public void Main(string argument, UpdateType updateSource)
    {
      HasDoneLCD();
      bool dataUpdated = UpdateData();

      switch (argument)
      {
case "up":
  SelectPreviousTodo();
  dataUpdated = true;
  break;
case "down":
  SelectNextTodo();
  dataUpdated = true;
  break;
case "done":
  dataUpdated = MarkSelectedDone();
  if(lastDone != "")
  {
    UpdateDoneList(lastDone);
    lastDone = "";
  }
  break;
      }

      if(dataUpdated)
      {
UpdatePanel();
      }
    }