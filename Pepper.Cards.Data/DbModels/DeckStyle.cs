namespace Pepper.Cards.Data.DbModels;

public class DeckStyle
{
    /// <summary>
    /// If the deck is a four colour deck.
    /// </summary>
    public bool IsFourColour { get; set; }
    
    /// <summary>
    /// If the deck is displayed with light foreground on a dark background
    /// </summary>
    public bool DarkMode { get; set; }
    
    /// <summary>
    /// If the cards are large print
    /// </summary>
    public bool LargePrint { get; set; }
    
    /// <summary>
    /// A name to distinguish the deck style. Such as "Copag Blue 4 Colour"
    /// </summary>
    public string Name { get; set; }
    public int Id { get; set; }
    
    /// <summary>
    /// A key to identify the asset to use for the back design of the cards. UIs can use this if they want to more accurately show "hidden" cards in player positions more true to life.
    /// </summary>
    public string BackDesignKey { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Id}) - " + $"{(IsFourColour ? "4C" : "2C")}, " + $"{(DarkMode ? "Dark" : "Light")}, "
            + $"{(LargePrint ? "Large Print" : "")}";
    }
}
