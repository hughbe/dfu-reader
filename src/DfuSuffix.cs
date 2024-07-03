using System.Buffers.Binary;

namespace DfuReader;

/// 2.2 DFU Suffix section 
/// The DFU Suffix, as specified in the DFU specification, allows the host software to detect and
/// prevent attempts to download incompatible firmware. The Suffix buffer is represented in
/// Little Endian order.
internal unsafe struct DfuSuffix
{
    public ushort bcdDevice;
    public ushort idProduct;
    public ushort idVendor;
    public ushort bcdDFU;
    public fixed byte ucDfuSignature[3];
    public byte bLength;
    public uint dwCRC;

    public DfuSuffix(Stream stream)
    {
        // bcdDevice (2 bytes)
        // The bcdDevice field gives the device release number.
        Span<byte> device = stackalloc byte[2];
        stream.Read(device);
        bcdDevice = BinaryPrimitives.ReadUInt16LittleEndian(device);

        // idProduct (2 bytes)
        // The idProduct and idVendor field give the Product ID and Vendor ID respectively of the
        // device that the file is intended for, or 0xFFFF if the field is ignored.
        Span<byte> product = stackalloc byte[2];
        stream.Read(product);
        idProduct = BinaryPrimitives.ReadUInt16LittleEndian(product);

        // idVendor (2 bytes)
        // The idProduct and idVendor field give the Product ID and Vendor ID respectively of the
        // device that the file is intended for, or 0xFFFF if the field is ignored.
        Span<byte> vendor = stackalloc byte[2];
        stream.Read(vendor);
        idVendor = BinaryPrimitives.ReadUInt16LittleEndian(vendor);

        // bcdDFU (2 bytes)
        // The bcdDFU field, fixed to 0x011A, gives the DFU specification number. This value
        // differs from that specified in standard DFU rev1.1.
        Span<byte> dfu = stackalloc byte[2];
        stream.Read(dfu);
        bcdDFU = BinaryPrimitives.ReadUInt16LittleEndian(dfu);
        if (bcdDFU != 0x011A)
        {
            throw new FormatException("Invalid DFU suffix version.");
        }

        // ucDfuSignature (3 bytes)
        // The ucSignature field contains a fixed string of three unsigned characters (44h, 46h,
        // 55h). In the file they appear in reverse order, allowing valid DFU files to be recognized.
        fixed (byte* pSignature = ucDfuSignature)
        {
            var buffer = new Span<byte>(pSignature, 3);
            stream.Read(buffer);
            if (!buffer.SequenceEqual("UFD"u8))
            {
                throw new FormatException("Invalid DFU suffix signature.");
            }
        }

        // bLength (1 byte)
        // The bLength field, currently fixed to 16, gives the length of the DFU Suffix itself in bytes.
        bLength = (byte)stream.ReadByte();
        if (bLength != 16)
        {
            throw new FormatException("Invalid DFU suffix length.");
        }

        // dwCRC (4 bytes)
        // The dwCRC field gives the CRC of the whole file, excluding the CRC field itself.
        Span<byte> crc = stackalloc byte[4];
        stream.Read(crc);
        dwCRC = BinaryPrimitives.ReadUInt32LittleEndian(crc);
    }
}
