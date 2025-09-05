namespace Pepper.Cards.Data.DbModels;

public class GameHand
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; }
    public ICollection<HandStep> Steps { get; set; } = new List<HandStep>();
    public DateTime StartedUtc { get; set; }
    public DateTime? EndedUtc { get; set; }
}
