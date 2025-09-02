using Pepper.Core.Control;

namespace Pepper.Device.C1.Ports;

public class ParsedResponse
{
    public ParsedResponse(Commands command, byte[] commandData)
    {
        Command = command;
        CommandData = commandData;
    }

    public Commands Command { get; }
    public Commands AcknowledgeRespondsTo => Command == Commands.Acknowledge ? (Commands) CommandData[0] : throw new ArgumentException("Not an Acknowledge command");
    public byte[] CommandData { get; }
    public byte[] AcknowledgeReponseCommand => Command == Commands.Acknowledge ? CommandData[1..] : throw new ArgumentException("Not an Acknowledge command");
}
