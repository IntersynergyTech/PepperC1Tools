using System.IO.Ports;
using Pepper.Core.Control;

namespace Pepper.Device.C1.Ports;

public class Uart : IC1Port
{
    private readonly string _portName;
    private readonly int _baudRate;
    private readonly int _dataBits;
    private readonly Parity _parity;
    private readonly StopBits _stopBits;

    private readonly SerialPort _port;

    private static readonly ushort LengthXor = 0xffff;
    private const byte frameStartByte = 0xF5;

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

        _port = port;
    }

    private void SendCommand(byte[] commandData)
    {
        var frame = BuildCommandFrame(commandData);
        _port.Write(frame, 0, frame.Length);
    }

    private byte[] BuildCommandFrame(byte[] commandData)
    {
        // Start byte
        List<byte> frame = new List<byte> { frameStartByte };

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

    private ParsedResponse ParseResponse(byte[] commandData)
    {
        if (commandData.Length < 6) // minimum length with no data is 6 bytes
        {
            throw new ArgumentException("Response data too short");
        }
        
        if (commandData[0] != frameStartByte)
        {
            throw new ArgumentException("Response does not start with frame start byte");
        }
        
        var length = BitConverter.ToUInt16(commandData, 1);
        
        var xorLength = BitConverter.ToUInt16(commandData, 3);
        if ((length ^ LengthXor) != xorLength)
        {
            throw new ArgumentException("Response length XOR mismatch");
        }
        
        if (commandData.Length != length + 5) // +4 for start byte and length bytes. Note the last 2 bytes are CRC
        {
            throw new ArgumentException("Response length does not match actual data length");
        }
        
        var dataLength = length - 2; // -2 for CRC bytes
        var data = new byte[dataLength];
        Array.Copy(commandData, 5, data, 0, dataLength);
        var crc = BitConverter.ToUInt16(commandData, commandData.Length - 2);
        var computedCrc = UartCrcCalculator.ComputeChecksum(data);
        if (crc != computedCrc)
        {
            throw new ArgumentException("Response CRC mismatch");
        }
        
        var command = (Commands) data[0];
        return new ParsedResponse(command, data.Skip(1).ToArray());
    }

    public byte[] SendCommandRaw(byte[] command)
    {
        SendCommand(command);
    }
}
