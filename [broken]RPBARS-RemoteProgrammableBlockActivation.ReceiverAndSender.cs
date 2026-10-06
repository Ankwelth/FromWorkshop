/*
 *   R e a d m e
 *   -----------
 *   Setup instruction can be found at 
 *   https://steamcommunity.com/sharedfiles/filedetails/?id=1545087605
 *   
 *   Version 1.0.1
 */


internal Dictionary<string,int> Version => new Dictionary<string, int>
{
    { "Major", 2 },
    { "Minor", 0 },
    { "Patch", 1 }
};

public ScriptSettings MyScriptSettings { get; }
private readonly Reciever _reciever;
private readonly Transmitter _transmitter;
public MessageReporter MyMessageReporter { get; }
public MessageLog MyMessageLog { get; }

public Program()
{
    try
    {
        Runtime.UpdateFrequency |= UpdateFrequency.None;

        MyScriptSettings = new ScriptSettings(Me);
        MyMessageReporter = new MessageReporter(this);
        MyMessageLog = new MessageLog(this);

        if (MyMessageReporter.HasLCD)
        {
            Echo = MyMessageReporter.EchoToLcd;
        }

        if (!string.IsNullOrWhiteSpace(Storage))
        {
            MyMessageReporter.AddMessage("Clearing storage on startup.");
            Storage = string.Empty;
        }

        _reciever = new Reciever(this);
        _transmitter = new Transmitter(this);
    }
    catch (Exception ex)
    {
        MyMessageReporter.AddMessage($"{ex.Message}\n{ex.StackTrace}");
    }
    Echo($"RPBARS v{Version["Major"]}.{Version["Minor"]}.{Version["Patch"]}\n{MyMessageReporter.GetMessage()}");
    MyMessageReporter.ClearMessage();
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

public void Main(string argument, UpdateType updateSource)
{
    try
    {
        MyMessageReporter.ClearMessage();

        switch (updateSource)
        {
            case UpdateType.None:
                // do nothing on load
                break;
            case UpdateType.Antenna:
                if (!MyMessageLog.IsDuplicate(argument))
                {
                    _reciever.ReceiveMessage(argument);

                    if (MyScriptSettings.RelayMode)
                    {
                        MyMessageReporter.AddMessage("Relaying message...");
                        _transmitter.TransmitMessage(argument, true);
                    }

                    MyMessageLog.LogMessage(argument);
                }
                break;
            case UpdateType.Terminal:
            case UpdateType.Trigger:
            case UpdateType.Mod:
            case UpdateType.Script:
            case UpdateType.Update1:
            case UpdateType.Update10:
            case UpdateType.Update100:
            case UpdateType.Once:
            default:
                var message = _transmitter.TransmitMessage(argument);

                if (!string.IsNullOrWhiteSpace(message))
                    MyMessageLog.LogMessage(message);
                break;
        }
    }
    catch (Exception ex)
    {
        MyMessageReporter.AddMessage($"{ex.Message}\n{ex.StackTrace}");
    }

    Echo($"RPBARS v{Version["Major"]}.{Version["Minor"]}.{Version["Patch"]}\n{MyMessageReporter.GetMessage()}");
}

public class MessageLog
{
    public Program MyProgram { get; }
    public List<string> Messages { get; }

    public MessageLog(Program myProgram)
    {
        MyProgram = myProgram;
        Messages = MyProgram.Storage.Trim().Split('\n').ToList();
        Messages.RemoveAll(string.IsNullOrWhiteSpace);

        CleanSentMessages();
    }

    public void LogMessage(string message)
    {
        message = message.Replace("\n", "\t");
        Messages.Add(message);
        MyProgram.Storage = string.Join("\n", Messages);
    }

    public void CleanSentMessages()
    {
        var toRemove = new List<string>();
        MyProgram.MyMessageReporter.AddMessage($"History: {Messages.Count}");

        foreach (var message in Messages)
        {
            var realMessage = message.Replace("\t", "\n");
            var messageParts = realMessage.Split(MyProgram.MyScriptSettings.MessageDelimiter);

            DateTime timestamp;
            if (Utility.TryParseTicks(messageParts[1], out timestamp))
            {
                if (timestamp < DateTime.Now.AddSeconds(MyProgram.MyScriptSettings.KeepMessagesForSeconds))
                {
                    toRemove.Add(message);
                }
            }
        }

        Messages.RemoveAll(x => toRemove.Contains(x));
        MyProgram.Storage = string.Join("\n", Messages);
    }

    public bool IsDuplicate(string message)
    {
        message = message.Replace("\n", "\t");
        CleanSentMessages();
        return Messages.Contains(message);
    }
}

public class MessageReporter
{
    public Program MyProgram { get; }

    private List<IMyTerminalBlock> ReportLCDs { get; set; }

    private StringBuilder Message { get; set; }

    public bool HasLCD => ReportLCDs.Any();

    public MessageReporter(Program program)
    {
        MyProgram = program;


        Message = new StringBuilder();
        ReportLCDs = new List<IMyTerminalBlock>();

        if (!string.IsNullOrWhiteSpace(MyProgram.MyScriptSettings.StatusLcd))
        {
            MyProgram.GridTerminalSystem.SearchBlocksOfName(MyProgram.MyScriptSettings.StatusLcd, ReportLCDs, item => item is IMyTextPanel && item.CubeGrid == MyProgram.Me.CubeGrid);

            foreach (var lcd in ReportLCDs)
            {
                if (!(lcd is IMyTextPanel)) continue;
                ((IMyTextPanel)lcd).ShowPublicTextOnScreen();
                ((IMyTextPanel)lcd).FontSize = 0.8f;
                ((IMyTextPanel)lcd).Font = "Debug";
            }
        }
    }

    public void AddMessage(string message)
    {
        Message.AppendLine(message);
    }

    public string GetMessage()
    {
        return Message.ToString();
    }

    internal void EchoToLcd(string obj)
    {
        if (!ReportLCDs.Any()) return;

        foreach (var lcd in ReportLCDs)
        {
            (lcd as IMyTextPanel)?.WritePublicText(obj);
        }
    }

    public void ClearMessage()
    {
        Message.Clear();
    }
}


public class Reciever
{
    private Program MyProgram { get; }


    public Reciever(Program program)
    {
        MyProgram = program;
    }

    /// <summary>
            /// Receives a message in the format:
            ///
            /// @@RPBARS v1.0.0@@:[timestamp]:[Channel]:[Receiver]:[Target PB]:[Arguments]
            ///
            /// </summary>
            /// <param name="message"></param>
    public void ReceiveMessage(string message)
    {
        MyProgram.MyMessageReporter.AddMessage("Message received...");
        var messageParts = message.Split(MyProgram.MyScriptSettings.MessageDelimiter);

        if (messageParts.Length != 6)
        {
            MyProgram.MyMessageReporter.AddMessage("  Not enough parts.");
            return;
        }

        var version = messageParts[0];

        // TODO: Move this to version checker class
        if (version !=
            $"@@RPBARS {MyProgram.Version["Major"]}.{MyProgram.Version["Minor"]}.{MyProgram.Version["Patch"]}@@"
        )
        {
            version = version.Replace("@@RPBARS ", "").Replace("@@", "");
            var versionParts = version.Split('.');
            var major = 0;
            var minor = 0;
            var patch = 0;
            if (versionParts.Length != 3 || !int.TryParse(versionParts[0], out major)
                || !int.TryParse(versionParts[1], out minor)
                || !int.TryParse(versionParts[2], out patch))
            {
                MyProgram.MyMessageReporter.AddMessage("  version is malformed.");
                return;
            }

            if (major != MyProgram.Version["Major"])
            {
                if(major > MyProgram.Version["Major"])
                    MyProgram.MyMessageReporter.AddMessage("FATAL: Sender is newer than receiver. Stopped.\n\nPlease upgrade the script\nin all programmable blocks.");
                else
                    MyProgram.MyMessageReporter.AddMessage("FATAL: Receiver is newer than sender. Stopped.\n\nPlease upgrade the script\nin all programmable blocks.");
                return;
            }

            if (minor != MyProgram.Version["Minor"] || patch != MyProgram.Version["Patch"])
            {
                MyProgram.MyMessageReporter.AddMessage("WARNING: Version mismatch. \n\nPlease upgrade the script\nin all programmable blocks.");
            }
        }

        DateTime timestamp;
        if (!Utility.TryParseTicks(messageParts[1], out timestamp))
        {
            MyProgram.MyMessageReporter.AddMessage("  invalid timestamp.");
        }

        // If it's more than 2 seconds since the message was sent
        // ignore it
        if (timestamp <= DateTime.Now.AddSeconds(-3))
        {
            MyProgram.MyMessageReporter.AddMessage("  message is too old.\nIgnored.");
        }

        MyProgram.MyMessageReporter.AddMessage($"  message received at {timestamp:g}");

        var channel = messageParts[2];
        var receiver = messageParts[3];
        var targetPb = messageParts[4];
        var args = messageParts[5];

        // This PB is not on the channel that the message was sent over so ignore it
        if (channel.ToLower() != "#all" && !MyProgram.MyScriptSettings.ReceiverChannels.Contains(channel))
        {
            MyProgram.MyMessageReporter.AddMessage("Channel not found.\nMessage ignored.");
            return;
        }

        // If this message was not meant for this receiver then ignore it
        MyProgram.MyMessageReporter.AddMessage($"Reciever name: \"{receiver}\"");
        if (receiver.Trim().ToLower() != MyProgram.MyScriptSettings.RecieverName.Trim().ToLower() &&
            receiver.Trim().ToLower() != "@all")
        {
            MyProgram.MyMessageReporter.AddMessage("--\nNot for me.\nNo action taken.");
            return;
        }

        MyProgram.MyMessageReporter.AddMessage($"PB: \"{targetPb}\"");

        var pbs = new List<IMyTerminalBlock>();
        MyProgram.GridTerminalSystem.SearchBlocksOfName(targetPb, pbs, x => x.IsFunctional && x is IMyProgrammableBlock && x.CubeGrid == MyProgram.Me.CubeGrid);

        if (!pbs.Any())
        {
            MyProgram.MyMessageReporter.AddMessage("Message received but could\nnot find a Programmable Block\nwith that name.");
            return;
        }

        foreach (var targetProgrammableBlock in pbs.Select(x => (IMyProgrammableBlock)x))
        {
            MyProgram.MyMessageReporter.AddMessage($"Argument: \"{args}\"");
            var tries = 0;
            var success = targetProgrammableBlock.TryRun(args);
            while (!success && tries < 3)
            {
                tries++;
                success = targetProgrammableBlock.TryRun(args);
            }
            if (!success) MyProgram.MyMessageReporter.AddMessage($"Failed to run programmable\nblock {targetPb} with argument \"{args}\"");
        }
    }
}

public class ScriptSettings
{
    private IDictionary<string,string> CustomData { get; set; }

    public string RecieverName => CustomData.ContainsKey("RecieverName") ? CustomData["RecieverName"] : string.Empty;
    public string BroadcastAntenna => CustomData.ContainsKey("BroadcastAntenna") ? CustomData["BroadcastAntenna"] : string.Empty;
    public string StatusLcd => CustomData.ContainsKey("StatusLcd") ? CustomData["StatusLcd"] : string.Empty;
    public IEnumerable<string> ReceiverChannels => CustomData.ContainsKey("ReceiverChannels") ? CustomData["ReceiverChannels"].Split(',').Select( x => x.Replace(" ", "")) : new string[] {"#transmit"};

    public int KeepMessagesForSeconds
    {
        get
        {
            if (CustomData.ContainsKey("KeepMessagesForSeconds"))
            {
                int val;
                if(int.TryParse(CustomData["KeepMessagesForSeconds"], out val))
                {
                    return val;
                }
            }
            return 10;
        }
    }

    public bool RelayMode => CustomData.ContainsKey("RelayMode") && CustomData["RelayMode"].ToLower() == "true";

    public char MessageDelimiter => CustomData.ContainsKey("MessageDelimiter")
        ? CustomData["MessageDelimiter"].ToCharArray().First()
        : ':';

    public ScriptSettings(IMyProgrammableBlock programmableBlock)
    {
        CustomData = programmableBlock.CustomData.Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Contains('='))
            .ToDictionary(line => line.Split('=')[0], line => line.Split('=')[1]);
    }
}

