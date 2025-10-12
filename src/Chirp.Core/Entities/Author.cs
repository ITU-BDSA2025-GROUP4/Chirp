namespace Chirp.Core.Entities;

// Annotations are set inside Chirp.Infrastructure/Data/ChirpDBContext.cs
public class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required List<Cheep> Cheeps { get; set; }
}