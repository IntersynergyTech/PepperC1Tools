using Intersynergy.Poker.ApiClient;
using Pepper.Cards.Data.Enums;

namespace Pepper.Cli;

public static class Extensions
{
    public static Rank ToRank(this CardValue value)
    {
        return value switch
        {
            CardValue.Two => Rank.Two,
            CardValue.Three => Rank.Three,
            CardValue.Four => Rank.Four,
            CardValue.Five => Rank.Five,
            CardValue.Six => Rank.Six,
            CardValue.Seven => Rank.Seven,
            CardValue.Eight => Rank.Eight,
            CardValue.Nine => Rank.Nine,
            CardValue.Ten => Rank.Ten,
            CardValue.Jack => Rank.Jack,
            CardValue.Queen => Rank.Queen,
            CardValue.King => Rank.King,
            CardValue.Ace => Rank.Ace,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}