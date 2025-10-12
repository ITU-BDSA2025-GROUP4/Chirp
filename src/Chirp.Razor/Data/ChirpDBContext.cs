using Microsoft.EntityFrameworkCore;
using Chirp.Razor.Models;

namespace Chirp.Razor.Data;

public class ChirpDbContext : DbContext
{
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) { }

    public DbSet<Cheep> Cheeps => Set<Cheep>();
    public DbSet<Author> Authors => Set<Author>();
    
    protected override void OnModelCreating(ModelBuilder mb)
    {
        foreach (var et in mb.Model.GetEntityTypes())
        {
            var clr = et.ClrType;
            mb.Entity(clr).Property<byte[]>("ETag")
                .IsRequired()
                .IsConcurrencyToken()
                .HasColumnType("BLOB"); // SQLite binary column
        }
    }
}