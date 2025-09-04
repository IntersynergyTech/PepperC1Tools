using Pepper.Cards.Data.Enums;

namespace Pepper.Cards.Data.DbModels;

public class Card
{
    public CardValue Value { get; set; }
    public int Id { get; set; }
    public byte[] TagUid { get; set; }
    public Suit Suit { get; set; }
    public DeckStyle DeckStyle { get; set; }

    public string IdString => BitConverter.ToString(TagUid);

    public override string ToString()
    {
        return $"{Value.ToShortDisplayString()}{Suit.ToShortDisplayString()} [{BitConverter.ToString(TagUid)}] ({DeckStyle.Name})";
    }
}
