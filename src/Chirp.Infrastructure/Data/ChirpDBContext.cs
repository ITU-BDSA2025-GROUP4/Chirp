using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Data;

public class ChirpDbContext : DbContext
{
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) { }

    public DbSet<Cheep> Cheeps => Set<Cheep>();
    public DbSet<Author> Authors => Set<Author>();
}