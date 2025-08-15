namespace Pepper.Cards.Data.Models;

public class DeckStyle
{
    public bool IsFourColour { get; set; }
    public bool DarkMode { get; set; }
    public bool LargePrint { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Id}) - " + $"{(IsFourColour ? "4C" : "2C")}, " + $"{(DarkMode ? "Dark" : "Light")}, "
            + $"{(LargePrint ? "Large Print" : "")}";
    }
}
