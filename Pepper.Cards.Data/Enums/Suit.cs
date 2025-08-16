namespace Pepper.Cards.Data.Enums;

public enum Suit
{
    Hearts = 0,
    Diamonds = 1,
    Clubs = 2,
    Spades = 3,
    NotApplicable = 4
}

public static class SuitExtensions
{
    public static string ToDisplayString(this Suit suit)
    {
        return suit switch
        {
            Suit.Hearts => "Hearts",
            Suit.Diamonds => "Diamonds",
            Suit.Clubs => "Clubs",
            Suit.Spades => "Spades",
            _ => "Unknown"
        };
    }
    
    public static string ToShortDisplayString(this Suit suit)
    {
        return suit switch
        {
            Suit.Hearts => "H",
            Suit.Diamonds => "D",
            Suit.Clubs => "C",
            Suit.Spades => "S",
            _ => "U"
        };
    }
    
    public static string ToSymbol(this Suit suit)
    {
        return suit switch
        {
            Suit.Hearts => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs => "♣",
            Suit.Spades => "♠",
            _ => "?"
        };
    }
}