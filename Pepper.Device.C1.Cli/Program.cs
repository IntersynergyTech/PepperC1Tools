using Pepper.Core.Control.Subcommands.PollingConfig;
using Pepper.Core.Data;
using Pepper.Device.C1.Ports;

namespace Pepper.Device.C1.Cli;

class Program
{
    static void Main(string[] args)
    {
        var c1Port = new Uart("COM5");
        Console.WriteLine($"C1 Port: {c1Port}");
        var version = c1Port.GetFirmwareVersion();
        Console.WriteLine($"Firmware version is: {version}");

        c1Port.TagRead += PollingResponseReceived;

        c1Port.SetPollingAntennas(ActiveAntennasMux.AllAntennas);
        c1Port.SetPollingSupportedTechnologies(SupportedCardTypes.MifareFamily);
        c1Port.SetPollingTimeout(200);
        //c1Port.SetPollingIgnoreTimeout(1000);
        c1Port.SetPollingEventPacket(PollingEventDataMode.Binary);
        c1Port.SetPollingActive(true);

        Console.WriteLine("Polling is enabled, press enter to get tag count");

        while (true)
        {
            Console.ReadLine();
            var data = c1Port.GetTagCount();
            Console.WriteLine($"Tag count: {data}");
        }
    }

    private static void PollingResponseReceived(object? sender, Tag e)
    {
        Console.WriteLine($"Polling response received: {e}");
    }
}
