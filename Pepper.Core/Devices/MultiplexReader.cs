using Pepper.Core.Data;
using Pepper.Core.Devices;

namespace Pepper.Device.C1;

/// <summary>
/// A class which encapsulates multiple readers together to make event handling easier. Don't forget to call StartAll() and StopAll() to control the readers.
/// </summary>
public class MultiplexReader
{
    private Dictionary<int, ITagReader> _tagReaders;

    /// <summary>
    /// You can set this if you need to control event behaviours like forcing events to fire on a certain thread, as they can come from anywhere.
    /// </summary>
    public Action<Action<DetectedTag>, DetectedTag> EventMarshaller { get; set; }

    public MultiplexReader(params ITagReader[] readers)
    {
        _tagReaders = readers.ToDictionary(x => x.ReaderId);

        foreach (var reader in _tagReaders.Values)
        {
            reader.TagDetected += ReaderTagDetected;
        }

        EventMarshaller = DefaultEventMarshaller;
    }

    private void ReaderTagDetected(object? sender, Tag e)
    {
        var reader = sender as ITagReader;
        if (reader == null) throw new ArgumentNullException(nameof(reader));
        var detectedTag = new DetectedTag(e, reader.ReaderId);

        EventMarshaller(tag => TagDetected?.Invoke(this, tag), detectedTag);
    }

    private void DefaultEventMarshaller(Action<DetectedTag> action, DetectedTag tag)
    {
        action(tag);
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
