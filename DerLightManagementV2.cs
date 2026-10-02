/*********************************************************************************************************************************************
 *  #################################
 *    Light Management V2.2
 *    by derDere
 *    last update 25.Jun.2022
 *  #################################
 *
 *  Block Setup: 
 *  =========
 *  Just folow this steps: 
 * 
 *  1. Build a programmable block. 
 *  3. Enter Script into programmable block. 
 *  4. Check Code
 *  5. Press OK
 *
 *  To Change the CurrentRoom:
 *  1. Enter Room Number (for Example 1) as Default Argument.
 *  2. Press Run
 *  Or to change via a ButtonPanel:
 *  1. Setup Actions
 *  2. Run With Argument. Enter Room Number as Argument.
 * 
 * 
 *  Easy Room Setup: 
 *  ============= 
 *  Every room is represented by a Number. Enter the current room number as default argument into the 
 *  programmable block. If the entered room has no default light the first unassigned light will be declared 
 *  as the new default light of this room. If the current room allready has a default light, every unassigned light will be assigned to the current room. 
 *  If you want create a new room definition just enter a new room number. 
 *  Try it by folowing this steps: 
 * 
 *  1. Set the default argument of the programmable block to your current/next room number (For example 104) and Run the Script once.
 *  2. Create a new Light. 
 *     (The script will now define this light as the default light for the current room 104) 
 *  4. Cofigure you light settings at you default light: color radius intensity ... 
 *     (Step 4 can be done at any time) 
 *  5. Setup all other light for your current room. 
 *     (The script will copy all settings from you default light to all new light and will assign all new light to the new room 104) 
 *  6. Create a new room by starting at step 1 but use a different room number. 
 * 
 *  If you don't want the programmable block to assign any new rooms just remove the room number from the default argument. 
 * 
 * 
 *  Advanced Settings: 
 *  ================== 
 *  You can insert different flags into the custom name of your lights to assing them as folowes: 
 *   
 *  [LM D104]   Default Light for room 104 
 *  [LM R104]   Room Light of room 104 
 *  [LM I]      This Light will be ignored by the script. 
 * 
 *  [LM O]      Enter this falg to a LCD panel to view the script output on it. 
 * 
 *  You can use a special argument to change a room color by fading into the new color. 
 *  Just run the Commandblock with the argument: 
 * 
 *  setColor <RoomNr> <Red>,<Green>,<Blue> 
 * 
 *  For example: 
 *  setColor 104 128,0,255 
 *
 **************************************************************************************************************************************************/ 

