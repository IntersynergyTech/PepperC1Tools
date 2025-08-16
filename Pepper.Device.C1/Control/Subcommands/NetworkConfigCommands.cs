namespace Pepper.Core.Control.Subcommands;

public enum NetworkConfigCommands
{
    WifiMode = 0x00,
    AuthorizationMode = 0x01,
    WifiChannel = 0x02,
    WifiSsid = 0x03,
    WifiPassword = 0x04,
    IpAddressMode = 0x05,
    StaticIpAddress = 0x06,
    StaticIpMask = 0x07,
    StaticIpGateway = 0x08,
    StaticIpDnsServer = 0x09,
    WebUiUsername = 0x0A,
    WebUiPassword = 0x0B,
}
