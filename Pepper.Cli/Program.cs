using Intersynergy.Poker.ApiClient;
using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data.Enums;
using Pepper.Cards.Database;
using Pepper.Cli.Windows;
using Pepper.Core.Data;
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
        optionsBulder.UseSqlServer("Data Source=localhost;Database=CardsData;Integrated Security=true;");
        CardsDbContext = new CardsDbContext(optionsBulder.Options);

        CardsDbContext.Database.EnsureCreated();
        Console.WriteLine("Database is ready!");

        SyncToRemote();
        Console.WriteLine("Getting reader(s)");

        // Add any more devices we want here.
        //var pepduino = new Device.PepDuino.PepDuino(readerId: 0, "COM3");
        
        var pepperUart = new Uart("COM5");
        var pepper = new PepperC1(pepperUart, readerId: 1);

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

        var multiReader = new MultiplexReader(
            //pepduino
            //, dummyReader1
            //, dummyReader2
            pepper
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

    private static void TagDetectedProxy(object? sender, DetectedTag tag)
    {
        Application.Invoke(() => { TagDetected?.Invoke(sender, tag); });
    }
    private static async void SyncToRemote()
    {
        var httpClient = new HttpClient();
        var client = new IntersynergyApiClient("https://localhost:7582", httpClient);
        var cards = CardsDbContext.Cards.Include(x => x.DeckStyle).ToList();
        var decks = CardsDbContext.DeckStyles.ToList();
        foreach (var deck in decks)
        {
            try
            {
                var dto = new CreateDeckDto
                {
                    RemoteId = deck.Id.ToString(),
                    Name = deck.Name,
                    Description = deck.Name,
                    HasFourCornerLegend = deck.HasFourCornerLegend
                };
                var req = await client.DeckPOSTAsync(dto);
            }
            catch (ApiException e)
            {
                Console.WriteLine(e);
            }
        }

        var allDecks = await client.DeckAllAsync();

        foreach (var card in cards)
        {
            try
            {
                if(card.Value == CardValue.JokerBlack || card.Value == CardValue.JokerRed)
                    continue; //Remove jokes for now
                var deck = allDecks.FirstOrDefault(x => x.RemoteId == card.Id.ToString());
                if (deck == null)
                {
                    Console.WriteLine($"Deck not found for card {card.DeckStyle.Name}, skipping.");
                    continue;
                }
                var dto = new CardTagDto()
                {
                    Rank = card.Value.ToRank(),
                    Suit = (Suit)card.Suit,
                    TagUid = card.TagUid
                };
                var req = await client.TagsAsync(deck.Id, dto);
            }
            catch (ApiException e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}

public static class Extensions
{
    public static Rank ToRank(this CardValue value)
    {
        return value switch
        {
            CardValue.Two => Rank.Two,
            CardValue.Three => Rank.Three,
            CardValue.Four => Rank.Four,
            CardValue.Five => Rank.Five,
            CardValue.Six => Rank.Six,
            CardValue.Seven => Rank.Seven,
            CardValue.Eight => Rank.Eight,
            CardValue.Nine => Rank.Nine,
            CardValue.Ten => Rank.Ten,
            CardValue.Jack => Rank.Jack,
            CardValue.Queen => Rank.Queen,
            CardValue.King => Rank.King,
            CardValue.Ace => Rank.Ace,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
