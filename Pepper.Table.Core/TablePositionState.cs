using System.Collections.ObjectModel;
using System.Diagnostics;
using Pepper.Cards.Data.DbModels;

namespace Pepper.Table.Core;

public class TablePositionState
{
    public TablePositionState(TablePosition position)
    {
        Position = position;
        CardsVisible = !position.NormallySecret;
    }

    public TablePosition Position { get; }

    public int? SeatNumber => Position.SeatNumber;
    public string Description => Position.Description;

    public bool CardsVisible { get; set; }

    public ObservableCollection<Card> Cards { get; } = new ObservableCollection<Card>();
    public object SyncLock { get; set; }

    private readonly List<string> _cardIds = new List<string>();

    internal void RemoveCard(Card card)
    {
        if (_cardIds.Contains(card.IdString))
        {
            Cards.Remove(card);
            _cardIds.Remove(card.IdString);
            Debug.WriteLine($"Removed card {card} from position {Position.Description}");
        }
    }

    internal void AddCard(Card card)
    {
        if (!_cardIds.Contains(card.IdString))
        {
            Cards.Add(card);
            _cardIds.Add(card.IdString);
            Debug.WriteLine($"Added card {card} to position {Position.Description}");
        }
    }

    internal void Reset()
    {
        CardsVisible = !Position.NormallySecret;
        Cards.Clear();
        _cardIds.Clear();
        Debug.WriteLine($"Reset position {Position.Description}");
    }
}
