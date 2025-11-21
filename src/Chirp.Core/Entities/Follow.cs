
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Entities;

// Remaining annotations are set inside Chirp.Infrastructure/Data/ChirpDBContext.cs
public class Follow
{
    public Author Follower { get; set; } = null!;
    [ForeignKey("Follower")]
    public int FollowerFK { get; set; }

    public Author Followee { get; set; } = null!;
    [ForeignKey("Followee")]
    public int FolloweeFK { get; set; }
}