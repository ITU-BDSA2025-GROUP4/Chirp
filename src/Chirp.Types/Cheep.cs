namespace Chirp.Types;

using Microsoft.EntityFrameworkCore;

using SimpleDB;

using System.Data;
using System.Data.SQLite;

public class Cheep : ISQLType<Cheep> {

    public int Id { get; set; }
    public string Author    { get; set; }
    public string Message   { get; set; }
    public long   Timestamp { get; set; }

    protected Cheep(int Id, string Author, string Message, long Timestamp)
    {
        this.Author = Author;
        this.Message = Message;
        this.Timestamp = Timestamp;
        this.Id = Id;
    }

    public Cheep(string Author, string Message, long Timestamp)
    {
        this.Author = Author;
        this.Message = Message;
        this.Timestamp = Timestamp;
    }

    public static string TableName()
    {
        return "Cheep";
    }

    public override String ToString()
    {
        return String.Format("Cheep {{ Author: {0}, Message: {1}, Timestamp {2} }}", Author, Message, Timestamp);
    }

    public bool Equals(Cheep? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Timestamp == other.Timestamp && Author == other.Author && Message == other.Message; 
    }
}
