
using Utils;

abstract public class Chirp
{
    // Theese properties are set to init since they are immutable (they depend on the state of the world when the Chirp is created)
    public string Username { get; private init; }
    public long Timestamp { get; private init; }

    public Chirp(string username, long timestamp)
    {
        Username = username;
        Timestamp = timestamp;
    }

    public override string ToString()
    {
        String format = "dd/MM/yy HH:mm:ss";
        System.Globalization.CultureInfo locale = System.Globalization.CultureInfo.CurrentCulture;
        DateTimeOffset actualTime = DateTimeOffset.FromUnixTimeSeconds(Timestamp).ToLocalTime();

        return Username + " @ " + actualTime.ToString(format, locale);
    }
}

public class MessageChirp : Chirp
{
    public string Message { get; private set; }
    public MessageChirp(string username, long timestamp, string message) : base(username, timestamp)
    {
        Message = message;
    }

    public override string ToString()
    {

        return base.ToString() + ": " + Message;
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

        var lines = File.ReadLines("./chirp_cli_db.csv");
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

            MessageChirp messageChirp = new MessageChirp(author, long.Parse(timestamp), message);
            Console.WriteLine(messageChirp);
        }
    }

    static void chirp(string message)
    {
        string name = Environment.UserName;
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        File.AppendAllText("./chirp_cli_db.csv", name + "," + timestamp +  ",\"" + message + "\"" + Environment.NewLine);
        Console.WriteLine(name + " @ " + timestamp + ": " + message);
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