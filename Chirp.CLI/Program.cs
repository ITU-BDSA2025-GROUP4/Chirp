
using Utils;
using System.Collections.Generic;
using DocoptNet;

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

    const string chirpDbPath = "Chirp.CLI/chirp_cli_db.csv";
    static void read()
    {
        var cheeps = new List<Cheep>();
        
        //var lines = File.ReadLines("<path-to-db-file>");
        var lines = File.ReadLines(chirpDbPath);
        foreach (var currLine in lines.Skip(1))
        {
            var parts = currLine.Split(",", 3);

            if (parts.Length != 3 || !StringUtils.IsInteger(parts[1]))
            {
                Console.WriteLine("Database file is incorrectly formatted");
                Logger.get.LogWarn(String.Format("Invalid line in database: '{0}'", currLine));
                return;
            }

            string author = parts[0];
            string timestamp = parts[1];
            string message = parts[2];

            Cheep cheep = new(author,  message, long.Parse(timestamp));
            cheeps.Add(cheep);
        }
        
        UserInterface.PrintCheeps(cheeps);
    }

    static void chirp(string message)
    {
        string name = Environment.UserName;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        //File.AppendAllText("<path-to-db-file>", name + "," + timestamp +  ",\"" + message + "\"" + Environment.NewLine);
        File.AppendAllText(chirpDbPath, name + "," + timestamp +  ",\"" + message + "\"" + Environment.NewLine);
    }

    static void helpfunc()
    {
        Console.WriteLine("Commands:\nchirp [MESSAGE] | Chirps a message\nread | Displays all chirps\n? | Displays this menu\nexit | Exits Chirp.CLI\n");
    }

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

    // 8 lines from DocoptNet docs
    static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
    static int ShowVersion(string version) { Console.WriteLine(version); return 0; }
    static int OnError(string usage) { Console.WriteLine(usage); return 1; }
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

    static void start(IDictionary<string, ArgValue> result) {
        /*
        if (result.Item[interactive] == true) {
            interactive();
        }
        if (result.Item[read] == true) {
            read();
        }
        if (result.Item[chirp] == true) {
            chirp(result.Item[text]);
        }*/ 
        Console.WriteLine("start");
    }

    static int Main(string[] args)
    {
        // Uncomment the line below in order to disable all logging
//        Logger.get.Disable();
        /*
        if (args.Length == 0)
        {
            interactive();
        }
        else
        {
            batch(args);
        }
        */
        //
        /*
        return Docopt.CreateParser(help)
                     .WithVersion("Naval Fate 2.0")
                     .Parse(args)
                     .Match(Run,
                            result => ShowHelp(result.Help),
                            result => ShowVersion(result.Version),
                            result => OnError(result.Usage));
        
        */
        return Docopt.CreateParser(help)
                    .WithVersion("Chirp.CLI 0.2")
                    .Parse(args)
                    .Match(Run,
                            result => ShowHelp(result.Help),
                            result => ShowVersion(result.Version),
                            result => OnError(result.Usage)
                            );

//        chirpExit(0);
    }

}