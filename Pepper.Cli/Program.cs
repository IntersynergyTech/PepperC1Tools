using System.IO.Ports;
using System.Runtime.InteropServices;
using Intersynergy.Poker.ApiClient;
using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data.Enums;
using Pepper.Cards.Database;
using Pepper.Cli.Windows;
using Pepper.Core.Data;
using Pepper.Core.Devices;
using Pepper.Core.Devices.C1;
using Pepper.Device.C1;
using Pepper.Device.C1.Ports;
using Pepper.Device.Dummy;
using Terminal.Gui.App;
using Suit = Intersynergy.Poker.ApiClient.Suit;

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
        optionsBulder.UseSqlServer(
            "Data Source=localhost;Database=CardsData;Integrated Security=true;TrustServerCertificate=True;");
        CardsDbContext = new CardsDbContext(optionsBulder.Options);

        CardsDbContext.Database.EnsureCreated();
        var pendingMigrations = CardsDbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Console.WriteLine("Applying database migrations...");
            CardsDbContext.Database.Migrate();
        }

        Console.WriteLine("Database is ready!");

        // Run remote sync via service
        try
        {
            var httpClient = new HttpClient();
            var client = new IntersynergyApiClient("https://localhost:7582", httpClient);
            var syncService = new Services.RemoteSyncService(CardsDbContext, client);
            syncService.SyncAllAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // Ensure any unexpected sync errors are logged but do not stop the app
            Console.WriteLine($"Unexpected error during initial sync: {ex}");
        }

        Console.WriteLine("Getting reader(s)");

        // Add any more devices we want here.
        //var pepduino = new Device.PepDuino.PepDuino(readerId: 0, "COM3");
        var peppers = new List<ITagReader>();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var availablePorts = GetAvailableSerialPorts();
            Console.WriteLine("Available Serial Ports:");
            
            foreach (var port in availablePorts)
            {
                Console.WriteLine("Attempting to setup Pepper C1 on port " + port);
                try
                {
                    var index = availablePorts.IndexOf(port);
                    var c1Port = new Uart(port);
                    var pepper = new PepperC1(c1Port, readerId: index);
                    peppers.Add(pepper);
                    Console.WriteLine("Successfully connected Pepper C1 on port " + port);
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to connect on port " + port);
                }
            }
            
        }



        /*// Add a couple of dummy readers for testing to spam cards randomly.
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
        );*/

        var multiReader = new MultiplexReader(peppers.ToArray()
        );
        multiReader.TagDetected += TagDetectedProxy;

        Console.WriteLine("Starting Readers...");
        multiReader.StartAll();

        Application.Run<MainMenu>();

        Console.WriteLine("Stopping Readers...");

        multiReader.StopAll();
        multiReader.DisposeAll();

        Console.WriteLine("Thanks for playing Wing Commander!");
    }

    private static List<string> GetAvailableSerialPorts()
    {
        var returnable = new List<string>();
        foreach (var port in SerialPort.GetPortNames())
        {
            try
            {
                var sp = new SerialPort(port)
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeout = 500,
                    WriteTimeout = 500
                };
                sp.Open();
                returnable.Add(port);
                sp.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with port {port}: {e.Message}");
                continue;
            }
        }
        return returnable;
    }

    private static void TagDetectedProxy(object? sender, DetectedTag tag)
    {
        Application.Invoke(() => { TagDetected?.Invoke(sender, tag); });
    }
}