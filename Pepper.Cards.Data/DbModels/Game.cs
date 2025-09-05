namespace Pepper.Cards.Data.DbModels;

public class Game
{
    public int Id { get; set; }
    public int TypeId { get; set; }
    public GameType Type { get; set; }
    public DateTime StartedUtc { get; set; }
    public DateTime? EndedUtc { get; set; }
    public ICollection<GameHand> Hands { get; set; }
    public string Notes { get; set; }
}
