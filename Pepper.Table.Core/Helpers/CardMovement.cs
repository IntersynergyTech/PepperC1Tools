using Pepper.Cards.Data.DbModels;
using Pepper.Cards.Data.Enums;

namespace Pepper.Table.Core.Helpers;

public class CardMovement
{
    public CardMovement(
        Card card,
        CardMovementType type,
        TablePosition newPosition,
        TablePosition? previousPosition,
        double timestamp
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
    public double Timestamp { get; }
    public CardMovementType Type { get; }
}