public void Main(string arg) {
    //Clearing output 
    Output.Clear();

    // Updating CurrentRoom
    if ( arg.Length > 0 ) {
      if ( Int32.TryParse(arg, out CurrentRoom) ) {
        return;
      }
    }
  
    //Collecting Light Blocks 
    List<IMyTerminalBlock> Lights = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(Lights); 
     
    //Collecting LCD Blocks
    List<IMyTerminalBlock> LcdPanels = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(LcdPanels); 
   
    //Dimensioning Dictionaries and Lists 
    Dictionary<int, IMyInteriorLight> DefaultLights = new Dictionary<int, IMyInteriorLight>();   
    Dictionary<int, List<IMyInteriorLight>> RoomLights = new Dictionary<int, List<IMyInteriorLight>>();   
    List<IMyInteriorLight> IgnoredLights = new List<IMyInteriorLight>(); 
    List<IMyInteriorLight> LeftoverLights = new List<IMyInteriorLight>(); 
  
    //Sorting Lights 
    foreach (IMyTerminalBlock block in Lights) {  
        IMyInteriorLight Light = (IMyInteriorLight)block;
        if ( DefaultLightRegEx.IsMatch( Light.CustomName ) ) {   
            int RoomNr = Int32.Parse( DefaultLightRegEx.Match( Light.CustomName ).Groups[2].Value ); 
            Light.CustomName = DefaultLightRegEx.Replace( Light.CustomName, "[LM D" + RoomNr + "]" ); 
            if ( DefaultLights.ContainsKey( RoomNr ) ) {   
                DefaultLights[RoomNr] = Light;   
            } else {   
                DefaultLights.Add( RoomNr, Light );   
            }
            if ( NewColorRegEx.IsMatch(Light.CustomName) ) { 
                int R = Int32.Parse( NewColorRegEx.Match(Light.CustomName).Groups[1].Value );
                int G = Int32.Parse( NewColorRegEx.Match(Light.CustomName).Groups[2].Value );
                int B = Int32.Parse( NewColorRegEx.Match(Light.CustomName).Groups[3].Value );
 
                if ( R < 0 ) R = 0; 
                if ( R > 255 ) R = 255; 

                if ( G < 0 ) G = 0; 
                if ( G > 255 ) G = 255;
 
                if ( B < 0 ) B = 0; 
                if ( B > 255 ) B = 255;

                Color C = Light.GetValue<Color>("Color");
                
                if ( C.R < R ) C.R += 1;
                if ( C.R > R ) C.R -= 1;
                
                if ( C.G < G ) C.G += 1;
                if ( C.G > G ) C.G -= 1;

                if ( C.B < B ) C.B += 1;
                if ( C.B > B ) C.B -= 1;

                Light.SetValue( "Color", C );
                Out( "Changing color at room: " + RoomNr.ToString() );
                
                if ( (C.R==R) && (C.G==G) && (C.B==B) ) {
                    Light.CustomName = NewColorRegEx.Replace( Light.CustomName, "" );
                }
            }
  
        } else if ( RoomLightRegEx.IsMatch( Light.CustomName ) ) {    
            int RoomNr = Int32.Parse( RoomLightRegEx.Match( Light.CustomName ).Groups[2].Value );  
            Light.CustomName = RoomLightRegEx.Replace( Light.CustomName, "[LM R" + RoomNr + "]" ); 
            if ( !RoomLights.ContainsKey( RoomNr ) ) {    
                RoomLights[RoomNr] = new List<IMyInteriorLight>();  
            }   
            RoomLights[RoomNr].Add( Light );  
  
        } else if ( IgnoreLightRegEx.IsMatch( Light.CustomName ) ) { 
            Light.CustomName = IgnoreLightRegEx.Replace( Light.CustomName, "[LM I]" ); 
            IgnoredLights.Add( Light );   
   
        } else {   
            LeftoverLights.Add( Light );   
        }   
    }  
    Lights.Clear();  
   
    //Setting NewColor info by Argument
    if ( arg.StartsWith("setColor") ) {   
        int RoomNr = Int32.Parse( arg.Split(' ')[1] );
        String Color = arg.Split(' ')[2];
        int R = Int32.Parse( Color.Split(',')[0] ); 
        int G = Int32.Parse( Color.Split(',')[1] ); 
        int B = Int32.Parse( Color.Split(',')[2] );
        if ( DefaultLights.ContainsKey(RoomNr) ) {
            if ( !NewColorRegEx.IsMatch(DefaultLights[RoomNr].CustomName) ) {
                DefaultLights[RoomNr].CustomName += "{NewColor:[" + R + "," + G + "," + B + "]}";
            } else {
                DefaultLights[RoomNr].CustomName = NewColorRegEx.Replace( DefaultLights[RoomNr].CustomName, "{NewColor:[" + R + "," + G + "," + B + "]}" );
            }
        }
        return; 
    }
  
    //Writing Status 
    Out( "Working " + RunSpinner[ RunSpinnerIndex++ ] );  
    if ( RunSpinnerIndex >= RunSpinner.Length ) RunSpinnerIndex = 0;  
    Out( "=======================" );
    Out( "CurrentRoom: " + CurrentRoom.ToString() );
    Out( "Found Rooms: " + DefaultLights.Count.ToString() );  
    foreach ( int Room in DefaultLights.Keys ) {  
        if ( RoomLights.ContainsKey(Room) ) 
            if ( RoomLights[Room].Count > 1 ) {  
                Out( " - Room(" + Room.ToString() + ") " + RoomLights[Room].Count.ToString() + " Lights" );  
            } else {  
                Out( " - Room(" + Room.ToString() + ") " + RoomLights[Room].Count.ToString() + " Light" );  
            }  
        else  
            Out( " - Room(" + Room.ToString() + ") 0 lights" );  
    }   
    Out( "Ignored Lights: " + IgnoredLights.Count.ToString() );  
    Out( "Leftover Lights: " + LeftoverLights.Count.ToString() ); 
    
    //Creating new Rooms
    if ( LeftoverLights.Count > 0 ) { 
        //Checking if current Room has a Default Light 
        if ( DefaultLights.ContainsKey(CurrentRoom) ) { 
            //Setting all Leftover Lights to current roomlights  
            LeftoverLights[0].CustomName += " [LM R" + CurrentRoom.ToString() + "]"; 
        } else { 
            //Setting first Leftover Light to new DefaultLight of Current Room 
            LeftoverLights[0].CustomName += " [LM D" + CurrentRoom.ToString() + "]";                 
        } 
    }
 
    //Setting Light Values 
    foreach ( int Room in DefaultLights.Keys ) { 
        IMyInteriorLight DL = DefaultLights[Room]; 
        if ( RoomLights.ContainsKey(Room) ) { 
            foreach ( IMyInteriorLight L in RoomLights[Room] ) {
                L.Enabled = DL.Enabled;
                L.SetValue("Color", DL.Color ); 
                L.SetValue("Radius", DL.Radius ); 
                L.SetValue("Falloff", DL.Falloff ); 
                L.SetValue("Intensity", DL.Intensity ); 
                L.SetValue("Offset", DL.GetValue<float>("Offset") ); 
                L.SetValue("Blink Interval", DL.BlinkIntervalSeconds ); 
                L.SetValue("Blink Lenght", DL.BlinkLength ); 
                L.SetValue("Blink Offset", DL.BlinkOffset );  
                L.SetValue("ShowInTerminal", false); 
            } 
        } 
    } 

    IMyTextSurface mesurface0 = Me.GetSurface(0);
			    mesurface0.WriteText(String.Join("\n", Output.ToArray()));
     
    //Writing Output to LCDs 
    foreach (IMyTerminalBlock block in LcdPanels) { 
        IMyTextPanel Lcd = (IMyTextPanel)block; 
        if ( OutputRegEx.IsMatch( Lcd.CustomName ) ) {  
            Lcd.CustomName = OutputRegEx.Replace( Lcd.CustomName, "[LM O]" ); 
            ((IMyTextSurface)Lcd).WriteText(String.Join("\n", Output.ToArray()));
        } 
    } 
}  

