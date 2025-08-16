using Pepper.Cards.Data.Enums;

namespace Pepper.Cards.Data.Models;

public class Card
{
    public CardValue Value { get; set; }
    public int Id { get; set; }
    public byte[] TagUid { get; set; }
    public Suit Suit { get; set; }
    public DeckStyle DeckStyle { get; set; }

    public override string ToString()
    {
        return $"{Value.ToShortDisplayString()}{Suit.ToShortDisplayString()} [{BitConverter.ToString(TagUid)}] ({DeckStyle.Name})";
    }
}
