namespace Pepper.Device.C1;

public interface IC1Port
{
    public byte[] SendCommandRaw(byte[] command);
}
