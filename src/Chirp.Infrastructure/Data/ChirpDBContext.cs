using Microsoft.EntityFrameworkCore;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Data;

public class ChirpDbContext : DbContext
{
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) {}

    public DbSet<Cheep> Cheeps => Set<Cheep>();
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnModelCreating(ModelBuilder builder) {
        { // Author
            builder.Entity<Author>()
                .HasIndex(e => new {e.Name, e.Email});
//            builder.Entity<Author>()
//                .Property(e => e.Email).IsRequired();
//            builder.Entity<Author>()
//                .Property(e => e.Name).IsRequired();
//            builder.Entity<Author>()
//                .Property(e => e.Id).IsRequired();
        }
        
        builder.Entity<Cheep>()
            .Property<byte[]>("ETag")
            .HasColumnName("ETag")
            .IsConcurrencyToken();

        { // Cheep
//            builder.Entity<Cheep>()
//                .Property(e => e.Author).IsRequired();
//            builder.Entity<Cheep>()
//                .Property(e => e.Text).IsRequired();
//            builder.Entity<Cheep>()
//                .Property(e => e.Timestamp).IsRequired();


        }
    }
}