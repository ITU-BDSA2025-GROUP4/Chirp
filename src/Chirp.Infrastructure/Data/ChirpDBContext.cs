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
    public DbSet<Follow> Follows => Set<Follow>();

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

            // Follow
            builder.Entity<Follow>()
                .HasKey(f => new { f.FollowerFK, f.FolloweeFK });

            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(a => a.Following)
                .HasForeignKey(f => f.FollowerFK);

            builder.Entity<Follow>()
                .HasOne(f => f.Followee)
                .WithMany(a => a.Followers)
                .HasForeignKey(f => f.FolloweeFK);

            builder.ApplyConfigurationsFromAssembly(typeof(ChirpDbContext).Assembly);
        }
    }
}