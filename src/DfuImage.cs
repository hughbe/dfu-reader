namespace DfuReader;

public class DfuImage
{
    private DfuTargetPrefix targetPrefix;
    
    public byte AlternateSetting => targetPrefix.bAlternateSetting;
    public bool IsTargetNamed => targetPrefix.bTargetNamed != 0;
    public string TargetName => targetPrefix.TargetName;
    public uint TargetSize => targetPrefix.dwTargetSize;
    public IReadOnlyList<DfuImageElement> Elements { get; }

    internal DfuImage(Stream stream)
    {
        // 2.3.1 DFU Image 
        // The DFU Image contains the effective data of the firmware, starting by a Target prefix record
        // followed by a number of Image elements.
        // Figure 5. DFU Image
        // Target Prefix
        // Image Element 1
        // ImageElement 2
        // ....
        // ImageElement N-1
        // ImageElement N
        targetPrefix = new DfuTargetPrefix(stream);

        long position = stream.Position;

        var elements = new List<DfuImageElement>((int)targetPrefix.dwNbElements);
        for (int i = 0; i < targetPrefix.dwNbElements; i++)
        {
            elements.Add(new DfuImageElement(stream));
        }

        Elements = elements;

        // Ensure that the stream is positioned at the end of the target prefix.
        if (stream.Position != position + targetPrefix.dwTargetSize)
        {
            throw new FormatException("Invalid DFU target size.");
        }
    }
}
