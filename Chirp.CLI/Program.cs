
using System.Collections.Generic;
using DocoptNet;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Utils;

// Todo: Fix this, for Now I just Assumed this was the format, for the UI class :)
public record Cheep(string Author, string Message, long Timestamp);
public static class UserInterface
{
    private const string timeFormat = "dd/MM/yy HH:mm:ss";
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach(Cheep cheep in cheeps)
        {
            DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
            
            Console.WriteLine(cheep.Author + " @ " + timestamp.ToString(timeFormat) + ": " + cheep.Message);
        }
    }
}

static class ChirpMain
{
    static void chirpExit(int statusCode) {
        Logger.get.Dispose();
        System.Environment.Exit(statusCode);
    }

    const string chirpDbPath = "Chirp.CLI/chirp_cli_db.csv"; // <- temp sulution to db pls fix later
    static void read()
    {
        var csvconfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            MissingFieldFound = null
        };

        try
        {
            using (var reader = new StreamReader("chirp_cli_db.csv"))
            using (var csv = new CsvReader(reader, csvconfig))
            {
                var cheeps = csv.GetRecords<Cheep>().ToList();
                UserInterface.PrintCheeps(cheeps);
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File to DataBase not found");
        }
        catch (Exception e)
        {
            Logger.get.LogWarn(String.Format("Error parsing database: '{0}'", e.ToString()));
            Console.WriteLine(e);
            throw;
        }
    }
    
    static void chirp(string message)
    {
        string name = Environment.UserName;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        using (var stream = new StreamWriter("chirp_cli_db.csv", append: true))
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = stream.BaseStream.Length == 0,
                ShouldQuote = args => args.Field.ToString() == message, // Correct way to quote
            };

            using (var csv = new CsvWriter(stream, csvConfig))
            {
                if (stream.BaseStream.Length == 0)
                {
                    csv.WriteHeader<Cheep>();
                    csv.NextRecord();
                }

                csv.WriteRecord(new Cheep(name, message, timestamp));
                csv.NextRecord();
            }
        }
    }
    static void helpfunc()
    {
        Console.WriteLine("Commands:\nchirp [MESSAGE] | Chirps a message\nread | Displays all chirps\n? | Displays this menu\nexit | Exits Chirp.CLI\n");
    }

    // @Obselete
    // Used by interactive
    static void batch(string[] args)
    {
        string command = args[0];

        switch (command)
        {
            case "read":
                read();
                break;
            case "chirp":
                if (args.Length < 2)
                {
                    Console.WriteLine("Chirp requires a message");
                    return;
                }
                chirp(args[1]);
                break;
            case "?":
                helpfunc();
                break;
            case "exit":
                chirpExit(0);
                break;
            default:
                Logger.get.Log(String.Format("User wrote unknown command: {0}", command));
                Console.WriteLine("Unknown command {0}, use '?' for help", command);
                break;
        }
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

            batch(tokens);
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
    static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
    static int ShowVersion(string version) { Console.WriteLine(version); return 0; }
    static int OnError(string usage) { Console.WriteLine(usage); return 1; }

    // Method to activate diffrent parts of the program
    static int Run(IDictionary<string, ArgValue> arguments)
    {
        bool chirpbool = false;
        foreach (var (key, value) in arguments){
            if (key == "interactive" && ((bool)value))
                interactive();
            if (key == "read" && ((bool)value)) 
                read();
            if (key == "chirp" && ((bool)value))
                chirpbool = true;
            if (key == "<text>" && chirpbool)
                chirp(((string)value));
            //Console.WriteLine("{0} = {1}", key, value);
        }
        return 0;
    }

    static int Main(string[] args)
    {
        // Uncomment the line below in order to disable all logging
//        Logger.get.Disable();
        Docopt.CreateParser(help)
                    .WithVersion("Chirp.CLI 0.2")
                    .Parse(args)
                    .Match(Run,
                            result => ShowHelp(result.Help),
                            result => ShowVersion(result.Version),
                            result => OnError(result.Usage)
                            );

        chirpExit(0);
        return 1;
    }

}