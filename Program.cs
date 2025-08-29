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
else if (command == "read")
{
    var lines = File.ReadLines("/chirp_cli_db.csv");
    foreach (var currLine in lines.Skip(1))
    {
        var parts = currLine.Split(",");
        foreach (var part in parts)
        {
            Console.WriteLine(part);
        }
        //        string author = parts[0];
        //        string message = parts[1];
        //        DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds(int.Parse(parts[2]));
        //
        //        Console.WriteLine(author + " @ " + timestamp + ": " + message);
    }

}