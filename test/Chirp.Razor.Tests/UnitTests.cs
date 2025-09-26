namespace Chirp.Razor.Tests;

public class UnitTests
{

    [Theory]
    [InlineData(0, "01/01/1970 00:00:00")]
    [InlineData(-1758894113, "07/04/1914 10:18:07")]
    [InlineData(1690895308, "01/08/2023 13:08:28")]
    public void TestUnixTimeStampToDateTimeString(long timestamp, string expected)
    {
        Assert.Equal(expected, CheepServiceUtils.UnixTimeStampToDateTimeString(timestamp));
    }
}