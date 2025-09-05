using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using Pepper.Core.Control;
using Pepper.Core.Control.Subcommands;
using Pepper.Core.Control.Subcommands.PollingConfig;
using Pepper.Core.Data;
using Pepper.Core.Extensions;
using Pepper.Device.C1.Exceptions;
using Pepper.Device.C1.Ports.Checksum;

namespace Pepper.Device.C1.Ports;

public class Uart : IC1Port, IDisposable
{
    private readonly string _portName;
    private readonly int _baudRate;
    private readonly int _dataBits;
    private readonly Parity _parity;
    private readonly StopBits _stopBits;

    private readonly SerialPort _port;
    private readonly CancellationTokenSource _portWatcherCancellationTokenSource = new ();
    private readonly Thread _portWatcherThread;

    private static readonly ushort LengthXor = 0xffff;
    private const byte frameStartByte = 0xF5;

    private bool _bytesWaiting = false;
    private bool _pollingEnabled = false;
    private EventHandler<byte[]> _frameReceived;
    private byte[] _lastPayload = [];

    public Uart(
        string portName,
        int baudRate = 115200,
        int dataBits = 8,
        Parity parity = Parity.None,
        StopBits stopBits = StopBits.One
    )
    {
        _portName = portName;
        _baudRate = baudRate;
        _dataBits = dataBits;
        _parity = parity;
        _stopBits = stopBits;

        var port = new SerialPort(
            _portName,
            _baudRate,
            _parity,
            _dataBits,
            _stopBits
        );

        port.Open();

        port.ReceivedBytesThreshold = 8; // minimum size of a response frame

        _port = port;

        _portWatcherThread = new Thread(() => PortWatcher(_portWatcherCancellationTokenSource.Token));
        _portWatcherThread.Start();

        //_port.DataReceived += PortOnDataReceived;
        _frameReceived += FrameReceived;
    }

    public void Close()
    {
        _portWatcherCancellationTokenSource.Cancel();
        _port.Close();
    }

    public void Dispose()
    {
        if (_pollingEnabled)
        {
            SetPollingActive(false);
        }
        Close();
        _port.Dispose();
    }

