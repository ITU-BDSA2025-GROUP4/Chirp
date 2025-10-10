namespace Chirp.Core.Entities;

public class Cheep
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public Author Author { get; set; }
    public int AuthorId { get; set; }
}