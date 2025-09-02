using Pepper.Core.Control.Subcommands.PollingConfig;
using Pepper.Core.Data;
using Pepper.Device.C1.Ports;

namespace Pepper.Device.C1;

public interface IC1Port
{
    public EventHandler<Tag> TagRead { get; set; }
    
    public ParsedResponse SendCommandRaw(byte[] command);

    public void DummyCommand();

    public string GetFirmwareVersion();

    public int GetTagCount();

    public void SetPollingActive(bool active);

    /// <summary>
    /// Which technologies are supported on the polling. Unclear what the downside is to enabling both, but probably best to limit to the ones you're using.
    /// </summary>
    public void SetPollingSupportedTechnologies(SupportedCardTypes technologies);

    public SupportedCardTypes GetPollingSupportedTechnologies();

    /// <summary>
    /// Sets the RFID power level. Defaults to Auto. When you set one it is set in the hardware, but doesn't apply until after reboot.
    /// </summary>
    public void SetPollingRfidPowerLevel(RfidPowerLevel level);
    public RfidPowerLevel GetPollingRfidPowerLevel();

    /// <summary>
    /// Set the time between polling reads. Value is in ms.
    /// </summary>
    /// <param name="timeout"></param>
    public void SetPollingTimeout(ushort timeout);
    public ushort GetPollingTimeout();
    
    /// <summary>
    /// the host can set the ignore timeout for the last detected tag. This timer starts counting when the tag is removed from the antenna field. Time in ms.
    /// </summary>
    /// <param name="timeout"></param>
    public void SetPollingIgnoreTimeout(ushort timeout);
    public ushort GetPollingIgnoreTimeout();

    /// <summary>
    /// Sets which antennas are actively being polled. You should probably only specify the antennas you actually have connected.
    /// </summary>
    public void SetPollingAntennas(ActiveAntennasMux antennas);
    public ActiveAntennasMux GetPollingAntennas();
    /// <summary>
    /// What kind of data you get back from the polling event. This would probably vary a bit depending on your connection mode, ie binary bytes won't be very useful for an MQTT polling event. 
    /// </summary>
    /// <param name="mode"></param>
    public void SetPollingEventPacket(PollingEventDataMode mode);
    public PollingEventDataMode GetPollingEventPacket();
}