    private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (_pollingEnabled)
        {
            var data = WaitForResponse(skipWait: true);

            if (data.Command == Commands.PollAsync)
            {
                var tag = ParseGetUidAsyncResponse(data);
                SendCommand([(byte) Commands.Acknowledge]);
                // raise event
                TagRead?.Invoke(this, tag);
            }
            else
            {
                throw new InvalidOperationException("Recieved non-poll data when polling is enablesd!");
            }
        }
        else
        {
            _bytesWaiting = true;
        }
    }

    private void FrameReceived(object? sender, byte[] e)
    {
        if (_pollingEnabled)
        {
            var data = ParseResponse(e);

            if (data.Command == Commands.PollAsync)
            {
                var tag = ParseGetUidAsyncResponse(data);
                SendCommand([(byte) Commands.Acknowledge]);
                // raise event
                TagRead?.Invoke(this, tag);
            }
            else
            {
                throw new InvalidOperationException("Recieved non-poll data when polling is enablesd!");
            }
        }
        else
        {
            _lastPayload = e;
            _bytesWaiting = true;
        }
    }

    private void SendCommand(byte[] commandData)
    {
        var frame = BuildCommandFrame(commandData);
        _port.Write(frame, 0, frame.Length);
    }

    private byte[] BuildCommandFrame(byte[] commandData)
    {
        // Start byte
        List<byte> frame = [frameStartByte];

        // actual command length
        var realCommandLength = (ushort) commandData.Length;

        if (realCommandLength >= 1022) // Actual max length with CRC is 1024
        {
            throw new ArgumentException("Command data too long");
        }

        ushort frameCommandLength = (ushort) (realCommandLength + 2); // +2 for CRC bytes

        var xorLength = (ushort) (frameCommandLength ^ LengthXor);

        // add length to frame
        frame.AddRange(BitConverter.GetBytes(frameCommandLength));

        // add xor'd length to frame
        frame.AddRange(BitConverter.GetBytes(xorLength));

        // add command data
        frame.AddRange(commandData);

        // calculate CRC
        var crc = UartCrcCalculator.ComputeChecksum(commandData);
        frame.AddRange(BitConverter.GetBytes(crc));

        // that's it, return the byte array
        return frame.ToArray();
    }

    private void PortWatcher(CancellationToken cancellationToken)
    {
        Queue<byte> readBuffer = new ();

        while (!cancellationToken.IsCancellationRequested)
        {
            // read any new bytes
            if (_port.BytesToRead > 0)
            {
                var bytesToRead = _port.BytesToRead;
                var bytesRead = new byte[bytesToRead];
                _port.Read(bytesRead, 0, bytesToRead);

                foreach (var b in bytesRead)
                {
                    readBuffer.Enqueue(b);
                }
            }
            else
            {
                Thread.Sleep(16);
            }

            // we need at least 5 bytes to start processing a frame
            if (readBuffer.Count < 5)
            {
                continue;
            }

            // Process any frames we might have
            var firstByte = readBuffer.Peek();

            if (firstByte != frameStartByte)
            {
                // the first byte isn't the start of the frame. If the buffer is full of garbage, we need to clear it out until we find a start byte.
                if (readBuffer.Count > 100)
                {
                    while (firstByte != frameStartByte && readBuffer.Count > 0)
                    {
                        readBuffer.Dequeue();

                        if (readBuffer.Count > 0)
                        {
                            firstByte = readBuffer.Peek();
                        }
                    }
                }
                else
                {
                    // just wait for more data then.
                    continue;
                }
            }

            // we should now be at the start of a frame
            if (readBuffer.Count < 5)
            {
                // not enough data to read length yet
                continue;
            }

            while (readBuffer.Count >= 5)
            {
                var firstFive = readBuffer.DequeueMultiple(5);

                // +1 for the command byte which isn't included in the length.
                var additionalRequiredBytes = ParseHeaderLength(firstFive);

                var framePayload = new List<byte>(firstFive);

                var bufferAvailableBytes = Math.Min(readBuffer.Count, additionalRequiredBytes);
                var portReadBytes = additionalRequiredBytes - bufferAvailableBytes;

                for (int i = 0; i < bufferAvailableBytes; i++)
                {
                    framePayload.Add(readBuffer.Dequeue());
                }

                if (portReadBytes > 0)
                {
                    while (_port.BytesToRead < portReadBytes)
                    {
                        Thread.Sleep(5);
                    }

                    for (int i = 0; i < portReadBytes; i++)
                    {
                        framePayload.Add((byte) _port.ReadByte());
                    }
                }

                // now we should have a full frame in framePayload
                _frameReceived?.Invoke(this, framePayload.ToArray());
            }
        }
    }

    /// <summary>
    /// Reads the first 5 bytes of a response and parses the length from it. You can provide a larger byte array, but only the first 5 bytes are used. Note the returned length is excluding the command byte, so the remaining bytes to dequeue will be the returned value plus 1.
    /// </summary>
    private int ParseHeaderLength(byte[] firstFiveBytes)
    {
        if (firstFiveBytes[0] != frameStartByte)
        {
            throw new ArgumentException("Response does not start with frame start byte");
        }

        var length = BitConverter.ToUInt16(firstFiveBytes, 1);

        var xorLength = BitConverter.ToUInt16(firstFiveBytes, 3);

        if ((length ^ LengthXor) != xorLength)
        {
            throw new ArgumentException("Response length XOR mismatch");
        }

        return length;
    }

    private ParsedResponse WaitForResponse(bool skipWait = false)
    {
        if (!skipWait)
        {
            while (!(_bytesWaiting))
            {
                // wait
                Thread.Sleep(16);
            }
        }

        var response = ParseResponse(_lastPayload);

        _bytesWaiting = false;

        if (response.Command == Commands.Error)
        {
            var respondingTo = (Commands) response.CommandData[0];
            var layerByte = response.CommandData[1];
            var errorCode = (ErrorCodes) response.CommandData[2];
            throw new DeviceErrorException(respondingTo, layerByte, errorCode);
        }

        return response;
    }

    private ParsedResponse ParseResponse(byte[] commandData)
    {
        if (commandData.Length < 6) // minimum length with no data is 6 bytes
        {
            throw new DataLengthException("Response data too short");
        }

        var length = ParseHeaderLength(commandData);

        if (commandData.Length != length + 5) // +4 for start byte and length bytes. Note the last 2 bytes are CRC
        {
            throw new DataLengthException("Response length does not match actual data length");
        }

        var dataLength = length - 2; // -2 for CRC bytes
        var data = new byte[dataLength];

        Array.Copy(
            commandData,
            5,
            data,
            0,
            dataLength
        );

        var crc = BitConverter.ToUInt16(commandData, commandData.Length - 2);
        var computedCrc = UartCrcCalculator.ComputeChecksum(data);

        if (crc != computedCrc)
        {
            throw new ArgumentException("Response CRC mismatch");
        }

        var command = (Commands) data[0];
        return new ParsedResponse(command, data.Skip(1).ToArray());
    }

    public EventHandler<Tag> TagRead { get; set; }

    private Tag ParseGetUidAsyncResponse(ParsedResponse response)
    {
        var commandData = response.CommandData;

        var command = (Commands) commandData[0];

        if (command != Commands.GetTagUid)
        {
            throw new ArgumentException("PollAsync response is not a GetUid response");
        }

        var cardTypeByte = commandData[1];
        var cardType = (CardType) cardTypeByte;

        var tagTypeByte = commandData[2];
        var tagType = (TagType) tagTypeByte;

        // this bit is undocumented :)
        var antennaId = commandData[3];
        var antennaIdInt = (int) antennaId;

        //okay and now we're documented again
        var uid = commandData[4..];

        var tag = new Tag(cardType, tagType, uid, antennaIdInt);
        return tag;
    }

