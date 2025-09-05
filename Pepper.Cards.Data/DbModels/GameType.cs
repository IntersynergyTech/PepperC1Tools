namespace Pepper.Cards.Data.DbModels;

public class GameType
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
