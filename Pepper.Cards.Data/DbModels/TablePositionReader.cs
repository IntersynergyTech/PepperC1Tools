namespace Pepper.Cards.Data.DbModels;

public class TablePositionReader
{
    public int Id { get; set; }
    public TablePosition TablePosition { get; set; }
    public int ReaderId { get; set; }
    public int AntennaId { get; set; }
    public int TablePositionId { get; set; }
}
