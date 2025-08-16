using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Database;
using Pepper.Cli.Windows;
using Pepper.Core.Data;
using Pepper.Core.Devices.C1;
using Pepper.Device.C1;
using Pepper.Device.Dummy;
using Terminal.Gui.App;

namespace Pepper.Cli;

class Program
{
    public static CardsDbContext CardsDbContext { get; set; }

    public static EventHandler<DetectedTag> TagDetected { get; set; }

    static void Main(string[] args)
    {
        Console.WriteLine("Pepper CLI Tool");

        Console.WriteLine("Preparing database...");
        var optionsBulder = new DbContextOptionsBuilder<CardsDbContext>();
        optionsBulder.UseSqlite("Data Source=C:/sources/pepper-c1-tool/cards.db");
        CardsDbContext = new CardsDbContext(optionsBulder.Options);

        CardsDbContext.Database.EnsureCreated();
        Console.WriteLine("Database is ready!");

        Console.WriteLine("Getting reader(s)");

        // Add any more devices we want here.
        var pepduino = new Device.PepDuino.PepDuino(readerId: 0, "COM3");

        // Add a couple of dummy readers for testing to spam cards randomly.
        var cardsList = CardsDbContext.Cards.ToList();

        var dummyReader1 = new DummyDevice(
            readerId: 1,
            cardsList,
            interval: 2500,
            startDelay: 5000,
            antennaIds: [0, 1, 2, 3, 4, 5, 6, 7]
        );

        var dummyReader2 = new DummyDevice(
            readerId: 2,
            cardsList,
            interval: 3000,
            startDelay: 6000,
            antennaIds: [0, 1, 2, 3, 4, 5, 6, 7]
        );

        var multiReader = new MultiplexReader(
            pepduino
            , dummyReader1
            , dummyReader2
        );
        multiReader.TagDetected += TagDetectedProxy;

        Console.WriteLine("Starting Readers...");
        multiReader.StartAll();

        Application.Run<MainMenu>();

        Console.WriteLine("Stopping Readers...");

        multiReader.StopAll();

        Console.WriteLine("Thanks for playing Wing Commander!");
    }

    private static void TagDetectedProxy(object? sender, DetectedTag tag)
    {
        Application.Invoke(() => { TagDetected?.Invoke(sender, tag); });
    }
}
