using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Entities;

// Annotations are set inside Chirp.Infrastructure/Data/ChirpDBContext.cs
public class Author : IdentityUser
{
    //public int Id { get; set; }
    //public required string Name { get; set; }
    //public required string Email { get; set; }
    /// <summary>
    /// Deprecated use UserName insted<br>
    /// Only Exist for backwards combability
    /// </summary>
    public string Name // For backwards combability. TODO: Remove later 
    {
        get { return UserName; }
        set { UserName = value; }
    }
    public required List<Cheep> Cheeps { get; set; }
}