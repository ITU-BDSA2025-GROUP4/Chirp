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

    public async Task<Optional<AuthorDTO>> FindAuthorByName(string name)
    {
        return await _context.Authors
            .Where(a => a.Name == name)
            .Select(x => Optional.Of<AuthorDTO>(new AuthorDTO(x.Id, x.Name, x.Email)))
            .DefaultIfEmpty(Optional.Empty<AuthorDTO>())
            .FirstOrDefaultAsync();
    }

    public async Task<Optional<AuthorDTO>> FindAuthorByEmail(string email)
    {
        return await _context.Authors
            .Where(a => a.Name == email)
            .Select(x => Optional.Of<AuthorDTO>(new AuthorDTO(x.Id, x.Name, x.Email)))
            .DefaultIfEmpty(Optional.Empty<AuthorDTO>())
            .FirstOrDefaultAsync();
    }

    public async void AddAuthor(AuthorDTO author)
    {
        var newAuthor = new Author();
        newAuthor.Name = author.Name;
        newAuthor.Email = author.Email;

        await _context.Authors.AddAsync(newAuthor);
    }

    public async Task<List<CheepDTO>> ReadAll()
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

    public async Task<List<CheepDTO>> Read(int pageNumber, int pageSize)
    {
        return await _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CheepDTO(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
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
            .Select(c => new CheepDTO(c.Author.Name, c.Text, Utils.TimestampUtils.DateTimeTimeStampToDateTimeString(c.Timestamp)))
            .ToListAsync();
    }

}