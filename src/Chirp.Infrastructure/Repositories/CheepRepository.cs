using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

using Chirp.Core.Application;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Utils;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _context;

    public CheepRepository(ChirpDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Add(CheepDTO cheep) {
        // TODO: Use FirstOrDefaultAsync() instead of ToListAsync()
        // But remember to correctly set the default value, otherwise this will blow up
        var authors = await _context.Authors.Where(x => x.Name == cheep.Author).ToListAsync();

        if(authors.Count() == 0) {
            return false;
        }

        var author = authors.First();

        var newCheep = new Cheep() {
            Author = author,
            AuthorId = author.Id,
            Text = cheep.Text,
            Timestamp = TimestampUtils.DateTimeStringToDateTimeTimeStamp(cheep.Timestamp),
        };

        await _context.Cheeps.AddAsync(newCheep);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CheepDTO>> ReadAll()
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Select(c => new CheepDTO(c.Author.Name, c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> ReadAsync(int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> QueryAsync(Expression<Func<Cheep, bool>> condition,
        int pageNumber,
        int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .Where(condition)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<AppResult<CheepDTO>> CreateAsync(CreateCheepRequest dto)
    {
        var cheep = new Cheep
        {
            Text = dto.Text, AuthorId = dto.AuthorId, Timestamp = DateTime.UtcNow
        };

        _context.Cheeps.Add(cheep);
        _context.Entry(cheep).Property("ETag").CurrentValue = ETagUtils.NewValue();

        await _context.SaveChangesAsync();

        var dtoOut = await ProjectCheepDtoAsync(cheep.Id);

        var etagBytes = (byte[])_context.Entry(cheep).Property("ETag").CurrentValue!;
        var etag = ETagUtils.ToBase64(etagBytes);

        return AppResult<CheepDTO>.Created(dtoOut, etag);
    }

    public async Task<AppResult<CheepDTO>> UpdateAsync(UpdateCheepRequest dto)
    {
        var e = new Cheep { Id = dto.CheepId };
        _context.Attach(e);

        _context.Entry(e).Property(x => x.Text).CurrentValue = dto.Text;
        _context.Entry(e).Property(x => x.Text).IsModified = true;

        _context.Entry(e).Property("ETag").OriginalValue = Convert.FromBase64String(dto.ETag);
        _context.Entry(e).Property("ETag").CurrentValue = ETagUtils.NewValue(); 

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return AppResult<CheepDTO>.Conflict("Cheep was modified by someone else.");
        }

        var newEtag = ETagUtils.ToBase64(
            (byte[])_context.Entry(e).Property("ETag").CurrentValue!);

        var dtoOut = await ProjectCheepDtoAsync(dto.CheepId);

        return AppResult<CheepDTO>.Ok(dtoOut, newEtag);
    }


// Should perhaps include cascading deletion down the line for likes and such
    public async Task<AppResult> DeleteAsync(DeleteCheepRequest dto)
    {
        var cheep = await _context.Cheeps.FindAsync(dto.CheepId);
        if (cheep is null)
            return AppResult.NotFound("Cheep not found.");

        // TODO: authorization (compare dto.RequesterId with cheep.AuthorId)

        var etagResult = TryStampDeleteEtag(cheep, dto.ETag);
        if (etagResult is not null)
            return etagResult;

        _context.Cheeps.Remove(cheep);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return AppResult.Conflict("Cheep was modified or removed by someone else.");
        }

        return AppResult.NoContent();
    }

private AppResult? TryStampDeleteEtag(Cheep cheep, string? etag)
{
    if (string.IsNullOrWhiteSpace(etag))
        return null;

    var entry = _context.Entry(cheep);
    var currentEtag = Convert.ToBase64String((byte[])entry.Property("ETag").CurrentValue!);

    if (!ETagUtils.Equals(currentEtag, etag))
        return AppResult.Conflict("Cheep was modified by someone else.");

    entry.Property("ETag").OriginalValue = Convert.FromBase64String(etag);
    return null;
}



    private Task<CheepDTO> ProjectCheepDtoAsync(int id) =>
        _context.Cheeps
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CheepDTO(
                c.Author.Name,
                c.Text,
                TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .SingleAsync();
}