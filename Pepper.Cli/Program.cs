using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Database;
using Pepper.Cli.Windows;
using Pepper.Core.Data;
using Pepper.Core.Devices.C1;
using Terminal.Gui.App;

namespace Pepper.Cli;

class Program
{
    public static CardsDbContext CardsDbContext { get; set; }
    public static PepperC1 PepperC1 { get; set; }

    public static EventHandler<Tag> TagDetected { get; set; }

    static void Main(string[] args)
    {
        Console.WriteLine("Pepper CLI Tool");

        Console.WriteLine("Preparing database...");
        var optionsBulder = new DbContextOptionsBuilder<CardsDbContext>();
        optionsBulder.UseSqlite("Data Source=cards.db");
        CardsDbContext = new CardsDbContext(optionsBulder.Options);

        CardsDbContext.Database.EnsureCreated();
        Console.WriteLine("Database is ready!");

        Console.WriteLine("Getting reader");
        var pepduino = new Device.PepDuino.PepDuino("COM3");
        PepperC1 = new PepperC1(pepduino, TagDetectedProxy);

        pepduino.Connect();

        Application.Run<MainMenu>();
        pepduino.Disconnect();
    }

    private static void TagDetectedProxy(object? sender, Tag tag)
    {
        Application.Invoke(() => { TagDetected?.Invoke(sender, tag); });
    }
}
