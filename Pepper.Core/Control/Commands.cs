namespace Pepper.Core.Control;

public enum Commands
{
    Acknowledge = 0x00,
    Dummy = 0x01,
    GetTagCount = 0x02,
    GetTagUid = 0x03,
    ActivateTag = 0x04,
    Halt = 0x05,
    SetPollingMode = 0x06,
    SetKey = 0x07,
    SaveKeys = 0x08,
    NetworkConfig = 0x09,
    Reboot = 0x0A,
    GetVersion = 0x0B,
    UartPassthrough = 0x0C,
    Sleep = 0x0D,
    Gpio = 0x0E,
    SetActiveAntenna = 0x0F,
    WpanPin = 0x10,
    FactoryReset = 0x11,
    ProtocolAuth = 0x12,
    ProtocolConfig = 0x13,
    Led = 0x14,
    WpanData = 0x15,
    PollingConfig = 0x16,
}
