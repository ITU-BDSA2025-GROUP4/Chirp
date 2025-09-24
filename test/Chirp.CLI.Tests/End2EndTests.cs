namespace Chirp.CLI.Tests;

using Xunit;
using Chirp.Cli;
using SimpleDB;
using APICore;
using Chirp.Types;
using System.Diagnostics;

    
using System.Net.Http;
using System.Net.Http.Json;




public class End2EndTests : IDisposable
{

    private readonly Process _process;
    public End2EndTests()
    {
        _process = new Process();
        _process.StartInfo.FileName = "dotnet";
        
        var projectPath = Path.GetFullPath("../../../../../src/Chirp.API/Chirp.API.csproj");
        var csvPath = Path.GetFullPath("../../../chirp_cli_db.csv");
        
        _process.StartInfo.Arguments = $"run --project {projectPath} --urls=http://localhost:5000/ --path {csvPath}";
        _process.Start();
        
        Thread.Sleep(1000);
    }
   
    [Fact]
    public void TestReadCheeps()
    {
        var args = new string[] { "read" };
        ConsoleListener.Listen();

        var result = ChirpMain.Main(args);
        var output = ConsoleListener.Export();

        Assert.Equal(0, result);
        
        var expectedResult = "ropf @ 01-08-23 14:09:20: Hello, BDSA students!\r\nadho @ 02-08-23 14:19:38: Welcome to the course!\r\nadho @ 02-08-23 14:37:38: I hope you had a good summer.\r\nropf @ 02-08-23 15:04:47: Cheeping cheeps on Chirp :)\r\n";
        Assert.Equal(expectedResult, output);
    }
        
        //todo add testcheep, logger error atm
        [Fact]
        public void TestCheep()
        {
        }

        public void Dispose()
        {
            if (!_process.HasExited)
            {
                _process.Kill(true);
                _process.WaitForExit();
            }

            _process.Dispose();
        }
}