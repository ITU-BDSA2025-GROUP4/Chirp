using Chirp.Razor.Models;
using Chirp.Razor.Data;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Chirp.Razor.Application.Contracts;

using Utils;

namespace Chirp.Razor.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _context;

    public CheepRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public async Task<List<CheepViewModel>> ReadAll()
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepViewModel>> Read(int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepViewModel>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .Where(condition)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<CheepViewModel> Create(Cheep cheep)
    {
        _context.Add(cheep);
        await _context.SaveChangesAsync();

        return new CheepViewModel(cheep.Author.Name, cheep.Text, TimestampUtils.DateTimeTimeStampToDateTimeString(cheep.Timestamp));
    }

    public Task<CheepViewModel> Update(Cheep cheep)
    {
        throw new NotImplementedException();
    }

    // public async Task<CheepViewModel> UpdateAsync(CheepUpdateDto update, CancellationToken ct = default)
    // {
    //     // Stub to avoid multi db accesses
    //     var entity = new Cheep { Id = update.Id, Text = update.Text };
    //
    //     _context.Attach(entity);
    //     _context.Entry(entity).State = EntityState.Modified;
    //
    //     // Communicate the original token to ef, so it goes into WHERE
    //     var original = Convert.FromBase64String(update.ETagBase64);
    //     _context.Entry(entity).Property("ETag").OriginalValue = original;
    //
    //     try
    //     {
    //         await _context.SaveChangesAsync(ct);
    //
    //         // Interceptor (or your SaveChanges override) has set the new token
    //         var newToken = (byte[])_context.Entry(entity).Property("ETag").CurrentValue!;
    //         return new CheepViewModel(
    //             Outcome: UpdateOutcome.Success,
    //             Entity: entity,
    //             NewETagBase64: Convert.ToBase64String(newToken)
    //         );
    //     }
    //     catch (DbUpdateConcurrencyException)
    //     {
    //         // Distinguish “deleted” vs “updated by someone else”
    //         var stillExists = await _context.Set<Cheep>()
    //             .AsNoTracking()
    //             .AnyAsync(c => c.Id == update.Id, ct);
    //
    //         return stillExists
    //             ? new CheepViewModel(UpdateOutcome.Conflict)
    //             : new CheepViewModel(UpdateOutcome.NotFound);
    //     }
    // }

    public async Task<CheepViewModel> Delete(Cheep cheep)
    {
        _context.Remove(cheep);
        await _context.SaveChangesAsync();
        
        return new CheepViewModel(cheep.Author.Name, cheep.Text, TimestampUtils.DateTimeTimeStampToDateTimeString(cheep.Timestamp));

    }
    

}
