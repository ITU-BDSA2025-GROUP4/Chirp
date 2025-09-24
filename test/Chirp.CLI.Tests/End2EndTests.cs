namespace Chirp.CLI.Tests;

using Xunit;

using Chirp.Cli;

public class End2EndTsests
{

    [Fact]
    public void TestReadCheeps()
    {
        var args = new string[] { "chirp", "read" };

    }

    [Fact]
    public void TestReadTenCheeps()
    {
        var args = new string[] { "chirp", "read", "10" };
    }

    [Fact]
    public void TestCheep()
    {
        var args = new string[] { "chirp", "Hello!!!" };
        ChirpMain.Main(args);
    }
}