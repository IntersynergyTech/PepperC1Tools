using Pepper.Core.Data;

namespace Pepper.Core.Devices;

public interface ITagReader
{
    public void StartReading();
    public void StopReading();

    public int ReaderId { get; }

    /// <summary>
    /// Raise an event that a tag was detected. Note that the sender MUST be the instance of <see cref="ITagReader"/> that detected the tag.
    /// </summary>
    public EventHandler<Tag> TagDetected { get; set; }
}
