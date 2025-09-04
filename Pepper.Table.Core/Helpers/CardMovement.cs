using Pepper.Cards.Data.DbModels;

namespace Pepper.Table.Core.Helpers;

public class CardMovement
{
    public CardMovement(
        Card card,
        CardMovementType type,
        TablePosition newPosition,
        TablePosition? previousPosition,
        int timestamp
    )
    {
        Card = card;
        Type = type;
        NewPosition = newPosition;
        PreviousPosition = previousPosition;
        Timestamp = timestamp;
    }

    public Card Card { get; }
    public TablePosition NewPosition { get; }
    public TablePosition? PreviousPosition { get; }
    public int Timestamp { get; }
    public CardMovementType Type { get; }
}

public enum CardMovementType
{
    New = 1,
    Moved = 2
}
