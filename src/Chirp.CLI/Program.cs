namespace Chirp.Cli;

using DocoptNet;
using Utils;

using Version = MetaData.Version;

using SimpleDB;
using Chirp.Types;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;

public static class UserInterface
{
    private const string timeFormat = "dd/MM/yy HH:mm:ss";

    public static string FormatTimestamp(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).ToString(timeFormat, System.Globalization.CultureInfo.InvariantCulture);
    }
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps)
        {
            Console.WriteLine(cheep.Author + " @ " + FormatTimestamp(cheep.Timestamp) + ": " + cheep.Message);
        }
    }
}

enum BatchResult
{
    Stop,
    Continue
}


public static class ChirpMain
{
    static void ChirpExit(int statusCode)
    {
        Logger.get.Dispose();
    }
    // string that contains the help message
    private const string help = @"Chirp.CLI.
    Usage:
      Chirp.CLI interactive [--azure]
      Chirp.CLI read [--azure]
      Chirp.CLI chirp <text> [--azure]
      Chirp.CLI (-h | --help)
      Chirp.CLI --version

    Options:
      -h --help     Show this screen.
      --version     Show version.

    ";

    private static HttpClient? client = null;

    private static void CreateHttpClient(bool useAzure)
    {
        client = new()
        {
        BaseAddress = new Uri(
                useAzure ?
                "https://bdsagroup4chirpremotedb-fsh6avgedzbpene2.norwayeast-01.azurewebsites.net"
                :
                "http://localhost:5000"
                )
        };
    }


    private static void Read()
    {
        if(client == null)
        {
            throw new Exception("HTTP Client is null, this shouldn't have happened");
        }

        try
        {
            List<Cheep>? result = client.GetFromJsonAsync<List<Cheep>>("/cheeps").Result;
            if (result != null)
            {
                UserInterface.PrintCheeps(result);
            }
        }
        catch (FileNotFoundException e)
        {
            Logger.get.LogWarn(string.Format("Database file not found: '{0}'", e));
        }
        catch (Exception e)
        {
            Logger.get.LogWarn(string.Format("Error parsing database: '{0}'", e));
            Console.WriteLine(e);
            throw;
        }
    }

    private static void Chirp(string message)
    {
        if(client == null)
        {
            throw new Exception("HTTP Client is null, this shouldn't have happened");
        }

        string name = Environment.UserName;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        string url = string.Format("/cheep?author={0}&message={1}&timestamp={2}", name, message,
            timestamp);

        Task<HttpResponseMessage> response = client.GetAsync(url);

        response.Wait();
    }

    private static void helpfunc()
    {
        Console.WriteLine(
            "Commands:\nchirp [MESSAGE] | Chirps a message\nread | Displays all chirps\n? | Displays this menu\nexit | Exits Chirp.CLI\n");
    }

    // @Obselete
    // Used by interactive
    static BatchResult batch(string[] args)
    {
        string command = args[0];

        switch (command)
        {
            case "read":
                Read();
                break;
            case "chirp":
                if (args.Length < 2)
                {
                    Console.WriteLine("Chirp requires a message");
                    break;
                }

                Chirp(args[1]);
                break;
            case "?":
                helpfunc();
                break;
            case "exit":
                return BatchResult.Stop;
            default:
                Logger.get.Log(string.Format("User wrote unknown command: {0}", command));
                Console.WriteLine("Unknown command {0}, use '?' for help", command);
                break;
        }

        return BatchResult.Continue;
    }

    private static void interactive()
    {
        Console.WriteLine("Running Chirp.CLI in interactive mode, type ? to re-display help");
        helpfunc();

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                break;
            }

            string[] tokens = input.TrimEnd().TrimStart().Split(" ", 2);
            if (tokens.Length == 0)
            {
                continue;
            }

            var result = batch(tokens);

            if (result == BatchResult.Stop)
            {
                break;
            }
        }
    }

    // 3 methods from DocoptNet docs
    // https://docopt.github.io/docopt.net/dev/#api
    private static int ShowHelp(string help)
    {
        Console.WriteLine(help);
        return 0;
    }

    private static int ShowVersion(string version)
    {
        Console.WriteLine(version);
        return 0;
    }

    private static int OnError(string usage)
    {
        Console.WriteLine(usage);
        return 1;
    }

    // Method to activate diffrent parts of the program
    private static int Run(IDictionary<string, ArgValue> arguments)
    {
        bool chirpbool = false;
        bool useAzure = false;

        if((bool)arguments["--azure"])
        {
            useAzure = true;
        }
        CreateHttpClient(useAzure);

        foreach ((string key, ArgValue value) in arguments)
        {

            if (key == "interactive" && (bool)value)
            {
                interactive();
            }

            if (key == "read" && (bool)value)
            {
                Read();
            }

            if (key == "chirp" && (bool)value)
            {
                chirpbool = true;
            }

            if (key == "<text>" && chirpbool)
            {
                Chirp((string)value);
            }
            //Console.WriteLine("{0} = {1}", key, value);
        }

        return 0;
    }

    private static int Main(string[] args)
    {
        // Uncomment the line below in order to disable all logging
        //        Logger.get.Disable();
        Docopt.CreateParser(help)
            .WithVersion(Version.version)
            .Parse(args)
            .Match(Run,
                result => ShowHelp(result.Help),
                result => ShowVersion(result.Version),
                result => OnError(result.Usage)
            );

        ChirpExit(0);
        return 0;
    }
}