namespace Chirp.Core.Entities;

public class Cheep
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public required Author Author { get; set; }
    public required string AuthorId { get; set; }
}