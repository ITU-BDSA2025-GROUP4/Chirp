namespace Chirp.Razor.Models;

using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), nameof(Name))]
public class Author
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<Cheep>? Cheeps { get; set; }
}