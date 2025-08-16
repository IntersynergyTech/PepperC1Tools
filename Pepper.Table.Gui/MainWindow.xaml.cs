using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data;
using Pepper.Cards.Data.DbModels;
using Pepper.Core.Data;
using Pepper.Device.C1;
using Pepper.Device.Dummy;
using Pepper.Device.PepDuino;

namespace Pepper.Table.Gui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MultiplexReader Reader { get; }
    private readonly Dispatcher _uiDispatcher;
    public EventHandler<DetectedCard> CardDetected { get; set; }

    public List<Card> CardCache { get; set; }
    public List<TablePositionReader> TablePositionReaders { get; set; }

    public ObservableCollection<DetectedCard> RecentDetectedCards { get; } = new ObservableCollection<DetectedCard>();

    public MainWindow()
    {
        InitializeComponent();

        _uiDispatcher = Dispatcher.CurrentDispatcher;

        var pepduino = new PepDuino(readerId: 0, "COM3");

        // Load all the DB data we will need
        CardCache = App.CardsDbContext.Cards.Include(d => d.DeckStyle).ToList();
        TablePositionReaders = App.CardsDbContext.TablePositionReaders.Include(r => r.TablePosition).ToList();

        var dummyReader1 = new DummyDevice(
            readerId: 1,
            CardCache,
            interval: 2500,
            startDelay: 5000,
            antennaIds: [0, 1, 2, 3, 4, 5, 6, 7]
        );

        var dummyReader2 = new DummyDevice(
            readerId: 2,
            CardCache,
            interval: 3000,
            startDelay: 6000,
            antennaIds: [0, 1, 2, 3, 4, 5, 6, 7]
        );

        Reader = new MultiplexReader(pepduino, dummyReader1, dummyReader2);
        Reader.TagDetected += TagDetectedHandler;

        Console.WriteLine("Starting Readers...");
        Reader.StartAll();
        
        RecentDetectionsControl.ItemsSource = RecentDetectedCards;
    }

    private void TagDetectedHandler(object? sender, DetectedTag e)
    {
        var card = CardCache.SingleOrDefault(c => c.TagUid == e.TagId);

        if (card == null)
        {
            return;
        }

        var tablePositionReader =
            TablePositionReaders.SingleOrDefault(r => r.ReaderId == e.ReaderId && r.AntennaId == e.AntennaId);

        if (tablePositionReader == null)
        {
            return;
        }

        var position = tablePositionReader.TablePosition;

        var detectedCard = new DetectedCard(card, position, tablePositionReader, e);

        _uiDispatcher.BeginInvoke(
            () =>
            {
                AddDetectedCard(detectedCard);
                CardDetected?.Invoke(this, detectedCard);
            }
        );
    }

    private void AddDetectedCard(DetectedCard card)
    {
        RecentDetectedCards.Insert(0, card);

        while (RecentDetectedCards.Count > 20)
        {
            RecentDetectedCards.RemoveAt(RecentDetectedCards.Count - 1);
        }
    }

    private static MultiplexReader _reader;

    private void StartReaders_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}
