namespace Pepper.Core.Data;

public class DetectedTag(Tag tag, int readerId) : Tag(tag.CardType, tag.TagType, tag.TagId, tag.AntennaId)
{
    public int ReaderId { get; } = readerId;
}
