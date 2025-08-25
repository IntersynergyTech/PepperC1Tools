using Pepper.Device.C1.Ports;

namespace Pepper.Device.C1.Cli;

class Program
{
    static void Main(string[] args)
    {
        var c1Port = new Uart("COM5");
        
    }
}
