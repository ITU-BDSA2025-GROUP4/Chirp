using Chirp.Razor.Models;
using Chirp.Razor.Data;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

    public async Task<CheepViewModel> UpdateAsync(CheepUpdateDto dto)
    {
        var now = TimestampUtils.DateTimeTimeStampToDateTimeString(DateTime.UtcNow);

        var rows = await _context.Cheeps
            .Where(c => c.Id == dto.Id && c.Version == dto.Version) 
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Text, _ => dto.Text)
                .SetProperty(c => c.Timestamp, _ => DateTime.UtcNow)
                .SetProperty(c => c.Version,  c => c.Version + 1));

        if (rows == 0) throw new KeyNotFoundException();
        
        return new CheepViewModel(dto.Author, dto.Text, now);
    }

    public async Task<CheepViewModel> Delete(Cheep cheep)
    {
        _context.Remove(cheep);
        await _context.SaveChangesAsync();
        
        return new CheepViewModel(cheep.Author.Name, cheep.Text, TimestampUtils.DateTimeTimeStampToDateTimeString(cheep.Timestamp));

    }
    

}
