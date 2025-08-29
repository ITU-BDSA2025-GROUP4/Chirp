string command = args[0];

if (command == "chirp")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Please specify a message to chirp!");
        return;
    }
    string name = Environment.UserName;
    long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

    string message = args[1];
    File.AppendAllText("./chirp_cli_db.csv", name + ",\"" + message + "\"," + timestamp + Environment.NewLine);
    Console.WriteLine(name + " @ " + timestamp + ": " + message);
}