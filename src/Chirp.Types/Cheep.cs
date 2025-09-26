namespace Chirp.Types;

using SimpleDB;
using System.Data.SQLite;

// Todo: Fix this, for Now I just Assumed this was the format, for the UI class :)
public record Cheep(string Author, string Message, long Timestamp) : ISQLType<Cheep> 
{
    public Cheep Extract(SQLiteDataReader reader)
    {
        reader.GetValues();

        string author = reader.GetString(1);
        string message = reader.GetString(2);
        long timestamp = reader.GetInt64(3);

        return new Cheep(author, message, timestamp);
    }
}
