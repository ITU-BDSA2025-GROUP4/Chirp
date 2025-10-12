using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Chirp.Razor.Data;

public sealed class ETagConcurrencyInterceptor : SaveChangesInterceptor
{
    private static byte[] NewToken()
    {
        var buf = new byte[16];
        RandomNumberGenerator.Fill(buf);
        return buf;
    }

    private static void RefreshEtags(DbContext ctx)
    {
        foreach (var e in ctx.ChangeTracker.Entries())
        {
            var p = e.Properties.FirstOrDefault(p => p.Metadata.Name == "ETag");
            if (p is null) continue;

            if (e.State == EntityState.Added && p.CurrentValue is null || e.State == EntityState.Modified)
                p.CurrentValue = NewToken();
        }
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null) RefreshEtags(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        if (eventData.Context is not null) RefreshEtags(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }
}