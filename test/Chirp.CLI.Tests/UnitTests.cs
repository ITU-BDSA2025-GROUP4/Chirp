namespace Chirp.CLI.Tests;

using Xunit;
using Chirp.Cli;
using SimpleDB;
using APICore;
using Chirp.Types;
using System.Diagnostics;

    
using System.Net.Http;
using System.Net.Http.Json;




public class UnitTests
{
    [Theory]
    [InlineData(0, "01/01/70 01:00:00")]
    [InlineData(-1758894113, "07/04/14 12:18:07")]
    [InlineData(1690895308, "01/08/23 15:08:28")]
    public void TestUnixTimeStampToDateTimeString(long timestamp, string expected)
    {
        Assert.Equal(expected, UserInterface.FormatTimestamp(timestamp));
    }
}