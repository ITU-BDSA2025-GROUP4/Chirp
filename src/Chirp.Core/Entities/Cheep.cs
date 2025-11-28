using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Entities;

public class Cheep
{
    public int Id { get; set; }

    [Required]
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }

    [Required]
    public Author Author { get; set; }
    public int AuthorId { get; set; }

    public List<Reply> Replies { get; set; } = [];

    public List<ReCheep> ReCheeps { get; set; } = [];
}
