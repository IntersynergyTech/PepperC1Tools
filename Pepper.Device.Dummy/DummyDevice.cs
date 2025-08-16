using System.Timers;
using Pepper.Cards.Data.DbModels;
using Pepper.Core.Data;
using Pepper.Core.Devices;
using Timer = System.Timers.Timer;

namespace Pepper.Device.Dummy;

public class DummyDevice : ITagReader
{
    public List<Card> CardsList { get; }
    public int[] AntennaIds { get; }
    public int Interval { get; }
    public int StartDelay { get; }

    public static Random Random { get; } = new Random();

    public Timer RandomTimer { get; }

    public DummyDevice(
        int readerId,
        List<Card> cardsList,
        int interval,
        int startDelay,
        params int[] antennaIds
    )
    {
        CardsList = cardsList;
        AntennaIds = antennaIds;
        Interval = interval;
        StartDelay = startDelay;
        ReaderId = readerId;

        RandomTimer = new Timer(startDelay);
        RandomTimer.Elapsed += TimerCallback;
    }

    private void TimerCallback(object? sender, ElapsedEventArgs e)
    {
        RandomTimer.Interval = Interval;
        GenerateTagEvent();
    }

    private void GenerateTagEvent()
    {
        var card = CardsList[Random.Next(CardsList.Count)];
        var antennaId = AntennaIds[Random.Next(AntennaIds.Length)];

        var tag = new Tag(CardType.Dummy, TagType.DummyTag, card.TagUid, antennaId);

        TagDetected?.Invoke(this, tag);
    }

    public void StartReading()
    {
        RandomTimer.Start();
    }

    public void StopReading()
    {
        RandomTimer.Stop();
    }

    public int ReaderId { get; }
    public EventHandler<Tag> TagDetected { get; set; }
}
