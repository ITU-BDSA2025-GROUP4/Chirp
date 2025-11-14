using Chirp.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chirp.Infrastructure.Data.Configurations;

public class CheepConfiguration : IEntityTypeConfiguration<Cheep>
{
    private const int TextMaxLength = 160;
    
    public void Configure(EntityTypeBuilder<Cheep> e)
    {
        e.Property(x => x.Text).HasMaxLength(TextMaxLength);
        
        e.ToTable(t => t.HasCheckConstraint(
            "ck_cheep_text_length",
            $"length(text) <= {TextMaxLength}"));
        
        e.Property(x => x.Timestamp)
            .IsRequired();
        
        e.HasOne(x => x.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey(x => x.AuthorId)
            .IsRequired();
        
        e.Property<byte[]>("ETag")
            .HasColumnName("ETag")
            .IsConcurrencyToken();
    }
}