#region "Commands"

    public ParsedResponse SendCommandRaw(byte[] command)
    {
        SendCommand(command);
        return WaitForResponse();
    }

    public void DummyCommand()
    {
        SendCommand([(byte) Commands.Dummy]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge && response.AcknowledgeRespondsTo != Commands.Dummy)
        {
            throw new ArgumentException("Did not receive acknowledge for dummy command");
        }
    }

    public string GetFirmwareVersion()
    {
        SendCommand([(byte) Commands.GetVersion]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.GetVersion)
        {
            throw new ArgumentException("Did not receive firmware version response");
        }

        var versionString = System.Text.Encoding.ASCII.GetString(response.AcknowledgeReponseCommand);
        return versionString;
    }

    public int GetTagCount()
    {
        SendCommand([(byte) Commands.GetTagCount]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.GetTagCount)
        {
            throw new ArgumentException("Did not receive tag count response");
        }

        var tagCount = response.AcknowledgeReponseCommand[0];

        return tagCount;
    }

    public void SetPollingActive(bool active)
    {
        SendCommand([(byte) Commands.SetPollingMode, (byte) (active ? 1 : 0)]);

        if (!active)
        {
            _pollingEnabled = active;
        }

        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.SetPollingMode)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling active command");
        }

        if (active)
        {
            _pollingEnabled = active;
        }
    }

    public void SetPollingSupportedTechnologies(SupportedCardTypes types)
    {
        var techByte = (byte) types;
        SendCommand([(byte) Commands.PollingConfig, (byte) PollingConfigCommands.SupportedCardTypes, techByte]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling supported technologies command");
        }
    }

    public SupportedCardTypes GetPollingSupportedTechnologies()
    {
        throw new NotImplementedException();
    }

    public void SetPollingRfidPowerLevel(RfidPowerLevel level)
    {
        var levelByte = (byte) level;
        SendCommand([(byte) Commands.PollingConfig, (byte) PollingConfigCommands.RfidPower, levelByte]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling RFID power level command");
        }
    }

    public RfidPowerLevel GetPollingRfidPowerLevel()
    {
        throw new NotImplementedException();
    }

    public void SetPollingTimeout(ushort timeout)
    {
        var timeoutBytes = BitConverter.GetBytes(timeout);

        SendCommand(
            [
                (byte) Commands.PollingConfig, (byte) PollingConfigCommands.PollingTimeout, timeoutBytes[0],
                timeoutBytes[1]
            ]
        );

        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling timeout command");
        }
    }

    public ushort GetPollingTimeout()
    {
        throw new NotImplementedException();
    }

    public void SetPollingIgnoreTimeout(ushort timeout)
    {
        var timeoutBytes = BitConverter.GetBytes(timeout);

        SendCommand(
            [
                (byte) Commands.PollingConfig, (byte) PollingConfigCommands.IgnoreTimeout, timeoutBytes[0],
                timeoutBytes[1]
            ]
        );

        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling ignore timeout command");
        }
    }

    public ushort GetPollingIgnoreTimeout()
    {
        throw new NotImplementedException();
    }

    public void SetPollingAntennas(ActiveAntennasMux antennas)
    {
        var antennasByte = (byte) antennas;
        SendCommand([(byte) Commands.PollingConfig, (byte) PollingConfigCommands.ActiveAntennasMux, antennasByte]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling antennas command");
        }
    }

    public ActiveAntennasMux GetPollingAntennas()
    {
        throw new NotImplementedException();
    }

    public void SetPollingEventPacket(PollingEventDataMode mode)
    {
        // Note the device wants this set for both "known" and "unknown" types, even though everything will be unknown, so we have to send tit twice.
        var modeByte = (byte) mode;
        SendCommand([(byte) Commands.PollingConfig, (byte) PollingConfigCommands.EventDataMode, 1, modeByte]);
        var response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling event packet command");
        }

        SendCommand([(byte) Commands.PollingConfig, (byte) PollingConfigCommands.EventDataMode, 0, modeByte]);
        response = WaitForResponse();

        if (response.Command != Commands.Acknowledge || response.AcknowledgeRespondsTo != Commands.PollingConfig)
        {
            throw new ArgumentException("Did not receive acknowledge for set polling event packet command");
        }
    }

    public PollingEventDataMode GetPollingEventPacket()
    {
        throw new NotImplementedException();
    }

#endregion
}
