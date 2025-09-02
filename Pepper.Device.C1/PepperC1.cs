using Pepper.Core.Control.Subcommands;
using Pepper.Core.Data;
using Pepper.Device.C1;

namespace Pepper.Core.Devices.C1;

//doc: https://eccel.co.uk/wp-content/downloads/Pepper_C1/C1_software_manual.pdf

public class PepperC1 : ITagReader
{
    public PepperC1(IC1Port port, int readerId)
    {
        Port = port;
    }

    public IC1Port Port { get; }

    public void StartReading()
    {
        throw new NotImplementedException();
    }

    public void StopReading()
    {
        throw new NotImplementedException();
    }

    public int ReaderId { get; set; }

    public EventHandler<Tag> TagDetected { get; set; }

    public void DummyCommand()
    {
        
    }
    
    public void Acknowledge()
    {
        //acknowledge
        throw new NotImplementedException();
    }

    public void GetTagCount()
    {
        //GetTagCount
        throw new NotImplementedException();
    }

    /// <summary>
    /// This command should be executed after GET_TAG_COUNT frame to read information about the tag
    /// </summary>
    /// <param name="tagIndex">TAG index in module memory, must me less than number of tags reported by <see cref="GetTag"/> command</param>
    /// <exception cref="NotImplementedException"></exception>
    public Tag GetTag(UInt16 tagIndex)
    {
        //GetTagUid
        throw new NotImplementedException();
    }

    /// <summary>
    /// The command executed to activate a TAG after the discovery loop if more than one TAG is detected
    /// </summary>
    public void ActivateTag()
    {
        // ActivateTag
        throw new NotImplementedException();
    }

    /// <summary>
    /// The Halt command takes no arguments. It halts the tag and turns off the RF field. It must be executed at the end of each operation on a tag to disable the antenna and reduce the power consumption.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Halt()
    {
        // halt
        throw new NotImplementedException();
    }

    /// <summary>
    /// The module can’t perform polling mode and RFID requests over the communication channels simultaneously. When polling is enabled and the host wants to request an RFID command, this command should be executed first with a STOP parameter, and then START again if needed afterwards. This command does not change polling configuration permanently, so after a reset, the module performs polling as configured in the Web Interface.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void SetPollingMode()
    {
        // set polling mode
        throw new NotImplementedException();
    }

    /// <summary>
    /// This command sets a KEY in Key Storage Memory on a selected slot. Set key can be used for all RFID functions needing authorization like e.g. READ/WRITE memory on the TAG etc. This command changes a key in volatile memory, so if the user wants to save it permanently and load automatically after boot-up, then the user should use the CMD_SAVE_KEYS command.
    /// </summary>
    /// <param name="keyNumber">Key number in Key Storage Memory</param>
    /// <param name="keyType">Key type. Different key types have different byte length requirements.</param>
    /// <param name="keyBytes">The key value. Length depends on the specified <see cref="keyType"/></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetKey(
        uint keyNumber,
        SetKeyTypes keyType,
        byte[] keyBytes
    )
    {
        //SetKey
        throw new NotImplementedException();
    }

    /// <summary>
    /// This command should be called if the user wants to save keys changed using the SET_KEY command in the module non-volatile memory. Saved keys will be automatically loaded after power up or reboot.
    /// </summary>
    public void SaveKeys()
    {
        //SaveKeys
    }

    /// <summary>
    /// Reboots the module. May be unresponsive for a little bit.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Reboot()
    {
        //reboot
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets firmware version.
    /// </summary>
    /// <returns></returns>
    public string GetVersion()
    {
        //GetVersion
        throw new NotImplementedException();
    }

    public byte[] UartPassthrough(byte[] uartData)
    {
        //uartPassthrough
        throw new NotImplementedException();
    }

    /// <summary>
    /// Puts the module to sleep
    /// </summary>
    public void Sleep()
    {
        //SLeep
        throw new NotImplementedException();
    }

    // not even bothering with GPIO command as we are definitely not using it and the sig is too much effort.

    /// <summary>
    /// This command sets the active antenna number. 
    /// </summary>
    /// <param name="antennaNumber">Available numbers are from 1 to 8</param>
    public void SetActiveAntennaMux(uint antennaNumber)
    {
        // SetACtiveAntenna
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the pin for the WPAN interface.
    /// </summary>
    /// <returns>the current PIN.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public string GetWpanPin()
    {
        //Wpan command, get
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the PIN for the WPAN interface.
    /// </summary>
    /// <param name="pinNumber">The new pin.</param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetWpanPin(string pinNumber)
    {
        //wpan command, set
        throw new NotImplementedException();
    }

    /// <summary>
    /// This command should be user to perform a factory reset
    /// </summary>
    public void FactoryReset()
    {
        //FactoryReset
        throw new NotImplementedException();
    }

    public void Led(uint rgbColor, UInt16 timeoutMs)
    {
        //Led
        throw new NotImplementedException();
    }

    /// <summary>
    /// This command should be used to send/receive frames from WPAN or WPAN SPP interface
    /// </summary>
    /// <param name="wpanBytes"></param>
    public void WpanPassthrough(byte[] wpanBytes)
    {
        //WpanData
        throw new NotImplementedException();
    }

    //Todo Breakouts:
    //Protcol settings (including protocol auth)
    // - includind children, ie wifi, wtc.
    //Polling settings
}
