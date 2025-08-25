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
    public byte[] CommandData { get; }
}
