using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.Models;

public interface IVersioned
{
    int Version { get; set; }
}

public class Cheep : IVersioned
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime Timestamp { get; set; }
    public Author? Author { get; set; }
    public int AuthorId { get; set; }
    public int Version { get; set; }   
}