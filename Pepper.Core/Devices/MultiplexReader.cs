using Pepper.Core.Data;
using Pepper.Core.Devices;

namespace Pepper.Device.C1;

/// <summary>
/// A class which encapsulates multiple readers together to make event handling easier. Don't forget to call StartAll() and StopAll() to control the readers.
/// </summary>
public class MultiplexReader
{
    private Dictionary<int, ITagReader> _tagReaders;
    
    public MultiplexReader(params ITagReader[] readers)
    {
        _tagReaders = readers.ToDictionary(x => x.ReaderId);

        foreach (var reader in _tagReaders.Values)
        {
            reader.TagDetected += ReaderTagDetected;
        }
    }

    private void ReaderTagDetected(object? sender, Tag e)
    {
        var reader = sender as ITagReader;
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var detectedTag = new DetectedTag(e, reader.ReaderId);
        TagDetected?.Invoke(this, detectedTag);
    }

    public void StartAll()
    {
        foreach (var reader in _tagReaders.Values)
        {
            reader.StartReading();
        }
    }

    public void StopAll()
    {
        foreach (var reader in _tagReaders.Values)
        {
            reader.StopReading();
        }
    }

    public EventHandler<DetectedTag> TagDetected { get; set; }
}
