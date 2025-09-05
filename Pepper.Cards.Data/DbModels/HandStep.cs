using Pepper.Cards.Data.Enums;

namespace Pepper.Cards.Data.DbModels;

public class HandStep
{
    public int Id { get; set; }
    public int HandId { get; set; }
    public GameHand Hand { get; set; }
    public CardMovementType Type { get; set; }
    public int? FromPositionId { get; set; }
    public TablePosition? FromPosition { get; set; }
    public int ToPositionId { get; set; }
    public TablePosition ToPosition { get; set; }
    public int CardId { get; set; }
    public Card Card { get; set; }
    public double Time { get; set; }
}