public class Transmitter
{
    public Program MyProgram { get; }

    public Transmitter(Program program)
    {
        MyProgram = program;
    }

    /// <summary>
            /// Takes the message in format:
            ///
            /// [Channel]:[Receiver]:[Target PB]:[Arguments]
            ///
            /// Transmits a message in the format:
            ///
            /// @@RPBARS v1.0.0@@:[timestamp]:[Channel]:[Receiver]:[Target PB]:[Arguments]
            ///
            /// </summary>
            /// <param name="message"></param>
    public string TransmitMessage(string message, bool raw = false)
    {
        MyProgram.MyMessageReporter.AddMessage("Attempting transmit...");

        var messageParts = message.Split(MyProgram.MyScriptSettings.MessageDelimiter);
        if (!raw && messageParts.Length != 4)
        {
            MyProgram.MyMessageReporter.AddMessage("Not enough parts.");
            return string.Empty;
        }

        if (!raw)
        {
            var channel = messageParts[0];
            var receiver = messageParts[1];
            var targetPb = messageParts[2];
            var args = messageParts[3];

            var messageList = new List<string>
            {
                $"@@RPBARS {MyProgram.Version["Major"]}.{MyProgram.Version["Minor"]}.{MyProgram.Version["Patch"]}@@",
                DateTime.Now.Ticks.ToString(),
                channel,
                receiver,
                targetPb,
                args
            };

            MyProgram.MyMessageReporter.AddMessage($"  Channel: {channel}");
            MyProgram.MyMessageReporter.AddMessage($"  Receiver: {receiver}");
            MyProgram.MyMessageReporter.AddMessage($"  PB: {targetPb}");
            MyProgram.MyMessageReporter.AddMessage($"  Args: {args}");

            message = string.Join(":", messageList);
        }
        else
        {
            MyProgram.MyMessageReporter.AddMessage($"  Sending RAW message");
        }

        var antennas = new List<IMyRadioAntenna>();
        if (!string.IsNullOrWhiteSpace(MyProgram.MyScriptSettings.BroadcastAntenna))
        {
            MyProgram.MyMessageReporter.AddMessage($"Locating antenna {MyProgram.MyScriptSettings.BroadcastAntenna}...");
            var blocks = new List<IMyTerminalBlock>();
            MyProgram.GridTerminalSystem.SearchBlocksOfName(MyProgram.MyScriptSettings.BroadcastAntenna, blocks,
                a => a is IMyRadioAntenna && a.CubeGrid == MyProgram.Me.CubeGrid &&
                     a.Name == MyProgram.MyScriptSettings.BroadcastAntenna);

            antennas = blocks.Where(a => ((IMyRadioAntenna)a).Enabled && ((IMyRadioAntenna)a).EnableBroadcasting)
                .Select(a => (IMyRadioAntenna)a).ToList();
        }
        else
        {
            MyProgram.MyMessageReporter.AddMessage("No dedicated antenna.\nLooking for antennas...");
            MyProgram.GridTerminalSystem.GetBlocksOfType(antennas,
                a => a.CubeGrid == MyProgram.Me.CubeGrid && a.Enabled && a.EnableBroadcasting);
        }

        if (!antennas.Any())
        {
            MyProgram.MyMessageReporter.AddMessage("Cannot find an antenna.");
            return string.Empty;
        }

        var antenna = antennas.OrderByDescending(x => x.Radius).FirstOrDefault();

        if (antenna == null)
        {
            MyProgram.MyMessageReporter.AddMessage("Cannot find an antenna.");
            return string.Empty;
        }

        MyProgram.MyMessageReporter.AddMessage("Antenna(s) found.");

        var success = false;
        MyProgram.MyMessageReporter.AddMessage($"Sending...");
        var tries = 0;
        success = antenna.TransmitMessage(message);
        while (!success && tries < 3)
        {
            tries++;
            success = antenna.TransmitMessage(message);
        }

        MyProgram.MyMessageReporter.AddMessage(!success
            ? "  Failed."
            : "  Message sent.");

        return message;
    }
}

}
static class Utility
{
    public static bool TryParseTicks(string strTicks, out DateTime value)
    {
        long ticks;
        if (long.TryParse(strTicks, out ticks))
        {
            value = new DateTime(ticks);
            return true;
        }

        value = DateTime.MinValue;
        return false;
    }