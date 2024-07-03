using System.Buffers.Binary;

namespace DfuReader;

/// 2.3.3 Image Element
/// The Image element structured as follows, provides a data record containing the effective
/// firmware data preceded by the data address and data size. The Image Element buffer is
/// represented in Big Endian order.
public struct DfuImageElement
{
    private uint dwElementAddress;
    private uint dwElementSize;
    private byte[] _Data;

    public uint Address => dwElementAddress;
    public int Size => (int)dwElementSize;
    public ReadOnlySpan<byte> Data => _Data;

    public DfuImageElement(Stream stream)
    {
        // dwElementAddress (4 bytes)
        // The dwElementAddress field gives the 4-byte starting address of the data.
        Span<byte> address = stackalloc byte[4];
        stream.Read(address);
        dwElementAddress = BinaryPrimitives.ReadUInt32LittleEndian(address);

        // dwElementSize (4 bytes)
        // The dwElementSize field gives the size of the contained data.
        Span<byte> size = stackalloc byte[4];
        stream.Read(size);
        dwElementSize = BinaryPrimitives.ReadUInt32LittleEndian(size);

        // Data (dwElementSize bytes)
        // The Data field present the effective data. 
        _Data = new byte[dwElementSize];
        stream.Read(_Data);
    }
}
