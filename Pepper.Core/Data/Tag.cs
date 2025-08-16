namespace Pepper.Core.Data;

public class Tag
{
    public Tag(
        CardType cardType,
        TagType tagType,
        byte[] tagId,
        int antennaId
    )
    {
        CardType = cardType;
        TagType = tagType;
        TagId = tagId;
        AntennaId = antennaId;
    }

    public CardType CardType { get; }
    public TagType TagType { get; }
    public byte[] TagId { get; }

    public int AntennaId { get; }

    public override string ToString()
    {
        return $"{CardType} {TagType} {BitConverter.ToString(TagId).Replace("-", "")}";
    }
}
