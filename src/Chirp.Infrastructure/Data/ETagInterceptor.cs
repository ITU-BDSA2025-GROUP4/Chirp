using Chirp.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chirp.Infrastructure.Data;

public sealed class ETagInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private static void Stamp(DbContext? ctx)
    {
        if (ctx is null) return;

        foreach (var entry in ctx.ChangeTracker.Entries<Cheep>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Property("ETag").CurrentValue = Guid.NewGuid().ToByteArray();
        }
    }
}
