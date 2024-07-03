namespace DfuReader;

public class DfuFile
{
    private readonly DfuPrefix _prefix;
    private readonly DfuSuffix _suffix;

    public byte Version => _prefix.bVersion;
    public uint ImageSize => _prefix.DFUImageSize;

    public IReadOnlyList<DfuImage> Images { get; }

    public ushort DFUVersion => _suffix.bcdDFU;

    public ushort VendorID => _suffix.idVendor;
    public ushort ProductID => _suffix.idProduct;
    public ushort DeviceNumber => _suffix.bcdDevice;

    public unsafe DfuFile(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        int startPosition = (int)stream.Position;

        // The basic DFU file format to be used with STMicroelectronics DFU solution is based on
        // three sections; Prefix, Images and Suffix, described as follows:
        // Figure 1. DFU File general format
        // DFU PREFIX
        // DFU Images
        // DFU SUFFIX

        // The DFU prefix placed as a header file is the first part read by the software application, used
        // to retrieve the file context, and enable valid DFU files to be recognized. The Prefix buffer is
        // represented in Big Endian order.
        _prefix = new DfuPrefix(stream);

        // The images section placed between the prefix and suffix as a body of the DFU file, contains
        // a list of DFU images indexed by the alternate setting.
        // Figure 4. DFU Images*
        // DFU Image 1
        // DFU Image 2
        // ....
        // DFU Image N-1
        // DFU Image N
        var images = new List<DfuImage>(_prefix.bTargets);
        for (int i = 0; i < _prefix.bTargets; i++)
        {
            images.Add(new DfuImage(stream));
        }
        
        Images = images;

        // Validate we read everything.
        if (stream.Position != startPosition + _prefix.DFUImageSize)
        {
            throw new FormatException("Invalid DFU file size.");
        }

        // The DFU Suffix, as specified in the DFU specification, allows the host software to detect and
        // prevent attempts to download incompatible firmware. The Suffix buffer is represented in
        // Little Endian order.
        _suffix = new DfuSuffix(stream);

        int endPosition = (int)stream.Position;

        // Calculate CRC32.
        try
        {
            stream.Position = startPosition;

            Span<byte> crcBuffer = stackalloc byte[256];

            var crc32 = new System.IO.Hashing.Crc32();
            while (stream.Position < endPosition - 4)
            {
                // Read in 256 byte chunks up to the final 4 bytes.
                Span<byte> actualBuffer = crcBuffer.Slice(0, Math.Min(256, endPosition - (int)stream.Position - 4));
                stream.Read(actualBuffer);
                crc32.Append(actualBuffer);
            }
            
            uint expectedHash = (crc32.GetCurrentHashAsUInt32() ^ uint.MaxValue);
            if (_suffix.dwCRC != expectedHash)
            {
                throw new FormatException("Invalid CRC32.");
            }
        }
        finally
        {
            stream.Position = endPosition;
        }
    }
}
