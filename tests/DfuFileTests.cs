namespace DfuReader.Tests;

public class DfuFileTests
{
    [Fact]
    public void Ctor_Stream1()
    {
        using var stream = File.OpenRead("assets\\PeachyPrinter\\peachyprintertools\\peachyprinter-firmware-0.1.241.dfu");
        var dfu = new DfuFile(stream);
        Assert.Equal(1, dfu.Version);
        Assert.Equal(26673u, dfu.ImageSize);
        Assert.Single(dfu.Images);
        Assert.True(dfu.Images[0].IsTargetNamed);
        Assert.Equal("Internal Flash", dfu.Images[0].TargetName);
        Assert.Equal(0x00, dfu.Images[0].AlternateSetting);
        Assert.Single(dfu.Images[0].Elements);
        Assert.Equal(0x08000000u, dfu.Images[0].Elements[0].Address);
        Assert.Equal(26380, dfu.Images[0].Elements[0].Size);
        Assert.Equal(26380, dfu.Images[0].Elements[0].Data.Length);
        Assert.Equal(0x011A, dfu.DFUVersion);
        Assert.Equal(0x0483, dfu.VendorID);
        Assert.Equal(0xDF11, dfu.ProductID);
        Assert.Equal(0x2200, dfu.DeviceNumber);
    }
    
    [Fact]
    public void Ctor_Stream2()
    {
        using var stream = File.OpenRead("assets\\REVrobotics\\DfuSeFile\\TestDFU.dfu");
        var dfu = new DfuFile(stream);
        Assert.Equal(1, dfu.Version);
        Assert.Equal(71565u, dfu.ImageSize);
        Assert.Single(dfu.Images);
        Assert.True(dfu.Images[0].IsTargetNamed);
        Assert.Equal("SPARK MAX Firmware - V1.4.0\0PARK-MAX-FW-v1.4.0.dfu", dfu.Images[0].TargetName);
        Assert.Equal(0x00, dfu.Images[0].AlternateSetting);
        Assert.Single(dfu.Images[0].Elements);
        Assert.Equal(0x08000000u, dfu.Images[0].Elements[0].Address);
        Assert.Equal(71272, dfu.Images[0].Elements[0].Size);
        Assert.Equal(71272, dfu.Images[0].Elements[0].Data.Length);
        Assert.Equal(0x011A, dfu.DFUVersion);
        Assert.Equal(0x0483, dfu.VendorID);
        Assert.Equal(0xDF11, dfu.ProductID);
        Assert.Equal(0x1400, dfu.DeviceNumber);
    }

    [Fact]
    public void Ctor_EmptyStream_ThrowsFormatException()
    {
        using var stream = new MemoryStream();
        Assert.Throws<FormatException>(() => new DfuFile(stream));
    }

    [Fact]
    public void Ctor_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("stream", () => new DfuFile(null!));
    }
}