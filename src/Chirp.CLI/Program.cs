namespace Chirp.Cli;
using DocoptNet;

using CsvHelper;
using CsvHelper.Configuration;

using System.Globalization;

using Utils;

using MetaData;

using SimpleDB;
using Chirp.Types;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;

// This terrible construction merely exists to enable us to read the output from the application which is needed in the End2End tests of Chirp.CLI
// It works by redirecting stdout to its internal StringWriter. Use with caution.
public static class ConsoleListener
{
    private static StringWriter _buffer = new StringWriter();

    // Redirects stdout to the _buffer 
    public static void Listen()
    {
        Console.SetOut(_buffer);
    }

    // Converts the contets of recorded output to a string
    public static string Export()
    {
        string output = _buffer.ToString();
        return output;
    }
}

public static class UserInterface
{
    private const string timeFormat = "dd/MM/yy HH:mm:ss";
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps)
        {
            DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
            Console.WriteLine(cheep.Author + " @ " + timestamp.ToString(timeFormat) + ": " + cheep.Message);
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

    private static HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5000")
    };

    static void Read()
    {
        try
        {
            var result = client.GetFromJsonAsync<List<Cheep>>("/cheeps").Result;
            if (result != null) UserInterface.PrintCheeps(result);
        }
        catch (FileNotFoundException e)
        {
            Logger.get.LogWarn(String.Format("Database file not found: '{0}'", e.ToString()));
        }
        catch (Exception e)
        {
            Logger.get.LogWarn(String.Format("Error parsing database: '{0}'", e.ToString()));
            Console.WriteLine(e);
            throw;
        }
    }

    static void Chirp(string message)
    {
        string name = Environment.UserName;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        var url = String.Format("/cheep?author={0}&message={1}&timestamp={2}", name, message, timestamp);

        // TODO: This gives response 'Cheep'ed' if successful, we should check this
        var response = client.GetAsync(url);

        response.Wait();

    }

    static void helpfunc()
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
                Logger.get.Log(String.Format("User wrote unknown command: {0}", command));
                Console.WriteLine("Unknown command {0}, use '?' for help", command);
                break;
        }

        return BatchResult.Continue;
    }

    static void interactive()
    {
        Console.WriteLine("Running Chirp.CLI in interactive mode, type ? to re-display help");
        helpfunc();

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();

            if (input == null) break;

            string[] tokens = input.TrimEnd().TrimStart().Split(" ", 2);
            if (tokens.Length == 0) continue;

            var result = batch(tokens);

            if (result == BatchResult.Stop)
            {
                break;
            }
        }
    }


    // string that contains the help message
    const string help = @"Chirp.CLI.

    Usage:
      Chirp.CLI interactive
      Chirp.CLI read
      Chirp.CLI chirp <text>
      Chirp.CLI (-h | --help)
      Chirp.CLI --version

    Options:
      -h --help     Show this screen.
      --version     Show version.

    ";

    // 3 methods from DocoptNet docs
    // https://docopt.github.io/docopt.net/dev/#api
    static int ShowHelp(string help)
    {
        Console.WriteLine(help);
        return 0;
    }

    static int ShowVersion(string version)
    {
        Console.WriteLine(version);
        return 0;
    }

    static int OnError(string usage)
    {
        Console.WriteLine(usage);
        return 1;
    }

    // Method to activate diffrent parts of the program
    static int Run(IDictionary<string, ArgValue> arguments)
    {
        bool chirpbool = false;
        foreach (var (key, value) in arguments)
        {
            if (key == "interactive" && ((bool)value))
                interactive();
            if (key == "read" && ((bool)value))
                Read();
            if (key == "chirp" && ((bool)value))
                chirpbool = true;
            if (key == "<text>" && chirpbool)
                Chirp(((string)value));
            //Console.WriteLine("{0} = {1}", key, value);
        }

        return 0;
    }

    public static int Main(string[] args)
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
