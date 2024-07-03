# DFUReader
- A .NET library to read DFU files.

## Example Usage
```cs
using var stream = File.OpenRead("<path-to-file>.dfu");
var dfu = new DfuFile(stream);
Console.WriteLine($"Version: {dfu.Version}");
Console.WriteLine($"Image Size: {dfu.ImageSize}");
Console.WriteLine($"Images: {dfu.Images.Count}");
for (int i = 0; i < dfu.Images.Count; i++)
{
    Console.WriteLine($"- [{0}] Target Named: {dfu.Images[i].IsTargetNamed}");
    Console.WriteLine($"- [{0}] Target Name: {dfu.Images[i].TargetName}");
    Console.WriteLine($"- [{0}] Target Size: {dfu.Images[i].TargetSize}");
    Console.WriteLine($"- [{0}] Elements: {dfu.Images[i].Elements.Count}");
    for (int j = 0; j < dfu.Images[i].Elements.Count; j++)
    {
        Console.WriteLine($"  - [{0}] Element Address: 0x{dfu.Images[i].Elements[j].Address:X8}");
        Console.WriteLine($"  - [{0}] Element Size: {dfu.Images[i].Elements[j].Size}");
    }
}
```
