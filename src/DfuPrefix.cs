using System.Buffers.Binary;

namespace DfuReader;

/// 2.1 DFU Prefix section
/// The DFU prefix placed as a header file is the first part read by the software application, used
/// to retrieve the file context, and enable valid DFU files to be recognized. The Prefix buffer is
/// represented in Big Endian order.
internal unsafe struct DfuPrefix
{
    public fixed byte szSignature[5];
    public byte bVersion;
    public uint DFUImageSize;
    public byte bTargets;

    public DfuPrefix(Stream stream)
    {
        // szSignature "DfuSe" (5 bytes)
        // The szSignature field, five-byte coded, presents the file identifier that enables valid
        // DFU files to be recognized, and incompatible changes detected. This identifier should
        // be updated when major changes are made to the file format. This field is set to “DfuSe”.
        fixed (byte* pSignature = szSignature)
        {
            var buffer = new Span<byte>(pSignature, 5);
            stream.Read(buffer);      
            if (!buffer.SequenceEqual("DfuSe"u8))
            {
                throw new FormatException("Invalid DFU file signature.");
            }
        }

        // bVersion (0x01) (1 byte)
        // The bVersion field, one-byte coded, presents the DFU format revision, The value will be
        // incremented if extra fields are added to one of the three sections. Software exploring
        // the file can either treat the file depending on its specified revision, or just test for valid
        // value.
        byte version = (byte)stream.ReadByte();
        if (version != 0x01)
        {
            throw new FormatException("Invalid DFU file version.");
        }

        bVersion = version;

        // DFUImageSize (4 bytes)
        // The DFUImageSize field, four-byte coded, presents the total DFU file length in bytes.
        Span<byte> imageSize = stackalloc byte[4];
        stream.Read(imageSize);
        DFUImageSize = BinaryPrimitives.ReadUInt32LittleEndian(imageSize);

        // bTargets (1 byte)
        // the bTargets field, one-byte coded, presents the number of DFU image stored in the
        // file.
        bTargets = (byte)stream.ReadByte();
    }
}