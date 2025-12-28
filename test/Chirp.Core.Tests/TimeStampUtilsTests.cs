using Chirp.Core.Utils;

namespace Chirp.Core.Tests.Unit;

public class UnitTests
{

    [Theory]
    [InlineData(0, "01/01/1970 00:00:00")]
    [InlineData(-1758894113, "07/04/1914 10:18:07")]
    [InlineData(1690895308, "01/08/2023 13:08:28")]
    public void TestUnixTimeStampToDateTimeString(long timestamp, string expected)
    {
        Assert.Equal(expected, TimestampUtils.UnixTimeStampToDateTimeString(timestamp));
    }


    [Theory]
    [InlineData("01/01/1970 00:00:00")]
    [InlineData("07/04/1914 10:18:07")]
    [InlineData("01/08/2023 13:08:28")]
    public void TestDateTimeTimeStampToDateTimeString(string dateTimeString)
    {
        DateTime dt = DateTime.ParseExact(dateTimeString, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        string result = TimestampUtils.DateTimeTimeStampToDateTimeString(dt);
        Assert.Equal(dateTimeString, result);
    }

    [Theory]
    [InlineData("01/01/1970 00:00:00")]
    [InlineData("07/04/1914 10:18:07")]
    [InlineData("01/08/2023 13:08:28")]
    public void TestDateTimeStringToDateTimeTimeStamp(string dateTimeString)
    {
        DateTime dt = TimestampUtils.DateTimeStringToDateTimeTimeStamp(dateTimeString);
        string result = dt.ToString("dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(dateTimeString, result);
    }
}