string command = args[0];

bool isInteger(string x) {
    if(x.Length == 0) return false;
    return x.Select(x => Char.IsDigit(x)).Aggregate((x, y) => x && y);
}

if (command == "chirp") {

    if (args.Length < 2) {
        Console.WriteLine("Please specify a message to chirp!");
        return;
    }
    string name = Environment.UserName;
    long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

    string message = args[1];
    int messageLength = message.Length + 2;

    File.AppendAllText("./chirp_cli_db.csv", name + "," + messageLength + ",\"" + message + "\"," + timestamp + Environment.NewLine);
    Console.WriteLine(name + " @ " + timestamp + ": " + message);
}
else if (command == "read")
{
    var lines = File.ReadLines("./chirp_cli_db.csv");
    foreach (var currLine in lines.Skip(1))
    {
        var parts = currLine.Split(",", 3);

        if(parts.Length != 3 || !isInteger(parts[1])) {
            Console.WriteLine("Database file is incorrectly formatted");
            return;
        }

        int messageLength = int.Parse(parts[1]);

        // +3 to skip the two commas before and after msg len and the quote
        int messageBeginIndex = parts[0].Length + parts[1].Length + 3;
        int messageEndIndex = messageBeginIndex + messageLength - 1;

        string author = parts[0];

        string message = currLine.Substring(messageBeginIndex, messageLength - 2);
        string timestamp = currLine.Substring(messageEndIndex+1);

        if(!isInteger(timestamp)) {
            Console.WriteLine("Database file is incorrectly formatted");
            return;
        }

        DateTimeOffset actualTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(timestamp)).ToLocalTime();
        Console.WriteLine("Author: {0}, Message: {1}, Timestamp: {2}", author, message, actualTime);

        //        string author = parts[0];
        //        string message = parts[1];
        //        DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds(int.Parse(parts[2]));
        //
        //        Console.WriteLine(author + " @ " + timestamp + ": " + message);
    }
}