namespace Pepper.Cards.Data.DbModels;

public class TablePosition
{
    public int Id { get; set; }
    public int? SeatNumber { get; set; }
    public bool NormallySecret { get; set; }

    public string Description { get; set; }
    public ICollection<TablePositionReader> Readers { get; set; }
}
