const string DISPLAYTAG = "[BobDisplay]";
const string BUILDANDREPAIRTAG = "BuildAndRepairSystem";


List<IMyTerminalBlock> BobArray; 
List<IMyTerminalBlock> LCDsArray;
List<IMyTextSurface> textSurfaces;
string LeadingText = "BOB: ";
readonly MyIni textSurfaceIni = new MyIni();
const string INI_SECTION_TEXT_SURF_PROVIDER = "BOB - Text Surface Config";
const string INI_TEXT_SURFACE_TEMPLATE = "Show on screen {0}";

void AddTextSurfaces(IMyTerminalBlock block, List<IMyTextSurface> textSurfaces)
{
    var textSurface = block as IMyTextSurface;
    if (textSurface != null)
    {
        textSurfaces.Add(textSurface);
        return;
    }

    var surfaceProvider = block as IMyTextSurfaceProvider;
    if (surfaceProvider == null)
        return;

    textSurfaceIni.Clear();
    textSurfaceIni.TryParse(block.CustomData);
    int surfaceCount = surfaceProvider.SurfaceCount;
    for (int i = 0; i < surfaceCount; ++i)
    {
        string iniKey = string.Format(INI_TEXT_SURFACE_TEMPLATE, i);
        bool display = textSurfaceIni.Get(INI_SECTION_TEXT_SURF_PROVIDER, iniKey).ToBoolean(i == 0 && !(block is IMyProgrammableBlock));
        if (display)
            textSurfaces.Add(surfaceProvider.GetSurface(i));

        textSurfaceIni.Set(INI_SECTION_TEXT_SURF_PROVIDER, iniKey, display);
    }

    block.CustomData = textSurfaceIni.ToString();
}
 
public void SEStatusInit() 
{ 
    // Search for Guns in Grid 
    BobArray = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.SearchBlocksOfName(BUILDANDREPAIRTAG, BobArray); 
 
    // Building list of Displays to show status 
    LCDsArray = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.SearchBlocksOfName(DISPLAYTAG, LCDsArray); 

    //The New Way
    textSurfaces = new List<IMyTextSurface>();
    for (int i = 0; i < LCDsArray.Count; i++) 
    {  
        if (LCDsArray[i] == null) 
        { 
        } 
        else
        {
            AddTextSurfaces(LCDsArray[i], textSurfaces);
        }
    }
} 
 
public Program() 
{ 
    // Setting script for self-update eache 100 ticks = 1.6 sec 
    Runtime.UpdateFrequency = UpdateFrequency.Update100; 
 
    // Call init on 1st run! 
    SEStatusInit(); 
} 
 
public void Save() 
{ 
    // Called when the program needs to save its state. Use 
    // this method to save your state to the Storage field 
    // or some other means.  
    //  
    // This method is optional and can be removed if not 
    // needed. 
} 
 
public void Main(string argument) 
{
    int OnlineCount = 0;
    int OfflineCount = 0;
    bool AllOnline = false;
    bool AllOffline = false;
    bool PartialOnline = false;

    // Check Online Status of Guns
    for (int j = 0; j < BobArray.Count; j++) 
    {
        IMyFunctionalBlock Block = BobArray[j] as IMyFunctionalBlock;
        if (BobArray[j] == null) 
        { 
        } 
        else if (Block.Enabled) 
        {
            OnlineCount++;
        }
        else
        {
            OfflineCount++;
        }    
    } 
    if (OnlineCount > 0 && OfflineCount == 0)
    {
        AllOnline = true;
    }
    if (OnlineCount > 0 && OfflineCount > 0)
    {
        PartialOnline = true;
    }
    if (OnlineCount == 0 && OfflineCount > 0)
    {  
        AllOffline = true;
    }

    // Loop through all lcds with designated name 
    for (int i = 0; i < textSurfaces.Count; i++) 
    { 
 
        if (textSurfaces[i] == null) 
        { 
            Echo("You lost some lcds. ReCompile this script!"); 
            return; 
        } 
        textSurfaces[i].WriteText(LeadingText);
        if (AllOnline) {textSurfaces[i].WriteText("ONLINE", true);} 
        if (AllOffline) {textSurfaces[i].WriteText("OFFLINE", true);}
        if (PartialOnline) {textSurfaces[i].WriteText("PARTIAL", true);}
    } 
} 
