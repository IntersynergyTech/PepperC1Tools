using Pepper.Core.Data;
using Pepper.Core.Devices.C1;

namespace pepper_c1_tool;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var pepduino = new Pepper.Device.PepDuino.PepDuino("COM3");
        var pepper = new PepperC1(pepduino, TagDetected);

        pepduino.Connect();

        Console.ReadLine();
        pepduino.Disconnect();
    }

    private static void TagDetected(object? sender, Tag tag)
    {
        Console.WriteLine(tag.ToString());
    }
}
