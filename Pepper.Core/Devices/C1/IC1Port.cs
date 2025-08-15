using Pepper.Core.Data;

namespace Pepper.Core.Devices.C1;

public interface IC1Port
{
    public EventHandler<Tag> TagDetected { get; set; }
}
