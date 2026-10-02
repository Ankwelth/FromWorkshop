/*
 * /* v:1.0000 20201110
 * * LCD Copy and Clear - In-game script by Mmiller14
 * *
 * * This script allows the player; 
 * *	to clear the screen of one or more text surface(s);
 * *	or to copy the Public or Custom text of one text surface to one or more text surface(s);
 * *	by using the Programmable Block’s Run command.
 * *	
 * *	You can control what information appears on a text surface using buttons, timer blocks, sensors, hot bars, or anything
 * *	that can submit a Run command on a programmable block.
 * *	For example, you can broadcast messages to several LCDs located throughout your ship or base just by clicking a button
 * *	or triggering a sensor. If you are in a tight space or just don't want a bunch of LCDs hanging on you walls, you can
 * *	use one LCD to show the output of several different LCDs. I have a script that generates news flashes, menus,
 * *	work schedules, etc, and I use this script to "page" through the different content on the ship's mess hall/lounge LCD.
 * *	In combination with sensors and timer blocks, I use this script to send the messages, Stand Here, Scanning, Cleared,
 * *	and Proceed, to a single LCD as a player passes through a security checkpoint.
 * *
 * *	---THE SCRIPT CURRENTLY DOES NOT SUPPORT BLOCKS WITH MULTIPLE TEXT SURFACES LIKE COCKPITS AND BUTTON PANELS---
 * *
 * * Programmable Block Run syntex
 * * CLEAR COMMAND- clear:GroupFlag:TargetBlockName 
 * *	GroupFlag is either g = Group or s = Single
 * *	TargetBlockName is either the Group name or the Single block name of the text surface(s) you want to clear
 * *	EXAMPLE- clear:g:AllAlertLCDs
 * *	NOTE- The colons ':' are required
 * *	This run command will clear the text being displayed by all the blocks in the AllAlertLCDs group
 * *
 * * COPY COMMAND- copy:SourceFlag:SourceBlockName:GroupFlag:TargetBlockName
 * *	SourceFlag is either p = Public text or c = Custom text
 * *   SourceBlockName is the name of the block who text is being copied 
 * *	GroupFlag is either g = Group or s = Single
 * *	TargetBlockName is either the Group name or the Single block name of the text surface(s) where the Source text is to be written 
 * *	EXAMPLE- copy:c:LCD_AlertMessage:g:AllAlertLCDs
 * *	NOTE- The colons ':' are required
 * *	This run command will copy the text in the Custom field of the LCD_AlertMessage block to all the blocks in the group AllAlertLCDs
 * *
 * *	The clear command allows you to blank a text surface without having to turn it off and seeing the Offline message
 * *	You can use the custom field of any block as a source for the text. It does not have to be a block with a text surface.
 * *	This allows you to have various messages you may want to broadcast to one on more text surfaces without having to create hidden
 * *	text surface blocks to contain the messages
 * *	Use a timer block if you want the copied text to be updated from the source.
 * *
 * *	Thanks to malware https://github.com/malware-dev/MDK-SE for your wonderful SDK and all the Space Engineer Script Masters
 * *	who have extended this game into unimagined realms.  
 * *
 * *---THE ARE NO USER CONFIGURABLE SETTINGS IN THE SCRIPT---
 */


public Program()
{

}

public void Save()
{

}

public string[] argumentArray = new string[5]; // holds the Run arguments

public void Main(string argument, UpdateType updateSource)
{
    if (argument != null)
    {
        ParseArgument(argument); //break the argument string into an array of words
        DoArgunments(); //process the augument words
    }
    else
    {
        //exit script
    }
}

public void ParseArgument(string argument)
{
    //Echo(argument);

    argumentArray = argument.Split(':'); //load the argumentArray

    //Echo("{" + argumentArray[0] + ":" + argumentArray[1] + ":" + argumentArray[2] + ":" + argumentArray[3] + ":" + argumentArray[4] + "}");

}

public void DoArgunments()
{
    Echo("Run argument:" + argumentArray[0]);
    switch (argumentArray[0].ToLower())
    {
        case "clear":
            {

                Echo(argumentArray[1]); //g group or s single block
                Echo(argumentArray[2]); // group name or block name

                try
                {
                    if (argumentArray[1].ToLower() == "g")
                    {
                        //we are clearing a group of LCDs
                        IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(argumentArray[2]) as IMyBlockGroup;
                        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                        blockGroup.GetBlocks(blocks);
                        foreach (var block in blocks)
                        {
                            IMyTextSurface myScreenb;
                            myScreenb = block as IMyTextSurface;
                            myScreenb.WriteText(""); //clear the text panel's old message
                        }
                    }
                    else
                    {
                        //we are going to clear 1 lcd
                        IMyTerminalBlock targetLCD = GridTerminalSystem.GetBlockWithName(argumentArray[2]) as IMyTerminalBlock; //get the target LCD block
                        IMyTextSurface myScreen;
                        myScreen = targetLCD as IMyTextSurface;
                        myScreen.WriteText(""); //clear the text panel's old message
                    }
                }
                catch (Exception e)
                {
                    Echo("Error:failed to get targetLCD block");
                    Echo($"Exception: {e}\n---");
                    break;
                }

                break;
            }
        case "copy":
            {
                Echo(argumentArray[1]); //c Custom or p Public Text flag
                Echo(argumentArray[2]); //source LCD text
                Echo(argumentArray[3]); //group or single
                Echo(argumentArray[4]); //target block or blocks

                string sourceText;
                try
                {
                    IMyTerminalBlock sourceLCD = GridTerminalSystem.GetBlockWithName(argumentArray[2]) as IMyTerminalBlock; //get the source LCD block

                    if (argumentArray[1].ToLower() == "c")
                    {
                        //use the custome text for the source text
                        sourceText = sourceLCD.CustomData;
                    }
                    else
                    {
                        //use the Public text for the source text
                        IMyTextSurface sourceScreen;
                        sourceScreen = sourceLCD as IMyTextSurface;
                        sourceText = sourceScreen.GetText();
                    }


                    if (argumentArray[3].ToLower() == "g")
                    {
                        //we are writing to a group of LCDs
                        IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(argumentArray[4]) as IMyBlockGroup;
                        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                        blockGroup.GetBlocks(blocks);
                        foreach (var block in blocks)
                        {
                            IMyTextSurface targetScreenb;
                            targetScreenb = block as IMyTextSurface;
                            targetScreenb.WriteText(sourceText);
                        }
                    }
                    else
                    {
                        //we are going to write to 1 lcd
                        IMyTerminalBlock targetLCD = GridTerminalSystem.GetBlockWithName(argumentArray[4]) as IMyTerminalBlock; //get the target LCD block
                        IMyTextSurface targetScreen;
                        targetScreen = targetLCD as IMyTextSurface;
                        targetScreen.WriteText(sourceText);

                    }

                }
                catch (Exception e)
                {
                    Echo("Error:failed to get sourceLCD or targetLCD block");
                    Echo($"Exception: {e}\n---");
                    break;
                }

                break;
            }
        default:
            {
                Echo("Run argumnet not recognised");
                break;
            }
    }
}