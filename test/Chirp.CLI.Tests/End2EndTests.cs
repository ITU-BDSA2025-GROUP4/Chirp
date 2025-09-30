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

    private readonly Process _apiProcess;
    public End2EndTests()
    {
        _apiProcess = new Process();
        _apiProcess.StartInfo.FileName = "dotnet";
        
        var projectPath = Path.GetFullPath("../../../../../src/Chirp.API/Chirp.API.csproj");
        var csvPath = Path.GetFullPath("../../../chirp_cli_db.csv");
        
        _apiProcess.StartInfo.Arguments = $"run --project {projectPath} --urls=http://localhost:5000/ --path {csvPath}";
        _apiProcess.Start();
        
        // Waits with proceeding till our API is actually up and running
        var waitTime = TimeSpan.FromSeconds(20);
        var stopwatch = Stopwatch.StartNew();
        
        using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        while (stopwatch.Elapsed < waitTime)
        {
            try
            {
                var res = client.GetAsync("/cheeps").Result;

                if (res.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch { /* If we get an error, the API process has not started */ }

            Thread.Sleep(500);
        }
        
        throw new TimeoutException("Chirp.API did not start within 20 seconds");
    }
    
    // This code is taking from the lecture slides: https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_03/Slides.md
    private string RunCliCommand(string command)
    {
        string output = "";
        using (var process = new Process()) 
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"run --project ../../../../../src/Chirp.CLI/Chirp.CLI.csproj -- {command}";            
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }

        return output;
    }
   
    [Fact]
    public void TestReadCheeps()
    {
        var output = RunCliCommand("read");
        var expectedResult = "ropf @ 01/08/23 12:09:20: Hello, BDSA students!adho @ 02/08/23 12:19:38: Welcome to the course!adho @ 02/08/23 12:37:38: I hope you had a good summer.ropf @ 02/08/23 13:04:47: Cheeping cheeps on Chirp :)";
        output = output.Replace("\n", "").Replace("\r", "").Replace("\t", "");
        Assert.Equal(expectedResult, output);
    }

    [Fact]
    public void TestCheep()
    {
        var author = "testuser";
        var message = $"Hello from the end-to-end test! {DateTimeOffset.Now.ToUnixTimeSeconds()}";
        var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        var url = $"/cheep?author={Uri.EscapeDataString(author)}&message={Uri.EscapeDataString(message)}&timestamp={timestamp}";
        
        var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
        var response = client.GetAsync(url).Result;
        Assert.True(response.IsSuccessStatusCode);
        var responseString = response.Content.ReadAsStringAsync().Result;
        Assert.Equal("Cheep'ed", responseString);
        
        var output = RunCliCommand("read");
        var expectedOutputSubstring = author + " @ " + DateTimeOffset.FromUnixTimeSeconds(timestamp).ToString("dd/MM/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) + ": " + message;
        Assert.Contains(expectedOutputSubstring, output);
        
    }
    
    public void Dispose()
    {
        if (_apiProcess != null)
        {
            if (!_apiProcess.HasExited)
            {
                _apiProcess.Kill(true);
                _apiProcess.WaitForExit();
            }

            _apiProcess.Dispose();
        }
    }
}