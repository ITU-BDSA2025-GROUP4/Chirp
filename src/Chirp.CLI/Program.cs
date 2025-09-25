using System.Net.Http.Json;

using Chirp.Types;

using DocoptNet;

using SimpleDB;

using Utils;

using Version = MetaData.Version;

namespace Chirp.Cli;

public static class UserInterface
{
    private const string timeFormat = "dd/MM/yy HH:mm:ss";

    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps)
        {
            DateTimeOffset timestamp =
                DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();

            Console.WriteLine(cheep.Author + " @ " + timestamp.ToString(timeFormat) + ": " +
                              cheep.Message);
        }
    }
}

internal static class ChirpMain
{
    // string that contains the help message
    private const string help = @"Chirp.CLI.

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

    private static readonly CsvDatabase<Cheep> Db = new(Path.Combine(AppContext.BaseDirectory,
        "Resources", "Data", "chirp_cli_db.csv"));

    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5000")
    };

    private static void ChirpExit(int statusCode)
    {
        Logger.get.Dispose();
        Db.Write();
        Environment.Exit(statusCode);
    }

    private static void Read()
    {
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
        string name = Environment.UserName;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        string url = string.Format("/cheep?author={0}&message={1}&timestamp={2}", name, message,
            timestamp);

        // TODO: This gives response 'Cheep'ed' if successful, we should check this
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
    private static void batch(string[] args)
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
                    return;
                }

                Chirp(args[1]);
                break;
            case "?":
                helpfunc();
                break;
            case "exit":
                ChirpExit(0);
                break;
            default:
                Logger.get.Log(string.Format("User wrote unknown command: {0}", command));
                Console.WriteLine("Unknown command {0}, use '?' for help", command);
                break;
        }
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

            batch(tokens);
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
        return 1;
    }
}