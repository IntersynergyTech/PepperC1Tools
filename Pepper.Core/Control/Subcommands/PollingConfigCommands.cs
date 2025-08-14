namespace Pepper.Core.Control.Subcommands;

public enum PollingConfigCommands
{
    SupportedCardTypes = 0x00,
    RfidPower = 0x01,
    InternalPollingMode = 0x02,
    PollingTimeout = 0x03,
    IgnoreTimeout = 0x04,
    ActiveAntennasMux = 0x05,
    EventDataMode = 0x06,
    EventLed = 0x07,
    EventGpio = 0x08,
    EventDuration = 0x09,
    EventCustomFormatString = 0x0A,
}
