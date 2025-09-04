namespace Pepper.Table.Core.Helpers;

internal record struct ReaderMappingKey
{
    public int ReaderId { get; }
    public int AntennaId { get; }

    public ReaderMappingKey(int readerId, int antennaId)
    {
        ReaderId = readerId;
        AntennaId = antennaId;
    }
}
