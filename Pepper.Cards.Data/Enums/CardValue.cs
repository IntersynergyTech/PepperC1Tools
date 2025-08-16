namespace Pepper.Cards.Data.Enums;

public enum CardValue
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,

    JokerRed = 14,
    JokerBlack = 15,
    Unknown = 16,
}

public static class CardValueExtensions
{
    public static string ToDisplayString(this CardValue value)
    {
        return value switch
        {
            CardValue.Ace => "Ace",
            CardValue.Two => "Two",
            CardValue.Three => "Three",
            CardValue.Four => "Four",
            CardValue.Five => "Five",
            CardValue.Six => "Six",
            CardValue.Seven => "Seven",
            CardValue.Eight => "Eight",
            CardValue.Nine => "Nine",
            CardValue.Ten => "Ten",
            CardValue.Jack => "Jack",
            CardValue.Queen => "Queen",
            CardValue.King => "King",
            CardValue.JokerRed => "Joker",
            CardValue.JokerBlack => "Wild",
            _ => "Unknown"
        };
    }

    public static string ToCardDisplayString(this CardValue value)
    {
        return value switch
        {
            CardValue.Ace => "A",
            CardValue.Two => "2",
            CardValue.Three => "3",
            CardValue.Four => "4",
            CardValue.Five => "5",
            CardValue.Six => "6",
            CardValue.Seven => "7",
            CardValue.Eight => "8",
            CardValue.Nine => "9",
            CardValue.Ten => "10",
            CardValue.Jack => "J",
            CardValue.Queen => "Q",
            CardValue.King => "K",
            CardValue.JokerRed => "@@",
            CardValue.JokerBlack => "##",
            _ => "XX"
        };
    }
    
    public static string ToShortDisplayString(this CardValue value)
    {
        return value switch
        {
            CardValue.Ace => "A",
            CardValue.Two => "2",
            CardValue.Three => "3",
            CardValue.Four => "4",
            CardValue.Five => "5",
            CardValue.Six => "6",
            CardValue.Seven => "7",
            CardValue.Eight => "8",
            CardValue.Nine => "9",
            CardValue.Ten => "T",
            CardValue.Jack => "J",
            CardValue.Queen => "Q",
            CardValue.King => "K",
            CardValue.JokerRed => "@",
            CardValue.JokerBlack => "#",
            CardValue.Unknown => "_",
            _ => value.ToDisplayString()
        };
    }
}
