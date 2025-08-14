namespace pepper.core.Control.Subcommands;

public enum ProtocolConfigCommands
{
    General = 0x00,
    Uart = 0x01,
    TcpServer = 0x02,
    TcpClient = 0x03,
    Wpan = 0x04,
    MqttClient = 0x05,
    RestApi = 0x06,
    Websocket = 0x07,
}
