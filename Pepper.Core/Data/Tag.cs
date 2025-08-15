namespace Pepper.Core.Data;

public class Tag
{
    public Tag(
        CardType cardType,
        TagType tagType,
        byte[] tagId
    )
    {
        CardType = cardType;
        TagType = tagType;
        TagId = tagId;
    }

    public CardType CardType { get; }
    public TagType TagType { get; }
    public byte[] TagId { get; }

    public override string ToString()
    {
        return $"{CardType} {TagType} {BitConverter.ToString(TagId).Replace("-", "")}";
    }
}
