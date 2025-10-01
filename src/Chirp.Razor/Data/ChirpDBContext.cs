using Microsoft.EntityFrameworkCore;
using Chirp.Razor.Models;

namespace Chirp.Razor.Data;

public class ChirpDbContext : DbContext
{
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Cheep> Cheeps => Set<Cheep>();
    public DbSet<Author> Authors => Set<Author>();
}