using Pepper.Cards.Data.DbModels;
using Pepper.Core.Data;

namespace Pepper.Cards.Data;

public class DetectedCard
{
    public DetectedCard(
        Card card,
        TablePosition tablePosition,
        TablePositionReader reader,
        Tag tag
    )
    {
        Card = card;
        TablePosition = tablePosition;
        Reader = reader;
        Tag = tag;
    }

    public Card Card { get; }
    public TablePosition TablePosition { get; set; }
    public TablePositionReader Reader { get; set; }
    public Tag Tag { get; }
}