public System.Text.RegularExpressions.Regex IsIntRegEx = new System.Text.RegularExpressions.Regex(@"^\d+$" );
public System.Text.RegularExpressions.Regex NewColorRegEx = new System.Text.RegularExpressions.Regex(@"\{NewColor:\[(\d{1,3}),(\d{1,3}),(\d{1,3})\]\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
public System.Text.RegularExpressions.Regex DefaultLightRegEx = new System.Text.RegularExpressions.Regex(@"(\[LM\s+D)(\d+)(\s*\])", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
public System.Text.RegularExpressions.Regex RoomLightRegEx = new System.Text.RegularExpressions.Regex(@"(\[LM\s+R)(\d+)(\s*\])", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
public System.Text.RegularExpressions.Regex IgnoreLightRegEx = new System.Text.RegularExpressions.Regex(@"(\[LM\s+I.*?\])", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
public System.Text.RegularExpressions.Regex OutputRegEx = new System.Text.RegularExpressions.Regex(@"(\[LM\s+O.*?\])", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
public int RunSpinnerIndex = 0;
public int CurrentRoom = 1;
public readonly String[] RunSpinner = new String[] {"     ",".    "," .   ","  .  ","   . ","    .",".   ."," .  .","  . .","   ..",".  .."," . ..","  ...",". ..."," ....",".....",".... ", "... .", "...  ", ".. . ","..  .","..   ",". .  ",".  . ",".   .",".    "," .   ","  .  ","   . ","    ."};
public List<String> Output = new List<String>();
 
void Out(String Line) { 
    Echo(Line); 
    Output.Add(Line); 
} 
  
public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    IMyTextSurface mesurface0 = Me.GetSurface(0);
    mesurface0.ContentType = ContentType.TEXT_AND_IMAGE;
    mesurface0.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
    mesurface0.WriteText("Booting UP ...");
    Echo("Booting UP ...");
}  
  
public void Save() {  
    // Called when the program needs to save its state.  
}  
