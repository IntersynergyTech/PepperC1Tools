using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Data;
using Pepper.Cards.Data.DbModels;
using Pepper.Core.Data;
using Pepper.Core.Devices.C1;
using Pepper.Device.C1;
using Pepper.Device.C1.Ports;
using Pepper.Device.Dummy;
using Pepper.Device.PepDuino;
using Pepper.Table.Core;

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

    public bool ShowIncoming { get; set; }

    public MainWindow()
    {
        InitializeComponent();

        _uiDispatcher = Dispatcher.CurrentDispatcher;

        // Load all the DB data we will need
        CardCache = App.CardsDbContext.Cards.Include(d => d.DeckStyle).ToList();
        TablePositionReaders = App.CardsDbContext.TablePositionReaders.Include(r => r.TablePosition).ToList();

        var pepperUart = new Uart("COM5");
        var pepperC1 = new PepperC1(pepperUart, readerId: 1);

        Reader = new MultiplexReader(pepperC1);
        Reader.EventMarshaller = UiThreadEventMarshaller;
        Reader.TagDetected += TagDetectedHandler;

        Console.WriteLine("Starting Readers...");
        Reader.StartAll();

        RecentDetectionsControl.ItemsSource = RecentDetectedCards;

        ShowIncoming = true;
    }

    private void UiThreadEventMarshaller(Action<DetectedTag> action, DetectedTag tag)
    {
        _uiDispatcher.BeginInvoke(action, tag);
    }

    private void TagDetectedHandler(object? sender, DetectedTag e)
    {
        if (!ShowIncoming)
        {
            return;
        }

        var card = CardCache.SingleOrDefault(c => c.IdString == e.TagIdString);

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

    private void OpenTableViewButtonClicked(object sender, RoutedEventArgs e)
    {
        var positions = App.CardsDbContext.TablePositions.Include(p => p.Readers).ToList();
        var cards = App.CardsDbContext.Cards.Include(c => c.DeckStyle).ToList();

        var tableState = new TableState(positions, cards, Reader);

        var tableWindow = new TableWindow(tableState);
        tableWindow.Show();

        ShowIncoming = false;
    }
}
