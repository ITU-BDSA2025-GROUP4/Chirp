using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Data;

public class ChirpDbContext
    : IdentityDbContext<Author, IdentityRole<int>, int>
{
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) { }

    public DbSet<Cheep> Cheeps => Set<Cheep>();
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        {
            // Author
            builder.Entity<Author>(b =>
            {
                b.Property(a => a.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                b.Property(a => a.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                b.HasIndex(a => a.NormalizedUserName).IsUnique();
                b.HasIndex(a => a.NormalizedEmail).IsUnique();
            });

           builder.ApplyConfigurationsFromAssembly(typeof(ChirpDbContext).Assembly);
        }
    }
}