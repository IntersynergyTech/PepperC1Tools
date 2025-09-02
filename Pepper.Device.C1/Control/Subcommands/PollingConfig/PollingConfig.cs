namespace Pepper.Core.Control.Subcommands.PollingConfig;

public enum SupportedCardTypes
{
    MifareFamily = 0x01,
    IcodeFamily = 0x10,
    MifareAndIcode = MifareFamily | IcodeFamily,
    
}

public enum RfidPowerLevel
{
    Auto = 0x00,
    Level1 = 0x01,
    Level2 = 0x02,
    Level3 = 0x03,
    Level4 = 0x04,
    Level5 = 0x05,
    Level6 = 0x06,
    Level7 = 0x07,
    Max = Level7
}

public enum InternalPollingMode
{
    Disabled = 0x00,
    Enabled = 0x01,
}

[Flags]
public enum ActiveAntennasMux
{
    Antenna1 = 0x01,
    Antenna2 = 0x02,
    Antenna3 = 0x04,
    Antenna4 = 0x08,
    Antenna5 = 0x10,
    Antenna6 = 0x20,
    Antenna7 = 0x40,
    Antenna8 = 0x80,
    
    AllAntennas = Antenna1 | Antenna2 | Antenna3 | Antenna4 | Antenna5 | Antenna6 | Antenna7 | Antenna8
}

public enum PollingEventDataMode
{
    None = 0x00,
    Binary = 0x01,
    Text = 0x02,
    Json = 0x03,
    Custom = 0x04,
}

public enum PollingEventLed
{
    None = 0x00,
    Red = 0x01,
    Green = 0x02,
    Blue = 0x03,
    White = 0x04,
}
