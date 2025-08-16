namespace Pepper.Core.Control.Subcommands.NetworkConfig;

public enum WifiMode
{
    AccessPoint = 0x00,
    Client = 0x01,
    Disabled = 0x02,
}

public enum AuthorizationMode
{
    Open = 0x00,
    Wep = 0x01,
    Wpa_Psk = 0x02,
    Wpa2_Psk = 0x03,
    Wpa_Wpa2_Psk = 0x04,
    Wpa2_Enterprise = 0x05,
}

public enum IpAddressMode
{
    Dhcp = 0x00,
    Static = 0x01,
}
