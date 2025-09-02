using Pepper.Core.Control;

namespace Pepper.Device.C1.Exceptions;

public class DeviceErrorException : Exception
{
    public DeviceErrorException(Commands errorCommand, byte layerByte,
        ErrorCodes errorCode
    )
    {
        ErrorCommand = errorCommand;
        LayerByte = layerByte;
        ErrorCode = errorCode;
    }

    public Commands ErrorCommand { get; }
    public byte LayerByte { get; }
    public ErrorCodes ErrorCode { get; }
}
