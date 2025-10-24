using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Utils;

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
}