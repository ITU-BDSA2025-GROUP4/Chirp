using Chirp.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chirp.Infrastructure.Data;

public sealed class ETagInterceptor : SaveChangesInterceptor
{
    // Updates ETags before a synchronous SaveChanges call
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    // Updates ETags before an asynchronous SaveChanges call
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    // Assigns a new ETag to added or modified Cheep entities
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