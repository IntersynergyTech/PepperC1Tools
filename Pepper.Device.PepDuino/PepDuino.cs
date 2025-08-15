using System.IO.Ports;
using Pepper.Core.Data;
using Pepper.Core.Devices.C1;

namespace Pepper.Device.PepDuino;

public class PepDuino : IC1Port
{
    private readonly string _serialPortName;
    private Thread _ctorThread;
    private Thread _portThread;
    private CancellationTokenSource _cts;
    private readonly byte[] _readBuffer = new byte[256];
    private int _readBufferIndex = 0;
    private readonly int _waitSpins = 20;
    private int _waitIndex = 0;

    private readonly SerialPort _serialPort;

    public PepDuino(string portName, int baudRate = 9600)
    {
        _serialPortName = portName;
        _ctorThread = Thread.CurrentThread;

        _serialPort = new SerialPort(_serialPortName, baudRate);
    }

    public void Connect()
    {
        if (!_serialPort.IsOpen)
        {
            try
            {
                _serialPort.Open();
                _cts = new CancellationTokenSource();
                _portThread = new Thread(() => PortWatcher(_cts.Token));
                _portThread.Start();
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to open serial port {_serialPortName}: {ex.Message}", ex);
            }
        }
    }

    public void Disconnect()
    {
        _cts.Cancel();

        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
        }
    }

    private void PortWatcher(CancellationToken cancellationToken)
    {
        _serialPort.ReadTimeout = 2000;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_serialPort.BytesToRead <= 0)
                {
                    Thread.Sleep(100);

                    if (_readBufferIndex > 0)
                    {
                        // If we have read some bytes but no new data, reset the buffer index
                        _waitIndex++;
                    }

                    if (_waitIndex > _waitSpins)
                    {
                        _readBufferIndex = 0;
                        _waitIndex = 0;
                    }

                    continue;
                }

                var byteRead = _serialPort.ReadByte();
                ProcessByteRead((byte) byteRead);
            }
            catch (TimeoutException)
            {
                _readBufferIndex = 0;
                // Ignore timeout exceptions, just continue to check for data
                continue;
            }
        }
    }

    private void ProcessByteRead(byte read)
    {
        if (_readBufferIndex >= _readBuffer.Length)
        {
            // Buffer overflow, reset index
            _readBufferIndex = 0;
        }

        _readBuffer[_readBufferIndex++] = read;

        ProcessReadBuffer();
    }

    private void ProcessReadBuffer()
    {
        if (_readBufferIndex < 4)
        {
            // Not enough data to process a command
            return;
        }

        if (_readBuffer[0] != 0x20 || _readBuffer[1] != 0xFF)
        {
            // Invalid start bytes, reset buffer
            _readBufferIndex = 0;
            return;
        }

        // Assuming the command length is in the third byte
        int dataLength = _readBuffer[2];

        // 5 because 2 start bytes, 1 length byte, and 2 end bytes
        if (_readBufferIndex != dataLength + 5)
        {
            // Not enough data to process the command
            return;
        }

        // Process read the bytes
        var bytes = new byte[dataLength];

        Array.Copy(
            _readBuffer,
            3,
            bytes,
            0,
            dataLength
        );

        GenerateTagEvent(bytes);

        _readBufferIndex = 0;
    }

    private void GenerateTagEvent(byte[] bytes)
    {
        var newTag = new Tag(CardType.Iso14443A, TagType.MifareClassic, bytes);

        TagDetected?.Invoke(this, newTag);
    }

    public EventHandler<Tag> TagDetected { get; set; }
}
