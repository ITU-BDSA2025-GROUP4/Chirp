namespace Chirp.CLI.Tests;

using Xunit;
using Chirp.Cli;


public class End2EndTests
{

    [Fact]
    public void TestReadCheeps()
    {
        var expectedResult = "ropf @ 08/01/23 14:09:20: Hello, BDSA students!/nrnie @ 08/02/23 14:19:38: Welcome to the course!/nrnie @ 08/02/23 14:37:38: I hope you had a good summer./nropf @ 08/02/23 15:04:47: Cheeping cheeps on Chirp :)";

        var args = new string[] { "read" };
        ConsoleListener.Listen();

        ChirpMain.Main(args);
        var output = ConsoleListener.Export();

        Assert.Equal(expectedResult, output);

        //nu assert at det passer med noget forventet :)
    }

    //    [Fact]
    //    public void TestReadTenCheeps()
    //    {
    //        var args = new string[] { "chirp", "read" };
    //        ConsoleListener.Listen();
    //
    //        var result = ChirpMain.Main(args);
    //        Assert.Equal(0, result);
    //
    //        var output = ConsoleListener.Export();
    //
    //        Console.WriteLine(output);
    //        //nu assert at det passer med noget forventet :)
    //    }
    //
    //    [Fact]
    //    public void TestCheep()
    //    {
    //        //var args = new string[] { "chirp", "Hello!!!" };
    //        //Assert.Equal(0, ChirpMain.Main(args));
    //
    //        // Now read and see if the chirp we added is there
    //    }
}