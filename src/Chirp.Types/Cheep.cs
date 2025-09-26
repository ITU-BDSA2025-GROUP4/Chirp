namespace Chirp.Types;

using SimpleDB;

using System;
using System.Data;
using System.Data.SQLite;

// Todo: Fix this, for Now I just Assumed this was the format, for the UI class :)
public record Cheep(string Author, string Message, long Timestamp) 
{
    public static Test Extract(SQLiteDataReader reader)
    {
        int id = reader.GetInt32(0);
        string author = reader.GetString(1);
        string message = reader.GetString(2);
        long timestamp = reader.GetInt64(3);

        return new Test(author, message, timestamp);
    }

    public static string ExpectedTable()
    {
        return "Cheep";
    }

}