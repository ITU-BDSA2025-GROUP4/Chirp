
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


class ChirpMain
{
    static void chirpExit(int statusCode) {
        Logger.get.Dispose();
        System.Environment.Exit(statusCode);
    }

    static void read()
    {
        var cheeps = new List<Cheep>();
        
        var lines = File.ReadLines("<path-to-db-file>");
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

        File.AppendAllText("<path-to-db-file>", name + "," + timestamp +  ",\"" + message + "\"" + Environment.NewLine);
    }

    static void help()
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
                help();
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
        help();

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

    static void Main(string[] args)
    {
        // Uncomment the line below in order to disable all logging
//        Logger.get.Disable();
        if (args.Length == 0)
        {
            interactive();
        }
        else
        {
            batch(args);
        }

        chirpExit(0);
    }

}
