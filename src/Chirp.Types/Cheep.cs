namespace Chirp.Types;

using SimpleDB;

using System;
using System.Data;
using System.Data.SQLite;

// Todo: Fix this, for Now I just Assumed this was the format, for the UI class :)
public record Cheep(string Author, string Message, long Timestamp) : ISQLType<Cheep>
{
    public static Cheep Extract(SQLiteDataReader reader)
    {
        int id = reader.GetInt32(0);
        string author = reader.GetString(1);
        string message = reader.GetString(2);
        long timestamp = reader.GetInt64(3);

        return new Cheep(author, message, timestamp);
    }

    public static string ExpectedTable()
    {
        return "Cheep";
    }

    public void Insert(SQLiteParameterCollection parameters)
    {
        parameters.Add("@param1", DbType.String);
        parameters["@param1"].Value = Author;

        parameters.Add("@param2", DbType.String);
        parameters["@param2"].Value = Message;

        parameters.Add("@param3", DbType.Int64);
        parameters["@param3"].Value = Timestamp;
    }

}