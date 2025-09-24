namespace Chirp.CLI.Tests;

using Xunit;

using Chirp.Cli;

public class End2EndTests
{

    [Fact]
    public void TestReadCheeps()
    {
        var args = new string[] { "chirp", "read" };
        ConsoleListener.Listen();

        //        var result = ChirpMain.Main(args);
        //        Assert.Equal(0, result);
        //
        Console.WriteLine("dk");
        Console.WriteLine("idk");
        var output = ConsoleListener.Export();


        Console.WriteLine(output);

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