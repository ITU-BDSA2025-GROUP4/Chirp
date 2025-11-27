using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Entities;

public class Reply
{
    public int Id { get; set; }

    [Required]
    public Cheep Cheep { get; set; } = null!;
    [ForeignKey("Cheep")]
    public int CheepId { get; set; }

    [Required]
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }

    [Required]
    public Author Author { get; set; }
    [ForeignKey("Author")]
    public int AuthorId { get; set; }
}