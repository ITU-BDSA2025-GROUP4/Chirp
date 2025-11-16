
using System.ComponentModel.DataAnnotations.Schema;

namespace Chirp.Core.Entities;

public class Follow
{
    public Author Follower { get; set; } = null!;
    [ForeignKey("Follower")]
    public int FollowerFK { get; set; }

    public Author Followee { get; set; } = null!;
    [ForeignKey("Followee")]
    public int FolloweeFK { get; set; }
}