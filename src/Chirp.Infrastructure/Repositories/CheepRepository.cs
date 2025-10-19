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

    public async Task<List<CheepDTO>> ReadAll()
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> Read(int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> Query(Expression<Func<Cheep, bool>> condition, int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .Where(condition)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }
    
    public async Task<AppResult<CheepDTO>> Create(CheepCreateDto dto)
    {
 
        var cheep = new Cheep
        {
            Text = dto.Text,
            Author = new Author{Name = dto.Author},
            Timestamp = DateTime.UtcNow
        };

        _context.Cheeps.Add(cheep);
        _context.Entry(cheep).Property("ETag").CurrentValue = ETagUtils.NewValue();

        await _context.SaveChangesAsync();

        var etagBytes = (byte[])_context.Entry(cheep).Property("ETag").CurrentValue!;
        var etag = ETagUtils.ToBase64(etagBytes);

      
        var dtoOut = new CheepDTO(
            Author: cheep.Author.Name,
            Text: cheep.Text,
            Timestamp: cheep.Timestamp.ToString("u")
        );

        return AppResult<CheepDTO>.Created(dtoOut, etag);
    }

    public async Task<AppResult<CheepDTO>> Update(CheepUpdateDTO dto)
    {
        var cheep = await _context.Cheeps.FindAsync(dto.Id);
        if (cheep == null) return AppResult<CheepDTO>.NotFound("Cheep not found");

       
        _context.Entry(cheep).Property<byte[]>("ETag").OriginalValue =
            Convert.FromBase64String(dto.ETag);

        
        cheep.Text = dto.Text;
        cheep.Timestamp = DateTime.UtcNow;
        
        try
        {
            await _context.SaveChangesAsync(); 
        }
        catch (DbUpdateConcurrencyException)
        {
            return AppResult<CheepDTO>.Conflict("ETag mismatch");
        }
        
        var resultDto = new CheepDTO(
            cheep.Author.Name,
            cheep.Text,
            TimestampUtils.DateTimeTimeStampToDateTimeString(cheep.Timestamp));


        var newETag = Convert.ToBase64String(
            _context.Entry(cheep).Property<byte[]>("ETag").CurrentValue!);

        return AppResult<CheepDTO>.Ok(resultDto, newETag);

    }
}