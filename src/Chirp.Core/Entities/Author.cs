using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Entities;

// Annotations are set inside Chirp.Infrastructure/Data/ChirpDBContext.cs
public class Author : IdentityUser<int>
{
    public string Name // For backwards combability. TODO: Remove later 
    {
        get { return UserName!; }
        set { UserName = value; }
    }

    public List<Cheep> Cheeps { get; set; } = [];
    public List<Follow> Following { get; set; } = [];
    public List<Follow> Followers { get; set; } = [];

}