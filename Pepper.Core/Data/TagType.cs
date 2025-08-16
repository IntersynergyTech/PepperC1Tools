// ReSharper disable InconsistentNaming
namespace Pepper.Core.Data;

// These tags are based on the Pepper C1 docs, if any other readers are added then they will have to map them across.
public enum TagType
{
    MifareUltralight = 0x01,
    MifareUltralightC = 0x02,
    MifareClassic = 0x03,
    MifareClassic1k = 0x04,
    MifareClassic4k = 0x05,
    MifarePlus = 0x06,
    MifarePlus2k = 0x07,
    MifarePlus4k = 0x08,
    MifarePlus2k_Sl2 = 0x09,
    MifarePlus4k_Sl2 = 0x0A,
    MifarePlus2k_Sl3 = 0x0B,
    MifarePlus4k_Sl3 = 0x0C,
    MifareDesfire = 0x0D,

    Jcop = 0x0F,

    MifareMini = 0x10,

    ICodeSli = 0x21,
    ICodeSli_S = 0x22,
    ICodeSli_L = 0x23,
    ICodeSliX = 0x24,
    ICodeSliX_S = 0x25,
    ICodeSliX_X = 0x26,
    ICodeSliX2 = 0x27,
    ICodeDna = 0x28,

    WpanLEDeviceUid = 0x42,
    WpanLePin = 0x50,
    
    DummyTag = 0xFF
}
