namespace Chirp.Types;

public class Cheep {

    public int    Id        { get; set; }
    public string Author    { get; set; }
    public string Message   { get; set; }
    public long   Timestamp { get; set; }

    // Need both constructors, otherwise Entity Core can't properly math the type with the SQL table
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

    public override String ToString()
    {
        return String.Format("Cheep {{ Author: {0}, Message: {1}, Timestamp {2} }}", Author, Message, Timestamp);
    }

    // Used in unit tests
    public bool Equals(Cheep? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Timestamp == other.Timestamp && Author == other.Author && Message == other.Message; 
    }
}