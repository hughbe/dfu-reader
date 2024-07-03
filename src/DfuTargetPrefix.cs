using System.Buffers.Binary;
using System.Text;

namespace DfuReader;

/// 2.3.2 Target Prefix
/// The target prefix record is used to describe the associated image. The Target Prefix buffer is
/// represented in Big Endian order.
internal unsafe struct DfuTargetPrefix
{
    public fixed byte szSignature[6];
    public byte bAlternateSetting;
    public uint bTargetNamed;
    public fixed byte szTargetName[255];

    public string TargetName
    {
        get
        {
            fixed (byte* pTargetName = szTargetName)
            {
                return Encoding.ASCII.GetString(pTargetName, 255).TrimEnd('\0');
            }
        }
    }

    public uint dwTargetSize;
    public uint dwNbElements;

    public DfuTargetPrefix(Stream stream)
    {
        // szSignature "Target" (6 bytes)
        // The szSignature field, 6-byte coded, fixed to “Target”.
        fixed (byte* pSignature = szSignature)
        {
            var buffer = new Span<byte>(pSignature, 6);
            stream.Read(buffer);
            if (!buffer.SequenceEqual("Target"u8))
            {
                throw new FormatException("Invalid DFU target signature.");
            }
        }

        // bAlternateSetting (1 byte)
        // The bAlternateSetting field gives the device alternate setting for which the associated
        // image can be used.
        bAlternateSetting = (byte)stream.ReadByte();

        // bTargetNamed (4 bytes)
        // The bTargetNamed field is a boolean value which indicates if the target is named or
        // not.
        Span<byte> targetNamed = stackalloc byte[4];
        stream.Read(targetNamed);
        bTargetNamed = BinaryPrimitives.ReadUInt32LittleEndian(targetNamed);

        // szTargetName (255 bytes)
        // The szTargetName field gives the target name.
        fixed (byte* pTargetName = szTargetName)
        {
            var buffer = new Span<byte>(pTargetName, 255);
            stream.Read(buffer);
        }

        // dwTargetSize (4 bytes)
        // The dwTargetSize field gives the whole length of the associated image excluding the
        // Target prefix.
        Span<byte> targetSize = stackalloc byte[4];
        stream.Read(targetSize);
        dwTargetSize = BinaryPrimitives.ReadUInt32LittleEndian(targetSize);

        // dwNbElements (4 bytes)
        // The dwNbElements field gives the number of elements in the associated image.
        Span<byte> nbElements = stackalloc byte[4];
        stream.Read(nbElements);
        dwNbElements = BinaryPrimitives.ReadUInt32LittleEndian(nbElements);
    }
}
