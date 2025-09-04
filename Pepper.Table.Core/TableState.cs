using System.Collections.ObjectModel;
using System.Diagnostics;
using Pepper.Cards.Data.DbModels;
using Pepper.Core.Data;
using Pepper.Device.C1;
using Pepper.Table.Core.Helpers;

namespace Pepper.Table.Core;

public class TableState
{
    private readonly IReadOnlyCollection<TablePosition> _tablePositions;
    private readonly IReadOnlyCollection<Card> _cardCache;
    private readonly MultiplexReader _readers;

    private readonly HashSet<string> _cardsInPlay = new HashSet<string>();

    private readonly Dictionary<string, Card> _fasterCardCache = new ();
    private readonly Dictionary<ReaderMappingKey, TablePositionState> _readerPositionMap = new ();

    private bool _readingEnabled = false;
    private DateTime _handStartTime;
    private Stopwatch _handTimer;

    public ObservableCollection<TablePositionState> Positions { get; set; } = new ();

    public EventHandler<CardMovement> CardMovementDetected { get; set; }

    /// <summary>
    /// for designer only.
    /// </summary>
    public TableState()
    {
        _readers = new MultiplexReader();

        _tablePositions = new[]
        {
            new TablePosition
            {
                Id = 0,
                Description = "Default Position",
                NormallySecret = false,
                SeatNumber = null,
                Readers = new List<TablePositionReader> { new TablePositionReader { ReaderId = 0, AntennaId = 0 } }
            }
        };

        _cardCache = new List<Card>();
    }

    public TableState(
        IReadOnlyCollection<TablePosition> tablePositions,
        IReadOnlyCollection<Card> cardCache,
        MultiplexReader readers
    )
    {
        _tablePositions = tablePositions;
        _cardCache = cardCache;
        _readers = readers;

        // Map the readers to positions

        foreach (var pos in _tablePositions)
        {
            var positionState = new TablePositionState(pos);

            foreach (var reader in pos.Readers)
            {
                var key = new ReaderMappingKey(reader.ReaderId, reader.AntennaId);

                if (!_readerPositionMap.TryAdd(key, positionState))
                {
                    throw new InvalidOperationException(
                        $"Duplicate reader: {reader.ReaderId}, {reader.AntennaId}, already exists in another position when being added to position {pos.Description}"
                    );
                }
            }

            Positions.Add(positionState);
        }

        // Add handler for reader
        _readers.TagDetected += TagDetectedHandler;
    }

    public void StartHand()
    {
        if (_readingEnabled)
        {
            throw new InvalidOperationException("Cannot start a new hand while another is in progress.");
        }

        _cardsInPlay.Clear();
        _handTimer = Stopwatch.StartNew();
        _handStartTime = DateTime.UtcNow;
        _readingEnabled = true;
        Debug.WriteLine("Hand started, reading enabled.");
    }

    public void EndHand()
    {
        if (!_readingEnabled)
        {
            throw new InvalidOperationException("Cannot end a hand when none is in progress.");
        }

        _readingEnabled = false;
        _handTimer.Stop();

        foreach (var position in Positions)
        {
            position.Reset();
        }

        Debug.WriteLine($"Hand ended, reading disabled. Duration: {_handTimer.Elapsed}");
    }

    private void TagDetectedHandler(object? sender, DetectedTag tag)
    {
        if (!_readingEnabled)
        {
            return;
        }

        var card = ResolveCard(tag);

        if (card == null)
        {
            // Unknown card
            return;
        }

        var positionKey = new ReaderMappingKey(tag.ReaderId, tag.AntennaId);

        if (!_readerPositionMap.TryGetValue(positionKey, out var position))
        {
            // No position mapped to this reader/antenna
            return;
        }

        ApplyCardToPosition(card, position);
    }

    private Card? ResolveCard(DetectedTag tag)
    {
        var id = tag.TagIdString;

        if (_fasterCardCache.TryGetValue(id, out var card))
        {
            return card;
        }

        // Not in the faster cache, search the collection for it.
        var foundCard = _cardCache.Where(c => c.IdString == tag.TagIdString).FirstOrDefault();

        if (foundCard == null) return null;

        // Add to the faster cache for next time.
        _fasterCardCache[id] = foundCard;
        return foundCard;
    }

    private void ApplyCardToPosition(Card card, TablePositionState position)
    {
        TablePosition? existingPosition = null;
        CardMovementType movement = CardMovementType.New;
        
        //Check if the card is already in the position we're adding it to

        if (position.Cards.Contains(card))
        {
            return;
        }

        if (_cardsInPlay.Contains(card.IdString))
        {
            // The card is already in a position somewhere. We should remove it.
            foreach (var pos in Positions)
            {
                if (pos.Cards.Any(c => c.IdString == card.IdString))
                {
                    existingPosition = pos.Position;
                    pos.RemoveCard(card);
                    break;
                }
            }
            movement = CardMovementType.Moved;
        }

        _cardsInPlay.Add(card.IdString);

        position.AddCard(card);

        var handTime = _handTimer.Elapsed.TotalMilliseconds;

        // build the event
        var cardEvent = new CardMovement(
            card,
            movement,
            position.Position,
            existingPosition,
            (int) handTime
        );

        CardMovementDetected?.Invoke(this, cardEvent);
    }
}
