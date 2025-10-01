using Chirp.Razor.Models;
using Chirp.Razor.Data;

using Microsoft.EntityFrameworkCore; // for ToListAsync()
using System.Linq.Expressions;

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
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepViewModel>> Read(int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
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
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepViewModel(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }
}
